using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001056 RID: 4182
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Unload a scene asynchronously by its name or index in Build Settings. Destroys all GameObjects associated with the given scene and removes the scene from the SceneManager.")]
	public class UnloadSceneAsynch : FsmStateAction
	{
		// Token: 0x0600725F RID: 29279 RVA: 0x00232200 File Offset: 0x00230400
		public override void Reset()
		{
			this.sceneReference = UnloadSceneAsynch.SceneReferenceOptions.SceneAtBuildIndex;
			this.sceneByName = null;
			this.sceneAtBuildIndex = null;
			this.sceneAtIndex = null;
			this.sceneByPath = null;
			this.sceneByGameObject = null;
			this.operationPriority = new FsmInt
			{
				UseVariable = true
			};
			this.isDone = null;
			this.progress = null;
			this.doneEvent = null;
			this.sceneNotFoundEvent = null;
		}

		// Token: 0x06007260 RID: 29280 RVA: 0x00232265 File Offset: 0x00230465
		public override void OnEnter()
		{
			this.isDone.Value = false;
			this.progress.Value = 0f;
			if (!this.DoUnLoadAsynch())
			{
				base.Fsm.Event(this.sceneNotFoundEvent);
				base.Finish();
			}
		}

		// Token: 0x06007261 RID: 29281 RVA: 0x002322A4 File Offset: 0x002304A4
		private bool DoUnLoadAsynch()
		{
			try
			{
				switch (this.sceneReference)
				{
				case UnloadSceneAsynch.SceneReferenceOptions.ActiveScene:
					this._asyncOperation = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
					break;
				case UnloadSceneAsynch.SceneReferenceOptions.SceneAtBuildIndex:
					this._asyncOperation = SceneManager.UnloadSceneAsync(this.sceneAtBuildIndex.Value);
					break;
				case UnloadSceneAsynch.SceneReferenceOptions.SceneAtIndex:
					this._asyncOperation = SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(this.sceneAtIndex.Value));
					break;
				case UnloadSceneAsynch.SceneReferenceOptions.SceneByName:
					this._asyncOperation = SceneManager.UnloadSceneAsync(this.sceneByName.Value);
					break;
				case UnloadSceneAsynch.SceneReferenceOptions.SceneByPath:
					this._asyncOperation = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByPath(this.sceneByPath.Value));
					break;
				case UnloadSceneAsynch.SceneReferenceOptions.SceneByGameObject:
				{
					GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.sceneByGameObject);
					if (ownerDefaultTarget == null)
					{
						throw new Exception("Null GameObject");
					}
					this._asyncOperation = SceneManager.UnloadSceneAsync(ownerDefaultTarget.scene);
					break;
				}
				}
			}
			catch (Exception ex)
			{
				base.LogError(ex.Message);
				return false;
			}
			if (!this.operationPriority.IsNone)
			{
				this._asyncOperation.priority = this.operationPriority.Value;
			}
			return true;
		}

		// Token: 0x06007262 RID: 29282 RVA: 0x002323E0 File Offset: 0x002305E0
		public override void OnUpdate()
		{
			if (this._asyncOperation == null)
			{
				return;
			}
			if (this._asyncOperation.isDone)
			{
				this.isDone.Value = true;
				this.progress.Value = this._asyncOperation.progress;
				this._asyncOperation = null;
				base.Fsm.Event(this.doneEvent);
				base.Finish();
				return;
			}
			this.progress.Value = this._asyncOperation.progress;
		}

		// Token: 0x06007263 RID: 29283 RVA: 0x0023245A File Offset: 0x0023065A
		public override void OnExit()
		{
			this._asyncOperation = null;
		}

		// Token: 0x0400724A RID: 29258
		[Tooltip("The reference options of the Scene")]
		public UnloadSceneAsynch.SceneReferenceOptions sceneReference;

		// Token: 0x0400724B RID: 29259
		[Tooltip("The name of the scene to load. The given sceneName can either be the last part of the path, without .unity extension or the full path still without the .unity extension")]
		public FsmString sceneByName;

		// Token: 0x0400724C RID: 29260
		[Tooltip("The build index of the scene to unload.")]
		public FsmInt sceneAtBuildIndex;

		// Token: 0x0400724D RID: 29261
		[Tooltip("The index of the scene to unload.")]
		public FsmInt sceneAtIndex;

		// Token: 0x0400724E RID: 29262
		[Tooltip("The scene Path.")]
		public FsmString sceneByPath;

		// Token: 0x0400724F RID: 29263
		[Tooltip("The GameObject unload scene of")]
		public FsmOwnerDefault sceneByGameObject;

		// Token: 0x04007250 RID: 29264
		[Tooltip("lets you tweak in which order async operation calls will be performed. Leave to none for default")]
		public FsmInt operationPriority;

		// Token: 0x04007251 RID: 29265
		[ActionSection("Result")]
		[Tooltip("The loading's progress.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat progress;

		// Token: 0x04007252 RID: 29266
		[Tooltip("True when loading is done")]
		[UIHint(UIHint.Variable)]
		public FsmBool isDone;

		// Token: 0x04007253 RID: 29267
		[Tooltip("Event sent when scene loading is done")]
		public FsmEvent doneEvent;

		// Token: 0x04007254 RID: 29268
		[Tooltip("Event sent if the scene to load was not found")]
		public FsmEvent sceneNotFoundEvent;

		// Token: 0x04007255 RID: 29269
		private AsyncOperation _asyncOperation;

		// Token: 0x02001BC2 RID: 7106
		public enum SceneReferenceOptions
		{
			// Token: 0x04009E93 RID: 40595
			ActiveScene,
			// Token: 0x04009E94 RID: 40596
			SceneAtBuildIndex,
			// Token: 0x04009E95 RID: 40597
			SceneAtIndex,
			// Token: 0x04009E96 RID: 40598
			SceneByName,
			// Token: 0x04009E97 RID: 40599
			SceneByPath,
			// Token: 0x04009E98 RID: 40600
			SceneByGameObject
		}
	}
}
