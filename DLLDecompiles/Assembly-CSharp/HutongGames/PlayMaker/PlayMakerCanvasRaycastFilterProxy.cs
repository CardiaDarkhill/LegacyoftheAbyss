using System;
using UnityEngine;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AE8 RID: 2792
	public class PlayMakerCanvasRaycastFilterProxy : MonoBehaviour, ICanvasRaycastFilter
	{
		// Token: 0x060058C4 RID: 22724 RVA: 0x001C312D File Offset: 0x001C132D
		public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			return this.RayCastingEnabled;
		}

		// Token: 0x04005413 RID: 21523
		public bool RayCastingEnabled = true;
	}
}
