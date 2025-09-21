using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200124F RID: 4687
	public class DoHeroRecoil : FsmStateAction
	{
		// Token: 0x06007BE0 RID: 31712 RVA: 0x00250B73 File Offset: 0x0024ED73
		public override void Reset()
		{
			this.RecoilSteps = null;
			this.RecoilSpeed = null;
			this.RecoilRight = new FsmBool
			{
				UseVariable = true
			};
		}

		// Token: 0x06007BE1 RID: 31713 RVA: 0x00250B98 File Offset: 0x0024ED98
		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			if (this.RecoilRight.IsNone)
			{
				instance.Recoil(this.RecoilSteps.Value, this.RecoilSpeed.Value);
			}
			else
			{
				instance.Recoil(this.RecoilRight.Value, this.RecoilSteps.Value, this.RecoilSpeed.Value);
			}
			base.Finish();
		}

		// Token: 0x04007C0C RID: 31756
		public FsmInt RecoilSteps;

		// Token: 0x04007C0D RID: 31757
		public FsmFloat RecoilSpeed;

		// Token: 0x04007C0E RID: 31758
		public FsmBool RecoilRight;
	}
}
