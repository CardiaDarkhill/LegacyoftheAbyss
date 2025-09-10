using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012D3 RID: 4819
	[ActionCategory("Hollow Knight")]
	public class SendEventToRegisterDelay : FsmStateAction
	{
		// Token: 0x06007DC4 RID: 32196 RVA: 0x0025741C File Offset: 0x0025561C
		public override void Reset()
		{
			this.EventName = new FsmString();
			this.ExcludeTarget = new FsmOwnerDefault();
		}

		// Token: 0x06007DC5 RID: 32197 RVA: 0x00257434 File Offset: 0x00255634
		public override void Awake()
		{
			this.eventNameHash = ((!this.EventName.UsesVariable) ? EventRegister.GetEventHashCode(this.EventName.Value) : 0);
		}

		// Token: 0x06007DC6 RID: 32198 RVA: 0x0025745C File Offset: 0x0025565C
		public override void OnEnter()
		{
			this.timer = this.delay.Value;
		}

		// Token: 0x06007DC7 RID: 32199 RVA: 0x0025746F File Offset: 0x0025566F
		public override void OnUpdate()
		{
			if (this.timer > 0f)
			{
				this.timer -= Time.deltaTime;
				return;
			}
			this.SendToRegister();
			base.Finish();
		}

		// Token: 0x06007DC8 RID: 32200 RVA: 0x0025749D File Offset: 0x0025569D
		private void SendToRegister()
		{
			if (this.eventNameHash != 0)
			{
				EventRegister.SendEvent(this.eventNameHash, this.ExcludeTarget.GetSafe(this));
				return;
			}
			EventRegister.SendEvent(this.EventName.Value, this.ExcludeTarget.GetSafe(this));
		}

		// Token: 0x04007DAA RID: 32170
		public FsmString EventName;

		// Token: 0x04007DAB RID: 32171
		public FsmFloat delay;

		// Token: 0x04007DAC RID: 32172
		public FsmOwnerDefault ExcludeTarget;

		// Token: 0x04007DAD RID: 32173
		private int eventNameHash;

		// Token: 0x04007DAE RID: 32174
		private float timer;
	}
}
