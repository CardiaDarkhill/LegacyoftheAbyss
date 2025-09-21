using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EAF RID: 3759
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Measures the Distance between a GameObject and a target GameObject/Position. If both GameObject and Position are defined, position is taken a local offset from the GameObject's position.")]
	public class GetDistanceXYZ : ComponentAction<Transform>
	{
		// Token: 0x17000BF9 RID: 3065
		// (get) Token: 0x06006A6D RID: 27245 RVA: 0x00213EA0 File Offset: 0x002120A0
		private Transform gameObjectTransform
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x06006A6E RID: 27246 RVA: 0x00213EA8 File Offset: 0x002120A8
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.position = null;
			this.storeDistance = null;
			this.space = Space.World;
			this.storeXDistance = null;
			this.storeYDistance = null;
			this.storeZDistance = null;
			this.everyFrame = true;
		}

		// Token: 0x06006A6F RID: 27247 RVA: 0x00213EF4 File Offset: 0x002120F4
		public override void OnEnter()
		{
			this.DoGetDistanceXYZ();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006A70 RID: 27248 RVA: 0x00213F0A File Offset: 0x0021210A
		public override void OnUpdate()
		{
			this.DoGetDistanceXYZ();
		}

		// Token: 0x06006A71 RID: 27249 RVA: 0x00213F14 File Offset: 0x00212114
		private void DoGetDistanceXYZ()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			if (this.target.Value != null && this.cachedTargetGameObject != this.target.Value)
			{
				this.cachedTargetGameObject = this.target.Value;
				this.targetTransform = this.cachedTargetGameObject.transform;
			}
			Vector3 vector = Vector3.zero;
			if (this.position.IsNone)
			{
				if (this.targetTransform != null)
				{
					vector = this.targetTransform.position;
				}
			}
			else
			{
				vector = ((this.targetTransform == null) ? this.position.Value : this.targetTransform.TransformPoint(this.position.Value));
			}
			if (!this.storeDistance.IsNone)
			{
				this.storeDistance.Value = Vector3.Distance(this.gameObjectTransform.position, vector);
			}
			if (this.storeXDistance.IsNone && this.storeYDistance.IsNone && this.storeZDistance.IsNone)
			{
				return;
			}
			if (this.space == Space.Self)
			{
				vector = this.gameObjectTransform.InverseTransformPoint(vector);
			}
			else
			{
				vector -= this.gameObjectTransform.position;
			}
			this.storeXDistance.Value = vector.x;
			this.storeYDistance.Value = vector.y;
			this.storeZDistance.Value = vector.z;
		}

		// Token: 0x040069BE RID: 27070
		[RequiredField]
		[Tooltip("Measure distance from this GameObject.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040069BF RID: 27071
		[Tooltip("Measure distance to this GameObject (or set world position below).")]
		public FsmGameObject target;

		// Token: 0x040069C0 RID: 27072
		[Tooltip("World position or local offset from target GameObject, if defined.")]
		public FsmVector3 position;

		// Token: 0x040069C1 RID: 27073
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the distance in a float variable.")]
		public FsmFloat storeDistance;

		// Token: 0x040069C2 RID: 27074
		[Tooltip("Space used to measure the distance in. E.g. along the world X axis or the GameObject's local X axis.")]
		public Space space;

		// Token: 0x040069C3 RID: 27075
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the distance along the X axis.")]
		public FsmFloat storeXDistance;

		// Token: 0x040069C4 RID: 27076
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the distance along the Y axis.")]
		public FsmFloat storeYDistance;

		// Token: 0x040069C5 RID: 27077
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the distance along the Z axis.")]
		public FsmFloat storeZDistance;

		// Token: 0x040069C6 RID: 27078
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040069C7 RID: 27079
		private GameObject cachedTargetGameObject;

		// Token: 0x040069C8 RID: 27080
		private Transform targetTransform;
	}
}
