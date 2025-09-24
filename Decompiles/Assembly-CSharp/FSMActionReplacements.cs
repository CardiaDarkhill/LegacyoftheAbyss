using System;
using UnityEngine;

// Token: 0x020004F1 RID: 1265
public static class FSMActionReplacements
{
	// Token: 0x06002D59 RID: 11609 RVA: 0x000C6038 File Offset: 0x000C4238
	public static void SetMaterialColor(Component me, Color color)
	{
		Renderer component = me.GetComponent<Renderer>();
		if (component != null)
		{
			component.material.color = color;
		}
	}

	// Token: 0x06002D5A RID: 11610 RVA: 0x000C6061 File Offset: 0x000C4261
	public static FSMActionReplacements.Directions CheckDirectionWithBrokenBehaviour(float angle)
	{
		if (angle < 45f)
		{
			return FSMActionReplacements.Directions.Right;
		}
		if (angle < 135f)
		{
			return FSMActionReplacements.Directions.Up;
		}
		if (angle < 225f)
		{
			return FSMActionReplacements.Directions.Left;
		}
		if (angle < 360f)
		{
			return FSMActionReplacements.Directions.Down;
		}
		return FSMActionReplacements.Directions.Unknown;
	}

	// Token: 0x020017F7 RID: 6135
	public enum Directions
	{
		// Token: 0x04009022 RID: 36898
		Right,
		// Token: 0x04009023 RID: 36899
		Up,
		// Token: 0x04009024 RID: 36900
		Left,
		// Token: 0x04009025 RID: 36901
		Down,
		// Token: 0x04009026 RID: 36902
		Unknown
	}
}
