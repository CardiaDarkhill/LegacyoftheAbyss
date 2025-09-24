using System;
using UnityEngine;

// Token: 0x0200033F RID: 831
public sealed class ActivateOnDoubleJump : MonoBehaviour
{
	// Token: 0x06001CF5 RID: 7413 RVA: 0x00086868 File Offset: 0x00084A68
	private void Start()
	{
		if (this.target == null)
		{
			this.target = base.gameObject;
		}
		this.hasTarget = true;
		this.hc = HeroController.instance;
		if (this.hc != null)
		{
			this.hc.OnDoubleJumped += this.HcOnDoubleJumped;
		}
		if (this.deactivateOnStart)
		{
			this.target.SetActive(false);
		}
	}

	// Token: 0x06001CF6 RID: 7414 RVA: 0x000868DA File Offset: 0x00084ADA
	private void OnDestroy()
	{
		if (this.hc != null)
		{
			this.hc.OnDoubleJumped -= this.HcOnDoubleJumped;
		}
	}

	// Token: 0x06001CF7 RID: 7415 RVA: 0x00086901 File Offset: 0x00084B01
	private void HcOnDoubleJumped()
	{
		this.target.SetActive(true);
	}

	// Token: 0x04001C50 RID: 7248
	[SerializeField]
	private GameObject target;

	// Token: 0x04001C51 RID: 7249
	[SerializeField]
	private bool deactivateOnStart;

	// Token: 0x04001C52 RID: 7250
	private HeroController hc;

	// Token: 0x04001C53 RID: 7251
	private bool hasTarget;
}
