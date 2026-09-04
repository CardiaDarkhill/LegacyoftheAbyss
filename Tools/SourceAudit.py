"""Five static checks over the mod's own C# source, run offline.

Why this exists
---------------
Each of these was written by hand, thrown away, and rediscovered at least twice. They answer
questions this codebase keeps asking and that the compiler cannot:

``reflection``
    Every ``GetField`` / ``GetProperty`` / ``GetMethod`` / ``AccessTools.*`` lookup against a game
    type, cross-checked against ``Tests/GameApiContract.cs``. The project rule is that a reflective
    lookup in shipped code has a matching assertion there, because reflection that comes back empty
    neither throws nor logs - the feature simply never runs, and in a bug report that is
    indistinguishable from "the situation never arose". This check has found lookups with no test
    behind them every time it has been run.

``dead``
    Declarations whose identifier appears exactly once in the whole tree, i.e. at the declaration.
    Interface implementations, Unity messages and Harmony patch members are the false positives;
    everything else is code nothing reaches.

``duplication``
    Windows of normalised statements that appear more than once. Literals and strings are blanked
    before hashing, so a copied block that had its numbers changed still matches. Most hits are
    ``using`` blocks; the interesting ones are two hand-written lists of the same thing, which is
    the failure mode this repo keeps hitting.

``perframe``
    Allocation-shaped lines inside a Unity per-frame message. Only the bodies of ``Update`` /
    ``LateUpdate`` / ``FixedUpdate`` / ``OnGUI`` are read - calls out of them are not followed - so
    this is a first pass over the obvious sites. Lines behind a one-shot deadline or an exception
    handler are the expected false positives; read the guard before believing a hit.

``subscriptions``
    ``x.Event += handler`` with no ``-=`` naming that event anywhere in the tree. Numeric ``+=`` is
    the bulk of the output and is filtered on the right-hand side looking like a handler, not
    perfectly. What this is looking for is a *static or long-lived* publisher left holding a
    destroyed ``MonoBehaviour``; a static handler on a static event is a permanent subscription by
    design and fine.

The compiler answers ``dead`` better than this does
---------------------------------------------------
For unused *private* members, Roslyn's own analysers are exact where the text scan here is
heuristic. They are off in the normal build because they would fail the zero-warning rule on Unity
message handlers, which look unused to every static analysis and are called by name::

    dotnet build -c Release -p:EnforceCodeStyleInBuild=true --no-incremental

IDE0051 is "private member is unused" and IDE0052 is "assigned but never read"; every Unity message
(``Awake``, ``Update``, ``OnTriggerEnter2D``, ...) is a false positive and everything else is real.
Adding ``dotnet_diagnostic.IDE0059.severity = warning`` and ``IDE0060`` to ``.editorconfig`` for one
run additionally reports discarded values and parameters nothing reads - remove them again
afterwards. Use the ``dead`` check below when a full build is inconvenient, and those when it is not.

Usage
-----
    python Tools/SourceAudit.py                 # all five
    python Tools/SourceAudit.py reflection
    python Tools/SourceAudit.py duplication 10  # window size in statements, default 8
    python Tools/SourceAudit.py perframe
    python Tools/SourceAudit.py subscriptions

Run from the repository root. Nothing here imports anything outside the standard library.
"""

from __future__ import annotations

import collections
import hashlib
import os
import re
import sys

# Directories with no first-party C# in them. Decompiles is the game's own source, kept as a
# reference; the build outputs are redirected out of the tree but may exist from an older layout.
SKIP_DIRS = {"Decompiles", "obj", "bin", ".git", "BuildTemplates", "Assets", "Tools"}

