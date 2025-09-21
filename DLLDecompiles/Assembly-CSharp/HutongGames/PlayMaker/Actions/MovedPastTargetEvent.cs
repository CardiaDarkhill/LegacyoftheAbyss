using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CC2 RID: 3266
	public class MovedPastTargetEvent : FsmStateAction
	{
		// Token: 0x06006185 RID: 24965 RVA: 0x001EE505 File Offset: 0x001EC705
		public override void Reset()
		{
			this.Mover = null;
			this.Target = null;
			this.DistancePast = null;
			this.MinTime = null;
			this.PassedXEvent = null;
			this.PassedYEvent = null;
		}

		// Token: 0x06006186 RID: 24966 RVA: 0x001EE534 File Offset: 0x001EC734
		public override void OnEnter()
		{
			if (this.PassedXEvent == null && this.PassedYEvent == null)
			{
				base.Finish();
				return;
			}
			GameObject safe = this.Mover.GetSafe(this);
			if (safe)
			{
				this.mover = safe.transform;
			}
			if (this.Target.Value)
			{
				this.target = this.Target.Value.transform;
			}
			if (!this.mover || !this.target)
			{
				base.Finish();
				return;
			}
			this.previousPosition = this.mover.position;
			this.DoAction();
		}

		// Token: 0x06006187 RID: 24967 RVA: 0x001EE5DE File Offset: 0x001EC7DE
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06006188 RID: 24968 RVA: 0x001EE5E8 File Offset: 0x001EC7E8
		private void DoAction()
		{
			Vector2 vector = this.mover.position;
			Vector2 vector2 = vector - this.previousPosition;
			this.previousPosition = vector;
			if (vector2.magnitude < Mathf.Epsilon)
			{
				return;
			}
			if (base.State.StateTime < this.MinTime.Value)
			{
				return;
			}
			Vector2 value = this.DistancePast.Value;
			Vector2 vector3 = this.target.position;
			vector3.x += ((vector2.x > 0f) ? value.x : (-value.x));
			vector3.y += ((vector2.y > 0f) ? value.y : (-value.y));
			if (value.x != 0f)
			{
				this.SendEventIfPassed(vector2.x, vector.x, vector3.x, this.PassedXEvent);
			}
			if (value.y != 0f)
			{
				this.SendEventIfPassed(vector2.y, vector.y, vector3.y, this.PassedYEvent);
			}
		}

		// Token: 0x06006189 RID: 24969 RVA: 0x001EE703 File Offset: 0x001EC903
		private void SendEventIfPassed(float offset, float pos, float targetPos, FsmEvent fsmEvent)
		{
			if ((offset > 0f && pos >= targetPos) || (offset < 0f && pos <= targetPos))
			{
				base.Fsm.Event(fsmEvent);
			}
		}

		// Token: 0x04005FB3 RID: 24499
		public FsmOwnerDefault Mover;

		// Token: 0x04005FB4 RID: 24500
		public FsmGameObject Target;

		// Token: 0x04005FB5 RID: 24501
		public FsmVector2 DistancePast;

		// Token: 0x04005FB6 RID: 24502
		public FsmFloat MinTime;

		// Token: 0x04005FB7 RID: 24503
		public FsmEvent PassedXEvent;

		// Token: 0x04005FB8 RID: 24504
		public FsmEvent PassedYEvent;

		// Token: 0x04005FB9 RID: 24505
		private Transform mover;

		// Token: 0x04005FBA RID: 24506
		private Transform target;

		// Token: 0x04005FBB RID: 24507
		private Vector2 previousPosition;
	}
}
