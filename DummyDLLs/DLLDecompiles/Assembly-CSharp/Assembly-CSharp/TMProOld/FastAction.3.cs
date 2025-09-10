using System;
using System.Collections.Generic;

namespace TMProOld
{
	// Token: 0x020007E0 RID: 2016
	public class FastAction<A, B>
	{
		// Token: 0x060046BB RID: 18107 RVA: 0x0013B5CF File Offset: 0x001397CF
		public void Add(Action<A, B> rhs)
		{
			if (this.lookup.ContainsKey(rhs))
			{
				return;
			}
			this.lookup[rhs] = this.delegates.AddLast(rhs);
		}

		// Token: 0x060046BC RID: 18108 RVA: 0x0013B5F8 File Offset: 0x001397F8
		public void Remove(Action<A, B> rhs)
		{
			LinkedListNode<Action<A, B>> node;
			if (this.lookup.TryGetValue(rhs, out node))
			{
				this.lookup.Remove(rhs);
				this.delegates.Remove(node);
			}
		}

		// Token: 0x060046BD RID: 18109 RVA: 0x0013B630 File Offset: 0x00139830
		public void Call(A a, B b)
		{
			for (LinkedListNode<Action<A, B>> linkedListNode = this.delegates.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value(a, b);
			}
		}

		// Token: 0x0400471A RID: 18202
		private LinkedList<Action<A, B>> delegates = new LinkedList<Action<A, B>>();

		// Token: 0x0400471B RID: 18203
		private Dictionary<Action<A, B>, LinkedListNode<Action<A, B>>> lookup = new Dictionary<Action<A, B>, LinkedListNode<Action<A, B>>>();
	}
}
