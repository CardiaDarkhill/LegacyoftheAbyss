using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001234 RID: 4660
	public class HeroBoxControlV2 : HeroBoxControl
	{
		// Token: 0x17000C18 RID: 3096
		// (get) Token: 0x06007B69 RID: 31593 RVA: 0x0024F934 File Offset: 0x0024DB34
		protected override bool IsEveryFrame
		{
			get
			{
				return !this.setOnExit.IsNone;
			}
		}

		// Token: 0x06007B6A RID: 31594 RVA: 0x0024F944 File Offset: 0x0024DB44
		public override void Reset()
		{
			base.Reset();
			this.setOnExit = new FsmEnum
			{
				UseVariable = true
			};
		}

		// Token: 0x06007B6B RID: 31595 RVA: 0x0024F960 File Offset: 0x0024DB60
		public override void OnExit()
		{
			if (this.setOnExit.IsNone)
			{
				return;
			}
			HeroBoxControl.HeroBoxState boxState = (HeroBoxControl.HeroBoxState)this.setOnExit.Value;
			base.SetBoxState(boxState);
		}

		// Token: 0x04007BAB RID: 31659
		[ObjectType(typeof(HeroBoxControl.HeroBoxState))]
		public FsmEnum setOnExit;
	}
}
