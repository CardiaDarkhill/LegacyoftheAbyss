using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DBE RID: 3518
	[ActionCategory(ActionCategory.AnimateVariables)]
	[Tooltip("Animates the value of a Vector3 Variable using an Animation Curve.")]
	public class AnimateVector3 : AnimateFsmAction
	{
		// Token: 0x060065F2 RID: 26098 RVA: 0x00203549 File Offset: 0x00201749
		public override void Reset()
		{
			base.Reset();
			this.vectorVariable = new FsmVector3
			{
				UseVariable = true
			};
		}

		// Token: 0x060065F3 RID: 26099 RVA: 0x00203564 File Offset: 0x00201764
		public override void OnEnter()
		{
			base.OnEnter();
			this.finishInNextStep = false;
			this.resultFloats = new float[3];
			this.fromFloats = new float[3];
			this.fromFloats[0] = (this.vectorVariable.IsNone ? 0f : this.vectorVariable.Value.x);
			this.fromFloats[1] = (this.vectorVariable.IsNone ? 0f : this.vectorVariable.Value.y);
			this.fromFloats[2] = (this.vectorVariable.IsNone ? 0f : this.vectorVariable.Value.z);
			this.curves = new AnimationCurve[3];
			this.curves[0] = this.curveX.curve;
			this.curves[1] = this.curveY.curve;
			this.curves[2] = this.curveZ.curve;
			this.calculations = new AnimateFsmAction.Calculation[3];
			this.calculations[0] = this.calculationX;
			this.calculations[1] = this.calculationY;
			this.calculations[2] = this.calculationZ;
			base.Init();
			if (Math.Abs(this.delay.Value) < 0.01f)
			{
				this.UpdateVariableValue();
			}
		}

		// Token: 0x060065F4 RID: 26100 RVA: 0x002036B8 File Offset: 0x002018B8
		private void UpdateVariableValue()
		{
			if (!this.vectorVariable.IsNone)
			{
				this.vectorVariable.Value = new Vector3(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2]);
			}
		}

		// Token: 0x060065F5 RID: 26101 RVA: 0x002036F0 File Offset: 0x002018F0
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

		// Token: 0x04006530 RID: 25904
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Vector3 Variable to animate.")]
		public FsmVector3 vectorVariable;

		// Token: 0x04006531 RID: 25905
		[RequiredField]
		[Tooltip("Curve to use for the X value.")]
		public FsmAnimationCurve curveX;

		// Token: 0x04006532 RID: 25906
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to vectorVariable.x.")]
		public AnimateFsmAction.Calculation calculationX;

		// Token: 0x04006533 RID: 25907
		[RequiredField]
		[Tooltip("Curve to use for the Y value.")]
		public FsmAnimationCurve curveY;

		// Token: 0x04006534 RID: 25908
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to vectorVariable.y.")]
		public AnimateFsmAction.Calculation calculationY;

		// Token: 0x04006535 RID: 25909
		[RequiredField]
		[Tooltip("Curve to use for the Z value.")]
		public FsmAnimationCurve curveZ;

		// Token: 0x04006536 RID: 25910
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to vectorVariable.z.")]
		public AnimateFsmAction.Calculation calculationZ;

		// Token: 0x04006537 RID: 25911
		private bool finishInNextStep;
	}
}
