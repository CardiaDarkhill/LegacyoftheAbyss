using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000591 RID: 1425
public class InventoryItemMainQuest : InventoryItemQuest, IInventorySelectionParent
{
	// Token: 0x1400009F RID: 159
	// (add) Token: 0x06003325 RID: 13093 RVA: 0x000E31E8 File Offset: 0x000E13E8
	// (remove) Token: 0x06003326 RID: 13094 RVA: 0x000E3220 File Offset: 0x000E1420
	public event Action<InventoryItemSelectable> OnSubSelected;

	// Token: 0x06003327 RID: 13095 RVA: 0x000E3258 File Offset: 0x000E1458
	public override void SetQuest(BasicQuestBase newQuest, bool isInCompletedSection)
	{
		base.SetQuest(newQuest, isInCompletedSection);
		if (!newQuest)
		{
			return;
		}
		MainQuest mainQuest = newQuest as MainQuest;
		if (mainQuest == null)
		{
			Debug.LogErrorFormat(this, "{0} is not a MainQuest. InventoryItemMainQuest should only be used with MainQuests!", new object[]
			{
				newQuest.name
			});
			return;
		}
		if (mainQuest.IsAnyAltTargetsComplete)
		{
			this.SetSubQuestsActive(false);
			return;
		}
		IReadOnlyList<SubQuest> subQuests = mainQuest.SubQuests;
		if (subQuests != null && subQuests.Count > 0)
		{
			this.subQuestItemTemplate.gameObject.SetActive(false);
			if (this.spawnedSubQuestItems == null)
			{
				this.spawnedSubQuestItems = new List<InventoryItemQuest>(subQuests.Count);
			}
			for (int i = subQuests.Count - this.spawnedSubQuestItems.Count; i > 0; i--)
			{
				InventoryItemQuest inventoryItemQuest = Object.Instantiate<InventoryItemQuest>(this.subQuestItemTemplate, this.subQuestItemTemplate.transform.parent);
				this.spawnedSubQuestItems.Add(inventoryItemQuest);
				inventoryItemQuest.OnSelected += this.SubSelected;
			}
			this.subQuestsLayout.constraintCount = ((subQuests.Count == 4) ? 2 : 3);
			int num = Mathf.CeilToInt((float)subQuests.Count / (float)this.subQuestsLayout.constraintCount);
			for (int j = 0; j < subQuests.Count; j++)
			{
				InventoryItemQuest inventoryItemQuest2 = this.spawnedSubQuestItems[j];
				inventoryItemQuest2.gameObject.SetActive(true);
				inventoryItemQuest2.SetQuest(subQuests[j].GetCurrent(), isInCompletedSection);
				inventoryItemQuest2.RegisterTextForUpdate();
				int num2 = j / this.subQuestsLayout.constraintCount;
				int index = j % this.subQuestsLayout.constraintCount;
				inventoryItemQuest2.Selectables[2] = ((j > 0) ? this.spawnedSubQuestItems[j - 1] : null);
				inventoryItemQuest2.Selectables[3] = ((j < this.spawnedSubQuestItems.Count - 1) ? this.spawnedSubQuestItems[j + 1] : null);
				if (num2 == 0)
				{
					inventoryItemQuest2.Selectables[0] = this;
				}
				else
				{
					inventoryItemQuest2.Selectables[0] = this.spawnedSubQuestItems[index];
				}
				if (num2 >= num - 1)
				{
					inventoryItemQuest2.Selectables[1] = null;
				}
				else
				{
					int index2 = j + this.subQuestsLayout.constraintCount;
					inventoryItemQuest2.Selectables[1] = this.spawnedSubQuestItems[index2];
				}
			}
			for (int k = subQuests.Count; k < this.spawnedSubQuestItems.Count; k++)
			{
				this.spawnedSubQuestItems[k].gameObject.SetActive(false);
			}
			this.SetSubQuestsActive(true);
			return;
		}
		this.SetSubQuestsActive(false);
	}

	// Token: 0x06003328 RID: 13096 RVA: 0x000E34DF File Offset: 0x000E16DF
	private void SetSubQuestsActive(bool value)
	{
		this.subQuestsParent.SetActive(value);
		this.layoutGroup.ForceUpdateLayoutNoCanvas();
	}

