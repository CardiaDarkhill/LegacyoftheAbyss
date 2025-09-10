using System;
using UnityEngine;

namespace TeamCherry.PS5
{
	// Token: 0x020008A0 RID: 2208
	public sealed class GamePad
	{
		// Token: 0x06004C53 RID: 19539 RVA: 0x00167AB7 File Offset: 0x00165CB7
		public GamePad()
		{
			this.Swipes = new GamePad.SwipeInput(this);
		}

		// Token: 0x06004C54 RID: 19540 RVA: 0x00167AE8 File Offset: 0x00165CE8
		static GamePad()
		{
			GamePad.gamePads = new GamePad[4];
			for (int i = 0; i < GamePad.gamePads.Length; i++)
			{
				GamePad.gamePads[i] = new GamePad();
				GamePad.gamePads[i].playerId = i;
				GamePad.gamePads[i].refreshInterval = i + 10;
				GamePad.gamePads[i].RefreshUserDetails();
			}
			GamePad.activeGamePad = GamePad.gamePads[0];
		}

		// Token: 0x06004C55 RID: 19541 RVA: 0x00167B6C File Offset: 0x00165D6C
		public static void Initialize()
		{
			if (!GamePad.initialised)
			{
				GamePad.initialised = true;
				if (Platform.Current)
				{
					Platform.Current.gameObject.AddComponent<PlaystationGamePadManager>();
					return;
				}
				GameObject gameObject = new GameObject("Playstation Gamepads");
				Object.DontDestroyOnLoad(gameObject);
				gameObject.AddComponent<PlaystationGamePadManager>();
			}
		}

		// Token: 0x06004C56 RID: 19542 RVA: 0x00167BB9 File Offset: 0x00165DB9
		public static void EnableInput(bool enable)
		{
			if (enable != GamePad.enableInput)
			{
				GamePad.enableInput = enable;
				if (enable)
				{
					GamePad.timeout = 1f;
				}
			}
		}

		// Token: 0x06004C57 RID: 19543 RVA: 0x00167BD6 File Offset: 0x00165DD6
		public static GamePad GetGamepad(int playerID)
		{
			if (playerID < 0 || playerID > 4)
			{
				return null;
			}
			return GamePad.gamePads[playerID];
		}

		// Token: 0x170008FC RID: 2300
		// (get) Token: 0x06004C58 RID: 19544 RVA: 0x00167BE9 File Offset: 0x00165DE9
		public static bool IsInputEnabled
		{
			get
			{
				return GamePad.enableInput;
			}
		}

		// Token: 0x170008FD RID: 2301
		// (get) Token: 0x06004C59 RID: 19545 RVA: 0x00167BF0 File Offset: 0x00165DF0
		public bool IsSquarePressed
		{
			get
			{
				return !this.previousFrame.square && this.currentFrame.square;
			}
		}

		// Token: 0x170008FE RID: 2302
		// (get) Token: 0x06004C5A RID: 19546 RVA: 0x00167C0C File Offset: 0x00165E0C
		public bool IsCirclePressed
		{
			get
			{
				return !this.previousFrame.circle && this.currentFrame.circle;
			}
		}

		// Token: 0x170008FF RID: 2303
		// (get) Token: 0x06004C5B RID: 19547 RVA: 0x00167C28 File Offset: 0x00165E28
		public bool IsTrianglePressed
		{
			get
			{
				return !this.previousFrame.triangle && this.currentFrame.triangle;
			}
		}

		// Token: 0x17000900 RID: 2304
		// (get) Token: 0x06004C5C RID: 19548 RVA: 0x00167C44 File Offset: 0x00165E44
		public bool IsCrossPressed
		{
			get
			{
				return !this.previousFrame.cross && this.currentFrame.cross;
			}
		}

		// Token: 0x17000901 RID: 2305
		// (get) Token: 0x06004C5D RID: 19549 RVA: 0x00167C60 File Offset: 0x00165E60
		public bool IsDpadDownPressed
		{
			get
			{
				return !this.previousFrame.dpad_down && this.currentFrame.dpad_down;
			}
		}

		// Token: 0x17000902 RID: 2306
		// (get) Token: 0x06004C5E RID: 19550 RVA: 0x00167C7C File Offset: 0x00165E7C
		public bool IsDpadRightPressed
		{
			get
			{
				return !this.previousFrame.dpad_right && this.currentFrame.dpad_right;
			}
		}

