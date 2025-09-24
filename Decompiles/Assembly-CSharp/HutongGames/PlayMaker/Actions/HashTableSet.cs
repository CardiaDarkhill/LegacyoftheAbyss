using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B4C RID: 2892
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Set an key/value pair to a PlayMaker HashTable Proxy component (PlayMakerHashTableProxy)")]
	public class HashTableSet : HashTableActions
	{
		// Token: 0x06005A25 RID: 23077 RVA: 0x001C82E1 File Offset: 0x001C64E1
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.key = null;
			this.variable = null;
		}

		// Token: 0x06005A26 RID: 23078 RVA: 0x001C82FF File Offset: 0x001C64FF
		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.SetHashTable();
			}
			base.Finish();
		}

		// Token: 0x06005A27 RID: 23079 RVA: 0x001C8331 File Offset: 0x001C6531
		public void SetHashTable()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.hashTable[this.key.Value] = PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.variable);
		}

		// Token: 0x040055B4 RID: 21940
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040055B5 RID: 21941
		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x040055B6 RID: 21942
		[RequiredField]
		[UIHint(UIHint.FsmString)]
		[Tooltip("The Key value for that hash set")]
		public FsmString key;

		// Token: 0x040055B7 RID: 21943
		[ActionSection("Result")]
		[Tooltip("The variable to set.")]
		public FsmVar variable;
	}
}
