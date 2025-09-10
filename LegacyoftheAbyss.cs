using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[BepInPlugin("com.legacyoftheabyss.helper", "Legacy of the Abyss - Helper", "0.1.0")]
public class LegacyHelper : BaseUnityPlugin
{
    private static GameObject helper;
    private static GameObject hud;

    void Awake()
    {
        Logger.LogInfo("Patching GameManager.BeginScene...");
        Harmony harmony = new Harmony("com.legacyoftheabyss.helper");
        harmony.PatchAll();
    }

    internal static void DisableStartup(GameManager gm)
    {
        if (gm == null) return;
        var t = gm.GetType();

        string[] logoFlags = { "playTeamCherryLogo", "playLogo", "showTeamCherryLogo", "displayTeamCherryLogo", "teamCherryLogo" };
        foreach (var n in logoFlags)
        {
            var f = t.GetField(n, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            if (f != null && f.FieldType == typeof(bool)) f.SetValue(gm, false);
        }

        string[] saveFlags = { "playSaveReminder", "showSaveReminder", "displaySaveReminder", "saveReminder" };
        foreach (var n in saveFlags)
        {
            var f = t.GetField(n, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            if (f != null && f.FieldType == typeof(bool)) f.SetValue(gm, false);
        }
    }

    [HarmonyPatch(typeof(GameManager), "BeginScene")]
    class GameManager_BeginScene_Patch
    {
        static void Postfix(GameManager __instance)
        {
            bool gameplay = __instance.IsGameplayScene();
            if (hud != null)
                hud.SetActive(gameplay);

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

                helper.AddComponent<ShadeController>();
            }

            if (hud == null)
            {
                hud = new GameObject("KnightHUDRoot");
                var kh = hud.AddComponent<KnightHUD>();
                kh.Init(__instance.hero_ctrl);
            }
            else
            {
                hud.SetActive(true);
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

    public class ShadeController : MonoBehaviour
    {
        public float moveSpeed = 8f;
        public float maxDistance = 14f;

        public float projectileSpeed = 22f;
        public float fireCooldown = 0.25f;
        public Vector2 muzzleOffset = new Vector2(0.9f, 0.0f); // spawn slightly in front

        private Transform hornetTransform;
        private float fireTimer;
        private SpriteRenderer sr;
        private int facing = 1; // 1 = right, -1 = left
        private const KeyCode FireKey = KeyCode.Space;

        public void Init(Transform hornet) { hornetTransform = hornet; }

        void Start()
        {
            if (hornetTransform == null)
            {
                var hornet = GameObject.FindWithTag("Player");
                if (hornet != null) hornetTransform = hornet.transform;
            }

            sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                var c = sr.color; c.a = 0.9f; sr.color = c;
            }
        }

        void Update()
        {
            if (hornetTransform == null) return;

            HandleMovementAndFacing();
            HandleFire();

            if (Input.GetKeyDown(KeyCode.F9))
            DumpNearestEnemyHealthManager();
        }

        private void HandleMovementAndFacing()
        {
            float h = (Input.GetKey(KeyCode.A) ? -1f : 0f) + (Input.GetKey(KeyCode.D) ? 1f : 0f);
            float v = (Input.GetKey(KeyCode.S) ? -1f : 0f) + (Input.GetKey(KeyCode.W) ? 1f : 0f);
            Vector2 input = new Vector2(h, v);
            if (input.sqrMagnitude > 1f) input.Normalize();

            transform.position += (Vector3)(input * moveSpeed * Time.deltaTime);

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
            fireTimer = fireCooldown;

            Vector2 dir = new Vector2(facing, 0f);
            SpawnProjectile(dir);
        }

        private void SpawnProjectile(Vector2 dir)
        {
            var proj = new GameObject("ShadeProjectile");
            proj.transform.position = transform.position + (Vector3)new Vector2(muzzleOffset.x * facing, muzzleOffset.y);

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
            var others = FindObjectsOfType<ShadeProjectile>();
            foreach (var o in others)
            {
                var oc = o.GetComponent<Collider2D>();
                if (oc) Physics2D.IgnoreCollision(col, oc, true);
            }

            var rb = proj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.velocity = dir.normalized * projectileSpeed;

            if (hornetTransform != null)
            {
                var hornetCols = hornetTransform.GetComponentsInChildren<Collider2D>(true);
                foreach (var hc in hornetCols)
                    if (hc && col) Physics2D.IgnoreCollision(col, hc, true);
            }

            var sp = proj.AddComponent<ShadeProjectile>();
            sp.damage = 1;
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
            var allMBs = GameObject.FindObjectsOfType<MonoBehaviour>();
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

            // Reuse projectile’s dumper (make it static if you want to call it directly)
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

    }


    [HarmonyPatch(typeof(InputHandler), "MapKeyboardLayoutFromGameSettings")]
    class BlockKeyboardRebinding
    {
        static bool Prefix()
        {
            Debug.Log("[HelperMod] Prevented rebinding of Hornet’s keyboard controls.");
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
        public int damage = 1;
        public Transform hornetRoot;
        public float lifeSeconds = 1.5f;

        private static bool dumpedHealthManagerInfo = false;

        void Start() => Destroy(gameObject, lifeSeconds);

        void OnTriggerEnter2D(Collider2D other)
        {
            // Ignore Hornet & self
            if (hornetRoot != null && other.transform.IsChildOf(hornetRoot)) return;
            if (other.transform == transform || other.transform.IsChildOf(transform)) return;

            // Log components (helps targeting the real API)
            string comps = "";
            var arr = other.GetComponents<Component>();
            for (int i = 0; i < arr.Length; i++)
            {
                var c = arr[i];
                if (c != null) comps += (i == 0 ? "" : ", ") + c.GetType().Name;
            }

            bool applied = TryApplyDamage(other.gameObject, damage);

            Debug.Log($"[HelperMod] Projectile hit '{other.name}' (layer {other.gameObject.layer}) " +
                    $"appliedDamage={applied} comps=[{comps}]");

            Destroy(gameObject);
        }

        private bool TryApplyDamage(GameObject target, int dmg)
        {
            // Prefer a HealthManager on the same GO
            var hm = FindComponentByName(target, "HealthManager");
            if (hm != null)
            {
                var ht = hm.GetType();

                // Attempt to call Hit/TakeHit with a HitInstance so enemy reacts like Hornet's needle
                var methods = ht.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                foreach (var m in methods)
                {
                    if (m.Name != "Hit" && m.Name != "TakeHit") continue;
                    var pars = m.GetParameters();
                    if (pars.Length != 1) continue;

                    var hit = CreateHitInstance(pars[0].ParameterType, dmg);
                    if (hit != null)
                    {
                        try { m.Invoke(hm, new object[] { hit }); return true; } catch { }
                    }
                }

                // Dump once so we can hard-wire the correct call in next pass
                if (!dumpedHealthManagerInfo)
                {
                    dumpedHealthManagerInfo = true;
                    DumpTypeInfo("[HelperMod] HealthManager", ht);
                }

                // Try common damage-like methods with int param
                string[] names = { "ApplyDamage", "DoDamage", "ReceiveHit", "TakeHit", "OnHit", "Damage" };
                foreach (var nm in names)
                {
                    var m = ht.GetMethod(nm, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (m == null) continue;
                    var pars = m.GetParameters();

                    if (pars.Length == 1 && pars[0].ParameterType == typeof(int))
                    {
                        try { m.Invoke(hm, new object[] { dmg }); return true; } catch { }
                    }

                    // First int anywhere
                    for (int i = 0; i < pars.Length; i++)
                    {
                        if (pars[i].ParameterType == typeof(int))
                        {
                            var args = new object[pars.Length];
                            for (int j = 0; j < args.Length; j++)
                                args[j] = pars[j].HasDefaultValue ? pars[j].DefaultValue : GetDefault(pars[j].ParameterType);
                            args[i] = dmg;
                            try { m.Invoke(hm, args); return true; } catch { }
                        }
                    }
                }

                // Fallback: direct HP adjust (fields)
                if (DirectHpAdjust(hm, dmg))
                    return true;
            }

            // Try TagDamageTaker fallback
            var taker = FindComponentByName(target, "TagDamageTaker");
            if (taker != null)
            {
                var tt = taker.GetType();
                foreach (var nm in new[] { "TakeDamage", "TryTakeDamage", "OnHit", "Hit" })
                {
                    var m = tt.GetMethod(nm, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (m == null) continue;
                    var pars = m.GetParameters();

                    // Simple int
                    if (pars.Length == 1 && pars[0].ParameterType == typeof(int))
                    {
                        try { m.Invoke(taker, new object[] { dmg }); return true; } catch { }
                    }

                    // Any int slot
                    for (int i = 0; i < pars.Length; i++)
                    {
                        if (pars[i].ParameterType == typeof(int))
                        {
                            var args = new object[pars.Length];
                            for (int j = 0; j < args.Length; j++)
                                args[j] = pars[j].HasDefaultValue ? pars[j].DefaultValue : GetDefault(pars[j].ParameterType);
                            args[i] = dmg;
                            try { m.Invoke(taker, args); return true; } catch { }
                        }
                    }
                }
            }

            // Remove the SendMessage fallbacks (they were generating noise)
            return false;
        }

        private bool DirectHpAdjust(Component hm, int dmg)
        {
            var t = hm.GetType();

            // Fields
            foreach (var fname in new[] { "hp", "HP", "health", "Health", "currentHP", "currentHealth" })
            {
                var f = t.GetField(fname, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (f != null && (f.FieldType == typeof(int) || f.FieldType == typeof(float)))
                {
                    try
                    {
                        if (f.FieldType == typeof(int))
                        {
                            int hp = (int)f.GetValue(hm);
                            int newHp = Mathf.Max(0, hp - dmg);
                            f.SetValue(hm, newHp);
                            if (hp != newHp) Debug.Log($"[HelperMod] HM.{fname}: {hp}→{newHp}");
                            TriggerHitEffects(hm);
                            TryCallDeathIfZero(hm, newHp);
                            return true;
                        }
                        else
                        {
                            float hp = (float)f.GetValue(hm);
                            float newHp = Mathf.Max(0f, hp - dmg);
                            f.SetValue(hm, newHp);
                            if (Mathf.Abs(hp - newHp) > 0.001f) Debug.Log($"[HelperMod] HM.{fname}: {hp}→{newHp}");
                            TriggerHitEffects(hm);
                            TryCallDeathIfZero(hm, newHp <= 0.001f ? 0 : 1);
                            return true;
                        }
                    } catch {}
                }
            }

            // Properties
            foreach (var pname in new[] { "HP", "Health", "CurrentHP", "CurrentHealth" })
            {
                var p = t.GetProperty(pname, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (p != null && p.CanRead && p.CanWrite)
                {
                    try
                    {
                        if (p.PropertyType == typeof(int))
                        {
                            int hp = (int)p.GetValue(hm, null);
                            int newHp = Mathf.Max(0, hp - dmg);
                            p.SetValue(hm, newHp, null);
                            if (hp != newHp) Debug.Log($"[HelperMod] HM.{pname}: {hp}→{newHp}");
                            TriggerHitEffects(hm);
                            TryCallDeathIfZero(hm, newHp);
                            return true;
                        }
                        else if (p.PropertyType == typeof(float))
                        {
                            float hp = (float)p.GetValue(hm, null);
                            float newHp = Mathf.Max(0f, hp - dmg);
                            p.SetValue(hm, newHp, null);
                            if (Mathf.Abs(hp - newHp) > 0.001f) Debug.Log($"[HelperMod] HM.{pname}: {hp}→{newHp}");
                            TriggerHitEffects(hm);
                            TryCallDeathIfZero(hm, newHp <= 0.001f ? 0 : 1);
                            return true;
                        }
                    } catch {}
                }
            }

            return false;
        }

        private void TryCallDeathIfZero(Component hm, int hpAfter)
        {
            if (hpAfter > 0) return;
            var t = hm.GetType();
            var methods = t.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            foreach (var nm in new[] { "Die", "TakeDamage", "Hit", "DoDeath", "Kill", "OnDeath", "Death" })
            {
                foreach (var m in methods)
                {
                    if (m.Name != nm) continue;

                    var pars = m.GetParameters();
                    object[] args = new object[pars.Length];

                    for (int i = 0; i < pars.Length; i++)
                    {
                        var p = pars[i];
                        var pt = p.ParameterType;

                        if (pt.Name == "HitInstance" || pt.FullName == "HitInstance")
                        {
                            args[i] = CreateHitInstance(pt, 0);
                        }
                        else if (p.HasDefaultValue)
                        {
                            args[i] = p.DefaultValue;
                        }
                        else
                        {
                            args[i] = GetDefault(pt);
                        }
                    }

                    try { m.Invoke(hm, args); return; } catch { }
                }
            }

            // Fall back to old behaviour if nothing matched
            var go = hm.gameObject;
            go.SendMessage("SpawnDeath", SendMessageOptions.DontRequireReceiver);
        }

        private void TriggerHitEffects(Component hm)
        {
            var t = hm.GetType();
            var methods = t.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            foreach (var nm in new[] { "Hit", "OnHit", "TakeDamage" })
            {
                foreach (var m in methods)
                {
                    if (m.Name != nm) continue;

                    var pars = m.GetParameters();
                    object[] args = new object[pars.Length];

                    for (int i = 0; i < pars.Length; i++)
                    {
                        var p = pars[i];
                        var pt = p.ParameterType;

                        if (pt.Name == "HitInstance" || pt.FullName == "HitInstance")
                        {
                            args[i] = CreateHitInstance(pt, 0);
                        }
                        else if (p.HasDefaultValue)
                        {
                            args[i] = p.DefaultValue;
                        }
                        else
                        {
                            args[i] = GetDefault(pt);
                        }
                    }

                    try { m.Invoke(hm, args); return; } catch { }
                }
            }

            hm.gameObject.SendMessage("OnHit", SendMessageOptions.DontRequireReceiver);
        }

        private object CreateHitInstance(System.Type hitType, int dmg)
        {
            object hit = null;
            try { hit = System.Activator.CreateInstance(hitType); } catch { return null; }

            // Attack type
            var atkTypes = hitType.Assembly.GetType("AttackTypes");
            if (atkTypes != null)
            {
                object generic = null;
                try { generic = System.Enum.Parse(atkTypes, "Generic"); } catch { }
                if (generic != null)
                {
                    var f = hitType.GetField("AttackType") ?? hitType.GetField("attackType");
                    if (f != null) f.SetValue(hit, generic);
                    var p = hitType.GetProperty("AttackType") ?? hitType.GetProperty("attackType");
                    if (p != null && p.CanWrite) p.SetValue(hit, generic, null);
                }
            }

            // ignoreEvasion
            var ieF = hitType.GetField("ignoreEvasion") ?? hitType.GetField("IgnoreEvasion");
            if (ieF != null) ieF.SetValue(hit, false);
            var ieP = hitType.GetProperty("ignoreEvasion") ?? hitType.GetProperty("IgnoreEvasion");
            if (ieP != null && ieP.CanWrite) ieP.SetValue(hit, false, null);

            // Attack direction (default)
            var dirF = hitType.GetField("AttackDirection") ?? hitType.GetField("attackDirection") ?? hitType.GetField("Direction") ?? hitType.GetField("direction");
            if (dirF != null)
            {
                if (dirF.FieldType == typeof(Vector3))
                    dirF.SetValue(hit, Vector3.right);
                else if (dirF.FieldType == typeof(Vector2))
                    dirF.SetValue(hit, Vector2.right);
            }
            var dirP = hitType.GetProperty("AttackDirection") ?? hitType.GetProperty("attackDirection") ?? hitType.GetProperty("Direction") ?? hitType.GetProperty("direction");
            if (dirP != null && dirP.CanWrite)
            {
                if (dirP.PropertyType == typeof(Vector3))
                    dirP.SetValue(hit, Vector3.right, null);
                else if (dirP.PropertyType == typeof(Vector2))
                    dirP.SetValue(hit, Vector2.right, null);
            }

            // Damage
            var dmgF = hitType.GetField("Damage") ?? hitType.GetField("damage");
            if (dmgF != null) dmgF.SetValue(hit, dmg);
            var dmgP = hitType.GetProperty("Damage") ?? hitType.GetProperty("damage");
            if (dmgP != null && dmgP.CanWrite) dmgP.SetValue(hit, dmg, null);

            return hit;
        }

        private void DumpTypeInfo(string label, System.Type ht)
        {
            var methods = ht.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var fields  = ht.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var props   = ht.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            string mlist = string.Join(", ", System.Array.ConvertAll(methods, m =>
                $"{m.Name}({string.Join(",", System.Array.ConvertAll(m.GetParameters(), p => p.ParameterType.Name + " " + p.Name))})"));
            string flist = string.Join(", ", System.Array.ConvertAll(fields,  f => f.FieldType.Name + " " + f.Name));
            string plist = string.Join(", ", System.Array.ConvertAll(props,   p => p.PropertyType.Name + " " + p.Name));

            Debug.Log($"{label} methods: {mlist}");
            Debug.Log($"{label} fields : {flist}");
            Debug.Log($"{label} props  : {plist}");
        }

        private Component FindComponentByName(GameObject go, string typeName)
        {
            var comps = go.GetComponents<MonoBehaviour>();
            foreach (var c in comps)
                if (c != null && c.GetType().Name == typeName) return c;
            return null;
        }

        private object GetDefault(System.Type t) => t.IsValueType ? System.Activator.CreateInstance(t) : null;
    }

    public class KnightHUD : MonoBehaviour
    {
        private object heroController;
        private Image[] masks;
        private Image soulFill;
        private Canvas canvas;

        public void Init(object hero)
        {
            heroController = hero;
        }

        void Start()
        {
            DontDestroyOnLoad(gameObject);

            var canvasGO = new GameObject("KnightHUDCanvas");
            canvasGO.transform.SetParent(transform);
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            var hornetHealth = FindHornetHealthRoot();
            Vector2 anchorPos = new Vector2(20, -20);
            Sprite maskSprite = null;
            if (hornetHealth != null)
            {
                anchorPos = hornetHealth.anchoredPosition;
                var refMask = hornetHealth.GetComponentsInChildren<Image>(true).FirstOrDefault(i => i.name.ToLower().StartsWith("mask"));
                if (refMask != null) maskSprite = refMask.sprite;
            }

            // Health masks
            var healthRoot = new GameObject("HealthRoot");
            healthRoot.transform.SetParent(canvas.transform);
            var hr = healthRoot.AddComponent<RectTransform>();
            hr.anchorMin = hr.anchorMax = new Vector2(1, 1);
            hr.pivot = new Vector2(1, 1);
            hr.anchoredPosition = new Vector2(-anchorPos.x, anchorPos.y);

            masks = new Image[10];
            for (int i = 0; i < masks.Length; i++)
            {
                var m = new GameObject("Mask" + i).AddComponent<Image>();
                m.transform.SetParent(healthRoot.transform);
                m.sprite = maskSprite != null ? maskSprite : GenerateMaskSprite();
                var r = m.rectTransform;
                r.anchorMin = r.anchorMax = new Vector2(1, 1);
                r.pivot = new Vector2(1, 1);
                r.sizeDelta = new Vector2(20, 20);
                r.anchoredPosition = new Vector2(-i * 22, 0);
                masks[i] = m;
            }

            // Soul meter
            var soulRoot = new GameObject("SoulRoot");
            soulRoot.transform.SetParent(canvas.transform);
            var sr = soulRoot.AddComponent<RectTransform>();
            sr.anchorMin = sr.anchorMax = new Vector2(1, 1);
            sr.pivot = new Vector2(1, 1);
            sr.anchoredPosition = new Vector2(-anchorPos.x, anchorPos.y - 30);

            var bg = new GameObject("SoulBG").AddComponent<Image>();
            bg.transform.SetParent(soulRoot.transform);
            var bgrt = bg.rectTransform;
            bgrt.sizeDelta = new Vector2(20, 40);

            soulFill = new GameObject("SoulFill").AddComponent<Image>();
            soulFill.transform.SetParent(bg.transform);
            var frt = soulFill.rectTransform;
            frt.anchorMin = new Vector2(0, 0);
            frt.anchorMax = new Vector2(1, 1);
            frt.pivot = new Vector2(0.5f, 0);
            frt.anchoredPosition = Vector2.zero;
            soulFill.color = Color.white;
        }

        private RectTransform FindHornetHealthRoot()
        {
            foreach (var img in Resources.FindObjectsOfTypeAll<Image>())
            {
                if (img == null) continue;
                if (!img.name.ToLower().StartsWith("mask")) continue;
                var parent = img.transform.parent as RectTransform;
                if (parent != null) return parent;
            }
            return null;
        }

        void Update()
        {
            if (heroController == null)
            {
                var gm = GameManager.instance;
                if (gm != null) heroController = gm.hero_ctrl;
            }

            object pd = null;
            if (heroController != null)
            {
                var ht = heroController.GetType();
                var pdField = ht.GetField("playerData", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (pdField != null) pd = pdField.GetValue(heroController);
            }

            int health = GetInt(pd, new[] { "health", "currentHealth", "HP", "hp" });
            int maxHealth = GetInt(pd, new[] { "maxHealth", "maxHP" });
            health = (health + 1) / 2;
            maxHealth = (maxHealth + 1) / 2;
            float soul = GetFloat(pd, new[] { "soul", "SOUL", "mpCharge", "MPCharge" });
            float maxSoul = GetFloat(pd, new[] { "maxSoul", "maxSOUL", "mpChargeMax", "MPChargeMax" });

            if (maxHealth <= 0) maxHealth = masks.Length;
            for (int i = 0; i < masks.Length; i++)
            {
                masks[i].enabled = i < maxHealth;
                masks[i].color = i < health ? Color.white : new Color(0, 0, 0, 0.5f);
            }

            float ratio = maxSoul > 0 ? Mathf.Clamp01(soul / maxSoul) : 0f;
            var rt = soulFill.rectTransform;
            rt.localScale = new Vector3(1f, ratio, 1f);
        }

        private Sprite GenerateMaskSprite()
        {
            var tex = new Texture2D(20, 20);
            for (int x = 0; x < 20; x++)
                for (int y = 0; y < 20; y++)
                {
                    float dx = x - 10, dy = y - 10;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    tex.SetPixel(x, y, dist <= 9 ? Color.white : Color.clear);
                }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 20, 20), new Vector2(0.5f, 0.5f));
        }

        private int GetInt(object obj, string[] names)
        {
            if (obj == null) return 0;
            var t = obj.GetType();
            foreach (var n in names)
            {
                var f = t.GetField(n, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (f != null)
                {
                    try
                    {
                        if (f.FieldType == typeof(int)) return (int)f.GetValue(obj);
                        if (f.FieldType == typeof(float)) return (int)(float)f.GetValue(obj);
                    }
                    catch { }
                }
                var p = t.GetProperty(n, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (p != null && p.CanRead)
                {
                    try
                    {
                        if (p.PropertyType == typeof(int)) return (int)p.GetValue(obj, null);
                        if (p.PropertyType == typeof(float)) return (int)(float)p.GetValue(obj, null);
                    }
                    catch { }
                }
            }
            return 0;
        }

        private float GetFloat(object obj, string[] names)
        {
            if (obj == null) return 0f;
            var t = obj.GetType();
            foreach (var n in names)
            {
                var f = t.GetField(n, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (f != null)
                {
                    try
                    {
                        if (f.FieldType == typeof(float)) return (float)f.GetValue(obj);
                        if (f.FieldType == typeof(int)) return (int)f.GetValue(obj);
                    }
                    catch { }
                }
                var p = t.GetProperty(n, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (p != null && p.CanRead)
                {
                    try
                    {
                        if (p.PropertyType == typeof(float)) return (float)p.GetValue(obj, null);
                        if (p.PropertyType == typeof(int)) return (int)p.GetValue(obj, null);
                    }
                    catch { }
                }
            }
            return 0f;
        }
    }
}
