using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FF1 RID: 4081
	[ActionCategory("PlayerPrefs")]
	[Tooltip("Returns the value corresponding to key in the preference file if it exists.")]
	public class PlayerPrefsGetInt : FsmStateAction
	{
		// Token: 0x06007071 RID: 28785 RVA: 0x0022BAD0 File Offset: 0x00229CD0
		public override void Reset()
		{
			this.keys = new FsmString[1];
			this.variables = new FsmInt[1];
		}

		// Token: 0x06007072 RID: 28786 RVA: 0x0022BAEC File Offset: 0x00229CEC
		public override void OnEnter()
		{
			for (int i = 0; i < this.keys.Length; i++)
			{
				if (!this.keys[i].IsNone || !this.keys[i].Value.Equals(""))
				{
					this.variables[i].Value = PlayerPrefs.GetInt(this.keys[i].Value, this.variables[i].IsNone ? 0 : this.variables[i].Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007047 RID: 28743
		[CompoundArray("Count", "Key", "Variable")]
		[Tooltip("Case sensitive key.")]
		public FsmString[] keys;

		// Token: 0x04007048 RID: 28744
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the integer in an Int Variable.")]
		public FsmInt[] variables;
	}
}
