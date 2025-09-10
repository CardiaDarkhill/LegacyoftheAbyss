using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200103A RID: 4154
	[ActionCategory(ActionCategory.RenderSettings)]
	[Tooltip("Sets the size of light halos.")]
	public class SetHaloStrength : FsmStateAction
	{
		// Token: 0x060071E0 RID: 29152 RVA: 0x002303A1 File Offset: 0x0022E5A1
		public override void Reset()
		{
			this.haloStrength = 0.5f;
			this.everyFrame = false;
		}

		// Token: 0x060071E1 RID: 29153 RVA: 0x002303BA File Offset: 0x0022E5BA
		public override void OnEnter()
		{
			this.DoSetHaloStrength();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060071E2 RID: 29154 RVA: 0x002303D0 File Offset: 0x0022E5D0
		public override void OnUpdate()
		{
			this.DoSetHaloStrength();
		}

		// Token: 0x060071E3 RID: 29155 RVA: 0x002303D8 File Offset: 0x0022E5D8
		private void DoSetHaloStrength()
		{
			RenderSettings.haloStrength = this.haloStrength.Value;
		}

		// Token: 0x040071A1 RID: 29089
		[RequiredField]
		[Tooltip("The size of light halos.")]
		public FsmFloat haloStrength;

		// Token: 0x040071A2 RID: 29090
		[Tooltip("Update every frame. Useful if the size is animated.")]
		public bool everyFrame;
	}
}
