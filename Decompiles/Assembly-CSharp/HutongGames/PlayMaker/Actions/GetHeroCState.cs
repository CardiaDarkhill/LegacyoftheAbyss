using System;
using System.Text;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001248 RID: 4680
	public class GetHeroCState : FsmStateAction
	{
		// Token: 0x06007BBC RID: 31676 RVA: 0x00250546 File Offset: 0x0024E746
		public override void Reset()
		{
			this.VariableName = null;
			this.StoreValue = null;
			this.EveryFrame = false;
		}

		// Token: 0x06007BBD RID: 31677 RVA: 0x0025055D File Offset: 0x0024E75D
		public override void OnEnter()
		{
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007BBE RID: 31678 RVA: 0x00250573 File Offset: 0x0024E773
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06007BBF RID: 31679 RVA: 0x0025057B File Offset: 0x0024E77B
		private void DoAction()
		{
			if (!this.VariableName.IsNone && !this.StoreValue.IsNone)
			{
				this.StoreValue.Value = HeroController.instance.GetCState(this.VariableName.Value);
			}
		}

		// Token: 0x06007BC0 RID: 31680 RVA: 0x002505B8 File Offset: 0x0024E7B8
		public override string ErrorCheck()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (string.IsNullOrEmpty(this.VariableName.Value))
			{
				stringBuilder.AppendLine("State name must be specified!");
			}
			else if (!HeroController.CStateExists(this.VariableName.Value))
			{
				stringBuilder.AppendLine("State could not be found in HeroControllerStates");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04007BF2 RID: 31730
		public FsmString VariableName;

		// Token: 0x04007BF3 RID: 31731
		[UIHint(UIHint.Variable)]
		public FsmBool StoreValue;

		// Token: 0x04007BF4 RID: 31732
		public bool EveryFrame;
	}
}
