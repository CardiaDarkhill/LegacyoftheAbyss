using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010D6 RID: 4310
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Clamps a rotation around a local axis. Optionally define the default rotation. Clamp is done on LateUpdate")]
	public class ClampRotation : FsmStateAction
	{
		// Token: 0x060074AE RID: 29870 RVA: 0x0023B1D8 File Offset: 0x002393D8
		public override void Reset()
		{
			this.gameObject = null;
			this.defaultRotation = new FsmVector3
			{
				UseVariable = true
			};
			this.constraintAxis = null;
			this.minAngle = -45f;
			this.maxAngle = 45f;
			this.everyFrame = false;
		}

		// Token: 0x060074AF RID: 29871 RVA: 0x0023B22C File Offset: 0x0023942C
		public override void OnPreprocess()
		{
			base.Fsm.HandleLateUpdate = true;
		}

		// Token: 0x060074B0 RID: 29872 RVA: 0x0023B23C File Offset: 0x0023943C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.thisTransform = ownerDefaultTarget.transform;
			this.axis = (ClampRotation.ConstraintAxis)this.constraintAxis.Value;
			this.axisIndex = (int)this.axis;
			switch (this.axis)
			{
			case ClampRotation.ConstraintAxis.x:
				this.rotateAround = Vector3.right;
				break;
			case ClampRotation.ConstraintAxis.y:
				this.rotateAround = Vector3.up;
				break;
			case ClampRotation.ConstraintAxis.z:
				this.rotateAround = Vector3.forward;
				break;
			}
			this._defaultRotation = ((!this.defaultRotation.IsNone) ? this.defaultRotation.Value : this.thisTransform.localRotation.eulerAngles);
			this.ComputeRange();
		}

		// Token: 0x060074B1 RID: 29873 RVA: 0x0023B30C File Offset: 0x0023950C
		public override void OnLateUpdate()
		{
			this.DoClampRotation();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060074B2 RID: 29874 RVA: 0x0023B324 File Offset: 0x00239524
		private void DoClampRotation()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.thisTransform = ownerDefaultTarget.transform;
			if (!this.defaultRotation.IsNone && this._defaultRotation != this.defaultRotation.Value)
			{
				this._defaultRotation = this.defaultRotation.Value;
				this.ComputeRange();
			}
			this.axisRotation = Quaternion.AngleAxis(this.thisTransform.localRotation.eulerAngles[this.axisIndex], this.rotateAround);
			this.angleFromMin = Quaternion.Angle(this.axisRotation, this.minQuaternion);
			this.angleFromMax = Quaternion.Angle(this.axisRotation, this.maxQuaternion);
			if (this.angleFromMin <= this.range && this.angleFromMax <= this.range)
			{
				return;
			}
			Vector3 eulerAngles = this.thisTransform.localRotation.eulerAngles;
			if (this.angleFromMin > this.angleFromMax)
			{
				eulerAngles[this.axisIndex] = this.maxQuaternion.eulerAngles[this.axisIndex];
			}
			else
			{
				eulerAngles[this.axisIndex] = this.minQuaternion.eulerAngles[this.axisIndex];
			}
			this.thisTransform.localEulerAngles = eulerAngles;
		}

		// Token: 0x060074B3 RID: 29875 RVA: 0x0023B490 File Offset: 0x00239690
		private void ComputeRange()
		{
			this.axisRotation = Quaternion.AngleAxis(this.defaultRotation.Value[this.axisIndex], this.rotateAround);
			this.minQuaternion = this.axisRotation * Quaternion.AngleAxis(this.minAngle.Value, this.rotateAround);
			this.maxQuaternion = this.axisRotation * Quaternion.AngleAxis(this.maxAngle.Value, this.rotateAround);
			this.range = this.maxAngle.Value - this.minAngle.Value;
		}

		// Token: 0x040074EB RID: 29931
		[RequiredField]
		[Tooltip("The GameObject to clamp rotation.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040074EC RID: 29932
		[Tooltip("The default rotation. If none, will use the GameObject target.")]
		public FsmVector3 defaultRotation;

		// Token: 0x040074ED RID: 29933
		[ObjectType(typeof(ClampRotation.ConstraintAxis))]
		[Tooltip("The axis to constraint the rotation")]
		public FsmEnum constraintAxis;

		// Token: 0x040074EE RID: 29934
		[Tooltip("The minimum angle allowed")]
		public FsmFloat minAngle;

		// Token: 0x040074EF RID: 29935
		[Tooltip("The maximum angle allowed")]
		public FsmFloat maxAngle;

		// Token: 0x040074F0 RID: 29936
		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		// Token: 0x040074F1 RID: 29937
		private float angleFromMin;

		// Token: 0x040074F2 RID: 29938
		private float angleFromMax;

		// Token: 0x040074F3 RID: 29939
		private Transform thisTransform;

		// Token: 0x040074F4 RID: 29940
		private Vector3 rotateAround;

		// Token: 0x040074F5 RID: 29941
		private Quaternion minQuaternion;

		// Token: 0x040074F6 RID: 29942
		private Quaternion maxQuaternion;

		// Token: 0x040074F7 RID: 29943
		private float range;

		// Token: 0x040074F8 RID: 29944
		private ClampRotation.ConstraintAxis axis;

		// Token: 0x040074F9 RID: 29945
		private int axisIndex;

		// Token: 0x040074FA RID: 29946
		private Quaternion axisRotation;

		// Token: 0x040074FB RID: 29947
		private Vector3 _defaultRotation;

		// Token: 0x02001BCA RID: 7114
		public enum ConstraintAxis
		{
			// Token: 0x04009EC3 RID: 40643
			x,
			// Token: 0x04009EC4 RID: 40644
			y,
			// Token: 0x04009EC5 RID: 40645
			z
		}
	}
}
