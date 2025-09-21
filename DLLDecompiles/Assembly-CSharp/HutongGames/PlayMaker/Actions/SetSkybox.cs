using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200103B RID: 4155
	[ActionCategory(ActionCategory.RenderSettings)]
	[Tooltip("Sets the global Skybox.")]
	public class SetSkybox : FsmStateAction
	{
		// Token: 0x060071E5 RID: 29157 RVA: 0x002303F2 File Offset: 0x0022E5F2
		public override void Reset()
		{
			this.skybox = null;
		}

		// Token: 0x060071E6 RID: 29158 RVA: 0x002303FB File Offset: 0x0022E5FB
		public override void OnEnter()
		{
			RenderSettings.skybox = this.skybox.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060071E7 RID: 29159 RVA: 0x0023041B File Offset: 0x0022E61B
		public override void OnUpdate()
		{
			RenderSettings.skybox = this.skybox.Value;
		}

		// Token: 0x040071A3 RID: 29091
		[Tooltip("The skybox material.")]
		public FsmMaterial skybox;

		// Token: 0x040071A4 RID: 29092
		[Tooltip("Repeat every frame. Useful if the Skybox is changing.")]
		public bool everyFrame;
	}
}
