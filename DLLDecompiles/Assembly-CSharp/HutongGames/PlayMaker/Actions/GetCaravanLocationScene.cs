using System;
using GlobalEnums;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001395 RID: 5013
	public class GetCaravanLocationScene : FsmStateAction
	{
		// Token: 0x060080B5 RID: 32949 RVA: 0x0025EF5E File Offset: 0x0025D15E
		public override void Reset()
		{
			this.Location = null;
			this.StoreSceneName = null;
			this.InvalidEvent = null;
		}

		// Token: 0x060080B6 RID: 32950 RVA: 0x0025EF78 File Offset: 0x0025D178
		public override void OnEnter()
		{
			if (!this.Location.IsNone)
			{
				string sceneName = CaravanLocationScenes.GetSceneName((CaravanTroupeLocations)this.Location.Value);
				this.StoreSceneName.Value = sceneName;
				if (string.IsNullOrEmpty(sceneName))
				{
					base.Fsm.Event(this.InvalidEvent);
				}
			}
			base.Finish();
		}

		// Token: 0x04007FF8 RID: 32760
		[ObjectType(typeof(CaravanTroupeLocations))]
		public FsmEnum Location;

		// Token: 0x04007FF9 RID: 32761
		[UIHint(UIHint.Variable)]
		public FsmString StoreSceneName;

		// Token: 0x04007FFA RID: 32762
		public FsmEvent InvalidEvent;
	}
}
