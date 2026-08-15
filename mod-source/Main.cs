using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using HarmonyLib;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Il2Cpp;

[assembly: MelonInfo(typeof(PtBrTranslation.Mod), "PT-BR Translation", PtBrTranslation.Mod.CurrentVersion, "xXSirius")]
[assembly: MelonGame("SmallMill", "PathOfIdle")]

namespace PtBrTranslation
{
    public class Mod : MelonMod
    {
        // Fonte única da versão: também usada no atributo MelonInfo acima
        // (precisa ser const pra valer como argumento de atributo) e na
        // checagem de atualização abaixo, pra nunca ficar dessincronizada.
        public const string CurrentVersion = "1.3.0";

        private const string LatestReleaseApiUrl = "https://api.github.com/repos/xXSirius/pathofidle-ptbr/releases/latest";
        private const string ReleasesPageUrl = "https://github.com/xXSirius/pathofidle-ptbr/releases/latest";

        // Caixa de diálogo nativa do Windows (user32), não a UI do jogo: já
        // tentamos usar o toast interno do jogo (Game.uiMgr.ShowTip) e ele
        // depende de um prefab que só fica pronto num momento imprevisível
        // da inicialização — não deu pra garantir que aparece. MessageBoxW
        // roda numa thread separada pra não travar o jogo enquanto espera o
        // clique, e abre a página da release no navegador padrão se a
        // pessoa responder "Sim".
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBoxW(IntPtr hWnd, string text, string caption, uint type);

        private const uint MB_YESNO = 0x00000004;
        private const uint MB_ICONINFORMATION = 0x00000040;
        private const uint MB_TOPMOST = 0x00040000;
        private const uint MB_SETFOREGROUND = 0x00010000;
        private const int IDYES = 6;

        private static void ShowUpdateNotification(string message)
        {
            var thread = new Thread(() =>
            {
                int result = MessageBoxW(IntPtr.Zero, message, "Tradução PT-BR - Path of Idle",
                    MB_YESNO | MB_ICONINFORMATION | MB_TOPMOST | MB_SETFOREGROUND);
                if (result == IDYES)
                {
                    try { Process.Start(new ProcessStartInfo(ReleasesPageUrl) { UseShellExecute = true }); }
                    catch { /* sem navegador padrão configurado, sem sorte — não é crítico */ }
                }
            })
            {
                IsBackground = true
            };
            thread.Start();
        }

        // GetL10n's "template" parameter is ALWAYS the Chinese key from the
        // TLanguage_MultiLang table, regardless of which language slot the
        // player has selected in-game (l10nIndex only affects what the
        // original method itself returns). So both dictionaries below are
        // keyed by the Chinese template string.
        public static Dictionary<string, string> PtBr = new Dictionary<string, string>();
        public static Dictionary<string, string> EnFallback = new Dictionary<string, string>();

        // Any Chinese key seen at runtime with l10nIndex==0 that has no
        // PT-BR translation gets recorded here (deduped) and flushed to
        // disk on quit, so gaps in coverage surface naturally from normal
        // play instead of requiring manual screen-by-screen review.
        private static readonly HashSet<string> missingKeys = new HashSet<string>();
        private static string missingPath;

        // Guarda "versão|data" do último aviso, pra mostrar a caixa de diálogo
        // no máximo uma vez por dia enquanto a pessoa não atualizar: avisar
        // toda partida incomoda quem não quer atualizar agora, e avisar só uma
        // vez na vida é fácil demais de esquecer. Versão nova avisa na hora,
        // sem esperar o dia virar.
        private static string notifiedVersionPath;

        public override void OnInitializeMelon()
        {
            string dir = MelonEnvironment.UserDataDirectory;
            PtBr = LoadDict(Path.Combine(dir, "ptbr_translation.json"), "PT-BR");
            EnFallback = LoadDict(Path.Combine(dir, "en_fallback.json"), "EN fallback");
            missingPath = Path.Combine(dir, "missing_strings.json");
            notifiedVersionPath = Path.Combine(dir, "last_notified_version.txt");

            // dispara em segundo plano e nunca bloqueia a inicialização do
            // mod — se a rede falhar ou o GitHub estiver fora do ar, o jogo
            // segue normalmente, só sem o aviso de atualização
            _ = CheckForUpdateAsync();
        }

        // Só avisa que existe uma versão nova (log + link pra release), não
        // baixa nem substitui nenhum arquivo sozinho: o .dll de tradução
        // está carregado em memória enquanto o jogo roda, então uma troca
        // seria arriscada, e baixar+aplicar um update sem verificação de
        // integridade dentro do processo do jogo abriria uma porta de
        // ataque desnecessária caso a conta/repositório do GitHub um dia
        // seja comprometido. Atualizar continua sendo: baixar o release e
        // rodar o instalador de novo.
        private async Task CheckForUpdateAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    // Resposta da API do GitHub é a única entrada externa do mod.
                    // Limitar o tamanho impede que uma resposta anormalmente grande
                    // (GitHub comprometido, proxy corporativo hostil) consuma memória
                    // do jogo. O JSON de uma release fica bem abaixo disso.
                    client.MaxResponseContentBufferSize = 512 * 1024;
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("PtBrTranslation-Mod");

