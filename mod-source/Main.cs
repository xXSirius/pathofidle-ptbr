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
        public const string CurrentVersion = "1.4.0";

        private const string LatestReleaseApiUrl = "https://api.github.com/repos/xXSirius/pathofidle-ptbr/releases/latest";
        private const string ReleasesPageUrl = "https://github.com/xXSirius/pathofidle-ptbr/releases/latest";
        private const string RawContentBaseUrl = "https://raw.githubusercontent.com/xXSirius/pathofidle-ptbr";

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

        // O jogo reabre uma vez sozinho logo na primeira inicialização depois
        // de instalar/atualizar mods (comportamento do launcher, não deste
        // mod) — esse primeiro processo morre em segundos, e uma thread
        // IsBackground morre junto assim que o processo fecha, no meio do
        // MessageBoxW. Por isso só marcamos "já avisado" DEPOIS que a caixa
        // retorna: se o processo curto matar a thread antes disso, nada fica
        // gravado e o aviso aparece de novo no processo real que fica de pé.
        private void ShowUpdateNotification(string remoteVersionText, string message)
        {
            var thread = new Thread(() =>
            {
                int result = MessageBoxW(IntPtr.Zero, message, "Tradução PT-BR - Path of Idle",
                    MB_YESNO | MB_ICONINFORMATION | MB_TOPMOST | MB_SETFOREGROUND);
                RememberNotification(remoteVersionText);
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

        // Guarda a última versão de dicionário aplicada por
        // TryAutoUpdateDictionariesAsync. Sem isso, toda vez que o jogo
        // abrisse a checagem compararia contra o CurrentVersion fixo do
        // .dll (que não muda em releases só de conteúdo) e baixaria os
        // mesmos arquivos de novo a cada sessão.
        private static string dictVersionPath;

        public override void OnInitializeMelon()
        {
            string dir = MelonEnvironment.UserDataDirectory;
            PtBr = LoadDict(Path.Combine(dir, "ptbr_translation.json"), "PT-BR");
            EnFallback = LoadDict(Path.Combine(dir, "en_fallback.json"), "EN fallback");
            missingPath = Path.Combine(dir, "missing_strings.json");
            notifiedVersionPath = Path.Combine(dir, "last_notified_version.txt");
            dictVersionPath = Path.Combine(dir, "dict_version.txt");

            // dispara em segundo plano e nunca bloqueia a inicialização do
            // mod — se a rede falhar ou o GitHub estiver fora do ar, o jogo
            // segue normalmente, só sem o aviso de atualização
            _ = CheckForUpdateAsync();
        }

        // A maioria esmagadora das releases é só conteúdo de tradução
        // (dicionário novo, sem mudar o .dll — ver versionamento no
        // CONTRIBUTING.md: PATCH = tradução, MINOR/MAJOR = mudança no
        // mod). Um PATCH novo (X.Y.Z -> X.Y.Z+1, mesmo X.Y) é seguro pra
        // baixar e aplicar sozinho: só troca dois arquivos de texto, nunca
        // o .dll carregado em memória. MINOR/MAJOR mudam código, então
        // continuam exigindo o instalador manual (aviso com link, como
        // sempre foi) — nunca baixamos nem substituímos nenhum executável
        // sozinhos, só os dicionários JSON.
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

                    // Baseline efetiva: normalmente o CurrentVersion compilado no
                    // .dll, mas se já sincronizamos um PATCH mais novo dentro do
                    // mesmo X.Y numa sessão anterior, usa esse em vez de re-baixar
                    // o mesmo conteúdo a cada abertura do jogo.
                    var lastSyncedDict = ReadLastSyncedDictVersion();
                    var baseline = (lastSyncedDict != null
                        && lastSyncedDict.Major == currentVersion.Major
                        && lastSyncedDict.Minor == currentVersion.Minor
                        && lastSyncedDict > currentVersion)
                        ? lastSyncedDict
                        : currentVersion;

                    if (remoteVersion.Major != currentVersion.Major || remoteVersion.Minor != currentVersion.Minor)
                    {
                        if (remoteVersion <= baseline) return;

                        LoggerInstance.Msg(
                            $"Nova versão da tradução disponível: v{remoteVersion} (você está na v{CurrentVersion}). " +
                            $"Baixe em: {ReleasesPageUrl}");

                        if (AlreadyNotifiedToday(remoteVersionText)) return;

                        ShowUpdateNotification(remoteVersionText,
                            $"Nova versão da tradução PT-BR disponível: v{remoteVersion} (você está na v{CurrentVersion}).\n\n" +
                            $"{ReleasesPageUrl}\n\n" +
                            "Abrir a página de download agora?");
                        return;
                    }

                    if (remoteVersion > baseline)
                    {
                        await TryAutoUpdateDictionariesAsync(tag, remoteVersion);
                    }
                }
            }
            catch
            {
                // sem rede, GitHub fora do ar, rate limit etc. — ignora
                // silenciosamente, isso nunca deve incomodar quem só quer jogar
            }
        }

        // Baixa só os dois arquivos de texto (nunca o .dll) direto da tag da
        // release, valida que desserializam como dicionário e que não vieram
        // suspeitosamente menores que o atual (download truncado/corrompido)
        // antes de aceitar. Na dúvida, mantém o que já está carregado e
        // tenta de novo na próxima abertura do jogo — nunca deixa o mod num
        // estado pior do que estava.
        private async Task TryAutoUpdateDictionariesAsync(string tag, System.Version remoteVersion)
        {
            const double MinAcceptableRatio = 0.9;

            string ptBrJson;
            string enJson;
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(15);
                // Os dicionários passam de 500KB e crescem a cada patch do
                // jogo; o limite da checagem de versão (512KB) é pequeno
                // demais pra eles, por isso este cliente é separado — o
                // MaxResponseContentBufferSize só pode ser definido antes da
                // primeira requisição de cada HttpClient.
                client.MaxResponseContentBufferSize = 5 * 1024 * 1024;
                client.DefaultRequestHeaders.UserAgent.ParseAdd("PtBrTranslation-Mod");

                ptBrJson = await client.GetStringAsync($"{RawContentBaseUrl}/{tag}/installer/UserData/ptbr_translation.json");
                enJson = await client.GetStringAsync($"{RawContentBaseUrl}/{tag}/installer/UserData/en_fallback.json");
            }

            var newPtBr = JsonConvert.DeserializeObject<Dictionary<string, string>>(ptBrJson);
            var newEn = JsonConvert.DeserializeObject<Dictionary<string, string>>(enJson);

            if (newPtBr == null || newPtBr.Count < PtBr.Count * MinAcceptableRatio)
            {
                LoggerInstance.Warning(
                    $"Dicionário PT-BR baixado (v{remoteVersion}) parece incompleto " +
                    $"({newPtBr?.Count ?? 0} vs {PtBr.Count} entradas atuais) — ignorando.");
                return;
            }
            if (newEn == null || newEn.Count < EnFallback.Count * MinAcceptableRatio)
            {
                LoggerInstance.Warning(
                    $"Dicionário EN fallback baixado (v{remoteVersion}) parece incompleto " +
                    $"({newEn?.Count ?? 0} vs {EnFallback.Count} entradas atuais) — ignorando.");
                return;
            }

            string dir = MelonEnvironment.UserDataDirectory;
            WriteDictFile(Path.Combine(dir, "ptbr_translation.json"), ptBrJson);
            WriteDictFile(Path.Combine(dir, "en_fallback.json"), enJson);

            // Troca a referência do dicionário em memória: leitura é sempre a
            // instância inteira (antiga ou nova, nunca parcial), então isso é
            // seguro mesmo com o Postfix do Harmony lendo em paralelo.
            PtBr = newPtBr;
            EnFallback = newEn;

            File.WriteAllText(dictVersionPath, remoteVersion.ToString());

            LoggerInstance.Msg($"Dicionário de tradução atualizado automaticamente para v{remoteVersion} ({newPtBr.Count} strings PT-BR).");
        }

        // Grava com backup (.bak do arquivo anterior) e passo intermediário
        // (.tmp) pra nunca deixar um JSON pela metade no disco se o jogo
        // fechar no meio da escrita.
        private static void WriteDictFile(string path, string newContent)
        {
            if (File.Exists(path))
            {
                File.Copy(path, path + ".bak", overwrite: true);
            }
            string tempPath = path + ".tmp";
            File.WriteAllText(tempPath, newContent);
            File.Move(tempPath, path, overwrite: true);
        }

        private static System.Version ReadLastSyncedDictVersion()
        {
            try
            {
                if (!File.Exists(dictVersionPath)) return null;
                return System.Version.TryParse(File.ReadAllText(dictVersionPath).Trim(), out var v) ? v : null;
            }
            catch
            {
                return null;
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
