using System;
using UnityEngine;

namespace InControl
{
	// Token: 0x02000927 RID: 2343
	public class Touch
	{
		// Token: 0x06005307 RID: 21255 RVA: 0x0017C11A File Offset: 0x0017A31A
		internal Touch()
		{
			this.fingerId = -1;
			this.phase = TouchPhase.Ended;
		}

		// Token: 0x06005308 RID: 21256 RVA: 0x0017C130 File Offset: 0x0017A330
		internal void Reset()
		{
			this.fingerId = -1;
			this.mouseButton = 0;
			this.phase = TouchPhase.Ended;
			this.tapCount = 0;
			this.position = Vector2.zero;
			this.startPosition = Vector2.zero;
			this.deltaPosition = Vector2.zero;
			this.lastPosition = Vector2.zero;
			this.deltaTime = 0f;
			this.updateTick = 0UL;
			this.type = TouchType.Direct;
			this.altitudeAngle = 0f;
			this.azimuthAngle = 0f;
			this.maximumPossiblePressure = 1f;
			this.pressure = 0f;
			this.radius = 0f;
			this.radiusVariance = 0f;
		}

		// Token: 0x17000B60 RID: 2912
		// (get) Token: 0x06005309 RID: 21257 RVA: 0x0017C1E1 File Offset: 0x0017A3E1
		[Obsolete("normalizedPressure is deprecated, please use NormalizedPressure instead.")]
		public float normalizedPressure
		{
			get
			{
				return Mathf.Clamp(this.pressure / this.maximumPossiblePressure, 0.001f, 1f);
			}
		}

		// Token: 0x17000B61 RID: 2913
		// (get) Token: 0x0600530A RID: 21258 RVA: 0x0017C1FF File Offset: 0x0017A3FF
		public float NormalizedPressure
		{
			get
			{
				return Mathf.Clamp(this.pressure / this.maximumPossiblePressure, 0.001f, 1f);
			}
		}

		// Token: 0x17000B62 RID: 2914
		// (get) Token: 0x0600530B RID: 21259 RVA: 0x0017C21D File Offset: 0x0017A41D
		public bool IsMouse
		{
			get
			{
				return this.type == TouchType.Mouse;
			}
		}

		// Token: 0x0600530C RID: 21260 RVA: 0x0017C228 File Offset: 0x0017A428
		internal void SetWithTouchData(Touch touch, ulong updateTick, float deltaTime)
		{
			this.phase = touch.phase;
			this.tapCount = touch.tapCount;
			this.mouseButton = 0;
			this.altitudeAngle = touch.altitudeAngle;
			this.azimuthAngle = touch.azimuthAngle;
			this.maximumPossiblePressure = touch.maximumPossiblePressure;
			this.pressure = touch.pressure;
			this.radius = touch.radius;
			this.radiusVariance = touch.radiusVariance;
			Vector2 vector = touch.position;
			vector.x = Mathf.Clamp(vector.x, 0f, (float)Screen.width);
			vector.y = Mathf.Clamp(vector.y, 0f, (float)Screen.height);
			if (this.phase == TouchPhase.Began)
			{
				this.startPosition = vector;
				this.deltaPosition = Vector2.zero;
				this.lastPosition = vector;
				this.position = vector;
			}
			else
			{
				if (this.phase == TouchPhase.Stationary)
				{
					this.phase = TouchPhase.Moved;
				}
				this.deltaPosition = vector - this.lastPosition;
				this.lastPosition = this.position;
				this.position = vector;
			}
			this.deltaTime = deltaTime;
			this.updateTick = updateTick;
		}

		// Token: 0x0600530D RID: 21261 RVA: 0x0017C354 File Offset: 0x0017A554
		internal bool SetWithMouseData(int button, ulong updateTick, float deltaTime)
		{
			if (Input.touchCount > 0)
			{
				return false;
			}
			if (button < 0 || button > 2)
			{
				return false;
			}
			Vector2 vector = InputManager.MouseProvider.GetPosition();
			Vector2 a = new Vector2(Mathf.Round(vector.x), Mathf.Round(vector.y));
			Mouse control = Mouse.LeftButton + button;
			if (InputManager.MouseProvider.GetButtonWasPressed(control))
			{
				this.phase = TouchPhase.Began;
				this.pressure = 1f;
				this.maximumPossiblePressure = 1f;
				this.tapCount = 1;
				this.type = TouchType.Mouse;
				this.mouseButton = button;
				this.startPosition = a;
				this.deltaPosition = Vector2.zero;
				this.lastPosition = a;
				this.position = a;
				this.deltaTime = deltaTime;
				this.updateTick = updateTick;
				return true;
			}
			if (InputManager.MouseProvider.GetButtonWasReleased(control))
			{
				this.phase = TouchPhase.Ended;
				this.pressure = 0f;
				this.maximumPossiblePressure = 1f;
				this.tapCount = 1;
				this.type = TouchType.Mouse;
				this.mouseButton = button;
				this.deltaPosition = a - this.lastPosition;
				this.lastPosition = this.position;
				this.position = a;
				this.deltaTime = deltaTime;
				this.updateTick = updateTick;
				return true;
			}
			if (InputManager.MouseProvider.GetButtonIsPressed(control))
			{
				this.phase = TouchPhase.Moved;
				this.pressure = 1f;
				this.maximumPossiblePressure = 1f;
				this.tapCount = 1;
				this.type = TouchType.Mouse;
				this.mouseButton = button;
				this.deltaPosition = a - this.lastPosition;
				this.lastPosition = this.position;
				this.position = a;
				this.deltaTime = deltaTime;
				this.updateTick = updateTick;
				return true;
			}
			return false;
		}

		// Token: 0x0400532C RID: 21292
		public const int FingerID_None = -1;

		// Token: 0x0400532D RID: 21293
		public const int FingerID_Mouse = -2;

		// Token: 0x0400532E RID: 21294
		public int fingerId;

		// Token: 0x0400532F RID: 21295
		public int mouseButton;

		// Token: 0x04005330 RID: 21296
		public TouchPhase phase;

		// Token: 0x04005331 RID: 21297
		public int tapCount;

		// Token: 0x04005332 RID: 21298
		public Vector2 position;

		// Token: 0x04005333 RID: 21299
		public Vector2 startPosition;

		// Token: 0x04005334 RID: 21300
		public Vector2 deltaPosition;

		// Token: 0x04005335 RID: 21301
		public Vector2 lastPosition;

		// Token: 0x04005336 RID: 21302
		public float deltaTime;

		// Token: 0x04005337 RID: 21303
		public ulong updateTick;

		// Token: 0x04005338 RID: 21304
		public TouchType type;

		// Token: 0x04005339 RID: 21305
		public float altitudeAngle;

		// Token: 0x0400533A RID: 21306
		public float azimuthAngle;

		// Token: 0x0400533B RID: 21307
		public float maximumPossiblePressure;

		// Token: 0x0400533C RID: 21308
		public float pressure;

		// Token: 0x0400533D RID: 21309
		public float radius;

		// Token: 0x0400533E RID: 21310
		public float radiusVariance;
	}
}
