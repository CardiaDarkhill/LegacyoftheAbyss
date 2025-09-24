using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020006F2 RID: 1778
[Serializable]
public struct PlayerDataBoolOperation
{
	// Token: 0x06003FC4 RID: 16324 RVA: 0x001193A1 File Offset: 0x001175A1
	private bool IsSetOperation()
	{
		return this.operation == PlayerDataBoolOperation.Operation.Set;
	}

	// Token: 0x06003FC5 RID: 16325 RVA: 0x001193AC File Offset: 0x001175AC
	public void Execute()
	{
		bool flag = PlayerData.instance.GetVariable(this.variableName);
		PlayerDataBoolOperation.Operation operation = this.operation;
		if (operation != PlayerDataBoolOperation.Operation.Set)
		{
			if (operation == PlayerDataBoolOperation.Operation.Flip)
			{
				flag = !flag;
			}
		}
		else
		{
			flag = this.value;
		}
		PlayerData.instance.SetVariable(this.variableName, flag);
	}

	// Token: 0x04004166 RID: 16742
	[SerializeField]
	[PlayerDataField(typeof(bool), true)]
	private string variableName;

	// Token: 0x04004167 RID: 16743
	[SerializeField]
	private PlayerDataBoolOperation.Operation operation;

	// Token: 0x04004168 RID: 16744
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsSetOperation", true, true, true)]
	public bool value;

	// Token: 0x020019E4 RID: 6628
	private enum Operation
	{
		// Token: 0x0400979B RID: 38811
		Set,
		// Token: 0x0400979C RID: 38812
		Flip
	}
}
