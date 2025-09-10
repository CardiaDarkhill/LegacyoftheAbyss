using System;
using GlobalEnums;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200136C RID: 4972
	public class GetTubeTravelScene : FsmStateAction
	{
		// Token: 0x0600802C RID: 32812 RVA: 0x0025D9AF File Offset: 0x0025BBAF
		public override void Reset()
		{
			this.Location = null;
			this.StoreSceneName = null;
			this.InvalidEvent = null;
		}

		// Token: 0x0600802D RID: 32813 RVA: 0x0025D9C8 File Offset: 0x0025BBC8
		public override void OnEnter()
		{
			if (!this.Location.IsNone)
			{
				string sceneName = FastTravelScenes.GetSceneName((TubeTravelLocations)this.Location.Value);
				this.StoreSceneName.Value = sceneName;
				if (string.IsNullOrEmpty(sceneName))
				{
					base.Fsm.Event(this.InvalidEvent);
				}
			}
			base.Finish();
		}

		// Token: 0x04007F94 RID: 32660
		[ObjectType(typeof(TubeTravelLocations))]
		public FsmEnum Location;

		// Token: 0x04007F95 RID: 32661
		[UIHint(UIHint.Variable)]
		public FsmString StoreSceneName;

		// Token: 0x04007F96 RID: 32662
		public FsmEvent InvalidEvent;
	}
}
