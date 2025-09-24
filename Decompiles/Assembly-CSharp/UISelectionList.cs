using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006FA RID: 1786
public class UISelectionList : MonoBehaviour
{
	// Token: 0x06003FE7 RID: 16359 RVA: 0x00119BE8 File Offset: 0x00117DE8
	private void Awake()
	{
		if (this.listParent)
		{
			this.listItems.Clear();
			foreach (object obj in this.listParent)
			{
				UISelectionListItem component = ((Transform)obj).GetComponent<UISelectionListItem>();
				if (component)
				{
					this.listItems.Add(component);
				}
			}
		}
		InventoryPaneBase componentInParent = base.GetComponentInParent<InventoryPaneBase>();
		if (componentInParent)
		{
			UISelectionList.ListType listType = this.listType;
			if (listType != UISelectionList.ListType.Vertical)
			{
				if (listType == UISelectionList.ListType.Horizontal)
				{
					componentInParent.OnInputLeft += delegate()
					{
						this.MoveSelection(-1);
					};
					componentInParent.OnInputRight += delegate()
					{
						this.MoveSelection(1);
					};
				}
			}
			else
			{
				componentInParent.OnInputDown += delegate()
				{
					this.MoveSelection(1);
				};
				componentInParent.OnInputUp += delegate()
				{
					this.MoveSelection(-1);
				};
			}
		}
		this.manager = base.GetComponentInParent<InventoryItemManager>();
	}

	// Token: 0x06003FE8 RID: 16360 RVA: 0x00119CE4 File Offset: 0x00117EE4
	private void Update()
	{
		if (this.isInactive)
		{
			return;
		}
		if (this.manager && this.manager.IsActionsBlocked)
		{
			return;
		}
		if (this.activeCooldown > 0f)
		{
			this.activeCooldown -= Time.deltaTime;
			return;
		}
		if (this.listItems.Count > 0 && this.selectedIndex >= 0 && this.selectedIndex < this.listItems.Count)
		{
			HeroActions inputActions = GameManager.instance.inputHandler.inputActions;
			Platform.MenuActions menuAction = Platform.Current.GetMenuAction(inputActions, false, false);
			if (menuAction == Platform.MenuActions.Submit)
			{
				this.listItems[this.selectedIndex].Submit();
				return;
			}
			if (menuAction == Platform.MenuActions.Cancel || InventoryPaneInput.IsInventoryButtonPressed(inputActions))
			{
				if (this.cancelItemIndex >= 0 && this.cancelItemIndex < this.listItems.Count)
				{
					this.SetSelected(this.cancelItemIndex, false, false, true);
				}
				this.listItems[this.selectedIndex].Cancel();
			}
		}
	}

	// Token: 0x06003FE9 RID: 16361 RVA: 0x00119DF0 File Offset: 0x00117FF0
	private void MoveSelection(int direction)
	{
		if (this.isInactive || !base.isActiveAndEnabled)
		{
			return;
		}
		if (direction != 0)
		{
			direction = (int)Mathf.Sign((float)direction);
		}
		int num = this.selectedIndex + direction;
		if (num >= this.listItems.Count)
		{
			num = 0;
		}
		else if (num < 0)
		{
			num = this.listItems.Count - 1;
		}
		if (this.listItems[num].gameObject.activeSelf)
		{
			this.SetSelected(num, false, false, false);
			return;
		}
		this.selectedIndex = num;
		this.MoveSelection(direction);
	}

	// Token: 0x06003FEA RID: 16362 RVA: 0x00119E7C File Offset: 0x0011807C
	private void SetSelected(int index, bool isInstant, bool force = false, bool skipSelectSound = false)
	{
		if (this.listItems.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.listItems.Count; i++)
		{
			if (i != index || force)
			{
				if (skipSelectSound)
				{
					this.listItems[i].SkipNextSelectSound();
				}
				this.listItems[i].SetSelected(false, isInstant);
			}
		}
		if (index >= 0)
		{
			if (skipSelectSound)
			{
				this.listItems[index].SkipNextSelectSound();
			}
			this.listItems[index].SetSelected(true, isInstant);
		}
		this.selectedIndex = index;
	}

	// Token: 0x06003FEB RID: 16363 RVA: 0x00119F14 File Offset: 0x00118114
	public void SetActive(bool value)
	{
		this.isInactive = !value;
		if (!value)
		{
			this.SetSelected(-1, true, true, false);
			return;
		}
		int index = 0;
		for (int i = 0; i < this.listItems.Count; i++)
		{
			if (this.listItems[i].gameObject.activeSelf && this.listItems[i].AutoSelect != null && this.listItems[i].AutoSelect())
			{
				index = i;
				break;
			}
		}
		this.SetSelected(index, false, true, false);
		this.activeCooldown = 0.2f;
	}

	// Token: 0x0400418F RID: 16783
	[SerializeField]
	private UISelectionList.ListType listType;

	// Token: 0x04004190 RID: 16784
	[SerializeField]
	private Transform listParent;

	// Token: 0x04004191 RID: 16785
	[SerializeField]
	private List<UISelectionListItem> listItems = new List<UISelectionListItem>();

	// Token: 0x04004192 RID: 16786
	[SerializeField]
	[Tooltip("If within listItems bounds will always select the item of this index before calling cancel.")]
	private int cancelItemIndex = -1;

	// Token: 0x04004193 RID: 16787
	private int selectedIndex;

	// Token: 0x04004194 RID: 16788
	private bool isInactive;

	// Token: 0x04004195 RID: 16789
	private float activeCooldown;

	// Token: 0x04004196 RID: 16790
	private InventoryItemManager manager;

	// Token: 0x020019EB RID: 6635
	private enum ListType
	{
		// Token: 0x040097BD RID: 38845
		Vertical,
		// Token: 0x040097BE RID: 38846
		Horizontal
	}
}
