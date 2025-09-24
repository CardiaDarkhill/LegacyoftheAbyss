using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000488 RID: 1160
public class ActivatorPlatform : AnimatorActivatingStates, IHitResponder
{
	// Token: 0x060029EB RID: 10731 RVA: 0x000B6004 File Offset: 0x000B4204
	protected override void Awake()
	{
		base.Awake();
		this.breakAnimHashCache = new AnimatorHashCache(this.breakAnim);
		this.collider = base.GetComponent<Collider2D>();
		this.hasCollider = (this.collider != null);
		if (this.hasCollider)
		{
			this.collider.enabled = false;
		}
	}

	// Token: 0x060029EC RID: 10732 RVA: 0x000B605C File Offset: 0x000B425C
	protected override void Start()
	{
		base.Start();
		if (this.GetUpDirection() != 1)
		{
			if (this.tiltPlatFsm)
			{
				this.tiltPlatFsm.enabled = false;
			}
			if (this.tiltPlatScript)
			{
				this.tiltPlatScript.enabled = false;
			}
		}
	}

	// Token: 0x060029ED RID: 10733 RVA: 0x000B60AA File Offset: 0x000B42AA
	protected override void OnValidate()
	{
		base.OnValidate();
		this.breakAnimHashCache = new AnimatorHashCache(this.breakAnim);
	}

	// Token: 0x060029EE RID: 10734 RVA: 0x000B60C4 File Offset: 0x000B42C4
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (this.isBroken)
		{
			return IHitResponder.Response.None;
		}
		HitInstance.HitDirection actualHitDirection = damageInstance.GetActualHitDirection(base.transform, HitInstance.TargetType.Regular);
		if (this.GetHitDirBitmask().IsBitSet((int)actualHitDirection))
		{
			return IHitResponder.Response.None;
		}
		this.hitCamShake.DoShake(this, true);
		this.OnHit.Invoke();
		if (this.hitEffectPrefab)
		{
			float overriddenDirection = damageInstance.GetOverriddenDirection(base.transform, HitInstance.TargetType.Regular);
			this.hitEffectPrefab.Spawn(base.transform.position).transform.SetRotation2D(Helper.GetReflectedAngle(overriddenDirection, true, false, false) + 180f);
		}
		this.hits++;
		if (this.hitsToBreak <= 0 || this.hits < this.hitsToBreak)
		{
			return IHitResponder.Response.GenericHit;
		}
		this.isBroken = true;
		base.SetActive(false, true);
		base.PlayAnim(this.breakAnimHashCache.Hash, false);
		this.breakCamShake.DoShake(this, true);
		this.OnBreak.Invoke();
		return IHitResponder.Response.GenericHit;
	}

	// Token: 0x060029EF RID: 10735 RVA: 0x000B61D2 File Offset: 0x000B43D2
	protected override void OnActivate()
	{
		this.hits = 0;
		this.isBroken = false;
		if (this.hasCollider)
		{
			this.collider.enabled = true;
		}
	}

	// Token: 0x060029F0 RID: 10736 RVA: 0x000B61F6 File Offset: 0x000B43F6
	protected override void OnDeactivate()
	{
		if (this.hasCollider)
		{
			this.collider.enabled = false;
		}
	}

	// Token: 0x060029F1 RID: 10737 RVA: 0x000B620C File Offset: 0x000B440C
	private int GetUpDirection()
	{
		Vector3 v = base.transform.TransformVector(Vector3.up);
		float num;
		for (num = Vector2.SignedAngle(Vector2.right, v); num < 0f; num += 360f)
		{
		}
		if (num < 45f)
		{
			return 0;
		}
		if (num < 135f)
		{
			return 1;
		}
		if (num < 225f)
		{
			return 2;
		}
		return 3;
	}

	// Token: 0x060029F2 RID: 10738 RVA: 0x000B626C File Offset: 0x000B446C
	private int GetHitDirBitmask()
	{
		switch (this.GetUpDirection())
		{
		case 0:
			return 0.SetBitAtIndex(0);
		case 1:
			return 0.SetBitAtIndex(3);
		case 2:
			return 0.SetBitAtIndex(1);
		case 3:
			return 0.SetBitAtIndex(2);
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x04002A5C RID: 10844
	[Space]
	[SerializeField]
	private PlayMakerFSM tiltPlatFsm;

	// Token: 0x04002A5D RID: 10845
	[SerializeField]
	private TiltPlat tiltPlatScript;

	// Token: 0x04002A5E RID: 10846
	[Space]
	[SerializeField]
	private int hitsToBreak;

	// Token: 0x04002A5F RID: 10847
	[SerializeField]
	private string breakAnim;

	// Token: 0x04002A60 RID: 10848
	[SerializeField]
	private GameObject hitEffectPrefab;

	// Token: 0x04002A61 RID: 10849
	[SerializeField]
	private CameraShakeTarget hitCamShake;

	// Token: 0x04002A62 RID: 10850
	[SerializeField]
	private CameraShakeTarget breakCamShake;

	// Token: 0x04002A63 RID: 10851
	[Space]
	public UnityEvent OnHit;

	// Token: 0x04002A64 RID: 10852
	public UnityEvent OnBreak;

	// Token: 0x04002A65 RID: 10853
	private int hits;

	// Token: 0x04002A66 RID: 10854
	private bool isBroken;

	// Token: 0x04002A67 RID: 10855
	private bool hasCollider;

	// Token: 0x04002A68 RID: 10856
	private Collider2D collider;

	// Token: 0x04002A69 RID: 10857
	private AnimatorHashCache breakAnimHashCache;
}
