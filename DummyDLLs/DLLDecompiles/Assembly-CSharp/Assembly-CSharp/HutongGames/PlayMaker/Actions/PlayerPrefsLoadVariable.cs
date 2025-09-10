using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FF4 RID: 4084
	[ActionCategory("PlayerPrefs")]
	[Tooltip("Load variable value saved with {{PlayerPrefs Save Variable}}. The Key should be a unique identifier for the variable.\nNOTE: You cannot save references to Scene Objects in PlayerPrefs!")]
	public class PlayerPrefsLoadVariable : FsmStateAction
	{
		// Token: 0x0600707A RID: 28794 RVA: 0x0022BCC8 File Offset: 0x00229EC8
		public override void Reset()
		{
			this.key = null;
			this.variable = null;
		}

		// Token: 0x0600707B RID: 28795 RVA: 0x0022BCD8 File Offset: 0x00229ED8
		public override void OnEnter()
		{
			if (!FsmString.IsNullOrEmpty(this.key) && !this.variable.IsNone)
			{
				string @string = PlayerPrefs.GetString(this.key.Value, "");
				if (@string == "")
				{
					base.Finish();
					return;
				}
				FsmVar fsmVar = JsonUtility.FromJson<FsmVar>(@string);
				if (fsmVar.Type == this.variable.Type && fsmVar.ObjectType == this.variable.ObjectType)
				{
					fsmVar.ApplyValueTo(this.variable.NamedVar);
				}
				this.variable.NamedVar.Init();
			}
			base.Finish();
		}

		// Token: 0x0400704F RID: 28751
		[Tooltip("Case sensitive key.")]
		public FsmString key;

		// Token: 0x04007050 RID: 28752
		[UIHint(UIHint.Variable)]
		[Tooltip("The variable to load.")]
		public FsmVar variable;
	}
}
