using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001053 RID: 4179
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Send an event when a scene was unloaded.")]
	public class SendSceneUnloadedEvent : FsmStateAction
	{
		// Token: 0x06007252 RID: 29266 RVA: 0x00231DDD File Offset: 0x0022FFDD
		public override void Reset()
		{
			this.sceneUnloaded = null;
		}

		// Token: 0x06007253 RID: 29267 RVA: 0x00231DE6 File Offset: 0x0022FFE6
		public override void OnEnter()
		{
			SceneManager.sceneUnloaded += this.SceneManager_sceneUnloaded;
			base.Finish();
		}

		// Token: 0x06007254 RID: 29268 RVA: 0x00231DFF File Offset: 0x0022FFFF
		private void SceneManager_sceneUnloaded(Scene scene)
		{
			Debug.Log(scene.name);
			SendSceneUnloadedEvent.lastUnLoadedScene = scene;
			base.Fsm.Event(this.sceneUnloaded);
			base.Finish();
		}

		// Token: 0x06007255 RID: 29269 RVA: 0x00231E2A File Offset: 0x0023002A
		public override void OnExit()
		{
			SceneManager.sceneUnloaded -= this.SceneManager_sceneUnloaded;
		}

		// Token: 0x04007231 RID: 29233
		[RequiredField]
		[Tooltip("The event to send when scene was unloaded")]
		public FsmEvent sceneUnloaded;

		// Token: 0x04007232 RID: 29234
		public static Scene lastUnLoadedScene;
	}
}
