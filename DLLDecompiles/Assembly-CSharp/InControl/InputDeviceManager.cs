using System;
using System.Collections.Generic;

namespace InControl
{
	// Token: 0x02000905 RID: 2309
	public abstract class InputDeviceManager
	{
		// Token: 0x0600515C RID: 20828
		public abstract void Update(ulong updateTick, float deltaTime);

		// Token: 0x0600515D RID: 20829 RVA: 0x001756AB File Offset: 0x001738AB
		public virtual void Destroy()
		{
		}

		// Token: 0x0400520D RID: 21005
		protected readonly List<InputDevice> devices = new List<InputDevice>();
	}
}
