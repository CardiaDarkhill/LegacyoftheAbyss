using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FF6 RID: 4086
	[ActionCategory("PlayerPrefs")]
	[Tooltip("Sets the value of the preference identified by key. Lets you save a float value that you can load later with {{PlayerPrefs Get Float}}.")]
	public class PlayerPrefsSetFloat : FsmStateAction
	{
		// Token: 0x06007080 RID: 28800 RVA: 0x0022BDF5 File Offset: 0x00229FF5
		public override void Reset()
		{
			this.keys = new FsmString[1];
			this.values = new FsmFloat[1];
		}

		// Token: 0x06007081 RID: 28801 RVA: 0x0022BE10 File Offset: 0x0022A010
		public override void OnEnter()
		{
			for (int i = 0; i < this.keys.Length; i++)
			{
				if (!this.keys[i].IsNone || !this.keys[i].Value.Equals(""))
				{
					PlayerPrefs.SetFloat(this.keys[i].Value, this.values[i].IsNone ? 0f : this.values[i].Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007053 RID: 28755
		[CompoundArray("Count", "Key", "Value")]
		[Tooltip("Case sensitive key.")]
		public FsmString[] keys;

		// Token: 0x04007054 RID: 28756
		[Tooltip("The value to save.")]
		public FsmFloat[] values;
	}
}
