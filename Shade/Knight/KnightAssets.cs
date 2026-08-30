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
        private static AssetBundleCreateRequest? s_preload;
        private static Dictionary<string, AudioClip>? s_audio;
        private static bool s_audioScannedGlobally;

        /// <summary>The Knight body prefab, or null when the bundle is missing or failed to load.</summary>
        internal static GameObject? KnightPrefab
            => s_prefabs.TryGetValue(KnightPrefabName, out var prefab) ? prefab : null;

        /// <summary>
        /// Any bundled prefab by name, or null. <see cref="Inventory"/> lists what is in there, and
        /// is written into every bug report, so a name can be checked against a real bundle rather
        /// than guessed at.
        /// </summary>
        internal static GameObject? FindPrefab(string name)
            => !string.IsNullOrEmpty(name) && s_prefabs.TryGetValue(name, out var prefab) ? prefab : null;

        internal static string BundlePath => Path.Combine(ModPaths.Assets, BundleFolder, BundleFile);

        /// <summary>
        /// Starts reading the bundle in the background, at launch, so that nothing later has to wait
        /// for it.
        /// <para>
        /// It is 54 MB, and loading it on demand meant the game locked up for about a second the
        /// first time anyone picked the Knight in the Characters menu. Asynchronous rather than
        /// simply moved into startup, so it costs a background read instead of a second on the
        /// splash screen; <see cref="TryLoad"/> finishes the request early if something does ask
        /// before it lands.
        /// </para>
        /// </summary>
        internal static void BeginPreload()
        {
            if (s_bundle != null || s_preload != null || s_loadFailed)
            {
                return;
            }

            string path = BundlePath;
            if (!File.Exists(path))
            {
                s_loadFailed = true;
                LegacyHelper.LogWarning($"Knight character disabled: asset bundle not found at {path}.");
                return;
            }

            s_preload = AssetBundle.LoadFromFileAsync(path);
        }

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

            // Reading .assetBundle on a request that has not finished blocks until it has, which is
            // exactly the wanted behaviour: whoever asked first pays whatever is left of the read
            // rather than starting a second one.
            s_bundle = s_preload != null ? s_preload.assetBundle : AssetBundle.LoadFromFile(path);
            s_preload = null;

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
        /// Whether this exact clip came out of the Knight bundle.
        /// <para>
        /// Identity, not name. Silksong reuses a great deal of Hollow Knight's audio library, so a
        /// clip called <c>hero_fireball</c> or <c>explosion_4_wet</c> playing in a room proves
        /// nothing on its own - the question is always whether it is <i>our</i> copy, which is the
        /// difference between a bug this mod caused and a sound the room was always going to make.
        /// </para>
        /// </summary>
        internal static bool IsBundleAudio(AudioClip clip)
        {
            if (clip == null || s_bundle == null)
            {
                return false;
            }

            return EnsurePrefabAudioMap().TryGetValue(clip.name, out var known) && ReferenceEquals(known, clip);
        }

        /// <summary>
        /// The clips reachable through the loaded prefabs' AudioSources. Deliberately without the
        /// global fallback scan, which would pull in Silksong's own clips and make
        /// <see cref="IsBundleAudio"/> answer yes for sounds that are nothing to do with us.
        /// </summary>
        private static Dictionary<string, AudioClip> EnsurePrefabAudioMap()
        {
            if (s_audio != null)
            {
                return s_audio;
            }

            s_audio = new Dictionary<string, AudioClip>();
            foreach (var prefab in s_prefabs.Values)
            {
                foreach (var source in prefab.GetComponentsInChildren<AudioSource>(true))
                {
                    var clip = source != null ? source.clip : null;
                    if (clip != null && !string.IsNullOrEmpty(clip.name))
                    {
                        s_audio[clip.name] = clip;
                    }
                }
            }

            return s_audio;
        }

        /// <summary>
        /// One of Hollow Knight's own sounds by name, or null.
        /// <para>
        /// <c>LoadAllAssets&lt;AudioClip&gt;()</c> returns none of these, which is what led to the
        /// wrong conclusion that the bundle ships no audio: it holds 162 clips, but every one of them
        /// is a dependency of a prefab rather than an asset of the bundle in its own right, and
        /// <c>LoadAllAssets</c> only returns the latter. They are reachable through the components
        /// that reference them instead. The <c>AudioSource</c> pass is authoritative - those are
        /// native components, so Unity always deserialises them. The global pass is the fallback for
        /// clips a script or FSM holds, which only load if their owning script bound to a real type.
        /// </para>
        /// </summary>
        internal static AudioClip? FindAudioClip(string name)
        {
            if (string.IsNullOrEmpty(name) || !TryLoad())
            {
                return null;
            }

            var audio = EnsurePrefabAudioMap();

            if (audio.TryGetValue(name, out var found))
            {
                return found;
            }

            if (!s_audioScannedGlobally)
            {
                s_audioScannedGlobally = true;
                foreach (var clip in Resources.FindObjectsOfTypeAll<AudioClip>())
                {
                    if (clip != null && !string.IsNullOrEmpty(clip.name) && !audio.ContainsKey(clip.name))
                    {
                        audio[clip.name] = clip;
                    }
                }

                return audio.TryGetValue(name, out var late) ? late : null;
            }

            return null;
        }

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

            // From the prefabs' AudioSources, not LoadAllAssets: see FindAudioClip for why the
            // latter reports none.
            var audio = new List<string>();
            foreach (var prefab in s_prefabs.Values)
            {
                foreach (var source in prefab.GetComponentsInChildren<AudioSource>(true))
                {
                    var clip = source != null ? source.clip : null;
                    if (clip != null && !string.IsNullOrEmpty(clip.name) && !audio.Contains(clip.name))
                    {
                        audio.Add(clip.name);
                    }
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
        /// A still from the Knight's idle animation, for the Characters menu.
        /// </summary>
        internal static Sprite? TryBuildIdlePreview() => TryBuildSprite(IdleClipName, 0);

        /// <summary>
        /// How many frames a bundled clip has, or zero when it is not there. Needed because
        /// <see cref="TryBuildSprite"/> clamps its index, so walking a clip by asking for frames
        /// until one comes back null would never end.
        /// </summary>
        internal static int GetClipFrameCount(string clipName)
        {
            if (!TryLoad())
            {
                return 0;
            }

            var clip = FindClip(clipName);
            return clip?.frames?.Length ?? 0;
        }

        /// <summary>
        /// Frames the atlas stores turned 90 degrees, by the key <see cref="TryBuildSprite"/> caches
        /// them under. tk2d packs sprites rotated when it saves space, and records that in the
        /// definition's <c>flipped</c> flag; a Unity Sprite has nowhere to carry it, so whoever draws
        /// the frame has to turn it back. The HUD masks arrived on their side because of this.
        /// </summary>
        private static readonly HashSet<string> s_rotatedSprites = new();

        internal static bool IsSpriteRotated(string clipName, int frameIndex)
            => s_rotatedSprites.Contains(SpriteKey(clipName, frameIndex));

        private static string SpriteKey(string clipName, int frameIndex)
            => clipName + "#" + frameIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);

        private static readonly Dictionary<string, Sprite?> s_builtSprites = new();

        /// <summary>
        /// One frame of a bundled animation, as a Unity <see cref="Sprite"/>.
        /// <para>
        /// tk2d does not store sprites: a frame is a rectangle of an atlas described by its UV
        /// corners, so this reads those back out and cuts a Sprite from the same texture. Results
        /// are cached because callers are UI code that asks per rebuild, and every call would
        /// otherwise leak another Sprite object over the same pixels.
        /// </para>
        /// </summary>
        internal static Sprite? TryBuildSprite(string clipName, int frameIndex)
        {
            string key = SpriteKey(clipName, frameIndex);
            if (s_builtSprites.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var built = BuildSprite(clipName, frameIndex);
            s_builtSprites[key] = built;
            return built;
        }

        /// <summary>
        /// A sprite cut from a whole bundled texture, for art that belongs to no animation.
        /// <para>
        /// The soul orb's filled interior is one of these: it is a standalone 130x125 texture rather
        /// than a frame of anything, so <see cref="TryBuildSprite"/> cannot reach it.
        /// </para>
        /// </summary>
        internal static Sprite? TryBuildSpriteFromTexture(string textureName)
        {
            string key = "tex:" + textureName;
            if (s_builtSprites.TryGetValue(key, out var cached))
            {
                return cached;
            }

            Sprite? built = null;
            if (TryLoad())
            {
                // A loaded-object scan rather than LoadAllAssets: bundle textures hang off materials
                // and sprites rather than being assets of the bundle in their own right, for the
                // same reason its audio does. Once per name, then cached.
                foreach (var texture in Resources.FindObjectsOfTypeAll<Texture2D>())
                {
                    if (texture == null || !string.Equals(texture.name, textureName, System.StringComparison.Ordinal))
                    {
                        continue;
                    }

                    built = Sprite.Create(
                        texture,
                        new Rect(0f, 0f, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f));
                    built.name = "Knight_" + textureName;
                    built.hideFlags = HideFlags.HideAndDontSave;
                    break;
                }

                if (built == null)
                {
                    LegacyHelper.LogWarning($"Knight bundle has no texture '{textureName}'; whatever asked for it falls back to the shipped art.");
                }
            }

            s_builtSprites[key] = built;
            return built;
        }

        private static Sprite? BuildSprite(string clipName, int frameIndex)
        {
            if (!TryLoad())
            {
                return null;
            }

            ApplyShaders();

            var clip = FindClip(clipName);
            if (clip?.frames == null || clip.frames.Length == 0)
            {
                LegacyHelper.LogWarning($"Knight bundle has no '{clipName}' clip; whatever asked for it falls back to the shipped art.");
                return null;
            }

            var frame = clip.frames[Mathf.Clamp(frameIndex, 0, clip.frames.Length - 1)];
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

            if (definition.flipped != tk2dSpriteDefinition.FlipMode.None)
            {
                s_rotatedSprites.Add(SpriteKey(clipName, frameIndex));
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
            sprite.name = "Knight_" + clipName;
            sprite.hideFlags = HideFlags.HideAndDontSave;
            return sprite;
        }

        /// <summary>
        /// A clip by name from anywhere in the bundle. Every animator in the rig shares one library,
        /// but the HUD's clips live on their own prefabs, so this looks past the Knight itself.
        /// </summary>
        private static tk2dSpriteAnimationClip? FindClip(string clipName)
        {
            foreach (var prefab in s_prefabs.Values)
            {
                foreach (var animator in prefab.GetComponentsInChildren<tk2dSpriteAnimator>(true))
                {
                    var clip = animator.Library != null ? animator.Library.GetClipByName(clipName) : null;
                    if (clip?.frames != null && clip.frames.Length > 0)
                    {
                        return clip;
                    }
                }
            }

            return null;
        }

        internal static void Unload()
        {
            s_prefabs.Clear();
            s_builtSprites.Clear();
            s_rotatedSprites.Clear();
            s_preload = null;
            s_audio = null;
            s_audioScannedGlobally = false;
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
