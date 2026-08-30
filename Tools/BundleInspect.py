"""Answer questions about the Knight bundle's prefabs and clips without launching the game.

Why this exists
---------------
Borrowing Hollow Knight's own art for the charms meant naming prefabs and clips in code, and a
wrong name costs a build, a play session and a bug report to find out - while looking exactly like
the charm being unimplemented. Worse, several of the names are actively misleading, because the
game sets at runtime what the prefab does not carry.

Every one of these was found here rather than in-game:

* `Charm Thorn Counter` is the charm's *inventory icon* and a trigger box. The vines are `Thorn
  Attack`, six frames on the Knight's shared clip library with no object of their own.
* `Grubberfly BeamD` is byte-for-byte `Grubberfly BeamL`, and `BeamU` is `BeamL` turned +90 degrees
  - which points the left-facing art *down*. Picking a beam by name gave the wrong direction three
  times; the fix was to take one base prefab and rotate it in code.
* `Knight Dung Cloud` and `Knight Spore Cloud` have no renderer at all. They are a collider and a
  bag of children: particle systems that are the cloud, and seven `Impact Lines` one-shots the FSM
  was going to fire outward. Stripped of the FSM those sat there as static orange streaks.
* Effect prefabs ship with their renderers *and* animators disabled, so anything borrowed has to be
  woken or it draws nothing while looking perfectly correct in the hierarchy.

Usage
-----
    pip install UnityPy

    python Tools/BundleInspect.py prefabs weaverling hatchling     # names, components, transforms
    python Tools/BundleInspect.py clips thorn dung                 # clip names, frame counts, fps
    python Tools/BundleInspect.py frames "Thorn Attack"            # per-frame sprite and atlas region
    python Tools/BundleInspect.py hosts "Thorn Attack"             # prefabs able to play that clip

`prefabs` matches on a substring of the name; the others take whole clip names. `hosts` is the one
to reach for when a clip has no prefab of its own: the whole rig shares one library, so any object
listed can be borrowed as a body to play it on.
"""

import os
import sys
import math

import UnityPy

HERE = os.path.dirname(os.path.abspath(__file__))

# The repo's checked-in copy first, then the one the running mod reads. See "Asset & logging paths"
# in AGENTS.md - they are different folders and either may be the one that is current.
BUNDLE_CANDIDATES = (
    os.path.join(HERE, "..", "Assets", "Knight", "knight.bundle"),
    os.path.join(HERE, "..", "..", "Assets", "Knight", "knight.bundle"),
)


def load_bundle():
    for path in BUNDLE_CANDIDATES:
        if os.path.isfile(path):
            print(f"# {os.path.normpath(path)}\n")
            return UnityPy.load(path)

    looked = "\n  ".join(os.path.normpath(p) for p in BUNDLE_CANDIDATES)
    raise SystemExit(f"knight.bundle not found. Looked in:\n  {looked}")


def name_of(obj):
    """An object's m_Name, or empty. Reading can throw on types UnityPy cannot parse."""
    try:
        return getattr(obj.read(), "m_Name", "") or ""
    except Exception:
        return ""


def component_refs(tree, objs):
    """The objects on a GameObject's component list, in order."""
    for entry in tree.get("m_Component", []):
        if not isinstance(entry, dict):
            continue
        path_id = entry.get("component", {}).get("m_PathID")
        ref = objs.get(path_id)
        if ref is not None:
            yield ref


def describe_component(ref, objs):
    """A component's type, naming the script for a MonoBehaviour - the useful half."""
    if ref.type.name != "MonoBehaviour":
        return ref.type.name

    try:
        script = objs.get(ref.read_typetree().get("m_Script", {}).get("m_PathID"))
        return f"MB<{name_of(script) or '?'}>"
    except Exception:
        return "MB<?>"


def euler_z(quaternion):
    x = quaternion.get("x", 0.0)
    y = quaternion.get("y", 0.0)
    z = quaternion.get("z", 0.0)
    w = quaternion.get("w", 1.0)
    return math.degrees(math.atan2(2 * (w * z + x * y), 1 - 2 * (y * y + z * z)))


