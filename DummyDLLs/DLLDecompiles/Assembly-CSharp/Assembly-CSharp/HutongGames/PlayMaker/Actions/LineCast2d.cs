using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FCF RID: 4047
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Casts a Ray against all Colliders in the scene.A linecast is an imaginary line between two points in world space. Any object making contact with the beam can be detected and reported. This differs from the similar raycast in that raycasting specifies the line using an origin and direction.Use GetRaycastHit2dInfo to get more detailed info.")]
	public class LineCast2d : FsmStateAction
	{
		// Token: 0x06006F92 RID: 28562 RVA: 0x00227BE4 File Offset: 0x00225DE4
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
			this.hitEvent = null;
			this.storeDidHit = null;
			this.storeHitObject = null;
			this.storeHitPoint = null;
			this.storeHitNormal = null;
			this.storeHitDistance = null;
			this.repeatInterval = 1;
			this.layerMask = new FsmInt[0];
			this.invertMask = false;
			this.debugColor = Color.yellow;
			this.debug = false;
		}

		// Token: 0x06006F93 RID: 28563 RVA: 0x00227C90 File Offset: 0x00225E90
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.fromGameObject);
			if (ownerDefaultTarget != null)
			{
				this._fromTrans = ownerDefaultTarget.transform;
			}
			GameObject value = this.toGameObject.Value;
			if (value != null)
			{
				this._toTrans = value.transform;
			}
			this.DoRaycast();
			if (this.repeatInterval.Value == 0)
			{
				base.Finish();
			}
		}

		// Token: 0x06006F94 RID: 28564 RVA: 0x00227CFE File Offset: 0x00225EFE
		public override void OnUpdate()
		{
			this.repeat--;
			if (this.repeat == 0)
			{
				this.DoRaycast();
			}
		}

		// Token: 0x06006F95 RID: 28565 RVA: 0x00227D1C File Offset: 0x00225F1C
		private void DoRaycast()
		{
			this.repeat = this.repeatInterval.Value;
			Vector2 value = this.fromPosition.Value;
			if (this._fromTrans != null)
			{
				value.x += this._fromTrans.position.x;
				value.y += this._fromTrans.position.y;
			}
			Vector2 value2 = this.toPosition.Value;
			if (this._toTrans != null)
			{
				value2.x += this._toTrans.position.x;
				value2.y += this._toTrans.position.y;
			}
			RaycastHit2D info;
			if (this.minDepth.IsNone && this.maxDepth.IsNone)
			{
				info = Physics2D.Linecast(value, value2, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
			}
			else
			{
				float num = this.minDepth.IsNone ? float.NegativeInfinity : ((float)this.minDepth.Value);
				float num2 = this.maxDepth.IsNone ? float.PositiveInfinity : ((float)this.maxDepth.Value);
				info = Physics2D.Linecast(value, value2, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value), num, num2);
			}
			Fsm.RecordLastRaycastHit2DInfo(base.Fsm, info);
			bool flag = info.collider != null;
			this.storeDidHit.Value = flag;
			if (flag)
			{
				this.storeHitObject.Value = info.collider.gameObject;
				this.storeHitPoint.Value = info.point;
				this.storeHitNormal.Value = info.normal;
				this.storeHitDistance.Value = info.fraction;
				base.Fsm.Event(this.hitEvent);
			}
			if (this.debug.Value)
			{
				Vector3 start = new Vector3(value.x, value.y, 0f);
				Vector3 end = new Vector3(value2.x, value2.y, 0f);
				Debug.DrawLine(start, end, this.debugColor.Value);
			}
		}

		// Token: 0x04006F5E RID: 28510
		[ActionSection("Setup")]
		[Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
		public FsmOwnerDefault fromGameObject;

		// Token: 0x04006F5F RID: 28511
		[Tooltip("Start ray at a vector2 world position. \nOr use fromGameObject parameter. If both define, will add fromPosition to the fromGameObject position")]
		public FsmVector2 fromPosition;

		// Token: 0x04006F60 RID: 28512
		[Tooltip("End ray at game object position. \nOr use From Position parameter.")]
		public FsmGameObject toGameObject;

		// Token: 0x04006F61 RID: 28513
		[Tooltip("End ray at a vector2 world position. \nOr use fromGameObject parameter. If both define, will add toPosition to the ToGameObject position")]
		public FsmVector2 toPosition;

		// Token: 0x04006F62 RID: 28514
		[Tooltip("Only include objects with a Z coordinate (depth) greater than this value. leave to none for no effect")]
		public FsmInt minDepth;

		// Token: 0x04006F63 RID: 28515
		[Tooltip("Only include objects with a Z coordinate (depth) less than this value. leave to none")]
		public FsmInt maxDepth;

		// Token: 0x04006F64 RID: 28516
		[ActionSection("Result")]
		[Tooltip("Event to send if the ray hits an object.")]
		[UIHint(UIHint.Variable)]
		public FsmEvent hitEvent;

		// Token: 0x04006F65 RID: 28517
		[Tooltip("Set a bool variable to true if hit something, otherwise false.")]
		[UIHint(UIHint.Variable)]
		public FsmBool storeDidHit;

		// Token: 0x04006F66 RID: 28518
		[Tooltip("Store the game object hit in a variable.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject storeHitObject;

		// Token: 0x04006F67 RID: 28519
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the 2d position of the ray hit point and store it in a variable.")]
		public FsmVector2 storeHitPoint;

		// Token: 0x04006F68 RID: 28520
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the 2d normal at the hit point and store it in a variable.\nNote, this is a direction vector not a rotation.")]
		public FsmVector2 storeHitNormal;

		// Token: 0x04006F69 RID: 28521
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
		public FsmFloat storeHitDistance;

		// Token: 0x04006F6A RID: 28522
		[ActionSection("Filter")]
		[Tooltip("Set how often to cast a ray. 0 = once, don't repeat; 1 = everyFrame; 2 = every other frame... \nBecause raycasts can get expensive use the highest repeat interval you can get away with.")]
		public FsmInt repeatInterval;

		// Token: 0x04006F6B RID: 28523
		[UIHint(UIHint.Layer)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;

		// Token: 0x04006F6C RID: 28524
		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		// Token: 0x04006F6D RID: 28525
		[ActionSection("Debug")]
		[Tooltip("The color to use for the debug line.")]
		public FsmColor debugColor;

		// Token: 0x04006F6E RID: 28526
		[Tooltip("Draw a debug line. Note: Check Gizmos in the Game View to see it in game.")]
		public FsmBool debug;

		// Token: 0x04006F6F RID: 28527
		private Transform _fromTrans;

		// Token: 0x04006F70 RID: 28528
		private Transform _toTrans;

		// Token: 0x04006F71 RID: 28529
		private int repeat;
	}
}
