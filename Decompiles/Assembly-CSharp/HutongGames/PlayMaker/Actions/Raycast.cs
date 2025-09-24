using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FAB RID: 4011
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Casts a Ray against all Colliders in the scene. Use either a Game Object or Vector3 world position as the origin of the ray. Use {{Get Raycast Info}} to get more detailed info.")]
	public class Raycast : FsmStateAction
	{
		// Token: 0x06006EC6 RID: 28358 RVA: 0x0022442C File Offset: 0x0022262C
		public override void Reset()
		{
			this.fromGameObject = null;
			this.fromPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.direction = null;
			this.space = Space.Self;
			this.distance = 100f;
			this.hitEvent = null;
			this.storeDidHit = null;
			this.storeHitObject = null;
			this.storeHitPoint = null;
			this.storeHitNormal = null;
			this.storeHitDistance = null;
			this.repeatInterval = new FsmInt
			{
				Value = 1
			};
			this.layerMask = new FsmInt[0];
			this.invertMask = false;
			this.debugColor = Color.yellow;
			this.debug = false;
		}

		// Token: 0x06006EC7 RID: 28359 RVA: 0x002244E0 File Offset: 0x002226E0
		public override void OnEnter()
		{
			this.DoRaycast();
			if (this.repeatInterval.Value == 0)
			{
				base.Finish();
			}
		}

		// Token: 0x06006EC8 RID: 28360 RVA: 0x002244FB File Offset: 0x002226FB
		public override void OnUpdate()
		{
			this.repeat--;
			if (this.repeat == 0)
			{
				this.DoRaycast();
			}
		}

		// Token: 0x06006EC9 RID: 28361 RVA: 0x0022451C File Offset: 0x0022271C
		private void DoRaycast()
		{
			this.repeat = this.repeatInterval.Value;
			if (this.distance.Value < 0.001f)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.fromGameObject);
			if (ownerDefaultTarget != this.cachedGameObject)
			{
				this.cachedGameObject = ownerDefaultTarget;
				this.cachedTransform = ((ownerDefaultTarget != null) ? ownerDefaultTarget.transform : null);
			}
			Vector3 vector = (this.cachedTransform != null) ? this.cachedTransform.position : this.fromPosition.Value;
			float num = float.PositiveInfinity;
			if (this.distance.Value > 0f)
			{
				num = this.distance.Value;
			}
			Vector3 a = this.direction.Value;
			if (this.cachedTransform != null && this.space == Space.Self)
			{
				a = this.cachedTransform.TransformDirection(this.direction.Value);
			}
			RaycastHit raycastHitInfo;
			Physics.Raycast(vector, a, out raycastHitInfo, num, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
			base.Fsm.RaycastHitInfo = raycastHitInfo;
			bool flag = raycastHitInfo.collider != null;
			this.storeDidHit.Value = flag;
			if (flag)
			{
				this.storeHitObject.Value = raycastHitInfo.collider.gameObject;
				this.storeHitPoint.Value = base.Fsm.RaycastHitInfo.point;
				this.storeHitNormal.Value = base.Fsm.RaycastHitInfo.normal;
				this.storeHitDistance.Value = base.Fsm.RaycastHitInfo.distance;
				base.Fsm.Event(this.hitEvent);
			}
			if (this.debug.Value)
			{
				float d = Mathf.Min(num, 1000f);
				Vector3 end = flag ? this.storeHitPoint.Value : (vector + a * d);
				if (this.repeatInterval.Value == 0)
				{
					Debug.DrawLine(vector, end, this.debugColor.Value, 0.1f);
					return;
				}
				Debug.DrawLine(vector, end, this.debugColor.Value);
			}
		}

		// Token: 0x04006E63 RID: 28259
		[Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
		public FsmOwnerDefault fromGameObject;

		// Token: 0x04006E64 RID: 28260
		[Tooltip("Start ray at a vector3 world position. \nOr use Game Object parameter.")]
		public FsmVector3 fromPosition;

		// Token: 0x04006E65 RID: 28261
		[Tooltip("A vector3 direction vector")]
		public FsmVector3 direction;

		// Token: 0x04006E66 RID: 28262
		[Tooltip("Cast the ray in world or local space. Note if no Game Object is specified, the direction is in world space.")]
		public Space space;

		// Token: 0x04006E67 RID: 28263
		[Tooltip("The length of the ray. Set to -1 for infinity.")]
		public FsmFloat distance;

		// Token: 0x04006E68 RID: 28264
		[ActionSection("Result")]
		[Tooltip("Event to send if the ray hits an object.")]
		[UIHint(UIHint.Variable)]
		public FsmEvent hitEvent;

		// Token: 0x04006E69 RID: 28265
		[Tooltip("Set a bool variable to true if hit something, otherwise false.")]
		[UIHint(UIHint.Variable)]
		public FsmBool storeDidHit;

		// Token: 0x04006E6A RID: 28266
		[Tooltip("Store the game object hit in a variable.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject storeHitObject;

		// Token: 0x04006E6B RID: 28267
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the world position of the ray hit point and store it in a variable.")]
		public FsmVector3 storeHitPoint;

		// Token: 0x04006E6C RID: 28268
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the normal at the hit point and store it in a variable.\nNote, this is a direction vector not a rotation. Use Look At Direction to rotate a GameObject to this direction.")]
		public FsmVector3 storeHitNormal;

		// Token: 0x04006E6D RID: 28269
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
		public FsmFloat storeHitDistance;

		// Token: 0x04006E6E RID: 28270
		[ActionSection("Filter")]
		[Tooltip("Set how often to cast a ray. 0 = once, don't repeat; 1 = everyFrame; 2 = every other frame... \nBecause raycasts can get expensive use the highest repeat interval you can get away with.")]
		public FsmInt repeatInterval;

		// Token: 0x04006E6F RID: 28271
		[UIHint(UIHint.Layer)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;

		// Token: 0x04006E70 RID: 28272
		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		// Token: 0x04006E71 RID: 28273
		[ActionSection("Debug")]
		[Tooltip("The color to use for the debug line.")]
		public FsmColor debugColor;

		// Token: 0x04006E72 RID: 28274
		[Tooltip("Draw a debug line. Note: Check Gizmos in the Game View to see it in game.")]
		public FsmBool debug;

		// Token: 0x04006E73 RID: 28275
		private int repeat;

		// Token: 0x04006E74 RID: 28276
		private GameObject cachedGameObject;

		// Token: 0x04006E75 RID: 28277
		private Transform cachedTransform;
	}
}
