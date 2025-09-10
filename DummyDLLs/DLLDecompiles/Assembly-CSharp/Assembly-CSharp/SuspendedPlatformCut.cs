using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200056A RID: 1386
public class SuspendedPlatformCut : MonoBehaviour, IHitResponder
{
	// Token: 0x0600318C RID: 12684 RVA: 0x000DBF6A File Offset: 0x000DA16A
	private void OnValidate()
	{
		if (this.cutsToBreak < 1)
		{
			this.cutsToBreak = 1;
		}
		if (this.sprites)
		{
			this.disableGameObjects = new GameObject[]
			{
				this.sprites
			};
			this.sprites = null;
		}
	}

	// Token: 0x0600318D RID: 12685 RVA: 0x000DBFA5 File Offset: 0x000DA1A5
	private void Awake()
	{
		this.OnValidate();
		this.platform = base.GetComponentInParent<SuspendedPlatformBase>();
		this.audioSource = base.GetComponentInParent<AudioSource>();
		this.col = base.GetComponent<Collider2D>();
	}

	// Token: 0x0600318E RID: 12686 RVA: 0x000DBFD1 File Offset: 0x000DA1D1
	private void OnEnable()
	{
		this.cutsLeft = this.cutsToBreak;
		if (this.cutPointParticles)
		{
			this.cutPointParticles.SetActive(false);
		}
		if (this.cutParticles)
		{
			this.cutParticles.SetActive(false);
		}
	}

	// Token: 0x0600318F RID: 12687 RVA: 0x000DC014 File Offset: 0x000DA214
	public void Cut()
	{
		this.activated = true;
		if (this.cutParticles)
		{
			this.cutParticles.SetActive(true);
		}
		if (this.audioSource && this.finalCutSound)
		{
			this.audioSource.PlayOneShot(this.finalCutSound);
		}
		this.platform.CutDown();
		this.onBreak.Invoke();
		this.Disable();
	}

	// Token: 0x06003190 RID: 12688 RVA: 0x000DC088 File Offset: 0x000DA288
	public void Disable()
	{
		this.disableGameObjects.SetAllActive(false);
	}

	// Token: 0x06003191 RID: 12689 RVA: 0x000DC098 File Offset: 0x000DA298
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (this.activated)
		{
			return IHitResponder.Response.None;
		}
		if (this.canCutTrigger && !this.canCutTrigger.IsInside)
		{
			return IHitResponder.Response.None;
		}
		AttackTypes attackType = damageInstance.AttackType;
		if (attackType != AttackTypes.Nail && attackType != AttackTypes.Heavy && !damageInstance.IsNailTag)
		{
			return IHitResponder.Response.None;
		}
		Vector3 position = damageInstance.Source.transform.position;
		if (position.y < this.col.bounds.min.y || position.y > this.col.bounds.max.y)
		{
			return IHitResponder.Response.None;
		}
		Vector3 position2;
		if (this.cutPointParticles)
		{
			position2 = this.cutPointParticles.transform.position;
			position2.y = position.y;
			if (this.cutPointParticles)
			{
				this.cutPointParticles.SetActive(false);
				this.cutPointParticles.transform.position = position2;
				this.cutPointParticles.SetActive(true);
			}
		}
		else
		{
			position2 = base.transform.position;
			position2.y = position.y;
		}
		if (this.cutEffectPrefab)
		{
			this.cutEffectPrefab.Spawn(position2);
		}
		this.onHit.Invoke();
		if (this.audioSource && this.cutSound)
		{
			this.audioSource.PlayOneShot(this.cutSound);
		}
		this.cutsLeft--;
		if (this.cutsLeft <= 0)
		{
			this.Cut();
		}
		return IHitResponder.Response.GenericHit;
	}

	// Token: 0x040034E2 RID: 13538
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private GameObject sprites;

	// Token: 0x040034E3 RID: 13539
	[SerializeField]
	private GameObject[] disableGameObjects;

	// Token: 0x040034E4 RID: 13540
	[Space]
	[SerializeField]
	private GameObject cutParticles;

	// Token: 0x040034E5 RID: 13541
	[SerializeField]
	private GameObject cutPointParticles;

	// Token: 0x040034E6 RID: 13542
	[SerializeField]
	private GameObject cutEffectPrefab;

	// Token: 0x040034E7 RID: 13543
	[Space]
	[SerializeField]
	private AudioClip cutSound;

	// Token: 0x040034E8 RID: 13544
	[SerializeField]
	private AudioClip finalCutSound;

	// Token: 0x040034E9 RID: 13545
	[Space]
	[SerializeField]
	private TrackTriggerObjects canCutTrigger;

	// Token: 0x040034EA RID: 13546
	[SerializeField]
	private int cutsToBreak = 1;

	// Token: 0x040034EB RID: 13547
	[Space]
	[SerializeField]
	private UnityEvent onHit;

	// Token: 0x040034EC RID: 13548
	[SerializeField]
	private UnityEvent onBreak;

	// Token: 0x040034ED RID: 13549
	private bool activated;

	// Token: 0x040034EE RID: 13550
	private int cutsLeft;

	// Token: 0x040034EF RID: 13551
	private SuspendedPlatformBase platform;

	// Token: 0x040034F0 RID: 13552
	private AudioSource audioSource;

	// Token: 0x040034F1 RID: 13553
	private Collider2D col;
}
