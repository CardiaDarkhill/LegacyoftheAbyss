using System;
using GlobalSettings;
using UnityEngine;

// Token: 0x02000552 RID: 1362
[RequireComponent(typeof(Rigidbody2D))]
public class SimpleProjectile : MonoBehaviour
{
	// Token: 0x060030B4 RID: 12468 RVA: 0x000D7088 File Offset: 0x000D5288
	private bool? ValidateAnim(string animName)
	{
		if (!this.animator || string.IsNullOrEmpty(animName))
		{
			return null;
		}
		return new bool?(this.animator.GetClipByName(animName) != null);
	}

	// Token: 0x060030B5 RID: 12469 RVA: 0x000D70C8 File Offset: 0x000D52C8
	private void Awake()
	{
		this.damager.DamagedEnemy += this.Break;
		this.damagerCollider = this.damager.GetComponent<Collider2D>();
		this.body = base.GetComponent<Rigidbody2D>();
		this.tinter = base.GetComponent<PoisonTintBase>();
	}

	// Token: 0x060030B6 RID: 12470 RVA: 0x000D7118 File Offset: 0x000D5318
	private void OnEnable()
	{
		this.damagerCollider.enabled = true;
		this.hasHit = false;
		this.breakOnLand = false;
		this.hasBrokenOnLand = false;
		this.body.linearVelocity = Vector2.zero;
		this.body.gravityScale = 0f;
		this.body.isKinematic = false;
		this.meshRenderer.enabled = true;
		this.terrainDetector.enabled = true;
		this.isPoison = Gameplay.PoisonPouchTool.IsEquipped;
		this.queuedUpdate = true;
	}

	// Token: 0x060030B7 RID: 12471 RVA: 0x000D71A4 File Offset: 0x000D53A4
	private void Update()
	{
		if (this.recycleTimer > 0f)
		{
			this.recycleTimer -= Time.deltaTime;
			if (this.recycleTimer <= 0f)
			{
				base.gameObject.Recycle();
			}
		}
		if (this.hasBrokenOnLand && this.breakEffectCurrent && !this.breakEffectCurrent.IsAlive(true))
		{
			base.gameObject.Recycle();
		}
		if (this.hasHit)
		{
			return;
		}
		if (this.queuedUpdate)
		{
			this.queuedUpdate = false;
			if (this.animator && !string.IsNullOrEmpty(this.shootAnim))
			{
				this.animator.Play(this.shootAnim);
			}
			Vector2 normalized = this.body.linearVelocity.normalized;
			float num = Vector2.Angle(Vector2.right, normalized);
			if (this.damager)
			{
				this.damager.direction = num;
			}
			base.transform.SetRotation2D(num);
		}
	}

