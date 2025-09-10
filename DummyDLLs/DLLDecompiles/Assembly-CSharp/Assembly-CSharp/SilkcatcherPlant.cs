using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005C5 RID: 1477
public class SilkcatcherPlant : MonoBehaviour, IHitResponder, IBreakerBreakable
{
	// Token: 0x170005D5 RID: 1493
	// (get) Token: 0x060034B1 RID: 13489 RVA: 0x000E9E89 File Offset: 0x000E8089
	public BreakableBreaker.BreakableTypes BreakableType
	{
		get
		{
			return BreakableBreaker.BreakableTypes.Basic;
		}
	}

	// Token: 0x060034B2 RID: 13490 RVA: 0x000E9E8C File Offset: 0x000E808C
	private void Awake()
	{
		PersistentBoolItem component = base.GetComponent<PersistentBoolItem>();
		if (component != null)
		{
			component.OnGetSaveState += delegate(out bool val)
			{
				val = this.destroyed;
			};
			component.OnSetSaveState += delegate(bool val)
			{
				if (val)
				{
					base.gameObject.SetActive(false);
				}
			};
			component.SemiPersistentReset += delegate()
			{
				this.destroyed = false;
				base.GetComponent<CircleCollider2D>().enabled = true;
				this.spriteRenderer.enabled = true;
				this.silkSpriteRenderer.enabled = true;
				this.breakEffects.SetActive(false);
				this.ptIdle.Play();
				this.ptBreak.Stop();
				if (this.silkGetEffect)
				{
					this.silkGetEffect.SetActive(false);
				}
			};
		}
		if (this.silkGetEffect)
		{
			this.silkGetEffect.SetActive(false);
		}
	}

	// Token: 0x060034B3 RID: 13491 RVA: 0x000E9EF8 File Offset: 0x000E80F8
	private void Start()
	{
		Transform transform = base.transform;
		if (!this.doNotRotate)
		{
			transform.SetRotationZ(Random.Range(0f, 360f));
		}
		int index = Random.Range(0, this.plantSprites.Count - 1);
		this.spriteRenderer.sprite = this.plantSprites[index];
		this.silkSpriteRenderer.sprite = this.silkSprites[index];
		if (!this.doNotFlip && Random.Range(1, 100) < 50)
		{
			Vector3 localScale = transform.localScale;
			localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
			transform.localScale = localScale;
		}
		this.heroController = GameManager.instance.hero_ctrl;
	}

	// Token: 0x060034B4 RID: 13492 RVA: 0x000E9FB7 File Offset: 0x000E81B7
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		this.Destroy(damageInstance.IsNailDamage);
		return IHitResponder.Response.GenericHit;
	}

	// Token: 0x060034B5 RID: 13493 RVA: 0x000E9FCC File Offset: 0x000E81CC
	public void BreakFromBreaker(BreakableBreaker breaker)
	{
		this.Destroy(false);
	}

	// Token: 0x060034B6 RID: 13494 RVA: 0x000E9FD5 File Offset: 0x000E81D5
	public void HitFromBreaker(BreakableBreaker breaker)
	{
		this.Destroy(false);
	}

	// Token: 0x060034B7 RID: 13495 RVA: 0x000E9FE0 File Offset: 0x000E81E0
	private void Destroy(bool giveSilk)
	{
		base.GetComponent<CircleCollider2D>().enabled = false;
		this.spriteRenderer.enabled = false;
		this.silkSpriteRenderer.enabled = false;
		this.breakEffects.SetActive(true);
		this.strikePrefab.Spawn(base.transform.position);
		this.slashPrefab.Spawn(base.transform.position);
		this.ptIdle.Stop();
		this.ptBreak.Play();
		if (this.silkGetEffect)
		{
			this.silkGetEffect.SetActive(giveSilk);
		}
		if (!giveSilk)
		{
			return;
		}
		if (!this.breakShake.TryShake(this, true))
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("CameraParent");
			if (gameObject != null)
			{
				PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(gameObject, "CameraShake");
				if (playMakerFSM != null)
				{
					playMakerFSM.SendEvent("EnemyKillShake");
				}
			}
		}
		this.heroController.AddSilk(2, true);
		this.destroyed = true;
	}

	// Token: 0x060034B9 RID: 13497 RVA: 0x000EA0DD File Offset: 0x000E82DD
	GameObject IBreakerBreakable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x04003820 RID: 14368
	public List<Sprite> plantSprites;

	// Token: 0x04003821 RID: 14369
	public List<Sprite> silkSprites;

	// Token: 0x04003822 RID: 14370
	public GameObject breakEffects;

	// Token: 0x04003823 RID: 14371
	public GameObject silkGetEffect;

	// Token: 0x04003824 RID: 14372
	public GameObject strikePrefab;

	// Token: 0x04003825 RID: 14373
	public GameObject slashPrefab;

	// Token: 0x04003826 RID: 14374
	public ParticleSystem ptIdle;

	// Token: 0x04003827 RID: 14375
	public ParticleSystem ptBreak;

	// Token: 0x04003828 RID: 14376
	public SpriteRenderer spriteRenderer;

	// Token: 0x04003829 RID: 14377
	public SpriteRenderer silkSpriteRenderer;

	// Token: 0x0400382A RID: 14378
	public CameraShakeTarget breakShake;

	// Token: 0x0400382B RID: 14379
	[SerializeField]
	private bool doNotRotate;

	// Token: 0x0400382C RID: 14380
	[SerializeField]
	private bool doNotFlip;

	// Token: 0x0400382D RID: 14381
	private HeroController heroController;

	// Token: 0x0400382E RID: 14382
	private bool destroyed;
}
