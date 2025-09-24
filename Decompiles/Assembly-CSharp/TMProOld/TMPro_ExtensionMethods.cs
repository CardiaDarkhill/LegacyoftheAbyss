using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMProOld
{
	// Token: 0x020007ED RID: 2029
	public static class TMPro_ExtensionMethods
	{
		// Token: 0x060047BD RID: 18365 RVA: 0x0014D7CC File Offset: 0x0014B9CC
		public static string ArrayToString(this char[] chars)
		{
			string text = string.Empty;
			int num = 0;
			while (num < chars.Length && chars[num] != '\0')
			{
				text += chars[num].ToString();
				num++;
			}
			return text;
		}

		// Token: 0x060047BE RID: 18366 RVA: 0x0014D808 File Offset: 0x0014BA08
		public static int FindInstanceID<T>(this List<T> list, T target) where T : Object
		{
			int instanceID = target.GetInstanceID();
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].GetInstanceID() == instanceID)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x060047BF RID: 18367 RVA: 0x0014D849 File Offset: 0x0014BA49
		public static bool Compare(this Color32 a, Color32 b)
		{
			return a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
		}

		// Token: 0x060047C0 RID: 18368 RVA: 0x0014D885 File Offset: 0x0014BA85
		public static bool CompareRGB(this Color32 a, Color32 b)
		{
			return a.r == b.r && a.g == b.g && a.b == b.b;
		}

		// Token: 0x060047C1 RID: 18369 RVA: 0x0014D8B3 File Offset: 0x0014BAB3
		public static bool Compare(this Color a, Color b)
		{
			return a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
		}

		// Token: 0x060047C2 RID: 18370 RVA: 0x0014D8EF File Offset: 0x0014BAEF
		public static bool CompareRGB(this Color a, Color b)
		{
			return a.r == b.r && a.g == b.g && a.b == b.b;
		}

		// Token: 0x060047C3 RID: 18371 RVA: 0x0014D920 File Offset: 0x0014BB20
		public static Color32 Multiply(this Color32 c1, Color32 c2)
		{
			byte r = (byte)((float)c1.r / 255f * ((float)c2.r / 255f) * 255f);
			byte g = (byte)((float)c1.g / 255f * ((float)c2.g / 255f) * 255f);
			byte b = (byte)((float)c1.b / 255f * ((float)c2.b / 255f) * 255f);
			byte a = (byte)((float)c1.a / 255f * ((float)c2.a / 255f) * 255f);
			return new Color32(r, g, b, a);
		}

		// Token: 0x060047C4 RID: 18372 RVA: 0x0014D9C0 File Offset: 0x0014BBC0
		public static Color32 Tint(this Color32 c1, Color32 c2)
		{
			byte r = (byte)((float)c1.r / 255f * ((float)c2.r / 255f) * 255f);
			byte g = (byte)((float)c1.g / 255f * ((float)c2.g / 255f) * 255f);
			byte b = (byte)((float)c1.b / 255f * ((float)c2.b / 255f) * 255f);
			byte a = (byte)((float)c1.a / 255f * ((float)c2.a / 255f) * 255f);
			return new Color32(r, g, b, a);
		}

		// Token: 0x060047C5 RID: 18373 RVA: 0x0014DA60 File Offset: 0x0014BC60
		public static Color32 Tint(this Color32 c1, float tint)
		{
			byte r = (byte)Mathf.Clamp((float)c1.r / 255f * tint * 255f, 0f, 255f);
			byte g = (byte)Mathf.Clamp((float)c1.g / 255f * tint * 255f, 0f, 255f);
			byte b = (byte)Mathf.Clamp((float)c1.b / 255f * tint * 255f, 0f, 255f);
			byte a = (byte)Mathf.Clamp((float)c1.a / 255f * tint * 255f, 0f, 255f);
			return new Color32(r, g, b, a);
		}

		// Token: 0x060047C6 RID: 18374 RVA: 0x0014DB0C File Offset: 0x0014BD0C
		public static bool Compare(this Vector3 v1, Vector3 v2, int accuracy)
		{
			bool flag = (int)(v1.x * (float)accuracy) == (int)(v2.x * (float)accuracy);
			bool flag2 = (int)(v1.y * (float)accuracy) == (int)(v2.y * (float)accuracy);
			bool flag3 = (int)(v1.z * (float)accuracy) == (int)(v2.z * (float)accuracy);
			return flag && flag2 && flag3;
		}

		// Token: 0x060047C7 RID: 18375 RVA: 0x0014DB64 File Offset: 0x0014BD64
		public static bool Compare(this Quaternion q1, Quaternion q2, int accuracy)
		{
			bool flag = (int)(q1.x * (float)accuracy) == (int)(q2.x * (float)accuracy);
			bool flag2 = (int)(q1.y * (float)accuracy) == (int)(q2.y * (float)accuracy);
			bool flag3 = (int)(q1.z * (float)accuracy) == (int)(q2.z * (float)accuracy);
			bool flag4 = (int)(q1.w * (float)accuracy) == (int)(q2.w * (float)accuracy);
			return flag && flag2 && flag3 && flag4;
		}
	}
}
