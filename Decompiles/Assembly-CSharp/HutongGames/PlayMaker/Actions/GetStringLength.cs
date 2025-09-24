using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010C3 RID: 4291
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Gets the Length of a String.")]
	public class GetStringLength : FsmStateAction
	{
		// Token: 0x06007457 RID: 29783 RVA: 0x0023A2F2 File Offset: 0x002384F2
		public override void Reset()
		{
			this.stringVariable = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06007458 RID: 29784 RVA: 0x0023A309 File Offset: 0x00238509
		public override void OnEnter()
		{
			this.DoGetStringLength();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007459 RID: 29785 RVA: 0x0023A31F File Offset: 0x0023851F
		public override void OnUpdate()
		{
			this.DoGetStringLength();
		}

		// Token: 0x0600745A RID: 29786 RVA: 0x0023A327 File Offset: 0x00238527
		private void DoGetStringLength()
		{
			if (this.stringVariable == null)
			{
				return;
			}
			if (this.storeResult == null)
			{
				return;
			}
			this.storeResult.Value = this.stringVariable.Value.Length;
		}

		// Token: 0x04007499 RID: 29849
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The string to measure.")]
		public FsmString stringVariable;

		// Token: 0x0400749A RID: 29850
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in an Int Variable.")]
		public FsmInt storeResult;

		// Token: 0x0400749B RID: 29851
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
