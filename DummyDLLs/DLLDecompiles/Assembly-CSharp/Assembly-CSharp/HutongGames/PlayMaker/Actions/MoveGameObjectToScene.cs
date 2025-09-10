using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001050 RID: 4176
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Move a GameObject from its current scene to a new scene. It is required that the GameObject is at the root of its current scene.")]
	public class MoveGameObjectToScene : GetSceneActionBase
	{
		// Token: 0x06007243 RID: 29251 RVA: 0x00231B8D File Offset: 0x0022FD8D
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.findRootIfNecessary = null;
			this.success = null;
			this.successEvent = null;
			this.failureEvent = null;
		}

		// Token: 0x06007244 RID: 29252 RVA: 0x00231BB8 File Offset: 0x0022FDB8
		public override void OnEnter()
		{
			base.OnEnter();
			if (this._sceneFound)
			{
				this._go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (this.findRootIfNecessary.Value)
				{
					this._go = this._go.transform.root.gameObject;
				}
				if (this._go.transform.parent == null)
				{
					SceneManager.MoveGameObjectToScene(this._go, this._scene);
					this.success.Value = true;
					base.Fsm.Event(this.successEvent);
				}
				else
				{
					base.LogError("GameObject must be a root ");
					this.success.Value = false;
					base.Fsm.Event(this.failureEvent);
				}
				base.Fsm.Event(this.sceneFoundEvent);
				this._go = null;
			}
			base.Finish();
		}

		// Token: 0x04007223 RID: 29219
		[RequiredField]
		[Tooltip("The Root GameObject to move to the referenced scene")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007224 RID: 29220
		[RequiredField]
		[Tooltip("Only root GameObject can be moved, set to true to get the root of the gameobject if necessary, else watch for failure events.")]
		public FsmBool findRootIfNecessary;

		// Token: 0x04007225 RID: 29221
		[ActionSection("Result")]
		[Tooltip("True if the merge succeeded")]
		[UIHint(UIHint.Variable)]
		public FsmBool success;

		// Token: 0x04007226 RID: 29222
		[Tooltip("Event sent if merge succeeded")]
		public FsmEvent successEvent;

		// Token: 0x04007227 RID: 29223
		[Tooltip("Event sent if merge failed. Check log for information")]
		public FsmEvent failureEvent;

		// Token: 0x04007228 RID: 29224
		private GameObject _go;
	}
}
