using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CEF RID: 3311
	[ActionCategory("Physics 2d")]
	[Tooltip("Casts a Ray against all Colliders in the scene. A raycast is conceptually like a laser beam that is fired from a point in space along a particular direction. Any object making contact with the beam can be detected and reported. Use GetRaycastHit2dInfo to get more detailed info. CHERRYNOTE: Added ability to measure distance in units to hit point.")]
	public class RayCast2dV2 : FsmStateAction
	{
		// Token: 0x06006251 RID: 25169 RVA: 0x001F1464 File Offset: 0x001EF664
		public override void Reset()
		{
			this.fromGameObject = null;
			this.fromPosition = new FsmVector2
			{
				UseVariable = true
			};
			this.direction = new FsmVector2
			{
				UseVariable = true
			};
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
			this.noHitEvent = null;
			this.storeDidHit = null;
			this.storeHitObject = null;
			this.storeHitPoint = null;
			this.storeHitNormal = null;
			this.storeHitDistance = null;
			this.repeatInterval = 1;
			this.layerMask = new FsmInt[0];
			this.invertMask = false;
			this.ignoreTriggers = false;
			this.debugColor = Color.yellow;
			this.debug = false;
		}

		// Token: 0x06006252 RID: 25170 RVA: 0x001F1554 File Offset: 0x001EF754
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.fromGameObject);
			if (ownerDefaultTarget != null)
			{
				this._trans = ownerDefaultTarget.transform;
			}
			this.DoRaycast();
			if (this.repeatInterval.Value == 0)
			{
				base.Finish();
			}
		}

		// Token: 0x06006253 RID: 25171 RVA: 0x001F15A1 File Offset: 0x001EF7A1
		public override void OnUpdate()
		{
			this.repeat--;
			if (this.repeat == 0)
			{
				this.DoRaycast();
			}
		}

		// Token: 0x06006254 RID: 25172 RVA: 0x001F15C0 File Offset: 0x001EF7C0
		private void DoRaycast()
		{
			this.repeat = this.repeatInterval.Value;
			if (this.distance.Value == 0f)
			{
				return;
			}
			Vector2 value = this.fromPosition.Value;
			if (this._trans != null)
			{
				value.x += this._trans.position.x;
				value.y += this._trans.position.y;
			}
			float num = float.PositiveInfinity;
			if (this.distance.Value > 0f)
			{
				num = this.distance.Value;
			}
			Vector2 normalized = this.direction.Value.normalized;
			if (this._trans != null && this.space == Space.Self)
			{
				Vector3 vector = this._trans.TransformDirection(new Vector3(this.direction.Value.x, this.direction.Value.y, 0f));
				normalized.x = vector.x;
				normalized.y = vector.y;
			}
			RaycastHit2D info;
			if (this.minDepth.IsNone && this.maxDepth.IsNone)
			{
				if (this.ignoreTriggers.Value)
				{
					Helper.IsRayHittingNoTriggers(value, normalized, num, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value), out info);
				}
				else
				{
					info = Helper.Raycast2D(value, normalized, num, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
				}
			}
			else
			{
				float num2 = this.minDepth.IsNone ? float.NegativeInfinity : ((float)this.minDepth.Value);
				float num3 = this.maxDepth.IsNone ? float.PositiveInfinity : ((float)this.maxDepth.Value);
				info = Helper.Raycast2D(value, normalized, num, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value), num2, num3);
			}
			if (info.collider != null && this.ignoreTriggers.Value && info.collider.isTrigger)
			{
				info = default(RaycastHit2D);
			}
			bool flag = info.collider != null;
			PlayMakerUnity2d.RecordLastRaycastHitInfo(base.Fsm, info);
			this.storeDidHit.Value = flag;
			if (flag)
			{
				this.storeHitObject.Value = info.collider.gameObject;
				this.storeHitPoint.Value = info.point;
				this.storeHitNormal.Value = info.normal;
				this.storeHitDistance.Value = info.fraction;
				this.storeDistance.Value = info.distance;
				base.Fsm.Event(this.hitEvent);
			}
			else
			{
				base.Fsm.Event(this.noHitEvent);
			}
			if (this.debug.Value)
			{
				float d = Mathf.Min(num, 1000f);
				Vector3 vector2 = new Vector3(value.x, value.y, 0f);
				Vector3 a = new Vector3(normalized.x, normalized.y, 0f);
				Vector3 end = vector2 + a * d;
				Debug.DrawLine(vector2, end, this.debugColor.Value);
			}
		}

		// Token: 0x04006082 RID: 24706
		[ActionSection("Setup")]
		[Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
		public FsmOwnerDefault fromGameObject;

		// Token: 0x04006083 RID: 24707
		[Tooltip("Start ray at a vector2 world position. \nOr use Game Object parameter.")]
		public FsmVector2 fromPosition;

		// Token: 0x04006084 RID: 24708
		[Tooltip("A vector2 direction vector")]
		public FsmVector2 direction;

		// Token: 0x04006085 RID: 24709
		[Tooltip("Cast the ray in world or local space. Note if no Game Object is specified, the direction is in world space.")]
		public Space space;

		// Token: 0x04006086 RID: 24710
		[Tooltip("The length of the ray. Set to -1 for infinity.")]
		public FsmFloat distance;

		// Token: 0x04006087 RID: 24711
		[Tooltip("Only include objects with a Z coordinate (depth) greater than this value. leave to none for no effect")]
		public FsmInt minDepth;

		// Token: 0x04006088 RID: 24712
		[Tooltip("Only include objects with a Z coordinate (depth) less than this value. leave to none")]
		public FsmInt maxDepth;

		// Token: 0x04006089 RID: 24713
		[ActionSection("Result")]
		[Tooltip("Event to send if the ray hits an object.")]
		[UIHint(UIHint.Variable)]
		public FsmEvent hitEvent;

		// Token: 0x0400608A RID: 24714
		[UIHint(UIHint.Variable)]
		public FsmEvent noHitEvent;

		// Token: 0x0400608B RID: 24715
		[Tooltip("Set a bool variable to true if hit something, otherwise false.")]
		[UIHint(UIHint.Variable)]
		public FsmBool storeDidHit;

		// Token: 0x0400608C RID: 24716
		[Tooltip("Store the game object hit in a variable.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject storeHitObject;

		// Token: 0x0400608D RID: 24717
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the 2d position of the ray hit point and store it in a variable.")]
		public FsmVector2 storeHitPoint;

		// Token: 0x0400608E RID: 24718
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the 2d normal at the hit point and store it in a variable.")]
		public FsmVector2 storeHitNormal;

		// Token: 0x0400608F RID: 24719
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
		public FsmFloat storeHitDistance;

		// Token: 0x04006090 RID: 24720
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the distance in units... hopefully.")]
		public FsmFloat storeDistance;

		// Token: 0x04006091 RID: 24721
		[ActionSection("Filter")]
		[Tooltip("Set how often to cast a ray. 0 = once, don't repeat; 1 = everyFrame; 2 = every other frame... \nSince raycasts can get expensive use the highest repeat interval you can get away with.")]
		public FsmInt repeatInterval;

		// Token: 0x04006092 RID: 24722
		[UIHint(UIHint.Layer)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;

		// Token: 0x04006093 RID: 24723
		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		// Token: 0x04006094 RID: 24724
		public FsmBool ignoreTriggers;

		// Token: 0x04006095 RID: 24725
		[ActionSection("Debug")]
		[Tooltip("The color to use for the debug line.")]
		public FsmColor debugColor;

		// Token: 0x04006096 RID: 24726
		[Tooltip("Draw a debug line. Note: Check Gizmos in the Game View to see it in game.")]
		public FsmBool debug;

		// Token: 0x04006097 RID: 24727
		private Transform _trans;

		// Token: 0x04006098 RID: 24728
		private int repeat;
	}
}
