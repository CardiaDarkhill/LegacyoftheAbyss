using System;
using UnityEngine;

namespace InControl
{
	// Token: 0x02000924 RID: 2340
	public class TouchStickControl : TouchControl
	{
		// Token: 0x060052CC RID: 21196 RVA: 0x0017B17D File Offset: 0x0017937D
		public override void CreateControl()
		{
			this.ring.Create("Ring", base.transform, 1000);
			this.knob.Create("Knob", base.transform, 1001);
		}

		// Token: 0x060052CD RID: 21197 RVA: 0x0017B1B5 File Offset: 0x001793B5
		public override void DestroyControl()
		{
			this.ring.Delete();
			this.knob.Delete();
			if (this.currentTouch != null)
			{
				this.TouchEnded(this.currentTouch);
				this.currentTouch = null;
			}
		}

		// Token: 0x060052CE RID: 21198 RVA: 0x0017B1E8 File Offset: 0x001793E8
		public override void ConfigureControl()
		{
			this.resetPosition = base.OffsetToWorldPosition(this.anchor, this.offset, this.offsetUnitType, true);
			base.transform.position = this.resetPosition;
			this.ring.Update(true);
			this.knob.Update(true);
			this.worldActiveArea = TouchManager.ConvertToWorld(this.activeArea, this.areaUnitType);
			this.worldKnobRange = TouchManager.ConvertToWorld(this.knobRange, this.knob.SizeUnitType);
		}

		// Token: 0x060052CF RID: 21199 RVA: 0x0017B270 File Offset: 0x00179470
		public override void DrawGizmos()
		{
			this.ring.DrawGizmos(this.RingPosition, Color.yellow);
			this.knob.DrawGizmos(this.KnobPosition, Color.yellow);
			Utility.DrawCircleGizmo(this.RingPosition, this.worldKnobRange, Color.red);
			Utility.DrawRectGizmo(this.worldActiveArea, Color.green);
		}

		// Token: 0x060052D0 RID: 21200 RVA: 0x0017B2D4 File Offset: 0x001794D4
		private void Update()
		{
			if (this.dirty)
			{
				this.ConfigureControl();
				this.dirty = false;
			}
			else
			{
				this.ring.Update();
				this.knob.Update();
			}
			if (this.IsNotActive)
			{
				if (this.resetWhenDone && this.KnobPosition != this.resetPosition)
				{
					Vector3 b = this.KnobPosition - this.RingPosition;
					this.RingPosition = Vector3.MoveTowards(this.RingPosition, this.resetPosition, this.ringResetSpeed * Time.unscaledDeltaTime);
					this.KnobPosition = this.RingPosition + b;
				}
				if (this.KnobPosition != this.RingPosition)
				{
					this.KnobPosition = Vector3.MoveTowards(this.KnobPosition, this.RingPosition, this.knobResetSpeed * Time.unscaledDeltaTime);
				}
			}
		}

		// Token: 0x060052D1 RID: 21201 RVA: 0x0017B3B1 File Offset: 0x001795B1
		public override void SubmitControlState(ulong updateTick, float deltaTime)
		{
			base.SubmitAnalogValue(this.target, this.value, this.lowerDeadZone, this.upperDeadZone, updateTick, deltaTime);
		}

		// Token: 0x060052D2 RID: 21202 RVA: 0x0017B3D8 File Offset: 0x001795D8
		public override void CommitControlState(ulong updateTick, float deltaTime)
		{
			base.CommitAnalog(this.target);
		}

		// Token: 0x060052D3 RID: 21203 RVA: 0x0017B3E8 File Offset: 0x001795E8
		public override void TouchBegan(Touch touch)
		{
			if (this.IsActive)
			{
				return;
			}
			this.beganPosition = TouchManager.ScreenToWorldPoint(touch.position);
			bool flag = this.worldActiveArea.Contains(this.beganPosition);
			bool flag2 = this.ring.Contains(this.beganPosition);
			if (this.snapToInitialTouch && (flag || flag2))
			{
				this.RingPosition = this.beganPosition;
				this.KnobPosition = this.beganPosition;
				this.currentTouch = touch;
			}
			else if (flag2)
			{
				this.KnobPosition = this.beganPosition;
				this.beganPosition = this.RingPosition;
				this.currentTouch = touch;
			}
			if (this.IsActive)
			{
				this.TouchMoved(touch);
				this.ring.State = true;
				this.knob.State = true;
			}
		}

