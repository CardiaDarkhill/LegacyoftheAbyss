using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200128D RID: 4749
	public class SavedItemGetV2 : FsmStateAction
	{
		// Token: 0x06007CD1 RID: 31953 RVA: 0x00254810 File Offset: 0x00252A10
		public override void Reset()
		{
			this.Item = null;
			this.Amount = 1;
			this.ShowPopup = true;
		}

		// Token: 0x06007CD2 RID: 31954 RVA: 0x00254834 File Offset: 0x00252A34
		public override void OnEnter()
		{
			SavedItem savedItem = this.Item.Value as SavedItem;
			if (savedItem != null)
			{
				savedItem.Get(this.Amount.Value, this.ShowPopup.Value);
			}
			base.Finish();
		}

		// Token: 0x04007CE7 RID: 31975
		[ObjectType(typeof(SavedItem))]
		public FsmObject Item;

		// Token: 0x04007CE8 RID: 31976
		public FsmInt Amount;

		// Token: 0x04007CE9 RID: 31977
		public FsmBool ShowPopup;
	}
}
