using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000305 RID: 773
[ActionCategory("Hollow Knight")]
public class BehaviourListRemove : FsmStateAction
{
	// Token: 0x06001B87 RID: 7047 RVA: 0x00080919 File Offset: 0x0007EB19
	public override void Reset()
	{
		this.owner = new FsmOwnerDefault();
	}

	// Token: 0x06001B88 RID: 7048 RVA: 0x00080928 File Offset: 0x0007EB28
	public override void OnEnter()
	{
		GameObject safe = this.owner.GetSafe(this);
		if (safe)
		{
			LimitBehaviour component = safe.GetComponent<LimitBehaviour>();
			if (component)
			{
				component.RemoveSelf();
			}
		}
		base.Finish();
	}

	// Token: 0x04001A8A RID: 6794
	[RequiredField]
	public FsmOwnerDefault owner;
}
