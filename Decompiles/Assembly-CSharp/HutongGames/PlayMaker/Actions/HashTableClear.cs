using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B3C RID: 2876
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Remove all content of a PlayMaker hashtable Proxy component")]
	public class HashTableClear : HashTableActions
	{
		// Token: 0x060059E6 RID: 23014 RVA: 0x001C74E1 File Offset: 0x001C56E1
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
		}

		// Token: 0x060059E7 RID: 23015 RVA: 0x001C74F1 File Offset: 0x001C56F1
		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.ClearHashTable();
			}
			base.Finish();
		}

		// Token: 0x060059E8 RID: 23016 RVA: 0x001C7523 File Offset: 0x001C5723
		public void ClearHashTable()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.hashTable.Clear();
		}

		// Token: 0x04005563 RID: 21859
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005564 RID: 21860
		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;
	}
}
