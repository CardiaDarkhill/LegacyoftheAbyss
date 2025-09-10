using System;
using UnityEngine;

namespace HutongGames.Extensions
{
	// Token: 0x02000AE3 RID: 2787
	public static class TweenExtensions
	{
		// Token: 0x0600588D RID: 22669 RVA: 0x001C270C File Offset: 0x001C090C
		public static Rect Lerp(this Rect rect, Rect from, Rect to, float t)
		{
			rect.Set(Mathf.Lerp(from.x, to.x, t), Mathf.Lerp(from.y, to.y, t), Mathf.Lerp(from.width, to.width, t), Mathf.Lerp(from.height, to.height, t));
			return rect;
		}
	}
}
