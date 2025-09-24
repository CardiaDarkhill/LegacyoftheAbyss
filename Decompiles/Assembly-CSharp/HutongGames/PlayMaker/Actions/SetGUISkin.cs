using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EDA RID: 3802
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("Sets the GUISkin used by GUI elements. Skins can be customized in the Unity editor. See unity docs: <a href=\"http://unity3d.com/support/documentation/Components/class-GUISkin.html\">GUISkin</a>.\n")]
	public class SetGUISkin : FsmStateAction
	{
		// Token: 0x06006B0B RID: 27403 RVA: 0x00215B92 File Offset: 0x00213D92
		public override void Reset()
		{
			this.skin = null;
			this.applyGlobally = true;
		}

		// Token: 0x06006B0C RID: 27404 RVA: 0x00215BA7 File Offset: 0x00213DA7
		public override void OnGUI()
		{
			if (this.skin != null)
			{
				GUI.skin = this.skin;
			}
			if (this.applyGlobally.Value)
			{
				PlayMakerGUI.GUISkin = this.skin;
				base.Finish();
			}
		}

		// Token: 0x04006A55 RID: 27221
		[RequiredField]
		[Tooltip("The skin to use.")]
		public GUISkin skin;

		// Token: 0x04006A56 RID: 27222
		[Tooltip("Apply this setting to all GUI calls, even in other scripts.")]
		public FsmBool applyGlobally;
	}
}
