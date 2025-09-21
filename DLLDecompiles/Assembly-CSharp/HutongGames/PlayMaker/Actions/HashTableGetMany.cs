using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B46 RID: 2886
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Gets items from a PlayMaker HashTable Proxy component")]
	public class HashTableGetMany : HashTableActions
	{
		// Token: 0x06005A0D RID: 23053 RVA: 0x001C7D58 File Offset: 0x001C5F58
		public override void Reset()
		{
			this.gameObject = null;
			this.keys = null;
			this.results = null;
		}

		// Token: 0x06005A0E RID: 23054 RVA: 0x001C7D6F File Offset: 0x001C5F6F
		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.Get();
			}
			base.Finish();
		}

		// Token: 0x06005A0F RID: 23055 RVA: 0x001C7DA4 File Offset: 0x001C5FA4
		public void Get()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			for (int i = 0; i < this.keys.Length; i++)
			{
				if (this.proxy.hashTable.ContainsKey(this.keys[i].Value))
				{
					PlayMakerUtils.ApplyValueToFsmVar(base.Fsm, this.results[i], this.proxy.hashTable[this.keys[i].Value]);
				}
			}
		}

		// Token: 0x04005596 RID: 21910
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005597 RID: 21911
		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x04005598 RID: 21912
		[ActionSection("Data")]
		[CompoundArray("Count", "Key", "Value")]
		[RequiredField]
		[UIHint(UIHint.FsmString)]
		[Tooltip("The Key value for that hash set")]
		public FsmString[] keys;

		// Token: 0x04005599 RID: 21913
		[Tooltip("The value for that key")]
		[UIHint(UIHint.Variable)]
		public FsmVar[] results;
	}
}
