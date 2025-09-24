using System;

namespace InControl
{
	// Token: 0x020008E2 RID: 2274
	public class KeyBindingSourceListener : BindingSourceListener
	{
		// Token: 0x06004F4F RID: 20303 RVA: 0x0016FC38 File Offset: 0x0016DE38
		public void Reset()
		{
			this.detectFound.Clear();
			this.detectPhase = 0;
		}

		// Token: 0x06004F50 RID: 20304 RVA: 0x0016FC4C File Offset: 0x0016DE4C
		public BindingSource Listen(BindingListenOptions listenOptions, InputDevice device)
		{
			if (!listenOptions.IncludeKeys)
			{
				return null;
			}
			if (this.detectFound.IncludeCount > 0 && !this.detectFound.IsPressed && this.detectPhase == 2)
			{
				BindingSource result = new KeyBindingSource(this.detectFound);
				this.Reset();
				return result;
			}
			KeyCombo keyCombo = KeyCombo.Detect(listenOptions.IncludeModifiersAsFirstClassKeys);
			if (keyCombo.IncludeCount > 0)
			{
				if (this.detectPhase == 1)
				{
					this.detectFound = keyCombo;
					this.detectPhase = 2;
				}
			}
			else if (this.detectPhase == 0)
			{
				this.detectPhase = 1;
			}
			return null;
		}

		// Token: 0x04005040 RID: 20544
		private KeyCombo detectFound;

		// Token: 0x04005041 RID: 20545
		private int detectPhase;
	}
}
