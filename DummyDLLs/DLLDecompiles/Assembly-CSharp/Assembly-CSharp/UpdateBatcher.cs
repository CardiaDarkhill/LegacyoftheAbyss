using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200079A RID: 1946
public class UpdateBatcher : MonoBehaviour
{
	// Token: 0x060044B5 RID: 17589 RVA: 0x0012CB8C File Offset: 0x0012AD8C
	private void Update()
	{
		List<IUpdateBatchableUpdate> list = this.updates.List;
		for (int i = list.Count - 1; i >= 0; i--)
		{
			IUpdateBatchableUpdate updateBatchableUpdate = list[i];
			try
			{
				if (updateBatchableUpdate.ShouldUpdate)
				{
					updateBatchableUpdate.BatchedUpdate();
				}
			}
			catch (Exception)
			{
			}
		}
	}

	// Token: 0x060044B6 RID: 17590 RVA: 0x0012CBE8 File Offset: 0x0012ADE8
	private void LateUpdate()
	{
		List<IUpdateBatchableLateUpdate> list = this.lateUpdates.List;
		for (int i = list.Count - 1; i >= 0; i--)
		{
			IUpdateBatchableLateUpdate updateBatchableLateUpdate = list[i];
			try
			{
				if (updateBatchableLateUpdate.ShouldUpdate)
				{
					updateBatchableLateUpdate.BatchedLateUpdate();
				}
			}
			catch (Exception)
			{
			}
		}
	}

	// Token: 0x060044B7 RID: 17591 RVA: 0x0012CC44 File Offset: 0x0012AE44
	private void FixedUpdate()
	{
		List<IUpdateBatchableFixedUpdate> list = this.fixedUpdates.List;
		for (int i = list.Count - 1; i >= 0; i--)
		{
			IUpdateBatchableFixedUpdate updateBatchableFixedUpdate = list[i];
			try
			{
				if (updateBatchableFixedUpdate.ShouldUpdate)
				{
					updateBatchableFixedUpdate.BatchedFixedUpdate();
				}
			}
			catch (Exception)
			{
			}
		}
	}

	// Token: 0x060044B8 RID: 17592 RVA: 0x0012CCA0 File Offset: 0x0012AEA0
	public void Add(MonoBehaviour behaviour)
	{
		this.lateUpdates.Add(behaviour);
		this.updates.Add(behaviour);
		this.fixedUpdates.Add(behaviour);
	}

	// Token: 0x060044B9 RID: 17593 RVA: 0x0012CCC8 File Offset: 0x0012AEC8
	public bool Remove(MonoBehaviour behaviour)
	{
		bool result = this.lateUpdates.Remove(behaviour);
		if (this.updates.Remove(behaviour))
		{
			result = true;
		}
		if (this.fixedUpdates.Remove(behaviour))
		{
			result = true;
		}
		return result;
	}

	// Token: 0x040045A7 RID: 17831
	private readonly UpdateBatcher.UpdateList<IUpdateBatchableLateUpdate> lateUpdates = new UpdateBatcher.UpdateList<IUpdateBatchableLateUpdate>();

	// Token: 0x040045A8 RID: 17832
	private readonly UpdateBatcher.UpdateList<IUpdateBatchableUpdate> updates = new UpdateBatcher.UpdateList<IUpdateBatchableUpdate>();

	// Token: 0x040045A9 RID: 17833
	private readonly UpdateBatcher.UpdateList<IUpdateBatchableFixedUpdate> fixedUpdates = new UpdateBatcher.UpdateList<IUpdateBatchableFixedUpdate>();

	// Token: 0x02001A74 RID: 6772
	private sealed class UpdateList<T>
	{
		// Token: 0x17001123 RID: 4387
		// (get) Token: 0x060096F5 RID: 38645 RVA: 0x002A9274 File Offset: 0x002A7474
		public List<T> List
		{
			get
			{
				this.Update();
				return this.list;
			}
		}

		// Token: 0x060096F6 RID: 38646 RVA: 0x002A9282 File Offset: 0x002A7482
		private void Update()
		{
			if (!this.dirty)
			{
				return;
			}
			this.dirty = false;
			this.List.Clear();
			this.List.AddRange(this.hashset);
		}

		// Token: 0x060096F7 RID: 38647 RVA: 0x002A92B0 File Offset: 0x002A74B0
		public void Add(MonoBehaviour monoBehaviour)
		{
			if (monoBehaviour is T)
			{
				T item = monoBehaviour as T;
				if (this.hashset.Add(item))
				{
					this.dirty = true;
				}
				return;
			}
		}

		// Token: 0x060096F8 RID: 38648 RVA: 0x002A92EC File Offset: 0x002A74EC
		public bool Remove(MonoBehaviour monoBehaviour)
		{
			if (!(monoBehaviour is T))
			{
				return false;
			}
			T item = monoBehaviour as T;
			if (!this.hashset.Remove(item))
			{
				return false;
			}
			this.dirty = true;
			return true;
		}

		// Token: 0x04009984 RID: 39300
		private readonly HashSet<T> hashset = new HashSet<T>();

		// Token: 0x04009985 RID: 39301
		private readonly List<T> list = new List<T>();

		// Token: 0x04009986 RID: 39302
		private bool dirty;
	}
}
