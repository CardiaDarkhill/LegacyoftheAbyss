using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F36 RID: 3894
	[ActionCategory(ActionCategory.Lights)]
	[Tooltip("Sets the Color of a Light.")]
	public class SetLightColor : ComponentAction<Light>
	{
		// Token: 0x06006C7B RID: 27771 RVA: 0x0021DEAA File Offset: 0x0021C0AA
		public override void Reset()
		{
			this.gameObject = null;
			this.lightColor = Color.white;
			this.everyFrame = false;
		}

		// Token: 0x06006C7C RID: 27772 RVA: 0x0021DECA File Offset: 0x0021C0CA
		public override void OnEnter()
		{
			this.DoSetLightColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006C7D RID: 27773 RVA: 0x0021DEE0 File Offset: 0x0021C0E0
		public override void OnUpdate()
		{
			this.DoSetLightColor();
		}

		// Token: 0x06006C7E RID: 27774 RVA: 0x0021DEE8 File Offset: 0x0021C0E8
		private void DoSetLightColor()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.light.color = this.lightColor.Value;
			}
		}

		// Token: 0x04006C46 RID: 27718
		[RequiredField]
		[CheckForComponent(typeof(Light))]
		[Tooltip("The Game Object with the Light Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006C47 RID: 27719
		[RequiredField]
		[Tooltip("The color of the light.")]
		public FsmColor lightColor;

		// Token: 0x04006C48 RID: 27720
		[Tooltip("Update every frame. Useful if the color is animated.")]
		public bool everyFrame;
	}
}
