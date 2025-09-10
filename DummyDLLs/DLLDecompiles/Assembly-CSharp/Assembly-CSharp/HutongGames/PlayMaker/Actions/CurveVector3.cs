using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DC3 RID: 3523
	[ActionCategory(ActionCategory.AnimateVariables)]
	[Tooltip("Animates the value of a Vector3 Variable FROM-TO with assistance of Deformation Curves.")]
	public class CurveVector3 : CurveFsmAction
	{
		// Token: 0x0600660B RID: 26123 RVA: 0x00204CC3 File Offset: 0x00202EC3
		public override void Reset()
		{
			base.Reset();
			this.vectorVariable = new FsmVector3
			{
				UseVariable = true
			};
			this.toValue = new FsmVector3
			{
				UseVariable = true
			};
			this.fromValue = new FsmVector3
			{
				UseVariable = true
			};
		}

		// Token: 0x0600660C RID: 26124 RVA: 0x00204D04 File Offset: 0x00202F04
		public override void OnEnter()
		{
			base.OnEnter();
			this.finishInNextStep = false;
			this.resultFloats = new float[3];
			this.fromFloats = new float[3];
			this.fromFloats[0] = (this.fromValue.IsNone ? 0f : this.fromValue.Value.x);
			this.fromFloats[1] = (this.fromValue.IsNone ? 0f : this.fromValue.Value.y);
			this.fromFloats[2] = (this.fromValue.IsNone ? 0f : this.fromValue.Value.z);
			this.toFloats = new float[3];
			this.toFloats[0] = (this.toValue.IsNone ? 0f : this.toValue.Value.x);
			this.toFloats[1] = (this.toValue.IsNone ? 0f : this.toValue.Value.y);
			this.toFloats[2] = (this.toValue.IsNone ? 0f : this.toValue.Value.z);
			this.curves = new AnimationCurve[3];
			this.curves[0] = this.curveX.curve;
			this.curves[1] = this.curveY.curve;
			this.curves[2] = this.curveZ.curve;
			this.calculations = new CurveFsmAction.Calculation[3];
			this.calculations[0] = this.calculationX;
			this.calculations[1] = this.calculationY;
			this.calculations[2] = this.calculationZ;
			base.Init();
		}

		// Token: 0x0600660D RID: 26125 RVA: 0x00204ECB File Offset: 0x002030CB
		public override void OnExit()
		{
		}

		// Token: 0x0600660E RID: 26126 RVA: 0x00204ED0 File Offset: 0x002030D0
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.vectorVariable.IsNone && this.isRunning)
			{
				this.vct = new Vector3(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2]);
				this.vectorVariable.Value = this.vct;
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
				if (!this.vectorVariable.IsNone)
				{
					this.vct = new Vector3(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2]);
					this.vectorVariable.Value = this.vct;
				}
				this.finishInNextStep = true;
			}
		}

		// Token: 0x04006570 RID: 25968
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Vector3 variable to animate.")]
		public FsmVector3 vectorVariable;

		// Token: 0x04006571 RID: 25969
		[RequiredField]
		[Tooltip("Animate from this value.")]
		public FsmVector3 fromValue;

		// Token: 0x04006572 RID: 25970
		[RequiredField]
		[Tooltip("Animate to this value.")]
		public FsmVector3 toValue;

		// Token: 0x04006573 RID: 25971
		[RequiredField]
		[Tooltip("Curve that controls the X value.")]
		public FsmAnimationCurve curveX;

		// Token: 0x04006574 RID: 25972
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.x and toValue.x.")]
		public CurveFsmAction.Calculation calculationX;

		// Token: 0x04006575 RID: 25973
		[RequiredField]
		[Tooltip("Curve that controls the Y value.")]
		public FsmAnimationCurve curveY;

		// Token: 0x04006576 RID: 25974
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.y and toValue.y.")]
		public CurveFsmAction.Calculation calculationY;

		// Token: 0x04006577 RID: 25975
		[RequiredField]
		[Tooltip("Curve that controls the Z value.")]
		public FsmAnimationCurve curveZ;

		// Token: 0x04006578 RID: 25976
		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.z and toValue.z.")]
		public CurveFsmAction.Calculation calculationZ;

		// Token: 0x04006579 RID: 25977
		private Vector3 vct;

		// Token: 0x0400657A RID: 25978
		private bool finishInNextStep;
	}
}
