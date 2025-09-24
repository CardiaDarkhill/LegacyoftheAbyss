using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DB9 RID: 3513
	[ActionCategory(ActionCategory.AnimateVariables)]
	[Tooltip("Animates the value of a Float Variable using an Animation Curve.")]
	public class AnimateFloatV2 : AnimateFsmAction
	{
		// Token: 0x060065D5 RID: 26069 RVA: 0x0020216E File Offset: 0x0020036E
		public override void Reset()
		{
			base.Reset();
			this.floatVariable = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x060065D6 RID: 26070 RVA: 0x00202188 File Offset: 0x00200388
		public override void OnEnter()
		{
			base.OnEnter();
			this.finishInNextStep = false;
			this.resultFloats = new float[1];
			this.fromFloats = new float[1];
			this.fromFloats[0] = (this.floatVariable.IsNone ? 0f : this.floatVariable.Value);
			this.calculations = new AnimateFsmAction.Calculation[1];
			this.calculations[0] = this.calculation;
			this.curves = new AnimationCurve[1];
			this.curves[0] = this.animCurve.curve;
			base.Init();
		}

		// Token: 0x060065D7 RID: 26071 RVA: 0x00202220 File Offset: 0x00200420
		public override void OnExit()
		{
		}

		// Token: 0x060065D8 RID: 26072 RVA: 0x00202224 File Offset: 0x00200424
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

		// Token: 0x040064FE RID: 25854
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The float variable to animate.")]
		public FsmFloat floatVariable;

		// Token: 0x040064FF RID: 25855
		[RequiredField]
		[Tooltip("The animation curve to use.")]
		public FsmAnimationCurve animCurve;

		// Token: 0x04006500 RID: 25856
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to floatVariable")]
		public AnimateFsmAction.Calculation calculation;

		// Token: 0x04006501 RID: 25857
		private bool finishInNextStep;
	}
}
