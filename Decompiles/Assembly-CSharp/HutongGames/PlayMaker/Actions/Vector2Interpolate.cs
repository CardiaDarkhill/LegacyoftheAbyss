using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200118B RID: 4491
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Interpolates between 2 Vector2 values over a specified Time.")]
	public class Vector2Interpolate : FsmStateAction
	{
		// Token: 0x06007857 RID: 30807 RVA: 0x002477C4 File Offset: 0x002459C4
		public override void Reset()
		{
			this.mode = InterpolationType.Linear;
			this.fromVector = new FsmVector2
			{
				UseVariable = true
			};
			this.toVector = new FsmVector2
			{
				UseVariable = true
			};
			this.time = 1f;
			this.storeResult = null;
			this.finishEvent = null;
			this.realTime = false;
		}

		// Token: 0x06007858 RID: 30808 RVA: 0x00247821 File Offset: 0x00245A21
		public override void OnEnter()
		{
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.currentTime = 0f;
			if (this.storeResult == null)
			{
				base.Finish();
				return;
			}
			this.storeResult.Value = this.fromVector.Value;
		}

		// Token: 0x06007859 RID: 30809 RVA: 0x00247860 File Offset: 0x00245A60
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
			if (interpolationType != InterpolationType.Linear && interpolationType == InterpolationType.EaseInOut)
			{
				num = Mathf.SmoothStep(0f, 1f, num);
			}
			this.storeResult.Value = Vector2.Lerp(this.fromVector.Value, this.toVector.Value, num);
			if (num >= 1f)
			{
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
				base.Finish();
			}
		}

		// Token: 0x040078C6 RID: 30918
		[Tooltip("The interpolation type")]
		public InterpolationType mode;

		// Token: 0x040078C7 RID: 30919
		[RequiredField]
		[Tooltip("The vector to interpolate from")]
		public FsmVector2 fromVector;

		// Token: 0x040078C8 RID: 30920
		[RequiredField]
		[Tooltip("The vector to interpolate to")]
		public FsmVector2 toVector;

		// Token: 0x040078C9 RID: 30921
		[RequiredField]
		[Tooltip("the interpolate time")]
		public FsmFloat time;

		// Token: 0x040078CA RID: 30922
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("the interpolated result")]
		public FsmVector2 storeResult;

		// Token: 0x040078CB RID: 30923
		[Tooltip("This event is fired when the interpolation is done.")]
		public FsmEvent finishEvent;

		// Token: 0x040078CC RID: 30924
		[Tooltip("Ignore TimeScale")]
		public bool realTime;

		// Token: 0x040078CD RID: 30925
		private float startTime;

		// Token: 0x040078CE RID: 30926
		private float currentTime;
	}
}
