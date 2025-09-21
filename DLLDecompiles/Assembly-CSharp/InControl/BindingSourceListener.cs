using System;

namespace InControl
{
	// Token: 0x020008DB RID: 2267
	public interface BindingSourceListener
	{
		// Token: 0x06004F25 RID: 20261
		void Reset();

		// Token: 0x06004F26 RID: 20262
		BindingSource Listen(BindingListenOptions listenOptions, InputDevice device);
	}
}
