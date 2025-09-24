using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BCF RID: 3023
	public class BoolTestToGameObject : FsmStateAction
	{
		// Token: 0x06005CC8 RID: 23752 RVA: 0x001D2EAE File Offset: 0x001D10AE
		public override void Reset()
		{
			this.Test = null;
			this.ExpectedValue = true;
			this.TrueGameObject = null;
			this.FalseGameObject = null;
			this.StoreResult = null;
			this.EveryFrame = false;
		}

		// Token: 0x06005CC9 RID: 23753 RVA: 0x001D2EDF File Offset: 0x001D10DF
		public override void OnEnter()
		{
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005CCA RID: 23754 RVA: 0x001D2EF5 File Offset: 0x001D10F5
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06005CCB RID: 23755 RVA: 0x001D2EFD File Offset: 0x001D10FD
		private void DoAction()
		{
			this.StoreResult.Value = ((this.Test.Value == this.ExpectedValue.Value) ? this.TrueGameObject.Value : this.FalseGameObject.Value);
		}

		// Token: 0x04005861 RID: 22625
		public FsmBool Test;

		// Token: 0x04005862 RID: 22626
		public FsmBool ExpectedValue;

		// Token: 0x04005863 RID: 22627
		public FsmGameObject TrueGameObject;

		// Token: 0x04005864 RID: 22628
		public FsmGameObject FalseGameObject;

		// Token: 0x04005865 RID: 22629
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreResult;

		// Token: 0x04005866 RID: 22630
		public bool EveryFrame;
	}
}
