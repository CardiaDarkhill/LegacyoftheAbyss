using System;
using System.Collections.Generic;

// Token: 0x02000796 RID: 1942
public sealed class UniqueList<T>
{
	// Token: 0x170007AD RID: 1965
	// (get) Token: 0x060044A5 RID: 17573 RVA: 0x0012C9F5 File Offset: 0x0012ABF5
	public List<T> List
	{
		get
		{
			this.UpdateList();
			return this.list;
		}
	}

	// Token: 0x170007AE RID: 1966
	// (get) Token: 0x060044A6 RID: 17574 RVA: 0x0012CA03 File Offset: 0x0012AC03
	public int Count
	{
		get
		{
			return this.hashSet.Count;
		}
	}

	// Token: 0x060044A7 RID: 17575 RVA: 0x0012CA10 File Offset: 0x0012AC10
	public void ReserveListUsage()
	{
		this.reservationCount++;
	}

	// Token: 0x060044A8 RID: 17576 RVA: 0x0012CA20 File Offset: 0x0012AC20
	public void ReleaseListUsage()
	{
		this.reservationCount--;
		if (this.reservationCount <= 0)
		{
			this.reservationCount = 0;
			if (this.pendingClear)
			{
				this.list.Clear();
				this.dirty = false;
				this.pendingClear = false;
			}
		}
	}

	// Token: 0x060044A9 RID: 17577 RVA: 0x0012CA6C File Offset: 0x0012AC6C
	public void UpdateList()
	{
		if (this.dirty)
		{
			this.dirty = false;
			this.list.Clear();
			this.list.AddRange(this.hashSet);
		}
	}

	// Token: 0x060044AA RID: 17578 RVA: 0x0012CA99 File Offset: 0x0012AC99
	public bool Add(T element)
	{
		if (this.hashSet.Add(element))
		{
			this.dirty = true;
			return true;
		}
		return false;
	}

	// Token: 0x060044AB RID: 17579 RVA: 0x0012CAB4 File Offset: 0x0012ACB4
	public bool Remove(T element)
	{
		if (this.hashSet.Remove(element))
		{
			this.dirty = true;
			if (this.hashSet.Count == 0)
			{
				if (this.reservationCount == 0)
				{
					this.list.Clear();
					this.dirty = false;
				}
				else
				{
					this.pendingClear = true;
				}
			}
			return true;
		}
		return false;
	}

	// Token: 0x060044AC RID: 17580 RVA: 0x0012CB09 File Offset: 0x0012AD09
	public void Clear()
	{
		this.hashSet.Clear();
		if (this.reservationCount == 0)
		{
			this.list.Clear();
			this.dirty = false;
			return;
		}
		this.dirty = true;
		this.pendingClear = true;
	}

	// Token: 0x060044AD RID: 17581 RVA: 0x0012CB3F File Offset: 0x0012AD3F
	public void FullClear()
	{
		this.hashSet.Clear();
		this.list.Clear();
		this.dirty = false;
		this.pendingClear = false;
		this.reservationCount = 0;
	}

	// Token: 0x040045A2 RID: 17826
	private readonly HashSet<T> hashSet = new HashSet<T>();

	// Token: 0x040045A3 RID: 17827
	private readonly List<T> list = new List<T>();

	// Token: 0x040045A4 RID: 17828
	private bool dirty;

	// Token: 0x040045A5 RID: 17829
	private bool pendingClear;

	// Token: 0x040045A6 RID: 17830
	private int reservationCount;
}
