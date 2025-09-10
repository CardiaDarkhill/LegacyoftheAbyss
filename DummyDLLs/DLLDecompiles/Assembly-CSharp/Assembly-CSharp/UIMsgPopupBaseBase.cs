using System;
using System.Runtime.CompilerServices;
using GlobalSettings;
using UnityEngine;

// Token: 0x02000742 RID: 1858
public abstract class UIMsgPopupBaseBase : MonoBehaviour
{
	// Token: 0x17000781 RID: 1921
	// (get) Token: 0x0600423E RID: 16958 RVA: 0x00124B21 File Offset: 0x00122D21
	// (set) Token: 0x0600423F RID: 16959 RVA: 0x00124B28 File Offset: 0x00122D28
	public static float MinYPos { get; set; } = float.MinValue;

	// Token: 0x06004240 RID: 16960 RVA: 0x00124B30 File Offset: 0x00122D30
	protected static void UpdatePosition(Transform transform)
	{
		UIMsgPopupBaseBase.<>c__DisplayClass5_0 CS$<>8__locals1;
		CS$<>8__locals1.transform = transform;
		if (UIMsgPopupBaseBase.LastActiveMsgShared)
		{
			if (CS$<>8__locals1.transform != UIMsgPopupBaseBase.LastActiveMsgShared)
			{
				UIMsgPopupBaseBase.<UpdatePosition>g__SetPos|5_0(UIMsgPopupBaseBase.LastActiveMsgShared.localPosition + UI.UIMsgPopupStackOffset, ref CS$<>8__locals1);
			}
		}
		else
		{
			UIMsgPopupBaseBase.<UpdatePosition>g__SetPos|5_0(UI.UIMsgPopupStartPosition, ref CS$<>8__locals1);
		}
		UIMsgPopupBaseBase.LastActiveMsgShared = CS$<>8__locals1.transform;
	}

	// Token: 0x06004243 RID: 16963 RVA: 0x00124BAB File Offset: 0x00122DAB
	[CompilerGenerated]
	internal static void <UpdatePosition>g__SetPos|5_0(Vector3 pos, ref UIMsgPopupBaseBase.<>c__DisplayClass5_0 A_1)
	{
		if (pos.y < UIMsgPopupBaseBase.MinYPos)
		{
			pos.y = UIMsgPopupBaseBase.MinYPos;
		}
		A_1.transform.localPosition = pos;
	}

	// Token: 0x040043E1 RID: 17377
	protected static Transform LastActiveMsgShared;
}
