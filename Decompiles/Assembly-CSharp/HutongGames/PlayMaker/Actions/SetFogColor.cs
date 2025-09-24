using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001038 RID: 4152
	[ActionCategory(ActionCategory.RenderSettings)]
	[Tooltip("Sets the color of the Fog in the scene.")]
	public class SetFogColor : FsmStateAction
	{
		// Token: 0x060071D6 RID: 29142 RVA: 0x002302FF File Offset: 0x0022E4FF
		public override void Reset()
		{
			this.fogColor = Color.white;
			this.everyFrame = false;
		}

		// Token: 0x060071D7 RID: 29143 RVA: 0x00230318 File Offset: 0x0022E518
		public override void OnEnter()
		{
			this.DoSetFogColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060071D8 RID: 29144 RVA: 0x0023032E File Offset: 0x0022E52E
		public override void OnUpdate()
		{
			this.DoSetFogColor();
		}

		// Token: 0x060071D9 RID: 29145 RVA: 0x00230336 File Offset: 0x0022E536
		private void DoSetFogColor()
		{
			RenderSettings.fogColor = this.fogColor.Value;
		}

		// Token: 0x0400719D RID: 29085
		[RequiredField]
		[Tooltip("The color of the fog.")]
		public FsmColor fogColor;

		// Token: 0x0400719E RID: 29086
		[Tooltip("Update every frame. Useful if the color is animated.")]
		public bool everyFrame;
	}
}
