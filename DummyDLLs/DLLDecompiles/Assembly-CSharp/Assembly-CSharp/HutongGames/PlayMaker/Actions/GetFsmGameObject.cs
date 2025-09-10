using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200108E RID: 4238
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Get the value of a Game Object Variable from another FSM.")]
	public class GetFsmGameObject : FsmStateAction
	{
		// Token: 0x0600734F RID: 29519 RVA: 0x00236A80 File Offset: 0x00234C80
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.storeValue = null;
		}

		// Token: 0x06007350 RID: 29520 RVA: 0x00236AA0 File Offset: 0x00234CA0
		public override void OnEnter()
		{
			this.DoGetFsmGameObject();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007351 RID: 29521 RVA: 0x00236AB6 File Offset: 0x00234CB6
		public override void OnUpdate()
		{
			this.DoGetFsmGameObject();
		}

		// Token: 0x06007352 RID: 29522 RVA: 0x00236AC0 File Offset: 0x00234CC0
		private void DoGetFsmGameObject()
		{
			if (this.storeValue == null)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget != this.goLastFrame || this.fsmName.Value != this.fsmNameLastFrame)
			{
				this.goLastFrame = ownerDefaultTarget;
				this.fsmNameLastFrame = this.fsmName.Value;
				this.fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
			}
			if (this.fsm == null)
			{
				return;
			}
			FsmGameObject fsmGameObject = this.fsm.FsmVariables.GetFsmGameObject(this.variableName.Value);
			if (fsmGameObject == null)
			{
				return;
			}
			this.storeValue.Value = fsmGameObject.Value;
		}

		// Token: 0x04007360 RID: 29536
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007361 RID: 29537
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04007362 RID: 29538
		[RequiredField]
		[UIHint(UIHint.FsmGameObject)]
		[Tooltip("The name of the FSM variable to get.")]
		public FsmString variableName;

		// Token: 0x04007363 RID: 29539
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the value in a GameObject variable in this FSM.")]
		public FsmGameObject storeValue;

		// Token: 0x04007364 RID: 29540
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x04007365 RID: 29541
		private GameObject goLastFrame;

		// Token: 0x04007366 RID: 29542
		private string fsmNameLastFrame;

		// Token: 0x04007367 RID: 29543
		private PlayMakerFSM fsm;
	}
}
