using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DC4 RID: 3524
	[ActionCategory(ActionCategory.AnimateVariables)]
	[Tooltip("Easing Animation - Color")]
	public class EaseColor : EaseFsmAction
	{
		// Token: 0x06006610 RID: 26128 RVA: 0x00204FBB File Offset: 0x002031BB
		public override void Reset()
		{
			base.Reset();
			this.colorVariable = null;
			this.fromValue = null;
			this.toValue = null;
			this.finishInNextStep = false;
		}

		// Token: 0x06006611 RID: 26129 RVA: 0x00204FE0 File Offset: 0x002031E0
		public override void OnEnter()
		{
			base.OnEnter();
			this.fromFloats = new float[4];
			this.fromFloats[0] = this.fromValue.Value.r;
			this.fromFloats[1] = this.fromValue.Value.g;
			this.fromFloats[2] = this.fromValue.Value.b;
			this.fromFloats[3] = this.fromValue.Value.a;
			this.toFloats = new float[4];
			this.toFloats[0] = this.toValue.Value.r;
			this.toFloats[1] = this.toValue.Value.g;
			this.toFloats[2] = this.toValue.Value.b;
			this.toFloats[3] = this.toValue.Value.a;
			this.resultFloats = new float[4];
			this.finishInNextStep = false;
			this.colorVariable.Value = (this.reverse.IsNone ? this.fromValue.Value : (this.reverse.Value ? this.toValue.Value : this.fromValue.Value));
		}

		// Token: 0x06006612 RID: 26130 RVA: 0x00205128 File Offset: 0x00203328
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06006613 RID: 26131 RVA: 0x00205130 File Offset: 0x00203330
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.colorVariable.IsNone && this.isRunning)
			{
				this.colorVariable.Value = new Color(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
			}
			if (this.finishInNextStep)
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
					this.colorVariable.Value = new Color(this.reverse.IsNone ? this.toValue.Value.r : (this.reverse.Value ? this.fromValue.Value.r : this.toValue.Value.r), this.reverse.IsNone ? this.toValue.Value.g : (this.reverse.Value ? this.fromValue.Value.g : this.toValue.Value.g), this.reverse.IsNone ? this.toValue.Value.b : (this.reverse.Value ? this.fromValue.Value.b : this.toValue.Value.b), this.reverse.IsNone ? this.toValue.Value.a : (this.reverse.Value ? this.fromValue.Value.a : this.toValue.Value.a));
				}
				this.finishInNextStep = true;
			}
		}

		// Token: 0x0400657B RID: 25979
		[RequiredField]
		[Tooltip("The Color value to ease from.")]
		public FsmColor fromValue;

		// Token: 0x0400657C RID: 25980
		[RequiredField]
		[Tooltip("The Color value to ease to.")]
		public FsmColor toValue;

		// Token: 0x0400657D RID: 25981
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Color Variable.")]
		public FsmColor colorVariable;

		// Token: 0x0400657E RID: 25982
		private bool finishInNextStep;
	}
}
