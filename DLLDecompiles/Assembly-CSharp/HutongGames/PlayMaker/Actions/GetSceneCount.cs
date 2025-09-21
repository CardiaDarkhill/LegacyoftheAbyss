using System;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001040 RID: 4160
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Get the total number of currently loaded scenes.")]
	public class GetSceneCount : FsmStateAction
	{
		// Token: 0x060071FA RID: 29178 RVA: 0x00230972 File Offset: 0x0022EB72
		public override void Reset()
		{
			this.sceneCount = null;
			this.everyFrame = false;
		}

		// Token: 0x060071FB RID: 29179 RVA: 0x00230982 File Offset: 0x0022EB82
		public override void OnEnter()
		{
			this.DoGetSceneCount();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060071FC RID: 29180 RVA: 0x00230998 File Offset: 0x0022EB98
		public override void OnUpdate()
		{
			this.DoGetSceneCount();
		}

		// Token: 0x060071FD RID: 29181 RVA: 0x002309A0 File Offset: 0x0022EBA0
		private void DoGetSceneCount()
		{
			this.sceneCount.Value = SceneManager.sceneCount;
		}

		// Token: 0x040071BE RID: 29118
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The number of currently loaded scenes.")]
		public FsmInt sceneCount;

		// Token: 0x040071BF RID: 29119
		[Tooltip("Repeat every Frame")]
		public bool everyFrame;
	}
}