		// Token: 0x060052D4 RID: 21204 RVA: 0x0017B4B0 File Offset: 0x001796B0
		public override void TouchMoved(Touch touch)
		{
			if (this.currentTouch != touch)
			{
				return;
			}
			this.movedPosition = TouchManager.ScreenToWorldPoint(touch.position);
			if (this.lockToAxis == LockAxis.Horizontal && this.allowDraggingAxis == DragAxis.Horizontal)
			{
				this.movedPosition.y = this.beganPosition.y;
			}
			else if (this.lockToAxis == LockAxis.Vertical && this.allowDraggingAxis == DragAxis.Vertical)
			{
				this.movedPosition.x = this.beganPosition.x;
			}
			Vector3 vector = this.movedPosition - this.beganPosition;
			Vector3 normalized = vector.normalized;
			float magnitude = vector.magnitude;
			if (this.allowDragging)
			{
				float num = magnitude - this.worldKnobRange;
				if (num < 0f)
				{
					num = 0f;
				}
				Vector3 b = num * normalized;
				if (this.allowDraggingAxis == DragAxis.Horizontal)
				{
					b.y = 0f;
				}
				else if (this.allowDraggingAxis == DragAxis.Vertical)
				{
					b.x = 0f;
				}
				this.beganPosition += b;
				this.RingPosition = this.beganPosition;
			}
			this.movedPosition = this.beganPosition + Mathf.Clamp(magnitude, 0f, this.worldKnobRange) * normalized;
			if (this.lockToAxis == LockAxis.Horizontal)
			{
				this.movedPosition.y = this.beganPosition.y;
			}
			else if (this.lockToAxis == LockAxis.Vertical)
			{
				this.movedPosition.x = this.beganPosition.x;
			}
			if (this.snapAngles != TouchControl.SnapAngles.None)
			{
				this.movedPosition = TouchControl.SnapTo(this.movedPosition - this.beganPosition, this.snapAngles) + this.beganPosition;
			}
			this.RingPosition = this.beganPosition;
			this.KnobPosition = this.movedPosition;
			this.value = (this.movedPosition - this.beganPosition) / this.worldKnobRange;
			this.value.x = this.inputCurve.Evaluate(Utility.Abs(this.value.x)) * Mathf.Sign(this.value.x);
			this.value.y = this.inputCurve.Evaluate(Utility.Abs(this.value.y)) * Mathf.Sign(this.value.y);
		}

		// Token: 0x060052D5 RID: 21205 RVA: 0x0017B70C File Offset: 0x0017990C
		public override void TouchEnded(Touch touch)
		{
			if (this.currentTouch != touch)
			{
				return;
			}
			this.value = Vector3.zero;
			float magnitude = (this.resetPosition - this.RingPosition).magnitude;
			this.ringResetSpeed = (Utility.IsZero(this.resetDuration) ? magnitude : (magnitude / this.resetDuration));
			float magnitude2 = (this.RingPosition - this.KnobPosition).magnitude;
			this.knobResetSpeed = (Utility.IsZero(this.resetDuration) ? this.knobRange : (magnitude2 / this.resetDuration));
			this.currentTouch = null;
			this.ring.State = false;
			this.knob.State = false;
		}

		// Token: 0x17000B53 RID: 2899
		// (get) Token: 0x060052D6 RID: 21206 RVA: 0x0017B7C2 File Offset: 0x001799C2
		public bool IsActive
		{
			get
			{
				return this.currentTouch != null;
			}
		}

		// Token: 0x17000B54 RID: 2900
		// (get) Token: 0x060052D7 RID: 21207 RVA: 0x0017B7CD File Offset: 0x001799CD
		public bool IsNotActive
		{
			get
			{
				return this.currentTouch == null;
			}
		}

		// Token: 0x17000B55 RID: 2901
		// (get) Token: 0x060052D8 RID: 21208 RVA: 0x0017B7D8 File Offset: 0x001799D8
		// (set) Token: 0x060052D9 RID: 21209 RVA: 0x0017B7FE File Offset: 0x001799FE
		public Vector3 RingPosition
		{
			get
			{
				if (!this.ring.Ready)
				{
					return base.transform.position;
				}
				return this.ring.Position;
			}
			set
			{
				if (this.ring.Ready)
				{
					this.ring.Position = value;
				}
			}
		}

		// Token: 0x17000B56 RID: 2902
		// (get) Token: 0x060052DA RID: 21210 RVA: 0x0017B819 File Offset: 0x00179A19
		// (set) Token: 0x060052DB RID: 21211 RVA: 0x0017B83F File Offset: 0x00179A3F
		public Vector3 KnobPosition
		{
			get
			{
				if (!this.knob.Ready)
				{
					return base.transform.position;
				}
				return this.knob.Position;
			}
			set
			{
				if (this.knob.Ready)
				{
					this.knob.Position = value;
				}
			}
		}

		// Token: 0x17000B57 RID: 2903
		// (get) Token: 0x060052DC RID: 21212 RVA: 0x0017B85A File Offset: 0x00179A5A
		// (set) Token: 0x060052DD RID: 21213 RVA: 0x0017B862 File Offset: 0x00179A62
		public TouchControlAnchor Anchor
		{
			get
			{
				return this.anchor;
			}
			set
			{
				if (this.anchor != value)
				{
					this.anchor = value;
					this.dirty = true;
				}
			}
		}

