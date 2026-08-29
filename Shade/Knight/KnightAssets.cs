#nullable enable

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LegacyoftheAbyss.Shade.Knight
{
    /// <summary>
    /// Loads the Knight's art from the asset bundle borrowed from Knight in Silksong. Loading is
    /// lazy — the bundle is ~54 MB and only a companion wearing the Knight needs it.
    /// </summary>
    internal static class KnightAssets
    {
        private const string BundleFolder = "Knight";
        private const string BundleFile = "knight.bundle";
        private const string ShaderMapFile = "MaterialShaderMap.json";
        private const string KnightPrefabName = "Knight_0";
        private const string IdleClipName = "Idle";

        private static AssetBundle? s_bundle;
        private static readonly Dictionary<string, GameObject> s_prefabs = new();
        private static Dictionary<string, string>? s_materialShaderMap;
        private static readonly Dictionary<string, Shader> s_shaders = new();
        private static bool s_shaderScanRegistered;
        private static bool s_shadersApplied;
        private static bool s_loadFailed;

        /// <summary>The Knight body prefab, or null when the bundle is missing or failed to load.</summary>
        internal static GameObject? KnightPrefab
            => s_prefabs.TryGetValue(KnightPrefabName, out var prefab) ? prefab : null;

        internal static string BundlePath => Path.Combine(ModPaths.Assets, BundleFolder, BundleFile);

        /// <summary>
        /// Loads the bundle if it is not already loaded. Returns false and logs once when the bundle
        /// is absent or unreadable, so a missing download disables the Knight rather than the mod.
        /// </summary>
        internal static bool TryLoad()
        {
            if (s_bundle != null)
            {
                return true;
            }

            if (s_loadFailed)
            {
                return false;
            }

            string path = BundlePath;
            if (!File.Exists(path))
            {
                s_loadFailed = true;
                LegacyHelper.LogWarning($"Knight character disabled: asset bundle not found at {path}.");
                return false;
            }

            s_bundle = AssetBundle.LoadFromFile(path);
            if (s_bundle == null)
            {
                s_loadFailed = true;
                LegacyHelper.LogWarning($"Knight character disabled: {path} could not be read as an asset bundle.");
                return false;
            }

            foreach (var prefab in s_bundle.LoadAllAssets<GameObject>())
            {
                if (prefab != null)
                {
                    s_prefabs[prefab.name] = prefab;
                }
            }

            if (!s_prefabs.ContainsKey(KnightPrefabName))
            {
                s_loadFailed = true;
                LegacyHelper.LogWarning($"Knight character disabled: '{KnightPrefabName}' is missing from {BundleFile}.");
                Unload();
                return false;
            }

            LoadShaderMap();
            PreparePrefabs();
            BeginShaderScan();
            LogBundleContents();
            return true;
        }

        /// <summary>What the bundle turned out to contain, for <c>BugReportState</c>.</summary>
        internal static string Inventory { get; private set; } = "bundle not loaded";

        /// <summary>
        /// Records the animation, audio and prefab names the bundle carries. Kept on the state
        /// snapshot rather than only logged: this happens once at first load, and the log ring is a
        /// few hundred lines, so by the time anyone files a report it has long scrolled away -
        /// which is exactly what happened the first time it was asked for.
        /// </summary>
        private static void LogBundleContents()
        {
            if (s_bundle == null)
            {
                return;
            }

            var clips = new List<string>();
            foreach (var prefab in s_prefabs.Values)
            {
                foreach (var animator in prefab.GetComponentsInChildren<tk2dSpriteAnimator>(true))
                {
                    var library = animator.Library;
                    if (library?.clips == null)
                    {
                        continue;
                    }

                    foreach (var clip in library.clips)
                    {
                        if (clip != null && !string.IsNullOrEmpty(clip.name) && !clips.Contains(clip.name))
                        {
                            clips.Add(clip.name);
                        }
                    }
                }
            }

            clips.Sort();

            var audio = new List<string>();
            foreach (var clip in s_bundle.LoadAllAssets<AudioClip>())
            {
                if (clip != null && !string.IsNullOrEmpty(clip.name))
                {
                    audio.Add(clip.name);
                }
            }

            audio.Sort();

            var roots = new List<string>(s_prefabs.Keys);
            roots.Sort();

            Inventory =
                $"anims ({clips.Count}): {string.Join(", ", clips)}"
                + $" | audio ({audio.Count}): {string.Join(", ", audio)}"
                + $" | prefabs ({roots.Count}): {string.Join(", ", roots)}";

            if (ModConfig.Instance.logShade)
            {
                LegacyHelper.LogInfo("Knight bundle contents: " + Inventory);
            }
        }

        /// <summary>
        /// A still from the Knight's idle animation, for the Characters menu, or null when the
        /// bundle or the clip is unavailable. Loading the bundle here is deliberate: it happens the
        /// first time the Knight row is focused, in a paused menu, rather than at launch for
        /// everyone.
        /// </summary>
        internal static Sprite? TryBuildIdlePreview()
        {
            if (!TryLoad())
            {
                return null;
            }

            ApplyShaders();

            var prefab = KnightPrefab;
            var animator = prefab != null ? prefab.GetComponentInChildren<tk2dSpriteAnimator>(true) : null;
            var clip = animator?.Library != null ? animator.Library.GetClipByName(IdleClipName) : null;
            if (clip?.frames == null || clip.frames.Length == 0)
            {
                LegacyHelper.LogWarning($"Knight preview falls back to the shipped still: no '{IdleClipName}' clip in the bundle.");
                return null;
            }

            var frame = clip.frames[0];
            var collection = frame?.spriteCollection;
            if (collection?.spriteDefinitions == null
                || frame == null
                || frame.spriteId < 0
                || frame.spriteId >= collection.spriteDefinitions.Length)
            {
                return null;
            }

            var definition = collection.spriteDefinitions[frame.spriteId];
            var texture = definition?.material != null ? definition.material.mainTexture as Texture2D : null;
            if (definition?.uvs == null || definition.uvs.Length == 0 || texture == null)
            {
                return null;
            }

            // The frame is a region of the atlas, described by its UV corners.
            Vector2 min = definition.uvs[0];
            Vector2 max = definition.uvs[0];
            for (int i = 1; i < definition.uvs.Length; i++)
            {
                min = Vector2.Min(min, definition.uvs[i]);
                max = Vector2.Max(max, definition.uvs[i]);
            }

            var rect = new Rect(
                min.x * texture.width,
                min.y * texture.height,
                (max.x - min.x) * texture.width,
                (max.y - min.y) * texture.height);

            if (rect.width < 1f || rect.height < 1f)
            {
                return null;
            }

            var sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
            sprite.name = "KnightIdlePreview";
            sprite.hideFlags = HideFlags.HideAndDontSave;
            return sprite;
        }

        internal static void Unload()
        {
            s_prefabs.Clear();
            s_shaders.Clear();
            s_shadersApplied = false;

            if (s_shaderScanRegistered)
            {
                SceneManager.activeSceneChanged -= HandleSceneChanged;
                s_shaderScanRegistered = false;
            }

            if (s_bundle != null)
            {
                s_bundle.Unload(unloadAllLoadedObjects: true);
                s_bundle = null;
            }
        }

        private static void LoadShaderMap()
        {
            string path = Path.Combine(ModPaths.Assets, BundleFolder, ShaderMapFile);
            if (!File.Exists(path))
            {
                LegacyHelper.LogWarning($"Knight materials will not be repaired: {ShaderMapFile} not found at {path}.");
                return;
            }

            s_materialShaderMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path));
            if (s_materialShaderMap == null)
            {
                LegacyHelper.LogWarning($"Knight materials will not be repaired: {ShaderMapFile} did not parse as a material/shader map.");
            }
        }

        /// <summary>
        /// Fixes what does not survive being bundled: sprite collections would collide with the
        /// game's own by name, and colliders come back with their interaction layers cleared.
        /// </summary>
        private static void PreparePrefabs()
        {
            foreach (var prefab in s_prefabs.Values)
            {
                foreach (var collection in prefab.GetComponentsInChildren<tk2dSpriteCollectionData>(true))
                {
                    if (collection.spriteCollectionName != null && !collection.spriteCollectionName.EndsWith("(Hallownest)"))
                    {
                        collection.spriteCollectionName += "(Hallownest)";
                        collection.name = collection.spriteCollectionName;
                    }
                }

                foreach (var collider in prefab.GetComponentsInChildren<Collider2D>(true))
                {
                    collider.callbackLayers = -1;
                    collider.contactCaptureLayers = -1;
                    collider.forceReceiveLayers = -1;
                    collider.forceSendLayers = -1;
                }
            }
        }

        /// <summary>
        /// A bundled material keeps its shader by name only, and the name resolves to nothing until
        /// a scene using that shader has loaded. So shaders are collected across scene changes and
        /// the repair runs once <see cref="ApplyShaders"/> is asked for.
        /// </summary>
        private static void BeginShaderScan()
        {
            CollectShaders();

            if (!s_shaderScanRegistered)
            {
                SceneManager.activeSceneChanged += HandleSceneChanged;
                s_shaderScanRegistered = true;
            }
        }

        private static void HandleSceneChanged(Scene from, Scene to) => CollectShaders();

        private static void CollectShaders()
        {
            foreach (var shader in Resources.FindObjectsOfTypeAll<Shader>())
            {
                if (shader != null && shader.isSupported && !s_shaders.ContainsKey(shader.name))
                {
                    s_shaders[shader.name] = shader;
                }
            }
        }

        /// <summary>
        /// Points every bundled material at a live shader. Without this the Knight draws magenta.
        /// Runs once, at the first Knight spawn, by which point a gameplay scene has supplied the
        /// sprite shaders the map asks for.
        /// </summary>
        internal static void ApplyShaders()
        {
            if (s_shadersApplied || s_bundle == null || s_materialShaderMap == null)
            {
                return;
            }

            s_shadersApplied = true;
            CollectShaders();
            SceneManager.activeSceneChanged -= HandleSceneChanged;
            s_shaderScanRegistered = false;

            int repaired = 0;
            int unresolved = 0;

            foreach (var material in CollectMaterials())
            {
                if (!s_materialShaderMap.TryGetValue(material.name, out var shaderName))
                {
                    unresolved++;
                    continue;
                }

                var shader = ResolveShader(shaderName);
                if (shader == null)
                {
                    unresolved++;
                    LegacyHelper.LogWarning($"Knight material '{material.name}' wants shader '{shaderName}', which this game build does not have.");
                    continue;
                }

                material.shader = shader;
                repaired++;
            }

            if (ModConfig.Instance.logShade || unresolved > 0)
            {
                LegacyHelper.LogInfo($"Knight materials repaired: {repaired} shaders reassigned, {unresolved} unresolved.");
            }
        }

        /// <summary>
        /// The substitutions cover shaders Hollow Knight had that Silksong does not ship under the
        /// same name; each falls back to the closest blend the game does have.
        /// </summary>
        private static Shader? ResolveShader(string shaderName)
        {
            if (s_shaders.TryGetValue(shaderName, out var shader))
            {
                return shader;
            }

            string? substitute = shaderName switch
            {
                "tk2d/BlendVertexColor" => "tk2d/BlendVertexColor (addressable)",
                "UI/BlendModes/Lighten" => "UI/BlendModes/Screen",
                "UI/BlendModes/Multiply" => "UI/BlendModes/Screen",
                "UI/BlendModes/VividLight" => "UI/BlendModes/Screen",
                _ => null
            };

            return substitute != null && s_shaders.TryGetValue(substitute, out var fallback) ? fallback : null;
        }

        private static HashSet<Material> CollectMaterials()
        {
            var materials = new HashSet<Material>();

            foreach (var prefab in s_prefabs.Values)
            {
                foreach (var renderer in prefab.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    if (renderer.sharedMaterial != null)
                        materials.Add(renderer.sharedMaterial);
                }

                foreach (var renderer in prefab.GetComponentsInChildren<ParticleSystemRenderer>(true))
                {
                    if (renderer.sharedMaterial != null)
                        materials.Add(renderer.sharedMaterial);
                }

                foreach (var data in prefab.GetComponentsInChildren<tk2dSpriteCollectionData>(true))
                {
                    if (data.materials == null)
                        continue;

                    foreach (var material in data.materials)
                    {
                        if (material != null)
                            materials.Add(material);
                    }
                }
            }

            return materials;
        }
    }
}
