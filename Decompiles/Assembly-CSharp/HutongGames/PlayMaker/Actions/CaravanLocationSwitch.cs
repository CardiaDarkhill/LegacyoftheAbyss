using System;
using GlobalEnums;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001396 RID: 5014
	public sealed class CaravanLocationSwitch : FsmStateAction
	{
		// Token: 0x060080B8 RID: 32952 RVA: 0x0025EFDB File Offset: 0x0025D1DB
		public override void Reset()
		{
			this.Locations = Array.Empty<CaravanLocationSwitch.LocationSwitch>();
		}

		// Token: 0x060080B9 RID: 32953 RVA: 0x0025EFE8 File Offset: 0x0025D1E8
		public override void OnEnter()
		{
			if (this.Locations.Length != 0 && GameManager.instance != null)
			{
				PlayerData playerData = GameManager.instance.playerData;
				if (playerData != null)
				{
					foreach (CaravanLocationSwitch.LocationSwitch locationSwitch in this.Locations)
					{
						if (!locationSwitch.Location.IsNone && playerData.CaravanTroupeLocation == (CaravanTroupeLocations)locationSwitch.Location.Value && locationSwitch.MatchEvent != null)
						{
							base.Fsm.Event(locationSwitch.MatchEvent);
							break;
						}
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04007FFB RID: 32763
		public CaravanLocationSwitch.LocationSwitch[] Locations;

		// Token: 0x02001BFD RID: 7165
		public class LocationSwitch
		{
			// Token: 0x04009FC5 RID: 40901
			[ObjectType(typeof(CaravanTroupeLocations))]
			public FsmEnum Location;

			// Token: 0x04009FC6 RID: 40902
			public FsmEvent MatchEvent;
		}
	}
}
