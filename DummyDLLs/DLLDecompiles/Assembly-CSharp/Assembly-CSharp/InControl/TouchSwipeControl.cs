using System;
using UnityEngine;

namespace InControl
{
	// Token: 0x02000925 RID: 2341
	public class TouchSwipeControl : TouchControl
	{
		// Token: 0x060052E7 RID: 21223 RVA: 0x0017B9DA File Offset: 0x00179BDA
		public override void CreateControl()
		{
		}

		// Token: 0x060052E8 RID: 21224 RVA: 0x0017B9DC File Offset: 0x00179BDC
		public override void DestroyControl()
		{
			if (this.currentTouch != null)
			{
				this.TouchEnded(this.currentTouch);
				this.currentTouch = null;
			}
		}

		// Token: 0x060052E9 RID: 21225 RVA: 0x0017B9F9 File Offset: 0x00179BF9
		public override void ConfigureControl()
		{
			this.worldActiveArea = TouchManager.ConvertToWorld(this.activeArea, this.areaUnitType);
		}

		// Token: 0x060052EA RID: 21226 RVA: 0x0017BA12 File Offset: 0x00179C12
		public override void DrawGizmos()
		{
			Utility.DrawRectGizmo(this.worldActiveArea, Color.yellow);
		}

		// Token: 0x060052EB RID: 21227 RVA: 0x0017BA24 File Offset: 0x00179C24
		private void Update()
		{
			if (this.dirty)
			{
				this.ConfigureControl();
				this.dirty = false;
			}
		}

		// Token: 0x060052EC RID: 21228 RVA: 0x0017BA3C File Offset: 0x00179C3C
		public override void SubmitControlState(ulong updateTick, float deltaTime)
		{
			Vector3 v = TouchControl.SnapTo(this.currentVector, this.snapAngles);
			base.SubmitAnalogValue(this.target, v, 0f, 1f, updateTick, deltaTime);
			base.SubmitButtonState(this.upTarget, this.fireButtonTarget && this.nextButtonTarget == this.upTarget, updateTick, deltaTime);
			base.SubmitButtonState(this.downTarget, this.fireButtonTarget && this.nextButtonTarget == this.downTarget, updateTick, deltaTime);
			base.SubmitButtonState(this.leftTarget, this.fireButtonTarget && this.nextButtonTarget == this.leftTarget, updateTick, deltaTime);
			base.SubmitButtonState(this.rightTarget, this.fireButtonTarget && this.nextButtonTarget == this.rightTarget, updateTick, deltaTime);
			base.SubmitButtonState(this.tapTarget, this.fireButtonTarget && this.nextButtonTarget == this.tapTarget, updateTick, deltaTime);
			if (this.fireButtonTarget && this.nextButtonTarget != TouchControl.ButtonTarget.None)
			{
				this.fireButtonTarget = !this.oneSwipePerTouch;
				this.lastButtonTarget = this.nextButtonTarget;
				this.nextButtonTarget = TouchControl.ButtonTarget.None;
			}
		}

		// Token: 0x060052ED RID: 21229 RVA: 0x0017BB74 File Offset: 0x00179D74
		public override void CommitControlState(ulong updateTick, float deltaTime)
		{
			base.CommitAnalog(this.target);
			base.CommitButton(this.upTarget);
			base.CommitButton(this.downTarget);
			base.CommitButton(this.leftTarget);
			base.CommitButton(this.rightTarget);
			base.CommitButton(this.tapTarget);
		}

		// Token: 0x060052EE RID: 21230 RVA: 0x0017BBCC File Offset: 0x00179DCC
		public override void TouchBegan(Touch touch)
		{
			if (this.currentTouch != null)
			{
				return;
			}
			this.beganPosition = TouchManager.ScreenToWorldPoint(touch.position);
			if (this.worldActiveArea.Contains(this.beganPosition))
			{
				this.lastPosition = this.beganPosition;
				this.currentTouch = touch;
				this.currentVector = Vector2.zero;
				this.currentVectorIsSet = false;
				this.fireButtonTarget = true;
				this.nextButtonTarget = TouchControl.ButtonTarget.None;
				this.lastButtonTarget = TouchControl.ButtonTarget.None;
			}
		}

		// Token: 0x060052EF RID: 21231 RVA: 0x0017BC48 File Offset: 0x00179E48
		public override void TouchMoved(Touch touch)
		{
			if (this.currentTouch != touch)
			{
				return;
			}
			Vector3 a = TouchManager.ScreenToWorldPoint(touch.position);
			Vector3 vector = a - this.lastPosition;
			if (vector.magnitude >= this.sensitivity)
			{
				this.lastPosition = a;
				if (!this.oneSwipePerTouch || !this.currentVectorIsSet)
				{
					this.currentVector = vector.normalized;
					this.currentVectorIsSet = true;
				}
				if (this.fireButtonTarget)
				{
					TouchControl.ButtonTarget buttonTargetForVector = this.GetButtonTargetForVector(this.currentVector);
					if (buttonTargetForVector != this.lastButtonTarget)
					{
						this.nextButtonTarget = buttonTargetForVector;
					}
				}
			}
		}

