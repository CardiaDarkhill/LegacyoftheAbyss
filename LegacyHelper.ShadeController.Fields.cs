#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LegacyoftheAbyss.Shade;
using LegacyoftheAbyss.Shade.Ai;
using UnityEngine;
using GlobalEnums;

public partial class LegacyHelper
{
    public partial class ShadeController : MonoBehaviour
    {
        private static readonly ShadeCharmStatBaseline s_defaultCharmStats = ShadeCharmStatBaseline.CreateDefault();

        private ShadeCharmLoadoutSnapshot charmSnapshot = ShadeCharmLoadoutSnapshot.FromBaseline(s_defaultCharmStats);
        private readonly List<ShadeCharmDefinition> equippedCharms = new List<ShadeCharmDefinition>();
        // Paired with the charm they came from, so a hook that throws can be named. A bare
        // callback list meant "this charm does nothing" and "this charm throws on its first line"
        // produced identical silence, which cost several rounds on Thorns of Agony.
        private readonly List<(string Charm, Action<ShadeCharmContext, float> Callback)> charmUpdateCallbacks = new List<(string, Action<ShadeCharmContext, float>)>();
        private readonly List<(string Charm, Action<ShadeCharmContext, ShadeCharmDamageEvent> Callback)> charmDamageCallbacks = new List<(string, Action<ShadeCharmContext, ShadeCharmDamageEvent>)>();

        /// <summary>Charm hooks already reported as throwing, so each is logged once, not per hit.</summary>
        private readonly HashSet<string> reportedCharmHookFailures = new HashSet<string>();
        private ShadeCharmAbilityToggles abilityOverrides = ShadeCharmAbilityToggles.None;

        // Movement and leash
        private float moveSpeed = s_defaultCharmStats.MoveSpeed;
        private float sprintMultiplier = s_defaultCharmStats.SprintMultiplier;
        public float maxDistance = 14f;
        public float softLeashRadius = 12f;
        public float hardLeashRadius = 22f;
        public float snapLeashRadius = 38f;
        public float softPullSpeed = 6f;
        public float hardPullSpeed = 30f;
        public float hardLeashTimeout = 2.5f;
        private bool inHardLeash;
        private float hardLeashTimer;

        // Dialogue / bench / cutscene docking. One flag, three consumers - the movement state machine below,
        // the Shade HUD (SimpleHUD reads ShadeController.HornetControlsLocked), and the combat gate
        // in Update. See HornetControlsLocked for what actually sets it.
        private bool hornetControlsLocked;

        /// <summary>Shade is off screen for a scripted hold - see <c>ApplyScriptedHoldVisibility</c>.</summary>
        private bool hiddenForScriptedHold;
        /// <summary>How far to Hornet's side the Shade docks while her controls are locked.</summary>
        public float dockOffsetX = 1.6f;
        /// <summary>Vertical offset of the docked position, so the Shade floats rather than stands.</summary>
        public float dockOffsetY = 0.9f;
        /// <summary>Approach speed toward the docked position, in units/second.</summary>
        public float dockApproachSpeed = 12f;
        private Rigidbody2D rb;
        private Collider2D bodyCol;
        private AggroProxyTracker aggroProxyTracker;
        private int shadeMaxHP;
        private int shadeHP;
        private int shadeLifeblood;
        private int shadeLifebloodMax;
        private int pendingRestoredLifeblood = -1;
        private int pendingRestoredLifebloodMax = -1;
        private float hazardCooldown;
        private float baseMaxDistance, baseSoftLeashRadius, baseHardLeashRadius, baseSnapLeashRadius;
        private float baseSprintMultiplier;
        private float baseFireCooldown;
        private float baseNailCooldown;
        private int baseFocusSoulCost;
        private int baseProjectileSoulCost;
        private int baseShriekSoulCost;
        private int baseQuakeSoulCost;
        private bool baselineStatsInitialized;
        private bool pendingCharmLoadoutRecompute;
        private bool wasInactive;
        public float hitKnockbackForce = 6f;
        private Vector2 knockbackVelocity;
        private float knockbackTimer;
        private BattleScene cachedBattle;
        private float battleCheckTimer;

        private static readonly string[] IgnoreDamageTokens =
            {"alert range", "attack range", "wake", "close range", "sight range", "terrain", "range", "physics pusher", "bounce collider", "emerge check"};

        // Ranged attack
        public float projectileSpeed = 22f;
        private float fireCooldown = s_defaultCharmStats.FireCooldown;
        private float nailCooldown = s_defaultCharmStats.NailCooldown;
        public Vector2 muzzleOffset = new Vector2(0.9f, 0f);

