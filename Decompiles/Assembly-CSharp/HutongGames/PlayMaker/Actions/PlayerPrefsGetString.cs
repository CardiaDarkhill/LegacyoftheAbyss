using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FF2 RID: 4082
	[ActionCategory("PlayerPrefs")]
	[Tooltip("Returns the value corresponding to key in the preference file if it exists.")]
	public class PlayerPrefsGetString : FsmStateAction
	{
		// Token: 0x06007074 RID: 28788 RVA: 0x0022BB80 File Offset: 0x00229D80
		public override void Reset()
		{
			this.keys = new FsmString[1];
			this.variables = new FsmString[1];
		}

		// Token: 0x06007075 RID: 28789 RVA: 0x0022BB9C File Offset: 0x00229D9C
		public override void OnEnter()
		{
			for (int i = 0; i < this.keys.Length; i++)
			{
				if (!this.keys[i].IsNone || !this.keys[i].Value.Equals(""))
				{
					this.variables[i].Value = PlayerPrefs.GetString(this.keys[i].Value, this.variables[i].IsNone ? "" : this.variables[i].Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007049 RID: 28745
		[CompoundArray("Count", "Key", "Variable")]
		[Tooltip("Case sensitive key.")]
		public FsmString[] keys;

		// Token: 0x0400704A RID: 28746
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the string in a String Variable.")]
		public FsmString[] variables;
	}
}
