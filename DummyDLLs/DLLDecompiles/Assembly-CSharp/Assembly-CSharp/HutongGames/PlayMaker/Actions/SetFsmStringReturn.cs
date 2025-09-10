using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001067 RID: 4199
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Set the value of a String Variable in another FSM, and returns it to it's previous value on exit.")]
	public class SetFsmStringReturn : FsmStateAction
	{
		// Token: 0x060072B8 RID: 29368 RVA: 0x00234C07 File Offset: 0x00232E07
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.setValue = null;
		}

		// Token: 0x060072B9 RID: 29369 RVA: 0x00234C28 File Offset: 0x00232E28
		public override void OnEnter()
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
			this.fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
			if (this.fsm == null)
			{
				base.LogWarning("Could not find FSM: " + this.fsmName.Value);
				return;
			}
			this.fsmString = this.fsm.FsmVariables.GetFsmString(this.variableName.Value);
			if (this.fsmString != null)
			{
				this.previousValue = this.fsmString.Value;
				this.fsmString.Value = this.setValue.Value;
			}
			else
			{
				base.LogWarning("Could not find variable: " + this.variableName.Value);
			}
			base.Finish();
		}

		// Token: 0x060072BA RID: 29370 RVA: 0x00234D0E File Offset: 0x00232F0E
		public override void OnExit()
		{
			if (this.fsmString != null)
			{
				this.fsmString.Value = this.previousValue;
			}
		}

		// Token: 0x040072BA RID: 29370
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040072BB RID: 29371
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object.")]
		public FsmString fsmName;

		// Token: 0x040072BC RID: 29372
		[RequiredField]
		[UIHint(UIHint.FsmString)]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		// Token: 0x040072BD RID: 29373
		[Tooltip("Set the value of the variable.")]
		public FsmString setValue;

		// Token: 0x040072BE RID: 29374
		private PlayMakerFSM fsm;

		// Token: 0x040072BF RID: 29375
		private FsmString fsmString;

		// Token: 0x040072C0 RID: 29376
		private string previousValue;
	}
}
