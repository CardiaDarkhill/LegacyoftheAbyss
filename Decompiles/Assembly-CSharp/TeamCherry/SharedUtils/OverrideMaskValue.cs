using System;

namespace TeamCherry.SharedUtils
{
	// Token: 0x020008C0 RID: 2240
	[Serializable]
	public class OverrideMaskValue<T> : OverrideMaskValueBase where T : Enum
	{
		// Token: 0x04004E4B RID: 20043
		public bool IsEnabled;

		// Token: 0x04004E4C RID: 20044
		public T Value;
	}
}