		// Token: 0x17000903 RID: 2307
		// (get) Token: 0x06004C5F RID: 19551 RVA: 0x00167C98 File Offset: 0x00165E98
		public bool IsDpadUpPressed
		{
			get
			{
				return !this.previousFrame.dpad_up && this.currentFrame.dpad_up;
			}
		}

		// Token: 0x17000904 RID: 2308
		// (get) Token: 0x06004C60 RID: 19552 RVA: 0x00167CB4 File Offset: 0x00165EB4
		public bool IsDpadLeftPressed
		{
			get
			{
				return !this.previousFrame.dpad_left && this.currentFrame.dpad_left;
			}
		}

		// Token: 0x17000905 RID: 2309
		// (get) Token: 0x06004C61 RID: 19553 RVA: 0x00167CD0 File Offset: 0x00165ED0
		public bool IsR3Pressed
		{
			get
			{
				return !this.previousFrame.R3 && this.currentFrame.R3;
			}
		}

		// Token: 0x17000906 RID: 2310
		// (get) Token: 0x06004C62 RID: 19554 RVA: 0x00167CEC File Offset: 0x00165EEC
		public Vector2 GetThumbstickLeft
		{
			get
			{
				return this.currentFrame.thumbstick_left;
			}
		}

		// Token: 0x17000907 RID: 2311
		// (get) Token: 0x06004C63 RID: 19555 RVA: 0x00167CF9 File Offset: 0x00165EF9
		public Vector2 GetThumbstickRight
		{
			get
			{
				return this.currentFrame.thumbstick_right;
			}
		}

		// Token: 0x17000908 RID: 2312
		// (get) Token: 0x06004C64 RID: 19556 RVA: 0x00167D06 File Offset: 0x00165F06
		// (set) Token: 0x06004C65 RID: 19557 RVA: 0x00167D0E File Offset: 0x00165F0E
		public GamePad.SwipeInput Swipes { get; private set; }

		// Token: 0x17000909 RID: 2313
		// (get) Token: 0x06004C66 RID: 19558 RVA: 0x00167D17 File Offset: 0x00165F17
		public bool IsTouchpadPressed
		{
			get
			{
				return !this.previousFrame.touchpad && this.currentFrame.touchpad;
			}
		}

		// Token: 0x1700090A RID: 2314
		// (get) Token: 0x06004C67 RID: 19559 RVA: 0x00167D34 File Offset: 0x00165F34
		private bool AnyInput
		{
			get
			{
				return this.currentFrame.cross || this.currentFrame.circle || this.currentFrame.triangle || this.currentFrame.square || this.currentFrame.dpad_down || this.currentFrame.dpad_right || this.currentFrame.dpad_up || this.currentFrame.dpad_left || this.currentFrame.L1 || this.currentFrame.L2 || this.currentFrame.L3 || this.currentFrame.R1 || this.currentFrame.R2 || this.currentFrame.R3 || (this.currentFrame.options || this.currentFrame.touchpad) || this.currentFrame.touchNum > 0 || Vector2.SqrMagnitude(this.currentFrame.thumbstick_left) > 0f || Vector2.SqrMagnitude(this.currentFrame.thumbstick_right) > 0f;
			}
		}

		// Token: 0x1700090B RID: 2315
		// (get) Token: 0x06004C68 RID: 19560 RVA: 0x00167E64 File Offset: 0x00166064
		public bool IsConnected
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06004C69 RID: 19561 RVA: 0x00167E67 File Offset: 0x00166067
		public void InitGamepad()
		{
			this.ToggleGamePad(false);
			this.input = PSInputBase.GetPSInput();
			this.input.Init(this);
		}

		// Token: 0x06004C6A RID: 19562 RVA: 0x00167E87 File Offset: 0x00166087
		public void RefreshUserDetails()
		{
		}

		// Token: 0x06004C6B RID: 19563 RVA: 0x00167E8C File Offset: 0x0016608C
		public void Update()
		{
			if (GamePad.timeout > 0f)
			{
				GamePad.timeout -= Time.deltaTime;
			}
			if (!GamePad.enableInput || GamePad.timeout > 0f)
			{
				this.previousFrame = default(GamePad.PS4GamePad);
				this.currentFrame = default(GamePad.PS4GamePad);
				return;
			}
		}

