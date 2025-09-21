using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001054 RID: 4180
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Set the scene to be active.")]
	public class SetActiveScene : FsmStateAction
	{
		// Token: 0x06007257 RID: 29271 RVA: 0x00231E48 File Offset: 0x00230048
		public override void Reset()
		{
			this.sceneReference = SetActiveScene.SceneReferenceOptions.SceneAtIndex;
			this.sceneByName = null;
			this.sceneAtBuildIndex = null;
			this.sceneAtIndex = null;
			this.sceneByPath = null;
			this.sceneByGameObject = null;
			this.success = null;
			this.successEvent = null;
			this.sceneFound = null;
			this.sceneNotActivatedEvent = null;
			this.sceneNotFoundEvent = null;
		}

		// Token: 0x06007258 RID: 29272 RVA: 0x00231EA4 File Offset: 0x002300A4
		public override void OnEnter()
		{
			this.DoSetActivate();
			if (!this.success.IsNone)
			{
				this.success.Value = this._success;
			}
			if (!this.sceneFound.IsNone)
			{
				this.sceneFound.Value = this._sceneFound;
			}
			if (this._success)
			{
				base.Fsm.Event(this.successEvent);
			}
		}

		// Token: 0x06007259 RID: 29273 RVA: 0x00231F0C File Offset: 0x0023010C
		private void DoSetActivate()
		{
			try
			{
				switch (this.sceneReference)
				{
				case SetActiveScene.SceneReferenceOptions.SceneAtIndex:
					this._scene = SceneManager.GetSceneAt(this.sceneAtIndex.Value);
					break;
				case SetActiveScene.SceneReferenceOptions.SceneByName:
					this._scene = SceneManager.GetSceneByName(this.sceneByName.Value);
					break;
				case SetActiveScene.SceneReferenceOptions.SceneByPath:
					this._scene = SceneManager.GetSceneByPath(this.sceneByPath.Value);
					break;
				case SetActiveScene.SceneReferenceOptions.SceneByGameObject:
				{
					GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.sceneByGameObject);
					if (ownerDefaultTarget == null)
					{
						throw new Exception("Null GameObject");
					}
					this._scene = ownerDefaultTarget.scene;
					break;
				}
				}
			}
			catch (Exception ex)
			{
				base.LogError(ex.Message);
				this._sceneFound = false;
				base.Fsm.Event(this.sceneNotFoundEvent);
				return;
			}
			if (this._scene == default(Scene))
			{
				this._sceneFound = false;
				base.Fsm.Event(this.sceneNotFoundEvent);
				return;
			}
			this._success = SceneManager.SetActiveScene(this._scene);
			this._sceneFound = true;
		}

		// Token: 0x04007233 RID: 29235
		[Tooltip("The reference options of the Scene.")]
		public SetActiveScene.SceneReferenceOptions sceneReference;

		// Token: 0x04007234 RID: 29236
		[Tooltip("The name of the scene to activate. The given sceneName can either be the last part of the path, without .unity extension or the full path still without the .unity extension")]
		public FsmString sceneByName;

		// Token: 0x04007235 RID: 29237
		[Tooltip("The build index of the scene to activate.")]
		public FsmInt sceneAtBuildIndex;

		// Token: 0x04007236 RID: 29238
		[Tooltip("The index of the scene to activate.")]
		public FsmInt sceneAtIndex;

		// Token: 0x04007237 RID: 29239
		[Tooltip("The scene Path.")]
		public FsmString sceneByPath;

		// Token: 0x04007238 RID: 29240
		[Tooltip("The GameObject scene to activate.")]
		public FsmOwnerDefault sceneByGameObject;

		// Token: 0x04007239 RID: 29241
		[ActionSection("Result")]
		[Tooltip("True if set active succeeded.")]
		[UIHint(UIHint.Variable)]
		public FsmBool success;

		// Token: 0x0400723A RID: 29242
		[Tooltip("Event sent if setActive succeeded.")]
		public FsmEvent successEvent;

		// Token: 0x0400723B RID: 29243
		[Tooltip("True if SceneReference resolves to a scene.")]
		[UIHint(UIHint.Variable)]
		public FsmBool sceneFound;

		// Token: 0x0400723C RID: 29244
		[Tooltip("Event sent if scene not activated yet.")]
		[UIHint(UIHint.Variable)]
		public FsmEvent sceneNotActivatedEvent;

		// Token: 0x0400723D RID: 29245
		[Tooltip("Event sent if SceneReference do not resolve to a scene.")]
		public FsmEvent sceneNotFoundEvent;

		// Token: 0x0400723E RID: 29246
		private Scene _scene;

		// Token: 0x0400723F RID: 29247
		private bool _sceneFound;

		// Token: 0x04007240 RID: 29248
		private bool _success;

		// Token: 0x02001BC0 RID: 7104
		public enum SceneReferenceOptions
		{
			// Token: 0x04009E86 RID: 40582
			SceneAtBuildIndex,
			// Token: 0x04009E87 RID: 40583
			SceneAtIndex,
			// Token: 0x04009E88 RID: 40584
			SceneByName,
			// Token: 0x04009E89 RID: 40585
			SceneByPath,
			// Token: 0x04009E8A RID: 40586
			SceneByGameObject
		}
	}
}
