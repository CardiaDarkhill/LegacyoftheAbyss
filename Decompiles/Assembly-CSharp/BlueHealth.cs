using System;
using UnityEngine;

// Token: 0x0200060C RID: 1548
public class BlueHealth : MonoBehaviour
{
	// Token: 0x17000658 RID: 1624
	// (get) Token: 0x06003750 RID: 14160 RVA: 0x000F4210 File Offset: 0x000F2410
	// (set) Token: 0x06003751 RID: 14161 RVA: 0x000F4217 File Offset: 0x000F2417
	public static int ActiveCount { get; private set; }

	// Token: 0x06003752 RID: 14162 RVA: 0x000F421F File Offset: 0x000F241F
	private void Reset()
	{
		this.controlFsm = base.GetComponent<PlayMakerFSM>();
	}

	// Token: 0x06003753 RID: 14163 RVA: 0x000F422D File Offset: 0x000F242D
	private void OnDisable()
	{
		this.RemovePoison();
		this.MarkInactive();
	}

	// Token: 0x06003754 RID: 14164 RVA: 0x000F423B File Offset: 0x000F243B
	public void MarkActive()
	{
		if (this.isActive)
		{
			return;
		}
		this.isActive = true;
		BlueHealth.ActiveCount++;
	}

	// Token: 0x06003755 RID: 14165 RVA: 0x000F4259 File Offset: 0x000F2459
	public void MarkInactive()
	{
		if (!this.isActive)
		{
			return;
		}
		this.isActive = false;
		BlueHealth.ActiveCount--;
	}

	// Token: 0x06003756 RID: 14166 RVA: 0x000F4278 File Offset: 0x000F2478
	public void CheckPoison()
	{
		if (this.setOnHc)
		{
			return;
		}
		if (!this.controlFsm.FsmVariables.FindFsmBool("Is Poison").Value)
		{
			return;
		}
		this.setOnHc = HeroController.instance;
		this.setOnHc.ReportPoisonHealthAdded();
	}

	// Token: 0x06003757 RID: 14167 RVA: 0x000F42C6 File Offset: 0x000F24C6
	public void RemovePoison()
	{
		if (!this.setOnHc)
		{
			return;
		}
		this.setOnHc.ReportPoisonHealthRemoved();
		this.setOnHc = null;
	}

	// Token: 0x04003A39 RID: 14905
	[SerializeField]
	private PlayMakerFSM controlFsm;

	// Token: 0x04003A3A RID: 14906
	private HeroController setOnHc;

	// Token: 0x04003A3B RID: 14907
	private bool isActive;
}
