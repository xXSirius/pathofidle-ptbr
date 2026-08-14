# -*- coding: utf-8 -*-
r"""
Mescla novas traducoes (cn -> pt) no dicionario oficial do projeto, garante
que cada chave nova tambem tenha uma entrada de fallback em ingles, e
sincroniza a pasta do jogo instalado localmente.

Uso:
    python merge_translations.py caminho\para\novas_traducoes.json

novas_traducoes.json: {"<chave em chines>": "<traducao em portugues>", ...}
(normalmente gerado traduzindo scripts\pendente_traducao.json)
"""
import json
import os
import sys

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
REPO_DIR = os.path.dirname(SCRIPT_DIR)
LANG_REF = os.path.join(SCRIPT_DIR, "language_reference.json")

PTBR_REPO = os.path.join(REPO_DIR, "installer", "UserData", "ptbr_translation.json")
ENFB_REPO = os.path.join(REPO_DIR, "installer", "UserData", "en_fallback.json")

GAME_DIR = r"C:\Program Files (x86)\Steam\steamapps\common\PathOfIdle"
PTBR_GAME = os.path.join(GAME_DIR, "UserData", "ptbr_translation.json")
ENFB_GAME = os.path.join(GAME_DIR, "UserData", "en_fallback.json")


def load(path):
    with open(path, encoding="utf-8") as f:
        return json.load(f)


def save(path, data):
    with open(path, "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=1)


def main():
    if len(sys.argv) != 2:
        print("Uso: python merge_translations.py caminho\\para\\novas_traducoes.json")
        sys.exit(1)

    novas = load(sys.argv[1])
    lang = load(LANG_REF)
    ptbr = load(PTBR_REPO)
    enfb = load(ENFB_REPO)

    before = len(ptbr)
    added_en = 0

    for cn, pt in novas.items():
        ptbr[cn] = pt
        if cn not in enfb or not enfb[cn]:
            meta = lang.get(cn)
            if meta and meta.get("en"):
                enfb[cn] = meta["en"]
                added_en += 1

    after = len(ptbr)

    # repo
    save(PTBR_REPO, ptbr)
    save(ENFB_REPO, enfb)

    # live game install, se existir
    synced_game = False
    if os.path.isdir(os.path.join(GAME_DIR, "UserData")):
        save(PTBR_GAME, ptbr)
        save(ENFB_GAME, enfb)
        synced_game = True

    print(f"PT-BR: {before} -> {after} entradas ({after - before} novas/atualizadas)")
    print(f"EN fallback: +{added_en} entradas novas")
    print(f"Repo atualizado: {PTBR_REPO}")
    print(f"Jogo local sincronizado: {'sim' if synced_game else 'nao encontrado, pulei'}")


if __name__ == "__main__":
    main()