		// Token: 0x060052F0 RID: 21232 RVA: 0x0017BCDC File Offset: 0x00179EDC
		public override void TouchEnded(Touch touch)
		{
			if (this.currentTouch != touch)
			{
				return;
			}
			this.currentTouch = null;
			this.currentVector = Vector2.zero;
			this.currentVectorIsSet = false;
			Vector3 b = TouchManager.ScreenToWorldPoint(touch.position);
			if ((this.beganPosition - b).magnitude < this.sensitivity)
			{
				this.fireButtonTarget = true;
				this.nextButtonTarget = this.tapTarget;
				this.lastButtonTarget = TouchControl.ButtonTarget.None;
				return;
			}
			this.fireButtonTarget = false;
			this.nextButtonTarget = TouchControl.ButtonTarget.None;
			this.lastButtonTarget = TouchControl.ButtonTarget.None;
		}

		// Token: 0x060052F1 RID: 21233 RVA: 0x0017BD6C File Offset: 0x00179F6C
		private TouchControl.ButtonTarget GetButtonTargetForVector(Vector2 vector)
		{
			Vector2 lhs = TouchControl.SnapTo(vector, TouchControl.SnapAngles.Four);
			if (lhs == Vector2.up)
			{
				return this.upTarget;
			}
			if (lhs == Vector2.right)
			{
				return this.rightTarget;
			}
			if (lhs == -Vector2.up)
			{
				return this.downTarget;
			}
			if (lhs == -Vector2.right)
			{
				return this.leftTarget;
			}
			return TouchControl.ButtonTarget.None;
		}

		// Token: 0x17000B5C RID: 2908
		// (get) Token: 0x060052F2 RID: 21234 RVA: 0x0017BDE1 File Offset: 0x00179FE1
		// (set) Token: 0x060052F3 RID: 21235 RVA: 0x0017BDE9 File Offset: 0x00179FE9
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

		// Token: 0x17000B5D RID: 2909
		// (get) Token: 0x060052F4 RID: 21236 RVA: 0x0017BE07 File Offset: 0x0017A007
		// (set) Token: 0x060052F5 RID: 21237 RVA: 0x0017BE0F File Offset: 0x0017A00F
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

		// Token: 0x04005308 RID: 21256
		[Header("Position")]
		[SerializeField]
		private TouchUnitType areaUnitType;

		// Token: 0x04005309 RID: 21257
		[SerializeField]
		private Rect activeArea = new Rect(25f, 25f, 50f, 50f);

		// Token: 0x0400530A RID: 21258
		[Header("Options")]
		[Range(0f, 1f)]
		public float sensitivity = 0.1f;

		// Token: 0x0400530B RID: 21259
		public bool oneSwipePerTouch;

		// Token: 0x0400530C RID: 21260
		[Header("Analog Target")]
		public TouchControl.AnalogTarget target;

		// Token: 0x0400530D RID: 21261
		public TouchControl.SnapAngles snapAngles;

		// Token: 0x0400530E RID: 21262
		[Header("Button Targets")]
		public TouchControl.ButtonTarget upTarget;

		// Token: 0x0400530F RID: 21263
		public TouchControl.ButtonTarget downTarget;

		// Token: 0x04005310 RID: 21264
		public TouchControl.ButtonTarget leftTarget;

		// Token: 0x04005311 RID: 21265
		public TouchControl.ButtonTarget rightTarget;

		// Token: 0x04005312 RID: 21266
		public TouchControl.ButtonTarget tapTarget;

		// Token: 0x04005313 RID: 21267
		private Rect worldActiveArea;

		// Token: 0x04005314 RID: 21268
		private Vector3 currentVector;

		// Token: 0x04005315 RID: 21269
		private bool currentVectorIsSet;

		// Token: 0x04005316 RID: 21270
		private Vector3 beganPosition;

		// Token: 0x04005317 RID: 21271
		private Vector3 lastPosition;

		// Token: 0x04005318 RID: 21272
		private Touch currentTouch;

		// Token: 0x04005319 RID: 21273
		private bool fireButtonTarget;

		// Token: 0x0400531A RID: 21274
		private TouchControl.ButtonTarget nextButtonTarget;

		// Token: 0x0400531B RID: 21275
		private TouchControl.ButtonTarget lastButtonTarget;

		// Token: 0x0400531C RID: 21276
		private bool dirty;
	}
}
