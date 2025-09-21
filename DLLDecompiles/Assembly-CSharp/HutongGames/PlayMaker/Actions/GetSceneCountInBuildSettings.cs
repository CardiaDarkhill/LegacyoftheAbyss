using System;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001041 RID: 4161
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Get the number of scenes in Build Settings.")]
	public class GetSceneCountInBuildSettings : FsmStateAction
	{
		// Token: 0x060071FF RID: 29183 RVA: 0x002309BA File Offset: 0x0022EBBA
		public override void Reset()
		{
			this.sceneCountInBuildSettings = null;
		}

		// Token: 0x06007200 RID: 29184 RVA: 0x002309C3 File Offset: 0x0022EBC3
		public override void OnEnter()
		{
			this.DoGetSceneCountInBuildSettings();
			base.Finish();
		}

		// Token: 0x06007201 RID: 29185 RVA: 0x002309D1 File Offset: 0x0022EBD1
		private void DoGetSceneCountInBuildSettings()
		{
			this.sceneCountInBuildSettings.Value = SceneManager.sceneCountInBuildSettings;
		}

		// Token: 0x040071C0 RID: 29120
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The number of scenes in Build Settings.")]
		public FsmInt sceneCountInBuildSettings;
	}
}
