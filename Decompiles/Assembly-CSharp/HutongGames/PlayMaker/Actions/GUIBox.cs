using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000ECA RID: 3786
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("GUI Box.")]
	public class GUIBox : GUIContentAction
	{
		// Token: 0x06006ADB RID: 27355 RVA: 0x00215330 File Offset: 0x00213530
		public override void OnGUI()
		{
			base.OnGUI();
			if (string.IsNullOrEmpty(this.style.Value))
			{
				GUI.Box(this.rect, this.content);
				return;
			}
			GUI.Box(this.rect, this.content, this.style.Value);
		}
	}
}
