using System;
using UnityEngine;

namespace InControl
{
	// Token: 0x020008F1 RID: 2289
	public static class DeadZone
	{
		// Token: 0x0600502E RID: 20526 RVA: 0x00172650 File Offset: 0x00170850
		public static Vector2 SeparateNotNormalized(float x, float y, float lowerDeadZone, float upperDeadZone)
		{
			float num = upperDeadZone - lowerDeadZone;
			float x2;
			if (x < 0f)
			{
				if (x > -lowerDeadZone)
				{
					x2 = 0f;
				}
				else if (x < -upperDeadZone)
				{
					x2 = -1f;
				}
				else
				{
					x2 = (x + lowerDeadZone) / num;
				}
			}
			else if (x < lowerDeadZone)
			{
				x2 = 0f;
			}
			else if (x > upperDeadZone)
			{
				x2 = 1f;
			}
			else
			{
				x2 = (x - lowerDeadZone) / num;
			}
			float y2;
			if (y < 0f)
			{
				if (y > -lowerDeadZone)
				{
					y2 = 0f;
				}
				else if (y < -upperDeadZone)
				{
					y2 = -1f;
				}
				else
				{
					y2 = (y + lowerDeadZone) / num;
				}
			}
			else if (y < lowerDeadZone)
			{
				y2 = 0f;
			}
			else if (y > upperDeadZone)
			{
				y2 = 1f;
			}
			else
			{
				y2 = (y - lowerDeadZone) / num;
			}
			return new Vector2(x2, y2);
		}

		// Token: 0x0600502F RID: 20527 RVA: 0x001726F8 File Offset: 0x001708F8
		public static Vector2 Separate(float x, float y, float lowerDeadZone, float upperDeadZone)
		{
			float num = upperDeadZone - lowerDeadZone;
			float num2;
			if (x < 0f)
			{
				if (x > -lowerDeadZone)
				{
					num2 = 0f;
				}
				else if (x < -upperDeadZone)
				{
					num2 = -1f;
				}
				else
				{
					num2 = (x + lowerDeadZone) / num;
				}
			}
			else if (x < lowerDeadZone)
			{
				num2 = 0f;
			}
			else if (x > upperDeadZone)
			{
				num2 = 1f;
			}
			else
			{
				num2 = (x - lowerDeadZone) / num;
			}
			float num3;
			if (y < 0f)
			{
				if (y > -lowerDeadZone)
				{
					num3 = 0f;
				}
				else if (y < -upperDeadZone)
				{
					num3 = -1f;
				}
				else
				{
					num3 = (y + lowerDeadZone) / num;
				}
			}
			else if (y < lowerDeadZone)
			{
				num3 = 0f;
			}
			else if (y > upperDeadZone)
			{
				num3 = 1f;
			}
			else
			{
				num3 = (y - lowerDeadZone) / num;
			}
			float num4 = (float)Math.Sqrt((double)(num2 * num2 + num3 * num3));
			if (num4 < 1E-05f)
			{
				return Vector2.zero;
			}
			return new Vector2(num2 / num4, num3 / num4);
		}

		// Token: 0x06005030 RID: 20528 RVA: 0x001727C4 File Offset: 0x001709C4
		public static Vector2 Circular(float x, float y, float lowerDeadZone, float upperDeadZone)
		{
			float num = (float)Math.Sqrt((double)(x * x + y * y));
			if (num < lowerDeadZone || num < 1E-05f)
			{
				return Vector2.zero;
			}
			Vector2 vector = new Vector2(x / num, y / num);
			if (num > upperDeadZone)
			{
				return vector;
			}
			return vector * ((num - lowerDeadZone) / (upperDeadZone - lowerDeadZone));
		}
	}
}
