using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001324 RID: 4900
	public class CheckIfToolEquipped : FSMUtility.CheckFsmStateAction
	{
		// Token: 0x06007EFE RID: 32510 RVA: 0x0025A536 File Offset: 0x00258736
		public override void Reset()
		{
			base.Reset();
			this.Tool = null;
			this.RequiredAmountLeft = new FsmInt
			{
				UseVariable = true
			};
		}

		// Token: 0x17000C36 RID: 3126
		// (get) Token: 0x06007EFF RID: 32511 RVA: 0x0025A558 File Offset: 0x00258758
		public override bool IsTrue
		{
			get
			{
				ToolItem toolItem = this.Tool.Value as ToolItem;
				return !(toolItem == null) && ((base.Owner.layer == 5) ? toolItem.IsEquippedHud : toolItem.IsEquipped) && (toolItem.BaseStorageAmount <= 0 || toolItem.SavedData.AmountLeft >= this.RequiredAmountLeft.Value);
			}
		}

		// Token: 0x04007E9C RID: 32412
		[ObjectType(typeof(ToolItem))]
		public FsmObject Tool;

		// Token: 0x04007E9D RID: 32413
		public FsmInt RequiredAmountLeft;
	}
}
