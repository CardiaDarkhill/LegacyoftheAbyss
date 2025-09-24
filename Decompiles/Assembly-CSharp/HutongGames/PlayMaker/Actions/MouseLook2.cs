using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F0F RID: 3855
	[Obsolete("Use MouseLook instead.")]
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("NOTE: This is a duplicate action and will be removed in a future update. Please use Mouse Look instead.\nRotates a GameObject based on mouse movement. Minimum and Maximum values can be used to constrain the rotation.")]
	public class MouseLook2 : ComponentAction<Rigidbody>
	{
		// Token: 0x06006BBE RID: 27582 RVA: 0x00217F70 File Offset: 0x00216170
		public override void Reset()
		{
			this.gameObject = null;
			this.axes = MouseLook2.RotationAxes.MouseXAndY;
			this.sensitivityX = 15f;
			this.sensitivityY = 15f;
			this.minimumX = -360f;
			this.maximumX = 360f;
			this.minimumY = -60f;
			this.maximumY = 60f;
			this.everyFrame = true;
		}

		// Token: 0x06006BBF RID: 27583 RVA: 0x00217FF4 File Offset: 0x002161F4
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			if (!base.UpdateCache(ownerDefaultTarget) && base.rigidbody)
			{
				base.rigidbody.freezeRotation = true;
			}
			this.DoMouseLook();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006BC0 RID: 27584 RVA: 0x00218059 File Offset: 0x00216259
		public override void OnUpdate()
		{
			this.DoMouseLook();
		}

		// Token: 0x06006BC1 RID: 27585 RVA: 0x00218064 File Offset: 0x00216264
		private void DoMouseLook()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Transform transform = ownerDefaultTarget.transform;
			switch (this.axes)
			{
			case MouseLook2.RotationAxes.MouseXAndY:
				transform.localEulerAngles = new Vector3(this.GetYRotation(), this.GetXRotation(), 0f);
				return;
			case MouseLook2.RotationAxes.MouseX:
				transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, this.GetXRotation(), 0f);
				return;
			case MouseLook2.RotationAxes.MouseY:
				transform.localEulerAngles = new Vector3(-this.GetYRotation(), transform.localEulerAngles.y, 0f);
				return;
			default:
				return;
			}
		}

		// Token: 0x06006BC2 RID: 27586 RVA: 0x00218110 File Offset: 0x00216310
		private float GetXRotation()
		{
			this.rotationX += Input.GetAxis("Mouse X") * this.sensitivityX.Value;
			this.rotationX = MouseLook2.ClampAngle(this.rotationX, this.minimumX, this.maximumX);
			return this.rotationX;
		}

		// Token: 0x06006BC3 RID: 27587 RVA: 0x00218164 File Offset: 0x00216364
		private float GetYRotation()
		{
			this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY.Value;
			this.rotationY = MouseLook2.ClampAngle(this.rotationY, this.minimumY, this.maximumY);
			return this.rotationY;
		}

		// Token: 0x06006BC4 RID: 27588 RVA: 0x002181B7 File Offset: 0x002163B7
		private static float ClampAngle(float angle, FsmFloat min, FsmFloat max)
		{
			if (!min.IsNone && angle < min.Value)
			{
				angle = min.Value;
			}
			if (!max.IsNone && angle > max.Value)
			{
				angle = max.Value;
			}
			return angle;
		}

		// Token: 0x04006B0B RID: 27403
		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006B0C RID: 27404
		[Tooltip("The axes to rotate around.")]
		public MouseLook2.RotationAxes axes;

		// Token: 0x04006B0D RID: 27405
		[RequiredField]
		[Tooltip("Speed around X axis. Higher = faster.")]
		public FsmFloat sensitivityX;

		// Token: 0x04006B0E RID: 27406
		[RequiredField]
		[Tooltip("Speed around Y axis. Higher = faster.")]
		public FsmFloat sensitivityY;

		// Token: 0x04006B0F RID: 27407
		[RequiredField]
		[HasFloatSlider(-360f, 360f)]
		[Tooltip("Minimum angle around X axis.")]
		public FsmFloat minimumX;

		// Token: 0x04006B10 RID: 27408
		[RequiredField]
		[HasFloatSlider(-360f, 360f)]
		[Tooltip("Maximum angle around X axis.")]
		public FsmFloat maximumX;

		// Token: 0x04006B11 RID: 27409
		[RequiredField]
		[HasFloatSlider(-360f, 360f)]
		[Tooltip("Minimum angle around Y axis.")]
		public FsmFloat minimumY;

		// Token: 0x04006B12 RID: 27410
		[RequiredField]
		[HasFloatSlider(-360f, 360f)]
		[Tooltip("Maximum angle around X axis.")]
		public FsmFloat maximumY;

		// Token: 0x04006B13 RID: 27411
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04006B14 RID: 27412
		private float rotationX;

		// Token: 0x04006B15 RID: 27413
		private float rotationY;

		// Token: 0x02001BAB RID: 7083
		public enum RotationAxes
		{
			// Token: 0x04009E21 RID: 40481
			MouseXAndY,
			// Token: 0x04009E22 RID: 40482
			MouseX,
			// Token: 0x04009E23 RID: 40483
			MouseY
		}
	}
}
