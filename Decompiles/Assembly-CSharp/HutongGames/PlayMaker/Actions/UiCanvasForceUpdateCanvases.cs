using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200110E RID: 4366
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Force all canvases to update their content.\nCode that relies on up-to-date layout or content can call this method to ensure it before executing code that relies on it.")]
	public class UiCanvasForceUpdateCanvases : FsmStateAction
	{
		// Token: 0x06007606 RID: 30214 RVA: 0x00240A96 File Offset: 0x0023EC96
		public override void OnEnter()
		{
			Canvas.ForceUpdateCanvases();
			base.Finish();
		}
	}
}
