using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E52 RID: 3666
	[ActionCategory(ActionCategory.Camera)]
	[Tooltip("Transforms a position from world space into screen space. \nNote: Uses the Main Camera unless you specify a camera to use.")]
	public class WorldToScreenPoint : FsmStateAction
	{
		// Token: 0x060068C2 RID: 26818 RVA: 0x0020E26C File Offset: 0x0020C46C
		public override void Reset()
		{
			this.worldPosition = null;
			this.worldX = new FsmFloat
			{
				UseVariable = true
			};
			this.worldY = new FsmFloat
			{
				UseVariable = true
			};
			this.worldZ = new FsmFloat
			{
				UseVariable = true
			};
			this.storeScreenPoint = null;
			this.storeScreenX = null;
			this.storeScreenY = null;
			this.everyFrame = false;
		}

		// Token: 0x060068C3 RID: 26819 RVA: 0x0020E2D4 File Offset: 0x0020C4D4
		private void InitCamera()
		{
			if (this.screenCamera == null || this.cameraGameObject != this.camera.Value)
			{
				this.cameraGameObject = this.camera.Value;
				if (this.cameraGameObject != null)
				{
					this.screenCamera = this.camera.Value.GetComponent<Camera>();
					return;
				}
				this.screenCamera = Camera.main;
				if (this.screenCamera != null)
				{
					this.cameraGameObject = this.screenCamera.gameObject;
				}
			}
		}

		// Token: 0x060068C4 RID: 26820 RVA: 0x0020E367 File Offset: 0x0020C567
		public override void OnEnter()
		{
			this.DoWorldToScreenPoint();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060068C5 RID: 26821 RVA: 0x0020E37D File Offset: 0x0020C57D
		public override void OnUpdate()
		{
			this.DoWorldToScreenPoint();
		}

		// Token: 0x060068C6 RID: 26822 RVA: 0x0020E388 File Offset: 0x0020C588
		private void DoWorldToScreenPoint()
		{
			if (PlayMakerFSM.ApplicationIsQuitting)
			{
				return;
			}
			this.InitCamera();
			if (this.screenCamera == null)
			{
				base.LogError("No camera defined!");
				base.Finish();
				return;
			}
			Vector3 vector = Vector3.zero;
			if (!this.worldPosition.IsNone)
			{
				vector = this.worldPosition.Value;
			}
			if (!this.worldX.IsNone)
			{
				vector.x = this.worldX.Value;
			}
			if (!this.worldY.IsNone)
			{
				vector.y = this.worldY.Value;
			}
			if (!this.worldZ.IsNone)
			{
				vector.z = this.worldZ.Value;
			}
			vector = Camera.main.WorldToScreenPoint(vector);
			if (this.normalize.Value)
			{
				vector.x /= (float)Screen.width;
				vector.y /= (float)Screen.height;
			}
			this.storeScreenPoint.Value = vector;
			this.storeScreenX.Value = vector.x;
			this.storeScreenY.Value = vector.y;
		}

		// Token: 0x060068C7 RID: 26823 RVA: 0x0020E4A8 File Offset: 0x0020C6A8
		public override string ErrorCheck()
		{
			this.InitCamera();
			if (this.screenCamera != null)
			{
				return null;
			}
			if (!(this.camera.Value == null))
			{
				return "@camera:GameObject has no Camera!";
			}
			if (Camera.main == null)
			{
				return "@camera:No MainCamera Defined!";
			}
			return null;
		}

		// Token: 0x040067EE RID: 26606
		[Tooltip("Camera GameObject to use. Defaults to MainCamera if not defined.")]
		public FsmGameObject camera;

		// Token: 0x040067EF RID: 26607
		[UIHint(UIHint.Variable)]
		[Tooltip("World position to transform into screen coordinates.")]
		public FsmVector3 worldPosition;

		// Token: 0x040067F0 RID: 26608
		[Tooltip("Override X coordinate.")]
		public FsmFloat worldX;

		// Token: 0x040067F1 RID: 26609
		[Tooltip("Override Y coordinate.")]
		public FsmFloat worldY;

		// Token: 0x040067F2 RID: 26610
		[Tooltip("Override Z coordinate.")]
		public FsmFloat worldZ;

		// Token: 0x040067F3 RID: 26611
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the screen position in a Vector3 Variable. Z will equal zero.")]
		public FsmVector3 storeScreenPoint;

		// Token: 0x040067F4 RID: 26612
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the screen X position in a Float Variable.")]
		public FsmFloat storeScreenX;

		// Token: 0x040067F5 RID: 26613
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the screen Y position in a Float Variable.")]
		public FsmFloat storeScreenY;

		// Token: 0x040067F6 RID: 26614
		[Tooltip("Normalize screen coordinates (0-1). Otherwise coordinates are in pixels.")]
		public FsmBool normalize;

		// Token: 0x040067F7 RID: 26615
		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		// Token: 0x040067F8 RID: 26616
		private GameObject cameraGameObject;

		// Token: 0x040067F9 RID: 26617
		private Camera screenCamera;
	}
}
