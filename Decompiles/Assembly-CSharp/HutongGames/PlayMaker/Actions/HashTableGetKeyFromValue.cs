using System;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B45 RID: 2885
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Return the key for a value ofna PlayMaker hashtable Proxy component. It will return the first entry found.")]
	public class HashTableGetKeyFromValue : HashTableActions
	{
		// Token: 0x06005A09 RID: 23049 RVA: 0x001C7C4F File Offset: 0x001C5E4F
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
		}

		// Token: 0x06005A0A RID: 23050 RVA: 0x001C7C5F File Offset: 0x001C5E5F
		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.SortHashTableByValues();
			}
			base.Finish();
		}

		// Token: 0x06005A0B RID: 23051 RVA: 0x001C7C94 File Offset: 0x001C5E94
		public void SortHashTableByValues()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			object valueFromFsmVar = PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.theValue);
			foreach (object obj in this.proxy.hashTable)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				if (dictionaryEntry.Value.Equals(valueFromFsmVar))
				{
					this.result.Value = (string)dictionaryEntry.Key;
					base.Fsm.Event(this.KeyFoundEvent);
					return;
				}
			}
			base.Fsm.Event(this.KeyNotFoundEvent);
		}

		// Token: 0x04005590 RID: 21904
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005591 RID: 21905
		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x04005592 RID: 21906
		[ActionSection("Value")]
		[RequiredField]
		[Tooltip("The value to search")]
		public FsmVar theValue;

		// Token: 0x04005593 RID: 21907
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The key of that value")]
		public FsmString result;

		// Token: 0x04005594 RID: 21908
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger when value is found")]
		public FsmEvent KeyFoundEvent;

		// Token: 0x04005595 RID: 21909
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger when value is not found")]
		public FsmEvent KeyNotFoundEvent;
	}
}
