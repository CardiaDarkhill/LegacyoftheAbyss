using System;
using GlobalEnums;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200136B RID: 4971
	public class GetFastTravelScene : FsmStateAction
	{
		// Token: 0x06008029 RID: 32809 RVA: 0x0025D933 File Offset: 0x0025BB33
		public override void Reset()
		{
			this.Location = null;
			this.StoreSceneName = null;
			this.InvalidEvent = null;
		}

		// Token: 0x0600802A RID: 32810 RVA: 0x0025D94C File Offset: 0x0025BB4C
		public override void OnEnter()
		{
			if (!this.Location.IsNone)
			{
				string sceneName = FastTravelScenes.GetSceneName((FastTravelLocations)this.Location.Value);
				this.StoreSceneName.Value = sceneName;
				if (string.IsNullOrEmpty(sceneName))
				{
					base.Fsm.Event(this.InvalidEvent);
				}
			}
			base.Finish();
		}

		// Token: 0x04007F91 RID: 32657
		[ObjectType(typeof(FastTravelLocations))]
		public FsmEnum Location;

		// Token: 0x04007F92 RID: 32658
		[UIHint(UIHint.Variable)]
		public FsmString StoreSceneName;

		// Token: 0x04007F93 RID: 32659
		public FsmEvent InvalidEvent;
	}
}
