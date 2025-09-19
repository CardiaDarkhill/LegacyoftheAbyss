#nullable disable
using UnityEngine;

public partial class LegacyHelper
{
    // Allow Hornet to pogo off the shade without damaging it.
    public class ShadePogoResponder : MonoBehaviour, IHitResponder
    {
        public bool HitRecurseUpwards => false;
        public int HitPriority => 0;
        private float lastBounceTime;
        private const float BounceCooldown = 0.05f;

        public IHitResponder.HitResponse Hit(HitInstance damageInstance)
        {
            // Only respond to Hornet's nail/needle attacks with a downward hit direction.
            try
            {
                if (damageInstance.AttackType != AttackTypes.Nail && damageInstance.AttackType != AttackTypes.NailBeam)
                    return IHitResponder.Response.None;

                if (!damageInstance.IsHeroDamage)
                    return IHitResponder.Response.None;

                float ang = damageInstance.Direction;
                if (ang < 0f) ang += 360f;
                // Consider downward band generous to account for angle variance.
                bool isDown = (ang >= 160f && ang <= 360f);
                if (!isDown)
                    return IHitResponder.Response.None;

                // Signal a generic hit so NailSlashRecoil can trigger RecoilDown.
                SpawnPuff();
                return IHitResponder.Response.GenericHit;
            }
            catch { return IHitResponder.Response.GenericHit; }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Fallback path: if hero down-slash collider hits us, force a recoil down.
            if (Time.time - lastBounceTime < BounceCooldown) return;
            try
            {
                var ns = other ? other.GetComponentInParent<NailSlash>() : null;
                if (!ns) return;
                var hc = HeroController.instance;
                if (!hc) return;
                // Require a downward attack state to avoid horizontal recoils
                bool down = false;
                try { down = hc.cState.downAttacking; } catch { down = false; }
                if (!down)
                {
                    // If state not available, infer by vertical relation
                    if (hc.transform && transform)
                        down = (hc.transform.position.y > transform.position.y + 0.25f);
                }
                if (down)
                {
                    lastBounceTime = Time.time;
                    SpawnPuff();
                    hc.RecoilDown();
                }
            }
            catch { }
        }

        private static Sprite s_dotSprite;
        private static Sprite GetDot()
        {
            if (s_dotSprite) return s_dotSprite;
            var tex = new Texture2D(6, 6, TextureFormat.ARGB32, false);
            for (int x = 0; x < tex.width; x++)
                for (int y = 0; y < tex.height; y++)
                    tex.SetPixel(x, y, Color.black);
            tex.Apply();
            s_dotSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 24f);
            return s_dotSprite;
        }

        private void SpawnPuff()
        {
            try
            {
                int count = 10;
                for (int i = 0; i < count; i++)
                {
                    var go = new GameObject("ShadePogoDot");
                    go.transform.position = transform.position + new Vector3(Random.Range(-0.15f, 0.15f), 0.05f + Random.Range(-0.05f, 0.05f), 0f);
                    var sr = go.AddComponent<SpriteRenderer>();
                    sr.sprite = GetDot();
                    sr.color = new Color(0f, 0f, 0f, 0.9f);
                    go.AddComponent<SimpleRiseFade>();
                }
            }
            catch { }
        }

        private class SimpleRiseFade : MonoBehaviour
        {
            private float life = 0.35f;
            private float vy;
            private float vx;
            private SpriteRenderer sr;
            void Awake()
            {
                sr = GetComponent<SpriteRenderer>();
                vy = Random.Range(1.5f, 3.0f);
                vx = Random.Range(-0.8f, 0.8f);
                transform.localScale = Vector3.one * Random.Range(0.5f, 1.1f);
            }
            void Update()
            {
                float dt = Time.deltaTime;
                life -= dt;
                transform.position += new Vector3(vx, vy, 0f) * dt;
                vy -= 6f * dt; // slight gravity
                if (sr)
                {
                    var c = sr.color; c.a = Mathf.Max(0f, life / 0.35f * 0.9f); sr.color = c;
                }
                if (life <= 0f) Destroy(gameObject);
            }
        }
    }
}
#nullable restore
