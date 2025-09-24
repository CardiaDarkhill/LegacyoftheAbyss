using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x0200061D RID: 1565
[Serializable]
public class ControllerImage
{
	// Token: 0x04003AB5 RID: 15029
	public string name;

	// Token: 0x04003AB6 RID: 15030
	public GamepadType gamepadType;

	// Token: 0x04003AB7 RID: 15031
	public Sprite sprite;

	// Token: 0x04003AB8 RID: 15032
	public ControllerButtonPositions buttonPositions;

	// Token: 0x04003AB9 RID: 15033
	public float displayScale = 1f;

	// Token: 0x04003ABA RID: 15034
	public float offsetY;
}
