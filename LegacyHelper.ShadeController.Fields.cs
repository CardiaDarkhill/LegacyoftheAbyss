#nullable disable
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using GlobalEnums;

public partial class LegacyHelper
{
    public partial class ShadeController : MonoBehaviour
    {
        // Movement and leash
        public float moveSpeed = 10f;
        public float sprintMultiplier = 2.5f;
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
        private float hazardCooldown;
        private float baseMaxDistance, baseSoftLeashRadius, baseHardLeashRadius, baseSnapLeashRadius;
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
        public float fireCooldown = 0.25f;
        public float nailCooldown = 0.3f;
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
        private Vector2 capturedMoveInput;
        private float capturedHorizontalInput;
        private bool capturedSprintHeld;
        // Spells use Fire + Up (Shriek) or Fire + Down (Descending Dark)

        // Teleport channel
        private bool isChannelingTeleport;
        private float teleportChannelTimer;
        public float teleportChannelTime = 0.6f;
        private float teleportCooldownTimer;
        public float teleportCooldown = 1.5f;

        private bool sprintUnlocked;
        private bool isSprinting;
        private float sprintDashTimer;
        private float sprintDashCooldownTimer;
        public float sprintDashMultiplier = 7.5f;
        public float sprintDashDuration = 0.075f;
        public float sprintDashCooldown = 1f;
        private ParticleSystem activeDashPs;
        private Vector2 activeDashDir;

        // Inactive state (at 0 HP)
        private bool isInactive;
        private bool isDying;
        private Coroutine deathRoutine;

        // Shade Soul resource
        public int shadeSoulMax = 99;
        public int shadeSoul;
        public int soulGainPerHit = 11;
        public int projectileSoulCost = 33;
        public int shriekSoulCost = 33;
        public int quakeSoulCost = 33;
        private float shriekTimer;
        private float quakeTimer;
        public float shriekCooldown = 0.5f;
        public float quakeCooldown = 1.1f;

        // Focus (heal) ability
        public int focusSoulCost = 33;
        public float focusChannelTime = 1.25f;
        private bool isFocusing;
        private float focusTimer;
        private float focusAlphaWhileChannel = 0.75f;
        private float focusHealRange = 6f;
        private float focusSoulAccumulator;
        private Renderer focusAuraRenderer;
        private float focusAuraBaseSize = 12f;
        private AudioSource focusSfx;
        private AudioClip sfxFocusCharge;
        private AudioClip sfxFocusComplete;
        private AudioClip sfxFocusReady;
        private int lastSoulForReady = -1;

        private SimpleHUD cachedHud;
        private float hurtCooldown;
        private const float HurtIFrameSeconds = 1.35f;
        private const float ReviveIFrameSeconds = 1.5f;
        private float ignoreRefreshTimer;
        private float hornetIgnoreRefreshTimer;
        private bool isCastingSpell;

        private int lastSavedHP;
        private int lastSavedMax;
        private int lastSavedSoul;
        private bool lastSavedCanTakeDamage = true;
    }
}
