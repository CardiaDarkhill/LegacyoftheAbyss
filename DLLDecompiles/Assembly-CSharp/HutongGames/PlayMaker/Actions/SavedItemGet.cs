using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200128C RID: 4748
	public class SavedItemGet : FsmStateAction
	{
		// Token: 0x06007CCE RID: 31950 RVA: 0x002547CB File Offset: 0x002529CB
		public override void Reset()
		{
			this.Item = null;
		}

		// Token: 0x06007CCF RID: 31951 RVA: 0x002547D4 File Offset: 0x002529D4
		public override void OnEnter()
		{
			SavedItem savedItem = this.Item.Value as SavedItem;
			if (savedItem != null)
			{
				savedItem.Get(true);
			}
			base.Finish();
		}

		// Token: 0x04007CE6 RID: 31974
		[ObjectType(typeof(SavedItem))]
		public FsmObject Item;
	}
}
