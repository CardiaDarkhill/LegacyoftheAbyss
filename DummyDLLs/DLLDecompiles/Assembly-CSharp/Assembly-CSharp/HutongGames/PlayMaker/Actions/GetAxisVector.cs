using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F02 RID: 3842
	[NoActionTargets]
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Gets a world direction Vector from 2 Input Axis. Typically used for a third person controller with Relative To set to the camera.")]
	[SeeAlso("Unity Input Manager")]
	public class GetAxisVector : FsmStateAction
	{
		// Token: 0x06006B84 RID: 27524 RVA: 0x002175B0 File Offset: 0x002157B0
		public override void Reset()
		{
			this.horizontalAxis = "Horizontal";
			this.verticalAxis = "Vertical";
			this.multiplier = 1f;
			this.mapToPlane = GetAxisVector.AxisPlane.XZ;
			this.storeVector = null;
			this.storeMagnitude = null;
		}

		// Token: 0x06006B85 RID: 27525 RVA: 0x00217604 File Offset: 0x00215804
		public override void OnUpdate()
		{
			Vector3 vector = default(Vector3);
			Vector3 a = default(Vector3);
			if (this.relativeTo.Value == null)
			{
				switch (this.mapToPlane)
				{
				case GetAxisVector.AxisPlane.XZ:
					vector = Vector3.forward;
					a = Vector3.right;
					break;
				case GetAxisVector.AxisPlane.XY:
					vector = Vector3.up;
					a = Vector3.right;
					break;
				case GetAxisVector.AxisPlane.YZ:
					vector = Vector3.up;
					a = Vector3.forward;
					break;
				}
			}
			else
			{
				Transform transform = this.relativeTo.Value.transform;
				GetAxisVector.AxisPlane axisPlane = this.mapToPlane;
				if (axisPlane != GetAxisVector.AxisPlane.XZ)
				{
					if (axisPlane - GetAxisVector.AxisPlane.XY <= 1)
					{
						vector = Vector3.up;
						vector.z = 0f;
						vector = vector.normalized;
						a = transform.TransformDirection(Vector3.right);
					}
				}
				else
				{
					vector = transform.TransformDirection(Vector3.forward);
					vector.y = 0f;
					vector = vector.normalized;
					a = new Vector3(vector.z, 0f, -vector.x);
				}
			}
			float d = (this.horizontalAxis.IsNone || string.IsNullOrEmpty(this.horizontalAxis.Value)) ? 0f : Input.GetAxis(this.horizontalAxis.Value);
			float d2 = (this.verticalAxis.IsNone || string.IsNullOrEmpty(this.verticalAxis.Value)) ? 0f : Input.GetAxis(this.verticalAxis.Value);
			Vector3 vector2 = d * a + d2 * vector;
			vector2 *= this.multiplier.Value;
			this.storeVector.Value = vector2;
			if (!this.storeMagnitude.IsNone)
			{
				this.storeMagnitude.Value = vector2.magnitude;
			}
		}

		// Token: 0x04006AD6 RID: 27350
		[Tooltip("The name of the horizontal input axis. See Unity Input Manager.")]
		public FsmString horizontalAxis;

		// Token: 0x04006AD7 RID: 27351
		[Tooltip("The name of the vertical input axis. See Unity Input Manager.")]
		public FsmString verticalAxis;

		// Token: 0x04006AD8 RID: 27352
		[Tooltip("Normally axis values are in the range -1 to 1. Use the multiplier to make this range bigger. \nE.g., A multiplier of 100 returns values from -100 to 100.\nTypically this represents the maximum movement speed.")]
		public FsmFloat multiplier;

		// Token: 0x04006AD9 RID: 27353
		[RequiredField]
		[Tooltip("Sets the world axis the input maps to. The remaining axis will be set to zero.")]
		public GetAxisVector.AxisPlane mapToPlane;

		// Token: 0x04006ADA RID: 27354
		[Tooltip("Calculate a vector relative to this game object. Typically the camera.")]
		public FsmGameObject relativeTo;

		// Token: 0x04006ADB RID: 27355
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the resulting vector. You can use this in {{Translate}} or other movement actions.")]
		public FsmVector3 storeVector;

		// Token: 0x04006ADC RID: 27356
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the magnitude of the vector. Useful if you want to measure the strength of the input and react accordingly. Hint: Use {{Float Compare}}.")]
		public FsmFloat storeMagnitude;

		// Token: 0x02001BA9 RID: 7081
		public enum AxisPlane
		{
			// Token: 0x04009E19 RID: 40473
			XZ,
			// Token: 0x04009E1A RID: 40474
			XY,
			// Token: 0x04009E1B RID: 40475
			YZ
		}
	}
}
