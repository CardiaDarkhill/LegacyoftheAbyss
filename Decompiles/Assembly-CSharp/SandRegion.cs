using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020000E3 RID: 227
public class SandRegion : DebugDrawColliderRuntimeAdder
{
	// Token: 0x0600071E RID: 1822 RVA: 0x000234C4 File Offset: 0x000216C4
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (this.breakCollisionBreakables)
		{
			SandRegionCollisionBreakable component = other.GetComponent<SandRegionCollisionBreakable>();
			if (component != null)
			{
				component.AddSandRegion(this);
			}
		}
		FsmBool inSandBool = SandRegion.GetInSandBool(other);
		if (inSandBool != null)
		{
			inSandBool.Value = true;
		}
	}

	// Token: 0x0600071F RID: 1823 RVA: 0x00023504 File Offset: 0x00021704
	private void OnTriggerExit2D(Collider2D other)
	{
		if (this.breakCollisionBreakables)
		{
			SandRegionCollisionBreakable component = other.GetComponent<SandRegionCollisionBreakable>();
			if (component != null)
			{
				component.RemoveSandRegion(this);
			}
		}
		FsmBool inSandBool = SandRegion.GetInSandBool(other);
		if (inSandBool != null)
		{
			inSandBool.Value = false;
		}
	}

	// Token: 0x06000720 RID: 1824 RVA: 0x00023544 File Offset: 0x00021744
	private static FsmBool GetInSandBool(Component other)
	{
		PlayMakerFSM component = other.GetComponent<PlayMakerFSM>();
		if (!component)
		{
			return null;
		}
		return component.FsmVariables.FindFsmBool("In Sand");
	}

	// Token: 0x06000721 RID: 1825 RVA: 0x00023572 File Offset: 0x00021772
	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.SandRegion, false);
	}

	// Token: 0x040006ED RID: 1773
	[SerializeField]
	private bool breakCollisionBreakables = true;
}
