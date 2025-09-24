using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B3B RID: 2875
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Add key/value pairs to a PlayMaker HashTable Proxy component (PlayMakerHashTableProxy).")]
	public class HashTableAddMany : HashTableActions
	{
		// Token: 0x060059E2 RID: 23010 RVA: 0x001C73B1 File Offset: 0x001C55B1
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.keys = null;
			this.variables = null;
			this.successEvent = null;
			this.keyExistsAlreadyEvent = null;
		}

		// Token: 0x060059E3 RID: 23011 RVA: 0x001C73E0 File Offset: 0x001C55E0
		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				if (this.keyExistsAlreadyEvent != null)
				{
					foreach (FsmString fsmString in this.keys)
					{
						if (this.proxy.hashTable.ContainsKey(fsmString.Value))
						{
							base.Fsm.Event(this.keyExistsAlreadyEvent);
							base.Finish();
						}
					}
				}
				this.AddToHashTable();
				base.Fsm.Event(this.successEvent);
			}
			base.Finish();
		}

		// Token: 0x060059E4 RID: 23012 RVA: 0x001C7480 File Offset: 0x001C5680
		public void AddToHashTable()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			for (int i = 0; i < this.keys.Length; i++)
			{
				this.proxy.hashTable.Add(this.keys[i].Value, PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.variables[i]));
			}
		}

		// Token: 0x0400555D RID: 21853
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400555E RID: 21854
		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x0400555F RID: 21855
		[ActionSection("Data")]
		[CompoundArray("Count", "Key", "Value")]
		[RequiredField]
		[UIHint(UIHint.FsmString)]
		[Tooltip("The Key")]
		public FsmString[] keys;

		// Token: 0x04005560 RID: 21856
		[RequiredField]
		[Tooltip("The value for that key")]
		public FsmVar[] variables;

		// Token: 0x04005561 RID: 21857
		[ActionSection("Result")]
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger when elements are added")]
		public FsmEvent successEvent;

		// Token: 0x04005562 RID: 21858
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger when elements exists already")]
		public FsmEvent keyExistsAlreadyEvent;
	}
}
