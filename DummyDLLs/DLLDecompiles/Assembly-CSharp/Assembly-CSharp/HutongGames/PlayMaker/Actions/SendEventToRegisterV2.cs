using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012D2 RID: 4818
	[ActionCategory("Hollow Knight")]
	public class SendEventToRegisterV2 : FsmStateAction
	{
		// Token: 0x06007DC0 RID: 32192 RVA: 0x00257384 File Offset: 0x00255584
		public override void Reset()
		{
			this.EventName = new FsmString();
			this.ExcludeTarget = new FsmOwnerDefault();
		}

		// Token: 0x06007DC1 RID: 32193 RVA: 0x0025739C File Offset: 0x0025559C
		public override void Awake()
		{
			this.eventNameHash = ((!this.EventName.UsesVariable) ? EventRegister.GetEventHashCode(this.EventName.Value) : 0);
		}

		// Token: 0x06007DC2 RID: 32194 RVA: 0x002573C4 File Offset: 0x002555C4
		public override void OnEnter()
		{
			if (this.eventNameHash != 0)
			{
				EventRegister.SendEvent(this.eventNameHash, this.ExcludeTarget.GetSafe(this));
			}
			else
			{
				EventRegister.SendEvent(this.EventName.Value, this.ExcludeTarget.GetSafe(this));
			}
			base.Finish();
		}

		// Token: 0x04007DA7 RID: 32167
		public FsmString EventName;

		// Token: 0x04007DA8 RID: 32168
		public FsmOwnerDefault ExcludeTarget;

		// Token: 0x04007DA9 RID: 32169
		private int eventNameHash;
	}
}
