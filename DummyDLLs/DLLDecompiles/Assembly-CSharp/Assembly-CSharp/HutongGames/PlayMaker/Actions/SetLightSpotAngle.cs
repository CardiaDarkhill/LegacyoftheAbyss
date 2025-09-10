using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F3B RID: 3899
	[ActionCategory(ActionCategory.Lights)]
	[Tooltip("Sets the Spot Angle of a Light.")]
	public class SetLightSpotAngle : ComponentAction<Light>
	{
		// Token: 0x06006C92 RID: 27794 RVA: 0x0021E0FA File Offset: 0x0021C2FA
		public override void Reset()
		{
			this.gameObject = null;
			this.lightSpotAngle = 20f;
			this.everyFrame = false;
		}

		// Token: 0x06006C93 RID: 27795 RVA: 0x0021E11A File Offset: 0x0021C31A
		public override void OnEnter()
		{
			this.DoSetLightRange();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006C94 RID: 27796 RVA: 0x0021E130 File Offset: 0x0021C330
		public override void OnUpdate()
		{
			this.DoSetLightRange();
		}

		// Token: 0x06006C95 RID: 27797 RVA: 0x0021E138 File Offset: 0x0021C338
		private void DoSetLightRange()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.light.spotAngle = this.lightSpotAngle.Value;
			}
		}

		// Token: 0x04006C53 RID: 27731
		[RequiredField]
		[CheckForComponent(typeof(Light))]
		[Tooltip("The Game Object with the Light Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006C54 RID: 27732
		[Tooltip("The angle of the spot light beam.")]
		public FsmFloat lightSpotAngle;

		// Token: 0x04006C55 RID: 27733
		[Tooltip("Update every frame. Useful if the spot angle is changing.")]
		public bool everyFrame;
	}
}
