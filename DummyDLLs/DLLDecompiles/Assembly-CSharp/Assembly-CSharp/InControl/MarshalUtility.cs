using System;
using System.Runtime.InteropServices;

namespace InControl
{
	// Token: 0x02000937 RID: 2359
	public static class MarshalUtility
	{
		// Token: 0x060053A7 RID: 21415 RVA: 0x0017F0F7 File Offset: 0x0017D2F7
		public static void Copy(IntPtr source, uint[] destination, int length)
		{
			Utility.ArrayExpand<int>(ref MarshalUtility.buffer, length);
			Marshal.Copy(source, MarshalUtility.buffer, 0, length);
			Buffer.BlockCopy(MarshalUtility.buffer, 0, destination, 0, 4 * length);
		}

		// Token: 0x0400538D RID: 21389
		private static int[] buffer = new int[32];
	}
}