        private Transform hornetTransform;

        /// <summary>Hornet, for anything spawned by this companion that must not hit her.</summary>
        internal Transform HornetRoot => hornetTransform;
        private float fireTimer;
        private SpriteRenderer sr;
        private float _spriteScale = 1.5f;
        public float SpriteScale
        {
            get => _spriteScale;
            set
            {
                _spriteScale = value;
                if (transform != null)
                    transform.localScale = Vector3.one * _spriteScale;
            }
        }
        private Sprite[] idleAnimFrames;
        private Sprite[] floatAnimFrames;
        private Sprite[] vengefulAnimFrames;
        private Sprite[] shadeSoulAnimFrames;
        private Sprite[] fireballCastAnimFrames;
        private Sprite[] quakeCastAnimFrames;
        private Sprite[] shriekCastAnimFrames;
        private Sprite[] abyssShriekAnimFrames;
        private Sprite[] howlingWraithsAnimFrames;
        private Sprite[] deathAnimFrames;
        private Sprite[] descendAnimFrames;
        private Sprite[] descendAuraAnimFrames;
        private Sprite[] dDiveSlamAnimFrames;
        private Sprite[] dDarkSlamAnimFrames;
        private Sprite[] dDarkBurstAnimFrames;
        private Sprite[] baldurShellFocusAnimFrames;
        private Sprite inactiveSprite;
        private SpriteRenderer inactivePulseSr;
        private Sprite[] currentAnimFrames;
        // Every texture decoded by LoadSpriteStrip for the skin currently in use, so a skin
        // switch can free the outgoing set instead of leaking ~20 MB per swap.
        private readonly List<Texture2D> loadedSpriteTextures = new List<Texture2D>();
        private string loadedSkinId;
        private const float RetiredSkinTextureLifetime = 3f;
        private int animFrameIndex;
        private float animTimer;
        private Coroutine spawnRoutine;
        private bool pendingSpawnAnimation;
        private bool isSpawning;
        private const float AnimFrameTime = 0.1f;
        private const float BaldurShellFrameTime = 0.08f;
        private Vector2 lastMoveDelta;
        private SpriteRenderer[] shadeLightRenderers = System.Array.Empty<SpriteRenderer>();
        private SpriteRenderer[] shadeLightSourceRenderers = System.Array.Empty<SpriteRenderer>();
        private Transform[] shadeLightRoots = System.Array.Empty<Transform>();
        private Vector3[] shadeLightRootBaseScales = System.Array.Empty<Vector3>();
        private float heroLightRadius;
        private Vector3 shadeLightBaseScale = Vector3.one;
        private static Texture2D s_simpleLightTex;
        private static Material s_simpleAdditiveMat;
        private static Mesh s_simpleQuadMesh;
        private static Material s_sprintBurstMat;
        private int facing = 1;

        /// <summary>Which way this companion is drawn, for anything that has to sit beside it.</summary>
        internal int Facing => facing;
        private float nailTimer;

        /// <summary>
        /// The swing, timed separately from the cooldown. Hollow Knight refuses the next strike
        /// until both have run out, and lets a turn or a dash end this one early once the cooldown
        /// already has - which is where the first game's faster attack rates come from.
        /// </summary>
        private float nailDurationTimer;

        private float nailDuration = s_defaultCharmStats.NailDuration;

        /// <summary>Last frame's facing, so a turn can be noticed without every mover reporting one.</summary>
        private int nailLastFacing = 1;
        internal static bool suppressActivateOnSlash;
        internal static Transform expectedSlashParent;
        private SpriteRenderer baldurShellRenderer;
        private Coroutine baldurShellRoutine;
        private bool baldurShellActive;
        private int baldurShellFrameIndex;

        private struct AxisLeashLimits
        {
            public float NegativeSoft;
            public float PositiveSoft;
            public float NegativeHard;
            public float PositiveHard;
            public float NegativeSnap;
            public float PositiveSnap;
        }

        private struct DynamicLeashLimits
        {
            public AxisLeashLimits X;
            public AxisLeashLimits Y;
        }

        private const float LeashScreenPadding = 0.75f;
        private const float SoftLimitRatio = 0.9f;
        private const float SnapExtraMultiplier = 1.2f;
        private const float SnapExtraMin = 0.75f;
        private const float SnapMinWhenNoRoom = 0.25f;

