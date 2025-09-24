using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BEE RID: 3054
	[ActionCategory("Enemy AI")]
	public class CheckPassedTarget : FsmStateAction
	{
		// Token: 0x06005D80 RID: 23936 RVA: 0x001D79A0 File Offset: 0x001D5BA0
		public override void Reset()
		{
			this.Self = null;
			this.Target = null;
			this.IsActive = true;
			this.DefaultFacingRight = null;
			this.DistancePast = null;
			this.StoreResult = null;
			this.PassedEvent = null;
			this.NotPassedEvent = null;
			this.EveryFrame = false;
		}

		// Token: 0x06005D81 RID: 23937 RVA: 0x001D79F1 File Offset: 0x001D5BF1
		public override void OnEnter()
		{
			this.self = null;
			this.target = null;
			if (!this.UpdateStartingValues())
			{
				base.Finish();
				return;
			}
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005D82 RID: 23938 RVA: 0x001D7A24 File Offset: 0x001D5C24
		public override void OnUpdate()
		{
			if (!this.UpdateStartingValues())
			{
				return;
			}
			this.DoAction();
		}

		// Token: 0x06005D83 RID: 23939 RVA: 0x001D7A38 File Offset: 0x001D5C38
		private bool UpdateStartingValues()
		{
			GameObject safe = this.Self.GetSafe(this);
			if (!safe || !this.Target.Value)
			{
				return false;
			}
			this.self = safe.transform;
			this.target = this.Target.Value.transform;
			bool flag;
			float num;
			float num2;
			this.GetParams(out flag, out num, out num2);
			return true;
		}

		// Token: 0x06005D84 RID: 23940 RVA: 0x001D7AA0 File Offset: 0x001D5CA0
		private void DoAction()
		{
			if (!this.IsActive.Value)
			{
				return;
			}
			bool flag;
			float num;
			float num2;
			this.GetParams(out flag, out num, out num2);
			float num3 = num2 + this.DistancePast.Value * (float)(flag ? 1 : -1);
			bool flag2 = (flag && num >= num3) || (!flag && num <= num3);
			this.StoreResult.Value = flag2;
			base.Fsm.Event(flag2 ? this.PassedEvent : this.NotPassedEvent);
		}

		// Token: 0x06005D85 RID: 23941 RVA: 0x001D7B24 File Offset: 0x001D5D24
		private void GetParams(out bool facingPositive, out float selfPosition, out float targetPosition)
		{
			float z = this.self.eulerAngles.z;
			if (z >= 45f && z <= 135f)
			{
				facingPositive = (this.self.localScale.x > 0f);
				selfPosition = this.self.position.y;
				targetPosition = this.target.position.y;
			}
			else if (z >= 135f && z <= 225f)
			{
				facingPositive = (this.self.localScale.x < 0f);
				selfPosition = this.self.position.x;
				targetPosition = this.target.position.x;
			}
			else if (z >= 225f && z <= 315f)
			{
				facingPositive = (this.self.localScale.x < 0f);
				selfPosition = this.self.position.y;
				targetPosition = this.target.position.y;
			}
			else
			{
				facingPositive = (this.self.localScale.x > 0f);
				selfPosition = this.self.position.x;
				targetPosition = this.target.position.x;
			}
			if (!this.DefaultFacingRight.Value)
			{
				facingPositive = !facingPositive;
			}
		}

		// Token: 0x040059AA RID: 22954
		[RequiredField]
		public FsmOwnerDefault Self;

		// Token: 0x040059AB RID: 22955
		[RequiredField]
		public FsmGameObject Target;

		// Token: 0x040059AC RID: 22956
		public FsmBool IsActive;

		// Token: 0x040059AD RID: 22957
		public FsmBool DefaultFacingRight;

		// Token: 0x040059AE RID: 22958
		public FsmFloat DistancePast;

		// Token: 0x040059AF RID: 22959
		[UIHint(UIHint.Variable)]
		public FsmBool StoreResult;

		// Token: 0x040059B0 RID: 22960
		public FsmEvent PassedEvent;

		// Token: 0x040059B1 RID: 22961
		public FsmEvent NotPassedEvent;

		// Token: 0x040059B2 RID: 22962
		private Transform self;

		// Token: 0x040059B3 RID: 22963
		private Transform target;

		// Token: 0x040059B4 RID: 22964
		public bool EveryFrame;
	}
}