		// Token: 0x17000B58 RID: 2904
		// (get) Token: 0x060052DE RID: 21214 RVA: 0x0017B87B File Offset: 0x00179A7B
		// (set) Token: 0x060052DF RID: 21215 RVA: 0x0017B883 File Offset: 0x00179A83
		public Vector2 Offset
		{
			get
			{
				return this.offset;
			}
			set
			{
				if (this.offset != value)
				{
					this.offset = value;
					this.dirty = true;
				}
			}
		}

		// Token: 0x17000B59 RID: 2905
		// (get) Token: 0x060052E0 RID: 21216 RVA: 0x0017B8A1 File Offset: 0x00179AA1
		// (set) Token: 0x060052E1 RID: 21217 RVA: 0x0017B8A9 File Offset: 0x00179AA9
		public TouchUnitType OffsetUnitType
		{
			get
			{
				return this.offsetUnitType;
			}
			set
			{
				if (this.offsetUnitType != value)
				{
					this.offsetUnitType = value;
					this.dirty = true;
				}
			}
		}

		// Token: 0x17000B5A RID: 2906
		// (get) Token: 0x060052E2 RID: 21218 RVA: 0x0017B8C2 File Offset: 0x00179AC2
		// (set) Token: 0x060052E3 RID: 21219 RVA: 0x0017B8CA File Offset: 0x00179ACA
		public Rect ActiveArea
		{
			get
			{
				return this.activeArea;
			}
			set
			{
				if (this.activeArea != value)
				{
					this.activeArea = value;
					this.dirty = true;
				}
			}
		}

		// Token: 0x17000B5B RID: 2907
		// (get) Token: 0x060052E4 RID: 21220 RVA: 0x0017B8E8 File Offset: 0x00179AE8
		// (set) Token: 0x060052E5 RID: 21221 RVA: 0x0017B8F0 File Offset: 0x00179AF0
		public TouchUnitType AreaUnitType
		{
			get
			{
				return this.areaUnitType;
			}
			set
			{
				if (this.areaUnitType != value)
				{
					this.areaUnitType = value;
					this.dirty = true;
				}
			}
		}

		// Token: 0x040052EB RID: 21227
		[Header("Position")]
		[SerializeField]
		private TouchControlAnchor anchor = TouchControlAnchor.BottomLeft;

		// Token: 0x040052EC RID: 21228
		[SerializeField]
		private TouchUnitType offsetUnitType;

		// Token: 0x040052ED RID: 21229
		[SerializeField]
		private Vector2 offset = new Vector2(20f, 20f);

		// Token: 0x040052EE RID: 21230
		[SerializeField]
		private TouchUnitType areaUnitType;

		// Token: 0x040052EF RID: 21231
		[SerializeField]
		private Rect activeArea = new Rect(0f, 0f, 50f, 100f);

		// Token: 0x040052F0 RID: 21232
		[Header("Options")]
		public TouchControl.AnalogTarget target = TouchControl.AnalogTarget.LeftStick;

		// Token: 0x040052F1 RID: 21233
		public TouchControl.SnapAngles snapAngles;

		// Token: 0x040052F2 RID: 21234
		public LockAxis lockToAxis;

		// Token: 0x040052F3 RID: 21235
		[Range(0f, 1f)]
		public float lowerDeadZone = 0.1f;

		// Token: 0x040052F4 RID: 21236
		[Range(0f, 1f)]
		public float upperDeadZone = 0.9f;

		// Token: 0x040052F5 RID: 21237
		public AnimationCurve inputCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040052F6 RID: 21238
		public bool allowDragging;

		// Token: 0x040052F7 RID: 21239
		public DragAxis allowDraggingAxis;

		// Token: 0x040052F8 RID: 21240
		public bool snapToInitialTouch = true;

		// Token: 0x040052F9 RID: 21241
		public bool resetWhenDone = true;

		// Token: 0x040052FA RID: 21242
		public float resetDuration = 0.1f;

		// Token: 0x040052FB RID: 21243
		[Header("Sprites")]
		public TouchSprite ring = new TouchSprite(20f);

		// Token: 0x040052FC RID: 21244
		public TouchSprite knob = new TouchSprite(10f);

		// Token: 0x040052FD RID: 21245
		public float knobRange = 7.5f;

		// Token: 0x040052FE RID: 21246
		private Vector3 resetPosition;

		// Token: 0x040052FF RID: 21247
		private Vector3 beganPosition;

		// Token: 0x04005300 RID: 21248
		private Vector3 movedPosition;

		// Token: 0x04005301 RID: 21249
		private float ringResetSpeed;

		// Token: 0x04005302 RID: 21250
		private float knobResetSpeed;

		// Token: 0x04005303 RID: 21251
		private Rect worldActiveArea;

		// Token: 0x04005304 RID: 21252
		private float worldKnobRange;

		// Token: 0x04005305 RID: 21253
		private Vector3 value;

		// Token: 0x04005306 RID: 21254
		private Touch currentTouch;

		// Token: 0x04005307 RID: 21255
		private bool dirty;
	}
}
