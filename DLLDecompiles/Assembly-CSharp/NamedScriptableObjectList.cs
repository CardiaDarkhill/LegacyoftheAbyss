using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020001E2 RID: 482
public class NamedScriptableObjectList<T> : NamedScriptableObjectListDummy, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IReadOnlyList<T>, IReadOnlyCollection<T> where T : ScriptableObject
{
	// Token: 0x1700020C RID: 524
	// (get) Token: 0x0600129E RID: 4766 RVA: 0x00056692 File Offset: 0x00054892
	protected List<T> List
	{
		get
		{
			return this.list;
		}
	}

	// Token: 0x0600129F RID: 4767 RVA: 0x0005669A File Offset: 0x0005489A
	private void OnEnable()
	{
		this.RemoveDuplicates();
		this.RemoveNullItems();
		this.UpdateDictionary();
	}

	// Token: 0x060012A0 RID: 4768 RVA: 0x000566B0 File Offset: 0x000548B0
	private void UpdateDictionary()
	{
		if (this.list != null)
		{
			this.dictionary = (from obj in this.list
			where obj != null
			group obj by obj.name).ToDictionary((IGrouping<string, T> group) => group.Key, (IGrouping<string, T> group) => group.FirstOrDefault<T>());
		}
	}

	// Token: 0x060012A1 RID: 4769 RVA: 0x00056760 File Offset: 0x00054960
	public T GetByName(string itemName)
	{
		if (!string.IsNullOrEmpty(itemName) && this.dictionary != null && this.dictionary.ContainsKey(itemName))
		{
			return this.dictionary[itemName];
		}
		return default(T);
	}

	// Token: 0x060012A2 RID: 4770 RVA: 0x000567A1 File Offset: 0x000549A1
	public void RemoveDuplicates()
	{
		this.InternalRemoveDuplicates();
		this.UpdateDictionary();
	}

	// Token: 0x060012A3 RID: 4771 RVA: 0x000567AF File Offset: 0x000549AF
	private void InternalRemoveDuplicates()
	{
		this.list = this.list.Distinct<T>().ToList<T>();
	}

	// Token: 0x060012A4 RID: 4772 RVA: 0x000567C7 File Offset: 0x000549C7
	public void RemoveNullItems()
	{
		this.InternalRemoveNullItems();
		this.UpdateDictionary();
	}

	// Token: 0x060012A5 RID: 4773 RVA: 0x000567D5 File Offset: 0x000549D5
	private void InternalRemoveNullItems()
	{
		this.list = (from item in this.list
		where item != null
		select item).ToList<T>();
	}

	// Token: 0x1700020D RID: 525
	// (get) Token: 0x060012A6 RID: 4774 RVA: 0x0005680C File Offset: 0x00054A0C
	public int Count
	{
		get
		{
			return this.list.Count;
		}
	}

	// Token: 0x1700020E RID: 526
	// (get) Token: 0x060012A7 RID: 4775 RVA: 0x00056819 File Offset: 0x00054A19
	public bool IsReadOnly
	{
		get
		{
			return ((ICollection<T>)this.list).IsReadOnly;
		}
	}

	// Token: 0x1700020F RID: 527
	public T this[int index]
	{
		get
		{
			return this.list[index];
		}
		set
		{
			this.list[index] = value;
			this.UpdateDictionary();
		}
	}

	// Token: 0x060012AA RID: 4778 RVA: 0x00056849 File Offset: 0x00054A49
	public void Add(T item)
	{
		this.list.Add(item);
		this.UpdateDictionary();
	}

	// Token: 0x060012AB RID: 4779 RVA: 0x0005685D File Offset: 0x00054A5D
	public void Clear()
	{
		this.list.Clear();
		this.dictionary.Clear();
	}

	// Token: 0x060012AC RID: 4780 RVA: 0x00056875 File Offset: 0x00054A75
	public bool Contains(T item)
	{
		return this.list.Contains(item);
	}

	// Token: 0x060012AD RID: 4781 RVA: 0x00056883 File Offset: 0x00054A83
	public void CopyTo(T[] array, int arrayIndex)
	{
		this.list.CopyTo(array, arrayIndex);
	}

	// Token: 0x060012AE RID: 4782 RVA: 0x00056892 File Offset: 0x00054A92
	public IEnumerator<T> GetEnumerator()
	{
		return this.list.GetEnumerator();
	}

	// Token: 0x060012AF RID: 4783 RVA: 0x000568A4 File Offset: 0x00054AA4
	public bool Remove(T item)
	{
		bool result = this.list.Remove(item);
		this.UpdateDictionary();
		return result;
	}

	// Token: 0x060012B0 RID: 4784 RVA: 0x000568B8 File Offset: 0x00054AB8
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.list.GetEnumerator();
	}

	// Token: 0x060012B1 RID: 4785 RVA: 0x000568CA File Offset: 0x00054ACA
	public int IndexOf(T item)
	{
		return this.list.IndexOf(item);
	}

	// Token: 0x060012B2 RID: 4786 RVA: 0x000568D8 File Offset: 0x00054AD8
	public void Insert(int index, T item)
	{
		this.list.Insert(index, item);
		this.UpdateDictionary();
	}

	// Token: 0x060012B3 RID: 4787 RVA: 0x000568ED File Offset: 0x00054AED
	public void RemoveAt(int index)
	{
		this.list.RemoveAt(index);
		this.UpdateDictionary();
	}

	// Token: 0x04001174 RID: 4468
	[SerializeField]
	[ContextMenuItem("Remove Duplicates", "RemoveDuplicates")]
	[ContextMenuItem("Remove Null Items", "RemoveNullItems")]
	private List<T> list = new List<T>();

	// Token: 0x04001175 RID: 4469
	[NonSerialized]
	private Dictionary<string, T> dictionary;
}
