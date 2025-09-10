using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F3D RID: 3901
	[ActionCategory(ActionCategory.Lights)]
	[Tooltip("Sets the strength of the shadows cast by a Light.")]
	public class SetShadowStrength : ComponentAction<Light>
	{
		// Token: 0x06006C9B RID: 27803 RVA: 0x0021E1F3 File Offset: 0x0021C3F3
		public override void Reset()
		{
			this.gameObject = null;
			this.shadowStrength = 0.8f;
			this.everyFrame = false;
		}

		// Token: 0x06006C9C RID: 27804 RVA: 0x0021E213 File Offset: 0x0021C413
		public override void OnEnter()
		{
			this.DoSetShadowStrength();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006C9D RID: 27805 RVA: 0x0021E229 File Offset: 0x0021C429
		public override void OnUpdate()
		{
			this.DoSetShadowStrength();
		}

		// Token: 0x06006C9E RID: 27806 RVA: 0x0021E234 File Offset: 0x0021C434
		private void DoSetShadowStrength()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.light.shadowStrength = this.shadowStrength.Value;
			}
		}

		// Token: 0x04006C58 RID: 27736
		[RequiredField]
		[CheckForComponent(typeof(Light))]
		[Tooltip("The Game Object with the Light Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006C59 RID: 27737
		[Tooltip("The strength of the shadows. 1 = opaque, 0 = transparent.")]
		public FsmFloat shadowStrength;

		// Token: 0x04006C5A RID: 27738
		[Tooltip("Update every frame. Useful if the shadow strength is animated.")]
		public bool everyFrame;
	}
}
