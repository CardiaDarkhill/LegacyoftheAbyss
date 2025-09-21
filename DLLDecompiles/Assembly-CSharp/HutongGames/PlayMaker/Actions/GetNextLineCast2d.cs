using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FC3 RID: 4035
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Iterate through a list of all colliders detected by a LineCastThe colliders iterated are sorted in order of increasing Z coordinate. No iteration will take place if there are no colliders within the area.")]
	public class GetNextLineCast2d : FsmStateAction
	{
		// Token: 0x06006F55 RID: 28501 RVA: 0x0022660C File Offset: 0x0022480C
		public override void Reset()
		{
			this.fromGameObject = null;
			this.fromPosition = new FsmVector2
			{
				UseVariable = true
			};
			this.toGameObject = null;
			this.toPosition = new FsmVector2
			{
				UseVariable = true
			};
			this.minDepth = new FsmInt
			{
				UseVariable = true
			};
			this.maxDepth = new FsmInt
			{
				UseVariable = true
			};
			this.layerMask = new FsmInt[0];
			this.invertMask = false;
			this.resetFlag = null;
			this.collidersCount = null;
			this.storeNextCollider = null;
			this.storeNextHitPoint = null;
			this.storeNextHitNormal = null;
			this.storeNextHitDistance = null;
			this.loopEvent = null;
			this.finishedEvent = null;
		}

		// Token: 0x06006F56 RID: 28502 RVA: 0x002266C0 File Offset: 0x002248C0
		public override void OnEnter()
		{
			if (this.hits == null || this.resetFlag.Value)
			{
				this.nextColliderIndex = 0;
				this.hits = this.GetLineCastAll();
				this.colliderCount = this.hits.Length;
				this.collidersCount.Value = this.colliderCount;
				this.resetFlag.Value = false;
			}
			this.DoGetNextCollider();
			base.Finish();
		}

		// Token: 0x06006F57 RID: 28503 RVA: 0x0022672C File Offset: 0x0022492C
		private void DoGetNextCollider()
		{
			if (this.nextColliderIndex >= this.colliderCount)
			{
				this.hits = null;
				this.nextColliderIndex = 0;
				base.Fsm.Event(this.finishedEvent);
				return;
			}
			Fsm.RecordLastRaycastHit2DInfo(base.Fsm, this.hits[this.nextColliderIndex]);
			this.storeNextCollider.Value = this.hits[this.nextColliderIndex].collider.gameObject;
			this.storeNextHitPoint.Value = this.hits[this.nextColliderIndex].point;
			this.storeNextHitNormal.Value = this.hits[this.nextColliderIndex].normal;
			this.storeNextHitDistance.Value = this.hits[this.nextColliderIndex].fraction;
			if (this.nextColliderIndex >= this.colliderCount)
			{
				this.hits = new RaycastHit2D[0];
				this.nextColliderIndex = 0;
				base.Fsm.Event(this.finishedEvent);
				return;
			}
			this.nextColliderIndex++;
			if (this.loopEvent != null)
			{
				base.Fsm.Event(this.loopEvent);
			}
		}

		// Token: 0x06006F58 RID: 28504 RVA: 0x00226868 File Offset: 0x00224A68
		private RaycastHit2D[] GetLineCastAll()
		{
			Vector2 value = this.fromPosition.Value;
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.fromGameObject);
			if (ownerDefaultTarget != null)
			{
				value.x += ownerDefaultTarget.transform.position.x;
				value.y += ownerDefaultTarget.transform.position.y;
			}
			Vector2 value2 = this.toPosition.Value;
			GameObject value3 = this.toGameObject.Value;
			if (value3 != null)
			{
				value2.x += value3.transform.position.x;
				value2.y += value3.transform.position.y;
			}
			if (this.minDepth.IsNone && this.maxDepth.IsNone)
			{
				return Physics2D.LinecastAll(value, value2, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
			}
			float num = this.minDepth.IsNone ? float.NegativeInfinity : ((float)this.minDepth.Value);
			float num2 = this.maxDepth.IsNone ? float.PositiveInfinity : ((float)this.maxDepth.Value);
			return Physics2D.LinecastAll(value, value2, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value), num, num2);
		}

		// Token: 0x04006EE9 RID: 28393
		[ActionSection("Setup")]
		[Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
		public FsmOwnerDefault fromGameObject;

		// Token: 0x04006EEA RID: 28394
		[Tooltip("Start ray at a vector2 world position. \nOr use fromGameObject parameter. If both define, will add fromPosition to the fromGameObject position")]
		public FsmVector2 fromPosition;

		// Token: 0x04006EEB RID: 28395
		[Tooltip("End ray at game object position. \nOr use From Position parameter.")]
		public FsmGameObject toGameObject;

		// Token: 0x04006EEC RID: 28396
		[Tooltip("End ray at a vector2 world position. \nOr use fromGameObject parameter. If both define, will add toPosition to the ToGameObject position")]
		public FsmVector2 toPosition;

		// Token: 0x04006EED RID: 28397
		[Tooltip("Only include objects with a Z coordinate (depth) greater than this value. leave to none for no effect")]
		public FsmInt minDepth;

		// Token: 0x04006EEE RID: 28398
		[Tooltip("Only include objects with a Z coordinate (depth) less than this value. leave to none")]
		public FsmInt maxDepth;

		// Token: 0x04006EEF RID: 28399
		[Tooltip("If you want to reset the iteration, raise this flag to true when you enter the state, it will indicate you want to start from the beginning again")]
		[UIHint(UIHint.Variable)]
		public FsmBool resetFlag;

		// Token: 0x04006EF0 RID: 28400
		[ActionSection("Filter")]
		[UIHint(UIHint.Layer)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;

		// Token: 0x04006EF1 RID: 28401
		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		// Token: 0x04006EF2 RID: 28402
		[ActionSection("Result")]
		[Tooltip("Store the number of colliders found for this overlap.")]
		[UIHint(UIHint.Variable)]
		public FsmInt collidersCount;

		// Token: 0x04006EF3 RID: 28403
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the next collider in a GameObject variable.")]
		public FsmGameObject storeNextCollider;

		// Token: 0x04006EF4 RID: 28404
		[Tooltip("Get the 2d position of the next ray hit point and store it in a variable.")]
		public FsmVector2 storeNextHitPoint;

		// Token: 0x04006EF5 RID: 28405
		[Tooltip("Get the 2d normal at the next hit point and store it in a variable.\nNote, this is a direction vector not a rotation.")]
		public FsmVector2 storeNextHitNormal;

		// Token: 0x04006EF6 RID: 28406
		[Tooltip("Get the distance along the ray to the next hit point and store it in a variable.")]
		public FsmFloat storeNextHitDistance;

		// Token: 0x04006EF7 RID: 28407
		[Tooltip("Event to send to get the next collider.")]
		public FsmEvent loopEvent;

		// Token: 0x04006EF8 RID: 28408
		[Tooltip("Event to send when there are no more colliders to iterate.")]
		public FsmEvent finishedEvent;

		// Token: 0x04006EF9 RID: 28409
		private RaycastHit2D[] hits;

		// Token: 0x04006EFA RID: 28410
		private int colliderCount;

		// Token: 0x04006EFB RID: 28411
		private int nextColliderIndex;
	}
}
