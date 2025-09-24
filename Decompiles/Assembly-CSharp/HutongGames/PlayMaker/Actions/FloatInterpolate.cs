using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F77 RID: 3959
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Interpolates between 2 Float values over a specified Time.")]
	public class FloatInterpolate : FsmStateAction
	{
		// Token: 0x06006DB4 RID: 28084 RVA: 0x002211B9 File Offset: 0x0021F3B9
		public override void Reset()
		{
			this.mode = InterpolationType.Linear;
			this.fromFloat = null;
			this.toFloat = null;
			this.time = 1f;
			this.storeResult = null;
			this.finishEvent = null;
			this.realTime = false;
		}

		// Token: 0x06006DB5 RID: 28085 RVA: 0x002211F5 File Offset: 0x0021F3F5
		public override void OnEnter()
		{
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.currentTime = 0f;
			if (this.storeResult == null)
			{
				base.Finish();
				return;
			}
			this.storeResult.Value = this.fromFloat.Value;
		}

		// Token: 0x06006DB6 RID: 28086 RVA: 0x00221234 File Offset: 0x0021F434
		public override void OnUpdate()
		{
			if (this.realTime)
			{
				this.currentTime = FsmTime.RealtimeSinceStartup - this.startTime;
			}
			else
			{
				this.currentTime += Time.deltaTime;
			}
			float num = this.currentTime / this.time.Value;
			InterpolationType interpolationType = this.mode;
			if (interpolationType != InterpolationType.Linear)
			{
				if (interpolationType == InterpolationType.EaseInOut)
				{
					this.storeResult.Value = Mathf.SmoothStep(this.fromFloat.Value, this.toFloat.Value, num);
				}
			}
			else
			{
				this.storeResult.Value = Mathf.Lerp(this.fromFloat.Value, this.toFloat.Value, num);
			}
			if (num >= 1f)
			{
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
				base.Finish();
			}
		}

		// Token: 0x04006D6C RID: 28012
		[Tooltip("Interpolation mode: Linear or EaseInOut.")]
		public InterpolationType mode;

		// Token: 0x04006D6D RID: 28013
		[RequiredField]
		[Tooltip("Interpolate from this value.")]
		public FsmFloat fromFloat;

		// Token: 0x04006D6E RID: 28014
		[RequiredField]
		[Tooltip("Interpolate to this value.")]
		public FsmFloat toFloat;

		// Token: 0x04006D6F RID: 28015
		[RequiredField]
		[Tooltip("Interpolate over this amount of time in seconds.")]
		public FsmFloat time;

		// Token: 0x04006D70 RID: 28016
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the current value in a float variable.")]
		public FsmFloat storeResult;

		// Token: 0x04006D71 RID: 28017
		[Tooltip("Event to send when the interpolation is finished.")]
		public FsmEvent finishEvent;

		// Token: 0x04006D72 RID: 28018
		[Tooltip("Ignore TimeScale. Useful if the game is paused (Time scaled to 0).")]
		public bool realTime;

		// Token: 0x04006D73 RID: 28019
		private float startTime;

		// Token: 0x04006D74 RID: 28020
		private float currentTime;
	}
}
