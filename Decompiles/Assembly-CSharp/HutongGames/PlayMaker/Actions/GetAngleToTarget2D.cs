using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C59 RID: 3161
	[ActionCategory("Enemy AI")]
	[Tooltip("Get the angle from Game Object to the target. 0 is right, 90 is up etc.")]
	public class GetAngleToTarget2D : FsmStateAction
	{
		// Token: 0x06005FAE RID: 24494 RVA: 0x001E57DB File Offset: 0x001E39DB
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.offsetX = null;
			this.offsetY = null;
			this.storeAngle = null;
			this.everyFrame = false;
			this.pause = null;
		}

		// Token: 0x06005FAF RID: 24495 RVA: 0x001E5810 File Offset: 0x001E3A10
		public override void OnEnter()
		{
			if (!this.pause.IsNone)
			{
				this.timer = this.pause.Value;
			}
			else
			{
				this.timer = 0f;
			}
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.offsetX.IsNone)
			{
				this.offsetX.Value = 0f;
			}
			if (this.offsetY.IsNone)
			{
				this.offsetY.Value = 0f;
			}
			if (this.timer == 0f)
			{
				this.DoGetAngle();
			}
		}

		// Token: 0x06005FB0 RID: 24496 RVA: 0x001E58B1 File Offset: 0x001E3AB1
		public override void OnUpdate()
		{
			if (this.timer > 0f)
			{
				this.timer -= Time.deltaTime;
				if (this.timer < 0f)
				{
					this.timer = 0f;
					return;
				}
			}
			else
			{
				this.DoGetAngle();
			}
		}

		// Token: 0x06005FB1 RID: 24497 RVA: 0x001E58F4 File Offset: 0x001E3AF4
		private void DoGetAngle()
		{
			if (this.target.Value == null)
			{
				return;
			}
			float num = this.target.Value.transform.position.y + this.offsetY.Value - this.self.Value.transform.position.y;
			float num2 = this.target.Value.transform.position.x + this.offsetX.Value - this.self.Value.transform.position.x;
			float num3;
			for (num3 = Mathf.Atan2(num, num2) * 57.295776f; num3 < 0f; num3 += 360f)
			{
			}
			this.storeAngle.Value = num3;
			if (!this.everyFrame)
			{
				base.Finish();
			}
			this.didGetAngle = true;
		}

		// Token: 0x06005FB2 RID: 24498 RVA: 0x001E59D8 File Offset: 0x001E3BD8
		public override void OnExit()
		{
			if (!this.didGetAngle)
			{
				this.DoGetAngle();
			}
		}

		// Token: 0x04005D06 RID: 23814
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005D07 RID: 23815
		[RequiredField]
		public FsmGameObject target;

		// Token: 0x04005D08 RID: 23816
		public FsmFloat offsetX;

		// Token: 0x04005D09 RID: 23817
		public FsmFloat offsetY;

		// Token: 0x04005D0A RID: 23818
		[RequiredField]
		public FsmFloat storeAngle;

		// Token: 0x04005D0B RID: 23819
		public FsmFloat pause;

		// Token: 0x04005D0C RID: 23820
		public bool everyFrame;

		// Token: 0x04005D0D RID: 23821
		private FsmGameObject self;

		// Token: 0x04005D0E RID: 23822
		private FsmFloat x;

		// Token: 0x04005D0F RID: 23823
		private FsmFloat y;

		// Token: 0x04005D10 RID: 23824
		private float timer;

		// Token: 0x04005D11 RID: 23825
		private bool didGetAngle;
	}
}
