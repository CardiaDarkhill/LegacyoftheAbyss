using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B42 RID: 2882
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Edit a key from a PlayMaker HashTable Proxy component (PlayMakerHashTableProxy)")]
	public class HashTableEditKey : HashTableActions
	{
		// Token: 0x060059FE RID: 23038 RVA: 0x001C79A1 File Offset: 0x001C5BA1
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.key = null;
			this.newKey = null;
			this.keyNotFoundEvent = null;
			this.newKeyExistsAlreadyEvent = null;
		}

		// Token: 0x060059FF RID: 23039 RVA: 0x001C79CD File Offset: 0x001C5BCD
		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.EditHashTableKey();
			}
			base.Finish();
		}

		// Token: 0x06005A00 RID: 23040 RVA: 0x001C7A00 File Offset: 0x001C5C00
		public void EditHashTableKey()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			if (!this.proxy.hashTable.ContainsKey(this.key.Value))
			{
				base.Fsm.Event(this.keyNotFoundEvent);
				return;
			}
			if (this.proxy.hashTable.ContainsKey(this.newKey.Value))
			{
				base.Fsm.Event(this.newKeyExistsAlreadyEvent);
				return;
			}
			object value = this.proxy.hashTable[this.key.Value];
			this.proxy.hashTable[this.newKey.Value] = value;
			this.proxy.hashTable.Remove(this.key.Value);
		}

		// Token: 0x0400557F RID: 21887
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005580 RID: 21888
		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x04005581 RID: 21889
		[RequiredField]
		[UIHint(UIHint.FsmString)]
		[Tooltip("The Key value to edit")]
		public FsmString key;

		// Token: 0x04005582 RID: 21890
		[RequiredField]
		[UIHint(UIHint.FsmString)]
		[Tooltip("The Key value to edit")]
		public FsmString newKey;

		// Token: 0x04005583 RID: 21891
		[ActionSection("Result")]
		[Tooltip("Event sent if this HashTable key does not exists")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent keyNotFoundEvent;

		// Token: 0x04005584 RID: 21892
		[Tooltip("Event sent if this HashTable already contains the new key")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent newKeyExistsAlreadyEvent;
	}
}
