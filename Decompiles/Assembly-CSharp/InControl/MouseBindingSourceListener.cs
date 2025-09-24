using System;

namespace InControl
{
	// Token: 0x020008E6 RID: 2278
	public class MouseBindingSourceListener : BindingSourceListener
	{
		// Token: 0x06004F82 RID: 20354 RVA: 0x0017048E File Offset: 0x0016E68E
		public void Reset()
		{
			this.detectFound = Mouse.None;
			this.detectPhase = 0;
		}

		// Token: 0x06004F83 RID: 20355 RVA: 0x001704A0 File Offset: 0x0016E6A0
		public BindingSource Listen(BindingListenOptions listenOptions, InputDevice device)
		{
			if (this.detectFound != Mouse.None && !this.IsPressed(this.detectFound) && this.detectPhase == 2)
			{
				BindingSource result = new MouseBindingSource(this.detectFound);
				this.Reset();
				return result;
			}
			Mouse mouse = this.ListenForControl(listenOptions);
			if (mouse != Mouse.None)
			{
				if (this.detectPhase == 1)
				{
					this.detectFound = mouse;
					this.detectPhase = 2;
				}
			}
			else if (this.detectPhase == 0)
			{
				this.detectPhase = 1;
			}
			return null;
		}

		// Token: 0x06004F84 RID: 20356 RVA: 0x00170512 File Offset: 0x0016E712
		private bool IsPressed(Mouse control)
		{
			if (control == Mouse.PositiveScrollWheel)
			{
				return MouseBindingSource.PositiveScrollWheelIsActive(MouseBindingSourceListener.ScrollWheelThreshold);
			}
			if (control == Mouse.NegativeScrollWheel)
			{
				return MouseBindingSource.NegativeScrollWheelIsActive(MouseBindingSourceListener.ScrollWheelThreshold);
			}
			return MouseBindingSource.ButtonIsPressed(control);
		}

		// Token: 0x06004F85 RID: 20357 RVA: 0x0017053C File Offset: 0x0016E73C
		private Mouse ListenForControl(BindingListenOptions listenOptions)
		{
			if (listenOptions.IncludeMouseButtons)
			{
				for (Mouse mouse = Mouse.None; mouse <= Mouse.Button7; mouse++)
				{
					if (MouseBindingSource.ButtonIsPressed(mouse))
					{
						return mouse;
					}
				}
			}
			if (listenOptions.IncludeMouseScrollWheel)
			{
				if (MouseBindingSource.NegativeScrollWheelIsActive(MouseBindingSourceListener.ScrollWheelThreshold))
				{
					return Mouse.NegativeScrollWheel;
				}
				if (MouseBindingSource.PositiveScrollWheelIsActive(MouseBindingSourceListener.ScrollWheelThreshold))
				{
					return Mouse.PositiveScrollWheel;
				}
			}
			return Mouse.None;
		}

		// Token: 0x0400505F RID: 20575
		public static float ScrollWheelThreshold = 0.001f;

		// Token: 0x04005060 RID: 20576
		private Mouse detectFound;

		// Token: 0x04005061 RID: 20577
		private int detectPhase;
	}
}
