using System;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200103D RID: 4157
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Create an empty new scene with the given name additively. The path of the new scene will be empty")]
	public class CreateScene : FsmStateAction
	{
		// Token: 0x060071EE RID: 29166 RVA: 0x002305B6 File Offset: 0x0022E7B6
		public override void Reset()
		{
			this.sceneName = null;
		}

		// Token: 0x060071EF RID: 29167 RVA: 0x002305BF File Offset: 0x0022E7BF
		public override void OnEnter()
		{
			SceneManager.CreateScene(this.sceneName.Value);
			base.Finish();
		}

		// Token: 0x040071AB RID: 29099
		[RequiredField]
		[Tooltip("The name of the new scene. It cannot be empty or null, or same as the name of the existing scenes.")]
		public FsmString sceneName;
	}
}
