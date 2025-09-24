using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B3F RID: 2879
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Check if a key exists in a PlayMaker HashTable Proxy component (PlayMakerHashTablePRoxy)")]
	public class HashTableContainsKey : HashTableActions
	{
		// Token: 0x060059F2 RID: 23026 RVA: 0x001C777C File Offset: 0x001C597C
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.key = null;
			this.containsKey = null;
			this.keyFoundEvent = null;
			this.keyNotFoundEvent = null;
		}

		// Token: 0x060059F3 RID: 23027 RVA: 0x001C77A8 File Offset: 0x001C59A8
		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.checkIfContainsKey();
			}
			base.Finish();
		}

		// Token: 0x060059F4 RID: 23028 RVA: 0x001C77DC File Offset: 0x001C59DC
		public void checkIfContainsKey()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.containsKey.Value = this.proxy.hashTable.ContainsKey(this.key.Value);
			if (this.containsKey.Value)
			{
				base.Fsm.Event(this.keyFoundEvent);
				return;
			}
			base.Fsm.Event(this.keyNotFoundEvent);
		}

		// Token: 0x04005570 RID: 21872
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005571 RID: 21873
		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x04005572 RID: 21874
		[RequiredField]
		[UIHint(UIHint.FsmString)]
		[Tooltip("The Key value to check for")]
		public FsmString key;

		// Token: 0x04005573 RID: 21875
		[UIHint(UIHint.FsmBool)]
		[Tooltip("Store the result of the test")]
		public FsmBool containsKey;

		// Token: 0x04005574 RID: 21876
		[ActionSection("Result")]
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger when key is found")]
		public FsmEvent keyFoundEvent;

		// Token: 0x04005575 RID: 21877
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger when key is not found")]
		public FsmEvent keyNotFoundEvent;
	}
}
