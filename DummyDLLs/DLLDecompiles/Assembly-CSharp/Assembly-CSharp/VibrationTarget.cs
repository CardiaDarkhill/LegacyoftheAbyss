using System;
using UnityEngine;

// Token: 0x020007AE RID: 1966
[Serializable]
public struct VibrationTarget
{
	// Token: 0x170007D0 RID: 2000
	// (get) Token: 0x0600458A RID: 17802 RVA: 0x0012F067 File Offset: 0x0012D267
	public VibrationMotors Motors
	{
		get
		{
			return this.motors;
		}
	}

	// Token: 0x0600458B RID: 17803 RVA: 0x0012F06F File Offset: 0x0012D26F
	public VibrationTarget(VibrationMotors motors)
	{
		this.motors = motors;
	}

	// Token: 0x0400462D RID: 17965
	[SerializeField]
	private VibrationMotors motors;
}
