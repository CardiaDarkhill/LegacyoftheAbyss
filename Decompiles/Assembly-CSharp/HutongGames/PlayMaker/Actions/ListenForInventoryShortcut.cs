using System;
using GlobalEnums;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CA3 RID: 3235
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForInventoryShortcut : FsmStateAction
	{
		// Token: 0x06006102 RID: 24834 RVA: 0x001EBBC9 File Offset: 0x001E9DC9
		public override void Reset()
		{
			this.WasPressed = null;
			this.StoreShortcut = null;
			this.CurrentPaneIndex = new FsmInt
			{
				UseVariable = true
			};
		}

		// Token: 0x06006103 RID: 24835 RVA: 0x001EBBEB File Offset: 0x001E9DEB
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
		}

		// Token: 0x06006104 RID: 24836 RVA: 0x001EBC0C File Offset: 0x001E9E0C
		public override void OnUpdate()
		{
			if (this.gm.isPaused)
			{
				return;
			}
			HeroActions inputActions = this.inputHandler.inputActions;
			InventoryPaneList.PaneTypes paneTypes = InventoryPaneInput.GetInventoryInputPressed(inputActions);
			if (paneTypes == InventoryPaneList.PaneTypes.None)
			{
				return;
			}
			if (paneTypes != InventoryPaneList.PaneTypes.Inv && !this.CurrentPaneIndex.IsNone && paneTypes != (InventoryPaneList.PaneTypes)this.CurrentPaneIndex.Value)
			{
				return;
			}
			if (inputActions.Pause.WasPressed && PlayerData.instance.isInventoryOpen)
			{
				paneTypes = InventoryPaneList.PaneTypes.Inv;
			}
			FsmEnum storeShortcut = this.StoreShortcut;
			InventoryShortcutButtons inventoryShortcutButtons;
			switch (paneTypes)
			{
			case InventoryPaneList.PaneTypes.Inv:
				inventoryShortcutButtons = InventoryShortcutButtons.Inventory;
				break;
			case InventoryPaneList.PaneTypes.Tools:
				inventoryShortcutButtons = InventoryShortcutButtons.Tools;
				break;
			case InventoryPaneList.PaneTypes.Quests:
				inventoryShortcutButtons = InventoryShortcutButtons.Quests;
				break;
			case InventoryPaneList.PaneTypes.Journal:
				inventoryShortcutButtons = InventoryShortcutButtons.Journal;
				break;
			case InventoryPaneList.PaneTypes.Map:
				inventoryShortcutButtons = InventoryShortcutButtons.Map;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			storeShortcut.Value = inventoryShortcutButtons;
			base.Fsm.Event(this.WasPressed);
		}

		// Token: 0x04005EBF RID: 24255
		public FsmEvent WasPressed;

		// Token: 0x04005EC0 RID: 24256
		[ObjectType(typeof(InventoryShortcutButtons))]
		public FsmEnum StoreShortcut;

		// Token: 0x04005EC1 RID: 24257
		public FsmInt CurrentPaneIndex;

		// Token: 0x04005EC2 RID: 24258
		private GameManager gm;

		// Token: 0x04005EC3 RID: 24259
		private InputHandler inputHandler;
	}
}
