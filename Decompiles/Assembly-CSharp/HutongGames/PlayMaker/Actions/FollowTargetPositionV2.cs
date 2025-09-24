using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C52 RID: 3154
	public class FollowTargetPositionV2 : FsmStateAction
	{
		// Token: 0x06005F92 RID: 24466 RVA: 0x001E532B File Offset: 0x001E352B
		public override void Reset()
		{
			this.Target = null;
			this.FollowPos = null;
			this.LerpSpeed = null;
		}

		// Token: 0x06005F93 RID: 24467 RVA: 0x001E5344 File Offset: 0x001E3544
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.self = safe.transform;
				if (this.self)
				{
					return;
				}
			}
			base.Finish();
		}

		// Token: 0x06005F94 RID: 24468 RVA: 0x001E5388 File Offset: 0x001E3588
		public override void OnUpdate()
		{
			Vector2 a = this.self.position;
			Vector2 value = this.FollowPos.Value;
			this.self.SetPosition2D(Vector2.Lerp(a, value, this.LerpSpeed.Value * Time.deltaTime));
		}

		// Token: 0x04005CEB RID: 23787
		public FsmOwnerDefault Target;

		// Token: 0x04005CEC RID: 23788
		public FsmVector2 FollowPos;

		// Token: 0x04005CED RID: 23789
		public FsmFloat LerpSpeed;

		// Token: 0x04005CEE RID: 23790
		private Transform self;
	}
}
