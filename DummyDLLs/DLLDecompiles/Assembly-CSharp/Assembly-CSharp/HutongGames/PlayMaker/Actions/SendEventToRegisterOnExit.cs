using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012D4 RID: 4820
	[ActionCategory("Hollow Knight")]
	public class SendEventToRegisterOnExit : FsmStateAction
	{
		// Token: 0x06007DCA RID: 32202 RVA: 0x002574E3 File Offset: 0x002556E3
		public override void Reset()
		{
			this.EventName = new FsmString();
			this.ExcludeTarget = new FsmOwnerDefault();
		}

		// Token: 0x06007DCB RID: 32203 RVA: 0x002574FB File Offset: 0x002556FB
		public override void Awake()
		{
			this.eventNameHash = ((!this.EventName.UsesVariable) ? EventRegister.GetEventHashCode(this.EventName.Value) : 0);
		}

		// Token: 0x06007DCC RID: 32204 RVA: 0x00257523 File Offset: 0x00255723
		public override void OnExit()
		{
			if (this.eventNameHash != 0)
			{
				EventRegister.SendEvent(this.eventNameHash, this.ExcludeTarget.GetSafe(this));
				return;
			}
			EventRegister.SendEvent(this.EventName.Value, this.ExcludeTarget.GetSafe(this));
		}

		// Token: 0x04007DAF RID: 32175
		public FsmString EventName;

		// Token: 0x04007DB0 RID: 32176
		public FsmOwnerDefault ExcludeTarget;

		// Token: 0x04007DB1 RID: 32177
		private int eventNameHash;
	}
}
