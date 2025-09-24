using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000304 RID: 772
[ActionCategory("Hollow Knight")]
public class BehaviourListAdd : FsmStateAction
{
	// Token: 0x06001B84 RID: 7044 RVA: 0x000808C7 File Offset: 0x0007EAC7
	public override void Reset()
	{
		this.owner = new FsmOwnerDefault();
	}

	// Token: 0x06001B85 RID: 7045 RVA: 0x000808D4 File Offset: 0x0007EAD4
	public override void OnEnter()
	{
		GameObject safe = this.owner.GetSafe(this);
		if (safe)
		{
			LimitBehaviour component = safe.GetComponent<LimitBehaviour>();
			if (component)
			{
				component.Add();
			}
		}
		base.Finish();
	}

	// Token: 0x04001A89 RID: 6793
	[RequiredField]
	public FsmOwnerDefault owner;
}
