using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DC5 RID: 3525
	[ActionCategory(ActionCategory.AnimateVariables)]
	[Tooltip("Easing Animation - Float")]
	public class EaseFloat : EaseFsmAction
	{
		// Token: 0x06006615 RID: 26133 RVA: 0x0020532C File Offset: 0x0020352C
		public override void Reset()
		{
			base.Reset();
			this.floatVariable = null;
			this.fromValue = null;
			this.toValue = null;
			this.finishInNextStep = false;
		}

		// Token: 0x06006616 RID: 26134 RVA: 0x00205350 File Offset: 0x00203550
		public override void OnEnter()
		{
			base.OnEnter();
			if (!this.isSetup)
			{
				this.isSetup = true;
				this.fromFloats = new float[1];
				this.toFloats = new float[1];
				this.resultFloats = new float[1];
			}
			this.fromFloats[0] = this.fromValue.Value;
			this.toFloats[0] = this.toValue.Value;
			this.finishInNextStep = false;
			if (this.delay.Value <= 0f)
			{
				this.floatVariable.Value = (this.reverse.IsNone ? this.fromValue.Value : (this.reverse.Value ? this.toValue.Value : this.fromValue.Value));
			}
		}

		// Token: 0x06006617 RID: 26135 RVA: 0x0020541F File Offset: 0x0020361F
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06006618 RID: 26136 RVA: 0x00205428 File Offset: 0x00203628
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.floatVariable.IsNone && this.isRunning)
			{
				this.floatVariable.Value = this.resultFloats[0];
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
				if (!this.floatVariable.IsNone)
				{
					this.floatVariable.Value = (this.reverse.IsNone ? this.toValue.Value : (this.reverse.Value ? this.fromValue.Value : this.toValue.Value));
				}
				this.finishInNextStep = true;
			}
		}

		// Token: 0x0400657F RID: 25983
		[RequiredField]
		[Tooltip("The float value to ease from.")]
		public FsmFloat fromValue;

		// Token: 0x04006580 RID: 25984
		[RequiredField]
		[Tooltip("The float value to ease to.")]
		public FsmFloat toValue;

		// Token: 0x04006581 RID: 25985
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Float Variable.")]
		public FsmFloat floatVariable;

		// Token: 0x04006582 RID: 25986
		private bool finishInNextStep;

		// Token: 0x04006583 RID: 25987
		private bool isSetup;
	}
}
