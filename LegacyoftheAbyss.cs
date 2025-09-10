using BepInEx;
using HarmonyLib;
using UnityEngine;

[BepInPlugin("com.legacyoftheabyss.helper", "Legacy of the Abyss - Helper", "0.1.0")]
public class LegacyHelper : BaseUnityPlugin
{
    private static GameObject helper;

    void Awake()
    {
        Logger.LogInfo("Patching GameManager.BeginScene...");
        Harmony harmony = new Harmony("com.legacyoftheabyss.helper");
        harmony.PatchAll();
    }

    [HarmonyPatch(typeof(GameManager), "BeginScene")]
    class GameManager_BeginScene_Patch
    {
        static void Postfix(GameManager __instance)
        {
            if (__instance.IsGameplayScene())
            {
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

                    // ShadeController is the class handling all input for the shade object
                    helper.AddComponent<ShadeController>();
                }
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
                            int newHp = Mathf.Max(0, hp - damage);
                            f.SetValue(hm, newHp);
                            if (hp != newHp) Debug.Log($"[HelperMod] HM.{fname}: {hp}→{newHp}");
                            TryCallDeathIfZero(hm, newHp);
                            return true;
                        }
                        else
                        {
                            float hp = (float)f.GetValue(hm);
                            float newHp = Mathf.Max(0f, hp - damage);
                            f.SetValue(hm, newHp);
                            if (Mathf.Abs(hp - newHp) > 0.001f) Debug.Log($"[HelperMod] HM.{fname}: {hp}→{newHp}");
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
                            int newHp = Mathf.Max(0, hp - damage);
                            p.SetValue(hm, newHp, null);
                            if (hp != newHp) Debug.Log($"[HelperMod] HM.{pname}: {hp}→{newHp}");
                            TryCallDeathIfZero(hm, newHp);
                            return true;
                        }
                        else if (p.PropertyType == typeof(float))
                        {
                            float hp = (float)p.GetValue(hm, null);
                            float newHp = Mathf.Max(0f, hp - damage);
                            p.SetValue(hm, newHp, null);
                            if (Mathf.Abs(hp - newHp) > 0.001f) Debug.Log($"[HelperMod] HM.{pname}: {hp}→{newHp}");
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
                            args[i] = CreateHitInstance(pt);
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
            go.SendMessage("OnHit", SendMessageOptions.DontRequireReceiver);
            go.SendMessage("SpawnDeath", SendMessageOptions.DontRequireReceiver);
        }

        private object CreateHitInstance(System.Type hitType)
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
}
