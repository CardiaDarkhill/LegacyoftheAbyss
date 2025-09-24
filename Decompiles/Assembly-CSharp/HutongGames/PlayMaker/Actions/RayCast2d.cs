using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FD6 RID: 4054
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Casts a Ray against all Colliders in the scene. A raycast is conceptually like a laser beam that is fired from a point in space along a particular direction. Any object making contact with the beam can be detected and reported. Use GetRaycastHit2dInfo to get more detailed info.")]
	public class RayCast2d : FsmStateAction
	{
		// Token: 0x06006FB6 RID: 28598 RVA: 0x00228830 File Offset: 0x00226A30
		public override void Reset()
		{
			this.fromGameObject = null;
			this.fromPosition = new FsmVector2
			{
				UseVariable = true
			};
			this.direction = null;
			this.space = Space.Self;
			this.minDepth = new FsmInt
			{
				UseVariable = true
			};
			this.maxDepth = new FsmInt
			{
				UseVariable = true
			};
			this.distance = 100f;
			this.hitEvent = null;
			this.storeDidHit = null;
			this.storeHitObject = null;
			this.storeHitPoint = null;
			this.storeHitNormal = null;
			this.storeHitDistance = null;
			this.storeHitFraction = null;
			this.repeatInterval = 1;
			this.layerMask = new FsmInt[0];
			this.invertMask = false;
			this.debugColor = Color.yellow;
			this.debug = false;
		}

		// Token: 0x06006FB7 RID: 28599 RVA: 0x0022890C File Offset: 0x00226B0C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.fromGameObject);
			if (ownerDefaultTarget != null)
			{
				this._transform = ownerDefaultTarget.transform;
			}
			this.DoRaycast();
			if (this.repeatInterval.Value == 0)
			{
				base.Finish();
			}
		}

		// Token: 0x06006FB8 RID: 28600 RVA: 0x00228959 File Offset: 0x00226B59
		public override void OnUpdate()
		{
			this.repeat--;
			if (this.repeat == 0)
			{
				this.DoRaycast();
			}
		}

		// Token: 0x06006FB9 RID: 28601 RVA: 0x00228978 File Offset: 0x00226B78
		private void DoRaycast()
		{
			this.repeat = this.repeatInterval.Value;
			if (Math.Abs(this.distance.Value) < Mathf.Epsilon)
			{
				return;
			}
			Vector2 value = this.fromPosition.Value;
			if (this._transform != null)
			{
				value.x += this._transform.position.x;
				value.y += this._transform.position.y;
			}
			float a = float.PositiveInfinity;
			if (this.distance.Value > 0f)
			{
				a = this.distance.Value;
			}
			Vector2 normalized = this.direction.Value.normalized;
			if (this._transform != null && this.space == Space.Self)
			{
				Vector3 vector = this._transform.TransformDirection(new Vector3(this.direction.Value.x, this.direction.Value.y, 0f));
				normalized.x = vector.x;
				normalized.y = vector.y;
			}
			RaycastHit2D info;
			if (this.minDepth.IsNone && this.maxDepth.IsNone)
			{
				info = Helper.Raycast2D(value, normalized, a, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
			}
			else
			{
				float num = this.minDepth.IsNone ? float.NegativeInfinity : ((float)this.minDepth.Value);
				float num2 = this.maxDepth.IsNone ? float.PositiveInfinity : ((float)this.maxDepth.Value);
				info = Helper.Raycast2D(value, normalized, a, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value), num, num2);
			}
			Fsm.RecordLastRaycastHit2DInfo(base.Fsm, info);
			bool flag = info.collider != null;
			this.storeDidHit.Value = flag;
			if (flag)
			{
				this.storeHitObject.Value = info.collider.gameObject;
				this.storeHitPoint.Value = info.point;
				this.storeHitNormal.Value = info.normal;
				this.storeHitDistance.Value = info.distance;
				this.storeHitFraction.Value = info.fraction;
				base.Fsm.Event(this.hitEvent);
			}
			if (this.debug.Value)
			{
				Vector3 vector2 = new Vector3(value.x, value.y, 0f);
				if (flag)
				{
					Debug.DrawLine(vector2, this.storeHitPoint.Value, this.debugColor.Value);
					return;
				}
				float d = Mathf.Min(a, 1000f);
				Vector3 a2 = new Vector3(normalized.x, normalized.y, 0f);
				Vector3 end = vector2 + a2 * d;
				Debug.DrawLine(vector2, end, this.debugColor.Value);
			}
		}

		// Token: 0x04006F9C RID: 28572
		[ActionSection("Setup")]
		[Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
		public FsmOwnerDefault fromGameObject;

		// Token: 0x04006F9D RID: 28573
		[Tooltip("Start ray at a vector2 world position, or offset from the GameObject's position.")]
		public FsmVector2 fromPosition;

		// Token: 0x04006F9E RID: 28574
		[Tooltip("A vector2 direction vector")]
		public FsmVector2 direction;

		// Token: 0x04006F9F RID: 28575
		[Tooltip("Cast the ray in world or local space. Note if no Game Object is specified, the direction is in world space.")]
		public Space space;

		// Token: 0x04006FA0 RID: 28576
		[Tooltip("The length of the ray. Set to -1 for infinity.")]
		public FsmFloat distance;

		// Token: 0x04006FA1 RID: 28577
		[Tooltip("Only include objects with a Z coordinate (depth) greater than this value. Leave as None for no filtering.")]
		public FsmInt minDepth;

		// Token: 0x04006FA2 RID: 28578
		[Tooltip("Only include objects with a Z coordinate (depth) less than this value. Leave as none for no filtering.")]
		public FsmInt maxDepth;

		// Token: 0x04006FA3 RID: 28579
		[ActionSection("Result")]
		[Tooltip("Event to send if the ray hits an object.")]
		[UIHint(UIHint.Variable)]
		public FsmEvent hitEvent;

		// Token: 0x04006FA4 RID: 28580
		[Tooltip("Set a bool variable to true if hit something, otherwise false.")]
		[UIHint(UIHint.Variable)]
		public FsmBool storeDidHit;

		// Token: 0x04006FA5 RID: 28581
		[Tooltip("Store the game object hit in a variable.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject storeHitObject;

		// Token: 0x04006FA6 RID: 28582
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the 2d position of the ray hit point and store it in a variable.")]
		public FsmVector2 storeHitPoint;

		// Token: 0x04006FA7 RID: 28583
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the 2d normal at the hit point and store it in a variable. \nNote, this is a direction vector not a rotation.")]
		public FsmVector2 storeHitNormal;

		// Token: 0x04006FA8 RID: 28584
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
		public FsmFloat storeHitDistance;

		// Token: 0x04006FA9 RID: 28585
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the fraction along the ray to the hit point and store it in a variable. If the ray's direction vector is normalized then this value is simply the distance between the origin and the hit point. If the direction is not normalized then this distance is expressed as a 'fraction' (which could be greater than 1) of the vector's magnitude.")]
		public FsmFloat storeHitFraction;

		// Token: 0x04006FAA RID: 28586
		[ActionSection("Filter")]
		[Tooltip("Set how often to cast a ray. 0 = once, don't repeat; 1 = everyFrame; 2 = every other frame... \nBecause raycasts can get expensive use the highest repeat interval you can get away with.")]
		public FsmInt repeatInterval;

		// Token: 0x04006FAB RID: 28587
		[UIHint(UIHint.Layer)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;

		// Token: 0x04006FAC RID: 28588
		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		// Token: 0x04006FAD RID: 28589
		[ActionSection("Debug")]
		[Tooltip("The color to use for the debug line.")]
		public FsmColor debugColor;

		// Token: 0x04006FAE RID: 28590
		[Tooltip("Draw a debug line. Note: Check Gizmos in the Game View to see it in game.")]
		public FsmBool debug;

		// Token: 0x04006FAF RID: 28591
		private Transform _transform;

		// Token: 0x04006FB0 RID: 28592
		private int repeat;
	}
}
