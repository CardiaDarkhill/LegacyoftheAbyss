using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B4D RID: 2893
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Set key/value pairs to a PlayMaker HashTable Proxy component (PlayMakerHashTableProxy)")]
	public class HashTableSetMany : HashTableActions
	{
		// Token: 0x06005A29 RID: 23081 RVA: 0x001C8370 File Offset: 0x001C6570
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.keys = null;
			this.variables = null;
		}

		// Token: 0x06005A2A RID: 23082 RVA: 0x001C838E File Offset: 0x001C658E
		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.SetHashTable();
			}
			base.Finish();
		}

		// Token: 0x06005A2B RID: 23083 RVA: 0x001C83C0 File Offset: 0x001C65C0
		public void SetHashTable()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			for (int i = 0; i < this.keys.Length; i++)
			{
				this.proxy.hashTable[this.keys[i].Value] = PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.variables[i]);
			}
		}

		// Token: 0x040055B8 RID: 21944
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040055B9 RID: 21945
		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x040055BA RID: 21946
		[ActionSection("Data")]
		[CompoundArray("Count", "Key", "Value")]
		[RequiredField]
		[UIHint(UIHint.FsmString)]
		[Tooltip("The Key values for that hash set")]
		public FsmString[] keys;

		// Token: 0x040055BB RID: 21947
		[Tooltip("The variable to set.")]
		public FsmVar[] variables;
	}
}
