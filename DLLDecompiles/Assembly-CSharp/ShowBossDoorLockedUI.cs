using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000395 RID: 917
[ActionCategory("Hollow Knight")]
public class ShowBossDoorLockedUI : FsmStateAction
{
	// Token: 0x06001EFE RID: 7934 RVA: 0x0008DA7E File Offset: 0x0008BC7E
	public override void Reset()
	{
		this.target = null;
		this.value = new FsmBool(true);
	}

	// Token: 0x06001EFF RID: 7935 RVA: 0x0008DA98 File Offset: 0x0008BC98
	public override void OnEnter()
	{
		GameObject safe = this.target.GetSafe(this);
		if (safe)
		{
			BossSequenceDoor component = safe.GetComponent<BossSequenceDoor>();
			if (component)
			{
				component.ShowLockUI(this.value.Value);
			}
		}
		base.Finish();
	}

	// Token: 0x04001DDB RID: 7643
	public FsmOwnerDefault target;

	// Token: 0x04001DDC RID: 7644
	public FsmBool value;
}
