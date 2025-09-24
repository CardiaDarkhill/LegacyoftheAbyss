using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C70 RID: 3184
	[ActionCategory(ActionCategory.Transform)]
	public class GetMiddleObjectX : FsmStateAction
	{
		// Token: 0x06006018 RID: 24600 RVA: 0x001E70AC File Offset: 0x001E52AC
		public override void Reset()
		{
			this.objectA = null;
			this.objectB = null;
			this.objectC = null;
			this.storeMiddleObject = null;
			this.everyFrame = false;
		}

		// Token: 0x06006019 RID: 24601 RVA: 0x001E70D1 File Offset: 0x001E52D1
		public override void OnEnter()
		{
			this.DoGetMiddle();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600601A RID: 24602 RVA: 0x001E70E7 File Offset: 0x001E52E7
		public override void OnUpdate()
		{
			this.DoGetMiddle();
		}

		// Token: 0x0600601B RID: 24603 RVA: 0x001E70F0 File Offset: 0x001E52F0
		private void DoGetMiddle()
		{
			float x = this.objectA.Value.transform.position.x;
			float x2 = this.objectB.Value.transform.position.x;
			float x3 = this.objectC.Value.transform.position.x;
			if ((x <= x2 && x >= x3) || (x >= x2 && x <= x3))
			{
				this.storeMiddleObject.Value = this.objectA.Value;
				return;
			}
			if ((x2 <= x && x2 >= x3) || (x2 >= x && x2 <= x3))
			{
				this.storeMiddleObject.Value = this.objectB.Value;
				return;
			}
			this.storeMiddleObject.Value = this.objectC.Value;
		}

		// Token: 0x04005D73 RID: 23923
		[RequiredField]
		public FsmGameObject objectA;

		// Token: 0x04005D74 RID: 23924
		[RequiredField]
		public FsmGameObject objectB;

		// Token: 0x04005D75 RID: 23925
		[RequiredField]
		public FsmGameObject objectC;

		// Token: 0x04005D76 RID: 23926
		[UIHint(UIHint.Variable)]
		public FsmGameObject storeMiddleObject;

		// Token: 0x04005D77 RID: 23927
		public bool everyFrame;
	}
}
