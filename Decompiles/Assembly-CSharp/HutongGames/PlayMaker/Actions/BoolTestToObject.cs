using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BD0 RID: 3024
	public class BoolTestToObject : FsmStateAction
	{
		// Token: 0x06005CCD RID: 23757 RVA: 0x001D2F42 File Offset: 0x001D1142
		public override void Reset()
		{
			this.Test = new FsmBool
			{
				UseVariable = true
			};
			this.ExpectedValue = true;
			this.TrueObject = null;
			this.FalseObject = null;
			this.StoreResult = null;
			this.EveryFrame = false;
		}

		// Token: 0x06005CCE RID: 23758 RVA: 0x001D2F7E File Offset: 0x001D117E
		public override string ErrorCheck()
		{
			this.TrueObject.ObjectType = this.StoreResult.ObjectType;
			this.FalseObject.ObjectType = this.StoreResult.ObjectType;
			return base.ErrorCheck();
		}

		// Token: 0x06005CCF RID: 23759 RVA: 0x001D2FB2 File Offset: 0x001D11B2
		public override void OnEnter()
		{
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005CD0 RID: 23760 RVA: 0x001D2FC8 File Offset: 0x001D11C8
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06005CD1 RID: 23761 RVA: 0x001D2FD0 File Offset: 0x001D11D0
		private void DoAction()
		{
			this.StoreResult.SetValue((this.Test.Value == this.ExpectedValue.Value) ? this.TrueObject.Value : this.FalseObject.Value);
		}

		// Token: 0x04005867 RID: 22631
		public FsmBool Test;

		// Token: 0x04005868 RID: 22632
		public FsmBool ExpectedValue;

		// Token: 0x04005869 RID: 22633
		public FsmObject TrueObject;

		// Token: 0x0400586A RID: 22634
		public FsmObject FalseObject;

		// Token: 0x0400586B RID: 22635
		[UIHint(UIHint.Variable)]
		public FsmVar StoreResult;

		// Token: 0x0400586C RID: 22636
		public bool EveryFrame;
	}
}
