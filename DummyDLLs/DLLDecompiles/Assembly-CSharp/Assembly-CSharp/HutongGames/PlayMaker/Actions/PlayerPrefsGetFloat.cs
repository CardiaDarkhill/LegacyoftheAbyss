using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FF0 RID: 4080
	[ActionCategory("PlayerPrefs")]
	[Tooltip("Returns the value corresponding to key in the preference file if it exists.")]
	public class PlayerPrefsGetFloat : FsmStateAction
	{
		// Token: 0x0600706E RID: 28782 RVA: 0x0022BA1D File Offset: 0x00229C1D
		public override void Reset()
		{
			this.keys = new FsmString[1];
			this.variables = new FsmFloat[1];
		}

		// Token: 0x0600706F RID: 28783 RVA: 0x0022BA38 File Offset: 0x00229C38
		public override void OnEnter()
		{
			for (int i = 0; i < this.keys.Length; i++)
			{
				if (!this.keys[i].IsNone || !this.keys[i].Value.Equals(""))
				{
					this.variables[i].Value = PlayerPrefs.GetFloat(this.keys[i].Value, this.variables[i].IsNone ? 0f : this.variables[i].Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007045 RID: 28741
		[CompoundArray("Count", "Key", "Variable")]
		[Tooltip("Case sensitive key.")]
		public FsmString[] keys;

		// Token: 0x04007046 RID: 28742
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the float in a Float Variable.")]
		public FsmFloat[] variables;
	}
}
