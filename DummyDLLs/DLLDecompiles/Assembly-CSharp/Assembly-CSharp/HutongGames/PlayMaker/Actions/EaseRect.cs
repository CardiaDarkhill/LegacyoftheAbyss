using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DC8 RID: 3528
	[ActionCategory("AnimateVariables")]
	[Tooltip("Easing Animation - Rect.")]
	public class EaseRect : EaseFsmAction
	{
		// Token: 0x06006644 RID: 26180 RVA: 0x002065EA File Offset: 0x002047EA
		public override void Reset()
		{
			base.Reset();
			this.rectVariable = null;
			this.fromValue = null;
			this.toValue = null;
			this.finishInNextStep = false;
		}

		// Token: 0x06006645 RID: 26181 RVA: 0x00206610 File Offset: 0x00204810
		public override void OnEnter()
		{
			base.OnEnter();
			this.fromFloats = new float[4];
			this.fromFloats[0] = this.fromValue.Value.x;
			this.fromFloats[1] = this.fromValue.Value.y;
			this.fromFloats[2] = this.fromValue.Value.width;
			this.fromFloats[3] = this.fromValue.Value.height;
			this.toFloats = new float[4];
			this.toFloats[0] = this.toValue.Value.x;
			this.toFloats[1] = this.toValue.Value.y;
			this.toFloats[2] = this.toValue.Value.width;
			this.toFloats[3] = this.toValue.Value.height;
			this.resultFloats = new float[4];
			this.finishInNextStep = false;
			this.rectVariable.Value = (this.reverse.IsNone ? this.fromValue.Value : (this.reverse.Value ? this.toValue.Value : this.fromValue.Value));
		}

		// Token: 0x06006646 RID: 26182 RVA: 0x00206770 File Offset: 0x00204970
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06006647 RID: 26183 RVA: 0x00206778 File Offset: 0x00204978
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.rectVariable.IsNone && this.isRunning)
			{
				this.rectVariable.Value = new Rect(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
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
				if (!this.rectVariable.IsNone)
				{
					this.rectVariable.Value = new Rect(this.reverse.IsNone ? this.toValue.Value.x : (this.reverse.Value ? this.fromValue.Value.x : this.toValue.Value.x), this.reverse.IsNone ? this.toValue.Value.y : (this.reverse.Value ? this.fromValue.Value.y : this.toValue.Value.y), this.reverse.IsNone ? this.toValue.Value.width : (this.reverse.Value ? this.fromValue.Value.width : this.toValue.Value.width), this.reverse.IsNone ? this.toValue.Value.height : (this.reverse.Value ? this.fromValue.Value.height : this.toValue.Value.height));
				}
				this.finishInNextStep = true;
			}
		}

		// Token: 0x040065A0 RID: 26016
		[RequiredField]
		[Tooltip("Ease from this Rect value.")]
		public FsmRect fromValue;

		// Token: 0x040065A1 RID: 26017
		[RequiredField]
		[Tooltip("Ease to this Rect value.")]
		public FsmRect toValue;

		// Token: 0x040065A2 RID: 26018
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the current value in a Rect Variable.")]
		public FsmRect rectVariable;

		// Token: 0x040065A3 RID: 26019
		private bool finishInNextStep;
	}
}