def cmd_prefabs(env, objs, terms):
    """Components, transform and children of every root GameObject matching a term.

    The transform matters as much as the component list: a prefab's baked rotation and scale are
    what a borrowed copy inherits, and for the beams they are the whole story.
    """
    terms = [t.lower() for t in terms]
    for obj in env.objects:
        if obj.type.name != "GameObject":
            continue

        name = name_of(obj)
        if not name or not any(t in name.lower() for t in terms):
            continue

        try:
            tree = obj.read_typetree()
        except Exception:
            continue

        components = [describe_component(ref, objs) for ref in component_refs(tree, objs)]
        print(f"{name}")
        print(f"    components: {', '.join(components) or '(none)'}")

        for ref in component_refs(tree, objs):
            if ref.type.name != "Transform":
                continue

            transform = ref.read_typetree()
            scale = transform.get("m_LocalScale", {})
            print(f"    rotZ={euler_z(transform.get('m_LocalRotation', {})):.1f}"
                  f"  scale=({scale.get('x')}, {scale.get('y')}, {scale.get('z')})")

            children = []
            for child in transform.get("m_Children") or []:
                child_ref = objs.get(child.get("m_PathID"))
                if child_ref is None:
                    continue
                owner = objs.get(child_ref.read_typetree().get("m_GameObject", {}).get("m_PathID"))
                children.append(name_of(owner) if owner else "?")

            if children:
                print(f"    children: {', '.join(children)}")

        print()


def iter_libraries(env):
    """Every tk2dSpriteAnimation in the bundle, as (object, clip list)."""
    for obj in env.objects:
        if obj.type.name != "MonoBehaviour":
            continue
        try:
            tree = obj.read_typetree()
        except Exception:
            continue
        clips = tree.get("clips")
        if isinstance(clips, list):
            yield obj, clips


def cmd_clips(env, objs, terms):
    """Clip name, frame count and rate for every clip matching a term."""
    terms = [t.lower() for t in terms]
    for _, clips in iter_libraries(env):
        for clip in clips:
            name = clip.get("name") or ""
            if not name or not any(t in name.lower() for t in terms):
                continue

            frames = clip.get("frames") or []
            print(f"{name}: {len(frames)} frames, fps={clip.get('fps')}, wrapMode={clip.get('wrapMode')}")


def cmd_frames(env, objs, names):
    """Per-frame sprite name, atlas region and material for one clip.

    `flipped` is the one to read: tk2d packs sprites turned to save atlas space, and a Unity Sprite
    has nowhere to carry that, so whoever draws the frame has to turn it back.
    """
    for _, clips in iter_libraries(env):
        for clip in clips:
            if clip.get("name") not in names:
                continue

            print(f"{clip.get('name')}: {len(clip.get('frames') or [])} frames")
            for index, frame in enumerate(clip.get("frames") or []):
                sprite_id = frame.get("spriteId")
                collection = objs.get((frame.get("spriteCollection") or {}).get("m_PathID"))
                if collection is None:
                    print(f"  {index}: spriteId={sprite_id} collection=MISSING")
                    continue

                definitions = collection.read_typetree().get("spriteDefinitions") or []
                if not 0 <= sprite_id < len(definitions):
                    print(f"  {index}: spriteId={sprite_id} out of range ({len(definitions)} defs)")
                    continue

                definition = definitions[sprite_id]
                material = objs.get((definition.get("material") or {}).get("m_PathID"))
                uvs = definition.get("uvs") or []
                span = ""
                if uvs:
                    xs = [u.get("x") for u in uvs]
                    ys = [u.get("y") for u in uvs]
                    span = f" uv=({min(xs):.4f}..{max(xs):.4f}, {min(ys):.4f}..{max(ys):.4f})"

                print(f"  {index}: '{definition.get('name')}' flipped={definition.get('flipped')}"
                      f" material='{name_of(material) if material else 'NONE'}'{span}")
            print()


def cmd_hosts(env, objs, names):
    """Prefabs whose animator library holds the named clip, i.e. anything able to play it.

    Art that exists only as a clip has to be played on *something*. The rig shares one library, so
    a small effect prefab from this list can stand in as a body without dragging the Knight in.
    """
    libraries = set()
    for obj, clips in iter_libraries(env):
        if any(clip.get("name") in names for clip in clips):
            libraries.add(obj.path_id)

    print(f"libraries holding {', '.join(repr(n) for n in names)}: {len(libraries)}")
    if not libraries:
        return

    hosts = set()
    for obj in env.objects:
        if obj.type.name != "GameObject":
            continue
        try:
            tree = obj.read_typetree()
        except Exception:
            continue

        for ref in component_refs(tree, objs):
            if ref.type.name != "MonoBehaviour":
                continue
            try:
                library = ref.read_typetree().get("library")
            except Exception:
                continue
            if isinstance(library, dict) and library.get("m_PathID") in libraries:
                hosts.add((name_of(obj), len(tree.get("m_Component", []))))

    for host, components in sorted(hosts):
        print(f"    {host}  ({components} components)")


COMMANDS = {
    "prefabs": cmd_prefabs,
    "clips": cmd_clips,
    "frames": cmd_frames,
    "hosts": cmd_hosts,
}


def main(argv):
    if len(argv) < 3 or argv[1] not in COMMANDS:
        raise SystemExit(__doc__)

    env = load_bundle()
    objs = {obj.path_id: obj for obj in env.objects}
    COMMANDS[argv[1]](env, objs, argv[2:])


if __name__ == "__main__":
    main(sys.argv)
