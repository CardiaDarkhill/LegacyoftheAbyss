using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012FF RID: 4863
	public class DropPlatformResolutionModeInScene : FsmStateAction
	{
		// Token: 0x06007E78 RID: 32376 RVA: 0x002590E0 File Offset: 0x002572E0
		public override void Reset()
		{
			this.ResolutionMode = null;
		}

		// Token: 0x06007E79 RID: 32377 RVA: 0x002590E9 File Offset: 0x002572E9
		public override void OnEnter()
		{
			Platform.Current.DropResolutionModeInScene((Platform.ResolutionModes)this.ResolutionMode.Value);
			base.Finish();
		}

		// Token: 0x04007E34 RID: 32308
		[ObjectType(typeof(Platform.ResolutionModes))]
		public FsmEnum ResolutionMode;
	}
}
