using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012EA RID: 4842
	[Tooltip("Activate / Disable Interaction System")]
	public class ActivateInteraction : FsmStateAction
	{
		// Token: 0x06007E38 RID: 32312 RVA: 0x002587E5 File Offset: 0x002569E5
		public override void Reset()
		{
			this.Activate = null;
		}

		// Token: 0x06007E39 RID: 32313 RVA: 0x002587EE File Offset: 0x002569EE
		public override void OnEnter()
		{
			InteractManager.IsDisabled = !this.Activate.Value;
			base.Finish();
		}

		// Token: 0x04007E0C RID: 32268
		[Tooltip("Activate / Disable Interaction System")]
		public FsmBool Activate;
	}
}
