# Changelog

## 1.3.1 — 2026-08-15

Correções encontradas ao revisar as tabelas de habilidade do Sacerdote.

- 5 descrições de habilidade do Sacerdote que apareciam em inglês agora estão
  traduzidas: Dreno de Alma, Frio nos Ossos, Ceifar Alma, Mão da Morte e
  Elixir de Fortitude
- Nome do item 凝视之瞳 padronizado como **Olho Vigilante** em todos os textos
  (aparecia como "Olho Fitante" e "Olho Fulgurante" em duas descrições longas,
  divergindo do nome usado na tabela de itens)

## 1.3.0 — 2026-08-15

Aviso de atualização agora aparece na tela, e tradução sincronizada com o patch
1.0.3 do jogo.

- O aviso de versão nova deixou de ficar só no log do MelonLoader: agora abre uma
  janela quando o jogo inicia, mostrando o link e com um botão que abre a página
  de download no navegador. O aviso aparece no máximo uma vez por dia enquanto não
  atualizar — quem preferir adiar não é incomodado a cada partida
- 19 strings novas traduzidas do patch 1.0.3 do jogo, incluindo o changelog interno
  (tela "Notícias"), que agora aparece em português
- Novo `Desinstalar.bat` no pacote: remove a tradução sem tocar no MelonLoader,
  nos seus outros mods ou no seu save
- README e LEIA-ME agora explicam o aviso de atualização (o mod nunca baixa nada
  sozinho) e por que alguns antivírus reclamam de mods `.dll`
- Corrigido o `!` que sumia das mensagens do instalador ("Instalacao concluida",
  "Bom jogo") — efeito colateral do `enabledelayedexpansion` no `.bat`
- Endurecimento: limite de tamanho na resposta da API do GitHub e proteção contra
  concorrência no registro de textos sem tradução
- Novo utilitário `scripts/extrair_language_reference.py`, que reconstrói a tabela
  de referência CN/EN/TC direto do assembly do jogo a cada patch novo
- Regra de versionamento documentada no `CONTRIBUTING.md`: sincronizar a tradução
  com um patch do jogo é PATCH; MINOR fica para capacidade nova do mod

## 1.2.0 — 2026-08-14

Nova landing page do projeto e aviso de atualização dentro do jogo.

- Landing page em `docs/` (hospedável via GitHub Pages) apresentando o projeto, com
  galeria de screenshots, changelog ao vivo e passo a passo de instalação
- O mod agora consulta a release mais recente do GitHub ao iniciar e avisa no log do
  MelonLoader quando há uma versão nova disponível (não baixa nem substitui nada
  sozinho — atualizar continua sendo baixar o pacote e rodar o instalador de novo)
- Corrigida a versão do mod no `[assembly: MelonInfo(...)]`, que estava presa em
  "1.0.0" mesmo depois do lançamento da 1.1.0

## 1.1.0 — 2026-08-14

Atualização de conteúdo: o jogo recebeu o patch 1.0.2 (renomeação de dano "Frio"
para "Gelo" em várias habilidades, ajustes de balanceamento e correções).

- 28 strings novas ou corrigidas traduzidas (habilidades, itens, changelog interno do jogo)
- Corrigida `scripts/language_reference.json`, que estava sendo extraída de uma
  versão desatualizada da tabela de localização do jogo (classe de backup pré-patch),
  causando referência errada para futuras traduções
- Removidas 24 traduções órfãs cuja chave em chinês não corresponde mais a nenhum
  texto do jogo atual

## 1.0.0 — 2026-08-14

Primeiro lançamento público.

- Tradução completa de gameplay: mais de 4.700 strings (itens, habilidades, talentos, afixos, inimigos, conjuntos, interface)
- Fallback automático para inglês em qualquer texto ainda não traduzido (nunca aparece chinês)
- Registro automático de textos sem tradução (`UserData/missing_strings.json`) para facilitar reports
- Instalador automático (`Instalar.bat`) com detecção do diretório do jogo
- Console do MelonLoader oculto por padrão
