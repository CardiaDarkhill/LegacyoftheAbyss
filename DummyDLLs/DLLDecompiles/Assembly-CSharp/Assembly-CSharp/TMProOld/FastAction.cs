using System;
using System.Collections.Generic;

namespace TMProOld
{
	// Token: 0x020007DE RID: 2014
	public class FastAction
	{
		// Token: 0x060046B3 RID: 18099 RVA: 0x0013B46E File Offset: 0x0013966E
		public void Add(Action rhs)
		{
			if (this.lookup.ContainsKey(rhs))
			{
				return;
			}
			this.lookup[rhs] = this.delegates.AddLast(rhs);
		}

		// Token: 0x060046B4 RID: 18100 RVA: 0x0013B498 File Offset: 0x00139698
		public void Remove(Action rhs)
		{
			LinkedListNode<Action> node;
			if (this.lookup.TryGetValue(rhs, out node))
			{
				this.lookup.Remove(rhs);
				this.delegates.Remove(node);
			}
		}

		// Token: 0x060046B5 RID: 18101 RVA: 0x0013B4D0 File Offset: 0x001396D0
		public void Call()
		{
			for (LinkedListNode<Action> linkedListNode = this.delegates.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value();
			}
		}

		// Token: 0x04004716 RID: 18198
		private LinkedList<Action> delegates = new LinkedList<Action>();

		// Token: 0x04004717 RID: 18199
		private Dictionary<Action, LinkedListNode<Action>> lookup = new Dictionary<Action, LinkedListNode<Action>>();
	}
}
