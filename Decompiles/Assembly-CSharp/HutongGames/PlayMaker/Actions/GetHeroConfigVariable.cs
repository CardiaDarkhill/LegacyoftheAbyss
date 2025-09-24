using System;
using System.Text;
using TeamCherry.SharedUtils;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012DC RID: 4828
	public class GetHeroConfigVariable : FsmStateAction
	{
		// Token: 0x06007DE5 RID: 32229 RVA: 0x002578AE File Offset: 0x00255AAE
		public override void Reset()
		{
			this.VariableName = null;
			this.StoreValue = null;
		}

		// Token: 0x06007DE6 RID: 32230 RVA: 0x002578C0 File Offset: 0x00255AC0
		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			if (!this.VariableName.IsNone && !this.StoreValue.IsNone)
			{
				this.StoreValue.SetValue(instance.Config.GetVariable(this.VariableName.Value, this.StoreValue.RealType));
			}
			base.Finish();
		}

		// Token: 0x06007DE7 RID: 32231 RVA: 0x0025791F File Offset: 0x00255B1F
		public string GetVariableName()
		{
			return this.VariableName.Value;
		}

		// Token: 0x06007DE8 RID: 32232 RVA: 0x0025792C File Offset: 0x00255B2C
		public Type GetVariableType()
		{
			return this.StoreValue.RealType;
		}

		// Token: 0x06007DE9 RID: 32233 RVA: 0x0025793C File Offset: 0x00255B3C
		public override string ErrorCheck()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (string.IsNullOrEmpty(this.GetVariableName()))
			{
				stringBuilder.AppendLine("Variable Name must be specified!");
			}
			if (this.GetVariableType() == null)
			{
				stringBuilder.AppendLine("Variable type must be specified!");
			}
			else if (!VariableExtensions.VariableExists<HeroControllerConfig>(this.GetVariableName(), this.GetVariableType()))
			{
				stringBuilder.AppendLine("Variable of correct type could not be found in HeroControllerConfig");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04007DC3 RID: 32195
		[RequiredField]
		public FsmString VariableName;

		// Token: 0x04007DC4 RID: 32196
		[UIHint(UIHint.Variable)]
		public FsmVar StoreValue;
	}
}
