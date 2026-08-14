import numpy as np
from PIL import Image, ImageFilter, ImageDraw

SRC = "C:/Users/lucas/Desktop/POI-Tradução-pt-br-logo.png"


def save(img, path, note=""):
    img.save(path)
    print(path, img.size, "-", note)


def recolor_outline_to_black(img):
    """Every letter's antialiased boundary (partial alpha, between the solid
    fill and full transparency) blends toward a light/pinkish tint in the
    source art - invisible against the light backgrounds the logo was
    designed on, but a bright halo against this site's dark background.
    Force RGB to black on every partial-alpha pixel (alpha strictly between 0
    and 255) so that fringe fades into the page's own dark color instead of a
    lighter tint; fully-opaque pixels (the letters' own interior shine/bevel)
    are untouched, and fully-transparent pixels don't matter."""
    arr = np.array(img)
    alpha = arr[:, :, 3]
    edge = (alpha > 0) & (alpha < 255)
    out = arr.copy()
    out[edge, 0:3] = 0
    return Image.fromarray(out, mode="RGBA")


im = Image.open(SRC).convert("RGBA")
im = recolor_outline_to_black(im)
save(im, "landing-page/logo-recolored-debug.png", "debug: full source after outline recolor")

# 1) Wordmark only ("Path of Idle", no eye, no subtitle) - header/footer brand.
# y=715 keeps every letter's full pointed serif tail (they hang lower than they
# look at a glance - a first attempt at y=650 sliced them flat, looked cut off).
wordmark = im.crop((0, 0, 2730, 715))
bbox = wordmark.split()[-1].getbbox()
wordmark = wordmark.crop(bbox)
target_w = 1100
wordmark = wordmark.resize((target_w, int(target_w * wordmark.height / wordmark.width)), Image.LANCZOS)
save(wordmark, "site/src/assets/logo/wordmark.png", "header/footer lockup")

# 2) Full logo (tight-cropped to real content), for the Hero. Higher target
# width + PNG (lossless) output at the Image() call site avoids the softness
# a webp re-encode adds to fine serif detail.
full = im.crop(im.split()[-1].getbbox())
target_w = 1800
full = full.resize((target_w, int(target_w * full.height / full.width)), Image.LANCZOS)
save(full, "site/src/assets/logo/full.png", "hero lockup")

# 3) Eye emblem only, square-padded - favicon / og accent.
eye = im.crop((1145, 745, 1495, 1025))
eye_bbox = eye.split()[-1].getbbox()
eye = eye.crop(eye_bbox)
side = max(eye.size)
pad = int(side * 0.14)
canvas = Image.new("RGBA", (side + pad * 2, side + pad * 2), (0, 0, 0, 0))
canvas.paste(eye, ((canvas.width - eye.width) // 2, (canvas.height - eye.height) // 2), eye)
canvas = canvas.resize((512, 512), Image.LANCZOS)
save(canvas, "site/public/favicon.png", "eye emblem, square")
save(canvas.resize((180, 180), Image.LANCZOS), "site/public/apple-touch-icon.png", "apple touch icon")

# 4) OG image: full logo over a dark branded card, for social link previews.
og = Image.new("RGBA", (1200, 630), (10, 8, 7, 255))
glow = Image.new("RGBA", (1200, 630), (0, 0, 0, 0))
draw = ImageDraw.Draw(glow)
draw.ellipse((300, -260, 900, 340), fill=(122, 20, 20, 130))
glow = glow.filter(ImageFilter.GaussianBlur(120))
og.alpha_composite(glow)

og_logo = im.crop(im.split()[-1].getbbox())
og_w = 820
og_logo = og_logo.resize((og_w, int(og_w * og_logo.height / og_logo.width)), Image.LANCZOS)
og.alpha_composite(og_logo, ((og.width - og_logo.width) // 2, (og.height - og_logo.height) // 2 - 20))

save(og.convert("RGB"), "site/public/og-image.png", "social preview card")
