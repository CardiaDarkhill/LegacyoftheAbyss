using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F0E RID: 3854
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Rotates a GameObject based on mouse movement. Minimum and Maximum values can be used to constrain the rotation.")]
	public class MouseLook : ComponentAction<Transform>
	{
		// Token: 0x06006BB6 RID: 27574 RVA: 0x00217C50 File Offset: 0x00215E50
		public override void Reset()
		{
			this.gameObject = null;
			this.axes = MouseLook.RotationAxes.MouseXAndY;
			this.sensitivityX = 15f;
			this.sensitivityY = 15f;
			this.minimumX = new FsmFloat
			{
				UseVariable = true
			};
			this.maximumX = new FsmFloat
			{
				UseVariable = true
			};
			this.minimumY = -60f;
			this.maximumY = 60f;
			this.everyFrame = true;
		}

		// Token: 0x06006BB7 RID: 27575 RVA: 0x00217CD8 File Offset: 0x00215ED8
		public override void OnEnter()
		{
			if (!base.UpdateCachedTransform(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			Rigidbody component = this.cachedGameObject.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.freezeRotation = true;
			}
			this.rotationX = base.cachedTransform.localRotation.eulerAngles.y;
			this.rotationY = base.cachedTransform.localRotation.eulerAngles.x;
			if (!this.everyFrame)
			{
				this.DoMouseLook();
				base.Finish();
			}
		}

		// Token: 0x06006BB8 RID: 27576 RVA: 0x00217D71 File Offset: 0x00215F71
		public override void OnUpdate()
		{
			this.DoMouseLook();
		}

		// Token: 0x06006BB9 RID: 27577 RVA: 0x00217D7C File Offset: 0x00215F7C
		private void DoMouseLook()
		{
			if (!base.UpdateCachedTransform(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			switch (this.axes)
			{
			case MouseLook.RotationAxes.MouseXAndY:
				base.cachedTransform.localEulerAngles = new Vector3(this.GetYRotation(1f), this.GetXRotation(), 0f);
				return;
			case MouseLook.RotationAxes.MouseX:
				base.cachedTransform.localEulerAngles = new Vector3(base.cachedTransform.localEulerAngles.x, this.GetXRotation(), 0f);
				return;
			case MouseLook.RotationAxes.MouseY:
				base.cachedTransform.localEulerAngles = new Vector3(this.GetYRotation(-1f), base.cachedTransform.localEulerAngles.y, 0f);
				return;
			default:
				return;
			}
		}

		// Token: 0x06006BBA RID: 27578 RVA: 0x00217E48 File Offset: 0x00216048
		private float GetXRotation()
		{
			this.rotationX += Input.GetAxis("Mouse X") * this.sensitivityX.Value;
			this.rotationX = MouseLook.ClampAngle(this.rotationX, this.minimumX, this.maximumX) % 360f;
			return this.rotationX;
		}

		// Token: 0x06006BBB RID: 27579 RVA: 0x00217EA4 File Offset: 0x002160A4
		private float GetYRotation(float invert = 1f)
		{
			this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY.Value * invert;
			this.rotationY = MouseLook.ClampAngle(this.rotationY, this.minimumY, this.maximumY) % 360f;
			return this.rotationY;
		}

		// Token: 0x06006BBC RID: 27580 RVA: 0x00217F00 File Offset: 0x00216100
		private static float ClampAngle(float angle, FsmFloat min, FsmFloat max)
		{
			if (angle < 0f)
			{
				angle = 360f + angle;
			}
			float num = min.IsNone ? -720f : min.Value;
			float b = max.IsNone ? 720f : max.Value;
			if (angle > 180f)
			{
				return Mathf.Max(angle, 360f + num);
			}
			return Mathf.Min(angle, b);
		}

		// Token: 0x04006B00 RID: 27392
		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006B01 RID: 27393
		[Tooltip("The axes to rotate around.")]
		public MouseLook.RotationAxes axes;

		// Token: 0x04006B02 RID: 27394
		[RequiredField]
		[Tooltip("Sensitivity of movement in X direction.")]
		public FsmFloat sensitivityX;

		// Token: 0x04006B03 RID: 27395
		[RequiredField]
		[Tooltip("Sensitivity of movement in Y direction.")]
		public FsmFloat sensitivityY;

		// Token: 0x04006B04 RID: 27396
		[HasFloatSlider(-360f, 360f)]
		[Tooltip("Clamp rotation around X axis. Set to None for no clamping.")]
		public FsmFloat minimumX;

		// Token: 0x04006B05 RID: 27397
		[HasFloatSlider(-360f, 360f)]
		[Tooltip("Clamp rotation around X axis. Set to None for no clamping.")]
		public FsmFloat maximumX;

		// Token: 0x04006B06 RID: 27398
		[HasFloatSlider(-360f, 360f)]
		[Tooltip("Clamp rotation around Y axis. Set to None for no clamping.")]
		public FsmFloat minimumY;

		// Token: 0x04006B07 RID: 27399
		[HasFloatSlider(-360f, 360f)]
		[Tooltip("Clamp rotation around Y axis. Set to None for no clamping.")]
		public FsmFloat maximumY;

		// Token: 0x04006B08 RID: 27400
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04006B09 RID: 27401
		private float rotationX;

		// Token: 0x04006B0A RID: 27402
		private float rotationY;

		// Token: 0x02001BAA RID: 7082
		public enum RotationAxes
		{
			// Token: 0x04009E1D RID: 40477
			MouseXAndY,
			// Token: 0x04009E1E RID: 40478
			MouseX,
			// Token: 0x04009E1F RID: 40479
			MouseY
		}
	}
}
