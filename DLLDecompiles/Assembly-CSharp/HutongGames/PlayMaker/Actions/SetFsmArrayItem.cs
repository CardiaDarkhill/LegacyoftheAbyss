using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E36 RID: 3638
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Set an item in an Array Variable in another FSM.")]
	public class SetFsmArrayItem : BaseFsmVariableIndexAction
	{
		// Token: 0x0600684A RID: 26698 RVA: 0x0020C9D3 File Offset: 0x0020ABD3
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.value = null;
		}

		// Token: 0x0600684B RID: 26699 RVA: 0x0020C9F3 File Offset: 0x0020ABF3
		public override void OnEnter()
		{
			this.DoSetFsmArray();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600684C RID: 26700 RVA: 0x0020CA0C File Offset: 0x0020AC0C
		private void DoSetFsmArray()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget, this.fsmName.Value))
			{
				return;
			}
			FsmArray fsmArray = this.fsm.FsmVariables.GetFsmArray(this.variableName.Value);
			if (fsmArray == null)
			{
				base.DoVariableNotFound(this.variableName.Value);
				return;
			}
			if (this.index.Value < 0 || this.index.Value >= fsmArray.Length)
			{
				base.Fsm.Event(this.indexOutOfRange);
				base.Finish();
				return;
			}
			if (fsmArray.ElementType == this.value.NamedVar.VariableType)
			{
				this.value.UpdateValue();
				fsmArray.Set(this.index.Value, this.value.GetValue());
				return;
			}
			base.LogWarning("Incompatible variable type: " + this.variableName.Value);
		}

		// Token: 0x0600684D RID: 26701 RVA: 0x0020CB08 File Offset: 0x0020AD08
		public override void OnUpdate()
		{
			this.DoSetFsmArray();
		}

		// Token: 0x0400677A RID: 26490
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400677B RID: 26491
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object.")]
		public FsmString fsmName;

		// Token: 0x0400677C RID: 26492
		[RequiredField]
		[UIHint(UIHint.FsmArray)]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		// Token: 0x0400677D RID: 26493
		[Tooltip("The index into the array.")]
		public FsmInt index;

		// Token: 0x0400677E RID: 26494
		[RequiredField]
		[Tooltip("Set the value of the array at the specified index.")]
		public FsmVar value;

		// Token: 0x0400677F RID: 26495
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;
	}
}
