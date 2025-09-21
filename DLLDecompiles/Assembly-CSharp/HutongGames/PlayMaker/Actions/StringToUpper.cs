using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D83 RID: 3459
	public class StringToUpper : FsmStateAction
	{
		// Token: 0x060064BC RID: 25788 RVA: 0x001FCC5E File Offset: 0x001FAE5E
		public override void Reset()
		{
			this.Source = null;
			this.Destination = null;
			this.EveryFrame = false;
		}

		// Token: 0x060064BD RID: 25789 RVA: 0x001FCC75 File Offset: 0x001FAE75
		public override void OnEnter()
		{
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060064BE RID: 25790 RVA: 0x001FCC8B File Offset: 0x001FAE8B
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x060064BF RID: 25791 RVA: 0x001FCC93 File Offset: 0x001FAE93
		private void DoAction()
		{
			this.Destination.Value = this.Source.Value.ToUpper();
		}

		// Token: 0x040063B4 RID: 25524
		[RequiredField]
		public FsmString Source;

		// Token: 0x040063B5 RID: 25525
		[UIHint(UIHint.Variable)]
		[RequiredField]
		public FsmString Destination;

		// Token: 0x040063B6 RID: 25526
		public bool EveryFrame;
	}
}
