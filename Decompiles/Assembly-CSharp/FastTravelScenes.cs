using System;
using System.Collections.Generic;
using GlobalEnums;

// Token: 0x0200065E RID: 1630
public static class FastTravelScenes
{
	// Token: 0x06003A36 RID: 14902 RVA: 0x000FF354 File Offset: 0x000FD554
	public static string GetSceneName(FastTravelLocations location)
	{
		return FastTravelScenes.GetSceneName<FastTravelLocations>(location, FastTravelScenes._scenes);
	}

	// Token: 0x06003A37 RID: 14903 RVA: 0x000FF361 File Offset: 0x000FD561
	public static string GetSceneName(TubeTravelLocations location)
	{
		return FastTravelScenes.GetSceneName<TubeTravelLocations>(location, FastTravelScenes._tubeScenes);
	}

	// Token: 0x06003A38 RID: 14904 RVA: 0x000FF370 File Offset: 0x000FD570
	private static string GetSceneName<T>(T location, Dictionary<T, string> dictionary)
	{
		string result;
		if (!dictionary.TryGetValue(location, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x04003CC6 RID: 15558
	private static readonly Dictionary<FastTravelLocations, string> _scenes = new Dictionary<FastTravelLocations, string>
	{
		{
			FastTravelLocations.Bonetown,
			"Bellway_01"
		},
		{
			FastTravelLocations.Docks,
			"Bellway_02"
		},
		{
			FastTravelLocations.BoneforestEast,
			"Bellway_03"
		},
		{
			FastTravelLocations.Greymoor,
			"Bellway_04"
		},
		{
			FastTravelLocations.Belltown,
			"Belltown_basement"
		},
		{
			FastTravelLocations.CoralTower,
			"Bellway_08"
		},
		{
			FastTravelLocations.City,
			"Bellway_City"
		},
		{
			FastTravelLocations.Peak,
			"Slab_06"
		},
		{
			FastTravelLocations.Shellwood,
			"Shellwood_19"
		},
		{
			FastTravelLocations.Bone,
			"Bone_05"
		},
		{
			FastTravelLocations.Shadow,
			"Bellway_Shadow"
		},
		{
			FastTravelLocations.Aqueduct,
			"Bellway_Aqueduct"
		}
	};

	// Token: 0x04003CC7 RID: 15559
	private static readonly Dictionary<TubeTravelLocations, string> _tubeScenes = new Dictionary<TubeTravelLocations, string>
	{
		{
			TubeTravelLocations.Hub,
			"Tube_Hub"
		},
		{
			TubeTravelLocations.Song,
			"Song_01b"
		},
		{
			TubeTravelLocations.Under,
			"Under_22"
		},
		{
			TubeTravelLocations.CityBellway,
			"Bellway_City"
		},
		{
			TubeTravelLocations.Hang,
			"Hang_06b"
		},
		{
			TubeTravelLocations.Enclave,
			"Song_Enclave_Tube"
		},
		{
			TubeTravelLocations.Arborium,
			"Arborium_Tube"
		}
	};
}
