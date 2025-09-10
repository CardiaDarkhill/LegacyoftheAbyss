using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001287 RID: 4743
	public class CancelAllTaggedDamage : FsmStateAction
	{
		// Token: 0x06007CB9 RID: 31929 RVA: 0x00254388 File Offset: 0x00252588
		public override void Reset()
		{
			this.Target = null;
		}

		// Token: 0x06007CBA RID: 31930 RVA: 0x00254394 File Offset: 0x00252594
		public override void OnEnter()
		{
			TagDamageTaker safe = this.Target.GetSafe(this);
			if (safe != null)
			{
				safe.ClearTagDamage();
			}
			base.Finish();
		}

		// Token: 0x04007CD1 RID: 31953
		public FsmOwnerDefault Target;
	}
}
