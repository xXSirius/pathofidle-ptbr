from PIL import Image, ImageFilter
import os

SRC = "landing-page/screenshots"
OUT = "docs/assets/img"
os.makedirs(OUT, exist_ok=True)

def save_webp(im, path, quality):
    im.convert("RGB").save(path, "WEBP", quality=quality, method=6)
    print(path, im.size, os.path.getsize(path) // 1024, "KB")

# Hero background: sky + eldritch tower silhouette from Screenshot_1.
# Crop keeps x:0-690 / y:132-380 specifically to fall *between* the HUD
# elements (resource counters + "Dadiva" button sit above y=130, the
# "3 Templo" location tag sits below y=383) so no UI text leaks into the
# banner. A touch of blur hides the upscale softness and helps text
# legibility once the CSS gradient overlay sits on top.
hero = Image.open(f"{SRC}/Screenshot_1.png")
hero_crop = hero.crop((0, 132, 690, 380))
hero_crop = hero_crop.resize((2200, int(2200 * hero_crop.height / hero_crop.width)), Image.LANCZOS)
hero_crop = hero_crop.filter(ImageFilter.GaussianBlur(radius=1.6))
save_webp(hero_crop, f"{OUT}/hero-bg.webp", 82)

gallery = [
    ("Screenshot_9.png", "gallery-manual.webp", (570, 200, 1400, 870)),
    ("Screenshot_8.png", "gallery-mapa.webp", (820, 0, 1919, 1079)),
    ("Screenshot_7.png", "gallery-codex.webp", (820, 0, 1919, 1079)),
    ("Screenshot_4.png", "gallery-talentos.webp", (640, 150, 1919, 1060)),
    ("Screenshot_10.png", "gallery-ficha.webp", (820, 0, 1919, 1079)),
    ("Screenshot_2.png", "gallery-item.webp", (820, 0, 1919, 1079)),
]

for src_name, out_name, box in gallery:
    im = Image.open(f"{SRC}/{src_name}")
    crop = im.crop(box)
    target_w = 1000
    crop = crop.resize((target_w, int(target_w * crop.height / crop.width)), Image.LANCZOS)
    save_webp(crop, f"{OUT}/{out_name}", 82)

print("done")
