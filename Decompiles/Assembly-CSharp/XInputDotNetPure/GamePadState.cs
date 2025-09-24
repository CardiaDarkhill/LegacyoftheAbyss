using System;

namespace XInputDotNetPure
{
	// Token: 0x020008D6 RID: 2262
	public struct GamePadState
	{
		// Token: 0x06004F04 RID: 20228 RVA: 0x0016F3AC File Offset: 0x0016D5AC
		internal GamePadState(bool isConnected, GamePadState.RawState rawState)
		{
			this.isConnected = isConnected;
			if (!isConnected)
			{
				rawState.dwPacketNumber = 0U;
				rawState.Gamepad.dwButtons = 0;
				rawState.Gamepad.bLeftTrigger = 0;
				rawState.Gamepad.bRightTrigger = 0;
				rawState.Gamepad.sThumbLX = 0;
				rawState.Gamepad.sThumbLY = 0;
				rawState.Gamepad.sThumbRX = 0;
				rawState.Gamepad.sThumbRY = 0;
			}
			this.packetNumber = rawState.dwPacketNumber;
			this.buttons = new GamePadButtons(((rawState.Gamepad.dwButtons & 16) != 0) ? ButtonState.Pressed : ButtonState.Released, ((rawState.Gamepad.dwButtons & 32) != 0) ? ButtonState.Pressed : ButtonState.Released, ((rawState.Gamepad.dwButtons & 64) != 0) ? ButtonState.Pressed : ButtonState.Released, ((rawState.Gamepad.dwButtons & 128) != 0) ? ButtonState.Pressed : ButtonState.Released, ((rawState.Gamepad.dwButtons & 256) != 0) ? ButtonState.Pressed : ButtonState.Released, ((rawState.Gamepad.dwButtons & 512) != 0) ? ButtonState.Pressed : ButtonState.Released, ((rawState.Gamepad.dwButtons & 4096) != 0) ? ButtonState.Pressed : ButtonState.Released, ((rawState.Gamepad.dwButtons & 8192) != 0) ? ButtonState.Pressed : ButtonState.Released, ((rawState.Gamepad.dwButtons & 16384) != 0) ? ButtonState.Pressed : ButtonState.Released, ((rawState.Gamepad.dwButtons & 32768) != 0) ? ButtonState.Pressed : ButtonState.Released);
			this.dPad = new GamePadDPad(((rawState.Gamepad.dwButtons & 1) != 0) ? ButtonState.Pressed : ButtonState.Released, ((rawState.Gamepad.dwButtons & 2) != 0) ? ButtonState.Pressed : ButtonState.Released, ((rawState.Gamepad.dwButtons & 4) != 0) ? ButtonState.Pressed : ButtonState.Released, ((rawState.Gamepad.dwButtons & 8) != 0) ? ButtonState.Pressed : ButtonState.Released);
			this.thumbSticks = new GamePadThumbSticks(new GamePadThumbSticks.StickValue((float)rawState.Gamepad.sThumbLX / 32767f, (float)rawState.Gamepad.sThumbLY / 32767f), new GamePadThumbSticks.StickValue((float)rawState.Gamepad.sThumbRX / 32767f, (float)rawState.Gamepad.sThumbRY / 32767f));
			this.triggers = new GamePadTriggers((float)rawState.Gamepad.bLeftTrigger / 255f, (float)rawState.Gamepad.bRightTrigger / 255f);
		}

		// Token: 0x17000A2D RID: 2605
		// (get) Token: 0x06004F05 RID: 20229 RVA: 0x0016F5FD File Offset: 0x0016D7FD
		public uint PacketNumber
		{
			get
			{
				return this.packetNumber;
			}
		}

		// Token: 0x17000A2E RID: 2606
		// (get) Token: 0x06004F06 RID: 20230 RVA: 0x0016F605 File Offset: 0x0016D805
		public bool IsConnected
		{
			get
			{
				return this.isConnected;
			}
		}

		// Token: 0x17000A2F RID: 2607
		// (get) Token: 0x06004F07 RID: 20231 RVA: 0x0016F60D File Offset: 0x0016D80D
		public GamePadButtons Buttons
		{
			get
			{
				return this.buttons;
			}
		}

		// Token: 0x17000A30 RID: 2608
		// (get) Token: 0x06004F08 RID: 20232 RVA: 0x0016F615 File Offset: 0x0016D815
		public GamePadDPad DPad
		{
			get
			{
				return this.dPad;
			}
		}

		// Token: 0x17000A31 RID: 2609
		// (get) Token: 0x06004F09 RID: 20233 RVA: 0x0016F61D File Offset: 0x0016D81D
		public GamePadTriggers Triggers
		{
			get
			{
				return this.triggers;
			}
		}

		// Token: 0x17000A32 RID: 2610
		// (get) Token: 0x06004F0A RID: 20234 RVA: 0x0016F625 File Offset: 0x0016D825
		public GamePadThumbSticks ThumbSticks
		{
			get
			{
				return this.thumbSticks;
			}
		}

		// Token: 0x04004F90 RID: 20368
		private bool isConnected;

		// Token: 0x04004F91 RID: 20369
		private uint packetNumber;

		// Token: 0x04004F92 RID: 20370
		private GamePadButtons buttons;

		// Token: 0x04004F93 RID: 20371
		private GamePadDPad dPad;

		// Token: 0x04004F94 RID: 20372
		private GamePadThumbSticks thumbSticks;

		// Token: 0x04004F95 RID: 20373
		private GamePadTriggers triggers;

		// Token: 0x02001B59 RID: 7001
		internal struct RawState
		{
			// Token: 0x04009C65 RID: 40037
			public uint dwPacketNumber;

			// Token: 0x04009C66 RID: 40038
			public GamePadState.RawState.GamePad Gamepad;

			// Token: 0x02001C39 RID: 7225
			public struct GamePad
			{
				// Token: 0x0400A04F RID: 41039
				public ushort dwButtons;

				// Token: 0x0400A050 RID: 41040
				public byte bLeftTrigger;

				// Token: 0x0400A051 RID: 41041
				public byte bRightTrigger;

				// Token: 0x0400A052 RID: 41042
				public short sThumbLX;

				// Token: 0x0400A053 RID: 41043
				public short sThumbLY;

				// Token: 0x0400A054 RID: 41044
				public short sThumbRX;

				// Token: 0x0400A055 RID: 41045
				public short sThumbRY;
			}
		}

		// Token: 0x02001B5A RID: 7002
		private enum ButtonsConstants
		{
			// Token: 0x04009C68 RID: 40040
			DPadUp = 1,
			// Token: 0x04009C69 RID: 40041
			DPadDown,
			// Token: 0x04009C6A RID: 40042
			DPadLeft = 4,
			// Token: 0x04009C6B RID: 40043
			DPadRight = 8,
			// Token: 0x04009C6C RID: 40044
			Start = 16,
			// Token: 0x04009C6D RID: 40045
			Back = 32,
			// Token: 0x04009C6E RID: 40046
			LeftThumb = 64,
			// Token: 0x04009C6F RID: 40047
			RightThumb = 128,
			// Token: 0x04009C70 RID: 40048
			LeftShoulder = 256,
			// Token: 0x04009C71 RID: 40049
			RightShoulder = 512,
			// Token: 0x04009C72 RID: 40050
			A = 4096,
			// Token: 0x04009C73 RID: 40051
			B = 8192,
			// Token: 0x04009C74 RID: 40052
			X = 16384,
			// Token: 0x04009C75 RID: 40053
			Y = 32768
		}
	}
}
