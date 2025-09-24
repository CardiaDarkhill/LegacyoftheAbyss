using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E34 RID: 3636
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Gets an item in an Array Variable in another FSM.")]
	public class GetFsmArrayItem : BaseFsmVariableIndexAction
	{
		// Token: 0x06006841 RID: 26689 RVA: 0x0020C739 File Offset: 0x0020A939
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.storeValue = null;
		}

		// Token: 0x06006842 RID: 26690 RVA: 0x0020C759 File Offset: 0x0020A959
		public override void OnEnter()
		{
			this.DoGetFsmArray();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006843 RID: 26691 RVA: 0x0020C770 File Offset: 0x0020A970
		private void DoGetFsmArray()
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
			if (fsmArray.ElementType == this.storeValue.NamedVar.VariableType)
			{
				this.storeValue.SetValue(fsmArray.Get(this.index.Value));
				return;
			}
			base.LogWarning("Incompatible variable type: " + this.variableName.Value);
		}

		// Token: 0x06006844 RID: 26692 RVA: 0x0020C861 File Offset: 0x0020AA61
		public override void OnUpdate()
		{
			this.DoGetFsmArray();
		}

		// Token: 0x0400676F RID: 26479
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006770 RID: 26480
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object.")]
		public FsmString fsmName;

		// Token: 0x04006771 RID: 26481
		[RequiredField]
		[UIHint(UIHint.FsmArray)]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		// Token: 0x04006772 RID: 26482
		[Tooltip("The index into the array.")]
		public FsmInt index;

		// Token: 0x04006773 RID: 26483
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the value of the array at the specified index.")]
		public FsmVar storeValue;

		// Token: 0x04006774 RID: 26484
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;
	}
}
