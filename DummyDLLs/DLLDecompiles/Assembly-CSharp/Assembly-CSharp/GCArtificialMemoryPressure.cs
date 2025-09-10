using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000765 RID: 1893
public static class GCArtificialMemoryPressure
{
	// Token: 0x17000791 RID: 1937
	// (get) Token: 0x06004374 RID: 17268 RVA: 0x00128CE1 File Offset: 0x00126EE1
	public static long AllocatedBytes
	{
		get
		{
			return (long)(GCArtificialMemoryPressure.allocated1KBChunks.Count * 1024);
		}
	}

	// Token: 0x06004375 RID: 17269 RVA: 0x00128CF4 File Offset: 0x00126EF4
	public static void ClearAllocatedMemory()
	{
		GCArtificialMemoryPressure.allocated1KBChunks.Clear();
	}

	// Token: 0x06004376 RID: 17270 RVA: 0x00128D00 File Offset: 0x00126F00
	public static void IncreaseGCPressure(float t)
	{
		if (t < 0f || t > 1f)
		{
			throw new InvalidOperationException("Invalid argument");
		}
		long bytesToThreshold = GCArtificialMemoryPressure.GetBytesToThreshold();
		if (bytesToThreshold < 0L)
		{
			Debug.LogWarning("Memory usage exceeds the threshold.");
			return;
		}
		GCArtificialMemoryPressure.Allocate((long)((double)bytesToThreshold * (double)t));
	}

	// Token: 0x06004377 RID: 17271 RVA: 0x00128D49 File Offset: 0x00126F49
	public static void DecreaseGCPressure(float t)
	{
		if (t < 0f || t > 1f)
		{
			throw new InvalidOperationException("Invalid argument");
		}
		GCArtificialMemoryPressure.Free((long)((double)GCArtificialMemoryPressure.AllocatedBytes * (double)t));
	}

	// Token: 0x06004378 RID: 17272 RVA: 0x00128D78 File Offset: 0x00126F78
	public static long GetBytesToThreshold()
	{
		long num = (long)GCManager.HeapUsageThreshold * 1024L * 1024L;
		long monoHeapUsage = GCManager.GetMonoHeapUsage();
		return num - monoHeapUsage;
	}

	// Token: 0x06004379 RID: 17273 RVA: 0x00128DA4 File Offset: 0x00126FA4
	public static void Free(long bytesCount)
	{
		bytesCount -= bytesCount % 1024L;
		long num = bytesCount / 1024L;
		if (num > (long)GCArtificialMemoryPressure.allocated1KBChunks.Count)
		{
			GCArtificialMemoryPressure.allocated1KBChunks.Clear();
			return;
		}
		GCArtificialMemoryPressure.allocated1KBChunks.RemoveRange(0, (int)num);
	}

	// Token: 0x0600437A RID: 17274 RVA: 0x00128DEC File Offset: 0x00126FEC
	public static void Allocate(long bytesCount)
	{
		bytesCount -= bytesCount % 1024L;
		long num = bytesCount / 1024L;
		int num2 = 0;
		while ((long)num2 < num)
		{
			GCArtificialMemoryPressure.Allocate1KB();
			num2++;
		}
	}

	// Token: 0x0600437B RID: 17275 RVA: 0x00128E20 File Offset: 0x00127020
	public static void Allocate1KB()
	{
		byte[] item = new byte[1008];
		GCArtificialMemoryPressure.allocated1KBChunks.Add(item);
	}

	// Token: 0x17000792 RID: 1938
	// (get) Token: 0x0600437C RID: 17276 RVA: 0x00128E44 File Offset: 0x00127044
	public static long FreeBytesCount
	{
		get
		{
			long num = (long)GCManager.HeapUsageThreshold * 1024L * 1024L;
			long monoHeapUsage = GCManager.GetMonoHeapUsage();
			return num - monoHeapUsage;
		}
	}

	// Token: 0x17000793 RID: 1939
	// (get) Token: 0x0600437D RID: 17277 RVA: 0x00128E6D File Offset: 0x0012706D
	public static long MaxUsageThreshold
	{
		get
		{
			return (long)GCManager.HeapUsageThreshold * 1024L * 1024L;
		}
	}

	// Token: 0x04004517 RID: 17687
	public static List<byte[]> allocated1KBChunks = new List<byte[]>();
}
