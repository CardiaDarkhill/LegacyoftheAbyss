using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DC9 RID: 3529
	[ActionCategory(ActionCategory.AnimateVariables)]
	[Tooltip("Easing Animation - Vector3")]
	public class EaseVector3 : EaseFsmAction
	{
		// Token: 0x06006649 RID: 26185 RVA: 0x00206998 File Offset: 0x00204B98
		public override void Reset()
		{
			base.Reset();
			this.vector3Variable = null;
			this.fromValue = null;
			this.toValue = null;
			this.finishInNextStep = false;
		}

		// Token: 0x0600664A RID: 26186 RVA: 0x002069BC File Offset: 0x00204BBC
		public override void OnEnter()
		{
			base.OnEnter();
			this.fromFloats = new float[3];
			this.fromFloats[0] = this.fromValue.Value.x;
			this.fromFloats[1] = this.fromValue.Value.y;
			this.fromFloats[2] = this.fromValue.Value.z;
			this.toFloats = new float[3];
			this.toFloats[0] = this.toValue.Value.x;
			this.toFloats[1] = this.toValue.Value.y;
			this.toFloats[2] = this.toValue.Value.z;
			this.resultFloats = new float[3];
			this.finishInNextStep = false;
			this.vector3Variable.Value = (this.reverse.IsNone ? this.fromValue.Value : (this.reverse.Value ? this.toValue.Value : this.fromValue.Value));
		}

		// Token: 0x0600664B RID: 26187 RVA: 0x00206AD4 File Offset: 0x00204CD4
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600664C RID: 26188 RVA: 0x00206ADC File Offset: 0x00204CDC
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.vector3Variable.IsNone && this.isRunning)
			{
				this.vector3Variable.Value = new Vector3(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2]);
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
				if (!this.vector3Variable.IsNone)
				{
					this.vector3Variable.Value = new Vector3(this.reverse.IsNone ? this.toValue.Value.x : (this.reverse.Value ? this.fromValue.Value.x : this.toValue.Value.x), this.reverse.IsNone ? this.toValue.Value.y : (this.reverse.Value ? this.fromValue.Value.y : this.toValue.Value.y), this.reverse.IsNone ? this.toValue.Value.z : (this.reverse.Value ? this.fromValue.Value.z : this.toValue.Value.z));
				}
				this.finishInNextStep = true;
			}
		}

		// Token: 0x040065A4 RID: 26020
		[RequiredField]
		[Tooltip("The Vector3 value to ease from.")]
		public FsmVector3 fromValue;

		// Token: 0x040065A5 RID: 26021
		[RequiredField]
		[Tooltip("The Vector3 value to ease to.")]
		public FsmVector3 toValue;

		// Token: 0x040065A6 RID: 26022
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Vector3 Variable.")]
		public FsmVector3 vector3Variable;

		// Token: 0x040065A7 RID: 26023
		private bool finishInNextStep;
	}
}
