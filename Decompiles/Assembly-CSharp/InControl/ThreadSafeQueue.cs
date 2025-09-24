using System;
using System.Collections.Generic;

namespace InControl
{
	// Token: 0x02000943 RID: 2371
	internal class ThreadSafeQueue<T>
	{
		// Token: 0x0600544E RID: 21582 RVA: 0x001800EC File Offset: 0x0017E2EC
		public ThreadSafeQueue()
		{
			this.sync = new object();
			this.data = new Queue<T>();
		}

		// Token: 0x0600544F RID: 21583 RVA: 0x0018010A File Offset: 0x0017E30A
		public ThreadSafeQueue(int capacity)
		{
			this.sync = new object();
			this.data = new Queue<T>(capacity);
		}

		// Token: 0x06005450 RID: 21584 RVA: 0x0018012C File Offset: 0x0017E32C
		public void Enqueue(T item)
		{
			object obj = this.sync;
			lock (obj)
			{
				this.data.Enqueue(item);
			}
		}

		// Token: 0x06005451 RID: 21585 RVA: 0x00180174 File Offset: 0x0017E374
		public bool Dequeue(out T item)
		{
			object obj = this.sync;
			lock (obj)
			{
				if (this.data.Count > 0)
				{
					item = this.data.Dequeue();
					return true;
				}
			}
			item = default(T);
			return false;
		}

		// Token: 0x06005452 RID: 21586 RVA: 0x001801DC File Offset: 0x0017E3DC
		public T Dequeue()
		{
			object obj = this.sync;
			lock (obj)
			{
				if (this.data.Count > 0)
				{
					return this.data.Dequeue();
				}
			}
			return default(T);
		}

		// Token: 0x06005453 RID: 21587 RVA: 0x00180240 File Offset: 0x0017E440
		public int Dequeue(ref IList<T> list)
		{
			object obj = this.sync;
			int result;
			lock (obj)
			{
				int count = this.data.Count;
				for (int i = 0; i < count; i++)
				{
					list.Add(this.data.Dequeue());
				}
				result = count;
			}
			return result;
		}

		// Token: 0x040053A3 RID: 21411
		private object sync;

		// Token: 0x040053A4 RID: 21412
		private Queue<T> data;
	}
}
