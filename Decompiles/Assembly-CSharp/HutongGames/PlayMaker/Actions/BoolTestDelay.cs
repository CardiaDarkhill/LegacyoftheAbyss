using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BCC RID: 3020
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends Events based on the value of a Boolean Variable.")]
	public class BoolTestDelay : FsmStateAction
	{
		// Token: 0x06005CBA RID: 23738 RVA: 0x001D2C3D File Offset: 0x001D0E3D
		public override void Reset()
		{
			this.boolVariable = null;
			this.isTrue = null;
			this.isFalse = null;
		}

		// Token: 0x06005CBB RID: 23739 RVA: 0x001D2C54 File Offset: 0x001D0E54
		public override void OnEnter()
		{
			this.timer = 0f;
		}

		// Token: 0x06005CBC RID: 23740 RVA: 0x001D2C64 File Offset: 0x001D0E64
		public override void OnUpdate()
		{
			if (this.timer < this.delay.Value)
			{
				this.timer += Time.deltaTime;
				return;
			}
			base.Fsm.Event(this.boolVariable.Value ? this.isTrue : this.isFalse);
			base.Finish();
		}

		// Token: 0x0400584E RID: 22606
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Readonly]
		[Tooltip("The Bool variable to test.")]
		public FsmBool boolVariable;

		// Token: 0x0400584F RID: 22607
		[Tooltip("Event to send if the Bool variable is True.")]
		public FsmEvent isTrue;

		// Token: 0x04005850 RID: 22608
		[Tooltip("Event to send if the Bool variable is False.")]
		public FsmEvent isFalse;

		// Token: 0x04005851 RID: 22609
		public FsmFloat delay;

		// Token: 0x04005852 RID: 22610
		private float timer;
	}
}
