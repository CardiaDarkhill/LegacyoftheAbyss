using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001039 RID: 4153
	[ActionCategory(ActionCategory.RenderSettings)]
	[Tooltip("Sets the density of the Fog in the scene.")]
	public class SetFogDensity : FsmStateAction
	{
		// Token: 0x060071DB RID: 29147 RVA: 0x00230350 File Offset: 0x0022E550
		public override void Reset()
		{
			this.fogDensity = 0.5f;
			this.everyFrame = false;
		}

		// Token: 0x060071DC RID: 29148 RVA: 0x00230369 File Offset: 0x0022E569
		public override void OnEnter()
		{
			this.DoSetFogDensity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060071DD RID: 29149 RVA: 0x0023037F File Offset: 0x0022E57F
		public override void OnUpdate()
		{
			this.DoSetFogDensity();
		}

		// Token: 0x060071DE RID: 29150 RVA: 0x00230387 File Offset: 0x0022E587
		private void DoSetFogDensity()
		{
			RenderSettings.fogDensity = this.fogDensity.Value;
		}

		// Token: 0x0400719F RID: 29087
		[RequiredField]
		[Tooltip("The density of the fog.")]
		public FsmFloat fogDensity;

		// Token: 0x040071A0 RID: 29088
		[Tooltip("Update every frame. Useful if the fog density is animated.")]
		public bool everyFrame;
	}
}
