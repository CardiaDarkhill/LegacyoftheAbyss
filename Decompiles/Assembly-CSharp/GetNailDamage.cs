using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000376 RID: 886
public class GetNailDamage : FsmStateAction
{
	// Token: 0x06001E51 RID: 7761 RVA: 0x0008BB3E File Offset: 0x00089D3E
	public override void Reset()
	{
		this.storeValue = null;
	}

	// Token: 0x06001E52 RID: 7762 RVA: 0x0008BB48 File Offset: 0x00089D48
	public override void OnEnter()
	{
		if (!this.storeValue.IsNone)
		{
			if (BossSequenceController.BoundNail)
			{
				this.storeValue.Value = Mathf.Min(GameManager.instance.playerData.nailDamage, BossSequenceController.BoundNailDamage);
			}
			else
			{
				this.storeValue.Value = GameManager.instance.playerData.nailDamage;
			}
		}
		base.Finish();
	}

	// Token: 0x04001D57 RID: 7511
	[UIHint(UIHint.Variable)]
	public FsmInt storeValue;
}
