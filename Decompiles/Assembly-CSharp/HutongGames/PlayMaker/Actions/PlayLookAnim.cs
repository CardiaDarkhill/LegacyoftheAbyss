using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011F8 RID: 4600
	public sealed class PlayLookAnim : FsmStateAction
	{
		// Token: 0x06007A87 RID: 31367 RVA: 0x0024CA36 File Offset: 0x0024AC36
		public override void Reset()
		{
			this.animation = null;
		}

		// Token: 0x06007A88 RID: 31368 RVA: 0x0024CA3F File Offset: 0x0024AC3F
		public override void OnEnter()
		{
			HeroTalkAnimation.PlayLookAnimation((HeroTalkAnimation.AnimationTypes)this.animation.Value, this.skipToLoop.Value);
			base.Finish();
		}

		// Token: 0x04007AC6 RID: 31430
		[ObjectType(typeof(HeroTalkAnimation.AnimationTypes))]
		public FsmEnum animation;

		// Token: 0x04007AC7 RID: 31431
		public FsmBool skipToLoop;
	}
}
