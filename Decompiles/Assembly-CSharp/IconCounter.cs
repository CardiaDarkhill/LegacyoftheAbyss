using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x0200066A RID: 1642
[ExecuteInEditMode]
public class IconCounter : MonoBehaviour
{
	// Token: 0x170006A4 RID: 1700
	// (get) Token: 0x06003AE2 RID: 15074 RVA: 0x0010351D File Offset: 0x0010171D
	// (set) Token: 0x06003AE3 RID: 15075 RVA: 0x00103528 File Offset: 0x00101728
	public int MaxValue
	{
		get
		{
			return this.maxValue;
		}
		set
		{
			int num = Mathf.Max(value, 0);
			if (this.maxValue == num)
			{
				return;
			}
			this.maxValue = num;
			this.Setup(false);
		}
	}

	// Token: 0x170006A5 RID: 1701
	// (get) Token: 0x06003AE4 RID: 15076 RVA: 0x00103555 File Offset: 0x00101755
	// (set) Token: 0x06003AE5 RID: 15077 RVA: 0x00103560 File Offset: 0x00101760
	public int CurrentValue
	{
		get
		{
			return this.currentValue;
		}
		set
		{
			int num = Mathf.Clamp(value, 0, this.maxValue);
			if (this.currentValue == num)
			{
				return;
			}
			this.currentValue = num;
			this.UpdateStates();
		}
	}

	// Token: 0x170006A6 RID: 1702
	// (get) Token: 0x06003AE6 RID: 15078 RVA: 0x00103592 File Offset: 0x00101792
	// (set) Token: 0x06003AE7 RID: 15079 RVA: 0x0010359A File Offset: 0x0010179A
	public int RowSplit
	{
		get
		{
			return this.rowSplit;
		}
		set
		{
			if (this.rowSplit == value)
			{
				return;
			}
			this.rowSplit = value;
			this.Setup(false);
		}
	}

	// Token: 0x170006A7 RID: 1703
	// (get) Token: 0x06003AE8 RID: 15080 RVA: 0x001035B4 File Offset: 0x001017B4
	// (set) Token: 0x06003AE9 RID: 15081 RVA: 0x001035BC File Offset: 0x001017BC
	public Vector2 MaxItemOffset
	{
		get
		{
			return this.maxItemOffset;
		}
		set
		{
			if (this.maxItemOffset == value)
			{
				return;
			}
			this.maxItemOffset = value;
			this.Setup(false);
		}
	}

	// Token: 0x06003AEA RID: 15082 RVA: 0x001035DC File Offset: 0x001017DC
	private void OnDrawGizmosSelected()
	{
		if (this.layoutGroup)
		{
			return;
		}
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Vector3 size = this.maxSize;
		Gizmos.DrawWireCube(this.maxSize / 2f, size);
	}

	// Token: 0x06003AEB RID: 15083 RVA: 0x00103630 File Offset: 0x00101830
	private void OnValidate()
	{
		if (!base.enabled || !base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.currentValue < 0)
		{
			this.currentValue = 0;
		}
		else if (this.currentValue > this.maxValue)
		{
			this.currentValue = this.maxValue;
		}
		if (this.maxValue < 0)
		{
			this.maxValue = 0;
		}
		this.Setup(false);
	}

	// Token: 0x06003AEC RID: 15084 RVA: 0x00103696 File Offset: 0x00101896
	private void OnEnable()
	{
		this.Setup(false);
	}

	// Token: 0x06003AED RID: 15085 RVA: 0x0010369F File Offset: 0x0010189F
	private void Setup(bool destroyExisting = false)
	{
		this.GetItems(destroyExisting);
		this.DoLayout();
		this.UpdateStates();
	}

