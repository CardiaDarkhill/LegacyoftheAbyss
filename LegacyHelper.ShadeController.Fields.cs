#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LegacyoftheAbyss.Shade;
using UnityEngine;
using GlobalEnums;

public partial class LegacyHelper
{
    public partial class ShadeController : MonoBehaviour
    {
        private static readonly ShadeCharmStatBaseline s_defaultCharmStats = ShadeCharmStatBaseline.CreateDefault();

        private ShadeCharmLoadoutSnapshot charmSnapshot = ShadeCharmLoadoutSnapshot.FromBaseline(s_defaultCharmStats);
        private readonly List<ShadeCharmDefinition> equippedCharms = new List<ShadeCharmDefinition>();
        private readonly List<Action<ShadeCharmContext, float>> charmUpdateCallbacks = new List<Action<ShadeCharmContext, float>>();
        private readonly List<Action<ShadeCharmContext, ShadeCharmDamageEvent>> charmDamageCallbacks = new List<Action<ShadeCharmContext, ShadeCharmDamageEvent>>();
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
        private Rigidbody2D rb;
        private Collider2D bodyCol;
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
            {"alert range", "attack range", "wake", "close range", "sight range", "terrain", "range", "physics pusher"};

        // Ranged attack
        public float projectileSpeed = 22f;
        private float fireCooldown = s_defaultCharmStats.FireCooldown;
        private float nailCooldown = s_defaultCharmStats.NailCooldown;
        public Vector2 muzzleOffset = new Vector2(0.9f, 0f);

        private Transform hornetTransform;
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
        private Sprite inactiveSprite;
        private SpriteRenderer inactivePulseSr;
        private Sprite[] currentAnimFrames;
        private int animFrameIndex;
        private float animTimer;
        private Coroutine spawnRoutine;
        private bool pendingSpawnAnimation;
        private bool isSpawning;
        private const float AnimFrameTime = 0.1f;
        private Vector2 lastMoveDelta;
        private Renderer[] shadeLightRenderers;
        public float simpleLightSize = 14f;
        private static Texture2D s_simpleLightTex;
        private static Material s_simpleAdditiveMat;
        private static Mesh s_simpleQuadMesh;
        private static Material s_sprintBurstMat;
        private int facing = 1;
        private float nailTimer;
        internal static bool suppressActivateOnSlash;
        internal static Transform expectedSlashParent;

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

        private GameObject furyAuraObject;
        private ParticleSystem furyAuraPs;
        private static Material s_furyAuraMat;

        // Inactive state (at 0 HP)
        private bool isInactive;
        private bool isDying;
        private Coroutine deathRoutine;

        // Shade Soul resource
        public int shadeSoulMax = s_defaultCharmStats.ShadeSoulCapacity;
        public int shadeSoul;
        public int soulGainPerHit = 11;
        private int baseSoulGainPerHit = 11;
        private int charmSoulGainBonus;
        private float charmNailDamageMultiplier = 1f;
        private float charmSpellDamageMultiplier = 1f;
        private float charmNailScaleMultiplier = 1f;
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
        private Renderer focusAuraRenderer;
        private float focusAuraBaseSize = 12f;
        private bool focusDamageShieldEnabled;
        private bool focusDamageShieldAbsorbedThisChannel;
        private bool focusHealingDisabled;
        private float carefreeMelodyChance;
        private AudioSource focusSfx;
        private AudioClip sfxFocusCharge;
        private AudioClip sfxFocusComplete;
        private AudioClip sfxFocusReady;
        private int lastSoulForReady = -1;

        private float baseFocusChannelTime;
        private float baseFocusHealRange;
        private float baseTeleportChannelTime;
        private float baseHitKnockbackForce;
        private int baseShadeMaxHP;

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

        private int lastSavedHP;
        private int lastSavedMax;
        private int lastSavedLifeblood;
        private int lastSavedLifebloodMax;
        private int lastSavedSoul;
        private bool lastSavedCanTakeDamage = true;
        private int persistenceSuppressionDepth;
        private bool pendingDeferredHealthSync;
        private bool pendingDeferredHealthSuppressDamage;
        private bool applyingCharmLoadout;
    }
}
