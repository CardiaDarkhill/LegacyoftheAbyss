using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C07 RID: 3079
	[ActionCategory("Object Pool")]
	[Tooltip("Creates an Object Pool")]
	public class CreatePool : FsmStateAction
	{
		// Token: 0x06005E03 RID: 24067 RVA: 0x001DA0D5 File Offset: 0x001D82D5
		public override void OnEnter()
		{
			if (base.Owner != null)
			{
				base.Owner.Recycle();
			}
			base.Finish();
		}
	}
}
