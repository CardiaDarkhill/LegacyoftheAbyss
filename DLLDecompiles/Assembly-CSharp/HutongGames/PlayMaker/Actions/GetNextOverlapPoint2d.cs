using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FC6 RID: 4038
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Iterate through a list of all colliders that overlap a point in space.The colliders iterated are sorted in order of increasing Z coordinate. No iteration will take place if there are no colliders overlap this point.")]
	public class GetNextOverlapPoint2d : FsmStateAction
	{
		// Token: 0x06006F64 RID: 28516 RVA: 0x00226FC0 File Offset: 0x002251C0
		public override void Reset()
		{
			this.gameObject = null;
			this.position = new FsmVector2
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
			this.loopEvent = null;
			this.finishedEvent = null;
		}

		// Token: 0x06006F65 RID: 28517 RVA: 0x00227048 File Offset: 0x00225248
		public override void OnEnter()
		{
			if (this.colliders == null || this.resetFlag.Value)
			{
				this.nextColliderIndex = 0;
				this.colliders = this.GetOverlapPointAll();
				this.colliderCount = this.colliders.Length;
				this.collidersCount.Value = this.colliderCount;
				this.resetFlag.Value = false;
			}
			this.DoGetNextCollider();
			base.Finish();
		}

		// Token: 0x06006F66 RID: 28518 RVA: 0x002270B4 File Offset: 0x002252B4
		private void DoGetNextCollider()
		{
			if (this.nextColliderIndex >= this.colliderCount)
			{
				this.nextColliderIndex = 0;
				base.Fsm.Event(this.finishedEvent);
				return;
			}
			this.storeNextCollider.Value = this.colliders[this.nextColliderIndex].gameObject;
			if (this.nextColliderIndex >= this.colliderCount)
			{
				this.colliders = null;
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

		// Token: 0x06006F67 RID: 28519 RVA: 0x0022715C File Offset: 0x0022535C
		private Collider2D[] GetOverlapPointAll()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			Vector2 value = this.position.Value;
			if (ownerDefaultTarget != null)
			{
				value.x += ownerDefaultTarget.transform.position.x;
				value.y += ownerDefaultTarget.transform.position.y;
			}
			if (this.minDepth.IsNone && this.maxDepth.IsNone)
			{
				return Physics2D.OverlapPointAll(value, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
			}
			float num = this.minDepth.IsNone ? float.NegativeInfinity : ((float)this.minDepth.Value);
			float num2 = this.maxDepth.IsNone ? float.PositiveInfinity : ((float)this.maxDepth.Value);
			return Physics2D.OverlapPointAll(value, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value), num, num2);
		}

		// Token: 0x04006F1B RID: 28443
		[ActionSection("Setup")]
		[Tooltip("Point using the gameObject position. \nOr use From Position parameter.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006F1C RID: 28444
		[Tooltip("Point as a world position. \nOr use gameObject parameter. If both define, will add position to the gameObject position")]
		public FsmVector2 position;

		// Token: 0x04006F1D RID: 28445
		[Tooltip("Only include objects with a Z coordinate (depth) greater than this value. leave to none for no effect")]
		public FsmInt minDepth;

		// Token: 0x04006F1E RID: 28446
		[Tooltip("Only include objects with a Z coordinate (depth) less than this value. leave to none")]
		public FsmInt maxDepth;

		// Token: 0x04006F1F RID: 28447
		[Tooltip("If you want to reset the iteration, raise this flag to true when you enter the state, it will indicate you want to start from the beginning again")]
		[UIHint(UIHint.Variable)]
		public FsmBool resetFlag;

		// Token: 0x04006F20 RID: 28448
		[ActionSection("Filter")]
		[UIHint(UIHint.Layer)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;

		// Token: 0x04006F21 RID: 28449
		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		// Token: 0x04006F22 RID: 28450
		[ActionSection("Result")]
		[Tooltip("Store the number of colliders found for this overlap.")]
		[UIHint(UIHint.Variable)]
		public FsmInt collidersCount;

		// Token: 0x04006F23 RID: 28451
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the next collider in a GameObject variable.")]
		public FsmGameObject storeNextCollider;

		// Token: 0x04006F24 RID: 28452
		[Tooltip("Event to send to get the next collider.")]
		public FsmEvent loopEvent;

		// Token: 0x04006F25 RID: 28453
		[Tooltip("Event to send when there are no more colliders to iterate.")]
		public FsmEvent finishedEvent;

		// Token: 0x04006F26 RID: 28454
		private Collider2D[] colliders;

		// Token: 0x04006F27 RID: 28455
		private int colliderCount;

		// Token: 0x04006F28 RID: 28456
		private int nextColliderIndex;
	}
}
