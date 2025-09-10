using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FEF RID: 4079
	[ActionCategory("PlayerPrefs")]
	[Tooltip("Removes key and its corresponding value from the preferences.")]
	public class PlayerPrefsDeleteKey : FsmStateAction
	{
		// Token: 0x0600706B RID: 28779 RVA: 0x0022B9C7 File Offset: 0x00229BC7
		public override void Reset()
		{
			this.key = "";
		}

		// Token: 0x0600706C RID: 28780 RVA: 0x0022B9D9 File Offset: 0x00229BD9
		public override void OnEnter()
		{
			if (!this.key.IsNone && !this.key.Value.Equals(""))
			{
				PlayerPrefs.DeleteKey(this.key.Value);
			}
			base.Finish();
		}

		// Token: 0x04007044 RID: 28740
		[Tooltip("The name of the PlayerPref.")]
		public FsmString key;
	}
}
