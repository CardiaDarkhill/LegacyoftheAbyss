using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001036 RID: 4150
	[ActionCategory(ActionCategory.RenderSettings)]
	[Tooltip("Sets the Ambient Light Color for the scene.")]
	public class SetAmbientLight : FsmStateAction
	{
		// Token: 0x060071CC RID: 29132 RVA: 0x0023025D File Offset: 0x0022E45D
		public override void Reset()
		{
			this.ambientColor = Color.gray;
			this.everyFrame = false;
		}

		// Token: 0x060071CD RID: 29133 RVA: 0x00230276 File Offset: 0x0022E476
		public override void OnEnter()
		{
			this.DoSetAmbientColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060071CE RID: 29134 RVA: 0x0023028C File Offset: 0x0022E48C
		public override void OnUpdate()
		{
			this.DoSetAmbientColor();
		}

		// Token: 0x060071CF RID: 29135 RVA: 0x00230294 File Offset: 0x0022E494
		private void DoSetAmbientColor()
		{
			RenderSettings.ambientLight = this.ambientColor.Value;
		}

		// Token: 0x04007199 RID: 29081
		[RequiredField]
		[Tooltip("Color of the ambient light.")]
		public FsmColor ambientColor;

		// Token: 0x0400719A RID: 29082
		[Tooltip("Update every frame. Useful if the color is animated.")]
		public bool everyFrame;
	}
}
