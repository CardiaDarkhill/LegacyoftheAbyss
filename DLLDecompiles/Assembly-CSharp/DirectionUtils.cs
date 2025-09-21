using System;
using UnityEngine;

// Token: 0x02000347 RID: 839
public static class DirectionUtils
{
	// Token: 0x06001D2C RID: 7468 RVA: 0x000871A7 File Offset: 0x000853A7
	public static int GetCardinalDirection(float degrees)
	{
		return DirectionUtils.NegSafeMod(Mathf.RoundToInt(degrees / 90f), 4);
	}

	// Token: 0x06001D2D RID: 7469 RVA: 0x000871BB File Offset: 0x000853BB
	public static int NegSafeMod(int val, int len)
	{
		return (val % len + len) % len;
	}

	// Token: 0x06001D2E RID: 7470 RVA: 0x000871C4 File Offset: 0x000853C4
	public static int GetX(int cardinalDirection)
	{
		int num = cardinalDirection % 4;
		int result;
		if (num != 0)
		{
			if (num != 2)
			{
				result = 0;
			}
			else
			{
				result = -1;
			}
		}
		else
		{
			result = 1;
		}
		return result;
	}

	// Token: 0x06001D2F RID: 7471 RVA: 0x000871EC File Offset: 0x000853EC
	public static int GetY(int cardinalDirection)
	{
		int num = cardinalDirection % 4;
		int result;
		if (num != 1)
		{
			if (num != 3)
			{
				result = 0;
			}
			else
			{
				result = -1;
			}
		}
		else
		{
			result = 1;
		}
		return result;
	}

	// Token: 0x06001D30 RID: 7472 RVA: 0x00087214 File Offset: 0x00085414
	public static float GetAngle(int cardinalDirection)
	{
		float result;
		switch (cardinalDirection)
		{
		case 0:
			result = 0f;
			break;
		case 1:
			result = 90f;
			break;
		case 2:
			result = 180f;
			break;
		case 3:
			result = 270f;
			break;
		default:
			throw new ArgumentOutOfRangeException("cardinalDirection", cardinalDirection, null);
		}
		return result;
	}

	// Token: 0x06001D31 RID: 7473 RVA: 0x0008726C File Offset: 0x0008546C
	public static float GetAngle(HitInstance.HitDirection hitDirection)
	{
		float result;
		switch (hitDirection)
		{
		case HitInstance.HitDirection.Left:
			result = 180f;
			break;
		case HitInstance.HitDirection.Right:
			result = 0f;
			break;
		case HitInstance.HitDirection.Up:
			result = 90f;
			break;
		case HitInstance.HitDirection.Down:
			result = 270f;
			break;
		default:
			throw new ArgumentOutOfRangeException("hitDirection", hitDirection, null);
		}
		return result;
	}

	// Token: 0x04001C75 RID: 7285
	public const int Right = 0;

	// Token: 0x04001C76 RID: 7286
	public const int Up = 1;

	// Token: 0x04001C77 RID: 7287
	public const int Left = 2;

	// Token: 0x04001C78 RID: 7288
	public const int Down = 3;
}
