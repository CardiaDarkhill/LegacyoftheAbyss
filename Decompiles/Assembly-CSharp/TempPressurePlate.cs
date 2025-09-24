using System;

// Token: 0x0200056D RID: 1389
public class TempPressurePlate : PressurePlateBase
{
	// Token: 0x1400009C RID: 156
	// (add) Token: 0x060031A2 RID: 12706 RVA: 0x000DC648 File Offset: 0x000DA848
	// (remove) Token: 0x060031A3 RID: 12707 RVA: 0x000DC680 File Offset: 0x000DA880
	public event Action PreActivated;

	// Token: 0x1400009D RID: 157
	// (add) Token: 0x060031A4 RID: 12708 RVA: 0x000DC6B8 File Offset: 0x000DA8B8
	// (remove) Token: 0x060031A5 RID: 12709 RVA: 0x000DC6F0 File Offset: 0x000DA8F0
	public event Action Activated;

	// Token: 0x17000561 RID: 1377
	// (get) Token: 0x060031A6 RID: 12710 RVA: 0x000DC725 File Offset: 0x000DA925
	protected override bool CanDepress
	{
		get
		{
			return !this.isActivated;
		}
	}

	// Token: 0x060031A7 RID: 12711 RVA: 0x000DC730 File Offset: 0x000DA930
	protected override void PreActivate()
	{
		if (this.isActivated)
		{
			return;
		}
		Action preActivated = this.PreActivated;
		if (preActivated == null)
		{
			return;
		}
		preActivated();
	}

	// Token: 0x060031A8 RID: 12712 RVA: 0x000DC74B File Offset: 0x000DA94B
	protected override void Activate()
	{
		if (this.isActivated)
		{
			return;
		}
		this.isActivated = true;
		Action activated = this.Activated;
		if (activated == null)
		{
			return;
		}
		activated();
	}

	// Token: 0x060031A9 RID: 12713 RVA: 0x000DC76D File Offset: 0x000DA96D
	public void ActivateSilent()
	{
		if (this.isActivated)
		{
			return;
		}
		this.isActivated = true;
		base.StartDrop(true);
	}

	// Token: 0x060031AA RID: 12714 RVA: 0x000DC786 File Offset: 0x000DA986
	protected override void Raised()
	{
		this.isActivated = false;
	}

	// Token: 0x060031AB RID: 12715 RVA: 0x000DC78F File Offset: 0x000DA98F
	public void Deactivate()
	{
		this.Deactivate(0f);
	}

	// Token: 0x060031AC RID: 12716 RVA: 0x000DC79C File Offset: 0x000DA99C
	public void DeactivateSilent()
	{
		if (!this.isActivated)
		{
			return;
		}
		this.isActivated = false;
		base.StartRaiseSilent(0f);
	}

	// Token: 0x060031AD RID: 12717 RVA: 0x000DC7B9 File Offset: 0x000DA9B9
	public void Deactivate(float raiseDelay)
	{
		if (!this.isActivated)
		{
			return;
		}
		this.isActivated = false;
		base.StartRaise(raiseDelay);
	}

	// Token: 0x04003514 RID: 13588
	private bool isActivated;
}
