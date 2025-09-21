using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DC0 RID: 3520
	[ActionCategory(ActionCategory.AnimateVariables)]
	[Tooltip("Animates the value of a Float Variable FROM-TO with assistance of Deformation Curve.")]
	public class CurveFloat : CurveFsmAction
	{
		// Token: 0x060065FC RID: 26108 RVA: 0x00203ADB File Offset: 0x00201CDB
		public override void Reset()
		{
			base.Reset();
			this.floatVariable = new FsmFloat
			{
				UseVariable = true
			};
			this.toValue = new FsmFloat
			{
				UseVariable = true
			};
			this.fromValue = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x060065FD RID: 26109 RVA: 0x00203B1C File Offset: 0x00201D1C
		public override void OnEnter()
		{
			base.OnEnter();
			this.finishInNextStep = false;
			this.resultFloats = new float[1];
			this.fromFloats = new float[1];
			this.fromFloats[0] = (this.fromValue.IsNone ? 0f : this.fromValue.Value);
			this.toFloats = new float[1];
			this.toFloats[0] = (this.toValue.IsNone ? 0f : this.toValue.Value);
			this.calculations = new CurveFsmAction.Calculation[1];
			this.calculations[0] = this.calculation;
			this.curves = new AnimationCurve[1];
			this.curves[0] = this.animCurve.curve;
			base.Init();
		}

		// Token: 0x060065FE RID: 26110 RVA: 0x00203BE7 File Offset: 0x00201DE7
		public override void OnExit()
		{
		}

		// Token: 0x060065FF RID: 26111 RVA: 0x00203BEC File Offset: 0x00201DEC
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.floatVariable.IsNone && this.isRunning)
			{
				this.floatVariable.Value = this.resultFloats[0];
			}
			if (this.finishInNextStep && !this.looping)
			{
				base.Finish();
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
			}
			if (this.finishAction && !this.finishInNextStep)
			{
				if (!this.floatVariable.IsNone)
				{
					this.floatVariable.Value = this.resultFloats[0];
				}
				this.finishInNextStep = true;
			}
		}

		// Token: 0x04006545 RID: 25925
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The float variable to animate.")]
		public FsmFloat floatVariable;

		// Token: 0x04006546 RID: 25926
		[RequiredField]
		[Tooltip("Animate from this value.")]
		public FsmFloat fromValue;

		// Token: 0x04006547 RID: 25927
		[RequiredField]
		[Tooltip("Animate to this value.")]
		public FsmFloat toValue;

		// Token: 0x04006548 RID: 25928
		[RequiredField]
		[Tooltip("The curve to use when animating.")]
		public FsmAnimationCurve animCurve;

		// Token: 0x04006549 RID: 25929
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue and toValue.")]
		public CurveFsmAction.Calculation calculation;

		// Token: 0x0400654A RID: 25930
		private bool finishInNextStep;
	}
}