        private bool canTakeDamage = true;
        private bool assistModeEnabled;
        private float sceneProtectionTimer;
        private bool sceneProtectionActive;
        private bool sceneProtectionDesiredDamageState = true;
        private bool sceneProtectionSuppressingPersistence;
        private readonly Collider2D[] sceneProtectionOverlapBuffer = new Collider2D[16];
        private Vector2 capturedMoveInput;
        private float capturedHorizontalInput;
        private bool capturedSprintHeld;
        private float damageStaggerTimer;
        private float damageStaggerDurationMultiplier = 1f;
        private const float DamageStaggerBaseDuration = 0.2f;
        // Spells use Fire + Up (Shriek) or Fire + Down (Descending Dark)

        // Teleport channel
        private bool isChannelingTeleport;
        private float teleportChannelTimer;
        public float teleportChannelTime = 0.6f;
        private float teleportCooldownTimer;
        private float teleportCooldown = s_defaultCharmStats.TeleportCooldown;

        private bool sprintUnlocked;
        private bool isSprinting;
        private float sprintDashTimer;
        private float sprintDashCooldownTimer;
        private float sprintDashMultiplier = s_defaultCharmStats.SprintDashMultiplier;
        private float sprintDashDuration = s_defaultCharmStats.SprintDashDuration;
        private float sprintDashCooldown = s_defaultCharmStats.SprintDashCooldown;
        private ParticleSystem activeDashPs;
        private Vector2 activeDashDir;
        private bool voidHeartEvadeActive;
        private bool sharpShadowEquipped;

        /// <summary>Drives the Knight's Sprintmaster walk cycle; the speed comes from the snapshot.</summary>
        private bool sprintmasterEquipped;

        /// <summary>Whether the nail throws a Grubberfly beam alongside its swing.</summary>
        private bool grubberflyElegyEquipped;

        /// <summary>
        /// Shaman Stone, as a plain flag. Its multiplier is not a substitute: Flukenest's damage is
        /// a fixed pair of numbers rather than a scaling, so it has to ask for the charm by name.
        /// </summary>
        private bool shamanStoneEquipped;

        /// <summary>
        /// Whether Fury of the Fallen is presently paying out. Set alongside the aura, so the one
        /// answer serves both the effect and anything that keys off the same "last mask" state.
        /// </summary>
        private bool furyModeActive;
        private bool sharpShadowDashActive;
        private GameObject sharpShadowDashHitbox;
        private ShadeAoE sharpShadowDashAoE;

        private GameObject furyAuraObject;
        private ParticleSystem furyAuraPs;
        private static Material s_furyAuraMat;

        // Shadow-wisp trail (LegacyHelper.ShadeController.ShadowParticles.cs). The texture and
        // material are shared across every Shade for the process lifetime, like the light quad above.
        private GameObject shadowParticleObject;
        private ParticleSystem shadowParticlePs;
        private ParticleSystemRenderer shadowParticleRenderer;
        private static Texture2D s_shadowWispTex;
        private static Material s_shadowWispMat;
        // Slewed toward the real SOUL fraction so a spell cast thins the smoke over ~a second
        // instead of snapping. appliedShadow* are the values currently pushed at the emitter.
        private float shadowSoulFraction;
        private float appliedShadowSoulFraction = -1f;
        private float appliedShadowIntensity = -1f;

        // Inactive state (at 0 HP)
        private bool isInactive;
        internal bool IsAggroEligible => !isInactive && isActiveAndEnabled && !assistModeEnabled;

        /// <summary>
        /// Every Shade currently in the scene, in spawn order. Maintained by <c>Start</c>/<c>OnDestroy</c>
        /// so per-frame callers (notably <see cref="ShadeAggroTargeting"/>, which runs off enemy AI
        /// actions) don't have to scan the scene to find them.
        /// </summary>
        private static readonly List<ShadeController> s_activeInstances = new List<ShadeController>();

        internal static IReadOnlyList<ShadeController> ActiveInstances => s_activeInstances;

        /// <summary>
        /// The primary companion's Shade, or null. Only for queries that genuinely concern one Shade
        /// — a broadcast (a setting change, an enemy picking a target) must walk
        /// <see cref="ActiveInstances"/> instead, or it silently ignores every Shade but the first.
        /// </summary>
        internal static ShadeController PrimaryInstance
            => ShadeCompanionRegistry.Primary.Controller;

        /// <summary>The companion whose state this controller reads and writes.</summary>
        internal ShadeCompanion Companion { get; private set; }

        internal void BindCompanion(ShadeCompanion companion)
        {
            Companion = companion;
        }

