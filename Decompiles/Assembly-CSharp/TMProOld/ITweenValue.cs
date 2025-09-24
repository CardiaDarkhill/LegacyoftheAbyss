using System;

namespace TMProOld
{
	// Token: 0x02000807 RID: 2055
	internal interface ITweenValue
	{
		// Token: 0x060047EA RID: 18410
		void TweenValue(float floatPercentage);

		// Token: 0x17000826 RID: 2086
		// (get) Token: 0x060047EB RID: 18411
		bool ignoreTimeScale { get; }

		// Token: 0x17000827 RID: 2087
		// (get) Token: 0x060047EC RID: 18412
		float duration { get; }

		// Token: 0x060047ED RID: 18413
		bool ValidTarget();
	}
}