	// Token: 0x06003AEE RID: 15086 RVA: 0x001036B4 File Offset: 0x001018B4
	private void GetItems(bool destroyExisting = false)
	{
		if (!this.templateItem)
		{
			this.items = null;
			return;
		}
		this.items = new List<IconCounterItem>();
		List<IconCounterItem> list = new List<IconCounterItem>(base.transform.childCount);
		foreach (object obj in base.transform)
		{
			IconCounterItem component = ((Transform)obj).GetComponent<IconCounterItem>();
			if (component && component != this.templateItem)
			{
				list.Add(component);
			}
		}
		if (destroyExisting)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				Object.DestroyImmediate(list[i].gameObject);
			}
			list.Clear();
		}
		this.items.AddRange(list);
		int j = this.maxValue - this.items.Count;
		this.templateItem.gameObject.SetActive(true);
		while (j > 0)
		{
			IconCounterItem iconCounterItem = Object.Instantiate<IconCounterItem>(this.templateItem);
			Transform transform = iconCounterItem.transform;
			transform.SetParent(base.transform);
			transform.localPosition = Vector3.zero;
			this.items.Add(iconCounterItem);
			j--;
		}
		this.templateItem.gameObject.SetActive(false);
		while (j < 0)
		{
			int index = this.items.Count - 1;
			this.items[index].gameObject.SetActive(false);
			this.items.RemoveAt(index);
			j++;
		}
		for (int k = 0; k < this.items.Count; k++)
		{
			IconCounterItem iconCounterItem2 = this.items[k];
			iconCounterItem2.gameObject.SetActive(true);
			if (this.getSpriteFunc != null)
			{
				iconCounterItem2.Sprite = this.getSpriteFunc(k);
			}
		}
	}

	// Token: 0x06003AEF RID: 15087 RVA: 0x001038A0 File Offset: 0x00101AA0
	private void DoLayout()
	{
		if (this.layoutGroup)
		{
			this.layoutGroup.ForceUpdateLayoutNoCanvas();
			return;
		}
		if (this.items == null || this.items.Count == 0)
		{
			return;
		}
		int num = (this.rowSplit > 0) ? Mathf.Min(this.rowSplit, this.items.Count) : this.items.Count;
		int num2 = (this.items.Count > num) ? Mathf.CeilToInt((float)this.items.Count / (float)this.rowSplit) : 1;
		Vector2 vector = this.maxItemOffset.MultiplyElements(new float?((float)num), new float?((float)num2));
		Vector2 vector2 = new Vector2((Mathf.Abs(this.maxSize.x) > 0f) ? (this.maxSize.x / vector.x) : vector.x, (Mathf.Abs(this.maxSize.y) > 0f) ? (this.maxSize.y / vector.y) : vector.y);
		if (Mathf.Abs(vector2.x) > 1f)
		{
			vector2.x = 1f;
		}
		if (Mathf.Abs(vector2.y) > 1f)
		{
			vector2.y = 1f;
		}
		Vector2 vector3 = this.maxItemOffset.MultiplyElements(vector2);
		for (int i = 0; i < this.items.Count; i++)
		{
			Transform transform = this.items[i].transform;
			if (num > 0)
			{
				int num3 = i % num;
				int num4 = i / num;
				float num7;
				if (this.centreHorizontal)
				{
					int num5 = num2 - 1;
					int num6;
					if (num4 >= num5)
					{
						num6 = this.items.Count;
						for (int j = 0; j < num5; j++)
						{
							num6 -= num;
						}
					}
					else
					{
						num6 = num;
					}
					num7 = (float)num6 * vector3.x * -0.5f + vector3.x * 0.5f;
				}
				else
				{
					num7 = 0f;
				}
				transform.SetLocalPosition2D(new Vector2(vector3.x * (float)num3 + num7, vector3.y * (float)num4));
			}
			else
			{
				transform.SetLocalPosition2D(new Vector2(vector3.x * (float)i, 0f));
			}
		}
	}

	// Token: 0x06003AF0 RID: 15088 RVA: 0x00103AF4 File Offset: 0x00101CF4
	private void UpdateStates()
	{
		if (this.items == null)
		{
			return;
		}
		for (int i = 0; i < this.items.Count; i++)
		{
			IconCounterItem iconCounterItem = this.items[i];
			Func<int, bool> func = this.getIsFilledFunc;
			iconCounterItem.SetFilled((func != null) ? func(i) : (i < this.currentValue));
		}
	}

	// Token: 0x06003AF1 RID: 15089 RVA: 0x00103B4C File Offset: 0x00101D4C
	public void SetFilledOverride(Func<int, bool> getIsFilled)
	{
		this.getIsFilledFunc = getIsFilled;
		this.UpdateStates();
	}

	// Token: 0x06003AF2 RID: 15090 RVA: 0x00103B5C File Offset: 0x00101D5C
	public void SetItemSprites(Func<int, Sprite> getSprite, float scale)
	{
		IconCounter.<>c__DisplayClass31_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.scale = scale;
		this.getSpriteFunc = getSprite;
		for (int i = 0; i < this.items.Count; i++)
		{
			this.<SetItemSprites>g__SetSprite|31_0(this.items[i], i, ref CS$<>8__locals1);
		}
	}

	// Token: 0x06003AF3 RID: 15091 RVA: 0x00103BAC File Offset: 0x00101DAC
	public void SetColor(Color color)
	{
		foreach (IconCounterItem iconCounterItem in this.items)
		{
			iconCounterItem.TintColor = color;
		}
	}

	// Token: 0x06003AF4 RID: 15092 RVA: 0x00103C00 File Offset: 0x00101E00
	[ContextMenu("Destroy Existing")]
	public void DestroyExisting()
	{
		this.Setup(true);
	}

	// Token: 0x06003AF5 RID: 15093 RVA: 0x00103C09 File Offset: 0x00101E09
	public void SetMaxComplete(int value)
	{
		this.maxValue = value;
		this.Setup(true);
	}

	// Token: 0x06003AF6 RID: 15094 RVA: 0x00103C19 File Offset: 0x00101E19
	public void SetCurrent(int value)
	{
		this.CurrentValue = value;
	}

	// Token: 0x06003AF7 RID: 15095 RVA: 0x00103C24 File Offset: 0x00101E24
	public void IncrementCurrent()
	{
		int num = this.CurrentValue;
		this.CurrentValue = num + 1;
	}

	// Token: 0x06003AF9 RID: 15097 RVA: 0x00103C49 File Offset: 0x00101E49
	[CompilerGenerated]
	private void <SetItemSprites>g__SetSprite|31_0(IconCounterItem item, int index, ref IconCounter.<>c__DisplayClass31_0 A_3)
	{
		item.Sprite = this.getSpriteFunc(index);
		item.Scale = new Vector3(A_3.scale, A_3.scale, 1f);
	}

	// Token: 0x04003D3C RID: 15676
	[Header("Required")]
	[SerializeField]
	private IconCounterItem templateItem;

	// Token: 0x04003D3D RID: 15677
	[Header("Layout")]
	[SerializeField]
	private LayoutGroup layoutGroup;

	// Token: 0x04003D3E RID: 15678
	[SerializeField]
	[FormerlySerializedAs("itemOffset")]
	[ModifiableProperty]
	[Conditional("layoutGroup", false, false, false)]
	private Vector2 maxItemOffset;

	// Token: 0x04003D3F RID: 15679
	[SerializeField]
	[ModifiableProperty]
	[Conditional("layoutGroup", false, false, false)]
	private int rowSplit;

	// Token: 0x04003D40 RID: 15680
	[SerializeField]
	[ModifiableProperty]
	[Conditional("layoutGroup", false, false, false)]
	private Vector2 maxSize;

	// Token: 0x04003D41 RID: 15681
	[SerializeField]
	private bool centreHorizontal;

	// Token: 0x04003D42 RID: 15682
	[Header("Parameters")]
	[SerializeField]
	private int maxValue;

	// Token: 0x04003D43 RID: 15683
	[SerializeField]
	private int currentValue;

	// Token: 0x04003D44 RID: 15684
	private Func<int, bool> getIsFilledFunc;

	// Token: 0x04003D45 RID: 15685
	private Func<int, Sprite> getSpriteFunc;

	// Token: 0x04003D46 RID: 15686
	private List<IconCounterItem> items;
}
