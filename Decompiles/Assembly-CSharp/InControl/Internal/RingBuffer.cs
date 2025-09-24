using System;

namespace InControl.Internal
{
	// Token: 0x0200094B RID: 2379
	public class RingBuffer<T>
	{
		// Token: 0x060054D8 RID: 21720 RVA: 0x00182450 File Offset: 0x00180650
		public RingBuffer(int size)
		{
			if (size <= 0)
			{
				throw new ArgumentException("RingBuffer size must be 1 or greater.");
			}
			this.size = size + 1;
			this.data = new T[this.size];
			this.head = 0;
			this.tail = 0;
			this.sync = new object();
		}

		// Token: 0x060054D9 RID: 21721 RVA: 0x001824A8 File Offset: 0x001806A8
		public void Enqueue(T value)
		{
			object obj = this.sync;
			lock (obj)
			{
				if (this.size > 1)
				{
					this.head = (this.head + 1) % this.size;
					if (this.head == this.tail)
					{
						this.tail = (this.tail + 1) % this.size;
					}
				}
				this.data[this.head] = value;
			}
		}

		// Token: 0x060054DA RID: 21722 RVA: 0x00182538 File Offset: 0x00180738
		public T Dequeue()
		{
			object obj = this.sync;
			T result;
			lock (obj)
			{
				if (this.size > 1 && this.tail != this.head)
				{
					this.tail = (this.tail + 1) % this.size;
				}
				result = this.data[this.tail];
			}
			return result;
		}

		// Token: 0x040053D0 RID: 21456
		private readonly int size;

		// Token: 0x040053D1 RID: 21457
		private readonly T[] data;

		// Token: 0x040053D2 RID: 21458
		private int head;

		// Token: 0x040053D3 RID: 21459
		private int tail;

		// Token: 0x040053D4 RID: 21460
		private readonly object sync;
	}
}
