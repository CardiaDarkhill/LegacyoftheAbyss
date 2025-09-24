using System;
using InControl;
using UnityEngine;

namespace TeamCherry.PS5
{
	// Token: 0x020008A2 RID: 2210
	public sealed class PS5InControlInput : PSInputBase
	{
		// Token: 0x06004C75 RID: 19573 RVA: 0x00168249 File Offset: 0x00166449
		public override void Init(GamePad gamePad)
		{
		}

		// Token: 0x06004C76 RID: 19574 RVA: 0x0016824B File Offset: 0x0016644B
		public override Vector2 GetThumbStickLeft()
		{
			return InputManager.ActiveDevice.LeftStick.Value;
		}

		// Token: 0x06004C77 RID: 19575 RVA: 0x0016825C File Offset: 0x0016645C
		public override Vector2 GetThumbStickRight()
		{
			return InputManager.ActiveDevice.RightStick.Value;
		}

		// Token: 0x06004C78 RID: 19576 RVA: 0x0016826D File Offset: 0x0016646D
		public override bool GetL3()
		{
			return InputManager.ActiveDevice.LeftStick.IsPressed;
		}

		// Token: 0x06004C79 RID: 19577 RVA: 0x0016827E File Offset: 0x0016647E
		public override bool GetR3()
		{
			return InputManager.ActiveDevice.RightStick.IsPressed;
		}

		// Token: 0x06004C7A RID: 19578 RVA: 0x0016828F File Offset: 0x0016648F
		public override bool GetOptions()
		{
			return InputManager.ActiveDevice.GetControl(InputControlType.Options).IsPressed;
		}

		// Token: 0x06004C7B RID: 19579 RVA: 0x001682A2 File Offset: 0x001664A2
		public override bool GetCross()
		{
			return InputManager.ActiveDevice.Action1.IsPressed;
		}

		// Token: 0x06004C7C RID: 19580 RVA: 0x001682B3 File Offset: 0x001664B3
		public override bool GetCircle()
		{
			return InputManager.ActiveDevice.Action2.IsPressed;
		}

		// Token: 0x06004C7D RID: 19581 RVA: 0x001682C4 File Offset: 0x001664C4
		public override bool GetSquare()
		{
			return InputManager.ActiveDevice.Action3.IsPressed;
		}

		// Token: 0x06004C7E RID: 19582 RVA: 0x001682D5 File Offset: 0x001664D5
		public override bool GetTriangle()
		{
			return InputManager.ActiveDevice.Action4.IsPressed;
		}

		// Token: 0x06004C7F RID: 19583 RVA: 0x001682E6 File Offset: 0x001664E6
		public override bool GetDpadRight()
		{
			return InputManager.ActiveDevice.DPadRight.IsPressed;
		}

		// Token: 0x06004C80 RID: 19584 RVA: 0x001682F7 File Offset: 0x001664F7
		public override bool GetDpadLeft()
		{
			return InputManager.ActiveDevice.DPadLeft.IsPressed;
		}

		// Token: 0x06004C81 RID: 19585 RVA: 0x00168308 File Offset: 0x00166508
		public override bool GetDpadUp()
		{
			return InputManager.ActiveDevice.DPadUp.IsPressed;
		}

		// Token: 0x06004C82 RID: 19586 RVA: 0x00168319 File Offset: 0x00166519
		public override bool GetDpadDown()
		{
			return InputManager.ActiveDevice.DPadDown.IsPressed;
		}

		// Token: 0x06004C83 RID: 19587 RVA: 0x0016832A File Offset: 0x0016652A
		public override bool GetR1()
		{
			return InputManager.ActiveDevice.RightBumper.IsPressed;
		}

		// Token: 0x06004C84 RID: 19588 RVA: 0x0016833B File Offset: 0x0016653B
		public override bool GetR2()
		{
			return InputManager.ActiveDevice.RightTrigger.IsPressed;
		}

		// Token: 0x06004C85 RID: 19589 RVA: 0x0016834C File Offset: 0x0016654C
		public override bool GetL1()
		{
			return InputManager.ActiveDevice.LeftBumper.IsPressed;
		}

		// Token: 0x06004C86 RID: 19590 RVA: 0x0016835D File Offset: 0x0016655D
		public override bool GetL2()
		{
			return InputManager.ActiveDevice.LeftTrigger.IsPressed;
		}

		// Token: 0x06004C87 RID: 19591 RVA: 0x0016836E File Offset: 0x0016656E
		public override bool TouchPadButton()
		{
			return InputManager.ActiveDevice.GetControl(InputControlType.TouchPadButton).IsPressed;
		}
	}
}
