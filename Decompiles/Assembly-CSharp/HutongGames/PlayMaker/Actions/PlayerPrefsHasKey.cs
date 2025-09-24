using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FF3 RID: 4083
	[ActionCategory("PlayerPrefs")]
	[Tooltip("Returns true if PlayerPref key exists in the preferences.")]
	public class PlayerPrefsHasKey : FsmStateAction
	{
		// Token: 0x06007077 RID: 28791 RVA: 0x0022BC34 File Offset: 0x00229E34
		public override void Reset()
		{
			this.key = "";
		}

		// Token: 0x06007078 RID: 28792 RVA: 0x0022BC48 File Offset: 0x00229E48
		public override void OnEnter()
		{
			base.Finish();
			if (!this.key.IsNone && !this.key.Value.Equals(""))
			{
				this.variable.Value = PlayerPrefs.HasKey(this.key.Value);
			}
			base.Fsm.Event(this.variable.Value ? this.trueEvent : this.falseEvent);
		}

		// Token: 0x0400704B RID: 28747
		[RequiredField]
		[Tooltip("The name of the PlayerPref to test for.")]
		public FsmString key;

		// Token: 0x0400704C RID: 28748
		[UIHint(UIHint.Variable)]
		[Title("Store Result")]
		[Tooltip("Store the result in a bool variable.")]
		public FsmBool variable;

		// Token: 0x0400704D RID: 28749
		[Tooltip("Event to send if the key exists.")]
		public FsmEvent trueEvent;

		// Token: 0x0400704E RID: 28750
		[Tooltip("Event to send if the key does not exist.")]
		public FsmEvent falseEvent;
	}
}
