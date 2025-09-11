using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Reflection;
using InControl;

[BepInPlugin("com.legacyoftheabyss.helper", "Legacy of the Abyss - Helper", "0.1.0")]
public class LegacyHelper : BaseUnityPlugin
{
    private static GameObject helper;
    private static bool loggedStartupFields;
    private static SimpleHUD hud;

    void Awake()
    {
        Logger.LogInfo("Patching GameManager.BeginScene...");
        Harmony harmony = new Harmony("com.legacyoftheabyss.helper");
        harmony.PatchAll();

        SceneManager.sceneLoaded += (scene, mode) =>
        {
            foreach (var go in scene.GetRootGameObjects())
            {
                var name = go.name.ToLowerInvariant();
                if (name.Contains("team cherry") || (name.Contains("save") && name.Contains("reminder")))
                    go.SetActive(false);
            }
        };
    }

    internal static void DisableStartup(GameManager gm)
    {
        if (gm == null) return;
        var t = gm.GetType();
        var fields = t.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        bool log = !loggedStartupFields;
        loggedStartupFields = true;
        foreach (var f in fields)
        {
            if (f.FieldType != typeof(bool)) continue;
            if (log)
            {
                bool val = false;
                try { val = (bool)f.GetValue(gm); } catch { }
                Debug.Log($"[HelperMod] GameManager bool field {f.Name}={val}");
            }
            var name = f.Name.ToLower();
            if (name.Contains("logo") || (name.Contains("save") && name.Contains("reminder")))
            {
                try
                {
                    f.SetValue(gm, false);
                    Debug.Log($"[HelperMod] Disabled GameManager field {f.Name}");
                }
                catch { }
            }
        }
    }

    [HarmonyPatch(typeof(GameManager), "BeginScene")]
    class GameManager_BeginScene_Patch
    {
        static void Postfix(GameManager __instance)
        {
            DisableStartup(__instance);
            bool gameplay = __instance.IsGameplayScene();
            // Toggle HUD visibility based on scene type
            if (hud != null)
            {
                try { hud.SetVisible(gameplay); } catch { }
            }
            if (!gameplay)
                return;

            Debug.Log("[HelperMod] Gameplay scene detected, spawning helper.");

            if (helper == null)
            {
                helper = new GameObject("HelperShade");
                helper.transform.position = __instance.hero_ctrl.transform.position;

                var sc = helper.AddComponent<ShadeController>();
                sc.Init(__instance.hero_ctrl.transform);

                var sr = helper.AddComponent<SpriteRenderer>();
                sr.sprite = GenerateDebugSprite();
                sr.color = Color.black;

                var hornetRenderer = __instance.hero_ctrl.GetComponentInChildren<SpriteRenderer>();
                if (hornetRenderer != null)
                {
                    sr.sortingLayerID = hornetRenderer.sortingLayerID;
                    sr.sortingOrder = hornetRenderer.sortingOrder + 1; //Render layers are weird, anything behind hornets layer seems to vanish entirely
                }
            }

            if (hud == null)
            {
                var hudGO = new GameObject("SimpleHUD");
                GameObject.DontDestroyOnLoad(hudGO);
                hud = hudGO.AddComponent<SimpleHUD>();
                hud.Init(__instance.playerData);
            }
            else
            {
                try { hud.SetPlayerData(__instance.playerData); } catch { }
            }
        }

