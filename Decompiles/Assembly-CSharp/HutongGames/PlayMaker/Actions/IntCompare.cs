using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F58 RID: 3928
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends Events based on the comparison of 2 Integers.")]
	public class IntCompare : FsmStateAction
	{
		// Token: 0x06006D20 RID: 27936 RVA: 0x0021F6B2 File Offset: 0x0021D8B2
		public override void Reset()
		{
			this.integer1 = 0;
			this.integer2 = 0;
			this.equal = null;
			this.lessThan = null;
			this.greaterThan = null;
			this.everyFrame = false;
		}

		// Token: 0x06006D21 RID: 27937 RVA: 0x0021F6E8 File Offset: 0x0021D8E8
		public override void OnEnter()
		{
			this.DoIntCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D22 RID: 27938 RVA: 0x0021F6FE File Offset: 0x0021D8FE
		public override void OnUpdate()
		{
			this.DoIntCompare();
		}

		// Token: 0x06006D23 RID: 27939 RVA: 0x0021F708 File Offset: 0x0021D908
		private void DoIntCompare()
		{
			if (this.integer1.Value == this.integer2.Value)
			{
				base.Fsm.Event(this.equal);
				return;
			}
			if (this.integer1.Value < this.integer2.Value)
			{
				base.Fsm.Event(this.lessThan);
				return;
			}
			if (this.integer1.Value > this.integer2.Value)
			{
				base.Fsm.Event(this.greaterThan);
			}
		}

		// Token: 0x06006D24 RID: 27940 RVA: 0x0021F792 File Offset: 0x0021D992
		public override string ErrorCheck()
		{
			if (FsmEvent.IsNullOrEmpty(this.equal) && FsmEvent.IsNullOrEmpty(this.lessThan) && FsmEvent.IsNullOrEmpty(this.greaterThan))
			{
				return "Action sends no events!";
			}
			return "";
		}

		// Token: 0x04006CE3 RID: 27875
		[RequiredField]
		[Tooltip("The first integer.")]
		public FsmInt integer1;

		// Token: 0x04006CE4 RID: 27876
		[RequiredField]
		[Tooltip("The second integer.")]
		public FsmInt integer2;

		// Token: 0x04006CE5 RID: 27877
		[Tooltip("Event sent if Integer 1 equals Integer 2")]
		public FsmEvent equal;

		// Token: 0x04006CE6 RID: 27878
		[Tooltip("Event sent if Integer 1 is less than Integer 2")]
		public FsmEvent lessThan;

		// Token: 0x04006CE7 RID: 27879
		[Tooltip("Event sent if Integer 1 is greater than Integer 2")]
		public FsmEvent greaterThan;

		// Token: 0x04006CE8 RID: 27880
		[Tooltip("Perform this action every frame. Useful if you want to wait for a comparison to be true before sending an event.")]
		public bool everyFrame;
	}
}
