using System;
using UnityEngine;

// Token: 0x020000F1 RID: 241
public class WalkArea : MonoBehaviour
{
	// Token: 0x060007A3 RID: 1955 RVA: 0x00024F47 File Offset: 0x00023147
	protected void Awake()
	{
		this.myCollider = base.GetComponent<Collider2D>();
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x00024F55 File Offset: 0x00023155
	private void Start()
	{
		this.gm = GameManager.instance;
		this.gm.UnloadingLevel += this.Deactivate;
	}

	// Token: 0x060007A5 RID: 1957 RVA: 0x00024F79 File Offset: 0x00023179
	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		if (otherCollider.gameObject.layer != 9)
		{
			return;
		}
		this.activated = true;
		HeroController.instance.SetWalkZone(true);
	}

	// Token: 0x060007A6 RID: 1958 RVA: 0x00024F9D File Offset: 0x0002319D
	private void OnTriggerStay2D(Collider2D otherCollider)
	{
		if (this.activated || !this.myCollider.enabled)
		{
			return;
		}
		if (otherCollider.gameObject.layer != 9)
		{
			return;
		}
		this.activated = true;
		HeroController.instance.SetWalkZone(true);
	}

	// Token: 0x060007A7 RID: 1959 RVA: 0x00024FD7 File Offset: 0x000231D7
	private void OnTriggerExit2D(Collider2D otherCollider)
	{
		if (otherCollider.gameObject.layer != 9)
		{
			return;
		}
		this.activated = false;
		HeroController.instance.SetWalkZone(false);
	}

	// Token: 0x060007A8 RID: 1960 RVA: 0x00024FFB File Offset: 0x000231FB
	private void Deactivate()
	{
		this.activated = false;
		HeroController.instance.SetWalkZone(false);
	}

	// Token: 0x060007A9 RID: 1961 RVA: 0x00025010 File Offset: 0x00023210
	private void OnDisable()
	{
		if (this.gm != null)
		{
			this.gm.UnloadingLevel -= this.Deactivate;
		}
		if (!this.activated)
		{
			return;
		}
		this.activated = false;
		HeroController silentInstance = HeroController.SilentInstance;
		if (silentInstance)
		{
			silentInstance.SetWalkZone(false);
		}
	}

	// Token: 0x04000774 RID: 1908
	private Collider2D myCollider;

	// Token: 0x04000775 RID: 1909
	private bool activated;

	// Token: 0x04000776 RID: 1910
	private GameManager gm;
}
