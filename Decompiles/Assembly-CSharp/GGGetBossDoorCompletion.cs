using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000397 RID: 919
[ActionCategory("Hollow Knight")]
public class GGGetBossDoorCompletion : FsmStateAction
{
	// Token: 0x06001F03 RID: 7939 RVA: 0x0008DB59 File Offset: 0x0008BD59
	public override void Reset()
	{
		this.unlocked = null;
		this.completed = null;
		this.allBindings = null;
		this.noHits = null;
		this.boundNail = null;
		this.boundShell = null;
		this.boundCharms = null;
		this.boundSoul = null;
	}

	// Token: 0x06001F04 RID: 7940 RVA: 0x0008DB94 File Offset: 0x0008BD94
	public override void OnEnter()
	{
		if (!string.IsNullOrEmpty(this.playerDataVariable.Value))
		{
			BossSequenceDoor.Completion completion = GameManager.instance.GetPlayerDataVariable<BossSequenceDoor.Completion>(this.playerDataVariable.Value);
			this.unlocked.Value = completion.unlocked;
			this.completed.Value = completion.completed;
			this.allBindings.Value = completion.allBindings;
			this.noHits.Value = completion.noHits;
			this.boundNail.Value = completion.boundNail;
			this.boundShell.Value = completion.boundShell;
			this.boundCharms.Value = completion.boundCharms;
			this.boundSoul.Value = completion.boundSoul;
		}
		base.Finish();
	}

	// Token: 0x04001DDD RID: 7645
	public FsmString playerDataVariable;

	// Token: 0x04001DDE RID: 7646
	[Space]
	[UIHint(UIHint.Variable)]
	public FsmBool unlocked;

	// Token: 0x04001DDF RID: 7647
	[UIHint(UIHint.Variable)]
	public FsmBool completed;

	// Token: 0x04001DE0 RID: 7648
	[UIHint(UIHint.Variable)]
	public FsmBool allBindings;

	// Token: 0x04001DE1 RID: 7649
	[UIHint(UIHint.Variable)]
	public FsmBool noHits;

	// Token: 0x04001DE2 RID: 7650
	[Space]
	[UIHint(UIHint.Variable)]
	public FsmBool boundNail;

	// Token: 0x04001DE3 RID: 7651
	[UIHint(UIHint.Variable)]
	public FsmBool boundShell;

	// Token: 0x04001DE4 RID: 7652
	[UIHint(UIHint.Variable)]
	public FsmBool boundCharms;

	// Token: 0x04001DE5 RID: 7653
	[UIHint(UIHint.Variable)]
	public FsmBool boundSoul;
}
