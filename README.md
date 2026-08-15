# Path of Idle: Old Gods Rising — Tradução PT-BR

Tradução de fã (não oficial) para português do Brasil do jogo [Path of Idle: Old Gods Rising](https://store.steampowered.com/) (Steam), via mod para [MelonLoader](https://melonwiki.xyz/).

Cobre praticamente todo o texto de gameplay: itens, habilidades, talentos, afixos, inimigos, sistemas, interface. Mais de 4.700 strings traduzidas.

> Este projeto não tem vínculo oficial com a desenvolvedora do jogo (SmallMill). É um trabalho de fã, gratuito, feito com auxílio de IA e revisado para fidelidade ao texto original.

## Como funciona

O jogo tem 3 idiomas nativos: `English`, `简体中文` e `繁體中文`. Este mod intercepta o texto do idioma **English** e substitui pela tradução em português. Qualquer texto ainda sem tradução aparece em inglês normalmente.

## Instalação

### Pré-requisito: MelonLoader

O mod roda sobre o [MelonLoader](https://melonwiki.xyz/), um framework de mods gratuito e de código aberto, usado por milhares de jogos Unity.

1. Baixe o instalador oficial em **https://melonwiki.xyz/**
2. Abra o instalador e aponte para o executável do jogo (`PathOfIdle.exe`)
3. Clique em **Install**
4. Abra o jogo uma vez e feche em seguida (isso cria as pastas `Mods` e `UserData`)

### Instalando a tradução

1. Baixe o pacote mais recente em [**Releases**](../../releases)
2. Extraia o `.zip`
3. Rode `Instalar.bat` (detecta a pasta do jogo automaticamente)
4. Abra o jogo, vá em **Configurações** e deixe o idioma em **English**

Pronto — a tradução aparece automaticamente.

### Instalação manual

Se preferir não usar o instalador, copie os arquivos manualmente:

| Arquivo                          | Destino (dentro da pasta do jogo) |
| -------------------------------- | --------------------------------- |
| `Mods/PtBrTranslation.dll`       | `Mods/`                           |
| `UserData/ptbr_translation.json` | `UserData/`                       |
| `UserData/en_fallback.json`      | `UserData/`                       |

## Desinstalar / desativar

- **Remover de vez**: rode `Desinstalar.bat` (vem no mesmo pacote do instalador). Ele apaga só os arquivos da tradução, sem tocar no MelonLoader, nos seus outros mods ou no seu save
- **Só desativar temporariamente**: renomeie `Mods/PtBrTranslation.dll` para `PtBrTranslation.dll.disabled`
- **Manualmente**: apague os 3 arquivos listados na tabela acima da pasta do jogo
- O MelonLoader pode continuar instalado sem problema, mesmo sem nenhum mod ativo

## Aviso de atualização

Ao abrir o jogo, o mod consulta a página de releases deste repositório para ver se saiu uma versão nova da tradução. Se tiver, aparece uma janela do Windows com o link e a opção de abrir a página de download — no máximo uma vez por dia, até você atualizar.

O mod **não baixa nem instala nada sozinho**: atualizar é sempre baixar o pacote e rodar o `Instalar.bat`, como na primeira vez. É a única conexão de rede que o mod faz, e se ela falhar (sem internet, GitHub fora do ar) o jogo continua normalmente.

## Meu antivírus reclamou, é vírus?

Não. Mods de jogos Unity são arquivos `.dll` que se injetam no processo do jogo — comportamento que alguns antivírus marcam como suspeito por padrão, sem análise real do conteúdo. É um falso positivo comum em qualquer mod de MelonLoader, não específico deste projeto.

Se preferir não confiar na palavra de um desconhecido na internet (e você não deveria mesmo!): todo o código do mod está neste repositório, em [`mod-source/Main.cs`](mod-source/Main.cs) — são ~170 linhas, dá pra ler inteiro em poucos minutos. Você pode compilar você mesmo seguindo o [CONTRIBUTING.md](CONTRIBUTING.md) e usar o seu próprio `.dll` em vez do que eu distribuo.

## Encontrou algo sem traduzir ou um erro?

Abra uma [Issue](../../issues/new/choose) — tem um modelo pronto para reportar texto sem tradução e outro para erros de tradução.

O próprio mod ajuda nisso: ele registra automaticamente qualquer texto que aparecer sem tradução em `UserData/missing_strings.json`. Se quiser contribuir, é só anexar esse arquivo numa Issue.

## Contribuindo

Veja [CONTRIBUTING.md](CONTRIBUTING.md) para saber como sugerir traduções, reportar erros, ou contribuir com o código do mod.

## Estrutura do repositório

```
mod-source/     código-fonte do mod (C#), para quem quiser compilar ou revisar
installer/      pacote pronto para instalar (mod compilado + dicionários + instalador)
```

## Como funciona por baixo dos panos

O mod usa [Harmony](https://github.com/pardeike/Harmony) para interceptar a função interna de localização do jogo (`GameMgr.GetL10n`). Quando o idioma selecionado é English, o mod substitui o texto retornado pela tradução em português correspondente, buscando por uma chave interna do jogo. Se não houver tradução para aquele texto, ele usa um fallback em inglês.

## Aviso legal

Este é um projeto de fã, sem fins lucrativos, que não distribui nenhum arquivo original do jogo — apenas um dicionário de tradução e um pequeno mod que aplica essa tradução em tempo real sobre uma cópia legitimamente adquirida do jogo. Path of Idle: Old Gods Rising é propriedade de sua desenvolvedora, SmallMill.

## Apoiar o projeto

A tradução é gratuita e sempre vai continuar sendo. Se quiser apoiar o trabalho, doações via Pix são bem-vindas (totalmente opcional):

```
c720ee26-eac5-47a8-9005-2c96578e9411

```

## Licença

Código deste repositório sob [MIT](LICENSE). Veja a licença para uma nota sobre o texto traduzido, que é derivado do jogo original.
