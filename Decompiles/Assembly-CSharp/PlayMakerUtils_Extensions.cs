using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000040 RID: 64
public static class PlayMakerUtils_Extensions
{
	// Token: 0x060001C8 RID: 456 RVA: 0x00009573 File Offset: 0x00007773
	public static int IndexOf(ArrayList target, object value)
	{
		return PlayMakerUtils_Extensions.IndexOf(target, value, 0, target.Count);
	}

	// Token: 0x060001C9 RID: 457 RVA: 0x00009583 File Offset: 0x00007783
	public static int IndexOf(ArrayList target, object value, int startIndex)
	{
		if (startIndex > target.Count)
		{
			throw new ArgumentOutOfRangeException("startIndex", "ArgumentOutOfRange_Index");
		}
		return PlayMakerUtils_Extensions.IndexOf(target, value, startIndex, target.Count - startIndex);
	}

	// Token: 0x060001CA RID: 458 RVA: 0x000095B0 File Offset: 0x000077B0
	public static int IndexOf(ArrayList target, object value, int startIndex, int count)
	{
		Debug.Log(startIndex.ToString() + " " + count.ToString());
		if (startIndex < 0 || startIndex >= target.Count)
		{
			throw new ArgumentOutOfRangeException("startIndex", "ArgumentOutOfRange_Index");
		}
		if (count < 0 || startIndex > target.Count - count)
		{
			throw new ArgumentOutOfRangeException("count", "ArgumentOutOfRange_Count");
		}
		if (target.Count == 0)
		{
			return -1;
		}
		int num = startIndex + count;
		if (value == null)
		{
			for (int i = startIndex; i < num; i++)
			{
				if (target[i] == null)
				{
					return i;
				}
			}
			return -1;
		}
		for (int j = startIndex; j < num; j++)
		{
			if (target[j] != null && target[j].Equals(value))
			{
				return j;
			}
		}
		return -1;
	}

	// Token: 0x060001CB RID: 459 RVA: 0x00009667 File Offset: 0x00007867
	public static int LastIndexOf(ArrayList target, object value)
	{
		return PlayMakerUtils_Extensions.LastIndexOf(target, value, target.Count - 1, target.Count);
	}

	// Token: 0x060001CC RID: 460 RVA: 0x0000967E File Offset: 0x0000787E
	public static int LastIndexOf(ArrayList target, object value, int startIndex)
	{
		return PlayMakerUtils_Extensions.LastIndexOf(target, value, startIndex, startIndex + 1);
	}

	// Token: 0x060001CD RID: 461 RVA: 0x0000968C File Offset: 0x0000788C
	public static int LastIndexOf(ArrayList target, object value, int startIndex, int count)
	{
		if (target.Count == 0)
		{
			return -1;
		}
		if (startIndex < 0 || startIndex >= target.Count)
		{
			throw new ArgumentOutOfRangeException("startIndex", "ArgumentOutOfRange_Index");
		}
		if (count < 0 || startIndex > target.Count - count)
		{
			throw new ArgumentOutOfRangeException("count", "ArgumentOutOfRange_Count");
		}
		int num = startIndex + count - 1;
		if (value == null)
		{
			for (int i = num; i >= startIndex; i--)
			{
				if (target[i] == null)
				{
					return i;
				}
			}
			return -1;
		}
		for (int j = num; j >= startIndex; j--)
		{
			if (target[j] != null && target[j].Equals(value))
			{
				return j;
			}
		}
		return -1;
	}
}
