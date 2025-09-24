using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E4F RID: 3663
	[ActionCategory(ActionCategory.Camera)]
	[Tooltip("Sets the Culling Mask used by the Camera.")]
	public class SetCameraCullingMask : ComponentAction<Camera>
	{
		// Token: 0x060068B5 RID: 26805 RVA: 0x0020E0E2 File Offset: 0x0020C2E2
		public override void Reset()
		{
			this.gameObject = null;
			this.cullingMask = new FsmInt[0];
			this.invertMask = false;
			this.everyFrame = false;
		}

		// Token: 0x060068B6 RID: 26806 RVA: 0x0020E10A File Offset: 0x0020C30A
		public override void OnEnter()
		{
			this.DoSetCameraCullingMask();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060068B7 RID: 26807 RVA: 0x0020E120 File Offset: 0x0020C320
		public override void OnUpdate()
		{
			this.DoSetCameraCullingMask();
		}

		// Token: 0x060068B8 RID: 26808 RVA: 0x0020E128 File Offset: 0x0020C328
		private void DoSetCameraCullingMask()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.camera.cullingMask = ActionHelpers.LayerArrayToLayerMask(this.cullingMask, this.invertMask.Value);
			}
		}

		// Token: 0x040067E6 RID: 26598
		[RequiredField]
		[CheckForComponent(typeof(Camera))]
		[Tooltip("The Game Object with the Camera component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040067E7 RID: 26599
		[Tooltip("Cull these layers.")]
		[UIHint(UIHint.Layer)]
		public FsmInt[] cullingMask;

		// Token: 0x040067E8 RID: 26600
		[Tooltip("Invert the mask, so you cull all layers except those defined above.")]
		public FsmBool invertMask;

		// Token: 0x040067E9 RID: 26601
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