	// Token: 0x060030B8 RID: 12472 RVA: 0x000D72A0 File Offset: 0x000D54A0
	private void OnCollisionEnter2D(Collision2D other)
	{
		if (this.breakOnLand)
		{
			this.DoBreakEffect();
			this.meshRenderer.enabled = false;
			this.body.isKinematic = true;
			this.body.linearVelocity = Vector2.zero;
			this.hasBrokenOnLand = true;
			return;
		}
		if (this.hasHit)
		{
			return;
		}
		Vector2 point = other.GetSafeContact().Point;
		if (this.terrainImpactSpawn)
		{
			this.terrainImpactSpawn.Spawn(point.ToVector3(this.terrainImpactSpawn.transform.position.z), Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
		}
		this.DidHit(other);
	}

	// Token: 0x060030B9 RID: 12473 RVA: 0x000D7359 File Offset: 0x000D5559
	private void DoBreakEffect()
	{
		this.breakEffectCurrent = (this.isPoison ? this.breakEffectPoison : this.breakEffect);
		if (this.breakEffectCurrent)
		{
			this.breakEffectCurrent.Play(true);
		}
	}

	// Token: 0x060030BA RID: 12474 RVA: 0x000D7390 File Offset: 0x000D5590
	public void Break()
	{
		if (this.hasHit)
		{
			return;
		}
		this.DidHit(null);
	}

	// Token: 0x060030BB RID: 12475 RVA: 0x000D73A4 File Offset: 0x000D55A4
	private void DidHit(Collision2D other)
	{
		if (Random.Range(0f, 1f) <= this.spawnChance)
		{
			this.BreakSpawn(other);
			base.gameObject.Recycle();
			return;
		}
		if (this.tinter)
		{
			this.tinter.Clear();
		}
		if (Random.Range(0f, 1f) <= this.shatterChance)
		{
			this.DoBreakEffect();
			this.meshRenderer.enabled = false;
			this.body.isKinematic = true;
			this.body.linearVelocity = Vector2.zero;
		}
		else
		{
			this.FlingBack(base.gameObject, other);
			this.body.gravityScale = 1f;
			if (this.brokenAnims.Length != 0)
			{
				this.animator.Play(this.brokenAnims.GetRandomElement<string>());
			}
			if (Random.Range(0f, 1f) <= this.brokenSolidChance)
			{
				this.breakOnLand = true;
			}
			else
			{
				this.terrainDetector.enabled = false;
			}
		}
		this.hasHit = true;
		this.damagerCollider.enabled = false;
		this.recycleTimer = 5f;
	}

	// Token: 0x060030BC RID: 12476 RVA: 0x000D74C0 File Offset: 0x000D56C0
	private void BreakSpawn(Collision2D other)
	{
		if (!this.spawnFlingOnBreak)
		{
			return;
		}
		GameObject obj = this.spawnFlingOnBreak.Spawn();
		this.FlingBack(obj, other);
	}

	// Token: 0x060030BD RID: 12477 RVA: 0x000D74F0 File Offset: 0x000D56F0
	private void FlingBack(GameObject obj, Collision2D other)
	{
		FlingUtils.SelfConfig selfConfig = this.spawnFling.GetSelfConfig(obj);
		bool flag;
		if (other != null)
		{
			Collision2DUtils.Collision2DSafeContact safeContact = other.GetSafeContact();
			if (Mathf.Abs(Vector2.Dot(safeContact.Normal, Vector2.right)) > 0.5f)
			{
				flag = (safeContact.Point.x < base.transform.position.x);
			}
			else
			{
				flag = (this.body.linearVelocity.x > 0f);
			}
		}
		else
		{
			float num;
			for (num = base.transform.eulerAngles.z; num < 0f; num += 360f)
			{
			}
			while (num >= 360f)
			{
				num -= 360f;
			}
			flag = (num > 90f && num < 270f);
		}
		if (flag)
		{
			selfConfig.AngleMin = Helper.GetReflectedAngle(selfConfig.AngleMin, true, false, false);
			selfConfig.AngleMax = Helper.GetReflectedAngle(selfConfig.AngleMax, true, false, false);
		}
		FlingUtils.FlingObject(selfConfig, base.transform, Vector3.zero);
	}

	// Token: 0x040033BF RID: 13247
	[SerializeField]
	private GameObject spawnFlingOnBreak;

	// Token: 0x040033C0 RID: 13248
	[SerializeField]
	[Range(0f, 1f)]
	private float spawnChance;

	// Token: 0x040033C1 RID: 13249
	[SerializeField]
	private FlingUtils.ObjectFlingParams spawnFling;

	// Token: 0x040033C2 RID: 13250
	[SerializeField]
	private GameObject terrainImpactSpawn;

	// Token: 0x040033C3 RID: 13251
	[SerializeField]
	private tk2dSpriteAnimator animator;

	// Token: 0x040033C4 RID: 13252
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateAnim")]
	private string shootAnim;

	// Token: 0x040033C5 RID: 13253
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateAnim")]
	private string[] brokenAnims;

	// Token: 0x040033C6 RID: 13254
	[SerializeField]
	[Range(0f, 1f)]
	private float shatterChance;

	// Token: 0x040033C7 RID: 13255
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private DamageEnemies damager;

	// Token: 0x040033C8 RID: 13256
	[SerializeField]
	private ParticleSystem breakEffect;

	// Token: 0x040033C9 RID: 13257
	[SerializeField]
	private ParticleSystem breakEffectPoison;

	// Token: 0x040033CA RID: 13258
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private MeshRenderer meshRenderer;

	// Token: 0x040033CB RID: 13259
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Collider2D terrainDetector;

	// Token: 0x040033CC RID: 13260
	[SerializeField]
	[Range(0f, 1f)]
	private float brokenSolidChance = 1f;

	// Token: 0x040033CD RID: 13261
	private bool queuedUpdate;

	// Token: 0x040033CE RID: 13262
	private bool hasHit;

	// Token: 0x040033CF RID: 13263
	private bool breakOnLand;

	// Token: 0x040033D0 RID: 13264
	private bool hasBrokenOnLand;

	// Token: 0x040033D1 RID: 13265
	private float recycleTimer;

	// Token: 0x040033D2 RID: 13266
	private bool isPoison;

	// Token: 0x040033D3 RID: 13267
	private ParticleSystem breakEffectCurrent;

	// Token: 0x040033D4 RID: 13268
	private Rigidbody2D body;

	// Token: 0x040033D5 RID: 13269
	private Collider2D damagerCollider;

	// Token: 0x040033D6 RID: 13270
	private PoisonTintBase tinter;
}
