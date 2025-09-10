using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200104C RID: 4172
	public abstract class GetSceneActionBase : FsmStateAction
	{
		// Token: 0x06007230 RID: 29232 RVA: 0x002312CC File Offset: 0x0022F4CC
		public override void Reset()
		{
			base.Reset();
			this.sceneReference = GetSceneActionBase.SceneAllReferenceOptions.ActiveScene;
			this.sceneAtIndex = null;
			this.sceneByName = null;
			this.sceneByPath = null;
			this.sceneByGameObject = null;
			this.sceneFound = null;
			this.sceneFoundEvent = null;
			this.sceneNotFoundEvent = null;
		}

		// Token: 0x06007231 RID: 29233 RVA: 0x0023130C File Offset: 0x0022F50C
		public override void OnEnter()
		{
			try
			{
				switch (this.sceneReference)
				{
				case GetSceneActionBase.SceneAllReferenceOptions.ActiveScene:
					this._scene = SceneManager.GetActiveScene();
					break;
				case GetSceneActionBase.SceneAllReferenceOptions.SceneAtIndex:
					this._scene = SceneManager.GetSceneAt(this.sceneAtIndex.Value);
					break;
				case GetSceneActionBase.SceneAllReferenceOptions.SceneByName:
					this._scene = SceneManager.GetSceneByName(this.sceneByName.Value);
					break;
				case GetSceneActionBase.SceneAllReferenceOptions.SceneByPath:
					this._scene = SceneManager.GetSceneByPath(this.sceneByPath.Value);
					break;
				case GetSceneActionBase.SceneAllReferenceOptions.SceneByGameObject:
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
			}
			if (this._scene == default(Scene))
			{
				this._sceneFound = false;
				if (!this.sceneFound.IsNone)
				{
					this.sceneFound.Value = false;
				}
				base.Fsm.Event(this.sceneNotFoundEvent);
				return;
			}
			this._sceneFound = true;
			if (!this.sceneFound.IsNone)
			{
				this.sceneFound.Value = true;
			}
		}

		// Token: 0x040071EF RID: 29167
		[Tooltip("The reference option of the Scene")]
		public GetSceneActionBase.SceneAllReferenceOptions sceneReference;

		// Token: 0x040071F0 RID: 29168
		[Tooltip("The scene Index.")]
		public FsmInt sceneAtIndex;

		// Token: 0x040071F1 RID: 29169
		[Tooltip("The scene Name.")]
		public FsmString sceneByName;

		// Token: 0x040071F2 RID: 29170
		[Tooltip("The scene Path.")]
		public FsmString sceneByPath;

		// Token: 0x040071F3 RID: 29171
		[Tooltip("The Scene of GameObject")]
		public FsmOwnerDefault sceneByGameObject;

		// Token: 0x040071F4 RID: 29172
		[Tooltip("True if SceneReference resolves to a scene")]
		[UIHint(UIHint.Variable)]
		public FsmBool sceneFound;

		// Token: 0x040071F5 RID: 29173
		[Tooltip("Event sent if SceneReference resolves to a scene")]
		public FsmEvent sceneFoundEvent;

		// Token: 0x040071F6 RID: 29174
		[Tooltip("Event sent if SceneReference do not resolve to a scene")]
		public FsmEvent sceneNotFoundEvent;

		// Token: 0x040071F7 RID: 29175
		[Tooltip("The Scene Cache")]
		protected Scene _scene;

		// Token: 0x040071F8 RID: 29176
		[Tooltip("True if a scene was found, use _scene to access it")]
		protected bool _sceneFound;

		// Token: 0x02001BBC RID: 7100
		public enum SceneReferenceOptions
		{
			// Token: 0x04009E76 RID: 40566
			SceneAtIndex,
			// Token: 0x04009E77 RID: 40567
			SceneByName,
			// Token: 0x04009E78 RID: 40568
			SceneByPath
		}

		// Token: 0x02001BBD RID: 7101
		public enum SceneSimpleReferenceOptions
		{
			// Token: 0x04009E7A RID: 40570
			SceneAtIndex,
			// Token: 0x04009E7B RID: 40571
			SceneByName
		}

		// Token: 0x02001BBE RID: 7102
		public enum SceneBuildReferenceOptions
		{
			// Token: 0x04009E7D RID: 40573
			SceneAtBuildIndex,
			// Token: 0x04009E7E RID: 40574
			SceneByName
		}

		// Token: 0x02001BBF RID: 7103
		public enum SceneAllReferenceOptions
		{
			// Token: 0x04009E80 RID: 40576
			ActiveScene,
			// Token: 0x04009E81 RID: 40577
			SceneAtIndex,
			// Token: 0x04009E82 RID: 40578
			SceneByName,
			// Token: 0x04009E83 RID: 40579
			SceneByPath,
			// Token: 0x04009E84 RID: 40580
			SceneByGameObject
		}
	}
}
