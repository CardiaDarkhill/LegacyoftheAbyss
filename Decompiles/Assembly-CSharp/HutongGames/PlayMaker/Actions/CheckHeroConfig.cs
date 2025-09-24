using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012DD RID: 4829
	public class CheckHeroConfig : FSMUtility.CheckFsmStateAction
	{
		// Token: 0x06007DEB RID: 32235 RVA: 0x002579B1 File Offset: 0x00255BB1
		public override void Reset()
		{
			base.Reset();
			this.Config = null;
		}

		// Token: 0x17000C1C RID: 3100
		// (get) Token: 0x06007DEC RID: 32236 RVA: 0x002579C0 File Offset: 0x00255BC0
		public override bool IsTrue
		{
			get
			{
				HeroControllerConfig heroControllerConfig = this.Config.Value as HeroControllerConfig;
				if (!heroControllerConfig)
				{
					return false;
				}
				HeroController instance = HeroController.instance;
				if (!instance)
				{
					return false;
				}
				HeroControllerConfig config = instance.Config;
				return config && config == heroControllerConfig;
			}
		}

		// Token: 0x04007DC5 RID: 32197
		[ObjectType(typeof(HeroControllerConfig))]
		public FsmObject Config;
	}
}
