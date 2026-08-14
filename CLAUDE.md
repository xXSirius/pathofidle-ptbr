# Path of Idle: Old Gods Rising — Tradução PT-BR

Mod de tradução de fã (não oficial) para o jogo Steam "Path of Idle: Old Gods Rising",
via MelonLoader + Harmony. Repositório público: `xXSirius/pathofidle-ptbr`.

## Arquitetura essencial

- O jogo passa sempre a chave em **chinês** para `GameMgr.GetL10n`, não importa o
  idioma selecionado — `l10nIndex` só afeta o que a função original devolveria.
  Por isso **todo dicionário deste projeto usa chave chinesa**, nunca inglês.
- O mod (`mod-source/Main.cs`) usa Harmony para interceptar `GameMgr.GetL10n` quando
  `l10nIndex == 0` (idioma "English" selecionado no jogo) e substitui o resultado pela
  tradução em `installer/UserData/ptbr_translation.json`. Sem tradução, cai no fallback
  de `en_fallback.json`. O jogador nunca vê chinês na tela.
- `scripts/language_reference.json` é a tabela mestra CN/EN/TC extraída do jogo —
  usada para achar o texto em inglês de qualquer chave chinesa nova.
- O mod registra sozinho, em `UserData/missing_strings.json` (na pasta do jogo, não no
  repo), qualquer texto que passou sem tradução PT-BR durante uma sessão de jogo.

## Estrutura do repositório

```
mod-source/     código C# do mod (compilar com dotnet, precisa de $(GameDir))
installer/      pacote pronto para distribuir (dll compilado + dicionários + Instalar.bat)
scripts/        utilitários Python de manutenção da tradução + tabela de referência
```

Jogo instalado localmente em: `C:\Program Files (x86)\Steam\steamapps\common\PathOfIdle`

## Manutenção (updates, missing_strings.json, releases)

Use o comando **`/traducao-pathofidle`** — ele tem o playbook completo (os dois
cenários de manutenção, como recompilar, como publicar release, e a lista de gotchas
já descobertos). Não redescubra esse processo do zero.

## Terminologia e convenções de tradução

Ver [CONTRIBUTING.md](CONTRIBUTING.md) — tem a tabela de termos padronizados
(Mettle→Ânimo, Sunder→Ruptura, etc.) e como reportar/contribuir.

## Qualidade

Sem abstrações desnecessárias nos scripts — são utilitários de manutenção pontual,
não uma aplicação. Ao editar `mod-source/Main.cs`, teste sempre abrindo o jogo e
conferindo `MelonLoader/Latest.log` antes de dar como resolvido — falhas no Harmony
patch são silenciosas (não geram erro, só param de traduzir).
