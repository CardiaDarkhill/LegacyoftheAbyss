using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200104E RID: 4174
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Loads the scene by its name or index in Build Settings.")]
	public class LoadSceneAsynch : FsmStateAction
	{
		// Token: 0x06007237 RID: 29239 RVA: 0x00231588 File Offset: 0x0022F788
		public override void Reset()
		{
			this.sceneReference = GetSceneActionBase.SceneSimpleReferenceOptions.SceneAtIndex;
			this.sceneByName = null;
			this.sceneAtIndex = null;
			this.loadSceneMode = null;
			this.aSyncOperationHashCode = null;
			this.allowSceneActivation = null;
			this.operationPriority = new FsmInt
			{
				UseVariable = true
			};
			this.pendingActivation = null;
			this.pendingActivationEvent = null;
			this.isDone = null;
			this.progress = null;
			this.doneEvent = null;
			this.sceneNotFoundEvent = null;
		}

		// Token: 0x06007238 RID: 29240 RVA: 0x002315FC File Offset: 0x0022F7FC
		public override void OnEnter()
		{
			this.pendingActivationCallBackDone = false;
			this.pendingActivation.Value = false;
			this.isDone.Value = false;
			this.progress.Value = 0f;
			if (!this.DoLoadAsynch())
			{
				base.Fsm.Event(this.sceneNotFoundEvent);
				base.Finish();
			}
		}

		// Token: 0x06007239 RID: 29241 RVA: 0x00231658 File Offset: 0x0022F858
		private bool DoLoadAsynch()
		{
			if (this.sceneReference == GetSceneActionBase.SceneSimpleReferenceOptions.SceneAtIndex)
			{
				if (SceneManager.GetActiveScene().buildIndex == this.sceneAtIndex.Value)
				{
					return false;
				}
				this._asyncOperation = SceneManager.LoadSceneAsync(this.sceneAtIndex.Value, (LoadSceneMode)this.loadSceneMode.Value);
			}
			else
			{
				if (SceneManager.GetActiveScene().name == this.sceneByName.Value)
				{
					return false;
				}
				this._asyncOperation = SceneManager.LoadSceneAsync(this.sceneByName.Value, (LoadSceneMode)this.loadSceneMode.Value);
			}
			if (this._asyncOperation == null)
			{
				return false;
			}
			if (!this.operationPriority.IsNone)
			{
				this._asyncOperation.priority = this.operationPriority.Value;
			}
			this._asyncOperation.allowSceneActivation = this.allowSceneActivation.Value;
			if (!this.aSyncOperationHashCode.IsNone)
			{
				if (LoadSceneAsynch.aSyncOperationLUT == null)
				{
					LoadSceneAsynch.aSyncOperationLUT = new Dictionary<int, AsyncOperation>();
				}
				this._asynchOperationUid = ++LoadSceneAsynch.aSynchUidCounter;
				this.aSyncOperationHashCode.Value = this._asynchOperationUid;
				LoadSceneAsynch.aSyncOperationLUT.Add(this._asynchOperationUid, this._asyncOperation);
			}
			return true;
		}

		// Token: 0x0600723A RID: 29242 RVA: 0x00231794 File Offset: 0x0022F994
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
				if (LoadSceneAsynch.aSyncOperationLUT != null && this._asynchOperationUid != -1)
				{
					LoadSceneAsynch.aSyncOperationLUT.Remove(this._asynchOperationUid);
				}
				this._asyncOperation = null;
				base.Fsm.Event(this.doneEvent);
				base.Finish();
				return;
			}
			this.progress.Value = this._asyncOperation.progress;
			if (!this._asyncOperation.allowSceneActivation && this.allowSceneActivation.Value)
			{
				this._asyncOperation.allowSceneActivation = true;
			}
			if (this._asyncOperation.progress == 0.9f && !this._asyncOperation.allowSceneActivation && !this.pendingActivationCallBackDone)
			{
				this.pendingActivationCallBackDone = true;
				if (!this.pendingActivation.IsNone)
				{
					this.pendingActivation.Value = true;
				}
				base.Fsm.Event(this.pendingActivationEvent);
			}
		}

		// Token: 0x0600723B RID: 29243 RVA: 0x002318AD File Offset: 0x0022FAAD
		public override void OnExit()
		{
			this._asyncOperation = null;
		}

		// Token: 0x04007200 RID: 29184
		[Tooltip("The reference options of the Scene")]
		public GetSceneActionBase.SceneSimpleReferenceOptions sceneReference;

		// Token: 0x04007201 RID: 29185
		[Tooltip("The name of the scene to load. The given sceneName can either be the last part of the path, without .unity extension or the full path still without the .unity extension")]
		public FsmString sceneByName;

		// Token: 0x04007202 RID: 29186
		[Tooltip("The index of the scene to load.")]
		public FsmInt sceneAtIndex;

		// Token: 0x04007203 RID: 29187
		[Tooltip("Allows you to specify whether or not to load the scene additively. See LoadSceneMode Unity doc for more information about the options.")]
		[ObjectType(typeof(LoadSceneMode))]
		public FsmEnum loadSceneMode;

		// Token: 0x04007204 RID: 29188
		[Tooltip("Allow the scene to be activated as soon as it's ready")]
		public FsmBool allowSceneActivation;

		// Token: 0x04007205 RID: 29189
		[Tooltip("lets you tweak in which order async operation calls will be performed. Leave to none for default")]
		public FsmInt operationPriority;

		// Token: 0x04007206 RID: 29190
		[ActionSection("Result")]
		[Tooltip("Use this hash to activate the Scene if you have set 'AllowSceneActivation' to false, you'll need to use it in the action 'AllowSceneActivation' to effectively load the scene.")]
		[UIHint(UIHint.Variable)]
		public FsmInt aSyncOperationHashCode;

		// Token: 0x04007207 RID: 29191
		[Tooltip("The loading's progress.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat progress;

		// Token: 0x04007208 RID: 29192
		[Tooltip("True when loading is done")]
		[UIHint(UIHint.Variable)]
		public FsmBool isDone;

		// Token: 0x04007209 RID: 29193
		[Tooltip("True when loading is done but still waiting for scene activation")]
		[UIHint(UIHint.Variable)]
		public FsmBool pendingActivation;

		// Token: 0x0400720A RID: 29194
		[Tooltip("Event sent when scene loading is done")]
		public FsmEvent doneEvent;

		// Token: 0x0400720B RID: 29195
		[Tooltip("Event sent when scene loading is done but scene not yet activated. Use aSyncOperationHashCode value in 'AllowSceneActivation' to proceed")]
		public FsmEvent pendingActivationEvent;

		// Token: 0x0400720C RID: 29196
		[Tooltip("Event sent if the scene to load was not found")]
		public FsmEvent sceneNotFoundEvent;

		// Token: 0x0400720D RID: 29197
		private AsyncOperation _asyncOperation;

		// Token: 0x0400720E RID: 29198
		private int _asynchOperationUid = -1;

		// Token: 0x0400720F RID: 29199
		private bool pendingActivationCallBackDone;

		// Token: 0x04007210 RID: 29200
		public static Dictionary<int, AsyncOperation> aSyncOperationLUT;

		// Token: 0x04007211 RID: 29201
		private static int aSynchUidCounter;
	}
}
