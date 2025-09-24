using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011A3 RID: 4515
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Interpolates between 2 Vector3 values over a specified Time.")]
	public class Vector3Interpolate : FsmStateAction
	{
		// Token: 0x060078C2 RID: 30914 RVA: 0x00248AB4 File Offset: 0x00246CB4
		public override void Reset()
		{
			this.mode = InterpolationType.Linear;
			this.fromVector = new FsmVector3
			{
				UseVariable = true
			};
			this.toVector = new FsmVector3
			{
				UseVariable = true
			};
			this.time = 1f;
			this.storeResult = null;
			this.finishEvent = null;
			this.realTime = false;
		}

		// Token: 0x060078C3 RID: 30915 RVA: 0x00248B11 File Offset: 0x00246D11
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

		// Token: 0x060078C4 RID: 30916 RVA: 0x00248B50 File Offset: 0x00246D50
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
			this.storeResult.Value = Vector3.Lerp(this.fromVector.Value, this.toVector.Value, num);
			if (num >= 1f)
			{
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
				base.Finish();
			}
		}

		// Token: 0x04007923 RID: 31011
		[Tooltip("The type of interpolation to use.")]
		public InterpolationType mode;

		// Token: 0x04007924 RID: 31012
		[RequiredField]
		[Tooltip("The start vector.")]
		public FsmVector3 fromVector;

		// Token: 0x04007925 RID: 31013
		[RequiredField]
		[Tooltip("The end vector.")]
		public FsmVector3 toVector;

		// Token: 0x04007926 RID: 31014
		[RequiredField]
		[Tooltip("How long it should take to interpolate from start to end.")]
		public FsmFloat time;

		// Token: 0x04007927 RID: 31015
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the interpolated vector in a Vector3 Variable.")]
		public FsmVector3 storeResult;

		// Token: 0x04007928 RID: 31016
		[Tooltip("Optionally send this event when finished.")]
		public FsmEvent finishEvent;

		// Token: 0x04007929 RID: 31017
		[Tooltip("Ignore TimeScale e.g., if the game is paused.")]
		public bool realTime;

		// Token: 0x0400792A RID: 31018
		private float startTime;

		// Token: 0x0400792B RID: 31019
		private float currentTime;
	}
}
