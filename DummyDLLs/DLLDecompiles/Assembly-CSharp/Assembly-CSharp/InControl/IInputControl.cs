using System;

namespace InControl
{
	// Token: 0x020008F2 RID: 2290
	public interface IInputControl
	{
		// Token: 0x17000A78 RID: 2680
		// (get) Token: 0x06005031 RID: 20529
		bool HasChanged { get; }

		// Token: 0x17000A79 RID: 2681
		// (get) Token: 0x06005032 RID: 20530
		bool IsPressed { get; }

		// Token: 0x17000A7A RID: 2682
		// (get) Token: 0x06005033 RID: 20531
		bool WasPressed { get; }

		// Token: 0x17000A7B RID: 2683
		// (get) Token: 0x06005034 RID: 20532
		bool WasReleased { get; }

		// Token: 0x06005035 RID: 20533
		void ClearInputState();
	}
}