        /// <summary>
        /// This Shade's own charms. Companions equip independently, so anything scaling this Shade's
        /// stats or damage must read here rather than from <see cref="ShadeRuntime.Charms"/>, which
        /// is the primary's. Falls back to the primary for a controller spawned outside the registry.
        /// </summary>
        private ShadeCharmInventory OwnCharms => Companion?.Charms ?? ShadeRuntime.Charms;

        /// <summary>
        /// Whether this companion is currently drawn, whichever renderer draws it. The Shade uses
        /// its sheet renderer; the Knight's is disabled in favour of its own rig, so anything
        /// following the body's visibility has to ask here rather than read <c>sr.enabled</c>.
        /// </summary>
        private bool CompanionVisible => UsesGroundedMovement
            ? knightView != null && knightView.IsVisible
            : sr && sr.enabled;
        private bool isDying;
        private Coroutine deathRoutine;

        // Shade Soul resource
        public int shadeSoulMax = s_defaultCharmStats.ShadeSoulCapacity;
        public int shadeSoul;
        public int soulGainPerHit = 11;
        private int baseSoulGainPerHit = 11;
        private int charmSoulGainBonus;
        private float charmNailDamageMultiplier = 1f;
        private float charmNailScaleMultiplier = 1f;
        private float charmNailKnockbackMultiplier = 1f;
        private int projectileSoulCost = s_defaultCharmStats.ProjectileSoulCost;
        private int shriekSoulCost = s_defaultCharmStats.ShriekSoulCost;
        private int quakeSoulCost = s_defaultCharmStats.QuakeSoulCost;
        private float shriekTimer;
        private float quakeTimer;
        private float shriekCooldown = s_defaultCharmStats.ShriekCooldown;
        private float quakeCooldown = s_defaultCharmStats.QuakeCooldown;

        // Focus (heal) ability
        private int focusSoulCost = s_defaultCharmStats.FocusSoulCost;
        public float focusChannelTime = 1.25f;
        private bool isFocusing;
        private float focusTimer;
        private float focusAlphaWhileChannel = 0.75f;
        private float focusHealRange = 6f;
        private float focusSoulAccumulator;

        /// <summary>
        /// SOUL this channel has already spent. The drain has to be able to tell "the meter ran
        /// dry because something else spent it" from "this channel spent the last of it", and the
        /// remaining total cannot: a full meter is an exact multiple of the cost, so the last heal
        /// legitimately ends on zero.
        /// </summary>
        private int focusSoulDrainedThisChannel;
        private Renderer focusAuraRenderer;
        private float focusAuraBaseSize = 12f;
        private bool focusDamageShieldEnabled;
        private bool focusDamageShieldAbsorbedThisChannel;
        private bool focusHealingDisabled;
        private bool carefreeMelodyEquipped;
        private float carefreeMelodyChance;
        private GameObject carefreeMelodyShieldEffect;
        private const float CarefreeMelodyChanceStep = 0.1f;
        private AudioSource focusSfx;
        private AudioClip sfxFocusCharge;
        private AudioClip sfxFocusComplete;
        private AudioClip sfxFocusReady;
        // Guards against re-running the filesystem probe + full loaded-object walk on
        // every cast when a clip genuinely cannot be resolved.
        private bool searchedFocusSfx;
        private bool searchedSpellSfx;
        private int lastSoulForReady = -1;

        private float baseFocusChannelTime;
        private float baseFocusHealRange;
        private float baseTeleportChannelTime;
        private float baseHitKnockbackForce;
        private int baseShadeMaxHP;

        /// <summary>
        /// Combined health the Shade had when the game was paused, or -1 while unpaused. The mask
        /// fraction setting resizes the live Shade as the player steps it, so this is what a resize
        /// restores from - see <see cref="RefreshDerivedMaskCount"/>.
        /// </summary>
        private int pausedHealthBaseline = -1;

        private int charmFocusHealBonus;
        private int charmHornetFocusHealBonus;
        private float charmFocusTimeMultiplier = 1f;
        private float charmTeleportChannelMultiplier = 1f;
        private float charmHurtIFrameMultiplier = 1f;
        private float currentHurtIFrameDuration = HurtIFrameSeconds;
        private int charmMaxHpBonus;
        private int charmLifebloodBonus;
        private bool jonisBlessingEquipped;
        private bool hivebloodPendingLifebloodRestore;
        private bool allowFocusMovement;
        private int knockbackSuppressionCount;
        private readonly Dictionary<string, float> conditionalNailDamageMultipliers = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
        private float conditionalNailDamageProduct = 1f;

