using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001210 RID: 4624
	public class SetBoxColliderConfig : FSMUtility.GetComponentFsmStateAction<BoxCollider2DConfigs>
	{
		// Token: 0x06007AD9 RID: 31449 RVA: 0x0024DC9B File Offset: 0x0024BE9B
		public override void Reset()
		{
			base.Reset();
			this.Index = null;
		}

		// Token: 0x06007ADA RID: 31450 RVA: 0x0024DCAA File Offset: 0x0024BEAA
		protected override void DoAction(BoxCollider2DConfigs component)
		{
			component.SetConfig(this.Index.Value);
		}

		// Token: 0x04007B29 RID: 31529
		[RequiredField]
		public FsmInt Index;
	}
}
