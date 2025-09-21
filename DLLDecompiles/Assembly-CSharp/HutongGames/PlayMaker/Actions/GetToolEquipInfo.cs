using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200132B RID: 4907
	public class GetToolEquipInfo : FsmStateAction
	{
		// Token: 0x06007F13 RID: 32531 RVA: 0x0025A846 File Offset: 0x00258A46
		public override void Reset()
		{
			this.Tool = null;
			this.StoreIsEquipped = null;
			this.StoreIsUnlocked = null;
			this.StoreAmountLeft = null;
			this.StoreMaxAmount = null;
			this.SomeLeftEquippedEvent = null;
			this.NoneLeftEquippedEvent = null;
			this.EveryFrame = false;
		}

		// Token: 0x06007F14 RID: 32532 RVA: 0x0025A880 File Offset: 0x00258A80
		public override void OnEnter()
		{
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007F15 RID: 32533 RVA: 0x0025A896 File Offset: 0x00258A96
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06007F16 RID: 32534 RVA: 0x0025A8A0 File Offset: 0x00258AA0
		private void DoAction()
		{
			ToolItem toolItem = this.Tool.Value as ToolItem;
			if (toolItem != null)
			{
				ToolItemsData.Data savedData = toolItem.SavedData;
				bool isEquipped = toolItem.IsEquipped;
				this.StoreIsEquipped.Value = isEquipped;
				this.StoreIsUnlocked.Value = savedData.IsUnlocked;
				this.StoreAmountLeft.Value = savedData.AmountLeft;
				this.StoreMaxAmount.Value = ToolItemManager.GetToolStorageAmount(toolItem);
				base.Fsm.Event((isEquipped && savedData.AmountLeft > 0) ? this.SomeLeftEquippedEvent : this.NoneLeftEquippedEvent);
				return;
			}
			this.StoreIsEquipped.Value = false;
			this.StoreIsUnlocked.Value = false;
			this.StoreAmountLeft.Value = 0;
			base.Fsm.Event(this.NoneLeftEquippedEvent);
		}

		// Token: 0x04007EAA RID: 32426
		[ObjectType(typeof(ToolItem))]
		public FsmObject Tool;

		// Token: 0x04007EAB RID: 32427
		[UIHint(UIHint.Variable)]
		public FsmBool StoreIsEquipped;

		// Token: 0x04007EAC RID: 32428
		[UIHint(UIHint.Variable)]
		public FsmBool StoreIsUnlocked;

		// Token: 0x04007EAD RID: 32429
		[UIHint(UIHint.Variable)]
		public FsmInt StoreAmountLeft;

		// Token: 0x04007EAE RID: 32430
		[UIHint(UIHint.Variable)]
		public FsmInt StoreMaxAmount;

		// Token: 0x04007EAF RID: 32431
		public FsmEvent SomeLeftEquippedEvent;

		// Token: 0x04007EB0 RID: 32432
		public FsmEvent NoneLeftEquippedEvent;

		// Token: 0x04007EB1 RID: 32433
		public bool EveryFrame;
	}
}
