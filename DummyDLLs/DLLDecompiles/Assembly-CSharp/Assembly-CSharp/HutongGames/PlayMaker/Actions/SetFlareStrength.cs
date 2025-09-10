using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001037 RID: 4151
	[ActionCategory(ActionCategory.RenderSettings)]
	[Tooltip("Sets the intensity of all Flares in the scene.")]
	public class SetFlareStrength : FsmStateAction
	{
		// Token: 0x060071D1 RID: 29137 RVA: 0x002302AE File Offset: 0x0022E4AE
		public override void Reset()
		{
			this.flareStrength = 0.2f;
			this.everyFrame = false;
		}

		// Token: 0x060071D2 RID: 29138 RVA: 0x002302C7 File Offset: 0x0022E4C7
		public override void OnEnter()
		{
			this.DoSetFlareStrength();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060071D3 RID: 29139 RVA: 0x002302DD File Offset: 0x0022E4DD
		public override void OnUpdate()
		{
			this.DoSetFlareStrength();
		}

		// Token: 0x060071D4 RID: 29140 RVA: 0x002302E5 File Offset: 0x0022E4E5
		private void DoSetFlareStrength()
		{
			RenderSettings.flareStrength = this.flareStrength.Value;
		}

		// Token: 0x0400719B RID: 29083
		[RequiredField]
		[Tooltip("The intensity of flares in the scene.")]
		public FsmFloat flareStrength;

		// Token: 0x0400719C RID: 29084
		[Tooltip("Update every frame. Useful if the intensity is animated.")]
		public bool everyFrame;
	}
}
