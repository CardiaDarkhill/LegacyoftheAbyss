using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000ECF RID: 3791
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("GUI Label.")]
	public class GUILabel : GUIContentAction
	{
		// Token: 0x06006AE9 RID: 27369 RVA: 0x00215634 File Offset: 0x00213834
		public override void OnGUI()
		{
			base.OnGUI();
			if (string.IsNullOrEmpty(this.style.Value))
			{
				GUI.Label(this.rect, this.content);
				return;
			}
			GUI.Label(this.rect, this.content, this.style.Value);
		}
	}
}
