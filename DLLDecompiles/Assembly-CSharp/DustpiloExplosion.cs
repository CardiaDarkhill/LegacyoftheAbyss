using System;
using UnityEngine;

// Token: 0x020005D6 RID: 1494
public sealed class DustpiloExplosion : MonoBehaviour
{
	// Token: 0x06003511 RID: 13585 RVA: 0x000EB7E0 File Offset: 0x000E99E0
	private void Awake()
	{
		this.fsm = base.GetComponent<PlayMakerFSM>();
	}

	// Token: 0x06003512 RID: 13586 RVA: 0x000EB7EE File Offset: 0x000E99EE
	private void Start()
	{
		this.started = true;
		this.RemoveOthers();
		DustpiloExplosion.activeExplosions.Add(this);
	}

	// Token: 0x06003513 RID: 13587 RVA: 0x000EB809 File Offset: 0x000E9A09
	private void OnEnable()
	{
		if (this.started)
		{
			this.RemoveOthers();
			DustpiloExplosion.activeExplosions.Add(this);
		}
	}

	// Token: 0x06003514 RID: 13588 RVA: 0x000EB825 File Offset: 0x000E9A25
	private void OnDisable()
	{
		DustpiloExplosion.activeExplosions.Remove(this);
	}

	// Token: 0x06003515 RID: 13589 RVA: 0x000EB834 File Offset: 0x000E9A34
	public void RemoveOthers()
	{
		float num = this.removalDistance * this.removalDistance;
		Vector2 b = base.transform.position;
		DustpiloExplosion.activeExplosions.ReserveListUsage();
		foreach (DustpiloExplosion dustpiloExplosion in DustpiloExplosion.activeExplosions.List)
		{
			if (Vector2.SqrMagnitude(dustpiloExplosion.transform.position - b) <= num)
			{
				dustpiloExplosion.End();
			}
		}
		DustpiloExplosion.activeExplosions.ReleaseListUsage();
	}

	// Token: 0x06003516 RID: 13590 RVA: 0x000EB8DC File Offset: 0x000E9ADC
	private void End()
	{
		if (this.fsm == null)
		{
			this.fsm = base.GetComponent<PlayMakerFSM>();
			if (this.fsm == null)
			{
				return;
			}
		}
		this.fsm.SendEvent("END");
	}

	// Token: 0x06003517 RID: 13591 RVA: 0x000EB917 File Offset: 0x000E9B17
	private void DrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.removalDistance);
	}

	// Token: 0x06003518 RID: 13592 RVA: 0x000EB939 File Offset: 0x000E9B39
	private void OnDrawGizmosSelected()
	{
		this.DrawGizmos();
	}

	// Token: 0x04003878 RID: 14456
	[SerializeField]
	private float removalDistance = 4f;

	// Token: 0x04003879 RID: 14457
	private static UniqueList<DustpiloExplosion> activeExplosions = new UniqueList<DustpiloExplosion>();

	// Token: 0x0400387A RID: 14458
	private PlayMakerFSM fsm;

	// Token: 0x0400387B RID: 14459
	private bool started;
}
