using System;
using UnityEngine;

// Token: 0x02000256 RID: 598
public sealed class ParticleEffectsScaleTarget : MonoBehaviour
{
	// Token: 0x1700024F RID: 591
	// (get) Token: 0x0600159C RID: 5532 RVA: 0x00061AD2 File Offset: 0x0005FCD2
	public Collider2D Target
	{
		get
		{
			this.Init();
			return this.target;
		}
	}

	// Token: 0x17000250 RID: 592
	// (get) Token: 0x0600159D RID: 5533 RVA: 0x00061AE0 File Offset: 0x0005FCE0
	// (set) Token: 0x0600159E RID: 5534 RVA: 0x00061AE8 File Offset: 0x0005FCE8
	public bool HasTarget { get; private set; }

	// Token: 0x0600159F RID: 5535 RVA: 0x00061AF1 File Offset: 0x0005FCF1
	private void OnValidate()
	{
		if (this.target == null)
		{
			this.target = base.GetComponent<Collider2D>();
		}
	}

	// Token: 0x060015A0 RID: 5536 RVA: 0x00061B10 File Offset: 0x0005FD10
	private void Init()
	{
		if (this.init)
		{
			return;
		}
		this.init = true;
		this.HasTarget = (this.target != null);
		if (!this.HasTarget)
		{
			this.target = base.GetComponent<Collider2D>();
			this.HasTarget = (this.target != null);
		}
	}

	// Token: 0x0400143F RID: 5183
	[SerializeField]
	private Collider2D target;

	// Token: 0x04001441 RID: 5185
	private bool init;
}
