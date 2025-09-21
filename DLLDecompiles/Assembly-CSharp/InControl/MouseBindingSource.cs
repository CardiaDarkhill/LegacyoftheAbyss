using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace InControl
{
	// Token: 0x020008E5 RID: 2277
	public class MouseBindingSource : BindingSource
	{
		// Token: 0x17000A4B RID: 2635
		// (get) Token: 0x06004F6D RID: 20333 RVA: 0x001701F3 File Offset: 0x0016E3F3
		// (set) Token: 0x06004F6E RID: 20334 RVA: 0x001701FB File Offset: 0x0016E3FB
		public Mouse Control { get; protected set; }

		// Token: 0x06004F6F RID: 20335 RVA: 0x00170204 File Offset: 0x0016E404
		internal MouseBindingSource()
		{
		}

		// Token: 0x06004F70 RID: 20336 RVA: 0x0017020C File Offset: 0x0016E40C
		public MouseBindingSource(Mouse mouseControl)
		{
			this.Control = mouseControl;
		}

		// Token: 0x06004F71 RID: 20337 RVA: 0x0017021B File Offset: 0x0016E41B
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool ButtonIsPressed(Mouse control)
		{
			return InputManager.MouseProvider.GetButtonIsPressed(control);
		}

		// Token: 0x06004F72 RID: 20338 RVA: 0x00170228 File Offset: 0x0016E428
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool NegativeScrollWheelIsActive(float threshold)
		{
			return Mathf.Min(InputManager.MouseProvider.GetDeltaScroll() * MouseBindingSource.ScaleZ, 0f) < -threshold;
		}

		// Token: 0x06004F73 RID: 20339 RVA: 0x00170248 File Offset: 0x0016E448
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool PositiveScrollWheelIsActive(float threshold)
		{
			return Mathf.Max(0f, InputManager.MouseProvider.GetDeltaScroll() * MouseBindingSource.ScaleZ) > threshold;
		}

		// Token: 0x06004F74 RID: 20340 RVA: 0x00170268 File Offset: 0x0016E468
		internal static float GetValue(Mouse mouseControl)
		{
			switch (mouseControl)
			{
			case Mouse.None:
				return 0f;
			case Mouse.NegativeX:
				return -Mathf.Min(InputManager.MouseProvider.GetDeltaX() * MouseBindingSource.ScaleX, 0f);
			case Mouse.PositiveX:
				return Mathf.Max(0f, InputManager.MouseProvider.GetDeltaX() * MouseBindingSource.ScaleX);
			case Mouse.NegativeY:
				return -Mathf.Min(InputManager.MouseProvider.GetDeltaY() * MouseBindingSource.ScaleY, 0f);
			case Mouse.PositiveY:
				return Mathf.Max(0f, InputManager.MouseProvider.GetDeltaY() * MouseBindingSource.ScaleY);
			case Mouse.PositiveScrollWheel:
				return Mathf.Max(0f, InputManager.MouseProvider.GetDeltaScroll() * MouseBindingSource.ScaleZ);
			case Mouse.NegativeScrollWheel:
				return -Mathf.Min(InputManager.MouseProvider.GetDeltaScroll() * MouseBindingSource.ScaleZ, 0f);
			}
			if (!InputManager.MouseProvider.GetButtonIsPressed(mouseControl))
			{
				return 0f;
			}
			return 1f;
		}

		// Token: 0x06004F75 RID: 20341 RVA: 0x0017036B File Offset: 0x0016E56B
		public override float GetValue(InputDevice inputDevice)
		{
			return MouseBindingSource.GetValue(this.Control);
		}

		// Token: 0x06004F76 RID: 20342 RVA: 0x00170378 File Offset: 0x0016E578
		public override bool GetState(InputDevice inputDevice)
		{
			return Utility.IsNotZero(this.GetValue(inputDevice));
		}

		// Token: 0x17000A4C RID: 2636
		// (get) Token: 0x06004F77 RID: 20343 RVA: 0x00170388 File Offset: 0x0016E588
		public override string Name
		{
			get
			{
				return this.Control.ToString();
			}
		}

		// Token: 0x17000A4D RID: 2637
		// (get) Token: 0x06004F78 RID: 20344 RVA: 0x001703A9 File Offset: 0x0016E5A9
		public override string DeviceName
		{
			get
			{
				return "Mouse";
			}
		}

		// Token: 0x17000A4E RID: 2638
		// (get) Token: 0x06004F79 RID: 20345 RVA: 0x001703B0 File Offset: 0x0016E5B0
		public override InputDeviceClass DeviceClass
		{
			get
			{
				return InputDeviceClass.Mouse;
			}
		}

		// Token: 0x17000A4F RID: 2639
		// (get) Token: 0x06004F7A RID: 20346 RVA: 0x001703B3 File Offset: 0x0016E5B3
		public override InputDeviceStyle DeviceStyle
		{
			get
			{
				return InputDeviceStyle.Unknown;
			}
		}

		// Token: 0x06004F7B RID: 20347 RVA: 0x001703B8 File Offset: 0x0016E5B8
		public override bool Equals(BindingSource other)
		{
			if (other == null)
			{
				return false;
			}
			MouseBindingSource mouseBindingSource = other as MouseBindingSource;
			return mouseBindingSource != null && this.Control == mouseBindingSource.Control;
		}

		// Token: 0x06004F7C RID: 20348 RVA: 0x001703F0 File Offset: 0x0016E5F0
		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			MouseBindingSource mouseBindingSource = other as MouseBindingSource;
			return mouseBindingSource != null && this.Control == mouseBindingSource.Control;
		}

		// Token: 0x06004F7D RID: 20349 RVA: 0x00170424 File Offset: 0x0016E624
		public override int GetHashCode()
		{
			return this.Control.GetHashCode();
		}

		// Token: 0x17000A50 RID: 2640
		// (get) Token: 0x06004F7E RID: 20350 RVA: 0x00170445 File Offset: 0x0016E645
		public override BindingSourceType BindingSourceType
		{
			get
			{
				return BindingSourceType.MouseBindingSource;
			}
		}

		// Token: 0x06004F7F RID: 20351 RVA: 0x00170448 File Offset: 0x0016E648
		public override void Save(BinaryWriter writer)
		{
			writer.Write((int)this.Control);
		}

		// Token: 0x06004F80 RID: 20352 RVA: 0x00170456 File Offset: 0x0016E656
		public override void Load(BinaryReader reader, ushort dataFormatVersion)
		{
			this.Control = (Mouse)reader.ReadInt32();
		}

		// Token: 0x0400505B RID: 20571
		public static float ScaleX = 0.05f;

		// Token: 0x0400505C RID: 20572
		public static float ScaleY = 0.05f;

		// Token: 0x0400505D RID: 20573
		public static float ScaleZ = 0.05f;

		// Token: 0x0400505E RID: 20574
		public static float JitterThreshold = 0.05f;
	}
}
