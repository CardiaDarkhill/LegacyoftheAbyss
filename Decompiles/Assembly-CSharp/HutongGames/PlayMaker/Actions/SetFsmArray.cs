using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E35 RID: 3637
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Copy an Array Variable in another FSM.")]
	public class SetFsmArray : BaseFsmVariableAction
	{
		// Token: 0x06006846 RID: 26694 RVA: 0x0020C871 File Offset: 0x0020AA71
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.variableName = null;
			this.setValue = null;
			this.copyValues = true;
		}

		// Token: 0x06006847 RID: 26695 RVA: 0x0020C89F File Offset: 0x0020AA9F
		public override void OnEnter()
		{
			this.DoSetFsmArrayCopy();
			base.Finish();
		}

		// Token: 0x06006848 RID: 26696 RVA: 0x0020C8B0 File Offset: 0x0020AAB0
		private void DoSetFsmArrayCopy()
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
			if (fsmArray.ElementType != this.setValue.ElementType)
			{
				base.LogError(string.Concat(new string[]
				{
					"Can only copy arrays with the same elements type. Found <",
					fsmArray.ElementType.ToString(),
					"> and <",
					this.setValue.ElementType.ToString(),
					">"
				}));
				return;
			}
			fsmArray.Resize(0);
			if (this.copyValues)
			{
				fsmArray.Values = (this.setValue.Values.Clone() as object[]);
			}
			else
			{
				fsmArray.Values = this.setValue.Values;
			}
			fsmArray.SaveChanges();
		}

		// Token: 0x04006775 RID: 26485
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006776 RID: 26486
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object.")]
		public FsmString fsmName;

		// Token: 0x04006777 RID: 26487
		[RequiredField]
		[UIHint(UIHint.FsmArray)]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		// Token: 0x04006778 RID: 26488
		[RequiredField]
		[Tooltip("Set the content of the array variable.")]
		[UIHint(UIHint.Variable)]
		public FsmArray setValue;

		// Token: 0x04006779 RID: 26489
		[Tooltip("If true, makes copies. if false, values share the same reference and editing one array item value will affect the source and vice versa. Warning, this only affect the current items of the source array. Adding or removing items doesn't affect other FsmArrays.")]
		public bool copyValues;
	}
}
