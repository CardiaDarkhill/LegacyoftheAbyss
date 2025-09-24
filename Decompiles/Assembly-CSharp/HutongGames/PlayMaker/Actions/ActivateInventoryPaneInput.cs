using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001286 RID: 4742
	public class ActivateInventoryPaneInput : FsmStateAction
	{
		// Token: 0x06007CB6 RID: 31926 RVA: 0x00254347 File Offset: 0x00252547
		public override void Reset()
		{
			this.Target = null;
			this.Activate = null;
		}

		// Token: 0x06007CB7 RID: 31927 RVA: 0x00254357 File Offset: 0x00252557
		public override void OnEnter()
		{
			this.Target.GetSafe(this).GetComponent<InventoryPaneInput>().enabled = this.Activate.Value;
			base.Finish();
		}

		// Token: 0x04007CCF RID: 31951
		public FsmOwnerDefault Target;

		// Token: 0x04007CD0 RID: 31952
		public FsmBool Activate;
	}
}
