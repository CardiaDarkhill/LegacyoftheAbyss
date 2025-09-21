using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F39 RID: 3897
	[ActionCategory(ActionCategory.Lights)]
	[Tooltip("Sets the Intensity of a Light.")]
	public class SetLightIntensity : ComponentAction<Light>
	{
		// Token: 0x06006C88 RID: 27784 RVA: 0x0021DFF1 File Offset: 0x0021C1F1
		public override void Reset()
		{
			this.gameObject = null;
			this.lightIntensity = 1f;
			this.everyFrame = false;
		}

		// Token: 0x06006C89 RID: 27785 RVA: 0x0021E011 File Offset: 0x0021C211
		public override void OnEnter()
		{
			this.DoSetLightIntensity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006C8A RID: 27786 RVA: 0x0021E027 File Offset: 0x0021C227
		public override void OnUpdate()
		{
			this.DoSetLightIntensity();
		}

		// Token: 0x06006C8B RID: 27787 RVA: 0x0021E030 File Offset: 0x0021C230
		private void DoSetLightIntensity()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.light.intensity = this.lightIntensity.Value;
			}
		}

		// Token: 0x04006C4D RID: 27725
		[RequiredField]
		[CheckForComponent(typeof(Light))]
		[Tooltip("The Game Object with the Light Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006C4E RID: 27726
		[Tooltip("The intensity of the light.")]
		public FsmFloat lightIntensity;

		// Token: 0x04006C4F RID: 27727
		[Tooltip("Update every frame. Useful if the intensity is animated.")]
		public bool everyFrame;
	}
}
