using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DB8 RID: 3512
	[ActionCategory(ActionCategory.AnimateVariables)]
	[Tooltip("Animates the value of a Float Variable using an Animation Curve.")]
	public class AnimateFloat : FsmStateAction
	{
		// Token: 0x060065D1 RID: 26065 RVA: 0x00201FD5 File Offset: 0x002001D5
		public override void Reset()
		{
			this.animCurve = null;
			this.floatVariable = null;
			this.finishEvent = null;
			this.realTime = false;
		}

		// Token: 0x060065D2 RID: 26066 RVA: 0x00201FF4 File Offset: 0x002001F4
		public override void OnEnter()
		{
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.currentTime = 0f;
			if (this.animCurve != null && this.animCurve.curve != null && this.animCurve.curve.keys.Length != 0)
			{
				this.endTime = this.animCurve.curve.keys[this.animCurve.curve.length - 1].time;
				this.looping = ActionHelpers.IsLoopingWrapMode(this.animCurve.curve.postWrapMode);
				this.floatVariable.Value = this.animCurve.curve.Evaluate(0f);
				return;
			}
			base.Finish();
		}

		// Token: 0x060065D3 RID: 26067 RVA: 0x002020B8 File Offset: 0x002002B8
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
			if (this.animCurve != null && this.animCurve.curve != null && this.floatVariable != null)
			{
				this.floatVariable.Value = this.animCurve.curve.Evaluate(this.currentTime);
			}
			if (this.currentTime >= this.endTime)
			{
				if (!this.looping)
				{
					base.Finish();
				}
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
			}
		}

		// Token: 0x040064F6 RID: 25846
		[RequiredField]
		[Tooltip("The animation curve to use.")]
		public FsmAnimationCurve animCurve;

		// Token: 0x040064F7 RID: 25847
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The float variable to set.")]
		public FsmFloat floatVariable;

		// Token: 0x040064F8 RID: 25848
		[Tooltip("Optionally send an Event when the animation finishes.")]
		public FsmEvent finishEvent;

		// Token: 0x040064F9 RID: 25849
		[Tooltip("Ignore TimeScale. Useful if the game is paused.")]
		public bool realTime;

		// Token: 0x040064FA RID: 25850
		private float startTime;

		// Token: 0x040064FB RID: 25851
		private float currentTime;

		// Token: 0x040064FC RID: 25852
		private float endTime;

		// Token: 0x040064FD RID: 25853
		private bool looping;
	}
}
