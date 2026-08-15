# Contribuindo

Obrigado por querer ajudar a melhorar a tradução! Formas de contribuir:

## 1. Reportar texto sem tradução

Se você encontrar algo em inglês (ou, raramente, em chinês) que deveria estar em português:

- Abra uma [Issue usando o modelo "Tradução faltando"](../../issues/new/choose)
- Se puder, anexe o arquivo `UserData/missing_strings.json` da pasta do jogo — o mod gera esse arquivo automaticamente ao fechar o jogo, listando qualquer texto que passou sem tradução durante a sessão

## 2. Reportar um erro de tradução

Encontrou uma tradução errada, incoerente com o resto do jogo, ou que não bate com o contexto?

- Abra uma [Issue usando o modelo "Erro de tradução"](../../issues/new/choose)
- Inclua o texto em português que está errado e, se souber, o texto original em inglês
- Sugestões de correção são muito bem-vindas

## 3. Sugerir uma tradução diretamente

Se você sabe programação e quer contribuir direto:

1. Faça um fork do repositório
2. As traduções ficam em `installer/UserData/ptbr_translation.json` (chave = texto original do jogo em chinês, valor = tradução em português — sim, é chinês mesmo, veja a nota técnica abaixo)
3. Abra um Pull Request explicando a mudança

### Nota técnica: por que as chaves são em chinês?

O jogo internamente sempre identifica cada texto pela sua chave em chinês (é assim que a tabela de localização do jogo funciona, independente do idioma exibido). Por isso `ptbr_translation.json` usa o texto chinês como chave — é a única forma confiável de bater com o texto certo em tempo real, não é uma escolha arbitrária.

Se você não sabe chinês e quer sugerir uma tradução, o jeito mais fácil é abrir uma Issue com o texto em inglês (que aparece no jogo quando não há tradução) e a sugestão em português — não precisa mexer no JSON.

## Terminologia usada no jogo

Para manter consistência, alguns termos já têm tradução padronizada em todo o dicionário:

| Inglês | Português |
|---|---|
| Fortitude | Fortitude |
| Mettle | Ânimo |
| Haste | Rapidez |
| Sunder | Ruptura |
| Slow | Lentidão |
| Wound | Ferimento |
| Bloodied | Ensanguentado |
| Minion | Lacaio |
| Ally | Aliado |
| Warcry | Grito de Guerra |
| Buff / Debuff | Buff / Debuff (mantidos como no original) |

Ao sugerir traduções novas, tente manter esses termos consistentes com o resto do dicionário.

## Versionamento

O mod usa `MAJOR.MINOR.PATCH`, mas o número é do **mod**, não do jogo — não
precisa (nem deve) tentar bater com a versão do jogo, que segue seu próprio
ritmo. Sincronizar tradução com um patch novo do jogo vai ser o tipo de
release mais recorrente, então **não** pode ser o que sobe o dígito do meio —
senão o problema só muda de dígito. A regra de quando subir cada parte:

- **PATCH** (x.y.**Z**): qualquer manutenção recorrente — tradução
  sincronizada com um patch novo do jogo (strings novas/corrigidas de
  gameplay, incluindo o changelog interno do jogo — ver nota técnica abaixo),
  correção de tradução, landing page, instalador, scripts, metadado etc.
- **MINOR** (x.**Y**.0): o **mod em si** ganhou uma capacidade nova (não a
  tradução) — ex.: um mecanismo novo no `mod-source`, uma nova forma de
  instalar/atualizar. Deve ser raro.
- **MAJOR**: só em mudança estrutural incompatível (formato do dicionário,
  forma de instalar, etc.).

Sempre cite no `CHANGELOG.md` qual patch do jogo aquela release cobre, já que
o número do mod não reflete isso diretamente.

### Traduzindo o changelog interno do jogo

A tela de "atualização da versão X" que o jogo mostra também é só mais uma
entrada no dicionário de localização, com chave em chinês igual a qualquer
habilidade ou item — não é um mecanismo separado. Ela aparece no fluxo normal
de sincronização de conteúdo (`missing_strings.json` / atualização de
`scripts/language_reference.json` após um patch do jogo) como mais uma string
faltante, só que mais longa (várias linhas com 【】 e `\n`). Ao traduzir,
mantenha a formatação e os nomes de habilidade/item consistentes com o resto
do glossário.

## Compilando o mod

Requisitos: [.NET 8 SDK](https://dotnet.microsoft.com/download), MelonLoader já instalado no jogo (para ter os assemblies de interop gerados).

```
cd mod-source
dotnet build -c Release -p:GameDir="CAMINHO_DA_PASTA_DO_JOGO"
```

O `.dll` compilado sai em `mod-source/bin/Release/PtBrTranslation.dll`.
