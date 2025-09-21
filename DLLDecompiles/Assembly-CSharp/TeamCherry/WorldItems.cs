using System;
using System.Collections.Generic;
using UnityEngine;

namespace TeamCherry
{
	// Token: 0x0200089F RID: 2207
	[Serializable]
	public class WorldItems : ScriptableObject
	{
		// Token: 0x06004C50 RID: 19536 RVA: 0x00167A98 File Offset: 0x00165C98
		public void OnEnable()
		{
			if (this.geoRocks == null)
			{
				this.geoRocks = new List<GeoRock>();
			}
		}

		// Token: 0x06004C51 RID: 19537 RVA: 0x00167AAD File Offset: 0x00165CAD
		public void RegisterGeoRock()
		{
		}

		// Token: 0x04004DA5 RID: 19877
		public List<GeoRock> geoRocks;
	}
}
