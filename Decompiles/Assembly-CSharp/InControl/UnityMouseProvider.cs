using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace InControl
{
	// Token: 0x02000912 RID: 2322
	public class UnityMouseProvider : IMouseProvider
	{
		// Token: 0x06005250 RID: 21072 RVA: 0x0017882D File Offset: 0x00176A2D
		public void Setup()
		{
			this.ClearState();
		}

		// Token: 0x06005251 RID: 21073 RVA: 0x00178835 File Offset: 0x00176A35
		public void Reset()
		{
			this.ClearState();
		}

		// Token: 0x06005252 RID: 21074 RVA: 0x00178840 File Offset: 0x00176A40
		public void Update()
		{
			if (Input.mousePresent)
			{
				Array.Copy(this.buttonPressed, this.lastButtonPressed, this.buttonPressed.Length);
				this.buttonPressed[1] = UnityMouseProvider.SafeGetMouseButton(0);
				this.buttonPressed[2] = UnityMouseProvider.SafeGetMouseButton(1);
				this.buttonPressed[3] = UnityMouseProvider.SafeGetMouseButton(2);
				this.buttonPressed[10] = UnityMouseProvider.SafeGetMouseButton(3);
				this.buttonPressed[11] = UnityMouseProvider.SafeGetMouseButton(4);
				this.buttonPressed[12] = UnityMouseProvider.SafeGetMouseButton(5);
				this.buttonPressed[13] = UnityMouseProvider.SafeGetMouseButton(6);
				this.lastPosition = this.position;
				this.position = Input.mousePosition;
				this.delta = new Vector2(Input.GetAxisRaw("mouse x"), Input.GetAxisRaw("mouse y"));
				this.scroll = Input.mouseScrollDelta;
				return;
			}
			this.ClearState();
		}

		// Token: 0x06005253 RID: 21075 RVA: 0x00178924 File Offset: 0x00176B24
		private static bool SafeGetMouseButton(int button)
		{
			if (button < UnityMouseProvider.maxSafeMouseButton)
			{
				try
				{
					return Input.GetMouseButton(button);
				}
				catch (ArgumentException)
				{
					UnityMouseProvider.maxSafeMouseButton = Mathf.Min(button, UnityMouseProvider.maxSafeMouseButton);
				}
				return false;
			}
			return false;
		}

		// Token: 0x06005254 RID: 21076 RVA: 0x00178968 File Offset: 0x00176B68
		private void ClearState()
		{
			Array.Clear(this.lastButtonPressed, 0, this.lastButtonPressed.Length);
			Array.Clear(this.buttonPressed, 0, this.buttonPressed.Length);
			this.lastPosition = Vector2.zero;
			this.position = Vector2.zero;
			this.delta = Vector2.zero;
			this.scroll = Vector2.zero;
		}

		// Token: 0x06005255 RID: 21077 RVA: 0x001789C9 File Offset: 0x00176BC9
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2 GetPosition()
		{
			return this.position;
		}

		// Token: 0x06005256 RID: 21078 RVA: 0x001789D1 File Offset: 0x00176BD1
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetDeltaX()
		{
			return this.delta.x;
		}

		// Token: 0x06005257 RID: 21079 RVA: 0x001789DE File Offset: 0x00176BDE
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetDeltaY()
		{
			return this.delta.y;
		}

		// Token: 0x06005258 RID: 21080 RVA: 0x001789EB File Offset: 0x00176BEB
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetDeltaScroll()
		{
			if (Utility.Abs(this.scroll.x) <= Utility.Abs(this.scroll.y))
			{
				return this.scroll.y;
			}
			return this.scroll.x;
		}

		// Token: 0x06005259 RID: 21081 RVA: 0x00178A26 File Offset: 0x00176C26
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool GetButtonIsPressed(Mouse control)
		{
			return this.buttonPressed[(int)control];
		}

		// Token: 0x0600525A RID: 21082 RVA: 0x00178A30 File Offset: 0x00176C30
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool GetButtonWasPressed(Mouse control)
		{
			return this.buttonPressed[(int)control] && !this.lastButtonPressed[(int)control];
		}

		// Token: 0x0600525B RID: 21083 RVA: 0x00178A49 File Offset: 0x00176C49
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool GetButtonWasReleased(Mouse control)
		{
			return !this.buttonPressed[(int)control] && this.lastButtonPressed[(int)control];
		}

		// Token: 0x0600525C RID: 21084 RVA: 0x00178A5F File Offset: 0x00176C5F
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasMousePresent()
		{
			return Input.mousePresent;
		}

		// Token: 0x04005291 RID: 21137
		private const string mouseXAxis = "mouse x";

		// Token: 0x04005292 RID: 21138
		private const string mouseYAxis = "mouse y";

		// Token: 0x04005293 RID: 21139
		private readonly bool[] lastButtonPressed = new bool[16];

		// Token: 0x04005294 RID: 21140
		private readonly bool[] buttonPressed = new bool[16];

		// Token: 0x04005295 RID: 21141
		private Vector2 lastPosition;

		// Token: 0x04005296 RID: 21142
		private Vector2 position;

		// Token: 0x04005297 RID: 21143
		private Vector2 delta;

		// Token: 0x04005298 RID: 21144
		private Vector2 scroll;

		// Token: 0x04005299 RID: 21145
		private static int maxSafeMouseButton = int.MaxValue;
	}
}
