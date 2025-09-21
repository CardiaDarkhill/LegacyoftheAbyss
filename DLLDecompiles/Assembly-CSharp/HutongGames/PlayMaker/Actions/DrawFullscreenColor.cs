using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EC6 RID: 3782
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("Fills the screen with a Color. NOTE: Uses OnGUI so you need a PlayMakerGUI component in the scene.")]
	public class DrawFullscreenColor : FsmStateAction
	{
		// Token: 0x06006ACF RID: 27343 RVA: 0x00214EF4 File Offset: 0x002130F4
		public override void Reset()
		{
			this.color = Color.white;
		}

		// Token: 0x06006AD0 RID: 27344 RVA: 0x00214F08 File Offset: 0x00213108
		public override void OnGUI()
		{
			Color color = GUI.color;
			GUI.color = this.color.Value;
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), ActionHelpers.WhiteTexture);
			GUI.color = color;
		}

		// Token: 0x04006A0F RID: 27151
		[RequiredField]
		[Tooltip("Color. NOTE: Uses OnGUI so you need a PlayMakerGUI component in the scene.")]
		public FsmColor color;
	}
}
