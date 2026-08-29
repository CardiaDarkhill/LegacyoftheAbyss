using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LegacyoftheAbyss.Diagnostics
{
    /// <summary>Shade-side detail for <see cref="BugReportState"/>. Populated by <c>ShadeController.CaptureDiagnostics</c>.</summary>
    internal sealed class BugReportShadeState
    {
        public bool Present;
        public float X;
        public float Y;
        public float VelocityX;
        public float VelocityY;
        public int Hp;
        public int MaxHp;
        public int BaseMaxHp;
        public int Lifeblood;
        public int LifebloodMax;
        public int Soul;
        public int SoulMax;
        public bool CanTakeDamage;
        public bool AssistMode;
        public int Facing;
        public string? Flags;
        public string? Skin;
        public string[]? EquippedCharms;
        public int NotchesUsed;
        public int NotchCapacity;
        public float HardLeashTimer;
        public float HazardCooldown;
        public float SceneProtectionTimer;
        public float TeleportCooldownTimer;
        public float FireTimer;
        public float NailTimer;
        public float FocusTimer;
        public float MoveSpeed;
        public bool AiEnabled;
        public string? AiReason;
        public string? AiAction;
        public int AiTargetId;
        public int AiTargetsInRange;
        public string? AiCommandState;
        public float AiCommandX;
        public float AiCommandY;
    }

    /// <summary>Hornet-side detail for <see cref="BugReportState"/>.</summary>
    internal sealed class BugReportHeroState
    {
        public bool Present;
        public float X;
        public float Y;
        public string? ActorState;
        public string? Flags;
        public bool ControlRelinquished;
        public int Health;
        public int MaxHealth;
        public int Silk;
        public int SilkMax;
        public int Rosaries;
        public int ProfileId;
        public string? RespawnScene;
    }

    internal sealed class BugReportPluginEntry
    {
        public string? Guid;
        public string? Name;
        public string? Version;
    }

    /// <summary>
    /// The point-in-time half of a bug report: everything that describes where the game was when the
    /// report was filed, as opposed to the log ring and flight recorder which describe how it got there.
    /// <para>
    /// Serialised to <c>state.json</c>. Deliberately a flat, plain-old-object model rather than a live
    /// object graph - reports are read back weeks later with no running game, so every value has to be
    /// resolved and copied at capture time.
    /// </para>
    /// </summary>
    internal sealed class BugReportState
    {
        public string? Title;
        public string? Message;
        public string? Trigger;
        public string? ReportId;

        public string? CapturedUtc;
        public string? CapturedLocal;
        public float Realtime;
        public int FrameCount;
        public float TimeScale;

        public string? ModVersion;
        public string? GameVersion;
        public string? UnityVersion;
        public string? Platform;
        public string? Resolution;
        public bool Fullscreen;

        public string? Scene;
        public string[]? SceneHistory;
        public bool Paused;
        public string? GameState;

        public BugReportHeroState? Hero;
        public BugReportShadeState? Shade;

        public string? ExceptionMessage;
        public string? ExceptionStackTrace;

        public BugReportPluginEntry[]? LoadedPlugins;
        public object? Config;

        /// <summary>
        /// Which row the settings menu cloned for its sliders, or why it could not find one.
        /// <para>
        /// Snapshotted rather than left to the log: the menu is built within a couple of seconds of
        /// launch and the log ring keeps only the last few hundred lines, so by the time anyone
        /// reports that the sliders look wrong, the line naming the template has long since aged out.

        /// </para>
        /// </summary>
        public string? MenuSliderTemplate;

        /// <summary>
        /// Why the co-op camera lean is or is not moving the shot. Every stage of it can decline
        /// for a legitimate reason, and from outside those are indistinguishable from it being
        /// broken - so the reason travels with the report rather than being guessed at.
        /// </summary>
        public string? CoopCamera;

        /// <summary>
        /// What the Knight's asset bundle turned out to contain. Recorded here rather than only
        /// logged because it is written once at first load and the log ring does not keep it.
        /// </summary>
        public string? KnightBundle;

        /// <summary>Which Knight sound each effect resolved to, or MISSING.</summary>
        public string? KnightAudio;
    }

    /// <summary>
    /// Reads the live game into a <see cref="BugReportState"/>.
    /// <para>
    /// Every field is fetched behind its own try/catch. That looks defensive to the point of noise, but
    /// a bug report is most valuable precisely when the game is already in a broken state - half the
    /// time this runs, something in the object graph it is walking is null or throwing, and a snapshot
    /// that captured twenty of thirty fields beats one that threw on the third.
    /// </para>
    /// </summary>
    internal static class BugReportStateCollector
    {
        private static readonly Dictionary<Type, FieldInfo[]> s_boolFieldCache = new Dictionary<Type, FieldInfo[]>();

        internal static BugReportState Capture(string trigger, string? title, string? message)
        {
            var state = new BugReportState
            {
                Trigger = trigger,
                Title = title,
                Message = message,
                CapturedUtc = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture),
                CapturedLocal = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
            };

            TryRun(() => state.Realtime = Time.realtimeSinceStartup);
            TryRun(() => state.FrameCount = Time.frameCount);
            TryRun(() => state.TimeScale = Time.timeScale);

            TryRun(() => state.ModVersion = LegacyoftheAbyss.PluginInfo.PLUGIN_VERSION);
            TryRun(() => state.GameVersion = Application.version);
            TryRun(() => state.UnityVersion = Application.unityVersion);
            TryRun(() => state.Platform = Application.platform.ToString());
            TryRun(() => state.Resolution = Screen.width.ToString(CultureInfo.InvariantCulture) + "x" + Screen.height.ToString(CultureInfo.InvariantCulture));
            TryRun(() => state.Fullscreen = Screen.fullScreen);

            TryRun(() => state.Scene = SceneManager.GetActiveScene().name);
            TryRun(() => state.MenuSliderTemplate = ShadeSettingsMenu.LastSliderTemplateDescription);
            TryRun(() => state.CoopCamera = LegacyHelper.CompanionCameraBias.DescribeState());
            TryRun(() => state.KnightBundle = LegacyoftheAbyss.Shade.Knight.KnightAssets.Inventory);
            TryRun(() => state.KnightAudio = LegacyoftheAbyss.Shade.Knight.KnightAudio.Report);

            var gameManager = MenuStateUtility.TryGetGameManager();
            if (gameManager != null)
            {
                TryRun(() => state.Paused = gameManager.isPaused);
                TryRun(() => state.GameState = gameManager.GameState.ToString());
            }

            state.Hero = CaptureHero(gameManager);
            state.Shade = CaptureShade();
            state.LoadedPlugins = CapturePlugins();
            TryRun(() => state.Config = ModConfig.Instance);

            return state;
        }

        private static BugReportHeroState CaptureHero(GameManager? gameManager)
        {
            var hero = new BugReportHeroState();
            HeroController? controller = null;
            TryRun(() => controller = gameManager != null ? gameManager.hero_ctrl : null);
            if (controller == null)
            {
                return hero;
            }

            hero.Present = true;
            TryRun(() =>
            {
                Vector3 position = controller.transform.position;
                hero.X = position.x;
                hero.Y = position.y;
            });
            TryRun(() => hero.ActorState = controller.hero_state.ToString());
            TryRun(() => hero.Flags = DescribeTrueBoolFields(controller.cState));
            TryRun(() => hero.ControlRelinquished = controller.controlReqlinquished);

            PlayerData? playerData = null;
            TryRun(() => playerData = controller.playerData);
            if (playerData == null)
            {
                return hero;
            }

            TryRun(() => hero.Health = playerData.health);
            TryRun(() => hero.MaxHealth = playerData.maxHealth);
            TryRun(() => hero.Silk = playerData.silk);
            TryRun(() => hero.SilkMax = playerData.silkMax);
            TryRun(() => hero.Rosaries = playerData.geo);
            TryRun(() => hero.ProfileId = playerData.profileID);
            TryRun(() => hero.RespawnScene = playerData.respawnScene);
            return hero;
        }

        private static BugReportShadeState CaptureShade()
        {
            try
            {
                if (LegacyHelper.TryGetShadeController(out var controller) && controller != null)
                {
                    return controller.CaptureDiagnostics();
                }
            }
            catch
            {
            }

            return new BugReportShadeState { Present = false };
        }

        private static BugReportPluginEntry[] CapturePlugins()
        {
            // Which other mods were loaded is the first question asked of any report that cannot be
            // reproduced, and it is exactly the detail a human never thinks to include.
            try
            {
                var infos = BepInEx.Bootstrap.Chainloader.PluginInfos;
                if (infos == null)
                {
                    return Array.Empty<BugReportPluginEntry>();
                }

                var entries = new List<BugReportPluginEntry>(infos.Count);
                foreach (var pair in infos)
                {
                    var metadata = pair.Value?.Metadata;
                    entries.Add(new BugReportPluginEntry
                    {
                        Guid = metadata?.GUID ?? pair.Key,
                        Name = metadata?.Name,
                        Version = metadata?.Version?.ToString()
                    });
                }

                entries.Sort((left, right) => string.CompareOrdinal(left.Guid, right.Guid));
                return entries.ToArray();
            }
            catch
            {
                return Array.Empty<BugReportPluginEntry>();
            }
        }

        /// <summary>
        /// Names of the currently-set bool fields on a game state object, joined by a vertical bar.
        /// <para>
        /// Reflection rather than a hand-written list because <c>HeroControllerStates</c> carries about
        /// sixty flags and belongs to the game, not to us: a hardcoded subset would silently stop
        /// reporting whichever new flag a Silksong patch adds, and that flag is disproportionately
        /// likely to be the interesting one. The field array is cached per type, so the per-call cost is
        /// the field reads alone.
        /// </para>
        /// </summary>
        internal static string DescribeTrueBoolFields(object? source)
        {
            if (source == null)
            {
                return string.Empty;
            }

            FieldInfo[]? fields;
            var type = source.GetType();
            lock (s_boolFieldCache)
            {
                if (!s_boolFieldCache.TryGetValue(type, out fields))
                {
                    var matches = new List<FieldInfo>();
                    foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
                    {
                        if (field.FieldType == typeof(bool))
                        {
                            matches.Add(field);
                        }
                    }

                    fields = matches.ToArray();
                    s_boolFieldCache[type] = fields;
                }
            }

            var builder = new StringBuilder();
            foreach (var field in fields)
            {
                try
                {
                    if (field.GetValue(source) is bool value && value)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append('|');
                        }

                        builder.Append(field.Name);
                    }
                }
                catch
                {
                }
            }

            return builder.ToString();
        }

        private static void TryRun(Action action)
        {
            try
            {
                action();
            }
            catch
            {
            }
        }
    }
}
