using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200031A RID: 794
public class SpellFluke : MonoBehaviour
{
	// Token: 0x06001BF9 RID: 7161 RVA: 0x000823C8 File Offset: 0x000805C8
	private void Awake()
	{
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.body = base.GetComponent<Rigidbody2D>();
		this.spriteFlash = base.GetComponent<SpriteFlash>();
		this.objectBounce = base.GetComponent<ObjectBounce>();
	}

	// Token: 0x06001BFA RID: 7162 RVA: 0x00082406 File Offset: 0x00080606
	private void Start()
	{
		this.damager.OnTriggerEntered += delegate(Collider2D collider, GameObject sender)
		{
			this.DoDamage(collider.gameObject, 2, true);
		};
		if (this.objectBounce)
		{
			this.objectBounce.Bounced += delegate()
			{
				if (this.hasBursted)
				{
					return;
				}
				this.hasBounced = true;
				if (this.body)
				{
					Vector2 linearVelocity = this.body.linearVelocity;
					linearVelocity.x = Random.Range(-5f, 5f);
					linearVelocity.y = Mathf.Clamp(linearVelocity.y, Random.Range(7.3f, 15f), Random.Range(20f, 25f));
					this.body.linearVelocity = linearVelocity;
				}
				if (this.animator)
				{
					this.animator.Play(this.flopAnim);
				}
				base.transform.SetRotationZ(0f);
			};
		}
	}

	// Token: 0x06001BFB RID: 7163 RVA: 0x00082444 File Offset: 0x00080644
	private void DoDamage(GameObject obj, int upwardRecursionAmount, bool burst = true)
	{
		HealthManager component = obj.GetComponent<HealthManager>();
		if (component)
		{
			if (component.IsInvincible && obj.tag != "Spell Vulnerable")
			{
				return;
			}
			if (!component.isDead)
			{
				component.hp -= this.damage;
				if (component.hp <= 0)
				{
					component.Die(new float?(0f), AttackTypes.Generic, false);
				}
			}
		}
		SpriteFlash component2 = obj.GetComponent<SpriteFlash>();
		if (component2)
		{
			component2.FlashShadowRecharge();
		}
		FSMUtility.SendEventToGameObject(obj.gameObject, "TOOK DAMAGE", false);
		upwardRecursionAmount--;
		if (upwardRecursionAmount > 0 && obj.transform.parent)
		{
			this.DoDamage(obj.transform.parent.gameObject, upwardRecursionAmount, false);
		}
		if (burst)
		{
			this.Burst();
		}
	}

	// Token: 0x06001BFC RID: 7164 RVA: 0x00082514 File Offset: 0x00080714
	private void OnEnable()
	{
		if (this.animator)
		{
			this.animator.Play(this.airAnim);
		}
		this.lifeEndTime = Time.timeAsDouble + (double)Random.Range(2f, 3f);
		if (this.meshRenderer)
		{
			this.meshRenderer.enabled = true;
		}
		if (this.body)
		{
			this.body.isKinematic = false;
		}
		float num = Random.Range(0.7f, 0.9f);
		base.transform.localScale = new Vector3(num, num, 0f);
		this.damage = 4;
		if (this.spriteFlash)
		{
			this.spriteFlash.flashArmoured();
		}
		this.hasBounced = false;
		this.hasBursted = false;
	}

	// Token: 0x06001BFD RID: 7165 RVA: 0x000825E4 File Offset: 0x000807E4
	private void Update()
	{
		if (this.hasBursted)
		{
			return;
		}
		if (!this.hasBounced)
		{
			Vector2 linearVelocity = this.body.linearVelocity;
			float z = Mathf.Atan2(linearVelocity.y, linearVelocity.x) * 57.295776f;
			base.transform.localEulerAngles = new Vector3(0f, 0f, z);
		}
		if (Time.timeAsDouble >= this.lifeEndTime)
		{
			this.Burst();
		}
	}

	// Token: 0x06001BFE RID: 7166 RVA: 0x00082654 File Offset: 0x00080854
	private void Burst()
	{
		if (!this.hasBursted)
		{
			base.StartCoroutine(this.BurstSequence());
		}
		this.hasBursted = true;
	}

	// Token: 0x06001BFF RID: 7167 RVA: 0x00082672 File Offset: 0x00080872
	private IEnumerator BurstSequence()
	{
		if (this.meshRenderer)
		{
			this.meshRenderer.enabled = false;
		}
		if (this.body)
		{
			this.body.linearVelocity = Vector2.zero;
			this.body.angularVelocity = 0f;
			this.body.isKinematic = true;
		}
		if (this.splatEffect)
		{
			this.splatEffect.SetActive(true);
		}
		this.splatSounds.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
		yield return new WaitForSeconds(1f);
		base.gameObject.Recycle();
		yield break;
	}

	// Token: 0x04001AF5 RID: 6901
	public string airAnim = "Air";

	// Token: 0x04001AF6 RID: 6902
	public string flopAnim = "Flop";

	// Token: 0x04001AF7 RID: 6903
	public TriggerEnterEvent damager;

	// Token: 0x04001AF8 RID: 6904
	public GameObject splatEffect;

	// Token: 0x04001AF9 RID: 6905
	public AudioSource audioPlayerPrefab;

	// Token: 0x04001AFA RID: 6906
	public AudioEventRandom splatSounds;

	// Token: 0x04001AFB RID: 6907
	private double lifeEndTime;

	// Token: 0x04001AFC RID: 6908
	private int damage;

	// Token: 0x04001AFD RID: 6909
	private bool hasBounced;

	// Token: 0x04001AFE RID: 6910
	private bool hasBursted;

	// Token: 0x04001AFF RID: 6911
	private tk2dSpriteAnimator animator;

	// Token: 0x04001B00 RID: 6912
	private MeshRenderer meshRenderer;

	// Token: 0x04001B01 RID: 6913
	private Rigidbody2D body;

	// Token: 0x04001B02 RID: 6914
	private SpriteFlash spriteFlash;

	// Token: 0x04001B03 RID: 6915
	private ObjectBounce objectBounce;
}
