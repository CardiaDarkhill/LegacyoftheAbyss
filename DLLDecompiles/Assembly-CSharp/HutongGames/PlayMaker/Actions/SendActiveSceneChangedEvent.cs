using System;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001051 RID: 4177
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Send an event when the active scene has changed.")]
	public class SendActiveSceneChangedEvent : FsmStateAction
	{
		// Token: 0x06007246 RID: 29254 RVA: 0x00231CAB File Offset: 0x0022FEAB
		public override void Reset()
		{
			this.activeSceneChanged = null;
		}

		// Token: 0x06007247 RID: 29255 RVA: 0x00231CB4 File Offset: 0x0022FEB4
		public override void OnEnter()
		{
			SceneManager.activeSceneChanged += this.SceneManager_activeSceneChanged;
			base.Finish();
		}

		// Token: 0x06007248 RID: 29256 RVA: 0x00231CCD File Offset: 0x0022FECD
		private void SceneManager_activeSceneChanged(Scene previousActiveScene, Scene activeScene)
		{
			SendActiveSceneChangedEvent.lastNewActiveScene = activeScene;
			SendActiveSceneChangedEvent.lastPreviousActiveScene = previousActiveScene;
			base.Fsm.Event(this.activeSceneChanged);
			base.Finish();
		}

		// Token: 0x06007249 RID: 29257 RVA: 0x00231CF2 File Offset: 0x0022FEF2
		public override void OnExit()
		{
			SceneManager.activeSceneChanged -= this.SceneManager_activeSceneChanged;
		}

		// Token: 0x04007229 RID: 29225
		[RequiredField]
		[Tooltip("The event to send when an active scene changed")]
		public FsmEvent activeSceneChanged;

		// Token: 0x0400722A RID: 29226
		public static Scene lastPreviousActiveScene;

		// Token: 0x0400722B RID: 29227
		public static Scene lastNewActiveScene;
	}
}
