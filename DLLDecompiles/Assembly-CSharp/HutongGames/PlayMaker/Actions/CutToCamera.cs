using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E4B RID: 3659
	[ActionCategory(ActionCategory.Camera)]
	[Tooltip("Activates a Camera in the scene.")]
	public class CutToCamera : FsmStateAction
	{
		// Token: 0x060068A3 RID: 26787 RVA: 0x0020DDA0 File Offset: 0x0020BFA0
		public override void Reset()
		{
			this.camera = null;
			this.makeMainCamera = true;
			this.cutBackOnExit = false;
		}

		// Token: 0x060068A4 RID: 26788 RVA: 0x0020DDB8 File Offset: 0x0020BFB8
		public override void OnEnter()
		{
			if (this.camera == null)
			{
				base.LogError("Missing camera!");
				return;
			}
			this.oldCamera = Camera.main;
			CutToCamera.SwitchCamera(Camera.main, this.camera);
			if (this.makeMainCamera)
			{
				this.camera.tag = "MainCamera";
			}
			base.Finish();
		}

		// Token: 0x060068A5 RID: 26789 RVA: 0x0020DE18 File Offset: 0x0020C018
		public override void OnExit()
		{
			if (this.cutBackOnExit)
			{
				CutToCamera.SwitchCamera(this.camera, this.oldCamera);
			}
		}

		// Token: 0x060068A6 RID: 26790 RVA: 0x0020DE33 File Offset: 0x0020C033
		private static void SwitchCamera(Camera camera1, Camera camera2)
		{
			if (camera1 != null)
			{
				camera1.enabled = false;
			}
			if (camera2 != null)
			{
				camera2.enabled = true;
			}
		}

		// Token: 0x040067D4 RID: 26580
		[RequiredField]
		[Tooltip("The Camera to activate.")]
		public Camera camera;

		// Token: 0x040067D5 RID: 26581
		[Tooltip("Makes the camera the new MainCamera. The old MainCamera will be untagged.")]
		public bool makeMainCamera;

		// Token: 0x040067D6 RID: 26582
		[Tooltip("Cut back to the original MainCamera when exiting this state.")]
		public bool cutBackOnExit;

		// Token: 0x040067D7 RID: 26583
		private Camera oldCamera;
	}
}
