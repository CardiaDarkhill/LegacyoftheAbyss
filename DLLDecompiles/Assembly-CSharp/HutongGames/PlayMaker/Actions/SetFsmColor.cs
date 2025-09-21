using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010B2 RID: 4274
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Set the value of a Color Variable in another FSM.")]
	public class SetFsmColor : FsmStateAction
	{
		// Token: 0x06007401 RID: 29697 RVA: 0x00238CF5 File Offset: 0x00236EF5
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.setValue = null;
		}

		// Token: 0x06007402 RID: 29698 RVA: 0x00238D15 File Offset: 0x00236F15
		public override void OnEnter()
		{
			this.DoSetFsmColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007403 RID: 29699 RVA: 0x00238D2C File Offset: 0x00236F2C
		private void DoSetFsmColor()
		{
			if (this.setValue == null)
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
				base.LogWarning("Could not find FSM: " + this.fsmName.Value);
				return;
			}
			FsmColor fsmColor = this.fsm.FsmVariables.GetFsmColor(this.variableName.Value);
			if (fsmColor != null)
			{
				fsmColor.Value = this.setValue.Value;
				return;
			}
			base.LogWarning("Could not find variable: " + this.variableName.Value);
		}

		// Token: 0x06007404 RID: 29700 RVA: 0x00238E29 File Offset: 0x00237029
		public override void OnUpdate()
		{
			this.DoSetFsmColor();
		}

		// Token: 0x04007417 RID: 29719
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007418 RID: 29720
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04007419 RID: 29721
		[RequiredField]
		[UIHint(UIHint.FsmColor)]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		// Token: 0x0400741A RID: 29722
		[RequiredField]
		[Tooltip("Set the value of the variable.")]
		public FsmColor setValue;

		// Token: 0x0400741B RID: 29723
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x0400741C RID: 29724
		private GameObject goLastFrame;

		// Token: 0x0400741D RID: 29725
		private string fsmNameLastFrame;

		// Token: 0x0400741E RID: 29726
		private PlayMakerFSM fsm;
	}
}
