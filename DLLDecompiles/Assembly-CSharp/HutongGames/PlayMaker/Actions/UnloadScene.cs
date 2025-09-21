using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001055 RID: 4181
	[Obsolete("Use UnloadSceneAsynch Instead.")]
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Unload Scene. Note that assets are currently not unloaded, in order to free up asset memory call Resources.UnloadUnusedAssets.")]
	public class UnloadScene : FsmStateAction
	{
		// Token: 0x0600725B RID: 29275 RVA: 0x0023203C File Offset: 0x0023023C
		public override void Reset()
		{
			this.sceneReference = UnloadScene.SceneReferenceOptions.SceneAtBuildIndex;
			this.sceneByName = null;
			this.sceneAtBuildIndex = null;
			this.sceneAtIndex = null;
			this.sceneByPath = null;
			this.sceneByGameObject = null;
			this.unloaded = null;
			this.unloadedEvent = null;
			this.failureEvent = null;
		}

		// Token: 0x0600725C RID: 29276 RVA: 0x00232088 File Offset: 0x00230288
		public override void OnEnter()
		{
			bool flag = false;
			try
			{
				switch (this.sceneReference)
				{
				case UnloadScene.SceneReferenceOptions.ActiveScene:
					flag = SceneManager.UnloadScene(SceneManager.GetActiveScene());
					break;
				case UnloadScene.SceneReferenceOptions.SceneAtBuildIndex:
					flag = SceneManager.UnloadScene(this.sceneAtBuildIndex.Value);
					break;
				case UnloadScene.SceneReferenceOptions.SceneAtIndex:
					flag = SceneManager.UnloadScene(SceneManager.GetSceneAt(this.sceneAtIndex.Value));
					break;
				case UnloadScene.SceneReferenceOptions.SceneByName:
					flag = SceneManager.UnloadScene(this.sceneByName.Value);
					break;
				case UnloadScene.SceneReferenceOptions.SceneByPath:
					flag = SceneManager.UnloadScene(SceneManager.GetSceneByPath(this.sceneByPath.Value));
					break;
				case UnloadScene.SceneReferenceOptions.SceneByGameObject:
				{
					GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.sceneByGameObject);
					if (ownerDefaultTarget == null)
					{
						throw new Exception("Null GameObject");
					}
					flag = SceneManager.UnloadScene(ownerDefaultTarget.scene);
					break;
				}
				}
			}
			catch (Exception ex)
			{
				base.LogError(ex.Message);
			}
			if (!this.unloaded.IsNone)
			{
				this.unloaded.Value = flag;
			}
			if (flag)
			{
				base.Fsm.Event(this.unloadedEvent);
			}
			else
			{
				base.Fsm.Event(this.failureEvent);
			}
			base.Finish();
		}

		// Token: 0x0600725D RID: 29277 RVA: 0x002321C0 File Offset: 0x002303C0
		public override string ErrorCheck()
		{
			switch (this.sceneReference)
			{
			default:
				return string.Empty;
			}
		}

		// Token: 0x04007241 RID: 29249
		[Tooltip("The reference options of the Scene")]
		public UnloadScene.SceneReferenceOptions sceneReference;

		// Token: 0x04007242 RID: 29250
		[Tooltip("The name of the scene to load. The given sceneName can either be the last part of the path, without .unity extension or the full path still without the .unity extension")]
		public FsmString sceneByName;

		// Token: 0x04007243 RID: 29251
		[Tooltip("The build index of the scene to unload.")]
		public FsmInt sceneAtBuildIndex;

		// Token: 0x04007244 RID: 29252
		[Tooltip("The index of the scene to unload.")]
		public FsmInt sceneAtIndex;

		// Token: 0x04007245 RID: 29253
		[Tooltip("The scene Path.")]
		public FsmString sceneByPath;

		// Token: 0x04007246 RID: 29254
		[Tooltip("The GameObject unload scene of")]
		public FsmOwnerDefault sceneByGameObject;

		// Token: 0x04007247 RID: 29255
		[ActionSection("Result")]
		[Tooltip("True if scene was unloaded")]
		[UIHint(UIHint.Variable)]
		public FsmBool unloaded;

		// Token: 0x04007248 RID: 29256
		[Tooltip("Event sent if scene was unloaded ")]
		public FsmEvent unloadedEvent;

		// Token: 0x04007249 RID: 29257
		[Tooltip("Event sent scene was not unloaded")]
		[UIHint(UIHint.Variable)]
		public FsmEvent failureEvent;

		// Token: 0x02001BC1 RID: 7105
		public enum SceneReferenceOptions
		{
			// Token: 0x04009E8C RID: 40588
			ActiveScene,
			// Token: 0x04009E8D RID: 40589
			SceneAtBuildIndex,
			// Token: 0x04009E8E RID: 40590
			SceneAtIndex,
			// Token: 0x04009E8F RID: 40591
			SceneByName,
			// Token: 0x04009E90 RID: 40592
			SceneByPath,
			// Token: 0x04009E91 RID: 40593
			SceneByGameObject
		}
	}
}
