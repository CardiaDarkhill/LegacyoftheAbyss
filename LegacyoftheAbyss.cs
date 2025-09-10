using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;
using System.Reflection;

[BepInPlugin("com.legacyoftheabyss.helper", "Legacy of the Abyss - Helper", "0.1.0")]
public class LegacyHelper : BaseUnityPlugin
{
    private static GameObject helper;
    private static GameObject hud;
    private static bool loggedStartupFields;

    void Awake()
    {
        Logger.LogInfo("Patching GameManager.BeginScene...");
        Harmony harmony = new Harmony("com.legacyoftheabyss.helper");
        harmony.PatchAll();
        SceneManager.sceneLoaded += (s, m) => DisableStartupObjects(s);
        DisableStartupObjects(SceneManager.GetActiveScene());
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

    private static void DisableStartupObjects(Scene scene)
    {
        foreach (var go in scene.GetRootGameObjects())
        {
            var lname = go.name.ToLower();
            if (lname.Contains("team") && lname.Contains("cherry") ||
                (lname.Contains("save") && lname.Contains("reminder")))
            {
                go.SetActive(false);
                Debug.Log($"[HelperMod] Disabled startup object {go.name}");
            }
        }
    }

    [HarmonyPatch(typeof(GameManager), "BeginScene")]
    class GameManager_BeginScene_Patch
    {
        static void Postfix(GameManager __instance)
        {
            DisableStartup(__instance);
            DisableStartupObjects(SceneManager.GetActiveScene());

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
            }

            if (hud == null)
                hud = new GameObject("KnightHUDRoot");

            var kh = hud.GetComponent<KnightHUD>();
            if (kh == null) kh = hud.AddComponent<KnightHUD>();
            kh.Init(__instance.hero_ctrl);
            hud.SetActive(true);
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
            DisableStartupObjects(SceneManager.GetActiveScene());
        }
    }

    [HarmonyPatch(typeof(GameManager), "Start")]
    class GameManager_Start_Patch
    {
        static void Postfix(GameManager __instance)
        {
            DisableStartup(__instance);
            DisableStartupObjects(SceneManager.GetActiveScene());
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
            proj.tag = "Hero Spell";

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
        public int damage = 20;
        public Transform hornetRoot;
        public float lifeSeconds = 1.5f;

        void Start() => Destroy(gameObject, lifeSeconds);

        void OnTriggerEnter2D(Collider2D other)
        {
            if (hornetRoot != null && other.transform.IsChildOf(hornetRoot)) return;
            if (other.transform == transform || other.transform.IsChildOf(transform)) return;

            var hm = other.GetComponentInParent<HealthManager>();
            if (hm != null)
            {
                var rb = GetComponent<Rigidbody2D>();
                float angle;
                if (rb != null)
                    angle = rb.linearVelocity.x > 0f ? 180f : 0f;
                else
                    angle = other.transform.position.x > transform.position.x ? 180f : 0f;

                var hit = new HitInstance
                {
                    Source = gameObject,
                    AttackType = AttackTypes.Spell,
                    DamageDealt = damage,
                    Direction = angle,
                    MagnitudeMultiplier = 1f,
                    IsHeroDamage = true,
                    IsFirstHit = true
                };
                hm.Hit(hit);
            }

            Destroy(gameObject);
        }
    }

}
