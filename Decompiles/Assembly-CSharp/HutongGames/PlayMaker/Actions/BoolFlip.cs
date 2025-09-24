using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F70 RID: 3952
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Flips the value of a Bool Variable. True becomes False, False becomes True.")]
	public class BoolFlip : FsmStateAction
	{
		// Token: 0x06006D94 RID: 28052 RVA: 0x00220ECE File Offset: 0x0021F0CE
		public override void Reset()
		{
			this.boolVariable = null;
		}

		// Token: 0x06006D95 RID: 28053 RVA: 0x00220ED7 File Offset: 0x0021F0D7
		public override void OnEnter()
		{
			this.boolVariable.Value = !this.boolVariable.Value;
			base.Finish();
		}

		// Token: 0x04006D57 RID: 27991
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Bool variable to flip. True becomes False, False becomes True.")]
		public FsmBool boolVariable;
	}
}
