using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E32 RID: 3634
	[ActionCategory(ActionCategory.Array)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Obsolete("This action was wip and accidentally released.")]
	[Tooltip("Set an item in an Array Variable in another FSM.")]
	public class FsmArraySet : FsmStateAction
	{
		// Token: 0x06006838 RID: 26680 RVA: 0x0020C4B3 File Offset: 0x0020A6B3
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.setValue = null;
		}

		// Token: 0x06006839 RID: 26681 RVA: 0x0020C4D3 File Offset: 0x0020A6D3
		public override void OnEnter()
		{
			this.DoSetFsmString();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600683A RID: 26682 RVA: 0x0020C4EC File Offset: 0x0020A6EC
		private void DoSetFsmString()
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
			if (ownerDefaultTarget != this.goLastFrame)
			{
				this.goLastFrame = ownerDefaultTarget;
				this.fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
			}
			if (this.fsm == null)
			{
				base.LogWarning("Could not find FSM: " + this.fsmName.Value);
				return;
			}
			FsmString fsmString = this.fsm.FsmVariables.GetFsmString(this.variableName.Value);
			if (fsmString != null)
			{
				fsmString.Value = this.setValue.Value;
				return;
			}
			base.LogWarning("Could not find variable: " + this.variableName.Value);
		}

		// Token: 0x0600683B RID: 26683 RVA: 0x0020C5C0 File Offset: 0x0020A7C0
		public override void OnUpdate()
		{
			this.DoSetFsmString();
		}

		// Token: 0x04006763 RID: 26467
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006764 RID: 26468
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object.")]
		public FsmString fsmName;

		// Token: 0x04006765 RID: 26469
		[RequiredField]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		// Token: 0x04006766 RID: 26470
		[Tooltip("Set the value of the variable.")]
		public FsmString setValue;

		// Token: 0x04006767 RID: 26471
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x04006768 RID: 26472
		private GameObject goLastFrame;

		// Token: 0x04006769 RID: 26473
		private PlayMakerFSM fsm;
	}
}
