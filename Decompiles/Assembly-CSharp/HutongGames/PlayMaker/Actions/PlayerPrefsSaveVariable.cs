using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FF5 RID: 4085
	[ActionCategory("PlayerPrefs")]
	[Tooltip("Save a variable value in PlayerPrefs. You can load the value later with {{PlayerPrefs Load Variable}}.\nNOTE: You cannot save references to Scene Objects in PlayerPrefs!")]
	public class PlayerPrefsSaveVariable : FsmStateAction
	{
		// Token: 0x0600707D RID: 28797 RVA: 0x0022BD8D File Offset: 0x00229F8D
		public override void Reset()
		{
			this.key = null;
			this.variable = null;
		}

		// Token: 0x0600707E RID: 28798 RVA: 0x0022BDA0 File Offset: 0x00229FA0
		public override void OnEnter()
		{
			if (!FsmString.IsNullOrEmpty(this.key))
			{
				this.variable.UpdateValue();
				string value = JsonUtility.ToJson(this.variable);
				PlayerPrefs.SetString(this.key.Value, value);
				PlayerPrefs.Save();
			}
			base.Finish();
		}

		// Token: 0x04007051 RID: 28753
		[Tooltip("Case sensitive key.")]
		public FsmString key;

		// Token: 0x04007052 RID: 28754
		[UIHint(UIHint.Variable)]
		[Tooltip("The variable to save.")]
		public FsmVar variable;
	}
}
