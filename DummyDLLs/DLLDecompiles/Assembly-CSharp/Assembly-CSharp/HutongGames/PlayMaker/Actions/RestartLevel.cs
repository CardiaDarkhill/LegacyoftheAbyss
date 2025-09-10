using System;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F35 RID: 3893
	[ActionCategory(ActionCategory.Level)]
	[Note("Reloads the current scene.")]
	[Tooltip("Reloads the current scene.")]
	public class RestartLevel : FsmStateAction
	{
		// Token: 0x06006C79 RID: 27769 RVA: 0x0021DE7C File Offset: 0x0021C07C
		public override void OnEnter()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
			base.Finish();
		}
	}
}
