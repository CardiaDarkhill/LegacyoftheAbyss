using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200128F RID: 4751
	public class SavedItemCanGetMore : FSMUtility.CheckFsmStateAction
	{
		// Token: 0x06007CDA RID: 31962 RVA: 0x00254950 File Offset: 0x00252B50
		public override void Reset()
		{
			base.Reset();
			this.Item = null;
		}

		// Token: 0x17000C1A RID: 3098
		// (get) Token: 0x06007CDB RID: 31963 RVA: 0x00254960 File Offset: 0x00252B60
		public override bool IsTrue
		{
			get
			{
				SavedItem savedItem = this.Item.Value as SavedItem;
				return savedItem != null && savedItem.CanGetMore();
			}
		}

		// Token: 0x04007CED RID: 31981
		[ObjectType(typeof(SavedItem))]
		public FsmObject Item;
	}
}
