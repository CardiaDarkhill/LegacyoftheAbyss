using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F14 RID: 3860
	[NoActionTargets]
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Transforms 2d input into a 3d world space vector. E.g., can be used to transform input from a touch joystick to a movement vector.")]
	public class TransformInputToWorldSpace : FsmStateAction
	{
		// Token: 0x06006BDA RID: 27610 RVA: 0x002187AF File Offset: 0x002169AF
		public override void Reset()
		{
			this.horizontalInput = null;
			this.verticalInput = null;
			this.multiplier = 1f;
			this.mapToPlane = TransformInputToWorldSpace.AxisPlane.XZ;
			this.storeVector = null;
			this.storeMagnitude = null;
		}

		// Token: 0x06006BDB RID: 27611 RVA: 0x002187E4 File Offset: 0x002169E4
		public override void OnUpdate()
		{
			Vector3 vector = default(Vector3);
			Vector3 a = default(Vector3);
			if (this.relativeTo.Value == null)
			{
				switch (this.mapToPlane)
				{
				case TransformInputToWorldSpace.AxisPlane.XZ:
					vector = Vector3.forward;
					a = Vector3.right;
					break;
				case TransformInputToWorldSpace.AxisPlane.XY:
					vector = Vector3.up;
					a = Vector3.right;
					break;
				case TransformInputToWorldSpace.AxisPlane.YZ:
					vector = Vector3.up;
					a = Vector3.forward;
					break;
				}
			}
			else
			{
				Transform transform = this.relativeTo.Value.transform;
				TransformInputToWorldSpace.AxisPlane axisPlane = this.mapToPlane;
				if (axisPlane != TransformInputToWorldSpace.AxisPlane.XZ)
				{
					if (axisPlane - TransformInputToWorldSpace.AxisPlane.XY <= 1)
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
			float d = this.horizontalInput.IsNone ? 0f : this.horizontalInput.Value;
			float d2 = this.verticalInput.IsNone ? 0f : this.verticalInput.Value;
			Vector3 vector2 = d * a + d2 * vector;
			vector2 *= this.multiplier.Value;
			this.storeVector.Value = vector2;
			if (!this.storeMagnitude.IsNone)
			{
				this.storeMagnitude.Value = vector2.magnitude;
			}
		}

		// Token: 0x04006B35 RID: 27445
		[UIHint(UIHint.Variable)]
		[Tooltip("The horizontal input.")]
		public FsmFloat horizontalInput;

		// Token: 0x04006B36 RID: 27446
		[UIHint(UIHint.Variable)]
		[Tooltip("The vertical input.")]
		public FsmFloat verticalInput;

		// Token: 0x04006B37 RID: 27447
		[Tooltip("Input axis are reported in the range -1 to 1, this multiplier lets you set a new range.")]
		public FsmFloat multiplier;

		// Token: 0x04006B38 RID: 27448
		[RequiredField]
		[Tooltip("The world plane to map the 2d input onto.")]
		public TransformInputToWorldSpace.AxisPlane mapToPlane;

		// Token: 0x04006B39 RID: 27449
		[Tooltip("Make the result relative to a GameObject, typically the main camera.")]
		public FsmGameObject relativeTo;

		// Token: 0x04006B3A RID: 27450
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the direction vector.")]
		public FsmVector3 storeVector;

		// Token: 0x04006B3B RID: 27451
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the length of the direction vector.")]
		public FsmFloat storeMagnitude;

		// Token: 0x02001BAC RID: 7084
		public enum AxisPlane
		{
			// Token: 0x04009E25 RID: 40485
			XZ,
			// Token: 0x04009E26 RID: 40486
			XY,
			// Token: 0x04009E27 RID: 40487
			YZ
		}
	}
}
