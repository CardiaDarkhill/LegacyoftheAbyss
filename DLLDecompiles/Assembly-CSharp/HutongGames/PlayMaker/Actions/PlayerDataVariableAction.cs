using System;
using System.Text;
using TeamCherry.SharedUtils;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CCE RID: 3278
	public abstract class PlayerDataVariableAction : FsmStateAction
	{
		// Token: 0x060061BC RID: 25020
		public abstract bool GetShouldErrorCheck();

		// Token: 0x060061BD RID: 25021
		public abstract string GetVariableName();

		// Token: 0x060061BE RID: 25022
		public abstract Type GetVariableType();

		// Token: 0x060061BF RID: 25023 RVA: 0x001EF1F8 File Offset: 0x001ED3F8
		public override string ErrorCheck()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.GetShouldErrorCheck() && string.IsNullOrEmpty(this.GetVariableName()))
			{
				stringBuilder.AppendLine("Variable Name must be specified!");
			}
			if (this.GetVariableType() == null)
			{
				stringBuilder.AppendLine("Variable type must be specified!");
			}
			else if (this.GetShouldErrorCheck() && !VariableExtensions.VariableExists<PlayerData>(this.GetVariableName(), this.GetVariableType()))
			{
				stringBuilder.AppendLine("Variable of correct type could not be found in PlayerData");
			}
			return stringBuilder.ToString();
		}
	}
}