		// Token: 0x06004C6C RID: 19564 RVA: 0x00167EE1 File Offset: 0x001660E1
		private void ToggleGamePad(bool active)
		{
			if (active)
			{
				this.RefreshUserDetails();
				this.hasSetupGamepad = true;
				return;
			}
			this.hasSetupGamepad = false;
		}

		// Token: 0x06004C6D RID: 19565 RVA: 0x00167EFC File Offset: 0x001660FC
		private void Thumbsticks()
		{
			this.currentFrame.thumbstick_left = this.input.GetThumbStickLeft();
			this.currentFrame.thumbstick_right = this.input.GetThumbStickRight();
			this.currentFrame.L3 = this.input.GetL3();
			this.currentFrame.R3 = this.input.GetR3();
		}

		// Token: 0x06004C6E RID: 19566 RVA: 0x00167F64 File Offset: 0x00166164
		private void InputButtons()
		{
			this.currentFrame.cross = this.input.GetCross();
			this.currentFrame.circle = this.input.GetCircle();
			this.currentFrame.square = this.input.GetSquare();
			this.currentFrame.triangle = this.input.GetTriangle();
		}

		// Token: 0x06004C6F RID: 19567 RVA: 0x00167FCC File Offset: 0x001661CC
		private void DPadButtons()
		{
			this.currentFrame.dpad_right = this.input.GetDpadRight();
			this.currentFrame.dpad_left = this.input.GetDpadLeft();
			this.currentFrame.dpad_up = this.input.GetDpadUp();
			this.currentFrame.dpad_down = this.input.GetDpadDown();
		}

		// Token: 0x06004C70 RID: 19568 RVA: 0x00168034 File Offset: 0x00166234
		private void TriggerShoulderButtons()
		{
			this.currentFrame.L2 = this.input.GetL2();
			this.currentFrame.R2 = this.input.GetR2();
			this.currentFrame.L1 = this.input.GetL1();
			this.currentFrame.R1 = this.input.GetR1();
		}

		// Token: 0x06004C71 RID: 19569 RVA: 0x0016809C File Offset: 0x0016629C
		private void TouchPad()
		{
			this.currentFrame.touchpad = this.input.TouchPadButton();
			this.currentFrame.isTouching = (this.currentFrame.touchNum > 0 || this.currentFrame.touchpad);
			if (this.previousFrame.touchNum == 0 && this.currentFrame.touchNum > 0)
			{
				this.touchStart = new Vector2((float)this.currentFrame.touch0X, (float)this.currentFrame.touch0Y);
			}
			this.up = false;
			this.right = false;
			this.down = false;
			this.left = false;
			if (this.previousFrame.touchNum > 0 && this.currentFrame.touchNum == 0)
			{
				Vector2 vector = new Vector2((float)this.previousFrame.touch0X, (float)this.previousFrame.touch0Y) - this.touchStart;
				if (vector.sqrMagnitude >= 90000f)
				{
					if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
					{
						if (vector.x < 0f)
						{
							this.left = true;
							return;
						}
						this.right = true;
						return;
					}
					else
					{
						if (vector.y < 0f)
						{
							this.up = true;
							return;
						}
						this.down = true;
					}
				}
			}
		}

		// Token: 0x04004DA6 RID: 19878
		public static GamePad activeGamePad = null;

		// Token: 0x04004DA7 RID: 19879
		public static GamePad[] gamePads;

		// Token: 0x04004DA8 RID: 19880
		private static bool initialised;

		// Token: 0x04004DA9 RID: 19881
		private static bool enableInput = true;

		// Token: 0x04004DAA RID: 19882
		private static float timeout = 0f;

		// Token: 0x04004DAB RID: 19883
		public int playerId = -1;

		// Token: 0x04004DAC RID: 19884
		public int refreshInterval = 10;

		// Token: 0x04004DAD RID: 19885
		private const float SWIPE_THRESHOLD = 300f;

		// Token: 0x04004DAE RID: 19886
		private const float SWIPE_THRESHOLD_SQR = 90000f;

