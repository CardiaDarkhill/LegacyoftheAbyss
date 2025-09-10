using System;
using System.Collections.Generic;

namespace TMProOld
{
	// Token: 0x020007E1 RID: 2017
	public class FastAction<A, B, C>
	{
		// Token: 0x060046BF RID: 18111 RVA: 0x0013B680 File Offset: 0x00139880
		public void Add(Action<A, B, C> rhs)
		{
			if (this.lookup.ContainsKey(rhs))
			{
				return;
			}
			this.lookup[rhs] = this.delegates.AddLast(rhs);
		}

		// Token: 0x060046C0 RID: 18112 RVA: 0x0013B6AC File Offset: 0x001398AC
		public void Remove(Action<A, B, C> rhs)
		{
			LinkedListNode<Action<A, B, C>> node;
			if (this.lookup.TryGetValue(rhs, out node))
			{
				this.lookup.Remove(rhs);
				this.delegates.Remove(node);
			}
		}

		// Token: 0x060046C1 RID: 18113 RVA: 0x0013B6E4 File Offset: 0x001398E4
		public void Call(A a, B b, C c)
		{
			for (LinkedListNode<Action<A, B, C>> linkedListNode = this.delegates.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value(a, b, c);
			}
		}

		// Token: 0x0400471C RID: 18204
		private LinkedList<Action<A, B, C>> delegates = new LinkedList<Action<A, B, C>>();

		// Token: 0x0400471D RID: 18205
		private Dictionary<Action<A, B, C>, LinkedListNode<Action<A, B, C>>> lookup = new Dictionary<Action<A, B, C>, LinkedListNode<Action<A, B, C>>>();
	}
}
