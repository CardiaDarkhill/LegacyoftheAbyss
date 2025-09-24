using System;
using InControl;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DAC RID: 3500
	[ActionCategory("InControl")]
	[Tooltip("Gets a world direction Vector from 2 Incontrol control Axis for a given device. Typically used for a third person controller with Relative To set to the camera.")]
	public class GetInControlDeviceInputAxisVector : FsmStateAction
	{
		// Token: 0x060065A0 RID: 26016 RVA: 0x00201348 File Offset: 0x001FF548
		public override void Reset()
		{
			this.deviceIndex = 0;
			this.horizontalAxis = InputControlType.LeftStickRight;
			this.verticalAxis = InputControlType.LeftStickRight;
			this.multiplier = 1f;
			this.mapToPlane = GetInControlDeviceInputAxisVector.AxisPlane.XZ;
			this.storeVector = null;
			this.storeMagnitude = null;
		}

		// Token: 0x060065A1 RID: 26017 RVA: 0x00201394 File Offset: 0x001FF594
		public override void OnUpdate()
		{
			Vector3 vector = default(Vector3);
			Vector3 a = default(Vector3);
			if (this.relativeTo.Value == null)
			{
				switch (this.mapToPlane)
				{
				case GetInControlDeviceInputAxisVector.AxisPlane.XZ:
					vector = Vector3.forward;
					a = Vector3.right;
					break;
				case GetInControlDeviceInputAxisVector.AxisPlane.XY:
					vector = Vector3.up;
					a = Vector3.right;
					break;
				case GetInControlDeviceInputAxisVector.AxisPlane.YZ:
					vector = Vector3.up;
					a = Vector3.forward;
					break;
				}
			}
			else
			{
				Transform transform = this.relativeTo.Value.transform;
				GetInControlDeviceInputAxisVector.AxisPlane axisPlane = this.mapToPlane;
				if (axisPlane != GetInControlDeviceInputAxisVector.AxisPlane.XZ)
				{
					if (axisPlane - GetInControlDeviceInputAxisVector.AxisPlane.XY <= 1)
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
			if (this.deviceIndex.Value == -1)
			{
				this._inputDevice = InputManager.ActiveDevice;
			}
			else
			{
				this._inputDevice = InputManager.Devices[this.deviceIndex.Value];
			}
			float value = this._inputDevice.GetControl(this.horizontalAxis).Value;
			float value2 = this._inputDevice.GetControl(this.verticalAxis).Value;
			Vector3 vector2 = value * a + value2 * vector;
			vector2 *= this.multiplier.Value;
			this.storeVector.Value = vector2;
			if (!this.storeMagnitude.IsNone)
			{
				this.storeMagnitude.Value = vector2.magnitude;
			}
		}

		// Token: 0x040064BD RID: 25789
		[Tooltip("The index of the device. -1 to use the active device")]
		public FsmInt deviceIndex;

		// Token: 0x040064BE RID: 25790
		public InputControlType horizontalAxis;

		// Token: 0x040064BF RID: 25791
		public InputControlType verticalAxis;

		// Token: 0x040064C0 RID: 25792
		[Tooltip("Input axis are reported in the range -1 to 1, this multiplier lets you set a new range.")]
		public FsmFloat multiplier;

		// Token: 0x040064C1 RID: 25793
		[RequiredField]
		[Tooltip("The world plane to map the 2d input onto.")]
		public GetInControlDeviceInputAxisVector.AxisPlane mapToPlane;

		// Token: 0x040064C2 RID: 25794
		[Tooltip("Make the result relative to a GameObject, typically the main camera.")]
		public FsmGameObject relativeTo;

		// Token: 0x040064C3 RID: 25795
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the direction vector.")]
		public FsmVector3 storeVector;

		// Token: 0x040064C4 RID: 25796
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the length of the direction vector.")]
		public FsmFloat storeMagnitude;

		// Token: 0x040064C5 RID: 25797
		private InputDevice _inputDevice;

		// Token: 0x040064C6 RID: 25798
		private InputControl _inputControl;

		// Token: 0x02001B95 RID: 7061
		public enum AxisPlane
		{
			// Token: 0x04009DA9 RID: 40361
			XZ,
			// Token: 0x04009DAA RID: 40362
			XY,
			// Token: 0x04009DAB RID: 40363
			YZ
		}
	}
}
