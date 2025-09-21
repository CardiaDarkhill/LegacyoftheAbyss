using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001035 RID: 4149
	[ActionCategory(ActionCategory.RenderSettings)]
	[Tooltip("Enables/Disables Fog in the scene.")]
	public class EnableFog : FsmStateAction
	{
		// Token: 0x060071C8 RID: 29128 RVA: 0x0023020E File Offset: 0x0022E40E
		public override void Reset()
		{
			this.enableFog = true;
			this.everyFrame = false;
		}

		// Token: 0x060071C9 RID: 29129 RVA: 0x00230223 File Offset: 0x0022E423
		public override void OnEnter()
		{
			RenderSettings.fog = this.enableFog.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060071CA RID: 29130 RVA: 0x00230243 File Offset: 0x0022E443
		public override void OnUpdate()
		{
			RenderSettings.fog = this.enableFog.Value;
		}

		// Token: 0x04007197 RID: 29079
		[Tooltip("Set to True to enable, False to disable.")]
		public FsmBool enableFog;

		// Token: 0x04007198 RID: 29080
		[Tooltip("Repeat every frame. Useful if the Enable Fog setting is changing.")]
		public bool everyFrame;
	}
}
