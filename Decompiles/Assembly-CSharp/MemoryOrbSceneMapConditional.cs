using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020003E5 RID: 997
public class MemoryOrbSceneMapConditional : MonoBehaviour
{
	// Token: 0x1700038B RID: 907
	// (get) Token: 0x0600222E RID: 8750 RVA: 0x0009DA50 File Offset: 0x0009BC50
	public bool IsActive
	{
		get
		{
			return (string.IsNullOrEmpty(this.pdBool) || PlayerData.instance.GetBool(this.pdBool) == this.targetValue) && (string.IsNullOrEmpty(this.pdBitmask) || PlayerData.instance.GetVariable(this.pdBitmask) == 0UL) && (!this.other || !this.other.IsActive);
		}
	}

	// Token: 0x040020FF RID: 8447
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string pdBool;

	// Token: 0x04002100 RID: 8448
	[SerializeField]
	private bool targetValue;

	// Token: 0x04002101 RID: 8449
	[SerializeField]
	[PlayerDataField(typeof(ulong), false)]
	private string pdBitmask;

	// Token: 0x04002102 RID: 8450
	[SerializeField]
	private MemoryOrbSceneMapConditional other;
}
