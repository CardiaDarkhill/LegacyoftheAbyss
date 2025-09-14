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
        public float sprintMultiplier = 1.8f;
        public float maxDistance = 14f;
        public float softLeashRadius = 10f;
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
        private const KeyCode FireKey = KeyCode.Space;
        private const KeyCode NailKey = KeyCode.J;
        private const KeyCode TeleportKey = KeyCode.K;
        private const KeyCode SprintKeyPrimary = KeyCode.LeftShift;
        private const KeyCode SprintKeySecondary = KeyCode.RightShift;
        private const KeyCode DamageToggleKey = KeyCode.Alpha0;
        private bool canTakeDamage = true;
        // Spells use FireKey + W (Shriek) or FireKey + S (Descending Dark)

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
        private const KeyCode FocusKey = KeyCode.H; // hold to focus
        public int focusSoulCost = 33;
        public float focusChannelTime = 1.25f;
        private bool isFocusing;
        private float focusTimer;
        private float focusAlphaWhileChannel = 0.75f;
        private float focusHealRange = 6f;
        private float focusSoulAccumulator;
        private Renderer focusAuraRenderer;
        private float focusAuraBaseSize = 7f;
        private AudioSource focusSfx;
        private AudioClip sfxFocusCharge;
        private AudioClip sfxFocusComplete;
        private AudioClip sfxFocusReady;
        private int lastSoulForReady = -1;

        private SimpleHUD cachedHud;
        private float hurtCooldown;
        private const float HurtIFrameSeconds = 1.35f;
        private float ignoreRefreshTimer;
        private float hornetIgnoreRefreshTimer;
        private bool isCastingSpell;

        private int lastSavedHP;
        private int lastSavedMax;
        private int lastSavedSoul;
        private bool lastSavedCanTakeDamage = true;

        public void RestorePersistentState(int hp, int max, int soul, bool canDamage = true)
        {
            shadeMaxHP = Mathf.Max(1, max);
            shadeHP = Mathf.Clamp(hp, 0, shadeMaxHP);
            shadeSoul = Mathf.Clamp(soul, 0, shadeSoulMax);
            canTakeDamage = canDamage;
            lastSavedCanTakeDamage = canTakeDamage;
        }

        public void FullHealFromBench()
        {
            shadeHP = Mathf.Max(shadeHP, shadeMaxHP);
            if (shadeHP > 0)
            {
                isInactive = false;
                CancelDeathAnimation();
            }
            PushShadeStatsToHud();
        }

        public void ReviveToAtLeast(int hp)
        {
            int target = Mathf.Max(1, hp);
            shadeHP = Mathf.Max(shadeHP, target);
            if (shadeHP > 0)
            {
                isInactive = false;
                CancelDeathAnimation();
            }
            PushShadeStatsToHud();
            PersistIfChanged();
        }

        public int GetCurrentHP() => shadeHP;
        public int GetMaxHP() => shadeMaxHP;
        public int GetShadeSoul() => shadeSoul;
        public bool GetCanTakeDamage() => canTakeDamage;

        public void Init(Transform hornet) { hornetTransform = hornet; }

        private void Start()
        {
            SetupPhysics();
            if (hornetTransform == null)
            {
                var hornet = GameObject.FindWithTag("Player");
                if (hornet != null)
                {
                    hornetTransform = hornet.transform;
                }
            }

            sr = GetComponent<SpriteRenderer>();
            transform.localScale = Vector3.one * SpriteScale;
            LoadShadeSprites();
            if (sr != null)
            {
                if (idleAnimFrames != null && idleAnimFrames.Length > 0)
                    sr.sprite = idleAnimFrames[0];
                else if (floatAnimFrames != null && floatAnimFrames.Length > 0)
                    sr.sprite = floatAnimFrames[0];
                else if (inactiveSprite != null)
                    sr.sprite = inactiveSprite;
                var c = sr.color; c.a = 0.9f; sr.color = c;
            }

            // Ensure the shade can act as a pogo surface for Hornet
            try { gameObject.tag = "Recoiler"; } catch { }

            SetupShadeLight();
            cachedHud = Object.FindFirstObjectByType<SimpleHUD>();
            PushSoulToHud();
            CheckHazardOverlap();
            // Ensure pogo target is present for Hornet downslash bounces
            // Add a dedicated pogo target with HitResponse so hero slashes can register even when OnlyDamageEnemies is true
            EnsurePogoTarget();
            try
            {
                var pd = GameManager.instance != null ? GameManager.instance.playerData : null;
                if (pd != null)
                {
                    int computedMax = Mathf.Max(1, (pd.maxHealth + 1) / 2);
                    shadeMaxHP = computedMax;
                    if (!LegacyHelper.HasSavedShadeState && shadeHP <= 0)
                        shadeHP = Mathf.Clamp((pd.health + 1) / 2, 0, shadeMaxHP);
                    shadeHP = Mathf.Clamp(shadeHP, 0, shadeMaxHP);
                    PushShadeStatsToHud();
                }
            }
            catch { }

            lastSavedHP = lastSavedMax = lastSavedSoul = -999;
            PersistIfChanged();
            lastSoulForReady = shadeSoul;
        }

        private void LoadShadeSprites()
        {
            try
            {
                var dir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets", "Knight_Shade_Sprites");
                idleAnimFrames = LoadSpriteStrip(Path.Combine(dir, "Shade_Idle_Sheet.png"), 9);
                floatAnimFrames = LoadSpriteStrip(Path.Combine(dir, "Shade_Float_Sheet.png"), 6);
                vengefulAnimFrames = LoadSpriteStrip(Path.Combine(dir, "Vengeful_Spirit_Sheet.png"), 2);
                shadeSoulAnimFrames = LoadSpriteStrip(Path.Combine(dir, "Shade_Soul_Sheet.png"), 4);
                fireballCastAnimFrames = LoadSpriteStrip(Path.Combine(dir, "Shade_Fireball_Cast_Sheet.png"), 4);
                quakeCastAnimFrames = LoadSpriteStrip(Path.Combine(dir, "Shade_Quake_Cast_Sheet.png"), 2);
                shriekCastAnimFrames = LoadSpriteStrip(Path.Combine(dir, "Shade_Shriek_Cast_Sheet.png"), 2);
                abyssShriekAnimFrames = LoadSpriteStrip(Path.Combine(dir, "Abyss_Shriek_sheet.png"), 8);
                howlingWraithsAnimFrames = LoadSpriteStrip(Path.Combine(dir, "Howling_Wraiths_Sheet.png"), 7);
                deathAnimFrames = LoadSpriteStrip(Path.Combine(dir, "Shade_Death_Sheet.png"), 6);
                descendAnimFrames = LoadSpriteStrip(Path.Combine(dir, "Shade_Descend_Sheet.png"), 3);
                descendAuraAnimFrames = LoadSpriteStrip(Path.Combine(dir, "Quake_Descend_Aura_Sheet.png"), 3);
                dDiveSlamAnimFrames = LoadSpriteStrip(Path.Combine(dir, "DDive_Slam_Sheet.png"), 2);
                dDarkSlamAnimFrames = LoadSpriteStrip(Path.Combine(dir, "DDark_Slam_Sheet.png"), 6);
                dDarkBurstAnimFrames = LoadSpriteStrip(Path.Combine(dir, "DDark_Burst_sheet.png"), 7);
                var inactive = LoadSpriteStrip(Path.Combine(dir, "ShadeInactive.png"));
                inactiveSprite = inactive.Length > 0 ? inactive[0] : null;
            }
            catch
            {
                idleAnimFrames = System.Array.Empty<Sprite>();
                floatAnimFrames = System.Array.Empty<Sprite>();
                vengefulAnimFrames = System.Array.Empty<Sprite>();
                shadeSoulAnimFrames = System.Array.Empty<Sprite>();
                fireballCastAnimFrames = System.Array.Empty<Sprite>();
                quakeCastAnimFrames = System.Array.Empty<Sprite>();
                shriekCastAnimFrames = System.Array.Empty<Sprite>();
                abyssShriekAnimFrames = System.Array.Empty<Sprite>();
                howlingWraithsAnimFrames = System.Array.Empty<Sprite>();
                deathAnimFrames = System.Array.Empty<Sprite>();
                descendAnimFrames = System.Array.Empty<Sprite>();
                descendAuraAnimFrames = System.Array.Empty<Sprite>();
                dDiveSlamAnimFrames = System.Array.Empty<Sprite>();
                dDarkSlamAnimFrames = System.Array.Empty<Sprite>();
                dDarkBurstAnimFrames = System.Array.Empty<Sprite>();
                inactiveSprite = null;
            }
        }

        private Sprite[] LoadSpriteStrip(string path, int frames = 0)
        {
            if (!File.Exists(path)) return System.Array.Empty<Sprite>();
            var bytes = File.ReadAllBytes(path);
            var tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            TryLoadImage(tex, bytes);
            tex.filterMode = FilterMode.Point;
            int cols = frames > 0 ? frames : Mathf.Max(1, tex.width / tex.height);
            int frameWidth = tex.width / cols;
            int frameHeight = tex.height;
            var sprites = new Sprite[cols];
            for (int i = 0; i < cols; i++)
                sprites[i] = Sprite.Create(tex, new Rect(i * frameWidth, 0, frameWidth, frameHeight), new Vector2(0.5f, 0.5f));
            return sprites;
        }

        private static bool TryLoadImage(Texture2D tex, byte[] bytes)
        {
            try
            {
                var t = System.Type.GetType("UnityEngine.ImageConversion, UnityEngine.ImageConversionModule");
                if (t != null)
                {
                    var m = t.GetMethod("LoadImage", BindingFlags.Public | BindingFlags.Static, null, new System.Type[] { typeof(Texture2D), typeof(byte[]), typeof(bool) }, null);
                    if (m != null) { m.Invoke(null, new object[] { tex, bytes, false }); return true; }
                }
            }
            catch { }
            return false;
        }

        private void EnsureInactivePulse()
        {
            if (inactivePulseSr != null) return;
            var pulseGO = new GameObject("InactivePulse");
            pulseGO.transform.SetParent(transform, false);
            pulseGO.transform.localPosition = Vector3.zero;
            inactivePulseSr = pulseGO.AddComponent<SpriteRenderer>();
            if (sr != null)
            {
                inactivePulseSr.sortingLayerID = sr.sortingLayerID;
                inactivePulseSr.sortingOrder = sr.sortingOrder - 1;
            }
            inactivePulseSr.color = new Color(1f, 1f, 1f, 0f);
        }

        private void HandleAnimation()
        {
            if (sr == null) return;
            sr.flipX = (facing == 1);

            if (isCastingSpell && currentAnimFrames != null)
                return;

            if (isInactive)
            {
                if (inactiveSprite != null) sr.sprite = inactiveSprite;
                var cInact = sr.color;
                cInact.a = 0.8f + 0.2f * Mathf.Sin(Time.time * 3f);
                sr.color = cInact;
                EnsureInactivePulse();
                if (inactivePulseSr != null)
                {
                    inactivePulseSr.gameObject.SetActive(true);
                    inactivePulseSr.sprite = inactiveSprite;
                    float alpha = 0.35f + 0.25f * Mathf.Sin(Time.time * 3f);
                    var pc = inactivePulseSr.color;
                    pc.a = alpha;
                    inactivePulseSr.color = pc;
                    float scale = 1.05f + 0.03f * Mathf.Sin(Time.time * 2f);
                    inactivePulseSr.transform.localScale = new Vector3(scale, scale, 1f);
                }
                return;
            }
            else if (inactivePulseSr != null)
            {
                inactivePulseSr.gameObject.SetActive(false);
            }

            var c = sr.color; c.a = 0.9f; sr.color = c;
            Sprite[] frames = (Mathf.Abs(lastMoveDelta.x) > 0.01f) ? floatAnimFrames : idleAnimFrames;
            if (frames == null || frames.Length == 0) return;

            if (currentAnimFrames != frames)
            {
                currentAnimFrames = frames;
                animFrameIndex = 0;
                animTimer = 0f;
                sr.sprite = frames[0];
            }

            animTimer += Time.deltaTime;
            if (animTimer >= AnimFrameTime)
            {
                animTimer -= AnimFrameTime;
                animFrameIndex = (animFrameIndex + 1) % frames.Length;
                sr.sprite = frames[animFrameIndex];
            }
        }

        private void EnsurePogoTarget()
        {
            try
            {
                var pogo = transform.Find("PogoTarget")?.gameObject;
                if (pogo == null)
                {
                    pogo = new GameObject("PogoTarget");
                    pogo.transform.SetParent(transform, false);
                    pogo.transform.localPosition = Vector3.zero;
                }

                // Ensure collider present (trigger) so slash damager trigger can overlap
                var col = pogo.GetComponent<Collider2D>();
                if (!col)
                {
                    var cap = pogo.AddComponent<CapsuleCollider2D>();
                    cap.direction = CapsuleDirection2D.Vertical;
                    cap.size = new Vector2(0.95f, 1.45f);
                    cap.isTrigger = true;
                    col = cap;
                }
                else col.isTrigger = true;

                // Put on INTERACTIVE_OBJECT (or Default) so DownAttack's HitResponded path will process and allow bounce
                int interactiveLayer = LayerMask.NameToLayer("Interactive Object");
                if (interactiveLayer < 0) interactiveLayer = LayerMask.NameToLayer("Default");
                if (interactiveLayer < 0) interactiveLayer = 0;
                pogo.layer = interactiveLayer;

                // Attach HitResponse and configure to accept hero nail hits downward only
                // Optional puff-on-collision helper
                if (!pogo.GetComponent<ShadePogoPuff>()) pogo.AddComponent<ShadePogoPuff>();

                // Make sure shade's own rigidbody exists for trigger events
                if (rb) { /* already present */ }
            }
            catch { }
        }

        private void Update()
        {
            if (hornetTransform == null) return;

            if (hazardCooldown > 0f) hazardCooldown = Mathf.Max(0f, hazardCooldown - Time.deltaTime);
            if (hurtCooldown > 0f) hurtCooldown = Mathf.Max(0f, hurtCooldown - Time.deltaTime);
            if (Input.GetKeyDown(DamageToggleKey))
            {
                canTakeDamage = !canTakeDamage;
                try { UnityEngine.Debug.Log($"[ShadeDebug] Damage {(canTakeDamage ? "enabled" : "disabled")}"); } catch { }
                PersistIfChanged();
            }
            ignoreRefreshTimer -= Time.deltaTime;
            if (ignoreRefreshTimer <= 0f)
            {
                RefreshCollisionIgnores();
                ignoreRefreshTimer = 1f;
            }

            hornetIgnoreRefreshTimer -= Time.deltaTime;
            if (hornetIgnoreRefreshTimer <= 0f)
            {
                EnsureIgnoreHornetCollisions();
                hornetIgnoreRefreshTimer = 0.5f;
            }

            // Track inactive flag
            isInactive = (!isDying && shadeHP <= 0);

            HandleTeleportChannel();

            CheckSprintUnlock();

            HandleMovementAndFacing();
            // Allow starting focus even when not casting other spells; focusing itself sets isCastingSpell
            if (!inHardLeash && !isChannelingTeleport && !isInactive)
            {
                HandleFocus();
                if (!isCastingSpell)
                    HandleFire();
                if (!isCastingSpell)
                {
                    HandleNailAttack();
                    HandleShriek();
                    HandleDescendingDark();
                }
            }

            if (!cachedHud) cachedHud = Object.FindFirstObjectByType<SimpleHUD>();
            PushSoulToHud();
            CheckHazardOverlap();
            SyncShadeLight();
            PersistIfChanged();
            CheckFocusReadySfx();
            HandleAnimation();
        }

        public void ApplyBindHealFromHornet(Transform hornet)
        {
            try
            {
                var h = hornet != null ? hornet : hornetTransform;
                if (h == null) return;
                float dist = Vector2.Distance(h.position, transform.position);
                if (dist <= 6f)
                {
                    int before = shadeHP;
                    shadeHP = Mathf.Min(shadeHP + 2, shadeMaxHP);
                    if (shadeHP != before)
                    {
                        if (shadeHP > 0)
                        {
                            isInactive = false;
                            CancelDeathAnimation();
                        }
                        PushShadeStatsToHud();
                        PersistIfChanged();
                    }
                }
            }
            catch { }
        }

        private void PersistIfChanged()
        {
            if (lastSavedHP != shadeHP || lastSavedMax != shadeMaxHP || lastSavedSoul != shadeSoul || lastSavedCanTakeDamage != canTakeDamage)
            {
                LegacyHelper.SaveShadeState(shadeHP, shadeMaxHP, shadeSoul, canTakeDamage);
                lastSavedHP = shadeHP; lastSavedMax = shadeMaxHP; lastSavedSoul = shadeSoul; lastSavedCanTakeDamage = canTakeDamage;
            }
        }

        private void PushSoulToHud()
        {
            if (cachedHud)
            {
                try { cachedHud.SetShadeSoul(shadeSoul, shadeSoulMax); } catch { }
            }
        }

        private void PushShadeStatsToHud()
        {
            if (cachedHud)
            {
                try { cachedHud.SetShadeStats(shadeHP, shadeMaxHP); } catch { }
            }
        }

        private void HandleMovementAndFacing()
        {
            if (isCastingSpell) { lastMoveDelta = Vector2.zero; return; }
            float h = (Input.GetKey(KeyCode.A) ? -1f : 0f) + (Input.GetKey(KeyCode.D) ? 1f : 0f);
            float v = (Input.GetKey(KeyCode.S) ? -1f : 0f) + (Input.GetKey(KeyCode.W) ? 1f : 0f);
            Vector2 input = new Vector2(h, v);
            if (input.sqrMagnitude > 1f) input.Normalize();

            // Freeze manual input while channeling teleport
            if (isChannelingTeleport || isFocusing) input = Vector2.zero;

            Vector2 to = (Vector2)(hornetTransform.position - transform.position);
            float dist = to.magnitude;

            if (dist > snapLeashRadius)
            {
                TeleportToHornet();
                inHardLeash = false; hardLeashTimer = 0f; EnableCollisions(true);
                return;
            }

            Vector2 moveDelta = Vector2.zero;
            if (dist > softLeashRadius && dist <= hardLeashRadius)
            {
                float t = Mathf.InverseLerp(softLeashRadius, hardLeashRadius, dist);
                Vector2 pullDir = to.normalized;
                moveDelta += pullDir * (Mathf.Lerp(softPullSpeed, softPullSpeed * 1.5f, t)) * Time.deltaTime;
                inHardLeash = false; hardLeashTimer = 0f; EnableCollisions(true);
            }

            if (dist > hardLeashRadius)
            {
                inHardLeash = true;
                hardLeashTimer += Time.deltaTime;
                EnableCollisions(false);
                Vector2 dir = to.normalized;
                moveDelta = dir * hardPullSpeed * Time.deltaTime;
                if (hardLeashTimer >= hardLeashTimeout)
                {
                    TeleportToHornet();
                    inHardLeash = false; hardLeashTimer = 0f; EnableCollisions(true);
                    return;
                }
            }
            else if (inHardLeash)
            {
                inHardLeash = false; hardLeashTimer = 0f; EnableCollisions(true);
            }

            if (!inHardLeash)
            {
                float speed = moveSpeed;
                bool sprinting = sprintUnlocked &&
                                 (Input.GetKey(SprintKeyPrimary) || Input.GetKey(SprintKeySecondary)) &&
                                 input.sqrMagnitude > 0f;
                bool startedSprint = sprinting && !isSprinting;
                if (startedSprint)
                {
                    SpawnSprintBurst(-input.normalized);
                    if (sprintDashCooldownTimer <= 0f)
                    {
                        sprintDashTimer = sprintDashDuration;
                        sprintDashCooldownTimer = sprintDashCooldown;
                        TryPlayDashSfx();
                    }
                }
                if (sprintDashTimer > 0f)
                {
                    speed *= sprintDashMultiplier;
                    sprintDashTimer -= Time.deltaTime;
                    if (activeDashPs)
                    {
                        var emit = new ParticleSystem.EmitParams();
                        emit.velocity = activeDashDir * UnityEngine.Random.Range(4f, 8f);
                        emit.startSize = UnityEngine.Random.Range(0.15f, 0.25f);
                        activeDashPs.Emit(emit, 1);
                    }
                }
                else
                {
                    activeDashPs = null;
                    if (sprinting)
                    {
                        speed *= sprintMultiplier;
                    }
                }
                if (sprintDashCooldownTimer > 0f)
                    sprintDashCooldownTimer -= Time.deltaTime;

                moveDelta += input * speed * Time.deltaTime;
                isSprinting = sprinting;
            }
            else
            {
                isSprinting = false;
                sprintDashTimer = 0f;
            }

            // Compute proposed next position and clamp against transition gates at map edges
            Vector2 curPos = rb ? rb.position : (Vector2)transform.position;
            Vector2 proposed = curPos + moveDelta;
            proposed = ClampAgainstTransitionGates(proposed);

            if (rb) rb.MovePosition(proposed);
            else transform.position = proposed;
            lastMoveDelta = proposed - curPos;

            // Update facing only from player's horizontal input.
            // Do not auto-face Hornet when idle; preserve last manual facing.
            if (h > 0.1f) facing = 1;
            else if (h < -0.1f) facing = -1;

            if (sr != null) sr.flipX = (facing == 1);

            if (dist > maxDistance)
            {
                Vector3 toShade = transform.position - hornetTransform.position;
                transform.position = hornetTransform.position + toShade.normalized * maxDistance;
            }
        }

        private void CheckSprintUnlock()
        {
            if (sprintUnlocked) return;
            try
            {
                var hc = HeroController.instance;
                if (hc != null && hc.CanSprint())
                    sprintUnlocked = true;
            }
            catch { }
        }

        private void SpawnSprintBurst(Vector2 dir)
        {
            try
            {
                Vector2 ndir = dir.normalized;
                GameObject go = new GameObject("ShadeSprintBurst");
                go.transform.position = transform.position;
                var ps = go.AddComponent<ParticleSystem>();
                var main = ps.main;
                main.startLifetime = 0.4f;
                main.startSpeed = 0f;
                main.startSize = new ParticleSystem.MinMaxCurve(0.15f, 0.25f);
                main.startColor = Color.black;
                var psr = ps.GetComponent<ParticleSystemRenderer>();
                if (psr != null)
                {
                    if (s_sprintBurstMat == null)
                    {
                        s_sprintBurstMat = new Material(Shader.Find("Sprites/Default"));
                        s_sprintBurstMat.color = Color.black;
                    }
                    psr.sharedMaterial = s_sprintBurstMat;
                    psr.sharedMaterial.mainTexture = MakeDotSprite().texture;
                }
                var col = ps.colorOverLifetime;
                col.enabled = true;
                Gradient g = new Gradient();
                g.SetKeys(
                    new[] { new GradientColorKey(Color.black, 0f), new GradientColorKey(Color.black, 1f) },
                    new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
                col.color = g;
                var emission = ps.emission;
                emission.enabled = false;
                for (int i = 0; i < 12; i++)
                {
                    var emit = new ParticleSystem.EmitParams();
                    emit.velocity = ndir * UnityEngine.Random.Range(4f, 8f);
                    emit.startColor = Color.black;
                    emit.startSize = UnityEngine.Random.Range(0.15f, 0.25f);
                    ps.Emit(emit, 1);
                }
                ps.Play();
                activeDashPs = ps;
                activeDashDir = ndir;
                Destroy(go, 1f);
            }
            catch { }
        }

        private void TryPlayDashSfx()
        {
            try
            {
                var hc = HeroController.instance;
                if (hc != null && hc.AudioCtrl != null)
                    hc.AudioCtrl.PlaySound(HeroSounds.DASH);
            }
            catch { }
        }

        private void HandleFire()
        {
            fireTimer -= Time.deltaTime;
            if (!Input.GetKey(FireKey) || fireTimer > 0f) return;
            // If aiming a spell with up/down, don't fire projectile
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)) return;
            if (!IsProjectileUnlocked()) return;
            if (shadeSoul < projectileSoulCost) return;
            fireTimer = fireCooldown;
            shadeSoul = Mathf.Max(0, shadeSoul - projectileSoulCost);
            PushSoulToHud();
            CheckHazardOverlap();
            StartCoroutine(FireballCastRoutine());
        }

        private void HandleShriek()
        {
            shriekTimer -= Time.deltaTime;
            // Trigger on FireKey + Up
            if (!Input.GetKeyDown(FireKey) || !Input.GetKey(KeyCode.W)) return;
            if (!IsShriekUnlocked()) return; // locked until 3rd unlock
            if (shriekTimer > 0f) return;
            if (shadeSoul < shriekSoulCost) return;
            shriekTimer = shriekCooldown;
            shadeSoul = Mathf.Max(0, shadeSoul - shriekSoulCost);
            PushSoulToHud();
            CheckHazardOverlap();
            StartCoroutine(ShriekCastRoutine());
        }

        private IEnumerator FireballCastRoutine()
        {
            isCastingSpell = true;
            if (fireballCastAnimFrames != null && fireballCastAnimFrames.Length > 0)
            {
                currentAnimFrames = fireballCastAnimFrames;
                animFrameIndex = 0;
                animTimer = 0f;
                float perFrame = 0.25f / fireballCastAnimFrames.Length;
                for (int i = 0; i < fireballCastAnimFrames.Length; i++)
                {
                    if (sr) sr.sprite = fireballCastAnimFrames[i];
                    yield return new WaitForSeconds(perFrame);
                }
            }
            else
            {
                yield return new WaitForSeconds(0.25f);
            }

            Vector2 dir = new Vector2(facing, 0f);
            SpawnProjectile(dir);
            currentAnimFrames = null;
            isCastingSpell = false;
        }

        private IEnumerator ShriekCastRoutine()
        {
            isCastingSpell = true;
            bool upgraded = IsShriekUpgraded();
            int dmg = ComputeSpellDamageMultiplier(4f, upgraded);
            TryPlayShriekSfx(upgraded);
            float life = 0.18f;
            Vector2 localOffset = new Vector2(0f, 0.8f);
            SpawnShriekCone(12f, 95f, dmg, life, localOffset);
            SpawnShriekFx(upgraded);

            if (shriekCastAnimFrames != null && shriekCastAnimFrames.Length > 0)
            {
                currentAnimFrames = shriekCastAnimFrames;
                animFrameIndex = 0;
                animTimer = 0f;
                float perFrame = 0.25f / shriekCastAnimFrames.Length;
                float elapsed = 0f;
                while (elapsed < 0.25f)
                {
                    if (sr) sr.sprite = shriekCastAnimFrames[(int)(elapsed / perFrame) % shriekCastAnimFrames.Length];
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(0.25f);
            }
            currentAnimFrames = null;
            isCastingSpell = false;
        }

        private void HandleDescendingDark()
        {
            quakeTimer -= Time.deltaTime;
            // Trigger on FireKey + Down
            if (!Input.GetKeyDown(FireKey) || !Input.GetKey(KeyCode.S)) return;
            if (!IsDescendingDarkUnlocked()) return; // locked until 2nd unlock
            if (quakeTimer > 0f) return;
            if (shadeSoul < quakeSoulCost) return;
            quakeTimer = quakeCooldown;
            shadeSoul = Mathf.Max(0, shadeSoul - quakeSoulCost);
            PushSoulToHud();
            CheckHazardOverlap();

            bool upgraded = IsDescendingDarkUpgraded();
            TryPlayQuakePrepareSfx();
            int dmg = ComputeSpellDamageMultiplier(3f, upgraded); // Descending Dark base 3x
            StartCoroutine(DescendingDarkRoutine(dmg, upgraded));
        }

        // Spell progression helpers
        private int ShadeSpellProgress
        {
            get
            {
                try
                {
                    var pd = GameManager.instance != null ? GameManager.instance.playerData : null;
                    if (pd == null) return 0;
                    int c = 0;
                    if (pd.hasNeedleThrow) c++;
                    if (pd.hasThreadSphere) c++;
                    if (pd.hasSilkCharge) c++;
                    if (pd.hasParry) c++;
                    if (pd.hasSilkBomb) c++;
                    if (pd.hasSilkBossNeedle) c++;
                    return Mathf.Clamp(c, 0, 6);
                }
                catch { return 0; }
            }
        }
        private bool IsProjectileUnlocked() => ShadeSpellProgress >= 1;
        private bool IsDescendingDarkUnlocked() => ShadeSpellProgress >= 2;
        private bool IsShriekUnlocked() => ShadeSpellProgress >= 3;
        private bool IsProjectileUpgraded() => ShadeSpellProgress >= 4;
        private bool IsDescendingDarkUpgraded() => ShadeSpellProgress >= 5;
        private bool IsShriekUpgraded() => ShadeSpellProgress >= 6;
        private int ComputeSpellDamageMultiplier(float baseMult, bool upgraded)
        {
            int nail = Mathf.Max(1, GetHornetNailDamage());
            float mult = upgraded ? baseMult : baseMult * 0.7f; // Soul variant = 30% less
            int dmg = Mathf.RoundToInt(nail * mult);
            return Mathf.Max(1, dmg);
        }

        private void SpawnAoE(string name, Vector3 worldPos, float radius, int damage, float lifeSeconds)
        {
            var go = new GameObject(name);
            go.transform.position = worldPos;
            go.tag = "Hero Spell";
            int spellLayer = LayerMask.NameToLayer("Hero Spell");
            int atkLayer = LayerMask.NameToLayer("Hero Attack");
            if (spellLayer >= 0) go.layer = spellLayer; else if (atkLayer >= 0) go.layer = atkLayer;

            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = radius;

            var aoe = go.AddComponent<ShadeAoE>();
            aoe.damage = damage;
            aoe.hornetRoot = hornetTransform;
            aoe.lifeSeconds = lifeSeconds;

            // Optional visual hint
            try
            {
                var sr2 = go.AddComponent<SpriteRenderer>();
                sr2.sprite = MakeDotSprite();
                var c = new Color(0f, 0f, 0f, 0.25f);
                sr2.color = c;
                sr2.sortingLayerID = sr ? sr.sortingLayerID : 0;
                sr2.sortingOrder = sr ? (sr.sortingOrder - 1) : -1;
                go.transform.localScale = Vector3.one * (radius * 2.2f);
            }
            catch { }

            IgnoreHornetForCollider(col);
        }

        private void IgnoreHornetForCollider(Collider2D col)
        {
            try
            {
                if (!col || !hornetTransform) return;
                var hornetCols = hornetTransform.root.GetComponentsInChildren<Collider2D>(true);
                foreach (var hc in hornetCols)
                    if (hc) Physics2D.IgnoreCollision(col, hc, true);
            }
            catch { }
        }

        private void SpawnShriekCone(float height, float degrees, int damage, float lifeSeconds, Vector2 localOffset)
        {
            var go = new GameObject("ShadeShriekCone");
            go.transform.position = transform.position + (Vector3)localOffset;
            go.tag = "Hero Spell";
            int spellLayer = LayerMask.NameToLayer("Hero Spell");
            int atkLayer = LayerMask.NameToLayer("Hero Attack");
            if (spellLayer >= 0) go.layer = spellLayer; else if (atkLayer >= 0) go.layer = atkLayer;

            var poly = go.AddComponent<PolygonCollider2D>();
            poly.isTrigger = true;
            // Build wedge polygon with apex at (0,0) and arc up
            int segments = 8;
            float half = degrees * 0.5f;
            List<Vector2> pts = new List<Vector2>();
            pts.Add(Vector2.zero);
            for (int i = 0; i <= segments; i++)
            {
                float a = Mathf.Lerp(-half, half, i / (float)segments);
                float ang = (90f + a) * Mathf.Deg2Rad; // around up axis
                Vector2 p = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * height;
                pts.Add(p);
            }
            poly.SetPath(0, pts.ToArray());

            var aoe = go.AddComponent<ShadeAoE>();
            aoe.damage = damage;
            aoe.hornetRoot = hornetTransform;
            aoe.lifeSeconds = lifeSeconds;

            IgnoreHornetForCollider(poly);
        }

        private void SpawnShriekFx(bool upgraded)
        {
            var frames = upgraded ? abyssShriekAnimFrames : howlingWraithsAnimFrames;
            if (frames == null || frames.Length == 0 || sr == null) return;
            var go = new GameObject(upgraded ? "AbyssShriekFx" : "HowlingWraithsFx");
            go.transform.position = transform.position;
            go.layer = gameObject.layer;
            var fxSr = go.AddComponent<SpriteRenderer>();
            fxSr.sortingLayerID = sr.sortingLayerID;
            fxSr.sortingOrder = sr.sortingOrder - 1;
            float fxScale = SpriteScale * 3f;
            go.transform.localScale = Vector3.one * fxScale;
            StartCoroutine(PlayShriekFx(fxSr, frames, fxScale));
        }

        private IEnumerator PlayShriekFx(SpriteRenderer fxSr, Sprite[] frames, float fxScale)
        {
            if (fxSr == null || frames == null || frames.Length == 0) yield break;
            float shadeBottom = transform.position.y;
            if (sr && sr.sprite)
                shadeBottom -= sr.sprite.bounds.extents.y * SpriteScale;
            float fxExt = frames[0].bounds.extents.y * fxScale;
            var pos = fxSr.transform.position;
            pos.y = shadeBottom + fxExt;
            fxSr.transform.position = pos;

            float duration = 0.4f;
            float per = duration / frames.Length;
            float t = 0f;
            fxSr.sprite = frames[0];
            while (t < duration)
            {
                int idx = Mathf.Min((int)(t / per), frames.Length - 1);
                fxSr.sprite = frames[idx];
                t += Time.deltaTime;
                yield return null;
            }
            Destroy(fxSr.gameObject);
        }

        private IEnumerator DescendingDarkRoutine(int totalDamage, bool upgraded)
        {
            isCastingSpell = true;
            float prevVelY = rb ? rb.linearVelocity.y : 0f;
            if (rb) rb.linearVelocity = Vector2.zero;
            if (quakeCastAnimFrames != null && quakeCastAnimFrames.Length > 0)
            {
                currentAnimFrames = quakeCastAnimFrames;
                animFrameIndex = 0;
                animTimer = 0f;
                float perFrame = 0.25f / quakeCastAnimFrames.Length;
                for (int i = 0; i < quakeCastAnimFrames.Length; i++)
                {
                    if (sr) sr.sprite = quakeCastAnimFrames[i];
                    yield return new WaitForSeconds(perFrame);
                }
            }
            else
            {
                yield return new WaitForSeconds(0.25f);
            }

            // Enemy i-frames during descent (not hazards)
            hurtCooldown = Mathf.Max(hurtCooldown, 0.6f);

            if (descendAnimFrames != null && descendAnimFrames.Length > 0 && sr)
            {
                currentAnimFrames = descendAnimFrames;
                animFrameIndex = 0;
                animTimer = 0f;
                sr.sprite = descendAnimFrames[0];
            }
            var aura = SpawnDescendAura();

            // Find ground below: ignore Hornet/enemy/hazard hitboxes so we only stop on terrain
            Vector2 start = transform.position;
            float maxDist = 60f;
            var hits = Physics2D.RaycastAll(start, Vector2.down, maxDist);
            RaycastHit2D? pick = null;
            Transform hornetRoot = hornetTransform ? hornetTransform.root : null;
            foreach (var h in hits)
            {
                if (!h.collider) continue;
                if (h.collider.isTrigger) continue;
                var ht = h.collider.transform;
                // ignore self (any part of the shade hierarchy)
                if (ht == transform || ht.IsChildOf(transform) || transform.IsChildOf(ht)) continue;
                // ignore hornet using hierarchy checks
                if (hornetTransform)
                {
                    if (ht == hornetTransform || ht.IsChildOf(hornetTransform) || (hornetRoot && ht.root == hornetRoot))
                        continue;
                }
                // also skip any collider attached to Hornet via HeroController component
                try
                {
                    if (h.collider.GetComponentInParent<HeroController>() != null)
                        continue;
                }
                catch { }
                if (h.collider.name == "Hero Physics Pusher")
                    continue;
                if (h.collider.CompareTag("Player"))
                    continue;
                // ignore enemies/hazards (anything that damages hero)
                try { if (h.collider.GetComponentInParent<DamageHero>() != null) continue; } catch { }
                // otherwise this is acceptable ground
                pick = h;
                UnityEngine.Debug.Log($"[ShadeDebug] Descending Dark ground hit {h.collider.name} tag={h.collider.tag} layer={h.collider.gameObject.layer}");
                break;
            }

            Vector3 targetPos = transform.position + Vector3.down * 8f; // fallback
            float groundY = targetPos.y;
            float extY = bodyCol ? bodyCol.bounds.extents.y : 0.7f;
            if (pick.HasValue)
            {
                groundY = pick.Value.point.y;
                targetPos = new Vector3(transform.position.x, groundY + extY + 0.02f, transform.position.z);
            }

            // Quick drop over 0.12s
            float dropTime = 0.12f;
            Vector3 from = transform.position;
            float elapsed = 0f;
            float descTimer = 0f;
            float descFrame = 0.05f;
            while (elapsed < dropTime)
            {
                elapsed += Time.deltaTime;
                float u = Mathf.Clamp01(elapsed / dropTime);
                Vector3 p = Vector3.Lerp(from, targetPos, u*u); // ease in
                TeleportToPosition(p);
                if (descendAnimFrames != null && descendAnimFrames.Length > 0 && sr)
                {
                    descTimer += Time.deltaTime;
                    if (descTimer >= descFrame)
                    {
                        descTimer -= descFrame;
                        animFrameIndex = (animFrameIndex + 1) % descendAnimFrames.Length;
                        sr.sprite = descendAnimFrames[animFrameIndex];
                    }
                }
                yield return null;
            }
            TeleportToPosition(targetPos);
            if (aura) Destroy(aura);

            // If landing area is a hazard, skip the impact
            if (IsHazardAtPosition(new Vector2(targetPos.x, groundY + 0.2f), 0.8f))
            {
                currentAnimFrames = null;
                isCastingSpell = false;
                yield break;
            }

            // Spawn two hitboxes: ground strip (10 units wide), and teardrop (6x8) above
            TryPlayQuakeImpactSfx(upgraded);
            int half = Mathf.Max(1, Mathf.RoundToInt(totalDamage * 0.5f));
            SpawnQuakeImpact(groundY, half);
            SpawnQuakeTeardrop(groundY, half);
            if (upgraded)
            {
                SpawnGroundSlamFx(dDarkSlamAnimFrames, groundY);
                SpawnDarkBurstFx(groundY);
            }
            else
            {
                SpawnGroundSlamFx(dDiveSlamAnimFrames, groundY);
            }

            // Small delay to keep i-frames briefly after impact
            yield return new WaitForSeconds(0.1f);
            if (rb) rb.linearVelocity = new Vector2(rb.linearVelocity.x, prevVelY);
            currentAnimFrames = null;
            isCastingSpell = false;
        }

        private bool IsHazardAtPosition(Vector2 pos, float radius)
        {
            try
            {
                var hits = Physics2D.OverlapCircleAll(pos, radius, ~0, -Mathf.Infinity, Mathf.Infinity);
                Transform hornetRoot = hornetTransform ? hornetTransform.root : null;
                foreach (var c in hits)
                {
                    if (!c) continue;
                    if (c.transform == transform || c.transform.IsChildOf(transform)) continue;
                    if (hornetRoot && c.transform.root == hornetRoot) continue;
                    var dh = c.GetComponentInParent<DamageHero>();
                    if (dh != null)
                    {
                        var hz = GetHazardType(dh);
                        if (IsTerrainHazard(hz)) return true;
                    }
                }
            }
            catch { }
            return false;
        }

        private void SpawnQuakeImpact(float groundY, int damage)
        {
            var go = new GameObject("ShadeQuakeStrip");
            go.transform.position = new Vector3(transform.position.x, groundY + 0.5f, transform.position.z);
            go.tag = "Hero Spell";
            int spellLayer = LayerMask.NameToLayer("Hero Spell");
            int atkLayer = LayerMask.NameToLayer("Hero Attack");
            if (spellLayer >= 0) go.layer = spellLayer; else if (atkLayer >= 0) go.layer = atkLayer;

            var box = go.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = new Vector2(10f, 1.0f);

            var aoe = go.AddComponent<ShadeAoE>();
            aoe.damage = damage;
            aoe.hornetRoot = hornetTransform;
            aoe.lifeSeconds = 0.25f;

            IgnoreHornetForCollider(box);
        }

        private void SpawnQuakeTeardrop(float groundY, int damage)
        {
            var go = new GameObject("ShadeQuakeTear");
            // Place center so bottom is at ground contact near the shade's position.
            float height = 8f; float width = 6f;
            float centerY = groundY + (height * 0.5f);
            go.transform.position = new Vector3(transform.position.x, centerY, transform.position.z);
            go.tag = "Hero Spell";
            int spellLayer = LayerMask.NameToLayer("Hero Spell");
            int atkLayer = LayerMask.NameToLayer("Hero Attack");
            if (spellLayer >= 0) go.layer = spellLayer; else if (atkLayer >= 0) go.layer = atkLayer;

            var cap = go.AddComponent<CapsuleCollider2D>();
            cap.isTrigger = true;
            cap.direction = CapsuleDirection2D.Vertical;
            cap.size = new Vector2(width, height);

            var aoe = go.AddComponent<ShadeAoE>();
            aoe.damage = damage;
            aoe.hornetRoot = hornetTransform;
            aoe.lifeSeconds = 0.25f;

            IgnoreHornetForCollider(cap);
        }

        private GameObject SpawnDescendAura()
        {
            if (descendAuraAnimFrames == null || descendAuraAnimFrames.Length == 0 || sr == null) return null;
            var go = new GameObject("ShadeDescendAura");
            go.transform.SetParent(transform, false);
            var auraSr = go.AddComponent<SpriteRenderer>();
            auraSr.sortingLayerID = sr.sortingLayerID;
            auraSr.sortingOrder = sr.sortingOrder - 1;
            StartCoroutine(PlayDescendAura(auraSr, descendAuraAnimFrames));
            return go;
        }

        private IEnumerator PlayDescendAura(SpriteRenderer auraSr, Sprite[] frames)
        {
            if (auraSr == null || frames == null || frames.Length == 0) yield break;
            int idx = 0; float timer = 0f; float frameTime = 0.05f;
            while (auraSr)
            {
                timer += Time.deltaTime;
                if (timer >= frameTime)
                {
                    timer -= frameTime;
                    idx = (idx + 1) % frames.Length;
                }
                var frame = frames[idx];
                auraSr.sprite = frame;
                if (sr) auraSr.flipX = sr.flipX;
                float shadeHeight = sr && sr.sprite ? sr.sprite.bounds.size.y * SpriteScale : 0f;
                float auraHeight = frame.bounds.size.y * SpriteScale;
                float auraBottom = -shadeHeight * 0.5f - auraHeight * 0.1f;
                auraSr.transform.localScale = Vector3.one * SpriteScale;
                auraSr.transform.localPosition = new Vector3(0f, auraBottom + auraHeight * 0.5f, 0f);
                yield return null;
            }
        }

        private void SpawnGroundSlamFx(Sprite[] frames, float groundY)
        {
            if (frames == null || frames.Length == 0 || sr == null) return;
            var go = new GameObject("ShadeQuakeSlamFx");
            var fxSr = go.AddComponent<SpriteRenderer>();
            fxSr.sortingLayerID = sr.sortingLayerID;
            fxSr.sortingOrder = sr.sortingOrder - 1;
            float desiredWidth = 15f;
            float spriteWidth = frames[0].bounds.size.x;
            float scale = desiredWidth / spriteWidth;
            go.transform.localScale = new Vector3(scale, scale, 1f);
            float height = frames[0].bounds.size.y * scale;
            go.transform.position = new Vector3(transform.position.x, groundY + height / 2f, transform.position.z);
            StartCoroutine(PlayAndDestroy(fxSr, frames, 0.05f));
        }

        private void SpawnDarkBurstFx(float groundY)
        {
            if (dDarkBurstAnimFrames == null || dDarkBurstAnimFrames.Length == 0 || sr == null) return;
            var go = new GameObject("ShadeDDarkBurstFx");
            var fxSr = go.AddComponent<SpriteRenderer>();
            fxSr.sortingLayerID = sr.sortingLayerID;
            fxSr.sortingOrder = sr.sortingOrder - 1;
            float desiredHeight = 12f;
            float spriteHeight = dDarkBurstAnimFrames[0].bounds.size.y;
            float scale = desiredHeight / spriteHeight;
            go.transform.localScale = new Vector3(scale, scale, 1f);
            float height = spriteHeight * scale;
            go.transform.position = new Vector3(transform.position.x, groundY + height / 2f, transform.position.z);
            StartCoroutine(PlayAndDestroy(fxSr, dDarkBurstAnimFrames, 0.05f));
        }

        private IEnumerator PlayAndDestroy(SpriteRenderer rend, Sprite[] frames, float perFrame)
        {
            for (int i = 0; i < frames.Length; i++)
            {
                if (rend) rend.sprite = frames[i];
                yield return new WaitForSeconds(perFrame);
            }
            if (rend) Destroy(rend.gameObject);
        }

        private void SetupPhysics()
        {
            rb = GetComponent<Rigidbody2D>();
            if (!rb) rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            bodyCol = GetComponent<Collider2D>();
            if (!bodyCol)
            {
                var cap = gameObject.AddComponent<CapsuleCollider2D>();
                cap.direction = CapsuleDirection2D.Vertical;
                cap.size = new Vector2(0.9f, 1.4f);
                cap.isTrigger = false;
                bodyCol = cap;
            }
            else
            {
                bodyCol.isTrigger = false;
            }

            // Add an additional trigger collider to interact with Hornet's attack triggers (for pogo)
            try
            {
                var existingTriggers = GetComponents<Collider2D>();
                bool hasTrigger = false;
                foreach (var c in existingTriggers) if (c && c.isTrigger) { hasTrigger = true; break; }
                if (!hasTrigger)
                {
                    var trigger = gameObject.AddComponent<CapsuleCollider2D>();
                    trigger.direction = CapsuleDirection2D.Vertical;
                    trigger.size = new Vector2(0.95f, 1.5f);
                    trigger.isTrigger = true;
                }
            }
            catch { }

            try
            {
                var hc = HeroController.instance;
                if (hc)
                {
                    // Place shade on a non-hero layer to avoid triggering transitions and interactables
                    int heroLayer = hc.gameObject.layer; // typically 9
                    int desiredLayer = LayerMask.NameToLayer("Default");
                    if (desiredLayer < 0 || desiredLayer == heroLayer)
                    {
                        // Fallback to a safe built-in layer that is not the hero layer
                        int ignoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
                        desiredLayer = (ignoreRaycast >= 0 && ignoreRaycast != heroLayer) ? ignoreRaycast : 0;
                    }

                    gameObject.layer = desiredLayer;
                    // Apply to immediate children we control
                    var allTransforms = GetComponentsInChildren<Transform>(true);
                    foreach (var t in allTransforms)
                    {
                        if (!t) continue;
                        t.gameObject.layer = desiredLayer;
                    }

                    // Still ignore collisions with Hornet (handled in a helper so we can call it later too)
                    EnsureIgnoreHornetCollisions();

                    // Initial enemy ignore pass
                    RefreshCollisionIgnores();
                }
            }
            catch { }
        }

        private void RefreshCollisionIgnores()
        {
            try
            {
                var myCols = GetComponentsInChildren<Collider2D>(true);
                if (myCols == null || myCols.Length == 0) return;

                // Ignore physical collisions with enemies (objects with HealthManager) but keep triggers for damage/hazards
                HealthManager[] enemies = null;
                try { enemies = Object.FindObjectsOfType<HealthManager>(); }
                catch { enemies = null; }
                if (enemies != null)
                {
                    foreach (var hm in enemies)
                    {
                        if (!hm) continue;
                        var cols = hm.GetComponentsInChildren<Collider2D>(true);
                        foreach (var ec in cols)
                        {
                            if (!ec || ec.isTrigger) continue; // don't ignore triggers to still receive hazard/damage events
                            foreach (var mc in myCols) if (mc) Physics2D.IgnoreCollision(mc, ec, true);
                        }
                    }
                }
            }
            catch { }
        }

        private void EnsureIgnoreHornetCollisions()
        {
            try
            {
                var hc = HeroController.instance;
                if (!hc) return;
                var myCols = GetComponentsInChildren<Collider2D>(true);
                var hornetCols = hc.transform.root.GetComponentsInChildren<Collider2D>(true);
                int heroAttackLayer = LayerMask.NameToLayer("Hero Attack");
                foreach (var mc in myCols)
                {
                    if (!mc) continue;
                    foreach (var hcCol in hornetCols)
                    {
                        if (!hcCol) continue;
                        if (mc.isTrigger || hcCol.isTrigger) continue;
                        if (hcCol.gameObject.layer == heroAttackLayer) continue; // allow hero attack contact
                        // Allow slashes (which may not be on Hero Attack layer) by checking their components
                        bool isSlash = false;
                        try { if (hcCol.GetComponentInParent<NailSlashTerrainThunk>()) isSlash = true; } catch { }
                        if (isSlash) continue;
                        Physics2D.IgnoreCollision(mc, hcCol, true);
                    }
                }
            }
            catch { }
        }

        private void EnableCollisions(bool enable)
        {
            try
            {
                if (bodyCol) bodyCol.enabled = enable;
                var extraCols = GetComponentsInChildren<Collider2D>(true);
                foreach (var c in extraCols) if (c && c != bodyCol) c.enabled = enable;
            }
            catch { }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            TryProcessDamageHero(collision.collider);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryProcessDamageHero(other);
        }

        private static readonly System.Collections.Generic.HashSet<string> DamageTags = new()
        {
            "Enemy Damager",
            "EnemyDamager",
            "Damager",
            "Hazard"
        };

        private static bool HasDamageTag(Collider2D col)
        {
            try { return DamageTags.Contains(col.tag); }
            catch { return false; }
        }

        private static bool CanDamageShade(DamageHero dh)
        {
            if (dh == null) return false;
            try
            {
                if (!dh.enabled || !dh.CanCauseDamage) return false;
                if (dh.damageDealt <= 0) return false;
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void TryProcessDamageHero(Collider2D col)
        {
            if (!col) return;
            try
            {
                if (bodyCol && col && !bodyCol.IsTouching(col)) return;
                if (col.transform == transform || col.transform.IsChildOf(transform)) return;
                if (hornetTransform)
                {
                    var hornetRoot = hornetTransform.root;
                    if (col.transform == hornetTransform || col.transform.IsChildOf(hornetTransform) || col.transform.root == hornetRoot)
                        return;
                }
                var dh = col.GetComponent<DamageHero>();
                if (dh != null && HasDamageTag(col) && CanDamageShade(dh))
                {
                    var hz = GetHazardType(dh);
                    if (IsTerrainHazard(hz)) { LogShadeDamage(dh, col); OnShadeHitHazard(); return; }
                    LogShadeDamage(dh, col);
                    OnShadeHitEnemy(dh);
                }
            }
            catch { }
        }

        private void LogShadeDamage(DamageHero dh, Collider2D src)
        {
            try
            {
                string obj = dh ? dh.gameObject?.name ?? dh.name : "<null>";
                string colName = src ? src.name : "<null>";
                int dmg = 0; try { dmg = dh.damageDealt; } catch { }
                var hz = GetHazardType(dh);
                UnityEngine.Debug.Log($"[ShadeDebug] Hurt by {obj} via {colName} hazard={hz} dmg={dmg}");
            }
            catch { }
        }

        private void TeleportToHornet()
        {
            if (!hornetTransform) return;
            bool hadSim = rb ? rb.simulated : false;
            if (rb) rb.simulated = false;
            transform.position = hornetTransform.position;
            if (rb)
            {
                rb.linearVelocity = Vector2.zero;
                rb.simulated = hadSim;
            }
        }

        public void TeleportToPosition(Vector3 position)
        {
            bool hadSim = rb ? rb.simulated : false;
            if (rb) rb.simulated = false;
            transform.position = position;
            if (rb)
            {
                rb.linearVelocity = Vector2.zero;
                rb.simulated = hadSim;
            }
        }

        private Vector2 ClampAgainstTransitionGates(Vector2 proposed)
        {
            try
            {
                if (!bodyCol) return proposed;
                // Approximate shade bounds at proposed position using current extents
                var ext = bodyCol.bounds.extents;
                Vector2 min = proposed - (Vector2)ext;
                Vector2 max = proposed + (Vector2)ext;

                var hits = Physics2D.OverlapAreaAll(min, max);
                if (hits == null || hits.Length == 0) return proposed;

                foreach (var h in hits)
                {
                    if (!h) continue;
                    var tp = h.GetComponentInParent<TransitionPoint>();
                    if (tp == null) continue;
                    bool isDoor = false;
                    try { isDoor = tp.isADoor; } catch { }
                    if (isDoor) continue; // block only edge-of-map gates

                    var gb = h.bounds;
                    var gatePos = tp.GetGatePosition();
                    switch (gatePos)
                    {
                        case GlobalEnums.GatePosition.right:
                            if (proposed.x > gb.min.x - ext.x)
                                proposed.x = gb.min.x - ext.x;
                            break;
                        case GlobalEnums.GatePosition.left:
                            if (proposed.x < gb.max.x + ext.x)
                                proposed.x = gb.max.x + ext.x;
                            break;
                        case GlobalEnums.GatePosition.top:
                            if (proposed.y > gb.min.y - ext.y)
                                proposed.y = gb.min.y - ext.y;
                            break;
                        case GlobalEnums.GatePosition.bottom:
                            if (proposed.y < gb.max.y + ext.y)
                                proposed.y = gb.max.y + ext.y;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch { }
            return proposed;
        }

        private void CheckHazardOverlap()
        {
            if (hazardCooldown > 0f) return;
            if (!bodyCol) return;
            var filter = new ContactFilter2D();
            filter.useTriggers = true;
            Collider2D[] results = new Collider2D[16];
            int count = bodyCol.Overlap(filter, results);
            for (int i = 0; i < count; i++)
            {
                var c = results[i];
                if (!c) continue;
                if (c.transform == transform || c.transform.IsChildOf(transform)) continue;
                if (hornetTransform && c.transform.root == hornetTransform.root) continue;
                var dh = c.GetComponent<DamageHero>();
                if (dh != null && HasDamageTag(c) && CanDamageShade(dh))
                {
                    var hz = GetHazardType(dh);
                    if (IsTerrainHazard(hz)) { LogShadeDamage(dh, c); OnShadeHitHazard(); return; }
                    LogShadeDamage(dh, c);
                    OnShadeHitEnemy(dh); return;
                }
            }
        }

        private static GlobalEnums.HazardType GetHazardType(DamageHero dh)
        {
            try { return dh.hazardType; }
            catch { return GlobalEnums.HazardType.NON_HAZARD; }
        }

        private void OnShadeHitHazard()
        {
            if (hazardCooldown > 0f) return;
            TeleportToHornet();
            if (canTakeDamage)
            {
                shadeHP = Mathf.Max(0, shadeHP - 1);
                if (shadeHP <= 0) StartDeathAnimation();
            }
            PushShadeStatsToHud();
            hazardCooldown = 0.25f;
            CancelFocus();
            PersistIfChanged();
        }

        private void OnShadeHitEnemy(DamageHero dh)
        {
            if (hurtCooldown > 0f) return;
            int dmg = 0;
            try { if (dh != null) dmg = dh.damageDealt; } catch { }
            if (dmg <= 0) return; // ignore non-damaging triggers
            if (!canTakeDamage)
            {
                hurtCooldown = HurtIFrameSeconds;
                return;
            }
            shadeHP = Mathf.Max(0, shadeHP - dmg);
            if (shadeHP <= 0) StartDeathAnimation();
            PushShadeStatsToHud();
            hurtCooldown = HurtIFrameSeconds;
            CancelFocus();
            PersistIfChanged();
        }

        private void StartDeathAnimation()
        {
            if (isDying) return;
            if (deathRoutine != null) StopCoroutine(deathRoutine);
            isDying = true;
            deathRoutine = StartCoroutine(DeathAnimationRoutine());
        }

        private void CancelDeathAnimation()
        {
            if (!isDying)
            {
                if (deathRoutine != null) { StopCoroutine(deathRoutine); deathRoutine = null; }
                return;
            }
            if (deathRoutine != null) StopCoroutine(deathRoutine);
            deathRoutine = null;
            isDying = false;
            isCastingSpell = false;
            currentAnimFrames = null;
        }

        private IEnumerator DeathAnimationRoutine()
        {
            isDying = true;
            isCastingSpell = true;
            if (deathAnimFrames != null && deathAnimFrames.Length > 0)
            {
                currentAnimFrames = deathAnimFrames;
                float perFrame = 0.5f / deathAnimFrames.Length;
                for (int i = 0; i < deathAnimFrames.Length; i++)
                {
                    if (shadeHP > 0)
                    {
                        isCastingSpell = false;
                        isDying = false;
                        currentAnimFrames = null;
                        yield break;
                    }
                    if (sr) sr.sprite = deathAnimFrames[i];
                    yield return new WaitForSeconds(perFrame);
                }
            }
            else
            {
                float t = 0f;
                while (t < 0.5f)
                {
                    if (shadeHP > 0)
                    {
                        isCastingSpell = false;
                        isDying = false;
                        yield break;
                    }
                    t += Time.deltaTime;
                    yield return null;
                }
            }
            currentAnimFrames = null;
            isCastingSpell = false;
            isDying = false;
            deathRoutine = null;
        }

        private void HandleFocus()
        {
            // Already channeling
            if (isFocusing)
            {
                // Cancel if key released or interrupted by teleport
                if (!Input.GetKey(FocusKey) || isChannelingTeleport || inHardLeash || isInactive)
                {
                    CancelFocus();
                    return;
                }

                // Show/update aura
                EnsureFocusAura();
                try
                {
                    if (focusAuraRenderer)
                    {
                        focusAuraRenderer.enabled = true;
                        var t = focusAuraRenderer.transform;
                        float pulse = 1f + 0.12f * Mathf.Sin(Time.time * 15f);
                        float size = focusAuraBaseSize * pulse;
                        t.localScale = new Vector3(size, size, 1f);
                    }
                }
                catch { }

                // Drain soul over time while channeling
                float drainRate = Mathf.Max(0.01f, (float)focusSoulCost / Mathf.Max(0.05f, focusChannelTime)); // soul per second
                focusSoulAccumulator += drainRate * Time.deltaTime;
                int drainThisFrame = Mathf.FloorToInt(focusSoulAccumulator);
                if (drainThisFrame > 0)
                {
                    focusSoulAccumulator -= drainThisFrame;
                    int beforeSoul = shadeSoul;
                    shadeSoul = Mathf.Max(0, shadeSoul - drainThisFrame);
                    if (shadeSoul != beforeSoul)
                    {
                        PushSoulToHud();
                    }
                    if (shadeSoul <= 0)
                    {
                        // Ran out of soul mid-channel; cancel with no benefit
                        CancelFocus();
                        return;
                    }
                }

                focusTimer -= Time.deltaTime;
                if (focusTimer > 0f) return;

                // Complete focus
                int canHeal = (shadeHP < shadeMaxHP) ? 1 : 0;
                if (canHeal > 0)
                {
                    int before = shadeHP;
                    shadeHP = Mathf.Min(shadeHP + canHeal, shadeMaxHP);
                    if (shadeHP != before)
                    {
                        if (shadeHP > 0)
                        {
                            isInactive = false;
                            CancelDeathAnimation();
                        }
                        PushShadeStatsToHud();
                    }

                    // Heal Hornet if close
                    try
                    {
                        var hc = HeroController.instance;
                        if (hc != null && hc.transform != null)
                        {
                            float dist = Vector2.Distance(hc.transform.position, transform.position);
                            if (dist <= focusHealRange)
                            {
                                // Avoid exceeding max via AddHealth handling
                                hc.AddHealth(1);
                            }
                        }
                    }
                    catch { }

                    // Play complete SFX
                    TryPlayFocusCompleteSfx();
                }

                // End channel regardless of success
                isFocusing = false;
                isCastingSpell = false;
                try { if (sr) { var c = sr.color; c.a = 0.9f; sr.color = c; } } catch { }
                try { if (focusAuraRenderer) focusAuraRenderer.enabled = false; } catch { }
                StopFocusChargeSfx();
                focusSoulAccumulator = 0f;
                PersistIfChanged();
                return;
            }

            // Start focus when holding key with enough soul and missing HP
            if (!Input.GetKey(FocusKey)) return;
            if (isCastingSpell || isChannelingTeleport || inHardLeash || isInactive) return;
            if (shadeHP >= shadeMaxHP) return; // already full
            if (shadeSoul < focusSoulCost) return; // not enough soul

            isFocusing = true;
            isCastingSpell = true;
            focusTimer = Mathf.Max(0.05f, focusChannelTime);
            try { if (sr) { var c = sr.color; c.a = focusAlphaWhileChannel; sr.color = c; } } catch { }
            focusSoulAccumulator = 0f;
            EnsureFocusAura();
            try { if (focusAuraRenderer) focusAuraRenderer.enabled = true; } catch { }
            StartFocusChargeSfx();
        }

        private void CancelFocus()
        {
            if (!isFocusing) return;
            isFocusing = false;
            isCastingSpell = false;
            try { if (sr) { var c = sr.color; c.a = 0.9f; sr.color = c; } } catch { }
            try { if (focusAuraRenderer) focusAuraRenderer.enabled = false; } catch { }
            StopFocusChargeSfx();
            focusSoulAccumulator = 0f;
        }

        private void EnsureFocusAura()
        {
            try
            {
                if (focusAuraRenderer && focusAuraRenderer.gameObject)
                    return;
                // Create aura
                var go = new GameObject("ShadeFocusAura");
                go.transform.SetParent(transform, false);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                EnsureSimpleLightResources();
                var mf = go.AddComponent<MeshFilter>();
                mf.sharedMesh = s_simpleQuadMesh;
                var mr = go.AddComponent<MeshRenderer>();
                mr.sharedMaterial = s_simpleAdditiveMat;
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.receiveShadows = false;
                var shadeSR = GetComponent<SpriteRenderer>();
                mr.sortingLayerID = shadeSR ? shadeSR.sortingLayerID : 0;
                mr.sortingOrder = shadeSR ? (shadeSR.sortingOrder - 2) : -2;
                go.transform.localScale = new Vector3(focusAuraBaseSize, focusAuraBaseSize, 1f);
                focusAuraRenderer = mr;
                mr.enabled = false;
            }
            catch { }
        }

        private void EnsureFocusSfx()
        {
            try
            {
                if (focusSfx == null)
                {
                    var go = new GameObject("ShadeFocusSFX");
                    go.transform.SetParent(transform, false);
                    go.transform.localPosition = Vector3.zero;
                    focusSfx = go.AddComponent<AudioSource>();
                    focusSfx.playOnAwake = false;
                    focusSfx.spatialBlend = 0f; // 2D; set to small 3D if desired
                    focusSfx.volume = 1f;
                }

                // Prefer HK1 SFX dropped into the mod Assets folder (wav)
                // Primary (per your filenames), with fallback aliases
                if (sfxFocusCharge == null)
                    sfxFocusCharge = TryLoadAudioFromAssets("focus_health_charging.wav") ?? TryLoadAudioFromAssets("focus_charge.wav");
                if (sfxFocusComplete == null)
                    sfxFocusComplete = TryLoadAudioFromAssets("focus_health_heal.wav") ?? TryLoadAudioFromAssets("focus_complete.wav");
                if (sfxFocusReady == null)
                    sfxFocusReady = TryLoadAudioFromAssets("focus_ready.wav");

                if (sfxFocusCharge == null || sfxFocusComplete == null || sfxFocusReady == null)
                {
                    var all = Resources.FindObjectsOfTypeAll<AudioClip>();
                    AudioClip bestCharge = null; int bestChargeScore = int.MinValue;
                    AudioClip bestComplete = null; int bestCompleteScore = int.MinValue;
                    AudioClip bestReady = null; int bestReadyScore = int.MinValue;
                    foreach (var c in all)
                    {
                        if (!c) continue; string n = c.name ?? string.Empty; string lname = n.ToLowerInvariant();
                        int chargeScore = 0;
                        if (lname.Contains("focus")) chargeScore += 5;
                        if (lname.Contains("charge") || lname.Contains("loop") || lname.Contains("start")) chargeScore += 3;
                        if (lname.Contains("spell")) chargeScore += 1;
                        if (lname.Contains("bind")) chargeScore += 1; // fallback to Silksong bind if no focus
                        if (chargeScore > bestChargeScore) { bestChargeScore = chargeScore; bestCharge = c; }

                        int completeScore = 0;
                        if (lname.Contains("focus")) completeScore += 4;
                        if (lname.Contains("heal") || lname.Contains("end") || lname.Contains("complete") || lname.Contains("release")) completeScore += 4;
                        if (lname.Contains("spell")) completeScore += 1;
                        if (lname.Contains("bind")) completeScore += 1; // fallback
                        if (completeScore > bestCompleteScore) { bestCompleteScore = completeScore; bestComplete = c; }

                        int readyScore = 0;
                        if (lname.Contains("focus")) readyScore += 3;
                        if (lname.Contains("ready") || lname.Contains("available") || lname.Contains("charge_complete") || lname.Contains("full")) readyScore += 3;
                        if (lname.Contains("bind")) readyScore += 1;
                        if (readyScore > bestReadyScore) { bestReadyScore = readyScore; bestReady = c; }
                    }
                    if (bestChargeScore > 0 && sfxFocusCharge == null) sfxFocusCharge = bestCharge;
                    if (bestCompleteScore > 0 && sfxFocusComplete == null) sfxFocusComplete = bestComplete;
                    if (bestReadyScore > 0 && sfxFocusReady == null) sfxFocusReady = bestReady;
                }
            }
            catch { }
        }

        private static AudioClip TryLoadAudioFromAssets(string fileName)
        {
            try
            {
                string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string assets = Path.Combine(dir, "Assets");
                string path = Path.Combine(assets, fileName);
                if (!File.Exists(path)) return null;
                // WAV only for now (16-bit PCM). Keep simple and dependency-free.
                if (path.EndsWith(".wav", System.StringComparison.OrdinalIgnoreCase))
                {
                    return LoadPcmWav(path);
                }
            }
            catch { }
            return null;
        }

        private static AudioClip LoadPcmWav(string path)
        {
            try
            {
                using (var fs = File.OpenRead(path))
                using (var br = new BinaryReader(fs))
                {
                    // RIFF header
                    if (new string(br.ReadChars(4)) != "RIFF") return null;
                    br.ReadInt32(); // Chunk size
                    if (new string(br.ReadChars(4)) != "WAVE") return null;

                    int channels = 1;
                    int sampleRate = 44100;
                    int bitsPerSample = 16;
                    int dataSize = 0;
                    long dataPos = 0;

                    // Read chunks
                    while (br.BaseStream.Position + 8 <= br.BaseStream.Length)
                    {
                        string chunkId = new string(br.ReadChars(4));
                        int chunkSize = br.ReadInt32();
                        long next = br.BaseStream.Position + chunkSize;

                        if (chunkId == "fmt ")
                        {
                            int audioFormat = br.ReadInt16();
                            channels = br.ReadInt16();
                            sampleRate = br.ReadInt32();
                            br.ReadInt32(); // byteRate
                            br.ReadInt16(); // blockAlign
                            bitsPerSample = br.ReadInt16();
                            if (chunkSize > 16)
                            {
                                // skip any extra bytes in fmt chunk
                                br.BaseStream.Position = next;
                            }
                            if (audioFormat != 1) return null; // PCM only
                        }
                        else if (chunkId == "data")
                        {
                            dataPos = br.BaseStream.Position;
                            dataSize = chunkSize;
                            br.BaseStream.Position = next;
                        }
                        else
                        {
                            // Skip unknown chunks
                            br.BaseStream.Position = next;
                        }
                    }

                    if (dataPos == 0 || dataSize <= 0) return null;

                    // Read samples
                    fs.Position = dataPos;
                    int bytesPerSample = bitsPerSample / 8;
                    int totalSamples = dataSize / bytesPerSample;
                    int sampleCountPerChannel = totalSamples / channels;
                    float[] data = new float[sampleCountPerChannel * channels];

                    if (bitsPerSample == 16)
                    {
                        for (int i = 0; i < totalSamples; i++)
                        {
                            short s = br.ReadInt16();
                            data[i] = Mathf.Clamp(s / 32768f, -1f, 1f);
                        }
                    }
                    else if (bitsPerSample == 8)
                    {
                        for (int i = 0; i < totalSamples; i++)
                        {
                            byte b = br.ReadByte();
                            data[i] = (b - 128) / 128f;
                        }
                    }
                    else
                    {
                        return null;
                    }

                    // Avoid SetData to keep compatibility with some UnityEngine builds.
                    // Use streaming clip with a PCM reader callback.
                    string name = Path.GetFileNameWithoutExtension(path);
                    int pos = 0;
                    var clip = AudioClip.Create(name, sampleCountPerChannel, channels, sampleRate, true,
                        (float[] outData) =>
                        {
                            int len = outData.Length;
                            for (int i = 0; i < len; i++)
                            {
                                outData[i] = (pos < data.Length) ? data[pos++] : 0f;
                            }
                        },
                        (int newPosition) =>
                        {
                            // newPosition is per-channel sample index
                            pos = Mathf.Clamp(newPosition * channels, 0, data.Length);
                        }
                    );
                    return clip;
                }
            }
            catch { }
            return null;
        }

        private void StartFocusChargeSfx()
        {
            try
            {
                EnsureFocusSfx();
                if (focusSfx != null && sfxFocusCharge != null)
                {
                    focusSfx.loop = true;
                    focusSfx.clip = sfxFocusCharge;
                    focusSfx.Play();
                }
            }
            catch { }
        }

        private void StopFocusChargeSfx()
        {
            try
            {
                if (focusSfx != null)
                {
                    focusSfx.loop = false;
                    focusSfx.Stop();
                }
            }
            catch { }
        }

        private void TryPlayFocusCompleteSfx()
        {
            try
            {
                EnsureFocusSfx();
                if (focusSfx != null && sfxFocusComplete != null)
                {
                    focusSfx.PlayOneShot(sfxFocusComplete);
                }
            }
            catch { }
        }

        private void CheckFocusReadySfx()
        {
            try
            {
                if (lastSoulForReady < 0) { lastSoulForReady = shadeSoul; return; }
                if (lastSoulForReady < focusSoulCost && shadeSoul >= focusSoulCost)
                {
                    EnsureFocusSfx();
                    if (focusSfx != null && sfxFocusReady != null)
                        focusSfx.PlayOneShot(sfxFocusReady);
                }
                lastSoulForReady = shadeSoul;
            }
            catch { }
        }

        // ========== Spell SFX (Projectile, Shriek, Quake) ==========
        private AudioSource spellSfx;
        private AudioClip sfxFireball;
        private AudioClip sfxQuakePrepare;
        private AudioClip sfxQuakeImpact;
        private AudioClip sfxVoidQuakeImpact;
        private AudioClip sfxScream;
        private AudioClip sfxVoidScream;

        private void EnsureSpellSfx()
        {
            try
            {
                if (spellSfx == null)
                {
                    var go = new GameObject("ShadeSpellSFX");
                    go.transform.SetParent(transform, false);
                    go.transform.localPosition = Vector3.zero;
                    spellSfx = go.AddComponent<AudioSource>();
                    spellSfx.playOnAwake = false;
                    spellSfx.spatialBlend = 0f;
                    spellSfx.volume = 1f;
                }
                if (sfxFireball == null) sfxFireball = TryLoadAudioFromAssets("hero_fireball.wav");
                if (sfxQuakePrepare == null) sfxQuakePrepare = TryLoadAudioFromAssets("hero_quake_spell_prepare.wav");
                if (sfxQuakeImpact == null) sfxQuakeImpact = TryLoadAudioFromAssets("hero_quake_spell_impact.wav");
                if (sfxVoidQuakeImpact == null) sfxVoidQuakeImpact = TryLoadAudioFromAssets("hero_void_quake_impact.wav");
                if (sfxScream == null) sfxScream = TryLoadAudioFromAssets("hero_scream_spell.wav");
                if (sfxVoidScream == null) sfxVoidScream = TryLoadAudioFromAssets("hero_void_scream_spell.wav");

                if (sfxFireball == null || sfxQuakePrepare == null || sfxQuakeImpact == null || sfxVoidQuakeImpact == null || sfxScream == null || sfxVoidScream == null)
                {
                    var all = Resources.FindObjectsOfTypeAll<AudioClip>();
                    AudioClip best(string[] keys)
                    {
                        AudioClip pick = null; int scoreBest = int.MinValue;
                        foreach (var c in all)
                        {
                            if (!c) continue; string n = (c.name ?? string.Empty).ToLowerInvariant();
                            int sc = 0; foreach (var k in keys) if (n.Contains(k)) sc += 2; // favor multiple matches
                            if (sc > scoreBest) { scoreBest = sc; pick = c; }
                        }
                        return pick;
                    }
                    if (sfxFireball == null) sfxFireball = best(new[] { "fireball", "vengeful", "spirit", "spell" });
                    if (sfxQuakePrepare == null) sfxQuakePrepare = best(new[] { "quake", "prepare", "start", "spell" });
                    if (sfxQuakeImpact == null) sfxQuakeImpact = best(new[] { "quake", "impact", "spell" });
                    if (sfxVoidQuakeImpact == null) sfxVoidQuakeImpact = best(new[] { "void", "quake", "impact" });
                    if (sfxScream == null) sfxScream = best(new[] { "scream", "wraith", "howl", "spell" });
                    if (sfxVoidScream == null) sfxVoidScream = best(new[] { "void", "scream", "abyss" });
                }
            }
            catch { }
        }

        private void TryPlayFireballSfx()
        {
            try
            {
                EnsureSpellSfx();
                if (spellSfx != null && sfxFireball != null) spellSfx.PlayOneShot(sfxFireball);
            }
            catch { }
        }

        private void TryPlayShriekSfx(bool upgraded)
        {
            try
            {
                EnsureSpellSfx();
                var clip = upgraded ? sfxVoidScream : sfxScream;
                if (spellSfx != null && clip != null) spellSfx.PlayOneShot(clip);
            }
            catch { }
        }

        private void TryPlayQuakePrepareSfx()
        {
            try
            {
                EnsureSpellSfx();
                if (spellSfx != null && sfxQuakePrepare != null) spellSfx.PlayOneShot(sfxQuakePrepare);
            }
            catch { }
        }

        private void TryPlayQuakeImpactSfx(bool upgraded)
        {
            try
            {
                EnsureSpellSfx();
                var clip = upgraded ? sfxVoidQuakeImpact : sfxQuakeImpact;
                if (spellSfx != null && clip != null) spellSfx.PlayOneShot(clip);
            }
            catch { }
        }

        private void SetupShadeLight()
        {
            try
            {
                var lightGO = new GameObject("ShadeLightSimple");
                lightGO.transform.SetParent(transform, false);
                lightGO.transform.localPosition = Vector3.zero;
                lightGO.transform.localRotation = Quaternion.identity;
                EnsureSimpleLightResources();

                var mf = lightGO.AddComponent<MeshFilter>();
                mf.sharedMesh = s_simpleQuadMesh;
                var mr = lightGO.AddComponent<MeshRenderer>();
                mr.sharedMaterial = s_simpleAdditiveMat;
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.receiveShadows = false;
                var shadeSR = GetComponent<SpriteRenderer>();
                mr.sortingLayerID = shadeSR ? shadeSR.sortingLayerID : 0;
                mr.sortingOrder = shadeSR ? (shadeSR.sortingOrder - 1) : -1;
                lightGO.transform.localScale = new Vector3(simpleLightSize, simpleLightSize, 1f);
                shadeLightRenderers = new Renderer[] { mr };
            }
            catch { }
        }

        private void SyncShadeLight()
        {
            try
            {
                if (shadeLightRenderers == null) return;
                var shadeSR = GetComponent<SpriteRenderer>();
                int baseLayer = shadeSR ? shadeSR.sortingLayerID : 0;
                int baseOrder = shadeSR ? shadeSR.sortingOrder : 0;
                foreach (var r in shadeLightRenderers)
                {
                    if (!r) continue;
                    r.enabled = true;
                    r.sortingLayerID = baseLayer;
                    r.sortingOrder = baseOrder - 1;
                }
                // Keep focus aura sorted just below the shade sprite as well
                if (focusAuraRenderer)
                {
                    focusAuraRenderer.sortingLayerID = baseLayer;
                    focusAuraRenderer.sortingOrder = baseOrder - 2;
                }
            }
            catch { }
        }

        private IEnumerator EnableShadeLightNextFrame()
        {
            yield return null;
        }

        private static void EnsureSimpleLightResources()
        {
            try
            {
                if (s_simpleQuadMesh == null)
                {
                    s_simpleQuadMesh = new Mesh();
                    s_simpleQuadMesh.name = "ShadeLightQuad";
                    s_simpleQuadMesh.vertices = new Vector3[]
                    {
                        new Vector3(-0.5f, -0.5f, 0f),
                        new Vector3( 0.5f, -0.5f, 0f),
                        new Vector3(-0.5f,  0.5f, 0f),
                        new Vector3( 0.5f,  0.5f, 0f)
                    };
                    s_simpleQuadMesh.uv = new Vector2[] {
                        new Vector2(0,0), new Vector2(1,0), new Vector2(0,1), new Vector2(1,1)
                    };
                    s_simpleQuadMesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };
                    s_simpleQuadMesh.RecalculateNormals();
                }
                if (s_simpleLightTex == null)
                {
                    int size = 128;
                    s_simpleLightTex = new Texture2D(size, size, TextureFormat.ARGB32, false);
                    s_simpleLightTex.filterMode = FilterMode.Bilinear;
                    for (int y = 0; y < size; y++)
                    {
                        for (int x = 0; x < size; x++)
                        {
                            float nx = (x + 0.5f) / size * 2f - 1f;
                            float ny = (y + 0.5f) / size * 2f - 1f;
                            float r = Mathf.Sqrt(nx * nx + ny * ny);
                            float a = Mathf.Clamp01(1f - r);
                            a = Mathf.Pow(a, 3.5f) * 0.55f;
                            s_simpleLightTex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                        }
                    }
                    s_simpleLightTex.Apply();
                }
                if (s_simpleAdditiveMat == null)
                {
                    var shader = Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Transparent");
                    s_simpleAdditiveMat = new Material(shader)
                    {
                        name = "ShadeLightAdditiveMat",
                        mainTexture = s_simpleLightTex,
                        renderQueue = 3000
                    };
                    try { s_simpleAdditiveMat.SetColor("_Color", new Color(1f, 1f, 1f, 0.35f)); } catch { }
                }
            }
            catch { }
        }

        private static bool IsTerrainHazard(GlobalEnums.HazardType hz)
        {
            switch (hz)
            {
                case GlobalEnums.HazardType.SPIKES:
                case GlobalEnums.HazardType.ACID:
                case GlobalEnums.HazardType.LAVA:
                case GlobalEnums.HazardType.PIT:
                case GlobalEnums.HazardType.COAL:
                case GlobalEnums.HazardType.ZAP:
                case GlobalEnums.HazardType.SINK:
                case GlobalEnums.HazardType.STEAM:
                case GlobalEnums.HazardType.COAL_SPIKES:
                case GlobalEnums.HazardType.RESPAWN_PIT:
                    return true;
                default:
                    return false;
            }
        }

        private void HandleTeleportChannel()
        {
            teleportCooldownTimer = Mathf.Max(0f, teleportCooldownTimer - Time.deltaTime);

            // Start channel
            if (!isChannelingTeleport && teleportCooldownTimer <= 0f && Input.GetKeyDown(TeleportKey))
            {
                isChannelingTeleport = true;
                teleportChannelTimer = teleportChannelTime;
            }

            if (!isChannelingTeleport) return;

            // Visual hint: fade sprite slightly while channeling
            try
            {
                if (sr)
                {
                    var c = sr.color; c.a = 0.6f; sr.color = c;
                }
            }
            catch { }

            teleportChannelTimer -= Time.deltaTime;
            if (teleportChannelTimer <= 0f)
            {
                TeleportToHornet();
                teleportCooldownTimer = teleportCooldown;
                isChannelingTeleport = false;
                // restore sprite alpha
                try { if (sr) { var c = sr.color; c.a = 0.9f; sr.color = c; } } catch { }
            }

            // Cancel on movement or attack input
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(NailKey) || Input.GetKeyDown(FireKey))
            {
                isChannelingTeleport = false;
                try { if (sr) { var c = sr.color; c.a = 0.9f; sr.color = c; } } catch { }
            }
        }

        private int GetHornetNailDamage()
        {
            try
            {
                var gm = GameManager.instance;
                var pd = gm != null ? gm.playerData : null;
                if (pd == null) return 5;
                int baseDmg = Mathf.Max(1, pd.nailDamage);
                bool bound = false;
                try { bound = BossSequenceController.BoundNail; } catch { bound = false; }
                if (bound)
                {
                    int boundVal = 0;
                    try { boundVal = BossSequenceController.BoundNailDamage; } catch { boundVal = baseDmg; }
                    return Mathf.Min(baseDmg, Mathf.Max(1, boundVal));
                }
                return baseDmg;
            }
            catch { return 5; }
        }
    }
}
