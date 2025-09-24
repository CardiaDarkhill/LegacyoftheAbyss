using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D20 RID: 3360
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets the value of a Bool Variable after a certain point in time has passed.")]
	public class SetBoolValueAtTime : FsmStateAction
	{
		// Token: 0x0600631A RID: 25370 RVA: 0x001F5553 File Offset: 0x001F3753
		public override void Reset()
		{
			this.BoolVariable = null;
			this.BoolValue = null;
			this.Time = null;
		}

		// Token: 0x0600631B RID: 25371 RVA: 0x001F556C File Offset: 0x001F376C
		public override void OnEnter()
		{
			this.timer = 0f;
			if (this.SetOppositeOnStateEntry)
			{
				this.BoolVariable.Value = !this.BoolValue.Value;
			}
			if (this.Time.Value <= 0f)
			{
				this.Set();
			}
		}

		// Token: 0x0600631C RID: 25372 RVA: 0x001F55BD File Offset: 0x001F37BD
		public override void OnUpdate()
		{
			if (this.timer >= this.Time.Value)
			{
				this.Set();
				return;
			}
			this.timer += UnityEngine.Time.deltaTime;
		}

		// Token: 0x0600631D RID: 25373 RVA: 0x001F55EB File Offset: 0x001F37EB
		private void Set()
		{
			this.BoolVariable.Value = this.BoolValue.Value;
			base.Finish();
		}

		// Token: 0x04006188 RID: 24968
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmBool BoolVariable;

		// Token: 0x04006189 RID: 24969
		[RequiredField]
		public FsmBool BoolValue;

		// Token: 0x0400618A RID: 24970
		public FsmFloat Time;

		// Token: 0x0400618B RID: 24971
		public bool SetOppositeOnStateEntry;

		// Token: 0x0400618C RID: 24972
		private float timer;
	}
}
