using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DB7 RID: 3511
	[ActionCategory(ActionCategory.AnimateVariables)]
	[Tooltip("Animates the value of a Color Variable using an Animation Curve.")]
	public class AnimateColor : AnimateFsmAction
	{
		// Token: 0x060065CC RID: 26060 RVA: 0x00201D6E File Offset: 0x001FFF6E
		public override void Reset()
		{
			base.Reset();
			this.colorVariable = new FsmColor
			{
				UseVariable = true
			};
		}

		// Token: 0x060065CD RID: 26061 RVA: 0x00201D88 File Offset: 0x001FFF88
		public override void OnEnter()
		{
			base.OnEnter();
			this.finishInNextStep = false;
			this.resultFloats = new float[4];
			this.fromFloats = new float[4];
			this.fromFloats[0] = (this.colorVariable.IsNone ? 0f : this.colorVariable.Value.r);
			this.fromFloats[1] = (this.colorVariable.IsNone ? 0f : this.colorVariable.Value.g);
			this.fromFloats[2] = (this.colorVariable.IsNone ? 0f : this.colorVariable.Value.b);
			this.fromFloats[3] = (this.colorVariable.IsNone ? 0f : this.colorVariable.Value.a);
			this.curves = new AnimationCurve[4];
			this.curves[0] = this.curveR.curve;
			this.curves[1] = this.curveG.curve;
			this.curves[2] = this.curveB.curve;
			this.curves[3] = this.curveA.curve;
			this.calculations = new AnimateFsmAction.Calculation[4];
			this.calculations[0] = this.calculationR;
			this.calculations[1] = this.calculationG;
			this.calculations[2] = this.calculationB;
			this.calculations[3] = this.calculationA;
			base.Init();
			if (Math.Abs(this.delay.Value) < 0.01f)
			{
				this.UpdateVariableValue();
			}
		}

		// Token: 0x060065CE RID: 26062 RVA: 0x00201F29 File Offset: 0x00200129
		private void UpdateVariableValue()
		{
			if (!this.colorVariable.IsNone)
			{
				this.colorVariable.Value = new Color(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
			}
		}

		// Token: 0x060065CF RID: 26063 RVA: 0x00201F68 File Offset: 0x00200168
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

		// Token: 0x040064EC RID: 25836
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Color Variable to animate.")]
		public FsmColor colorVariable;

		// Token: 0x040064ED RID: 25837
		[RequiredField]
		[Tooltip("The curve used to animate the red value.")]
		public FsmAnimationCurve curveR;

		// Token: 0x040064EE RID: 25838
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to the red channel.")]
		public AnimateFsmAction.Calculation calculationR;

		// Token: 0x040064EF RID: 25839
		[RequiredField]
		[Tooltip("The curve used to animate the green value.")]
		public FsmAnimationCurve curveG;

		// Token: 0x040064F0 RID: 25840
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to the green channel.")]
		public AnimateFsmAction.Calculation calculationG;

		// Token: 0x040064F1 RID: 25841
		[RequiredField]
		[Tooltip("The curve used to animate the blue value.")]
		public FsmAnimationCurve curveB;

		// Token: 0x040064F2 RID: 25842
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to the blue channel.")]
		public AnimateFsmAction.Calculation calculationB;

		// Token: 0x040064F3 RID: 25843
		[RequiredField]
		[Tooltip("The curve used to animate the alpha value.")]
		public FsmAnimationCurve curveA;

		// Token: 0x040064F4 RID: 25844
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to the alpha channel.")]
		public AnimateFsmAction.Calculation calculationA;

		// Token: 0x040064F5 RID: 25845
		private bool finishInNextStep;
	}
}
