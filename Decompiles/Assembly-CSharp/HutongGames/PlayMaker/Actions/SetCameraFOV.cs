using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E50 RID: 3664
	[ActionCategory(ActionCategory.Camera)]
	[Tooltip("Sets Field of View used by the Camera.")]
	public class SetCameraFOV : ComponentAction<Camera>
	{
		// Token: 0x060068BA RID: 26810 RVA: 0x0020E179 File Offset: 0x0020C379
		public override void Reset()
		{
			this.gameObject = null;
			this.fieldOfView = 50f;
			this.everyFrame = false;
		}

		// Token: 0x060068BB RID: 26811 RVA: 0x0020E199 File Offset: 0x0020C399
		public override void OnEnter()
		{
			this.DoSetCameraFOV();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060068BC RID: 26812 RVA: 0x0020E1AF File Offset: 0x0020C3AF
		public override void OnUpdate()
		{
			this.DoSetCameraFOV();
		}

		// Token: 0x060068BD RID: 26813 RVA: 0x0020E1B8 File Offset: 0x0020C3B8
		private void DoSetCameraFOV()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.camera.fieldOfView = this.fieldOfView.Value;
			}
		}

		// Token: 0x040067EA RID: 26602
		[RequiredField]
		[CheckForComponent(typeof(Camera))]
		[Tooltip("The Game Object with the Camera component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040067EB RID: 26603
		[RequiredField]
		[Tooltip("Field of view in degrees.")]
		public FsmFloat fieldOfView;

		// Token: 0x040067EC RID: 26604
		[Tooltip("Repeat every frame. Useful if the fov is animated.")]
		public bool everyFrame;
	}
}