	// Token: 0x06003329 RID: 13097 RVA: 0x000E34F8 File Offset: 0x000E16F8
	private void SubSelected(InventoryItemSelectable selectable)
	{
		if (this.OnSubSelected != null)
		{
			this.OnSubSelected(selectable);
		}
	}

	// Token: 0x0600332A RID: 13098 RVA: 0x000E3510 File Offset: 0x000E1710
	public override InventoryItemSelectable Get(InventoryItemManager.SelectionDirection? direction)
	{
		InventoryItemManager.SelectionDirection? selectionDirection = direction;
		InventoryItemManager.SelectionDirection selectionDirection2 = InventoryItemManager.SelectionDirection.Up;
		if ((selectionDirection.GetValueOrDefault() == selectionDirection2 & selectionDirection != null) && this.spawnedSubQuestItems != null)
		{
			foreach (InventoryItemQuest y in this.spawnedSubQuestItems)
			{
				if (this.Manager.CurrentSelected == y)
				{
					return base.Get(direction);
				}
			}
			InventoryItemQuest closestSub = this.GetClosestSub(false);
			if (closestSub)
			{
				return closestSub;
			}
		}
		return base.Get(direction);
	}

	// Token: 0x0600332B RID: 13099 RVA: 0x000E35B8 File Offset: 0x000E17B8
	public override InventoryItemSelectable GetNextSelectable(InventoryItemManager.SelectionDirection direction)
	{
		if (direction == InventoryItemManager.SelectionDirection.Down && this.spawnedSubQuestItems != null)
		{
			InventoryItemQuest closestSub = this.GetClosestSub(true);
			if (closestSub)
			{
				return closestSub;
			}
		}
		return base.GetNextSelectable(direction);
	}

	// Token: 0x0600332C RID: 13100 RVA: 0x000E35EC File Offset: 0x000E17EC
	public InventoryItemSelectable GetNextSelectable(InventoryItemSelectable source, InventoryItemManager.SelectionDirection? direction)
	{
		InventoryItemManager.SelectionDirection? selectionDirection = direction;
		InventoryItemManager.SelectionDirection selectionDirection2 = InventoryItemManager.SelectionDirection.Down;
		if (selectionDirection.GetValueOrDefault() == selectionDirection2 & selectionDirection != null)
		{
			return base.GetNextSelectable(direction.Value);
		}
		return null;
	}

	// Token: 0x0600332D RID: 13101 RVA: 0x000E3620 File Offset: 0x000E1820
	private InventoryItemQuest GetClosestSub(bool fromRoot)
	{
		if (this.spawnedSubQuestItems.Count == 0)
		{
			return null;
		}
		if (fromRoot && this.subQuestsLayout.constraintCount == 2)
		{
			return this.spawnedSubQuestItems[0];
		}
		float num = float.MaxValue;
		float num2 = float.MaxValue;
		InventoryItemQuest result = null;
		foreach (InventoryItemQuest inventoryItemQuest in this.spawnedSubQuestItems)
		{
			if (inventoryItemQuest.gameObject.activeInHierarchy)
			{
				Vector3 position = inventoryItemQuest.transform.position;
				if (position.y <= num2)
				{
					num2 = position.y;
					float num3 = Mathf.Abs(position.x - base.transform.position.x);
					if (num3 <= num)
					{
						num = num3;
						result = inventoryItemQuest;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x040036F4 RID: 14068
	[Space]
	[SerializeField]
	private InventoryItemQuest subQuestItemTemplate;

	// Token: 0x040036F5 RID: 14069
	[SerializeField]
	private GameObject subQuestsParent;

	// Token: 0x040036F6 RID: 14070
	[SerializeField]
	private GridLayoutGroup subQuestsLayout;

	// Token: 0x040036F7 RID: 14071
	[SerializeField]
	private LayoutGroup layoutGroup;

	// Token: 0x040036F8 RID: 14072
	private List<InventoryItemQuest> spawnedSubQuestItems;
}
