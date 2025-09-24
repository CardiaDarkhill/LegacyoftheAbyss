using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000688 RID: 1672
public class InventoryItemButtonPromptDisplayList : MonoBehaviour
{
	// Token: 0x170006BF RID: 1727
	// (get) Token: 0x06003BB1 RID: 15281 RVA: 0x001067DE File Offset: 0x001049DE
	// (set) Token: 0x06003BB2 RID: 15282 RVA: 0x001067E6 File Offset: 0x001049E6
	public int order { get; set; }

	// Token: 0x06003BB3 RID: 15283 RVA: 0x001067EF File Offset: 0x001049EF
	private void Awake()
	{
		if (this.promptTemplateSingle)
		{
			this.promptTemplateSingle.gameObject.SetActive(false);
		}
		if (this.promptTemplateCombo)
		{
			this.promptTemplateCombo.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003BB4 RID: 15284 RVA: 0x00106830 File Offset: 0x00104A30
	public void Append(InventoryItemButtonPromptData promptData, bool forceDisabled, int order = 0)
	{
		InventoryItemButtonPromptDisplay display = this.GetDisplay<InventoryItemButtonPromptDisplay>(this.promptTemplateSingle);
		display.order = order;
		display.Show(promptData, forceDisabled);
		this.AddAndOrder(display);
	}

	// Token: 0x06003BB5 RID: 15285 RVA: 0x00106860 File Offset: 0x00104A60
	public void Append(InventoryItemComboButtonPromptDisplay.Display promptData, int order = 0)
	{
		InventoryItemComboButtonPromptDisplay display = this.GetDisplay<InventoryItemComboButtonPromptDisplay>(this.promptTemplateCombo);
		display.order = order;
		display.Show(promptData);
		this.AddAndOrder(display);
	}

	// Token: 0x06003BB6 RID: 15286 RVA: 0x00106890 File Offset: 0x00104A90
	private void AddAndOrder(InventoryItemButtonPromptDisplayList.IPromptDisplayListOrder display)
	{
		int i = this.activeDisplays.BinarySearch(display, InventoryItemButtonPromptDisplayList.orderComparer);
		if (i >= 0)
		{
			while (i < this.activeDisplays.Count)
			{
				if (this.activeDisplays[i].order != display.order)
				{
					break;
				}
				if (this.activeDisplays[i].Equals(display))
				{
					return;
				}
				i++;
			}
		}
		else
		{
			i = ~i;
		}
		this.activeDisplays.Insert(i, display);
		if (i > 0)
		{
			Transform transform = this.activeDisplays[i - 1].transform;
			Transform transform2 = display.transform;
			if (transform2 != null && transform != null)
			{
				transform2.SetSiblingIndex(transform.GetSiblingIndex() + 1);
			}
		}
	}

	// Token: 0x06003BB7 RID: 15287 RVA: 0x00106948 File Offset: 0x00104B48
	private T GetDisplay<T>(T template) where T : MonoBehaviour, InventoryItemButtonPromptDisplayList.IPromptDisplayListOrder
	{
		if (!template)
		{
			return default(T);
		}
		GameObject gameObject = (from kvp in this.promptDisplays
		where kvp.Key == typeof(T)
		select kvp.Value).FirstOrDefault((GameObject obj) => !obj.activeSelf);
		T t;
		if (gameObject != null)
		{
			t = gameObject.GetComponent<T>();
		}
		else
		{
			template.gameObject.SetActive(true);
			t = Object.Instantiate<T>(template, template.transform.parent);
			template.gameObject.SetActive(false);
			this.promptDisplays.Add(new KeyValuePair<Type, GameObject>(typeof(T), t.gameObject));
		}
		return t;
	}

	// Token: 0x06003BB8 RID: 15288 RVA: 0x00106A50 File Offset: 0x00104C50
	public void Clear()
	{
		foreach (KeyValuePair<Type, GameObject> keyValuePair in this.promptDisplays)
		{
			keyValuePair.Value.SetActive(false);
		}
		this.activeDisplays.Clear();
	}

	// Token: 0x04003DE4 RID: 15844
	[FormerlySerializedAs("promptTemplate")]
	[SerializeField]
	private InventoryItemButtonPromptDisplay promptTemplateSingle;

	// Token: 0x04003DE5 RID: 15845
	[SerializeField]
	private InventoryItemComboButtonPromptDisplay promptTemplateCombo;

	// Token: 0x04003DE6 RID: 15846
	private readonly List<KeyValuePair<Type, GameObject>> promptDisplays = new List<KeyValuePair<Type, GameObject>>();

	// Token: 0x04003DE7 RID: 15847
	private static readonly InventoryItemButtonPromptDisplayList.PromptOrderComparer orderComparer = new InventoryItemButtonPromptDisplayList.PromptOrderComparer();

	// Token: 0x04003DE8 RID: 15848
	private List<InventoryItemButtonPromptDisplayList.IPromptDisplayListOrder> activeDisplays = new List<InventoryItemButtonPromptDisplayList.IPromptDisplayListOrder>();

	// Token: 0x0200198C RID: 6540
	public interface IPromptDisplayListOrder
	{
		// Token: 0x170010A8 RID: 4264
		// (get) Token: 0x0600947E RID: 38014
		// (set) Token: 0x0600947F RID: 38015
		int order { get; set; }

		// Token: 0x170010A9 RID: 4265
		// (get) Token: 0x06009480 RID: 38016
		Transform transform { get; }
	}

	// Token: 0x0200198D RID: 6541
	private class PromptOrderComparer : IComparer<InventoryItemButtonPromptDisplayList.IPromptDisplayListOrder>
	{
		// Token: 0x06009481 RID: 38017 RVA: 0x002A2CE4 File Offset: 0x002A0EE4
		public int Compare(InventoryItemButtonPromptDisplayList.IPromptDisplayListOrder x, InventoryItemButtonPromptDisplayList.IPromptDisplayListOrder y)
		{
			return x.order.CompareTo(y.order);
		}
	}
}
