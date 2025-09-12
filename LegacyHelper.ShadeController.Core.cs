using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public partial class LegacyHelper
{
    public partial class ShadeController : MonoBehaviour
    {
        // Movement and leash
        public float moveSpeed = 8f;
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
        private Renderer[] shadeLightRenderers;
        public float simpleLightSize = 14f;
        private static Texture2D s_simpleLightTex;
        private static Material s_simpleAdditiveMat;
        private static Mesh s_simpleQuadMesh;
        private int facing = 1;
        private float nailTimer;
        private const KeyCode FireKey = KeyCode.Space;
        private const KeyCode NailKey = KeyCode.J;

        // Shade Soul resource
        public int shadeSoulMax = 99;
        public int shadeSoul;
        public int soulGainPerHit = 11;
        public int projectileSoulCost = 33;

        private SimpleHUD cachedHud;
        private float hurtCooldown;
        private const float HurtIFrameSeconds = 1.35f;

        private int lastSavedHP;
        private int lastSavedMax;
        private int lastSavedSoul;

        public void RestorePersistentState(int hp, int max, int soul)
        {
            shadeMaxHP = Mathf.Max(1, max);
            shadeHP = Mathf.Clamp(hp, 0, shadeMaxHP);
            shadeSoul = Mathf.Clamp(soul, 0, shadeSoulMax);
        }

        public void FullHealFromBench()
        {
            shadeHP = Mathf.Max(shadeHP, shadeMaxHP);
            PushShadeStatsToHud();
        }

        public int GetCurrentHP() => shadeHP;
        public int GetMaxHP() => shadeMaxHP;
        public int GetShadeSoul() => shadeSoul;

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
            if (sr != null)
            {
                var c = sr.color; c.a = 0.9f; sr.color = c;
            }

            SetupShadeLight();
            cachedHud = Object.FindFirstObjectByType<SimpleHUD>();
            PushSoulToHud();
            CheckHazardOverlap();
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
        }

        private void Update()
        {
            if (hornetTransform == null) return;

            if (hazardCooldown > 0f) hazardCooldown = Mathf.Max(0f, hazardCooldown - Time.deltaTime);
            if (hurtCooldown > 0f) hurtCooldown = Mathf.Max(0f, hurtCooldown - Time.deltaTime);

            HandleMovementAndFacing();
            if (!inHardLeash)
            {
                HandleFire();
                HandleNailAttack();
            }

            if (!cachedHud) cachedHud = Object.FindFirstObjectByType<SimpleHUD>();
            PushSoulToHud();
            CheckHazardOverlap();
            SyncShadeLight();
            PersistIfChanged();
        }

        public void ApplyBindHealFromHornet(Transform hornet)
        {
            try
            {
                var h = hornet != null ? hornet : hornetTransform;
                if (h == null) return;
                float dist = Vector2.Distance(h.position, transform.position);
                if (dist <= 3.5f)
                {
                    int before = shadeHP;
                    shadeHP = Mathf.Min(shadeHP + 2, shadeMaxHP);
                    if (shadeHP != before)
                    {
                        PushShadeStatsToHud();
                        PersistIfChanged();
                    }
                }
            }
            catch { }
        }

        private void PersistIfChanged()
        {
            if (lastSavedHP != shadeHP || lastSavedMax != shadeMaxHP || lastSavedSoul != shadeSoul)
            {
                LegacyHelper.SaveShadeState(shadeHP, shadeMaxHP, shadeSoul);
                lastSavedHP = shadeHP; lastSavedMax = shadeMaxHP; lastSavedSoul = shadeSoul;
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
            float h = (Input.GetKey(KeyCode.A) ? -1f : 0f) + (Input.GetKey(KeyCode.D) ? 1f : 0f);
            float v = (Input.GetKey(KeyCode.S) ? -1f : 0f) + (Input.GetKey(KeyCode.W) ? 1f : 0f);
            Vector2 input = new Vector2(h, v);
            if (input.sqrMagnitude > 1f) input.Normalize();

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
                moveDelta += input * moveSpeed * Time.deltaTime;

            if (rb) rb.MovePosition(rb.position + moveDelta);
            else transform.position += (Vector3)moveDelta;

            if (h > 0.1f) facing = 1;
            else if (h < -0.1f) facing = -1;
            else if (Mathf.Abs(to.x) > 0.1f) facing = (to.x >= 0f ? 1 : -1);

            if (sr != null) sr.flipX = (facing == -1);

            if (dist > maxDistance)
            {
                Vector3 toShade = transform.position - hornetTransform.position;
                transform.position = hornetTransform.position + toShade.normalized * maxDistance;
            }
        }

        private void HandleFire()
        {
            fireTimer -= Time.deltaTime;
            if (!Input.GetKey(FireKey) || fireTimer > 0f) return;
            if (shadeSoul < projectileSoulCost) return;
            fireTimer = fireCooldown;
            shadeSoul = Mathf.Max(0, shadeSoul - projectileSoulCost);
            PushSoulToHud();
            CheckHazardOverlap();

            Vector2 dir = new Vector2(facing, 0f);
            SpawnProjectile(dir);
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

        private void TryProcessDamageHero(Collider2D col)
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
                    OnShadeHitEnemy(dh);
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
                if (hornetTransform && (c.transform == hornetTransform || c.transform.IsChildOf(hornetTransform))) continue;
                var dh = c.GetComponentInParent<DamageHero>();
                if (dh != null)
                {
                    var hz = GetHazardType(dh);
                    if (IsTerrainHazard(hz)) { OnShadeHitHazard(); return; }
                    OnShadeHitEnemy(dh);
                    return;
                }
            }
        }

        private static GlobalEnums.HazardType GetHazardType(DamageHero dh)
        {
            try
            {
                var tf = typeof(DamageHero).GetField("hazardType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (tf != null) return (GlobalEnums.HazardType)tf.GetValue(dh);
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
            PersistIfChanged();
        }

        private void OnShadeHitEnemy(DamageHero dh)
        {
            if (hurtCooldown > 0f) return;
            int dmg = 1;
            try { if (dh != null) dmg = Mathf.Max(1, dh.damageDealt); } catch { }
            shadeHP = Mathf.Max(0, shadeHP - dmg);
            PushShadeStatsToHud();
            hurtCooldown = HurtIFrameSeconds;
            PersistIfChanged();
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
    }
}

