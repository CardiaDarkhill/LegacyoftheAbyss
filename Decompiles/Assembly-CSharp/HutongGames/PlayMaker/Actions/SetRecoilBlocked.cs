using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012C7 RID: 4807
	public class SetRecoilBlocked : FsmStateAction
	{
		// Token: 0x06007D9E RID: 32158 RVA: 0x00256DB0 File Offset: 0x00254FB0
		public override void Reset()
		{
			this.Target = null;
			this.IsUpBlocked = null;
			this.IsDownBlocked = null;
			this.IsLeftBlocked = null;
			this.IsRightBlocked = null;
		}

		// Token: 0x06007D9F RID: 32159 RVA: 0x00256DD8 File Offset: 0x00254FD8
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				Recoil component = safe.GetComponent<Recoil>();
				if (component)
				{
					if (!this.IsUpBlocked.IsNone)
					{
						component.IsUpBlocked = this.IsUpBlocked.Value;
					}
					if (!this.IsDownBlocked.IsNone)
					{
						component.IsDownBlocked = this.IsDownBlocked.Value;
					}
					if (!this.IsLeftBlocked.IsNone)
					{
						component.IsLeftBlocked = this.IsLeftBlocked.Value;
					}
					if (!this.IsRightBlocked.IsNone)
					{
						component.IsRightBlocked = this.IsRightBlocked.Value;
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04007D89 RID: 32137
		public FsmOwnerDefault Target;

		// Token: 0x04007D8A RID: 32138
		public FsmBool IsUpBlocked;

		// Token: 0x04007D8B RID: 32139
		public FsmBool IsDownBlocked;

		// Token: 0x04007D8C RID: 32140
		public FsmBool IsLeftBlocked;

		// Token: 0x04007D8D RID: 32141
		public FsmBool IsRightBlocked;
	}
}
