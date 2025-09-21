using System;

namespace TeamCherry.GameCore
{
	// Token: 0x020008B5 RID: 2229
	public static class SafeExtension
	{
		// Token: 0x06004D40 RID: 19776 RVA: 0x0016B1F8 File Offset: 0x001693F8
		public static void SafeInvoke<T>(this Action<T> callback, T value)
		{
			if (callback != null)
			{
				CoreLoop.InvokeSafe(delegate
				{
					callback(value);
				});
			}
		}
	}
}