                    string json = await client.GetStringAsync(LatestReleaseApiUrl);
                    string tag = (string)JObject.Parse(json)["tag_name"];
                    if (string.IsNullOrEmpty(tag)) return;

                    string remoteVersionText = tag.StartsWith("v", StringComparison.OrdinalIgnoreCase) ? tag.Substring(1) : tag;
                    if (!System.Version.TryParse(remoteVersionText, out var remoteVersion)) return;
                    if (!System.Version.TryParse(CurrentVersion, out var currentVersion)) return;

                    if (remoteVersion > currentVersion)
                    {
                        LoggerInstance.Msg(
                            $"Nova versão da tradução disponível: v{remoteVersion} (você está na v{CurrentVersion}). " +
                            $"Baixe em: {ReleasesPageUrl}");

                        if (AlreadyNotifiedToday(remoteVersionText)) return;
                        RememberNotification(remoteVersionText);

                        ShowUpdateNotification(
                            $"Nova versão da tradução PT-BR disponível: v{remoteVersion} (você está na v{CurrentVersion}).\n\n" +
                            $"{ReleasesPageUrl}\n\n" +
                            "Abrir a página de download agora?");
                    }
                }
            }
            catch
            {
                // sem rede, GitHub fora do ar, rate limit etc. — ignora
                // silenciosamente, isso nunca deve incomodar quem só quer jogar
            }
        }

        private bool AlreadyNotifiedToday(string remoteVersionText)
        {
            try
            {
                if (!File.Exists(notifiedVersionPath)) return false;
                return File.ReadAllText(notifiedVersionPath).Trim() == NotificationStamp(remoteVersionText);
            }
            catch
            {
                return false; // na dúvida avisa: perder o aviso é pior que repeti-lo
            }
        }

        private void RememberNotification(string remoteVersionText)
        {
            try
            {
                File.WriteAllText(notifiedVersionPath, NotificationStamp(remoteVersionText));
            }
            catch (Exception ex)
            {
                LoggerInstance.Warning("Não consegui registrar o aviso de atualização: " + ex.Message);
            }
        }

        private static string NotificationStamp(string remoteVersionText)
        {
            return $"{remoteVersionText}|{DateTime.Now:yyyy-MM-dd}";
        }

        // HashSet não é thread-safe e este método é chamado de dentro do patch
        // do GetL10n — se o jogo algum dia localizar texto fora da thread
        // principal (carregamento em background, por exemplo), dois Add
        // simultâneos poderiam corromper o set e travar o jogo num laço
        // infinito. O lock é barato aqui: só roda quando falta tradução.
        public static void RecordMissing(string template)
        {
            lock (missingKeys)
            {
                missingKeys.Add(template);
            }
        }

        public override void OnApplicationQuit()
        {
            if (missingPath == null) return;
            try
            {
                List<string> list;
                lock (missingKeys)
                {
                    if (missingKeys.Count == 0) return;
                    list = new List<string>(missingKeys);
                }
                string json = JsonConvert.SerializeObject(list, Formatting.Indented);
                File.WriteAllText(missingPath, json);
                LoggerInstance.Msg($"Wrote {list.Count} untranslated strings to {missingPath}");
            }
            catch (Exception ex)
            {
                LoggerInstance.Error("Failed to write missing_strings.json: " + ex);
            }
        }

        private Dictionary<string, string> LoadDict(string path, string label)
        {
            try
            {
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                    LoggerInstance.Msg($"Loaded {dict.Count} {label} strings from {path}");
                    return dict;
                }
                LoggerInstance.Warning($"{label} file not found at {path}.");
            }
            catch (Exception ex)
            {
                LoggerInstance.Error($"Failed to load {label} file: " + ex);
            }
            return new Dictionary<string, string>();
        }
    }

    [HarmonyPatch(typeof(GameMgr), nameof(GameMgr.GetL10n))]
    public static class GetL10nPatch
    {
        // The game's language selector only has 3 slots (en=0, cn=1, tc=2).
        // We hijack slot 0 ("English") in the UI, but "template" here is
        // ALWAYS the Chinese key regardless of the selected slot, so we
        // look Portuguese up by that Chinese key directly. If there is no
        // PT-BR translation for it, we fall back to English (also looked
        // up by the Chinese key) instead of leaving the original result
        // (which would be Chinese, since l10nIndex==0 wasn't actually
        // changing the template we receive).
        static void Postfix(string template, ref string __result)
        {
            if (string.IsNullOrEmpty(template)) return;
            if (Game.dataMgr.nativeData.l10nIndex != 0) return;

            if (Mod.PtBr.TryGetValue(template, out string pt) && !string.IsNullOrEmpty(pt))
            {
                __result = pt;
            }
            else if (Mod.EnFallback.TryGetValue(template, out string en) && !string.IsNullOrEmpty(en))
            {
                __result = en;
                Mod.RecordMissing(template);
            }
            else
            {
                Mod.RecordMissing(template);
            }
        }
    }
}
