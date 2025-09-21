using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DC2 RID: 3522
	[ActionCategory("AnimateVariables")]
	[Tooltip("Animates the value of a Rect Variable FROM-TO with assistance of Deformation Curves.")]
	public class CurveRect : CurveFsmAction
	{
		// Token: 0x06006606 RID: 26118 RVA: 0x0020492E File Offset: 0x00202B2E
		public override void Reset()
		{
			base.Reset();
			this.rectVariable = new FsmRect
			{
				UseVariable = true
			};
			this.toValue = new FsmRect
			{
				UseVariable = true
			};
			this.fromValue = new FsmRect
			{
				UseVariable = true
			};
		}

		// Token: 0x06006607 RID: 26119 RVA: 0x0020496C File Offset: 0x00202B6C
		public override void OnEnter()
		{
			base.OnEnter();
			this.finishInNextStep = false;
			this.resultFloats = new float[4];
			this.fromFloats = new float[4];
			this.fromFloats[0] = (this.fromValue.IsNone ? 0f : this.fromValue.Value.x);
			this.fromFloats[1] = (this.fromValue.IsNone ? 0f : this.fromValue.Value.y);
			this.fromFloats[2] = (this.fromValue.IsNone ? 0f : this.fromValue.Value.width);
			this.fromFloats[3] = (this.fromValue.IsNone ? 0f : this.fromValue.Value.height);
			this.toFloats = new float[4];
			this.toFloats[0] = (this.toValue.IsNone ? 0f : this.toValue.Value.x);
			this.toFloats[1] = (this.toValue.IsNone ? 0f : this.toValue.Value.y);
			this.toFloats[2] = (this.toValue.IsNone ? 0f : this.toValue.Value.width);
			this.toFloats[3] = (this.toValue.IsNone ? 0f : this.toValue.Value.height);
			this.curves = new AnimationCurve[4];
			this.curves[0] = this.curveX.curve;
			this.curves[1] = this.curveY.curve;
			this.curves[2] = this.curveW.curve;
			this.curves[3] = this.curveH.curve;
			this.calculations = new CurveFsmAction.Calculation[4];
			this.calculations[0] = this.calculationX;
			this.calculations[1] = this.calculationY;
			this.calculations[2] = this.calculationW;
			this.calculations[2] = this.calculationH;
			base.Init();
		}

		// Token: 0x06006608 RID: 26120 RVA: 0x00204BC4 File Offset: 0x00202DC4
		public override void OnExit()
		{
		}

		// Token: 0x06006609 RID: 26121 RVA: 0x00204BC8 File Offset: 0x00202DC8
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.rectVariable.IsNone && this.isRunning)
			{
				this.rct = new Rect(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
				this.rectVariable.Value = this.rct;
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
				if (!this.rectVariable.IsNone)
				{
					this.rct = new Rect(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
					this.rectVariable.Value = this.rct;
				}
				this.finishInNextStep = true;
			}
		}

		// Token: 0x04006563 RID: 25955
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Rect Variable to animate.")]
		public FsmRect rectVariable;

		// Token: 0x04006564 RID: 25956
		[RequiredField]
		[Tooltip("The Rect to animate from.")]
		public FsmRect fromValue;

		// Token: 0x04006565 RID: 25957
		[RequiredField]
		[Tooltip("The Rect to animate to.")]
		public FsmRect toValue;

		// Token: 0x04006566 RID: 25958
		[RequiredField]
		[Tooltip("Curve that controls the X value.")]
		public FsmAnimationCurve curveX;

		// Token: 0x04006567 RID: 25959
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.x and toValue.x.")]
		public CurveFsmAction.Calculation calculationX;

		// Token: 0x04006568 RID: 25960
		[RequiredField]
		[Tooltip("Curve that controls the Y value.")]
		public FsmAnimationCurve curveY;

		// Token: 0x04006569 RID: 25961
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.y and toValue.y.")]
		public CurveFsmAction.Calculation calculationY;

		// Token: 0x0400656A RID: 25962
		[RequiredField]
		[Tooltip("Curve that controls the Width.")]
		public FsmAnimationCurve curveW;

		// Token: 0x0400656B RID: 25963
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.width and toValue.width.")]
		public CurveFsmAction.Calculation calculationW;

		// Token: 0x0400656C RID: 25964
		[RequiredField]
		[Tooltip("Curve that controls the Height.")]
		public FsmAnimationCurve curveH;

		// Token: 0x0400656D RID: 25965
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.height and toValue.height.")]
		public CurveFsmAction.Calculation calculationH;

		// Token: 0x0400656E RID: 25966
		private Rect rct;

		// Token: 0x0400656F RID: 25967
		private bool finishInNextStep;
	}
}
