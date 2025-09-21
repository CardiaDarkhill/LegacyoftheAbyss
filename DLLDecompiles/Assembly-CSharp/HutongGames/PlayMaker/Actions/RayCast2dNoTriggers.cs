using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CEE RID: 3310
	[ActionCategory("Physics 2d")]
	public class RayCast2dNoTriggers : FsmStateAction
	{
		// Token: 0x0600624C RID: 25164 RVA: 0x001F11D0 File Offset: 0x001EF3D0
		public override void Reset()
		{
			this.FromGameObject = null;
			this.FromPosition = new FsmVector2
			{
				UseVariable = true
			};
			this.Direction = new FsmVector2
			{
				UseVariable = true
			};
			this.Distance = 100f;
			this.HitEvent = null;
			this.NoHitEvent = null;
			this.StoreDidHit = null;
			this.StoreHitObject = null;
			this.StoreHitPoint = null;
			this.StoreHitNormal = null;
			this.StoreHitDistance = null;
			this.StoreDistance = null;
			this.RepeatInterval = 1;
			this.LayerMask = new FsmInt[0];
			this.InvertMask = false;
		}

		// Token: 0x0600624D RID: 25165 RVA: 0x001F1274 File Offset: 0x001EF474
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.FromGameObject);
			if (ownerDefaultTarget != null)
			{
				this.trans = ownerDefaultTarget.transform;
			}
			this.DoRaycast();
			if (this.RepeatInterval.Value == 0)
			{
				base.Finish();
			}
		}

		// Token: 0x0600624E RID: 25166 RVA: 0x001F12C1 File Offset: 0x001EF4C1
		public override void OnUpdate()
		{
			this.repeat--;
			if (this.repeat == 0)
			{
				this.DoRaycast();
			}
		}

		// Token: 0x0600624F RID: 25167 RVA: 0x001F12E0 File Offset: 0x001EF4E0
		private void DoRaycast()
		{
			this.repeat = this.RepeatInterval.Value;
			if (Math.Abs(this.Distance.Value) <= Mathf.Epsilon)
			{
				return;
			}
			Vector2 value = this.FromPosition.Value;
			if (this.trans != null)
			{
				Vector3 position = this.trans.position;
				value.x += position.x;
				value.y += position.y;
			}
			float length = float.PositiveInfinity;
			if (this.Distance.Value > 0f)
			{
				length = this.Distance.Value;
			}
			Vector2 normalized = this.Direction.Value.normalized;
			RaycastHit2D info;
			bool flag = Helper.IsRayHittingNoTriggers(value, normalized, length, ActionHelpers.LayerArrayToLayerMask(this.LayerMask, this.InvertMask.Value), out info);
			PlayMakerUnity2d.RecordLastRaycastHitInfo(base.Fsm, info);
			this.StoreDidHit.Value = flag;
			if (flag)
			{
				this.StoreHitObject.Value = info.collider.gameObject;
				this.StoreHitPoint.Value = info.point;
				this.StoreHitNormal.Value = info.normal;
				this.StoreHitDistance.Value = info.fraction;
				this.StoreDistance.Value = info.distance;
				base.Fsm.Event(this.HitEvent);
				return;
			}
			base.Fsm.Event(this.NoHitEvent);
		}

		// Token: 0x04006071 RID: 24689
		public FsmOwnerDefault FromGameObject;

		// Token: 0x04006072 RID: 24690
		public FsmVector2 FromPosition;

		// Token: 0x04006073 RID: 24691
		public FsmVector2 Direction;

		// Token: 0x04006074 RID: 24692
		public FsmFloat Distance;

		// Token: 0x04006075 RID: 24693
		[UIHint(UIHint.Variable)]
		public FsmEvent HitEvent;

		// Token: 0x04006076 RID: 24694
		[UIHint(UIHint.Variable)]
		public FsmEvent NoHitEvent;

		// Token: 0x04006077 RID: 24695
		[UIHint(UIHint.Variable)]
		public FsmBool StoreDidHit;

		// Token: 0x04006078 RID: 24696
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreHitObject;

		// Token: 0x04006079 RID: 24697
		[UIHint(UIHint.Variable)]
		public FsmVector2 StoreHitPoint;

		// Token: 0x0400607A RID: 24698
		[UIHint(UIHint.Variable)]
		public FsmVector2 StoreHitNormal;

		// Token: 0x0400607B RID: 24699
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreHitDistance;

		// Token: 0x0400607C RID: 24700
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreDistance;

		// Token: 0x0400607D RID: 24701
		public FsmInt RepeatInterval;

		// Token: 0x0400607E RID: 24702
		[UIHint(UIHint.Layer)]
		public FsmInt[] LayerMask;

		// Token: 0x0400607F RID: 24703
		public FsmBool InvertMask;

		// Token: 0x04006080 RID: 24704
		private Transform trans;

		// Token: 0x04006081 RID: 24705
		private int repeat;
	}
}
