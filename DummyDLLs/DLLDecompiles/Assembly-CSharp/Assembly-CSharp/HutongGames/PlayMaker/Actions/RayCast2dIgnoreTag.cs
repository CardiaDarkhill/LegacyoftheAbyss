using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CED RID: 3309
	[ActionCategory("Physics 2d")]
	[Tooltip("Casts a Ray against all Colliders in the scene. A raycast is conceptually like a laser beam that is fired from a point in space along a particular direction. Any object making contact with the beam can be detected and reported. Use GetRaycastHit2dInfo to get more detailed info. CHERRYNOTE: Added ability to measure distance in units to hit point.")]
	public class RayCast2dIgnoreTag : FsmStateAction
	{
		// Token: 0x06006247 RID: 25159 RVA: 0x001F0CEC File Offset: 0x001EEEEC
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

		// Token: 0x06006248 RID: 25160 RVA: 0x001F0DDC File Offset: 0x001EEFDC
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

		// Token: 0x06006249 RID: 25161 RVA: 0x001F0E29 File Offset: 0x001EF029
		public override void OnUpdate()
		{
			this.repeat--;
			if (this.repeat == 0)
			{
				this.DoRaycast();
			}
		}

		// Token: 0x0600624A RID: 25162 RVA: 0x001F0E48 File Offset: 0x001EF048
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
			if (info.collider != null && info.collider.gameObject.tag != this.ignoreTag.Value)
			{
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

		// Token: 0x04006059 RID: 24665
		[ActionSection("Setup")]
		[Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
		public FsmOwnerDefault fromGameObject;

		// Token: 0x0400605A RID: 24666
		[Tooltip("Start ray at a vector2 world position. \nOr use Game Object parameter.")]
		public FsmVector2 fromPosition;

		// Token: 0x0400605B RID: 24667
		[Tooltip("A vector2 direction vector")]
		public FsmVector2 direction;

		// Token: 0x0400605C RID: 24668
		[Tooltip("Cast the ray in world or local space. Note if no Game Object is specified, the direction is in world space.")]
		public Space space;

		// Token: 0x0400605D RID: 24669
		[Tooltip("The length of the ray. Set to -1 for infinity.")]
		public FsmFloat distance;

		// Token: 0x0400605E RID: 24670
		[Tooltip("Only include objects with a Z coordinate (depth) greater than this value. leave to none for no effect")]
		public FsmInt minDepth;

		// Token: 0x0400605F RID: 24671
		[Tooltip("Only include objects with a Z coordinate (depth) less than this value. leave to none")]
		public FsmInt maxDepth;

		// Token: 0x04006060 RID: 24672
		[ActionSection("Result")]
		[Tooltip("Event to send if the ray hits an object.")]
		[UIHint(UIHint.Variable)]
		public FsmEvent hitEvent;

		// Token: 0x04006061 RID: 24673
		[UIHint(UIHint.Variable)]
		public FsmEvent noHitEvent;

		// Token: 0x04006062 RID: 24674
		[Tooltip("Set a bool variable to true if hit something, otherwise false.")]
		[UIHint(UIHint.Variable)]
		public FsmBool storeDidHit;

		// Token: 0x04006063 RID: 24675
		[Tooltip("Store the game object hit in a variable.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject storeHitObject;

		// Token: 0x04006064 RID: 24676
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the 2d position of the ray hit point and store it in a variable.")]
		public FsmVector2 storeHitPoint;

		// Token: 0x04006065 RID: 24677
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the 2d normal at the hit point and store it in a variable.")]
		public FsmVector2 storeHitNormal;

		// Token: 0x04006066 RID: 24678
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
		public FsmFloat storeHitDistance;

		// Token: 0x04006067 RID: 24679
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the distance in units... hopefully.")]
		public FsmFloat storeDistance;

		// Token: 0x04006068 RID: 24680
		[ActionSection("Filter")]
		[Tooltip("Set how often to cast a ray. 0 = once, don't repeat; 1 = everyFrame; 2 = every other frame... \nSince raycasts can get expensive use the highest repeat interval you can get away with.")]
		public FsmInt repeatInterval;

		// Token: 0x04006069 RID: 24681
		[UIHint(UIHint.Layer)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;

		// Token: 0x0400606A RID: 24682
		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		// Token: 0x0400606B RID: 24683
		public FsmBool ignoreTriggers;

		// Token: 0x0400606C RID: 24684
		public FsmString ignoreTag;

		// Token: 0x0400606D RID: 24685
		[ActionSection("Debug")]
		[Tooltip("The color to use for the debug line.")]
		public FsmColor debugColor;

		// Token: 0x0400606E RID: 24686
		[Tooltip("Draw a debug line. Note: Check Gizmos in the Game View to see it in game.")]
		public FsmBool debug;

		// Token: 0x0400606F RID: 24687
		private Transform _trans;

		// Token: 0x04006070 RID: 24688
		private int repeat;
	}
}