# Unity calls these by name, Harmony calls these by convention, and the runtime calls the rest
# through an interface. None of them is referenced anywhere a text search can see.
CALLED_BY_NAME = {
    "Awake", "Start", "Update", "LateUpdate", "FixedUpdate", "OnEnable", "OnDisable", "OnDestroy",
    "OnTriggerEnter2D", "OnTriggerExit2D", "OnTriggerStay2D", "OnCollisionEnter2D",
    "OnCollisionExit2D", "OnCollisionStay2D", "OnGUI", "OnApplicationQuit", "OnApplicationFocus",
    "OnApplicationPause", "OnDrawGizmos", "OnValidate", "OnLevelWasLoaded", "Reset",
    "OnBecameVisible", "OnBecameInvisible",
    "Prefix", "Postfix", "Transpiler", "Finalizer", "TargetMethod", "TargetMethods", "Prepare",
    "Cleanup",
    "ToString", "Equals", "GetHashCode", "Dispose", "CompareTo", "MoveNext", "GetEnumerator",
}


def source_files(root: str, include_tests: bool = True):
    for dirpath, dirnames, filenames in os.walk(root):
        dirnames[:] = [d for d in dirnames if d not in SKIP_DIRS]
        if not include_tests:
            dirnames[:] = [d for d in dirnames if d != "Tests"]
        for filename in sorted(filenames):
            if filename.endswith(".cs"):
                yield os.path.join(dirpath, filename)


def read(path: str) -> str:
    return open(path, encoding="utf-8-sig", errors="replace").read()


# --------------------------------------------------------------------------- reflection coverage

LOOKUP_WITH_OWNER = re.compile(
    r'(?:Get(?:Field|Property|Method)'
    r'|AccessTools\.(?:Field|Property|Method|DeclaredMethod|DeclaredField|DeclaredProperty'
    r'|PropertyGetter|PropertySetter|Inner))'
    r'\s*(?:<[^>]*>)?\s*\(\s*(?:typeof\(([A-Za-z0-9_.<>]+)\)|([A-Za-z0-9_]+))\s*,\s*"([^"]+)"')

# Any receiver, not only typeof(T): a lookup through a Type held in a local or a field is the same
# dependency, and one of those - the reflective ImageConversion.LoadImage - hid from an earlier
# version of this check that only looked for typeof.
LOOKUP_ON_ANY_RECEIVER = re.compile(
    r'([A-Za-z0-9_.?()\[\]<>]+?)\.Get(?:Field|Property|Method)\(\s*"([^"]+)"')

# FieldRefAccess throws out of a static initialiser rather than returning null, which is louder but
# still only at first touch, in-game.
FIELD_REF = re.compile(r'AccessTools\.FieldRefAccess<\s*([A-Za-z0-9_.]+)\s*,[^>]*>\s*\(\s*"([^"]+)"')


def check_reflection(root: str) -> int:
    sites: dict[tuple[str, str], list[str]] = {}

    def record(owner: str, member: str, path: str, text: str, offset: int) -> None:
        line = text[:offset].count("\n") + 1
        sites.setdefault((owner, member), []).append(f"{os.path.relpath(path, root)}:{line}")

    for path in source_files(root, include_tests=False):
        text = read(path)
        for match in LOOKUP_WITH_OWNER.finditer(text):
            record(match.group(1) or match.group(2), match.group(3), path, text, match.start())
        for match in LOOKUP_ON_ANY_RECEIVER.finditer(text):
            record(match.group(1), match.group(2), path, text, match.start())
        for match in FIELD_REF.finditer(text):
            record(match.group(1), match.group(2), path, text, match.start())

    tests_dir = os.path.join(root, "Tests")
    tests = "".join(
        read(os.path.join(tests_dir, name))
        for name in sorted(os.listdir(tests_dir))
        if name.endswith(".cs"))

    missing = [key for key in sorted(sites) if f'"{key[1]}"' not in tests]

    print(f"reflection: {len(sites)} distinct lookups, {len(missing)} with no test naming the member")
    for owner, member in missing:
        print(f"  {owner}.{member}")
        for site in sites[(owner, member)][:3]:
            print(f"      {site}")
    return len(missing)


