# -*- coding: utf-8 -*-
r"""
Cruza um missing_strings.json (lista de chaves em chines, gerado pelo mod
em UserData/ do jogo) com a tabela de referencia CN/EN/TC e com o
dicionario PT-BR atual, para produzir uma lista do que realmente precisa
ser traduzido (com o texto em ingles como contexto).

Uso:
    python resolver_missing.py caminho\para\missing_strings.json

Gera: scripts\pendente_traducao.json  (cn -> en, so o que ainda falta)
"""
import json
import os
import sys

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
REPO_DIR = os.path.dirname(SCRIPT_DIR)
LANG_REF = os.path.join(SCRIPT_DIR, "language_reference.json")
PTBR_DICT = os.path.join(REPO_DIR, "installer", "UserData", "ptbr_translation.json")
OUT_PATH = os.path.join(SCRIPT_DIR, "pendente_traducao.json")


def main():
    if len(sys.argv) != 2:
        print("Uso: python resolver_missing.py caminho\\para\\missing_strings.json")
        sys.exit(1)

    missing_path = sys.argv[1]

    with open(LANG_REF, encoding="utf-8") as f:
        lang = json.load(f)  # cn -> {cn, en, tc}

    with open(PTBR_DICT, encoding="utf-8") as f:
        ptbr = json.load(f)  # cn -> pt

    with open(missing_path, encoding="utf-8") as f:
        missing_keys = json.load(f)  # list of cn strings

    pendente = {}
    ja_traduzido = 0
    sem_referencia = []

    for cn in missing_keys:
        if cn in ptbr and ptbr[cn]:
            ja_traduzido += 1
            continue
        meta = lang.get(cn)
        if meta:
            pendente[cn] = meta.get("en", "")
        else:
            sem_referencia.append(cn)

    with open(OUT_PATH, "w", encoding="utf-8") as f:
        json.dump(pendente, f, ensure_ascii=False, indent=1)

    print(f"Chaves no missing_strings.json: {len(missing_keys)}")
    print(f"Ja tinham traducao PT-BR (falso alarme / cache antigo): {ja_traduzido}")
    print(f"Pendentes de traducao: {len(pendente)} -> {OUT_PATH}")
    print(f"Sem correspondencia na tabela de referencia (chave nova, provavelmente conteudo de update): {len(sem_referencia)}")
    if sem_referencia:
        sem_ref_path = os.path.join(SCRIPT_DIR, "chaves_sem_referencia.json")
        with open(sem_ref_path, "w", encoding="utf-8") as f:
            json.dump(sem_referencia, f, ensure_ascii=False, indent=1)
        print(f"  -> salvas em {sem_ref_path} (precisa extrair do assembly novo do jogo)")


if __name__ == "__main__":
    main()
