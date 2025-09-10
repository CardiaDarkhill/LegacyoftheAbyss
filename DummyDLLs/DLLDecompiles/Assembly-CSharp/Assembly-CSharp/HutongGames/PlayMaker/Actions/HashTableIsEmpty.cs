using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B48 RID: 2888
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Check if an HashTable Proxy component is empty.")]
	public class HashTableIsEmpty : ArrayListActions
	{
		// Token: 0x06005A16 RID: 23062 RVA: 0x001C80A0 File Offset: 0x001C62A0
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.isEmpty = null;
			this.isEmptyEvent = null;
			this.isNotEmptyEvent = null;
		}

		// Token: 0x06005A17 RID: 23063 RVA: 0x001C80C8 File Offset: 0x001C62C8
		public override void OnEnter()
		{
			bool flag = base.GetHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value, true).hashTable.Count == 0;
			this.isEmpty.Value = flag;
			if (flag)
			{
				base.Fsm.Event(this.isEmptyEvent);
			}
			else
			{
				base.Fsm.Event(this.isNotEmptyEvent);
			}
			base.Finish();
		}

		// Token: 0x040055A6 RID: 21926
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040055A7 RID: 21927
		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x040055A8 RID: 21928
		[ActionSection("Result")]
		[Tooltip("Store in a bool wether it is empty or not")]
		[UIHint(UIHint.Variable)]
		public FsmBool isEmpty;

		// Token: 0x040055A9 RID: 21929
		[Tooltip("Event sent if this HashTable is empty ")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent isEmptyEvent;

		// Token: 0x040055AA RID: 21930
		[Tooltip("Event sent if this HashTable is not empty")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent isNotEmptyEvent;
	}
}
