using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CF7 RID: 3319
	[ActionCategory("Object Pool")]
	[Tooltip("Recycles the Owner of the Fsm. Useful for Object Pool spawned Prefabs that need to kill themselves, e.g., a projectile that explodes on impact.")]
	public class RecycleSelf : FsmStateAction
	{
		// Token: 0x06006270 RID: 25200 RVA: 0x001F256E File Offset: 0x001F076E
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
