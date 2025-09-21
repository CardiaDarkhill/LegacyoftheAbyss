using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DBB RID: 3515
	[ActionCategory("AnimateVariables")]
	[Tooltip("Animates the value of a Rect Variable using an Animation Curve.")]
	public class AnimateRect : AnimateFsmAction
	{
		// Token: 0x060065E3 RID: 26083 RVA: 0x00202D95 File Offset: 0x00200F95
		public override void Reset()
		{
			base.Reset();
			this.rectVariable = new FsmRect
			{
				UseVariable = true
			};
		}

		// Token: 0x060065E4 RID: 26084 RVA: 0x00202DB0 File Offset: 0x00200FB0
		public override void OnEnter()
		{
			base.OnEnter();
			this.finishInNextStep = false;
			this.resultFloats = new float[4];
			this.fromFloats = new float[4];
			this.fromFloats[0] = (this.rectVariable.IsNone ? 0f : this.rectVariable.Value.x);
			this.fromFloats[1] = (this.rectVariable.IsNone ? 0f : this.rectVariable.Value.y);
			this.fromFloats[2] = (this.rectVariable.IsNone ? 0f : this.rectVariable.Value.width);
			this.fromFloats[3] = (this.rectVariable.IsNone ? 0f : this.rectVariable.Value.height);
			this.curves = new AnimationCurve[4];
			this.curves[0] = this.curveX.curve;
			this.curves[1] = this.curveY.curve;
			this.curves[2] = this.curveW.curve;
			this.curves[3] = this.curveH.curve;
			this.calculations = new AnimateFsmAction.Calculation[4];
			this.calculations[0] = this.calculationX;
			this.calculations[1] = this.calculationY;
			this.calculations[2] = this.calculationW;
			this.calculations[3] = this.calculationH;
			base.Init();
			if (Math.Abs(this.delay.Value) < 0.01f)
			{
				this.UpdateVariableValue();
			}
		}

		// Token: 0x060065E5 RID: 26085 RVA: 0x00202F5D File Offset: 0x0020115D
		private void UpdateVariableValue()
		{
			if (!this.rectVariable.IsNone)
			{
				this.rectVariable.Value = new Rect(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
			}
		}

		// Token: 0x060065E6 RID: 26086 RVA: 0x00202F9C File Offset: 0x0020119C
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.isRunning)
			{
				this.UpdateVariableValue();
			}
			if (this.finishInNextStep && !this.looping)
			{
				base.Finish();
				base.Fsm.Event(this.finishEvent);
			}
			if (this.finishAction && !this.finishInNextStep)
			{
				this.UpdateVariableValue();
				this.finishInNextStep = true;
			}
		}

		// Token: 0x04006519 RID: 25881
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Rect Variable to animate.")]
		public FsmRect rectVariable;

		// Token: 0x0400651A RID: 25882
		[RequiredField]
		[Tooltip("Curve to use for the X value.")]
		public FsmAnimationCurve curveX;

		// Token: 0x0400651B RID: 25883
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to rectVariable.x.")]
		public AnimateFsmAction.Calculation calculationX;

		// Token: 0x0400651C RID: 25884
		[RequiredField]
		[Tooltip("Curve to use for the Y value.")]
		public FsmAnimationCurve curveY;

		// Token: 0x0400651D RID: 25885
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to rectVariable.y.")]
		public AnimateFsmAction.Calculation calculationY;

		// Token: 0x0400651E RID: 25886
		[RequiredField]
		[Tooltip("Curve to use for the Width.")]
		public FsmAnimationCurve curveW;

		// Token: 0x0400651F RID: 25887
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to rectVariable.width.")]
		public AnimateFsmAction.Calculation calculationW;

		// Token: 0x04006520 RID: 25888
		[RequiredField]
		[Tooltip("Curve to use for the Height.")]
		public FsmAnimationCurve curveH;

		// Token: 0x04006521 RID: 25889
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to rectVariable.height.")]
		public AnimateFsmAction.Calculation calculationH;

		// Token: 0x04006522 RID: 25890
		private bool finishInNextStep;
	}
}
