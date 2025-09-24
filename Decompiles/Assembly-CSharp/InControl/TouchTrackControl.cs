using System;
using UnityEngine;

namespace InControl
{
	// Token: 0x02000926 RID: 2342
	public class TouchTrackControl : TouchControl
	{
		// Token: 0x060052F7 RID: 21239 RVA: 0x0017BE5A File Offset: 0x0017A05A
		public override void CreateControl()
		{
			this.ConfigureControl();
		}

		// Token: 0x060052F8 RID: 21240 RVA: 0x0017BE62 File Offset: 0x0017A062
		public override void DestroyControl()
		{
			if (this.currentTouch != null)
			{
				this.TouchEnded(this.currentTouch);
				this.currentTouch = null;
			}
		}

		// Token: 0x060052F9 RID: 21241 RVA: 0x0017BE7F File Offset: 0x0017A07F
		public override void ConfigureControl()
		{
			this.worldActiveArea = TouchManager.ConvertToWorld(this.activeArea, this.areaUnitType);
		}

		// Token: 0x060052FA RID: 21242 RVA: 0x0017BE98 File Offset: 0x0017A098
		public override void DrawGizmos()
		{
			Utility.DrawRectGizmo(this.worldActiveArea, Color.yellow);
		}

		// Token: 0x060052FB RID: 21243 RVA: 0x0017BEAA File Offset: 0x0017A0AA
		private void OnValidate()
		{
			if (this.maxTapDuration < 0f)
			{
				this.maxTapDuration = 0f;
			}
		}

		// Token: 0x060052FC RID: 21244 RVA: 0x0017BEC4 File Offset: 0x0017A0C4
		private void Update()
		{
			if (this.dirty)
			{
				this.ConfigureControl();
				this.dirty = false;
			}
		}

		// Token: 0x060052FD RID: 21245 RVA: 0x0017BEDC File Offset: 0x0017A0DC
		public override void SubmitControlState(ulong updateTick, float deltaTime)
		{
			Vector3 a = this.thisPosition - this.lastPosition;
			base.SubmitRawAnalogValue(this.target, a * this.scale, updateTick, deltaTime);
			this.lastPosition = this.thisPosition;
			base.SubmitButtonState(this.tapTarget, this.fireButtonTarget, updateTick, deltaTime);
			this.fireButtonTarget = false;
		}

		// Token: 0x060052FE RID: 21246 RVA: 0x0017BF41 File Offset: 0x0017A141
		public override void CommitControlState(ulong updateTick, float deltaTime)
		{
			base.CommitAnalog(this.target);
			base.CommitButton(this.tapTarget);
		}

		// Token: 0x060052FF RID: 21247 RVA: 0x0017BF5C File Offset: 0x0017A15C
		public override void TouchBegan(Touch touch)
		{
			if (this.currentTouch != null)
			{
				return;
			}
			this.beganPosition = TouchManager.ScreenToWorldPoint(touch.position);
			if (this.worldActiveArea.Contains(this.beganPosition))
			{
				this.thisPosition = TouchManager.ScreenToViewPoint(touch.position * 100f);
				this.lastPosition = this.thisPosition;
				this.currentTouch = touch;
				this.beganTime = Time.realtimeSinceStartup;
			}
		}

		// Token: 0x06005300 RID: 21248 RVA: 0x0017BFCF File Offset: 0x0017A1CF
		public override void TouchMoved(Touch touch)
		{
			if (this.currentTouch != touch)
			{
				return;
			}
			this.thisPosition = TouchManager.ScreenToViewPoint(touch.position * 100f);
		}

		// Token: 0x06005301 RID: 21249 RVA: 0x0017BFF8 File Offset: 0x0017A1F8
		public override void TouchEnded(Touch touch)
		{
			if (this.currentTouch != touch)
			{
				return;
			}
			Vector3 vector = TouchManager.ScreenToWorldPoint(touch.position) - this.beganPosition;
			float num = Time.realtimeSinceStartup - this.beganTime;
			if (vector.magnitude <= this.maxTapMovement && num <= this.maxTapDuration && this.tapTarget != TouchControl.ButtonTarget.None)
			{
				this.fireButtonTarget = true;
			}
			this.thisPosition = Vector3.zero;
			this.lastPosition = Vector3.zero;
			this.currentTouch = null;
		}

		// Token: 0x17000B5E RID: 2910
		// (get) Token: 0x06005302 RID: 21250 RVA: 0x0017C077 File Offset: 0x0017A277
		// (set) Token: 0x06005303 RID: 21251 RVA: 0x0017C07F File Offset: 0x0017A27F
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

		// Token: 0x17000B5F RID: 2911
		// (get) Token: 0x06005304 RID: 21252 RVA: 0x0017C09D File Offset: 0x0017A29D
		// (set) Token: 0x06005305 RID: 21253 RVA: 0x0017C0A5 File Offset: 0x0017A2A5
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

		// Token: 0x0400531D RID: 21277
		[Header("Dimensions")]
		[SerializeField]
		private TouchUnitType areaUnitType;

		// Token: 0x0400531E RID: 21278
		[SerializeField]
		private Rect activeArea = new Rect(25f, 25f, 50f, 50f);

		// Token: 0x0400531F RID: 21279
		[Header("Analog Target")]
		public TouchControl.AnalogTarget target = TouchControl.AnalogTarget.LeftStick;

		// Token: 0x04005320 RID: 21280
		public float scale = 1f;

		// Token: 0x04005321 RID: 21281
		[Header("Button Target")]
		public TouchControl.ButtonTarget tapTarget;

		// Token: 0x04005322 RID: 21282
		public float maxTapDuration = 0.5f;

		// Token: 0x04005323 RID: 21283
		public float maxTapMovement = 1f;

		// Token: 0x04005324 RID: 21284
		private Rect worldActiveArea;

		// Token: 0x04005325 RID: 21285
		private Vector3 lastPosition;

		// Token: 0x04005326 RID: 21286
		private Vector3 thisPosition;

		// Token: 0x04005327 RID: 21287
		private Touch currentTouch;

		// Token: 0x04005328 RID: 21288
		private bool dirty;

		// Token: 0x04005329 RID: 21289
		private bool fireButtonTarget;

		// Token: 0x0400532A RID: 21290
		private float beganTime;

		// Token: 0x0400532B RID: 21291
		private Vector3 beganPosition;
	}
}
