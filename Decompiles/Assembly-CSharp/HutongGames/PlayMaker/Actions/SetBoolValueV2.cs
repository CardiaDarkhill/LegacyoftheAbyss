using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D21 RID: 3361
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets the value of a Bool Variable.")]
	public class SetBoolValueV2 : FsmStateAction
	{
		// Token: 0x0600631F RID: 25375 RVA: 0x001F5611 File Offset: 0x001F3811
		public override void Reset()
		{
			this.boolVariable = null;
			this.boolValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006320 RID: 25376 RVA: 0x001F5628 File Offset: 0x001F3828
		public override void OnEnter()
		{
			this.boolVariable.Value = this.boolValue.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006321 RID: 25377 RVA: 0x001F564E File Offset: 0x001F384E
		public override void OnUpdate()
		{
			this.boolVariable.Value = this.boolValue.Value;
		}

		// Token: 0x06006322 RID: 25378 RVA: 0x001F5666 File Offset: 0x001F3866
		public override void OnExit()
		{
			if (this.setOppositeOnExit)
			{
				this.boolVariable.Value = !this.boolValue.Value;
			}
		}

		// Token: 0x0400618D RID: 24973
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmBool boolVariable;

		// Token: 0x0400618E RID: 24974
		[RequiredField]
		public FsmBool boolValue;

		// Token: 0x0400618F RID: 24975
		public bool setOppositeOnExit;

		// Token: 0x04006190 RID: 24976
		public bool everyFrame;
	}
}
