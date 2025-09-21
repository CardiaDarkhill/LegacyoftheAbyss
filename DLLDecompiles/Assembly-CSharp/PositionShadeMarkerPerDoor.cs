using System;
using UnityEngine;

// Token: 0x0200047E RID: 1150
public class PositionShadeMarkerPerDoor : MonoBehaviour
{
	// Token: 0x0600299E RID: 10654 RVA: 0x000B52E4 File Offset: 0x000B34E4
	public void Start()
	{
		if (this.shadeMarker)
		{
			foreach (PositionShadeMarkerPerDoor.DoorShadePosition doorShadePosition in this.shadePositions)
			{
				if (doorShadePosition.door.name == GameManager.instance.entryGateName)
				{
					this.shadeMarker.transform.SetPosition2D(doorShadePosition.position);
					return;
				}
			}
		}
	}

	// Token: 0x04002A34 RID: 10804
	public GameObject shadeMarker;

	// Token: 0x04002A35 RID: 10805
	public PositionShadeMarkerPerDoor.DoorShadePosition[] shadePositions;

	// Token: 0x0200178C RID: 6028
	[Serializable]
	public struct DoorShadePosition
	{
		// Token: 0x04008E6A RID: 36458
		public GameObject door;

		// Token: 0x04008E6B RID: 36459
		public Vector2 position;
	}
}