        private SimpleHUD cachedHud;
        private bool pendingHudStatsSync;
        private bool pendingHudAssistSync;
        private bool pendingHudOvercharmSync;
        private bool pendingHudSoulSync;
        private bool pendingHudSuppressDamageSfx;
        private Coroutine hudSyncRoutine;
        private float hurtCooldown;
        private const float HurtIFrameSeconds = 1.35f;
        private const float ReviveIFrameSeconds = 1.5f;
        private float ignoreRefreshTimer;
        private float hornetIgnoreRefreshTimer;
        private bool isCastingSpell;

        // Shade AI. The decision itself lives in LegacyoftheAbyss.Shade.Ai; these are only the
        // driver's own state, and the driver is LegacyHelper.ShadeController.Ai.cs.
        private readonly ShadeAiBrain aiBrain = new ShadeAiBrain();
        private readonly ShadeAiTargetScanner aiScanner = new ShadeAiTargetScanner();
        private readonly ShadeAiNavigator aiNavigator = new ShadeAiNavigator();
        private bool aiEnabled;
        /// <summary>
        /// When the AI last published a slash. Its own governor, separate from <c>nailTimer</c>: the
        /// cooldown is what the game permits, this is the slower rate the AI is actually allowed.
        /// </summary>
        private float aiLastNailTime;
        /// <summary>Last computed SOUL reserve, kept only so the event line can report it without recomputing.</summary>
        private int aiSoulReserve;
        private readonly List<ShadeAiThreat> aiThreats = new List<ShadeAiThreat>();
        private readonly Collider2D[] aiThreatBuffer = new Collider2D[32];
        private ContactFilter2D aiThreatFilter;
        private bool aiThreatFilterReady;
        private int aiThreatCount;
        /// <summary>
        /// How many times running the navigator has given up on a route and turned round. A standing
        /// order that racks these up is one the Shade cannot honour, so it gets dropped rather than
        /// left grinding against whatever is in the way.
        /// </summary>
        private int aiStuckStreak;
        /// <summary>
        /// Keeps Hornet reading as airborne for a moment after she lands. Without it a run over
        /// broken ground toggles onGround several times a second and the Shade swaps escort corners
        /// with it, which looks like a bug even though each individual decision is correct.
        /// </summary>
        private float aiHornetAirborneHold;
        /// <summary>
        /// Hornet's airborne state as of this frame, held briefly past a landing. Read by the escort
        /// placement and by the leash, so it is resolved once a frame and shared rather than sampled
        /// twice with a timer that would decay at double rate.
        /// </summary>
        private bool aiHornetAirborne;
        private int aiHornetAirborneFrame = -1;

        // Shade command reticle (LegacyHelper.ShadeController.AiCommand.cs).
        private ShadeAiCommandState aiCommandState;
        /// <summary>The spot the Shade has been ordered to hold. Only meaningful while Holding.</summary>
        private Vector2 aiCommandPoint;
        /// <summary>
        /// How far the leash must reach for the current order to be keepable - the distance the order
        /// was placed at, plus slack. Zero when no order is held.
        /// </summary>
        private float aiCommandLeashFloor;
        /// <summary>Live reticle position while aiming.</summary>
        private Vector2 aiReticlePoint;
        /// <summary>Whether the reticle moved before it was confirmed - the only thing separating
        /// "stay where you are" from "go over there", and kept for the event log rather than for
        /// behaviour, since both end as a hold at the reticle.</summary>
        private bool aiReticleMoved;
        private Vector3 aiReticleMouseAnchor;
        private GameObject aiReticleObject;
        private SpriteRenderer aiReticleRenderer;
        private ShadeAiPlan aiPlan;
        private bool aiEngaged;
        /// <summary>
        /// Which way the AI wants the Shade to face, or 0 to leave facing to movement. Read by
        /// <c>HandleMovementAndFacing</c>, which would otherwise turn the Shade away from its target
        /// on the last half-unit of the approach - the strike point sits between the Shade and the
        /// enemy, so closing the final gap means briefly moving away from what it is about to hit.
        /// </summary>
        private int aiFacingOverride;
        private int aiTargetCount;
        private int aiLastEventKey;
        private float aiLastEventTime;

        private int lastSavedHP;
        private int lastSavedMax;
        private int lastSavedLifeblood;
        private int lastSavedLifebloodMax;
        private int lastSavedSoul;
        private int lastSavedVesselSoul;
        private bool lastSavedCanTakeDamage = true;
        private int persistenceSuppressionDepth;
        private bool pendingDeferredHealthSync;
        private bool pendingDeferredHealthSuppressDamage;
        private bool applyingCharmLoadout;
    }
}
