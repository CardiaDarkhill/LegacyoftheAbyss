"""Look at the Knight bundle's HUD art without launching the game.

Why this exists
---------------
Every HUD change so far has cost a round trip: ship it, play it, screenshot it, find out the sprite
was turned the wrong way. The art is all sitting in an asset bundle that can be read offline, so
most of those questions can be answered before a build - which way a sprite is stored, how big it
is, where a socket sits inside a plate, whether a layout reads correctly.

It answered three real ones already: that the masks were packed rotated, that the plate carries the
orb's dark socket as part of itself (so the flat disc behind it was a second circle), and where that
socket sits, which is the constant `HUD.Core.FrameSocketX/Y`.

Usage
-----
    pip install UnityPy pillow
    python Tools/HudPreview.py

Writes into `Tools/preview/`: each sprite on its own, both rotations of anything the atlas stored
turned, and `mock.png` - the pieces laid out the way the HUD arranges them. Compare that against a
reference screenshot before changing HUD code.
"""

import os
import UnityPy
from PIL import Image, ImageOps

HERE = os.path.dirname(os.path.abspath(__file__))
BUNDLE = os.path.join(HERE, "..", "Assets", "Knight", "knight.bundle")
OUT = os.path.join(HERE, "preview")

# Whole textures, which is how art that belongs to no animation is stored.
TEXTURES = {"soul_orb_glow0000", "soul_orb_full_v020000"}

# Atlas regions, named by the sprite definition each HUD clip's first frame resolves to.
REGIONS = {"idle_v020000", "HUD_frame_v020005", "health_backboard"}

# Where the plate's socket sits inside it, as a fraction of the turned frame. Re-derive with
# socket_centre() below if the art changes; HUD.Core.cs carries the same numbers.
FRAME_SOCKET = (0.704, 0.568)


def script_name(obj, by_id):
    try:
        tree = obj.read_typetree()
        return by_id[tree["m_Script"]["m_PathID"]].read().m_ClassName
    except Exception:
        return "?"


def socket_centre(image):
    """The centroid of the large dark disc, as a fraction of the image."""
    width, height = image.size
    pixels = image.load()
    xs, ys = [], []
    for y in range(height):
        for x in range(width):
            r, g, b, a = pixels[x, y]
            if a > 200 and r < 70 and g < 70 and b < 90:
                xs.append(x)
                ys.append(y)

    if not xs:
        return None

    return (sum(xs) / len(xs) / width, sum(ys) / len(ys) / height)


def extract():
    os.makedirs(OUT, exist_ok=True)
    env = UnityPy.load(os.path.abspath(BUNDLE))
    by_id = {o.path_id: o for o in env.objects}
    found = {}

    for obj in env.objects:
        if obj.type.name == "Texture2D":
            data = obj.read()
            if data.m_Name in TEXTURES:
                found[data.m_Name] = data.image
                print(f"texture {data.m_Name} {data.m_Width}x{data.m_Height}")
            continue

        if obj.type.name != "MonoBehaviour" or script_name(obj, by_id) != "tk2dSpriteCollectionData":
            continue

        for definition in obj.read_typetree().get("spriteDefinitions", []):
            name = definition.get("name")
            if name not in REGIONS or name in found:
                continue

            material = by_id.get(definition.get("material", {}).get("m_PathID", 0))
            if material is None:
                continue

            atlas = None
            for entry in material.read_typetree().get("m_SavedProperties", {}).get("m_TexEnvs", []):
                if entry[0] == "_MainTex":
                    texture = by_id.get(entry[1]["m_Texture"]["m_PathID"])
                    if texture is not None:
                        atlas = texture.read().image
            if atlas is None:
                continue

            width, height = atlas.size
            xs = [p["x"] for p in definition["uvs"]]
            ys = [p["y"] for p in definition["uvs"]]

            # UVs put the origin bottom left; PIL puts it top left.
            box = (
                int(round(min(xs) * width)),
                int(round(height - max(ys) * height)),
                int(round(max(xs) * width)),
                int(round(height - min(ys) * height)),
            )
            crop = atlas.crop(box)
            turned = bool(definition.get("flipped", 0))
            print(f"region {name} {crop.size} flipped={turned}")

            if turned:
                # Both ways round, because which one is right is a question for the eye.
                crop.rotate(90, expand=True).save(os.path.join(OUT, f"{name}_rot+90.png"))
                crop.rotate(-90, expand=True).save(os.path.join(OUT, f"{name}_rot-90.png"))
                crop = crop.rotate(90, expand=True)

            found[name] = crop

    for name, image in found.items():
        image.save(os.path.join(OUT, f"{name}.png"))

    return found


def mock(found):
    """The pieces arranged the way the HUD arranges them, mirrored to the top right."""
    frame = found.get("HUD_frame_v020005")
    fill = found.get("soul_orb_full_v020000")
    glow = found.get("soul_orb_glow0000")
    mask = found.get("idle_v020000")
    if not all((frame, fill, glow, mask)):
        print("mock skipped: missing art")
        return

    measured = socket_centre(frame)
    if measured:
        print(f"socket centre measured at {measured[0]:.3f}, {measured[1]:.3f} (code uses {FRAME_SOCKET})")

    canvas = Image.new("RGBA", (1000, 320), (18, 20, 24, 255))
    fx = canvas.width - frame.width - 30
    fy = 20
    canvas.alpha_composite(frame, (fx, fy))

    cx = fx + int(FRAME_SOCKET[0] * frame.width)
    cy = fy + int(FRAME_SOCKET[1] * frame.height)
    canvas.alpha_composite(fill, (cx - fill.width // 2, cy - fill.height // 2))
    canvas.alpha_composite(glow, (cx - glow.width // 2, cy - glow.height // 2))

    x = cx - fill.width // 2 - 40
    for _ in range(5):
        x -= mask.width
        canvas.alpha_composite(mask, (x, cy - mask.height // 2))
        x -= 8

    path = os.path.join(OUT, "mock.png")
    canvas.save(path)
    print("wrote " + path)


if __name__ == "__main__":
    mock(extract())
