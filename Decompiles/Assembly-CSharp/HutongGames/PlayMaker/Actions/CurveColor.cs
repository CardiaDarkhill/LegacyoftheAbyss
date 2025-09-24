using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DBF RID: 3519
	[ActionCategory(ActionCategory.AnimateVariables)]
	[Tooltip("Animates the value of a Color Variable FROM-TO with assistance of Deformation Curves.")]
	public class CurveColor : CurveFsmAction
	{
		// Token: 0x060065F7 RID: 26103 RVA: 0x0020375D File Offset: 0x0020195D
		public override void Reset()
		{
			base.Reset();
			this.colorVariable = new FsmColor
			{
				UseVariable = true
			};
			this.toValue = new FsmColor
			{
				UseVariable = true
			};
			this.fromValue = new FsmColor
			{
				UseVariable = true
			};
		}

		// Token: 0x060065F8 RID: 26104 RVA: 0x0020379C File Offset: 0x0020199C
		public override void OnEnter()
		{
			base.OnEnter();
			this.finishInNextStep = false;
			this.resultFloats = new float[4];
			this.fromFloats = new float[4];
			this.fromFloats[0] = (this.fromValue.IsNone ? 0f : this.fromValue.Value.r);
			this.fromFloats[1] = (this.fromValue.IsNone ? 0f : this.fromValue.Value.g);
			this.fromFloats[2] = (this.fromValue.IsNone ? 0f : this.fromValue.Value.b);
			this.fromFloats[3] = (this.fromValue.IsNone ? 0f : this.fromValue.Value.a);
			this.toFloats = new float[4];
			this.toFloats[0] = (this.toValue.IsNone ? 0f : this.toValue.Value.r);
			this.toFloats[1] = (this.toValue.IsNone ? 0f : this.toValue.Value.g);
			this.toFloats[2] = (this.toValue.IsNone ? 0f : this.toValue.Value.b);
			this.toFloats[3] = (this.toValue.IsNone ? 0f : this.toValue.Value.a);
			this.curves = new AnimationCurve[4];
			this.curves[0] = this.curveR.curve;
			this.curves[1] = this.curveG.curve;
			this.curves[2] = this.curveB.curve;
			this.curves[3] = this.curveA.curve;
			this.calculations = new CurveFsmAction.Calculation[4];
			this.calculations[0] = this.calculationR;
			this.calculations[1] = this.calculationG;
			this.calculations[2] = this.calculationB;
			this.calculations[3] = this.calculationA;
			base.Init();
		}

		// Token: 0x060065F9 RID: 26105 RVA: 0x002039DC File Offset: 0x00201BDC
		public override void OnExit()
		{
		}

		// Token: 0x060065FA RID: 26106 RVA: 0x002039E0 File Offset: 0x00201BE0
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.colorVariable.IsNone && this.isRunning)
			{
				this.clr = new Color(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
				this.colorVariable.Value = this.clr;
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
				if (!this.colorVariable.IsNone)
				{
					this.clr = new Color(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
					this.colorVariable.Value = this.clr;
				}
				this.finishInNextStep = true;
			}
		}

		// Token: 0x04006538 RID: 25912
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Color variable to animate.")]
		public FsmColor colorVariable;

		// Token: 0x04006539 RID: 25913
		[RequiredField]
		[Tooltip("Animate from this color.")]
		public FsmColor fromValue;

		// Token: 0x0400653A RID: 25914
		[RequiredField]
		[Tooltip("Animate to this color.")]
		public FsmColor toValue;

		// Token: 0x0400653B RID: 25915
		[RequiredField]
		[Tooltip("The curve used to animate the red value.")]
		public FsmAnimationCurve curveR;

		// Token: 0x0400653C RID: 25916
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Red and toValue.Rec.")]
		public CurveFsmAction.Calculation calculationR;

		// Token: 0x0400653D RID: 25917
		[RequiredField]
		[Tooltip("The curve used to animate the green value.")]
		public FsmAnimationCurve curveG;

		// Token: 0x0400653E RID: 25918
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Green and toValue.Green.")]
		public CurveFsmAction.Calculation calculationG;

		// Token: 0x0400653F RID: 25919
		[RequiredField]
		[Tooltip("The curve used to animate the blue value.")]
		public FsmAnimationCurve curveB;

		// Token: 0x04006540 RID: 25920
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Blue and toValue.Blue.")]
		public CurveFsmAction.Calculation calculationB;

		// Token: 0x04006541 RID: 25921
		[RequiredField]
		[Tooltip("The curve used to animate the alpha value.")]
		public FsmAnimationCurve curveA;

		// Token: 0x04006542 RID: 25922
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Alpha and toValue.Alpha.")]
		public CurveFsmAction.Calculation calculationA;

		// Token: 0x04006543 RID: 25923
		private Color clr;

		// Token: 0x04006544 RID: 25924
		private bool finishInNextStep;
	}
}
