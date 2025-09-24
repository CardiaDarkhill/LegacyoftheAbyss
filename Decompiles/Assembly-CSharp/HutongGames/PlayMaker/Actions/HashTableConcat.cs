using System;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B3D RID: 2877
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Concat joins two or more hashTable proxy components. if a target is specified, the method use the target store the concatenation, else the ")]
	public class HashTableConcat : HashTableActions
	{
		// Token: 0x060059EA RID: 23018 RVA: 0x001C7546 File Offset: 0x001C5746
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.hashTableGameObjectTargets = null;
			this.referenceTargets = null;
			this.overwriteExistingKey = null;
		}

		// Token: 0x060059EB RID: 23019 RVA: 0x001C756B File Offset: 0x001C576B
		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.DoHashTableConcat(this.proxy.hashTable);
			}
			base.Finish();
		}

		// Token: 0x060059EC RID: 23020 RVA: 0x001C75A8 File Offset: 0x001C57A8
		public void DoHashTableConcat(Hashtable source)
		{
			if (!base.isProxyValid())
			{
				return;
			}
			for (int i = 0; i < this.hashTableGameObjectTargets.Length; i++)
			{
				if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.hashTableGameObjectTargets[i]), this.referenceTargets[i].Value) && base.isProxyValid())
				{
					foreach (object key in this.proxy.hashTable.Keys)
					{
						if (source.ContainsKey(key))
						{
							if (this.overwriteExistingKey.Value)
							{
								source[key] = this.proxy.hashTable[key];
							}
						}
						else
						{
							source[key] = this.proxy.hashTable[key];
						}
					}
				}
			}
		}

		// Token: 0x04005565 RID: 21861
		[ActionSection("Storage")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005566 RID: 21862
		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component to store the concatenation ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x04005567 RID: 21863
		[ActionSection("HashTables to concatenate")]
		[CompoundArray("HashTables", "HashTable GameObject", "Reference")]
		[RequiredField]
		[Tooltip("The GameObject with the PlayMaker HashTable Proxy component to copy to")]
		[ObjectType(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault[] hashTableGameObjectTargets;

		// Token: 0x04005568 RID: 21864
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component to copy to ( necessary if several component coexists on the same GameObject")]
		public FsmString[] referenceTargets;

		// Token: 0x04005569 RID: 21865
		[Tooltip("Overwrite existing key with new values")]
		[UIHint(UIHint.FsmBool)]
		public FsmBool overwriteExistingKey;
	}
}
