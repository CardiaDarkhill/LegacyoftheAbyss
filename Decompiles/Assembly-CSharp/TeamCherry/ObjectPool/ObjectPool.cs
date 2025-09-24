using System;
using System.Collections.Generic;
using UnityEngine;

namespace TeamCherry.ObjectPool
{
	// Token: 0x020008C1 RID: 2241
	public class ObjectPool<T> : IDisposable, IPoolReleaser<T> where T : Object, IPoolable<T>
	{
		// Token: 0x1700091F RID: 2335
		// (get) Token: 0x06004D8C RID: 19852 RVA: 0x0016C31A File Offset: 0x0016A51A
		protected int TotalCount
		{
			get
			{
				return this.totalCount;
			}
		}

		// Token: 0x06004D8D RID: 19853 RVA: 0x0016C322 File Offset: 0x0016A522
		protected ObjectPool()
		{
			this.inactiveStack = new Stack<T>();
		}

		// Token: 0x06004D8E RID: 19854 RVA: 0x0016C335 File Offset: 0x0016A535
		public ObjectPool(ObjectPool<T>.ObjectReturnDelegate createNewAction, ObjectPool<T>.ObjectPassDelegate onGet, ObjectPool<T>.ObjectPassDelegate onRelease, ObjectPool<T>.ObjectPassDelegate onDestroy, int startCapacity = 10) : this()
		{
			this.factory = createNewAction;
			this.onGet = onGet;
			this.onRelease = onRelease;
			this.onDestroy = onDestroy;
			this.InitialisePool(startCapacity);
		}

		// Token: 0x06004D8F RID: 19855 RVA: 0x0016C364 File Offset: 0x0016A564
		protected void InitialisePool(int startCapacity)
		{
			for (int i = this.totalCount; i < startCapacity; i++)
			{
				T element = this.CreateNew();
				this.AddNew(element);
			}
		}

		// Token: 0x06004D90 RID: 19856 RVA: 0x0016C390 File Offset: 0x0016A590
		protected void AddNew(T element)
		{
			this.totalCount++;
			this.Release(element);
		}

		// Token: 0x06004D91 RID: 19857 RVA: 0x0016C3A7 File Offset: 0x0016A5A7
		protected T CreateNew()
		{
			return this.factory();
		}

		// Token: 0x06004D92 RID: 19858 RVA: 0x0016C3B4 File Offset: 0x0016A5B4
		public T Get()
		{
			T t;
			if (this.inactiveStack.Count < 1)
			{
				t = this.CreateNew();
				this.totalCount++;
			}
			else
			{
				t = this.inactiveStack.Pop();
			}
			ObjectPool<T>.ObjectPassDelegate objectPassDelegate = this.onGet;
			if (objectPassDelegate != null)
			{
				objectPassDelegate(t);
			}
			return t;
		}

		// Token: 0x06004D93 RID: 19859 RVA: 0x0016C405 File Offset: 0x0016A605
		public void Release(T element)
		{
			ObjectPool<T>.ObjectPassDelegate objectPassDelegate = this.onRelease;
			if (objectPassDelegate != null)
			{
				objectPassDelegate(element);
			}
			this.inactiveStack.Push(element);
		}

		// Token: 0x06004D94 RID: 19860 RVA: 0x0016C428 File Offset: 0x0016A628
		public void Clear()
		{
			if (this.onDestroy != null)
			{
				foreach (T element in this.inactiveStack)
				{
					this.onDestroy(element);
				}
			}
			this.inactiveStack.Clear();
			this.totalCount = 0;
		}

		// Token: 0x06004D95 RID: 19861 RVA: 0x0016C49C File Offset: 0x0016A69C
		public void Dispose()
		{
			this.Clear();
		}

		// Token: 0x04004E4D RID: 20045
		protected ObjectPool<T>.ObjectReturnDelegate factory;

		// Token: 0x04004E4E RID: 20046
		protected ObjectPool<T>.ObjectPassDelegate onGet;

		// Token: 0x04004E4F RID: 20047
		protected ObjectPool<T>.ObjectPassDelegate onRelease;

		// Token: 0x04004E50 RID: 20048
		protected ObjectPool<T>.ObjectPassDelegate onDestroy;

		// Token: 0x04004E51 RID: 20049
		private int totalCount;

		// Token: 0x04004E52 RID: 20050
		private readonly Stack<T> inactiveStack;

		// Token: 0x02001B4D RID: 6989
		// (Invoke) Token: 0x060099B0 RID: 39344
		public delegate T ObjectReturnDelegate();

		// Token: 0x02001B4E RID: 6990
		// (Invoke) Token: 0x060099B4 RID: 39348
		public delegate void ObjectPassDelegate(T element);
	}
}
