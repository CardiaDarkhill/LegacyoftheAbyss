using System;
using UnityEngine;

// Token: 0x02000343 RID: 835
public class BetaGateChanger : MonoBehaviour
{
	// Token: 0x06001D0E RID: 7438 RVA: 0x00086EC4 File Offset: 0x000850C4
	public void SwitchToBetaExit()
	{
		TransitionPoint[] array = this.gates;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetTargetScene("BetaEnd");
		}
	}

	// Token: 0x04001C6C RID: 7276
	public TransitionPoint[] gates;
}
