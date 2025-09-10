using System;
using System.Collections.Generic;

namespace TMProOld
{
	// Token: 0x02000812 RID: 2066
	internal static class TMP_ListPool<T>
	{
		// Token: 0x060048EB RID: 18667 RVA: 0x0015451D File Offset: 0x0015271D
		public static List<T> Get()
		{
			return TMP_ListPool<T>.s_ListPool.Get();
		}

		// Token: 0x060048EC RID: 18668 RVA: 0x00154529 File Offset: 0x00152729
		public static void Release(List<T> toRelease)
		{
			TMP_ListPool<T>.s_ListPool.Release(toRelease);
		}

		// Token: 0x04004901 RID: 18689
		private static readonly TMP_ObjectPool<List<T>> s_ListPool = new TMP_ObjectPool<List<T>>(null, delegate(List<T> l)
		{
			l.Clear();
		});
	}
}
