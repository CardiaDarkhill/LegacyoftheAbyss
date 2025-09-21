using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FF7 RID: 4087
	[ActionCategory("PlayerPrefs")]
	[Tooltip("Sets the value of the preference identified by key. Lets you save an int value that you can load later with {{PlayerPrefs Get Int}}.")]
	public class PlayerPrefsSetInt : FsmStateAction
	{
		// Token: 0x06007083 RID: 28803 RVA: 0x0022BE9B File Offset: 0x0022A09B
		public override void Reset()
		{
			this.keys = new FsmString[1];
			this.values = new FsmInt[1];
		}

		// Token: 0x06007084 RID: 28804 RVA: 0x0022BEB8 File Offset: 0x0022A0B8
		public override void OnEnter()
		{
			for (int i = 0; i < this.keys.Length; i++)
			{
				if (!this.keys[i].IsNone || !this.keys[i].Value.Equals(""))
				{
					PlayerPrefs.SetInt(this.keys[i].Value, this.values[i].IsNone ? 0 : this.values[i].Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007055 RID: 28757
		[CompoundArray("Count", "Key", "Value")]
		[Tooltip("Case sensitive key.")]
		public FsmString[] keys;

		// Token: 0x04007056 RID: 28758
		[Tooltip("The value to save.")]
		public FsmInt[] values;
	}
}
