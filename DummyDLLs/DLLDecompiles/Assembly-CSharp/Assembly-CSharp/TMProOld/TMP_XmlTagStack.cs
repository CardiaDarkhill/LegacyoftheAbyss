using System;

namespace TMProOld
{
	// Token: 0x02000832 RID: 2098
	public struct TMP_XmlTagStack<T>
	{
		// Token: 0x06004AAA RID: 19114 RVA: 0x001623A7 File Offset: 0x001605A7
		public TMP_XmlTagStack(T[] tagStack)
		{
			this.itemStack = tagStack;
			this.index = 0;
		}

		// Token: 0x06004AAB RID: 19115 RVA: 0x001623B7 File Offset: 0x001605B7
		public void Clear()
		{
			this.index = 0;
		}

		// Token: 0x06004AAC RID: 19116 RVA: 0x001623C0 File Offset: 0x001605C0
		public void SetDefault(T item)
		{
			if (this.itemStack == null)
			{
				this.itemStack = new T[16];
			}
			if (this.itemStack != null && this.itemStack.Length != 0)
			{
				this.itemStack[0] = item;
				this.index = 1;
			}
		}

		// Token: 0x06004AAD RID: 19117 RVA: 0x001623FC File Offset: 0x001605FC
		public void Add(T item)
		{
			if (this.index < this.itemStack.Length)
			{
				this.itemStack[this.index] = item;
				this.index++;
			}
		}

		// Token: 0x06004AAE RID: 19118 RVA: 0x0016242E File Offset: 0x0016062E
		public T Remove()
		{
			this.index--;
			if (this.index <= 0)
			{
				this.index = 0;
				return this.itemStack[0];
			}
			return this.itemStack[this.index - 1];
		}

		// Token: 0x06004AAF RID: 19119 RVA: 0x0016246E File Offset: 0x0016066E
		public T CurrentItem()
		{
			if (this.index > 0)
			{
				return this.itemStack[this.index - 1];
			}
			return this.itemStack[0];
		}

		// Token: 0x06004AB0 RID: 19120 RVA: 0x00162499 File Offset: 0x00160699
		public T PreviousItem()
		{
			if (this.index > 1)
			{
				return this.itemStack[this.index - 2];
			}
			return this.itemStack[0];
		}

		// Token: 0x04004A8C RID: 19084
		public T[] itemStack;

		// Token: 0x04004A8D RID: 19085
		public int index;
	}
}