		// Token: 0x04004DAF RID: 19887
		private bool up;

		// Token: 0x04004DB0 RID: 19888
		private bool right;

		// Token: 0x04004DB1 RID: 19889
		private bool down;

		// Token: 0x04004DB2 RID: 19890
		private bool left;

		// Token: 0x04004DB3 RID: 19891
		public Vector2 touchStart = Vector2.zero;

		// Token: 0x04004DB4 RID: 19892
		public GamePad.PS4GamePad previousFrame;

		// Token: 0x04004DB5 RID: 19893
		public GamePad.PS4GamePad currentFrame;

		// Token: 0x04004DB7 RID: 19895
		private bool hasSetupGamepad;

		// Token: 0x04004DB8 RID: 19896
		private PSInputBase input;

		// Token: 0x02001B03 RID: 6915
		public struct PS4GamePad
		{
			// Token: 0x04009B54 RID: 39764
			public Vector2 thumbstick_left;

			// Token: 0x04009B55 RID: 39765
			public Vector2 thumbstick_right;

			// Token: 0x04009B56 RID: 39766
			public bool cross;

			// Token: 0x04009B57 RID: 39767
			public bool circle;

			// Token: 0x04009B58 RID: 39768
			public bool triangle;

			// Token: 0x04009B59 RID: 39769
			public bool square;

			// Token: 0x04009B5A RID: 39770
			public bool dpad_down;

			// Token: 0x04009B5B RID: 39771
			public bool dpad_right;

			// Token: 0x04009B5C RID: 39772
			public bool dpad_up;

			// Token: 0x04009B5D RID: 39773
			public bool dpad_left;

			// Token: 0x04009B5E RID: 39774
			public bool L1;

			// Token: 0x04009B5F RID: 39775
			public bool L2;

			// Token: 0x04009B60 RID: 39776
			public bool L3;

			// Token: 0x04009B61 RID: 39777
			public bool R1;

			// Token: 0x04009B62 RID: 39778
			public bool R2;

			// Token: 0x04009B63 RID: 39779
			public bool R3;

			// Token: 0x04009B64 RID: 39780
			public bool options;

			// Token: 0x04009B65 RID: 39781
			public bool touchpad;

			// Token: 0x04009B66 RID: 39782
			public bool isTouching;

			// Token: 0x04009B67 RID: 39783
			public int touchNum;

			// Token: 0x04009B68 RID: 39784
			public int touch0X;

			// Token: 0x04009B69 RID: 39785
			public int touch0Y;

			// Token: 0x04009B6A RID: 39786
			public int touch0ID;

			// Token: 0x04009B6B RID: 39787
			public int touch1X;

			// Token: 0x04009B6C RID: 39788
			public int touch1Y;

			// Token: 0x04009B6D RID: 39789
			public int touch1ID;
		}

		// Token: 0x02001B04 RID: 6916
		public sealed class SwipeInput
		{
			// Token: 0x1700118C RID: 4492
			// (get) Token: 0x060098D3 RID: 39123 RVA: 0x002B01B1 File Offset: 0x002AE3B1
			public bool Up
			{
				get
				{
					return this.gamePad.up;
				}
			}

			// Token: 0x1700118D RID: 4493
			// (get) Token: 0x060098D4 RID: 39124 RVA: 0x002B01BE File Offset: 0x002AE3BE
			public bool Right
			{
				get
				{
					return this.gamePad.right;
				}
			}

			// Token: 0x1700118E RID: 4494
			// (get) Token: 0x060098D5 RID: 39125 RVA: 0x002B01CB File Offset: 0x002AE3CB
			public bool Down
			{
				get
				{
					return this.gamePad.down;
				}
			}

			// Token: 0x1700118F RID: 4495
			// (get) Token: 0x060098D6 RID: 39126 RVA: 0x002B01D8 File Offset: 0x002AE3D8
			public bool Left
			{
				get
				{
					return this.gamePad.left;
				}
			}

			// Token: 0x060098D7 RID: 39127 RVA: 0x002B01E5 File Offset: 0x002AE3E5
			public SwipeInput(GamePad gamePad)
			{
				this.gamePad = gamePad;
			}

			// Token: 0x04009B6E RID: 39790
			private readonly GamePad gamePad;
		}
	}
}
