using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BD1 RID: 3025
	public class BoolTestToSprite : FsmStateAction
	{
		// Token: 0x06005CD3 RID: 23763 RVA: 0x001D3015 File Offset: 0x001D1215
		public override void Reset()
		{
			this.Test = new FsmBool
			{
				UseVariable = true
			};
			this.ExpectedValue = true;
			this.TrueSprite = null;
			this.FalseSprite = null;
			this.StoreResult = null;
			this.EveryFrame = false;
		}

		// Token: 0x06005CD4 RID: 23764 RVA: 0x001D3051 File Offset: 0x001D1251
		public override void OnEnter()
		{
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005CD5 RID: 23765 RVA: 0x001D3067 File Offset: 0x001D1267
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06005CD6 RID: 23766 RVA: 0x001D306F File Offset: 0x001D126F
		private void DoAction()
		{
			this.StoreResult.Value = ((this.Test.Value == this.ExpectedValue.Value) ? this.TrueSprite.Value : this.FalseSprite.Value);
		}

		// Token: 0x0400586D RID: 22637
		public FsmBool Test;

		// Token: 0x0400586E RID: 22638
		public FsmBool ExpectedValue;

		// Token: 0x0400586F RID: 22639
		[ObjectType(typeof(Sprite))]
		public FsmObject TrueSprite;

		// Token: 0x04005870 RID: 22640
		[ObjectType(typeof(Sprite))]
		public FsmObject FalseSprite;

		// Token: 0x04005871 RID: 22641
		[ObjectType(typeof(Sprite))]
		[UIHint(UIHint.Variable)]
		public FsmObject StoreResult;

		// Token: 0x04005872 RID: 22642
		public bool EveryFrame;
	}
}
