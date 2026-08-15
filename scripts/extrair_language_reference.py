# -*- coding: utf-8 -*-
r"""
Reconstroi scripts\language_reference.json a partir do TLanguage_MultiLang.cs
decompilado do Assembly-CSharp.dll do jogo (ver secao "Cenario B" do comando
/traducao-pathofidle para o passo a passo completo de extracao).

Uso:
    python extrair_language_reference.py caminho\para\TLanguage_MultiLang.cs

Sobrescreve scripts\language_reference.json e imprime quantas entradas sao
novas ou tiveram o texto em ingles alterado desde a ultima extracao, pra
saber o que precisa ser traduzido.
"""
import json
import os
import re
import sys

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
LANG_REF = os.path.join(SCRIPT_DIR, "language_reference.json")

ENTRY_RE = re.compile(
    r'new TLanguage_MultiLang\s*\{\s*'
    r'template\s*=\s*"(?P<template>(?:[^"\\]|\\.)*)",\s*'
    r'cn\s*=\s*"(?P<cn>(?:[^"\\]|\\.)*)",\s*'
    r'en\s*=\s*"(?P<en>(?:[^"\\]|\\.)*)",\s*'
    r'tc\s*=\s*"(?P<tc>(?:[^"\\]|\\.)*)"\s*'
    r'\}',
    re.DOTALL,
)


ESCAPE_RE = re.compile(r'\\(u[0-9a-fA-F]{4}|.)')
ESCAPE_MAP = {"n": "\n", "t": "\t", "r": "\r", '"': '"', "\\": "\\", "0": "\0"}


def unescape(s):
    def repl(m):
        g = m.group(1)
        if g.startswith("u"):
            return chr(int(g[1:], 16))
        return ESCAPE_MAP.get(g, g)

    return ESCAPE_RE.sub(repl, s)


def main():
    if len(sys.argv) != 2:
        print("Uso: python extrair_language_reference.py caminho\\para\\TLanguage_MultiLang.cs")
        sys.exit(1)

    with open(sys.argv[1], encoding="utf-8") as f:
        source = f.read()

    novo = {}
    for m in ENTRY_RE.finditer(source):
        cn = unescape(m.group("cn"))
        novo[cn] = {
            "cn": cn,
            "en": unescape(m.group("en")),
            "tc": unescape(m.group("tc")),
        }

    antigo = {}
    if os.path.exists(LANG_REF):
        with open(LANG_REF, encoding="utf-8") as f:
            antigo = json.load(f)

    novas_chaves = [cn for cn in novo if cn not in antigo]
    mudou_en = [
        cn for cn in novo
        if cn in antigo and novo[cn].get("en") != antigo[cn].get("en")
    ]
    removidas = [cn for cn in antigo if cn not in novo]

    with open(LANG_REF, "w", encoding="utf-8") as f:
        json.dump(novo, f, ensure_ascii=False, indent=1)

    print(f"Entradas extraidas: {len(novo)} (antes: {len(antigo)})")
    print(f"Novas: {len(novas_chaves)}")
    print(f"Com texto EN alterado: {len(mudou_en)}")
    print(f"Removidas (nao existem mais no jogo): {len(removidas)}")

    if novas_chaves:
        out = os.path.join(SCRIPT_DIR, "chaves_novas.json")
        with open(out, "w", encoding="utf-8") as f:
            json.dump(novas_chaves, f, ensure_ascii=False, indent=1)
        print(f"  -> chaves novas salvas em {out}")
    if mudou_en:
        out = os.path.join(SCRIPT_DIR, "chaves_en_alterado.json")
        with open(out, "w", encoding="utf-8") as f:
            json.dump(mudou_en, f, ensure_ascii=False, indent=1)
        print(f"  -> chaves com EN alterado salvas em {out}")


if __name__ == "__main__":
    main()
