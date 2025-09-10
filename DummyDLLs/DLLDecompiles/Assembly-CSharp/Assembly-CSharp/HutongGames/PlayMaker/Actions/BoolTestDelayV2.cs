using System;
using TeamCherry.SharedUtils;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BCD RID: 3021
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends Events based on the value of a Boolean Variable.")]
	public class BoolTestDelayV2 : FsmStateAction
	{
		// Token: 0x06005CBE RID: 23742 RVA: 0x001D2CCB File Offset: 0x001D0ECB
		public override void Reset()
		{
			this.BoolVariable = null;
			this.IsTrue = null;
			this.IsFalse = null;
			this.MinDelay = null;
			this.MaxDelay = null;
		}

		// Token: 0x06005CBF RID: 23743 RVA: 0x001D2CF0 File Offset: 0x001D0EF0
		public override void OnEnter()
		{
			this.ResetTimer();
		}

		// Token: 0x06005CC0 RID: 23744 RVA: 0x001D2CF8 File Offset: 0x001D0EF8
		public override void OnUpdate()
		{
			if (this.BoolVariable.Value != this.previousBoolValue)
			{
				this.ResetTimer();
				return;
			}
			if (this.timer < this.delay)
			{
				this.timer += Time.deltaTime;
				return;
			}
			base.Fsm.Event(this.BoolVariable.Value ? this.IsTrue : this.IsFalse);
		}

		// Token: 0x06005CC1 RID: 23745 RVA: 0x001D2D68 File Offset: 0x001D0F68
		private void ResetTimer()
		{
			this.timer = 0f;
			this.delay = new MinMaxFloat(this.MinDelay.Value, this.MaxDelay.Value).GetRandomValue();
			this.previousBoolValue = this.BoolVariable.Value;
		}

		// Token: 0x04005853 RID: 22611
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Readonly]
		[Tooltip("The Bool variable to test.")]
		public FsmBool BoolVariable;

		// Token: 0x04005854 RID: 22612
		[Tooltip("Event to send if the Bool variable is True.")]
		public FsmEvent IsTrue;

		// Token: 0x04005855 RID: 22613
		[Tooltip("Event to send if the Bool variable is False.")]
		public FsmEvent IsFalse;

		// Token: 0x04005856 RID: 22614
		public FsmFloat MinDelay;

		// Token: 0x04005857 RID: 22615
		public FsmFloat MaxDelay;

		// Token: 0x04005858 RID: 22616
		private float timer;

		// Token: 0x04005859 RID: 22617
		private float delay;

		// Token: 0x0400585A RID: 22618
		private bool previousBoolValue;
	}
}
