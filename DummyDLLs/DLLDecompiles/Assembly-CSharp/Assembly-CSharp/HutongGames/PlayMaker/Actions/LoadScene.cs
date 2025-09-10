using System;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200104D RID: 4173
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Loads the scene by its name or index in Build Settings. ")]
	public class LoadScene : FsmStateAction
	{
		// Token: 0x06007233 RID: 29235 RVA: 0x00231458 File Offset: 0x0022F658
		public override void Reset()
		{
			this.sceneReference = GetSceneActionBase.SceneSimpleReferenceOptions.SceneAtIndex;
			this.sceneByName = null;
			this.sceneAtIndex = null;
			this.loadSceneMode = null;
			this.success = null;
			this.successEvent = null;
			this.failureEvent = null;
		}

		// Token: 0x06007234 RID: 29236 RVA: 0x0023148C File Offset: 0x0022F68C
		public override void OnEnter()
		{
			bool flag = this.DoLoadScene();
			if (!this.success.IsNone)
			{
				this.success.Value = flag;
			}
			if (flag)
			{
				base.Fsm.Event(this.successEvent);
			}
			else
			{
				base.Fsm.Event(this.failureEvent);
			}
			base.Finish();
		}

		// Token: 0x06007235 RID: 29237 RVA: 0x002314E8 File Offset: 0x0022F6E8
		private bool DoLoadScene()
		{
			if (this.sceneReference == GetSceneActionBase.SceneSimpleReferenceOptions.SceneAtIndex)
			{
				if (SceneManager.GetActiveScene().buildIndex == this.sceneAtIndex.Value)
				{
					return false;
				}
				SceneManager.LoadScene(this.sceneAtIndex.Value, (LoadSceneMode)this.loadSceneMode.Value);
			}
			else
			{
				if (SceneManager.GetActiveScene().name == this.sceneByName.Value)
				{
					return false;
				}
				SceneManager.LoadScene(this.sceneByName.Value, (LoadSceneMode)this.loadSceneMode.Value);
			}
			return true;
		}

		// Token: 0x040071F9 RID: 29177
		[Tooltip("The reference options of the Scene")]
		public GetSceneActionBase.SceneSimpleReferenceOptions sceneReference;

		// Token: 0x040071FA RID: 29178
		[Tooltip("The name of the scene to load. The given sceneName can either be the last part of the path, without .unity extension or the full path still without the .unity extension")]
		public FsmString sceneByName;

		// Token: 0x040071FB RID: 29179
		[Tooltip("The index of the scene to load.")]
		public FsmInt sceneAtIndex;

		// Token: 0x040071FC RID: 29180
		[Tooltip("Allows you to specify whether or not to load the scene additively. See LoadSceneMode Unity doc for more information about the options.")]
		[ObjectType(typeof(LoadSceneMode))]
		public FsmEnum loadSceneMode;

		// Token: 0x040071FD RID: 29181
		[ActionSection("Result")]
		[Tooltip("True if the scene was loaded")]
		public FsmBool success;

		// Token: 0x040071FE RID: 29182
		[Tooltip("Event sent if the scene was loaded")]
		public FsmEvent successEvent;

		// Token: 0x040071FF RID: 29183
		[Tooltip("Event sent if a problem occurred, check log for information")]
		public FsmEvent failureEvent;
	}
}
