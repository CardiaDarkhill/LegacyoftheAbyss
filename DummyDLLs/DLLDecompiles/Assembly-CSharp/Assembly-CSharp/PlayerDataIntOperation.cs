using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020006F3 RID: 1779
[Serializable]
public struct PlayerDataIntOperation
{
	// Token: 0x06003FC6 RID: 16326 RVA: 0x001193FC File Offset: 0x001175FC
	public void Execute()
	{
		int num = PlayerData.instance.GetVariable(this.variableName);
		switch (this.operation)
		{
		case PlayerDataIntOperation.Operation.Add:
			num += this.number;
			break;
		case PlayerDataIntOperation.Operation.Subtract:
			num -= this.number;
			break;
		case PlayerDataIntOperation.Operation.Multiply:
			num *= this.number;
			break;
		case PlayerDataIntOperation.Operation.Set:
			num = this.number;
			break;
		}
		PlayerData.instance.SetVariable(this.variableName, num);
	}

	// Token: 0x04004169 RID: 16745
	[SerializeField]
	[PlayerDataField(typeof(int), true)]
	private string variableName;

	// Token: 0x0400416A RID: 16746
	[SerializeField]
	private PlayerDataIntOperation.Operation operation;

	// Token: 0x0400416B RID: 16747
	[SerializeField]
	public int number;

	// Token: 0x020019E5 RID: 6629
	private enum Operation
	{
		// Token: 0x0400979E RID: 38814
		Add,
		// Token: 0x0400979F RID: 38815
		Subtract,
		// Token: 0x040097A0 RID: 38816
		Multiply,
		// Token: 0x040097A1 RID: 38817
		Set
	}
}
