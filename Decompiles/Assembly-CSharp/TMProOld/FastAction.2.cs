using System;
using System.Collections.Generic;

namespace TMProOld
{
	// Token: 0x020007DF RID: 2015
	public class FastAction<A>
	{
		// Token: 0x060046B7 RID: 18103 RVA: 0x0013B51E File Offset: 0x0013971E
		public void Add(Action<A> rhs)
		{
			if (this.lookup.ContainsKey(rhs))
			{
				return;
			}
			this.lookup[rhs] = this.delegates.AddLast(rhs);
		}

		// Token: 0x060046B8 RID: 18104 RVA: 0x0013B548 File Offset: 0x00139748
		public void Remove(Action<A> rhs)
		{
			LinkedListNode<Action<A>> node;
			if (this.lookup.TryGetValue(rhs, out node))
			{
				this.lookup.Remove(rhs);
				this.delegates.Remove(node);
			}
		}

		// Token: 0x060046B9 RID: 18105 RVA: 0x0013B580 File Offset: 0x00139780
		public void Call(A a)
		{
			for (LinkedListNode<Action<A>> linkedListNode = this.delegates.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value(a);
			}
		}

		// Token: 0x04004718 RID: 18200
		private LinkedList<Action<A>> delegates = new LinkedList<Action<A>>();

		// Token: 0x04004719 RID: 18201
		private Dictionary<Action<A>, LinkedListNode<Action<A>>> lookup = new Dictionary<Action<A>, LinkedListNode<Action<A>>>();
	}
}
