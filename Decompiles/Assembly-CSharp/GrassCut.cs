using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200023C RID: 572
public class GrassCut : MonoBehaviour, IBreakerBreakable
{
	// Token: 0x17000248 RID: 584
	// (get) Token: 0x060014EE RID: 5358 RVA: 0x0005EB75 File Offset: 0x0005CD75
	public BreakableBreaker.BreakableTypes BreakableType
	{
		get
		{
			return BreakableBreaker.BreakableTypes.Grass;
		}
	}

	// Token: 0x060014EF RID: 5359 RVA: 0x0005EB78 File Offset: 0x0005CD78
	private void Awake()
	{
		this.col = base.GetComponent<Collider2D>();
		this.children = base.GetComponentsInChildren<GrassCut>();
		if (this.persistent != null)
		{
			this.persistent.OnGetSaveState += delegate(out bool val)
			{
				val = this.isCut;
			};
			this.persistent.OnSetSaveState += delegate(bool val)
			{
				this.isCut = val;
				if (this.isCut)
				{
					base.gameObject.SetActive(false);
				}
			};
		}
	}

	// Token: 0x060014F0 RID: 5360 RVA: 0x0005EBDC File Offset: 0x0005CDDC
	private void Start()
	{
		this.enableGameObjects.SetAllActive(false);
		foreach (SpriteRenderer spriteRenderer in this.enable)
		{
			if (spriteRenderer)
			{
				spriteRenderer.enabled = false;
			}
		}
		foreach (Collider2D collider2D in this.enableColliders)
		{
			if (collider2D)
			{
				collider2D.enabled = false;
			}
		}
		if (this.particles)
		{
			this.particles.SetActive(false);
		}
	}

	// Token: 0x060014F1 RID: 5361 RVA: 0x0005EC61 File Offset: 0x0005CE61
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (GrassCut.ShouldCut(collision))
		{
			this.DoCut(collision);
		}
	}

	// Token: 0x060014F2 RID: 5362 RVA: 0x0005EC74 File Offset: 0x0005CE74
	private void DoCut(Collider2D collision)
	{
		if (this.callOnChildren)
		{
			GrassCut[] array = this.children;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Cut(collision);
			}
		}
		else
		{
			this.Cut(collision);
		}
		this.isCut = true;
		if (this.OnCut != null)
		{
			this.OnCut.Invoke();
		}
	}

	// Token: 0x060014F3 RID: 5363 RVA: 0x0005ECCC File Offset: 0x0005CECC
	public static bool ShouldCut(Collider2D collision)
	{
		NailAttackBase component = collision.GetComponent<NailAttackBase>();
		if (component && !component.CanHitSpikes)
		{
			return false;
		}
		if (collision.CompareTag("Sharp Shadow"))
		{
			return true;
		}
		DamageEnemies component2 = collision.GetComponent<DamageEnemies>();
		return (component2 && (component2.damageDealt > 0 || component2.useNailDamage) && !component2.OnlyDamageEnemies) || (collision.CompareTag("HeroBox") && HeroController.instance.cState.superDashing);
	}

	// Token: 0x060014F4 RID: 5364 RVA: 0x0005ED50 File Offset: 0x0005CF50
	public void Cut(Collider2D collision)
	{
		GrassBehaviour componentInParent = base.GetComponentInParent<GrassBehaviour>();
		foreach (SpriteRenderer spriteRenderer in this.disable)
		{
			if (!(spriteRenderer == null))
			{
				spriteRenderer.enabled = false;
			}
		}
		foreach (SpriteRenderer spriteRenderer2 in this.enable)
		{
			if (!(spriteRenderer2 == null))
			{
				spriteRenderer2.enabled = true;
			}
		}
		foreach (Collider2D collider2D in this.disableColliders)
		{
			if (!(collider2D == null))
			{
				collider2D.enabled = false;
			}
		}
		foreach (Collider2D collider2D2 in this.enableColliders)
		{
			if (!(collider2D2 == null))
			{
				collider2D2.enabled = true;
			}
		}
		foreach (GameObject gameObject in this.enableGameObjects)
		{
			if (!(gameObject == null))
			{
				gameObject.SetActive(true);
			}
		}
		if (this.particles)
		{
			this.particles.SetActive(true);
		}
		if (componentInParent)
		{
			componentInParent.CutReact(collision);
		}
		if (this.cutEffectPrefab)
		{
			Vector3 position;
			float num;
			if (collision)
			{
				position = (collision.bounds.center + this.col.bounds.center) / 2f;
				num = Mathf.Sign(base.transform.position.x - collision.transform.position.x);
			}
			else
			{
				position = this.col.bounds.center;
				num = 1f;
			}
			this.cutEffectPrefab.Spawn(position);
			this.cutEffectPrefab.transform.SetScaleX(-num * 0.6f);
			this.cutEffectPrefab.transform.SetScaleY(1f);
		}
		Object.Destroy(this);
	}

	// Token: 0x060014F5 RID: 5365 RVA: 0x0005EF43 File Offset: 0x0005D143
	public void BreakSelf()
	{
		this.DoCut(null);
	}

	// Token: 0x060014F6 RID: 5366 RVA: 0x0005EF4C File Offset: 0x0005D14C
	public void BreakFromBreaker(BreakableBreaker breaker)
	{
		this.DoCut(null);
	}

	// Token: 0x060014F7 RID: 5367 RVA: 0x0005EF55 File Offset: 0x0005D155
	public void HitFromBreaker(BreakableBreaker breaker)
	{
		this.DoCut(null);
	}

	// Token: 0x060014F9 RID: 5369 RVA: 0x0005EF66 File Offset: 0x0005D166
	GameObject IBreakerBreakable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x04001377 RID: 4983
	[SerializeField]
	private SpriteRenderer[] disable;

	// Token: 0x04001378 RID: 4984
	[SerializeField]
	private SpriteRenderer[] enable;

	// Token: 0x04001379 RID: 4985
	[Space]
	[SerializeField]
	private Collider2D[] disableColliders;

	// Token: 0x0400137A RID: 4986
	[SerializeField]
	private Collider2D[] enableColliders;

	// Token: 0x0400137B RID: 4987
	[SerializeField]
	private GameObject[] enableGameObjects;

	// Token: 0x0400137C RID: 4988
	[Space]
	[SerializeField]
	private GameObject particles;

	// Token: 0x0400137D RID: 4989
	[SerializeField]
	private GameObject cutEffectPrefab;

	// Token: 0x0400137E RID: 4990
	[Space]
	[SerializeField]
	private bool callOnChildren;

	// Token: 0x0400137F RID: 4991
	[SerializeField]
	private PersistentBoolItem persistent;

	// Token: 0x04001380 RID: 4992
	private bool isCut;

	// Token: 0x04001381 RID: 4993
	private Collider2D col;

	// Token: 0x04001382 RID: 4994
	private GrassCut[] children;

	// Token: 0x04001383 RID: 4995
	public UnityEvent OnCut;
}