        private static Sprite GenerateDebugSprite()
        {
            var tex = new Texture2D(160, 160);
            for (int x = 0; x < 160; x++)
                for (int y = 0; y < 160; y++)
                    tex.SetPixel(x, y, Color.white);

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 160, 160), new Vector2(0.5f, 0.5f));
        }
    }

    [HarmonyPatch(typeof(GameManager), "Awake")]
    class GameManager_Awake_Patch
    {
        static void Postfix(GameManager __instance)
        {
            DisableStartup(__instance);
        }
    }

    [HarmonyPatch(typeof(GameManager), "Start")]
    class GameManager_Start_Patch
    {
        static void Postfix(GameManager __instance)
        {
            DisableStartup(__instance);
        }
    }

    [HarmonyPatch(typeof(StartManager), "Start")]
    class StartManager_Start_Enumerator_Patch
    {
        static void Prefix(StartManager __instance)
        {
            // ensure animator does not queue logo sequence
            if (__instance.startManagerAnimator != null)
                __instance.startManagerAnimator.SetBool("WillShowQuote", false);
        }

        static void Postfix(StartManager __instance, ref IEnumerator __result)
        {
            if (__result == null) return;
            var fields = __result.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var f in fields)
            {
                if (f.FieldType == typeof(bool) && f.Name.Contains("showIntroSequence"))
                {
                    f.SetValue(__result, false);
                    if (__instance.startManagerAnimator != null)
                        __instance.startManagerAnimator.Play("LoadingIcon", 0, 1f);
                    Debug.Log("[HelperMod] Skipping intro sequence");
                    break;
                }
            }
        }
    }

    public class ShadeController : MonoBehaviour
    {
        // Movement and leash
        public float moveSpeed = 8f;
        public float maxDistance = 14f;
        private Rigidbody2D rb;
        private Collider2D bodyCol;
        private int shadeMaxHP;
        private int shadeHP;
        private float hazardCooldown;

        // Ranged attack
        public float projectileSpeed = 22f;
        public float fireCooldown = 0.25f;
        public float nailCooldown = 0.3f;
        public Vector2 muzzleOffset = new Vector2(0.9f, 0.0f); // spawn slightly in front

        private Transform hornetTransform;
        private float fireTimer;
        private SpriteRenderer sr;
        private int facing = 1; // 1 = right, -1 = left
        private float nailTimer;
        private const KeyCode FireKey = KeyCode.Space;
        private const KeyCode NailKey = KeyCode.J;

        // Shade Soul resource (gain via melee, spend via spells)
        public int shadeSoulMax = 99;
        public int shadeSoul = 0;
        public int soulGainPerHit = 11;      // per enemy hit by a slash
        public int projectileSoulCost = 33;  // cost per projectile

        private SimpleHUD cachedHud;

        public void Init(Transform hornet) { hornetTransform = hornet; }

        void Start()
        {
            SetupPhysics();
            if (hornetTransform == null)
            {
                var hornet = GameObject.FindWithTag("Player");
                if (hornet != null)
                {
                    hornetTransform = hornet.transform;
                    Debug.Log("[HelperMod] ShadeController.Start: Found Player tag -> set hornetTransform.");
                }
                else
                {
                    Debug.Log("[HelperMod] ShadeController.Start: Could not find Player by tag.");
                }
            }

            sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                var c = sr.color; c.a = 0.9f; sr.color = c;
            }

            // Cache HUD for shade soul updates
            cachedHud = Object.FindObjectOfType<SimpleHUD>();
            PushSoulToHud();
            CheckHazardOverlap();
            // Initialize Shade HP from PlayerData
            try
            {
                var pd = GameManager.instance != null ? GameManager.instance.playerData : null;
                if (pd != null)
                {
                    shadeMaxHP = Mathf.Max(1, (pd.maxHealth + 1) / 2);
                    shadeHP = Mathf.Clamp((pd.health + 1) / 2, 0, shadeMaxHP);
                    PushShadeStatsToHud();
                }
            }
            catch { }
        }

        void Update()
        {
            if (hornetTransform == null) return;

            if (hazardCooldown > 0f) hazardCooldown = Mathf.Max(0f, hazardCooldown - Time.deltaTime);

            HandleMovementAndFacing();
            HandleFire();
            HandleNailAttack();

            if (Input.GetKeyDown(KeyCode.F9))
            DumpNearestEnemyHealthManager();

            // Keep HUD in sync (cheap check)
            if (!cachedHud) cachedHud = Object.FindObjectOfType<SimpleHUD>();
            PushSoulToHud();
            CheckHazardOverlap();
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
            float h = (Input.GetKey(KeyCode.A) ? -1f : 0f) + (Input.GetKey(KeyCode.D) ? 1f : 0f);
            float v = (Input.GetKey(KeyCode.S) ? -1f : 0f) + (Input.GetKey(KeyCode.W) ? 1f : 0f);
            Vector2 input = new Vector2(h, v);
            if (input.sqrMagnitude > 1f) input.Normalize();

            if (rb){ rb.MovePosition(rb.position + input * moveSpeed * Time.deltaTime); } else { transform.position += (Vector3)(input * moveSpeed * Time.deltaTime); }

            // Update facing if player pressed left/right
            if (h > 0.1f) facing = 1;
            else if (h < -0.1f) facing = -1;

            // Flip sprite to match facing
            if (sr != null) sr.flipX = (facing == -1);

            // Leash to Hornet
            Vector3 toShade = transform.position - hornetTransform.position;
            float dist = toShade.magnitude;
            if (dist > maxDistance)
                transform.position = hornetTransform.position + toShade.normalized * maxDistance;
        }

        private void HandleFire()
        {
            fireTimer -= Time.deltaTime;
            if (!Input.GetKey(FireKey) || fireTimer > 0f) return;
            // Require soul to cast
            if (shadeSoul < projectileSoulCost)
            {
                // Not enough soul: small feedback can be added here later
                Debug.Log($"[HelperMod] Fire blocked: need {projectileSoulCost}, have {shadeSoul}.");
                return;
            }
            fireTimer = fireCooldown;
            shadeSoul = Mathf.Max(0, shadeSoul - projectileSoulCost);
            PushSoulToHud();
            CheckHazardOverlap();
            // Initialize Shade HP from PlayerData
            try
            {
                var pd = GameManager.instance != null ? GameManager.instance.playerData : null;
                if (pd != null)
                {
                    shadeMaxHP = Mathf.Max(1, (pd.maxHealth + 1) / 2);
                    shadeHP = Mathf.Clamp((pd.health + 1) / 2, 0, shadeMaxHP);
                    PushShadeStatsToHud();
                }
            }
            catch { }

            Vector2 dir = new Vector2(facing, 0f);
            SpawnProjectile(dir);
        }

        private void HandleNailAttack()
        {
            nailTimer -= Time.deltaTime;
            if (nailTimer > 0f) return;

            // Shade has independent input; do NOT mirror Hornet's Attack
            bool pressed = Input.GetMouseButtonDown(0) || Input.GetKeyDown(NailKey);
            if (pressed)
            {
                nailTimer = nailCooldown;
                Debug.Log("[HelperMod] HandleNailAttack: Trigger PerformNailSlash().");
                PerformNailSlash();
            }
        }

        private void PerformNailSlash()
        {
            var hc = HeroController.instance;
            if (hc == null) return;

            // Choose slash variant based on input: up / down / normal
            Debug.Log("[HelperMod] PerformNailSlash: Begin.");
            GameObject source = null;
            float v = (Input.GetKey(KeyCode.S) ? -1f : 0f) + (Input.GetKey(KeyCode.W) ? 1f : 0f);
            var upField = hc.GetType().GetField("UpSlashObject", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var downField = hc.GetType().GetField("DownSlashObject", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var normalField = hc.GetType().GetField("NormalSlashObject", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var altField = hc.GetType().GetField("AlternateSlashObject", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var upObj = upField?.GetValue(hc) as GameObject;
            var downObj = downField?.GetValue(hc) as GameObject;
            var normalObj = normalField?.GetValue(hc) as GameObject;
            var altObj = altField?.GetValue(hc) as GameObject;
            // Fallback to properties if fields are null
            NailSlash upProp = null, downProp = null, normalProp = null, altProp = null;
            try { upProp = hc.GetType().GetProperty("UpSlash", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(hc, null) as NailSlash; } catch { }
            try { downProp = hc.GetType().GetProperty("DownSlash", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(hc, null) as NailSlash; } catch { }
            try { normalProp = hc.GetType().GetProperty("NormalSlash", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(hc, null) as NailSlash; } catch { }
            try { altProp = hc.GetType().GetProperty("AlternateSlash", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(hc, null) as NailSlash; } catch { }
            var upGO2 = upProp ? upProp.gameObject : null;
            var downGO2 = downProp ? downProp.gameObject : null;
            var normalGO2 = normalProp ? normalProp.gameObject : null;
            var altGO2 = altProp ? altProp.gameObject : null;
            // Prefer field-based objects; use property-based objects as fallback
            var upGO = upObj ?? upGO2;
            var downGO = downObj ?? downGO2;
            var normalGO = normalObj ?? normalGO2;
            var altGO = altObj ?? altGO2;
            Debug.Log($"[HelperMod] PerformNailSlash: v={v:F2} up={(upGO?upGO.name:"null")} down={(downGO?downGO.name:"null")} normal={(normalGO?normalGO.name:"null")} alt={(altGO?altGO.name:"null")}.");
            if (v > 0.35f && upGO)
                source = upGO;
            else if (v < -0.35f && downGO)
                source = downGO;
            else
            {
                // Side slash: prefer Alternate for facing right, Normal for facing left
                if (facing >= 0)
                    source = altGO ?? normalGO;
                else
                    source = normalGO ?? altGO;
            }

            if (source == null)
            {
                // Fallback: search under Hero for any NailSlash prefabs to clone
                try
                {
                    var allHeroSlashes = hc.GetComponentsInChildren<NailSlash>(true);
                    Debug.Log($"[HelperMod] PerformNailSlash: Fallback search found {allHeroSlashes?.Length ?? 0} NailSlash components under Hero.");
                    NailSlash pick = null;
                    if (allHeroSlashes == null || allHeroSlashes.Length == 0)
                    {
                        // Global search as a last resort (includes inactive assets)
                        var all = Resources.FindObjectsOfTypeAll<NailSlash>();
                        Debug.Log($"[HelperMod] PerformNailSlash: Global search found {all?.Length ?? 0} NailSlash assets.");
                        allHeroSlashes = all;
                    }

                    if (allHeroSlashes != null && allHeroSlashes.Length > 0)
                    {
                        // Try to pick by direction hint and side-facing name if Alternate not available
                        bool MatchUp(NailSlash ns) { return ns && (((ns.name ?? "").ToLowerInvariant().Contains("up")) || ((ns.animName ?? "").ToLowerInvariant().Contains("up"))); }
                        bool MatchDown(NailSlash ns) { return ns && (((ns.name ?? "").ToLowerInvariant().Contains("down")) || ((ns.animName ?? "").ToLowerInvariant().Contains("down"))); }
                        bool MatchNormal(NailSlash ns) { return ns && !MatchUp(ns) && !MatchDown(ns); }
                        bool MatchRight(NailSlash ns) { if (!ns) return false; var n=(ns.name??"").ToLowerInvariant(); var a=(ns.animName??"").ToLowerInvariant(); return n.Contains("alt")||n.Contains("right")||a.Contains("alt")||a.Contains("right"); }
                        bool MatchLeft(NailSlash ns) { if (!ns) return false; var n=(ns.name??"").ToLowerInvariant(); var a=(ns.animName??"").ToLowerInvariant(); return n.Contains("left")||a.Contains("left"); }
                        if (v > 0.35f) pick = System.Array.Find(allHeroSlashes, s => MatchUp(s));
                        else if (v < -0.35f) pick = System.Array.Find(allHeroSlashes, s => MatchDown(s));
                        else
                        {
                            if (facing >= 0)
                            {
                                pick = System.Array.Find(allHeroSlashes, s => MatchNormal(s) && MatchRight(s)) ?? System.Array.Find(allHeroSlashes, s => MatchRight(s));
                            }
                            else
                            {
                                pick = System.Array.Find(allHeroSlashes, s => MatchNormal(s) && MatchLeft(s)) ?? System.Array.Find(allHeroSlashes, s => MatchLeft(s));
                            }
                        }
                        if (pick == null) pick = System.Array.Find(allHeroSlashes, s => MatchNormal(s));
                        if (pick == null) pick = allHeroSlashes[0];
                        source = pick ? pick.gameObject : null;
                        Debug.Log($"[HelperMod] PerformNailSlash: Fallback chose '{(pick?pick.name:"<none>")}'.");
                    }
                }
                catch { }

                if (source == null)
                {
                    Debug.Log("[HelperMod] PerformNailSlash: No slash source found.");
                    return;
                }
            }

            // Instantiate under Hero so NailSlash.Awake finds HeroController, then immediately reparent to Shade
            var slash = GameObject.Instantiate(source, hc.transform);
            slash.transform.position = transform.position;
            slash.transform.SetParent(transform, true);

            // Temporarily disable all colliders and damagers to prevent any early collisions before patching
            Collider2D[] tempCols = slash.GetComponentsInChildren<Collider2D>(true);
            foreach (var c in tempCols) if (c) c.enabled = false;
            var tempDamagers = slash.GetComponentsInChildren<DamageEnemies>(true);
            foreach (var d in tempDamagers) if (d) d.enabled = false;

            // Ensure correct layer/tag for terrain/destructible interaction
            try
            {
                int desiredLayer = source.layer;
                foreach (var t in slash.GetComponentsInChildren<Transform>(true))
                {
                    if (!t) continue;
                    t.gameObject.layer = desiredLayer;
                    // Use Untagged so DamageEnemies doesn't treat it as hero nail in Start()
                    t.gameObject.tag = "Untagged";
                }
            }
            catch { }
            Debug.Log($"[HelperMod] PerformNailSlash: Instantiated slash clone '{slash.name}' under Hero then reparented to Shade (pre-patched).");

            // Align facing to Shade
            try
            {
                var tr = slash.transform;
                var ls = tr.localScale;
                ls.x = Mathf.Abs(ls.x) * (facing >= 0 ? 1f : -1f);
                tr.localScale = ls;
                Debug.Log($"[HelperMod] PerformNailSlash: Set facing={facing}.");
            }
            catch { }

            var nailSlash = slash.GetComponent<NailSlash>();
            if (nailSlash != null)
            {
                var f = typeof(NailAttackBase).GetField("hc", BindingFlags.Instance | BindingFlags.NonPublic);
                f?.SetValue(nailSlash, hc);
                Debug.Log("[HelperMod] PerformNailSlash: Wired NailSlash.hc.");

                // Also set HeroController reference used by NailSlashTravel if present
                try
                {
                    var travel = slash.GetComponent<NailSlashTravel>();
                    if (travel != null)
                    {
                        var tf = typeof(NailSlashTravel).GetField("hc", BindingFlags.Instance | BindingFlags.NonPublic);
                        tf?.SetValue(travel, hc);
                        Debug.Log("[HelperMod] PerformNailSlash: Wired NailSlashTravel.hc.");
                    }
                }
                catch { }
                // Prevent Hornet recoil (remove NailSlashRecoil components)
                try
                {
                    var recoils = slash.GetComponentsInChildren<NailSlashRecoil>(true);
                    foreach (var r in recoils) if (r) Destroy(r);
                    Debug.Log($"[HelperMod] PerformNailSlash: Removed {recoils?.Length ?? 0} NailSlashRecoil components.");
                }
                catch { }

                // Ensure hits are not treated as Hornet damage (stop silk gain)
                try
                {
                    var damagers = slash.GetComponentsInChildren<DamageEnemies>(true);
                    var srcField = typeof(DamageEnemies).GetField("sourceIsHero", BindingFlags.Instance | BindingFlags.NonPublic);
                    var ihField  = typeof(DamageEnemies).GetField("isHeroDamage", BindingFlags.Instance | BindingFlags.NonPublic);
                    var dirField = typeof(DamageEnemies).GetField("direction", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    var moveDirField = typeof(DamageEnemies).GetField("moveDirection", BindingFlags.Instance | BindingFlags.NonPublic);
                    var flipBehindField = typeof(DamageEnemies).GetField("flipDirectionIfBehind", BindingFlags.Instance | BindingFlags.NonPublic);
                    var fwdVecField = typeof(DamageEnemies).GetField("forwardVector", BindingFlags.Instance | BindingFlags.NonPublic);
                    var isNailAttackField = typeof(DamageEnemies).GetField("isNailAttack", BindingFlags.Instance | BindingFlags.NonPublic);
                    var onlyEnemiesField = typeof(DamageEnemies).GetField("onlyDamageEnemies", BindingFlags.Instance | BindingFlags.NonPublic);
                    var setOnlyEnemies = typeof(DamageEnemies).GetMethod("setOnlyDamageEnemies", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    var silkGenField = typeof(DamageEnemies).GetField("silkGeneration", BindingFlags.Instance | BindingFlags.NonPublic);
                    int patched = 0;
                    foreach (var d in damagers)
                    {
                        if (!d) continue;
                        try { srcField?.SetValue(d, false); } catch { }
                        try { ihField?.SetValue(d, false); } catch { }
                        try { isNailAttackField?.SetValue(d, false); } catch { }
                        // Force slash direction based on Shade input/facing
                        try
                        {
                            float dir = 0f;
                            if (v > 0.35f) dir = 90f; // up
                            else if (v < -0.35f) dir = 270f; // down
                            else dir = (facing >= 0 ? 0f : 180f); // side (right=0, left=180)
                            dirField?.SetValue(d, dir);
                            moveDirField?.SetValue(d, false);
                            flipBehindField?.SetValue(d, false);
                            fwdVecField?.SetValue(d, Vector2.zero);
                        }
                        catch { }
                        // Ensure terrain can be hit
                        try
                        {
                            if (setOnlyEnemies != null) setOnlyEnemies.Invoke(d, new object[] { false });
                            else onlyEnemiesField?.SetValue(d, false);
                        }
                        catch { }
                        // Explicitly prevent any silk generation flags
                        try {
                            if (silkGenField != null)
                            {
                                var enumType = silkGenField.FieldType;
                                var noneVal = System.Enum.ToObject(enumType, 0); // None is usually 0
                                silkGenField.SetValue(d, noneVal);
                            }
                        } catch { }
                        patched++;
                    }
                    Debug.Log($"[HelperMod] PerformNailSlash: Patched DamageEnemies hero flags on {patched} component(s).");
                }
                catch { }

                // Ignore collisions with Hornet to prevent parry/recoil interactions
                try
                {
                    var shadeCols = slash.GetComponentsInChildren<Collider2D>(true);
                    var hornetCols = hc.GetComponentsInChildren<Collider2D>(true);
                    foreach (var sc in shadeCols)
                        foreach (var hcCol in hornetCols)
                            if (sc && hcCol) Physics2D.IgnoreCollision(sc, hcCol, true);
                }
                catch { }

                // Remove hero-specific helpers that may get added
                try
                {
                    var extras = slash.GetComponentsInChildren<HeroExtraNailSlash>(true);
                    foreach (var e in extras) if (e) Destroy(e);
                    var travels = slash.GetComponentsInChildren<NailSlashTravel>(true);
                    foreach (var tv in travels) if (tv) Destroy(tv);
                    var thunkers = slash.GetComponentsInChildren<NailSlashTerrainThunk>(true);
                    foreach (var t in thunkers) if (t) Destroy(t);

                    // Disable Extra Damager child and any additional DamageEnemies beyond the first
                    var extraDamager = slash.transform.Find("Extra Damager");
                    if (extraDamager) extraDamager.gameObject.SetActive(false);
                    var allDamagers = slash.GetComponentsInChildren<DamageEnemies>(true);
                    bool firstKept = false;
                    foreach (var d in allDamagers)
                    {
                        if (!d) continue;
                        if (!firstKept) { firstKept = true; continue; }
                        d.enabled = false;
                    }
                }
                catch { }

                // If this is a side slash, and facing right, prefer using AlternateSlash's animation clip
                try
                {
                    if (Mathf.Abs(v) < 0.35f && facing >= 0)
                    {
                        var altSlashProp = hc.GetType().GetProperty("AlternateSlash", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(hc, null) as NailSlash;
                        if (altSlashProp != null && !string.IsNullOrEmpty(altSlashProp.animName))
                        {
                            nailSlash.animName = altSlashProp.animName;
                            Debug.Log($"[HelperMod] PerformNailSlash: Using AlternateSlash anim '{nailSlash.animName}' for right-facing side slash.");
                        }
                    }
                }
                catch { }

                // Re-enable damagers now that they've been patched
                foreach (var d in tempDamagers) if (d) d.enabled = true;

                // Re-allow colliders; NailSlash animator will toggle hitboxes appropriately
                foreach (var c in tempCols) if (c) c.enabled = true;

                // Attach forward-filter for side slashes to prevent any rear-stage colliders from hitting
                try
                {
                    if (Mathf.Abs(v) < 0.35f)
                    {
                        var filt = slash.AddComponent<SlashForwardFilter>();
                        Vector2 fwd = (facing >= 0 ? Vector2.right : Vector2.left);
                        filt.Init(this.transform, fwd, 0.25f);
                    }
                }
                catch { }

                // For side slashes, cut trailing visual after brief forward window to remove backward swing look
                // No visual culling; prefer correct prefab variant per facing

                Debug.Log("[HelperMod] PerformNailSlash: Calling StartSlash().");
                nailSlash.StartSlash();

                // Award soul on each enemy damaged by this slash
                try
                {
                    var primaryDamager = nailSlash.EnemyDamager;
                    if (primaryDamager != null)
                    {
                        Debug.Log("[HelperMod] PerformNailSlash: Hook DamagedEnemy for soul gain.");
                        System.Action onDamaged = null; System.Action<bool> onEnded = null;
                        onDamaged = () =>
                        {
                            Debug.Log("[HelperMod] DamagedEnemy: +11 soul.");
                            shadeSoul = Mathf.Min(shadeSoulMax, shadeSoul + soulGainPerHit);
                            PushSoulToHud();
            CheckHazardOverlap();
            // Initialize Shade HP from PlayerData
            try
            {
                var pd = GameManager.instance != null ? GameManager.instance.playerData : null;
                if (pd != null)
                {
                    shadeMaxHP = Mathf.Max(1, (pd.maxHealth + 1) / 2);
                    shadeHP = Mathf.Clamp((pd.health + 1) / 2, 0, shadeMaxHP);
                    PushShadeStatsToHud();
                }
            }
            catch { }
                        };
                        primaryDamager.DamagedEnemy += onDamaged;

                        // Unsubscribe when damage window ends
                        onEnded = (didHit) =>
                        {
                            try { primaryDamager.DamagedEnemy -= onDamaged; } catch { }
                            try { nailSlash.EndedDamage -= onEnded; } catch { }
                            Debug.Log("[HelperMod] EndedDamage: unsubscribed handlers.");
                        };
                        nailSlash.EndedDamage += onEnded;
                    }
                    else
                    {
                        Debug.Log("[HelperMod] PerformNailSlash: EnemyDamager was null.");
                    }
                }
                catch { }
            }
            else
            {
                Debug.Log("[HelperMod] PerformNailSlash: NailSlash component not found on clone.");
            }

            // Post-configure one frame later to override any Start() defaults the prefab sets
            StartCoroutine(PostConfigureSlash(slash, v, facing, hc));
        }

        private IEnumerator PostConfigureSlash(GameObject slash, float v, int facingSign, HeroController hc)
        {
            yield return null; // wait one frame for Start() on components
            if (!slash) yield break;
            try
            {
                // Remove any recoil components that got added/activated
                var recoils = slash.GetComponentsInChildren<NailSlashRecoil>(true);
                foreach (var r in recoils) if (r) Destroy(r);

                // Patch damagers again to ensure hero flags are off and directions set
                var damagers = slash.GetComponentsInChildren<DamageEnemies>(true);
                var srcField = typeof(DamageEnemies).GetField("sourceIsHero", BindingFlags.Instance | BindingFlags.NonPublic);
                var ihField  = typeof(DamageEnemies).GetField("isHeroDamage", BindingFlags.Instance | BindingFlags.NonPublic);
                var dirField = typeof(DamageEnemies).GetField("direction", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var moveDirField = typeof(DamageEnemies).GetField("moveDirection", BindingFlags.Instance | BindingFlags.NonPublic);
                var flipBehindField = typeof(DamageEnemies).GetField("flipDirectionIfBehind", BindingFlags.Instance | BindingFlags.NonPublic);
                var fwdVecField = typeof(DamageEnemies).GetField("forwardVector", BindingFlags.Instance | BindingFlags.NonPublic);
                var onlyEnemiesField = typeof(DamageEnemies).GetField("onlyDamageEnemies", BindingFlags.Instance | BindingFlags.NonPublic);
                var setOnlyEnemies = typeof(DamageEnemies).GetMethod("setOnlyDamageEnemies", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var ignoreNailPosField = typeof(DamageEnemies).GetField("ignoreNailPosition", BindingFlags.Instance | BindingFlags.NonPublic);

                float dir = 0f;
                if (v > 0.35f) dir = 90f; // up
                else if (v < -0.35f) dir = 270f; // down
                else dir = (facingSign >= 0 ? 0f : 180f); // side (right=0, left=180)

                foreach (var d in damagers)
                {
                    if (!d) continue;
                    try { srcField?.SetValue(d, false); } catch { }
                    try { ihField?.SetValue(d, false); } catch { }
                    try { dirField?.SetValue(d, dir); } catch { }
                    try { moveDirField?.SetValue(d, false); } catch { }
                    try { flipBehindField?.SetValue(d, false); } catch { }
                    try { fwdVecField?.SetValue(d, Vector2.zero); } catch { }
                    try { if (setOnlyEnemies != null) setOnlyEnemies.Invoke(d, new object[] { false }); else onlyEnemiesField?.SetValue(d, false); } catch { }
                    try { ignoreNailPosField?.SetValue(d, true); } catch { }
                }

                // Keep ignoring Hornet collisions (handles extra damagers that enabled late)
                var shadeCols = slash.GetComponentsInChildren<Collider2D>(true);
                Collider2D[] hornetCols = new Collider2D[0];
                if (hc)
                {
                    hornetCols = hc.GetComponentsInChildren<Collider2D>(true);
                }
                foreach (var sc in shadeCols)
                    foreach (var hcCol in hornetCols)
                        if (sc && hcCol) Physics2D.IgnoreCollision(sc, hcCol, true);

                // Re-apply visual facing (in case animation flipped it)
                try
                {
                    var tr = slash.transform; var ls = tr.localScale; ls.x = Mathf.Abs(ls.x) * (facingSign >= 0 ? -1f : 1f); tr.localScale = ls;
                }
                catch { }
            }
            catch { }
        }

        // Removed trailing visual culling; we pick proper slash variant instead

        private void SpawnProjectile(Vector2 dir)
        {
            var proj = new GameObject("ShadeProjectile");
            proj.transform.position = transform.position + (Vector3)new Vector2(muzzleOffset.x * facing, muzzleOffset.y);
            proj.tag = "Hero Spell";
            int spellLayer = LayerMask.NameToLayer("Hero Spell");
            int atkLayer = LayerMask.NameToLayer("Hero Attack");
            if (spellLayer >= 0) proj.layer = spellLayer;
            else if (atkLayer >= 0) proj.layer = atkLayer;

            var psr = proj.AddComponent<SpriteRenderer>();
            psr.sprite = MakeDotSprite();
            if (sr != null)
            {
                psr.sortingLayerID = sr.sortingLayerID;
                psr.sortingOrder   = sr.sortingOrder + 1;
                psr.color          = Color.black;
            }

            var col = proj.AddComponent<CircleCollider2D>();
            col.isTrigger = true;

            // ignore collisions with other ShadeProjectiles
             var others = Object.FindObjectsByType<ShadeProjectile>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var o in others)
            {
                var oc = o.GetComponent<Collider2D>();
                if (oc) Physics2D.IgnoreCollision(col, oc, true);
            }

            var rb = proj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.linearVelocity = dir.normalized * projectileSpeed;

            if (hornetTransform != null)
            {
                var hornetCols = hornetTransform.GetComponentsInChildren<Collider2D>(true);
                foreach (var hc in hornetCols)
                    if (hc && col) Physics2D.IgnoreCollision(col, hc, true);
            }

            var sp = proj.AddComponent<ShadeProjectile>();
            sp.damage = 20;
            sp.hornetRoot = hornetTransform;
            sp.lifeSeconds = 1.5f;
        }


        private Sprite MakeDotSprite()
        {
            var tex = new Texture2D(6, 6);
            for (int x = 0; x < tex.width; x++)
                for (int y = 0; y < tex.height; y++)
                    tex.SetPixel(x, y, Color.black);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 16f);
        }

        private void DumpNearestEnemyHealthManager()
        {
            var allMBs = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            MonoBehaviour nearestHM = null;
            float bestDist = float.PositiveInfinity;

            foreach (var mb in allMBs)
            {
                if (mb == null) continue;
                var t = mb.GetType();
                if (t.Name != "HealthManager") continue;

                float d = Vector2.Distance(mb.transform.position, transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    nearestHM = mb;
                }
            }

            if (nearestHM == null)
            {
                Debug.Log("[HelperMod] F9 dump: No HealthManager found nearby.");
                return;
            }

            // Reuse projectileâ€™s dumper (make it static if you want to call it directly)
            var ht = nearestHM.GetType();
            var methods = ht.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var fields  = ht.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var props   = ht.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            string mlist = string.Join(", ", System.Array.ConvertAll(methods, m =>
                $"{m.Name}({string.Join(",", System.Array.ConvertAll(m.GetParameters(), p => p.ParameterType.Name + " " + p.Name))})"));
            string flist = string.Join(", ", System.Array.ConvertAll(fields,  f => f.FieldType.Name + " " + f.Name));
            string plist = string.Join(", ", System.Array.ConvertAll(props,   p => p.PropertyType.Name + " " + p.Name));

            Debug.Log($"[HelperMod] F9 HealthManager @dist {bestDist:F1} methods: {mlist}");
            Debug.Log($"[HelperMod] F9 HealthManager fields : {flist}");
            Debug.Log($"[HelperMod] F9 HealthManager props  : {plist}");
        }
        private void SetupPhysics()
        {
            rb = GetComponent<Rigidbody2D>();
            if (!rb)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
            }
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
                bodyCol = cap;
            }
            else
            {
                bodyCol.isTrigger = false;
            }

            try
            {
                var hc = HeroController.instance;
                if (hc)
                {
                    gameObject.layer = hc.gameObject.layer;
                    var myCols = GetComponentsInChildren<Collider2D>(true);
                    var hornetCols = hc.GetComponentsInChildren<Collider2D>(true);
                    foreach (var mc in myCols)
                        foreach (var hcCol in hornetCols)
                            if (mc && hcCol) Physics2D.IgnoreCollision(mc, hcCol, true);
                }
            }
            catch { }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            TryHazardTeleport(collision.collider);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryHazardTeleport(other);
        }

        private void TryHazardTeleport(Collider2D col)
        {
            if (!col) return;
            try
            {
                if (bodyCol && col && !bodyCol.IsTouching(col)) return;
                if (col.transform == transform || col.transform.IsChildOf(transform)) return;
                if (hornetTransform && (col.transform == hornetTransform || col.transform.IsChildOf(hornetTransform))) return;
                var dh = col.GetComponentInParent<DamageHero>();
                if (dh != null)
                {
                    var hz = GetHazardType(dh);
                    if (IsTerrainHazard(hz)) { OnShadeHitHazard(); return; }
                }
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

        
                private void CheckHazardOverlap()
        {
            if (hazardCooldown > 0f) return;
            if (!bodyCol) return;
            var filter = new ContactFilter2D();filter.useTriggers = true;
            Collider2D[] results = new Collider2D[16];
            int count = bodyCol.OverlapCollider(filter, results);
            for (int i = 0; i < count; i++)
            {
                var c = results[i];
                if (!c) continue;
                if (c.transform == transform || c.transform.IsChildOf(transform)) continue;
                if (hornetTransform && (c.transform == hornetTransform || c.transform.IsChildOf(hornetTransform))) continue;
                var dh = c.GetComponentInParent<DamageHero>();
                if (dh != null)
                {
                    var hz = GetHazardType(dh);
                    if (IsTerrainHazard(hz)) { OnShadeHitHazard(); return; }
                }
            }
        }

        private static GlobalEnums.HazardType GetHazardType(DamageHero dh)
        {
            try
            {
                var tf = typeof(DamageHero).GetField("hazardType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (tf != null)
                    return (GlobalEnums.HazardType)tf.GetValue(dh);
            }
            catch { }
            return GlobalEnums.HazardType.NON_HAZARD;
        }

        private void OnShadeHitHazard()
        {
            if (hazardCooldown > 0f) return;
            TeleportToHornet();
            shadeHP = Mathf.Max(0, shadeHP - 1);
            PushShadeStatsToHud();
            hazardCooldown = 0.25f;
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
    }
    public class SlashForwardFilter : MonoBehaviour
    {
        private Transform shade;
        private Vector2 forward;
        private float timeLeft;
        private int lastEnabledCount;

        public void Init(Transform shadeTransform, Vector2 fwd, float duration)
        {
            shade = shadeTransform;
            forward = fwd.normalized;
            timeLeft = duration;
            lastEnabledCount = -1;
        }

        void Update()
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0f) { Destroy(this); return; }
            var cols = GetComponentsInChildren<Collider2D>(true);
            int enabledCount = 0;
            foreach (var c in cols) if (c && c.enabled) enabledCount++;
            if (enabledCount == 0) return;
            if (enabledCount != lastEnabledCount)
            {
                lastEnabledCount = enabledCount;
                FilterForward(cols);
            }
            else
            {
                // keep enforcing within the short window
                FilterForward(cols);
            }
        }

        private void FilterForward(Collider2D[] cols)
        {
            if (!shade) return;
            Vector2 origin = shade.position;
            foreach (var c in cols)
            {
                if (!c || !c.enabled) continue;
                Vector2 center = c.bounds.center;
                Vector2 to = (center - origin).normalized;
                float dot = Vector2.Dot(forward, to);
                if (dot < 0f)
                {
                    c.enabled = false;
                }
            }
        }
    }


    [HarmonyPatch(typeof(InputHandler), "MapKeyboardLayoutFromGameSettings")]
    class BlockKeyboardRebinding
    {
        static bool Prefix()
        {
            Debug.Log("[HelperMod] Prevented rebinding of Hornetâ€™s keyboard controls.");
            return false; // skip the whole rebinding method
        }
    }
        // Disable default keyboard mapping
    [HarmonyPatch(typeof(InputHandler), "MapDefaultKeyboardLayout")]
    static class BlockDefaultKeyboardMap
    {
        static bool Prefix()
        {
            Debug.Log("[HelperMod] Blocked default keyboard layout for Hornet.");
            return false; // skip original
        }
    }
    public class ShadeProjectile : MonoBehaviour
    {
        public int damage = 20;
        public Transform hornetRoot;
        public float lifeSeconds = 1.5f;
        bool hasHit;

        void Start() => Destroy(gameObject, lifeSeconds);

        void OnTriggerEnter2D(Collider2D other)
        {
            if (hasHit) return;
            if (other == null) return;
            if (hornetRoot != null && other.transform.IsChildOf(hornetRoot)) return;
            if (other.transform == transform || other.transform.IsChildOf(transform)) return;
            hasHit = true;

            var rb = GetComponent<Rigidbody2D>();
            Vector2 vel = rb ? rb.linearVelocity : (other.transform.position - transform.position);
            float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
            if (angle < 0f) angle += 360f;

            var hit = new HitInstance
            {
                Source = gameObject,
                AttackType = AttackTypes.Spell,
                DamageDealt = damage,
                Direction = angle,
                MagnitudeMultiplier = 1f,
                Multiplier = 1f,
                IsHeroDamage = true,
                IsFirstHit = true
            };

            HitTaker.Hit(other.gameObject, hit);
            if (HitTaker.TryGetHealthManager(other.gameObject, out var hm))
                hm.Hit(hit);

            StartCoroutine(DestroyNextFrame());
        }

        IEnumerator DestroyNextFrame()
        {
            yield return null;
            Destroy(gameObject);
        }
    }

}












