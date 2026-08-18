# Changelog

## 1.4.1 — 2026-08-18

Correção de nomes que colidiam: a Torre Divina aparecia no menu com o mesmo
nome do Santuário (o prédio da cidade), o que fazia parecer que existiam dois
Santuários no jogo. O guia também ganhou uma seção sobre luta de chefe.

- Corrigido: a **Torre Divina** (a atividade ao lado de Exploração) aparecia
  como "Santuário", nome que pertence ao prédio da cidade. A causa é que a
  localização em inglês do próprio jogo é inconsistente — chama a Torre de
  "Divine Tower" na maioria dos textos, mas de "Sanctum" no rótulo do menu,
  que é justamente o nome do Santuário
- Unificado o efeito de Conjunto "Santuário Divino" para **Abrigo Divino**,
  que é como a habilidade do Clérigo já aparecia — era a mesma habilidade
  com dois nomes diferentes dependendo da tela
- Corrigido "Contagem de Conjunto:" para **"Definir quantidade:"** — o
  inglês "Set Count:" é ambíguo e tinha sido lido como Conjunto de
  equipamento, quando ali "Set" é o verbo "definir"
- Guia: nova seção **"Chefes e a Torre"**, em português e inglês — as regras
  que mudam numa luta de chefe, quais maestrias viram peso morto contra
  alvo único, as que são feitas pra chefe, e uma composição de time de 3
  para subir a Torre

## 1.4.0 — 2026-08-17

O mod agora se atualiza sozinho quando a nova versão é só dicionário de
tradução (a grande maioria dos lançamentos) — sem precisar baixar o zip e
rodar o instalador de novo toda vez. Também corrigido o widget de changelog
do site, que podia ficar travado em "Carregando..." pra sempre.

- Auto-update: ao detectar uma release nova que muda só o PATCH (ex: 1.4.0 →
  1.4.1), o mod baixa e aplica os dois dicionários (PT-BR e fallback EN)
  sozinho, com validação de integridade e backup do arquivo anterior. Só
  troca texto — nunca o `.dll` — e continua exigindo o instalador manual
  quando a mudança é de verdade no mod (MINOR/MAJOR)
- Corrigido o widget "O Que Mudou" da landing page: sem timeout, uma falha
  de rede no `raw.githubusercontent.com` podia deixar a tela presa em
  "Carregando changelog..." pra sempre em vez de mostrar o aviso de erro

## 1.3.6 — 2026-08-17

Correção da sincronização com o patch 1.0.5: a varredura anterior só tinha
capturado 1 das várias strings novas que o update trouxe.

- Extraída a tabela de idiomas real do jogo (em vez de depender só do que
  aparece durante uma sessão de jogo) para achar tudo que o patch 1.0.5
  mudou de uma vez
- Traduzidas 44 strings novas: nova tela de configuração automática da loja,
  textos de talento por nível de Bênção, habilidades mutadas, corrupção
  sempre benéfica, entre outras
- Traduzida a segunda variante do changelog interno do patch 1.0.5 (o jogo
  mostra duas versões ligeiramente diferentes do mesmo texto em telas
  diferentes — a primeira já tinha sido traduzida na v1.3.5, a segunda
  estava aparecendo em chinês)

## 1.3.5 — 2026-08-17

Tradução sincronizada com o patch 1.0.5 do jogo.

- Changelog interno do patch 1.0.5 (tela "Notícias") agora aparece em português

## 1.3.4 — 2026-08-16

Nova página no site: o "Códice do Devoto", guia de gameplay em `/guia`.

- Classes, mecânicas, maestrias, conjuntos, prisão e as 39 builds oficiais,
  extraídos das tabelas internas do jogo
- Nova seção explicando Percepção (por item do Codex, não global) e Selos de
  Afixo (a única forma de mirar o que cai ao selar um afixo)
- Linkado no Header e no Footer do site

## 1.3.3 — 2026-08-16

Separa dois termos que o jogo distingue em ingles mas a traducao tinha
colapsado num so, causando confusao sobre qual barra de nivel e qual.

- **`Worshiper` agora e "Adorador"** (antes: "Devoto"). E o seu nivel de conta,
  o do canto superior direito — o teto dele e 5 x o numero do capitulo, e subir
  aumenta quantos personagens voce pode ter e as recompensas offline
- **`Godsworn` continua "Devoto"**, agora de forma consistente: 29 textos usavam
  "Escolhido" para o mesmo termo. Sao os personagens que voce recruta na Prisao
- Antes das duas correcoes, "Devoto" aparecia para as duas coisas ao mesmo tempo,
  e frases como "aumentar o nivel do Devoto aumenta o numero maximo de Devotos"
  ficavam sem sentido
- **Corrigido: o aviso de atualizacao nunca parava de aparecer.** O mod se
  identificava como `1.3.0` porque o `.dll` nao era recompilado desde aquela
  versao — entao quem instalava a v1.3.1 ou v1.3.2 (que eram so dicionario)
  continuava vendo "nova versao disponivel" todo dia, mesmo ja estando em dia.
  Agora a versao do mod acompanha a da release

## 1.3.2 — 2026-08-16

Tradução sincronizada com o patch 1.0.4 do jogo.

- Changelog interno do patch 1.0.4 (tela "Notícias") agora aparece em português

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
