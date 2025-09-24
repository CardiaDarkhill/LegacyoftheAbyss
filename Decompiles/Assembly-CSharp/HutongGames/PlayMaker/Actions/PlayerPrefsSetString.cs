using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FF8 RID: 4088
	[ActionCategory("PlayerPrefs")]
	[Tooltip("Sets the value of the preference identified by key. Lets you save a string that you can load later with {{PlayerPrefs Get String}}.")]
	public class PlayerPrefsSetString : FsmStateAction
	{
		// Token: 0x06007086 RID: 28806 RVA: 0x0022BF3F File Offset: 0x0022A13F
		public override void Reset()
		{
			this.keys = new FsmString[1];
			this.values = new FsmString[1];
		}

		// Token: 0x06007087 RID: 28807 RVA: 0x0022BF5C File Offset: 0x0022A15C
		public override void OnEnter()
		{
			for (int i = 0; i < this.keys.Length; i++)
			{
				if (!this.keys[i].IsNone || !this.keys[i].Value.Equals(""))
				{
					PlayerPrefs.SetString(this.keys[i].Value, this.values[i].IsNone ? "" : this.values[i].Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007057 RID: 28759
		[CompoundArray("Count", "Key", "Value")]
		[Tooltip("Case sensitive key.")]
		public FsmString[] keys;

		// Token: 0x04007058 RID: 28760
		[Tooltip("The value to save.")]
		public FsmString[] values;
	}
}
