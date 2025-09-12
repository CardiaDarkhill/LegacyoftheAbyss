using UnityEngine;

public partial class LegacyHelper
{
    // Attach to PogoTarget to emit a simple puff when the hero slash collides.
    public class ShadePogoPuff : MonoBehaviour
    {
        private float last;
        private const float Cooldown = 0.08f;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (Time.time - last < Cooldown) return;
            try
            {
                if (!HeroController.instance) return;
                if (!HeroController.instance.cState.downAttacking) return;
                // Prefer only collisions from hero attack layer
                int heroAttack = LayerMask.NameToLayer("Hero Attack");
                if (heroAttack >= 0 && collision.collider.gameObject.layer != heroAttack) return;
                last = Time.time;
                SpawnPuff(transform.position + Vector3.up * 0.1f);
                ForceBounce();
            }
            catch { }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (Time.time - last < Cooldown) return;
            try
            {
                if (!HeroController.instance) return;
                if (!HeroController.instance.cState.downAttacking) return;
                last = Time.time;
                SpawnPuff(transform.position + Vector3.up * 0.1f);
                ForceBounce();
            }
            catch { }
        }

        private static void ForceBounce()
        {
            try
            {
                var hc = HeroController.instance;
                if (!hc) return;
                // Mimic HeroDownAttack bounce path
                hc.StartDownspikeInvulnerability();
                if (hc.CanCustomRecoil())
                {
                    hc.AffectedByGravity(false);
                    hc.ResetVelocity();
                }
                hc.DownspikeBounce(false, null);
            }
            catch { }
        }

        // Procedural impact sprites (soft blob, smudge, ring)
        private static Sprite s_blobSprite;
        private static Sprite s_smudgeSprite;
        private static Sprite s_ringSprite;

        private static Sprite GetImpactSprite()
        {
            EnsureImpactSprites();
            int pick = Random.Range(0, 6); // weight: blob 3, smudge 2, ring 1
            if (pick <= 2) return s_blobSprite;
            if (pick <= 4) return s_smudgeSprite;
            return s_ringSprite;
        }

        private static void EnsureImpactSprites()
        {
            if (!s_blobSprite) s_blobSprite = BuildSoftBlob(48);
            if (!s_smudgeSprite) s_smudgeSprite = BuildSmudge(64, 0.55f);
            if (!s_ringSprite) s_ringSprite = BuildRing(64, 0.35f, 0.6f);
        }

        private static Sprite BuildSoftBlob(int size)
        {
            var tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
            tex.filterMode = FilterMode.Bilinear;
            Vector2 c = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            float maxR = c.x;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = (x - c.x) / maxR;
                    float dy = (y - c.y) / maxR;
                    float r = Mathf.Sqrt(dx * dx + dy * dy);
                    float a = Mathf.Clamp01(1f - r);
                    // Sharpen center, soften edge
                    a = Mathf.Pow(a, 2.2f);
                    tex.SetPixel(x, y, new Color(0f, 0f, 0f, a));
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 64f);
        }

        private static Sprite BuildSmudge(int size, float aspect)
        {
            var tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
            tex.filterMode = FilterMode.Bilinear;
            Vector2 c = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            float rx = c.x; float ry = c.y * aspect; // ellipse radii
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = (x - c.x) / rx;
                    float dy = (y - c.y) / ry;
                    float r = Mathf.Sqrt(dx * dx + dy * dy);
                    float a = Mathf.Clamp01(1f - r);
                    // Slight asymmetry: fade bottom quicker
                    float ny = (y - c.y) / c.y; if (ny < 0f) a *= 0.7f + 0.3f * (1f + ny);
                    a = Mathf.Pow(a, 1.8f);
                    tex.SetPixel(x, y, new Color(0f, 0f, 0f, a));
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 64f);
        }

        private static Sprite BuildRing(int size, float innerFrac, float outerFrac)
        {
            var tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
            tex.filterMode = FilterMode.Bilinear;
            Vector2 c = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            float rMax = c.x * outerFrac;
            float rMin = c.x * innerFrac;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - c.x; float dy = y - c.y;
                    float r = Mathf.Sqrt(dx * dx + dy * dy);
                    float a = 0f;
                    if (r <= rMax && r >= rMin)
                    {
                        float t = Mathf.InverseLerp(rMax, rMin, r);
                        a = Mathf.SmoothStep(0f, 1f, Mathf.Min(t, 1f - t)) * 0.9f;
                    }
                    tex.SetPixel(x, y, new Color(0f, 0f, 0f, a));
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 64f);
        }

        private static void SpawnPuff(Vector3 pos)
        {
            int count = 22;
            for (int i = 0; i < count; i++)
            {
                var go = new GameObject("ShadePogoDot");
                go.transform.position = pos + new Vector3(Random.Range(-0.18f, 0.18f), Random.Range(-0.06f, 0.06f), 0f);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = GetImpactSprite();
                sr.sortingOrder = 20000;
                sr.color = new Color(0f, 0f, 0f, 1f);
                go.AddComponent<SimpleRiseFade>();
                // Random rotation for variety
                go.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            }
        }

        private class SimpleRiseFade : MonoBehaviour
        {
            private float life = 0.22f;
            private float vy;
            private float vx;
            private SpriteRenderer sr;
            void Awake()
            {
                sr = GetComponent<SpriteRenderer>();
                float angle = Random.Range(-75f, 75f) * Mathf.Deg2Rad;
                float speed = Random.Range(3.0f, 6.2f);
                vx = Mathf.Cos(angle) * speed;
                vy = Mathf.Sin(angle) * speed + 1.2f;
                // Slight anisotropy for a more organic feel
                float s = Random.Range(0.7f, 1.5f);
                float sx = s * Random.Range(0.8f, 1.4f);
                float sy = s * Random.Range(0.8f, 1.4f);
                transform.localScale = new Vector3(sx, sy, 1f);
            }
            void Update()
            {
                float dt = Time.deltaTime;
                life -= dt;
                transform.position += new Vector3(vx, vy, 0f) * dt;
                vy -= 18f * dt;
                transform.localScale += Vector3.one * (2.4f * dt);
                if (sr)
                {
                    var c = sr.color; c.a = Mathf.Clamp01(life / 0.22f); sr.color = c;
                }
                if (life <= 0f) Destroy(gameObject);
            }
        }
    }
}