# ------------------------------------------------------------------------------------- dead code

DECLARATIONS = [
    (re.compile(
        r'^[ \t]*(?:\[[^\]]*\]\s*)*(?:public|private|internal|protected)\s+'
        r'(?:static\s+|virtual\s+|override\s+|sealed\s+|async\s+|new\s+|unsafe\s+|extern\s+|partial\s+)*'
        r'[\w\.<>\[\],\?\s]+?\s+(\w+)\s*(?:<[^>]*>)?\s*\(', re.M), "method"),
    (re.compile(
        r'^[ \t]*(?:\[[^\]]*\]\s*)*(?:public|private|internal|protected)\s+'
        r'(?:static\s+|readonly\s+|const\s+|volatile\s+|new\s+)*'
        r'[\w\.<>\[\],\?]+(?:\s*\[\])?\s+(\w+)\s*(?:=|;|\{\s*get)', re.M), "member"),
]


def check_dead(root: str) -> int:
    texts = {path: read(path) for path in source_files(root)}
    frequency = collections.Counter(re.findall(r'\b[A-Za-z_]\w*\b', "\n".join(texts.values())))

    findings = []
    for path, text in texts.items():
        relative = os.path.relpath(path, root)
        # Test method names are only ever named by the attribute above them.
        if relative.startswith("Tests" + os.sep):
            continue
        for pattern, kind in DECLARATIONS:
            for match in pattern.finditer(text):
                name = match.group(1)
                if name in CALLED_BY_NAME or len(name) < 3 or frequency[name] > 1:
                    continue
                line = text[:match.start()].count("\n") + 1
                findings.append((relative, line, kind, name))

    print(f"dead: {len(findings)} declarations whose name appears nowhere else")
    print("      (interface implementations and JSON DTO properties are the expected false positives)")
    for relative, line, kind, name in sorted(findings):
        print(f"  {relative}:{line}  [{kind}] {name}")
    return len(findings)


# ------------------------------------------------------------------------------------ duplication

def normalise(line: str) -> str | None:
    stripped = line.strip()
    if not stripped or stripped.startswith(("//", "*", "/*")):
        return None
    if stripped in ("{", "}", "});", "};", ")", ";"):
        return None
    stripped = re.sub(r'"[^"]*"', "STR", stripped)
    stripped = re.sub(r'\b\d+(?:\.\d+)?f?\b', "NUM", stripped)
    return re.sub(r'\s+', " ", stripped)


def check_duplication(root: str, window: int) -> int:
    statements = []
    for path in source_files(root):
        relative = os.path.relpath(path, root)
        for number, line in enumerate(read(path).split("\n"), 1):
            text = normalise(line)
            if text:
                statements.append((relative, number, text))

    buckets: dict[str, list[tuple[str, int, int]]] = collections.defaultdict(list)
    for index in range(len(statements) - window):
        run = statements[index:index + window]
        if len({entry[0] for entry in run}) != 1:
            continue  # a window spanning two files is not a run of anything
        key = hashlib.md5("\n".join(entry[2] for entry in run).encode()).hexdigest()
        buckets[key].append((run[0][0], run[0][1], run[-1][1]))

    groups = sorted((v for v in buckets.values() if len(v) >= 2), key=len, reverse=True)

    # Overlapping windows describe the same duplication, so only the first to claim a line is shown.
    covered: set[tuple[str, int]] = set()
    shown = 0
    for group in groups:
        if any((f, line) in covered for f, start, end in group for line in range(start, end + 1)):
            continue
        for f, start, end in group:
            covered.update((f, line) for line in range(start, end + 1))
        print(f"\n  {len(group)} copies of a {window}-statement run:")
        for f, start, end in group:
            print(f"      {f}:{start}-{end}")
        shown += 1

    print(f"\nduplication: {shown} repeated runs of {window} statements")
    return shown


# ------------------------------------------------------------------------- per-frame allocation

