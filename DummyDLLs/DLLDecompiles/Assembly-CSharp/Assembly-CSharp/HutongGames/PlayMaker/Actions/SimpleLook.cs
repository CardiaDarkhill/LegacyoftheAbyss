using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010E8 RID: 4328
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Rotates a GameObject based on a Vector2 input, typically from a PlayerInput action. Use it on a player GameObject for MouseLook type behaviour. It is common to setup the camera as a child of the 'body', so the body rotates left/right while the camera tilts up/down.Minimum and Maximum values can be used to constrain the rotation.")]
	public class SimpleLook : ComponentAction<Transform>
	{
		// Token: 0x0600751B RID: 29979 RVA: 0x0023CDC4 File Offset: 0x0023AFC4
		public override void Reset()
		{
			this.gameObject = null;
			this.vector2Input = new FsmVector2
			{
				UseVariable = true
			};
			this.sensitivityX = 1f;
			this.sensitivityY = 1f;
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

		// Token: 0x0600751C RID: 29980 RVA: 0x0023CE58 File Offset: 0x0023B058
		public override void OnEnter()
		{
			if (!base.UpdateCachedTransform(base.Fsm.GetOwnerDefaultTarget(this.gameObject)) || this.vector2Input.IsNone)
			{
				base.Finish();
				return;
			}
			if (this.camera.Value != null)
			{
				this.cachedCameraTransform = this.camera.Value.transform;
			}
			Rigidbody component = this.cachedGameObject.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.freezeRotation = true;
			}
			Quaternion localRotation = base.cachedTransform.localRotation;
			this.rotationX = localRotation.eulerAngles.y;
			this.rotationY = ((this.cachedCameraTransform == null) ? localRotation.eulerAngles.x : this.cachedCameraTransform.localRotation.eulerAngles.x);
			if (!this.everyFrame)
			{
				this.DoLookRotate();
				base.Finish();
			}
		}

		// Token: 0x0600751D RID: 29981 RVA: 0x0023CF43 File Offset: 0x0023B143
		public override void OnUpdate()
		{
			this.DoLookRotate();
		}

		// Token: 0x0600751E RID: 29982 RVA: 0x0023CF4C File Offset: 0x0023B14C
		private void DoLookRotate()
		{
			if (!base.UpdateCachedTransform(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			if (this.cachedCameraTransform == null)
			{
				base.cachedTransform.localEulerAngles = new Vector3(this.GetYRotation(1f), this.GetXRotation(), 0f);
				return;
			}
			base.cachedTransform.localEulerAngles = new Vector3(base.cachedTransform.localEulerAngles.x, this.GetXRotation(), 0f);
			this.cachedCameraTransform.localEulerAngles = new Vector3(this.GetYRotation(-1f), this.cachedCameraTransform.localEulerAngles.y, 0f);
		}

		// Token: 0x0600751F RID: 29983 RVA: 0x0023D00C File Offset: 0x0023B20C
		private float GetXRotation()
		{
			this.rotationX += this.vector2Input.Value.x * this.sensitivityX.Value;
			this.rotationX = SimpleLook.ClampAngle(this.rotationX, this.minimumX, this.maximumX) % 360f;
			return this.rotationX;
		}

		// Token: 0x06007520 RID: 29984 RVA: 0x0023D06C File Offset: 0x0023B26C
		private float GetYRotation(float invert = 1f)
		{
			this.rotationY += this.vector2Input.Value.y * invert * this.sensitivityY.Value;
			this.rotationY = SimpleLook.ClampAngle(this.rotationY, this.minimumY, this.maximumY) % 360f;
			return this.rotationY;
		}

		// Token: 0x06007521 RID: 29985 RVA: 0x0023D0D0 File Offset: 0x0023B2D0
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

		// Token: 0x04007575 RID: 30069
		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007576 RID: 30070
		[Tooltip("The Camera is often the child of the GameObject 'body'. If you specify a Camera, it will tilt up down, while the body rotates left/right. If you leave this empty, all rotations will be applied to the main GameObject.")]
		public new FsmGameObject camera;

		// Token: 0x04007577 RID: 30071
		[RequiredField]
		[Tooltip("Vector2 input, typically from a PlayerInput action.")]
		public FsmVector2 vector2Input;

		// Token: 0x04007578 RID: 30072
		[RequiredField]
		[Tooltip("Sensitivity of movement in X direction (rotate left/right).")]
		public FsmFloat sensitivityX;

		// Token: 0x04007579 RID: 30073
		[RequiredField]
		[Tooltip("Sensitivity of movement in Y direction (tilt up/down).")]
		public FsmFloat sensitivityY;

		// Token: 0x0400757A RID: 30074
		[HasFloatSlider(-360f, 360f)]
		[Tooltip("Clamp rotation around X axis. Set to None for no clamping.")]
		public FsmFloat minimumX;

		// Token: 0x0400757B RID: 30075
		[HasFloatSlider(-360f, 360f)]
		[Tooltip("Clamp rotation around X axis. Set to None for no clamping.")]
		public FsmFloat maximumX;

		// Token: 0x0400757C RID: 30076
		[HasFloatSlider(-360f, 360f)]
		[Tooltip("Clamp rotation around Y axis. Set to None for no clamping.")]
		public FsmFloat minimumY;

		// Token: 0x0400757D RID: 30077
		[HasFloatSlider(-360f, 360f)]
		[Tooltip("Clamp rotation around Y axis. Set to None for no clamping.")]
		public FsmFloat maximumY;

		// Token: 0x0400757E RID: 30078
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x0400757F RID: 30079
		private float rotationX;

		// Token: 0x04007580 RID: 30080
		private float rotationY;

		// Token: 0x04007581 RID: 30081
		private Transform cachedCameraTransform;
	}
}
