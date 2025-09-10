using System;

namespace TMProOld
{
	// Token: 0x020007EE RID: 2030
	public static class TMP_Math
	{
		// Token: 0x060047C8 RID: 18376 RVA: 0x0014DBD2 File Offset: 0x0014BDD2
		public static bool Approximately(float a, float b)
		{
			return b - 0.0001f < a && a < b + 0.0001f;
		}

		// Token: 0x04004790 RID: 18320
		public const float FLOAT_MAX = 32768f;

		// Token: 0x04004791 RID: 18321
		public const float FLOAT_MIN = -32768f;

		// Token: 0x04004792 RID: 18322
		public const int INT_MAX = 2147483647;

		// Token: 0x04004793 RID: 18323
		public const int INT_MIN = -2147483647;
	}
}