PER_FRAME_METHOD = re.compile(
    r'\b(?:private|public|internal|protected)?\s*(?:static\s+)?void\s+'
    r'(Update|LateUpdate|FixedUpdate|OnGUI)\s*\(\s*\)')

ALLOCATION = re.compile(
    r'(\.ToArray\(\)|\.ToList\(\)|\.Where\(|\.Select\(|\.OrderBy\(|\.Any\('
    r'|new List<|new Dictionary<|new HashSet<|new StringBuilder|string\.Join'
    r'|GetComponentsInChildren|FindObjectsOfType|\$")')


def method_body(lines, index):
    """Line range of the body whose signature is on `index`, or None."""
    opening = index
    while opening < len(lines) and "{" not in lines[opening]:
        if ";" in lines[opening]:
            return None
        opening += 1
    if opening >= len(lines):
        return None

    depth = 0
    closing = opening
    while closing < len(lines):
        depth += lines[closing].count("{") - lines[closing].count("}")
        if depth <= 0 and closing > opening:
            break
        closing += 1
    return opening, min(closing, len(lines) - 1)


def check_perframe(root: str) -> int:
    hits = 0
    for path in source_files(root, include_tests=False):
        rel = os.path.relpath(path, root)
        lines = read(path).replace("\r\n", "\n").split("\n")
        i = 0
        while i < len(lines):
            match = PER_FRAME_METHOD.search(lines[i])
            if not match:
                i += 1
                continue
            span = method_body(lines, i)
            if span is None:
                i += 1
                continue
            start, end = span
            for k in range(start, end + 1):
                line = lines[k]
                if line.lstrip().startswith("//"):
                    continue
                if ALLOCATION.search(line):
                    print(f"  {rel}:{k + 1}  [{match.group(1)}]  {line.strip()[:110]}")
                    hits += 1
            i = end + 1

    print(f"\nperframe: {hits} allocation-shaped lines inside a per-frame method")
    return hits


# ------------------------------------------------------------------------------ event lifetimes

SUBSCRIBE = re.compile(r'([A-Za-z_][\w.]*)\s*\+=\s*([A-Za-z_][\w.]*)\s*;')
UNSUBSCRIBE = re.compile(r'([A-Za-z_][\w.]*)\s*-=\s*([A-Za-z_][\w.]*)\s*;')


def check_subscriptions(root: str) -> int:
    added = collections.defaultdict(list)
    removed = set()

    for path in source_files(root, include_tests=False):
        rel = os.path.relpath(path, root)
        for number, line in enumerate(read(path).replace("\r\n", "\n").split("\n"), 1):
            if line.lstrip().startswith("//"):
                continue
            for match in SUBSCRIBE.finditer(line):
                event = match.group(1).split(".")[-1]
                added[event].append((rel, number, line.strip()[:100]))
            for match in UNSUBSCRIBE.finditer(line):
                removed.add(match.group(1).split(".")[-1])

    unmatched = {name: sites for name, sites in added.items() if name not in removed}
    for name in sorted(unmatched):
        for rel, number, text in unmatched[name]:
            print(f"  {rel}:{number}  [{name}]  {text}")

    print(f"\nsubscriptions: {len(unmatched)} names subscribed with no matching -= anywhere")
    return len(unmatched)


# ------------------------------------------------------------------------------------------ main

def main() -> int:
    root = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    which = sys.argv[1] if len(sys.argv) > 1 else "all"
    window = int(sys.argv[2]) if len(sys.argv) > 2 else 8

    if which in ("all", "reflection"):
        check_reflection(root)
        print()
    if which in ("all", "dead"):
        check_dead(root)
        print()
    if which in ("all", "duplication"):
        check_duplication(root, window)
        print()
    if which in ("all", "perframe"):
        check_perframe(root)
        print()
    if which in ("all", "subscriptions"):
        check_subscriptions(root)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
