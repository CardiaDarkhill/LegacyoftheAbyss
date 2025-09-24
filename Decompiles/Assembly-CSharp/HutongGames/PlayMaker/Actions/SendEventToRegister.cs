using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012D1 RID: 4817
	[ActionCategory("Hollow Knight")]
	public class SendEventToRegister : FsmStateAction
	{
		// Token: 0x06007DBD RID: 32189 RVA: 0x00257356 File Offset: 0x00255556
		public override void Reset()
		{
			this.eventName = new FsmString();
		}

		// Token: 0x06007DBE RID: 32190 RVA: 0x00257363 File Offset: 0x00255563
		public override void OnEnter()
		{
			EventRegister.SendEvent(this.eventName.Value, null);
			base.Finish();
		}

		// Token: 0x04007DA6 RID: 32166
		public FsmString eventName;
	}
}
