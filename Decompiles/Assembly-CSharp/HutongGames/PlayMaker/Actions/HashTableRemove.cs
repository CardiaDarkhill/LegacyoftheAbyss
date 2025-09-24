using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B4A RID: 2890
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Remove an item by key ( key/value pairs) in a PlayMaker HashTable Proxy component (PlayMakerHashTableProxy).")]
	public class HashTableRemove : HashTableActions
	{
		// Token: 0x06005A1D RID: 23069 RVA: 0x001C820A File Offset: 0x001C640A
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.key = null;
		}

		// Token: 0x06005A1E RID: 23070 RVA: 0x001C8221 File Offset: 0x001C6421
		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.doHashTableRemove();
			}
			base.Finish();
		}

		// Token: 0x06005A1F RID: 23071 RVA: 0x001C8253 File Offset: 0x001C6453
		public void doHashTableRemove()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.hashTable.Remove(this.key.Value);
		}

		// Token: 0x040055AF RID: 21935
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040055B0 RID: 21936
		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x040055B1 RID: 21937
		[RequiredField]
		[Tooltip("The item key in that hashTable")]
		public FsmString key;
	}
}
