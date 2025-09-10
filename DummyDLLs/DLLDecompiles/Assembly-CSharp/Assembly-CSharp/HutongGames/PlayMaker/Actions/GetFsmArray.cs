using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E33 RID: 3635
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Copy an Array Variable from another FSM.")]
	public class GetFsmArray : BaseFsmVariableAction
	{
		// Token: 0x0600683D RID: 26685 RVA: 0x0020C5D0 File Offset: 0x0020A7D0
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.variableName = null;
			this.storeValue = null;
			this.copyValues = true;
		}

		// Token: 0x0600683E RID: 26686 RVA: 0x0020C5FE File Offset: 0x0020A7FE
		public override void OnEnter()
		{
			this.DoSetFsmArrayCopy();
			base.Finish();
		}

		// Token: 0x0600683F RID: 26687 RVA: 0x0020C60C File Offset: 0x0020A80C
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
			if (fsmArray.ElementType != this.storeValue.ElementType)
			{
				base.LogError(string.Concat(new string[]
				{
					"Can only copy arrays with the same elements type. Found <",
					fsmArray.ElementType.ToString(),
					"> and <",
					this.storeValue.ElementType.ToString(),
					">"
				}));
				return;
			}
			this.storeValue.Resize(0);
			if (this.copyValues)
			{
				this.storeValue.Values = (fsmArray.Values.Clone() as object[]);
			}
			else
			{
				this.storeValue.Values = fsmArray.Values;
			}
			this.storeValue.SaveChanges();
		}

		// Token: 0x0400676A RID: 26474
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400676B RID: 26475
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object.")]
		public FsmString fsmName;

		// Token: 0x0400676C RID: 26476
		[RequiredField]
		[UIHint(UIHint.FsmArray)]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		// Token: 0x0400676D RID: 26477
		[RequiredField]
		[Tooltip("Get the content of the array variable.")]
		[UIHint(UIHint.Variable)]
		public FsmArray storeValue;

		// Token: 0x0400676E RID: 26478
		[Tooltip("If true, makes copies. if false, values share the same reference and editing one array item value will affect the source and vice versa. Warning, this only affect the current items of the source array. Adding or removing items doesn't affect other FsmArrays.")]
		public bool copyValues;
	}
}
