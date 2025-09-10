using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B3A RID: 2874
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Add an key/value pair to a PlayMaker HashTable Proxy component (PlayMakerHashTableProxy).")]
	public class HashTableAdd : HashTableActions
	{
		// Token: 0x060059DE RID: 23006 RVA: 0x001C72C8 File Offset: 0x001C54C8
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.key = null;
			this.variable = null;
			this.successEvent = null;
			this.keyExistsAlreadyEvent = null;
		}

		// Token: 0x060059DF RID: 23007 RVA: 0x001C72F4 File Offset: 0x001C54F4
		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				if (this.proxy.hashTable.ContainsKey(this.key.Value))
				{
					base.Fsm.Event(this.keyExistsAlreadyEvent);
				}
				else
				{
					this.AddToHashTable();
					base.Fsm.Event(this.successEvent);
				}
			}
			base.Finish();
		}

		// Token: 0x060059E0 RID: 23008 RVA: 0x001C7372 File Offset: 0x001C5572
		public void AddToHashTable()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.hashTable.Add(this.key.Value, PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.variable));
		}

		// Token: 0x04005557 RID: 21847
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005558 RID: 21848
		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x04005559 RID: 21849
		[ActionSection("Data")]
		[RequiredField]
		[UIHint(UIHint.FsmString)]
		[Tooltip("The Key value for that hash set")]
		public FsmString key;

		// Token: 0x0400555A RID: 21850
		[RequiredField]
		[Tooltip("The variable to add.")]
		public FsmVar variable;

		// Token: 0x0400555B RID: 21851
		[ActionSection("Result")]
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger when element is added")]
		public FsmEvent successEvent;

		// Token: 0x0400555C RID: 21852
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger when element exists already")]
		public FsmEvent keyExistsAlreadyEvent;
	}
}
