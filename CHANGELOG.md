# Changelog

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
