using System;
using System.Collections.Generic;
using GlobalEnums;

// Token: 0x02000753 RID: 1875
public static class CaravanLocationScenes
{
	// Token: 0x06004294 RID: 17044 RVA: 0x00125AD4 File Offset: 0x00123CD4
	public static string GetSceneName(CaravanTroupeLocations location)
	{
		if (CaravanLocationScenes._scenes.ContainsKey(location))
		{
			return CaravanLocationScenes._scenes[location];
		}
		return null;
	}

	// Token: 0x04004417 RID: 17431
	private static readonly Dictionary<CaravanTroupeLocations, string> _scenes = new Dictionary<CaravanTroupeLocations, string>
	{
		{
			CaravanTroupeLocations.Bone,
			"Bone_10"
		},
		{
			CaravanTroupeLocations.Greymoor,
			"Greymoor_08"
		},
		{
			CaravanTroupeLocations.CoralJudge,
			"Coral_Judge_Arena"
		},
		{
			CaravanTroupeLocations.Aqueduct,
			"Aqueduct_05"
		}
	};
}
