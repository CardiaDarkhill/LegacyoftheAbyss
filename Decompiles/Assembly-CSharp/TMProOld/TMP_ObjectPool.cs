using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TMProOld
{
	// Token: 0x02000816 RID: 2070
	internal class TMP_ObjectPool<T> where T : new()
	{
		// Token: 0x17000863 RID: 2147
		// (get) Token: 0x0600490B RID: 18699 RVA: 0x00156072 File Offset: 0x00154272
		// (set) Token: 0x0600490C RID: 18700 RVA: 0x0015607A File Offset: 0x0015427A
		public int countAll { get; private set; }

		// Token: 0x17000864 RID: 2148
		// (get) Token: 0x0600490D RID: 18701 RVA: 0x00156083 File Offset: 0x00154283
		public int countActive
		{
			get
			{
				return this.countAll - this.countInactive;
			}
		}

		// Token: 0x17000865 RID: 2149
		// (get) Token: 0x0600490E RID: 18702 RVA: 0x00156092 File Offset: 0x00154292
		public int countInactive
		{
			get
			{
				return this.m_Stack.Count;
			}
		}

		// Token: 0x0600490F RID: 18703 RVA: 0x0015609F File Offset: 0x0015429F
		public TMP_ObjectPool(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease)
		{
			this.m_ActionOnGet = actionOnGet;
			this.m_ActionOnRelease = actionOnRelease;
		}

		// Token: 0x06004910 RID: 18704 RVA: 0x001560C0 File Offset: 0x001542C0
		public T Get()
		{
			T t;
			if (this.m_Stack.Count == 0)
			{
				t = Activator.CreateInstance<T>();
				int countAll = this.countAll;
				this.countAll = countAll + 1;
			}
			else
			{
				t = this.m_Stack.Pop();
			}
			if (this.m_ActionOnGet != null)
			{
				this.m_ActionOnGet(t);
			}
			return t;
		}

		// Token: 0x06004911 RID: 18705 RVA: 0x00156114 File Offset: 0x00154314
		public void Release(T element)
		{
			if (this.m_Stack.Count > 0 && this.m_Stack.Peek() == element)
			{
				Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
			}
			if (this.m_ActionOnRelease != null)
			{
				this.m_ActionOnRelease(element);
			}
			this.m_Stack.Push(element);
		}

		// Token: 0x04004916 RID: 18710
		private readonly Stack<T> m_Stack = new Stack<T>();

		// Token: 0x04004917 RID: 18711
		private readonly UnityAction<T> m_ActionOnGet;

		// Token: 0x04004918 RID: 18712
		private readonly UnityAction<T> m_ActionOnRelease;
	}
}
