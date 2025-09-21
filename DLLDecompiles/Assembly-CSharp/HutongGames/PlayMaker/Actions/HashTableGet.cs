using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B44 RID: 2884
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Gets an item from a PlayMaker HashTable Proxy component")]
	public class HashTableGet : HashTableActions
	{
		// Token: 0x06005A05 RID: 23045 RVA: 0x001C7B6C File Offset: 0x001C5D6C
		public override void Reset()
		{
			this.gameObject = null;
			this.key = null;
			this.KeyFoundEvent = null;
			this.KeyNotFoundEvent = null;
			this.result = null;
		}

		// Token: 0x06005A06 RID: 23046 RVA: 0x001C7B91 File Offset: 0x001C5D91
		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.Get();
			}
			base.Finish();
		}

		// Token: 0x06005A07 RID: 23047 RVA: 0x001C7BC4 File Offset: 0x001C5DC4
		public void Get()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			if (!this.proxy.hashTable.ContainsKey(this.key.Value))
			{
				base.Fsm.Event(this.KeyNotFoundEvent);
				return;
			}
			PlayMakerUtils.ApplyValueToFsmVar(base.Fsm, this.result, this.proxy.hashTable[this.key.Value]);
			base.Fsm.Event(this.KeyFoundEvent);
		}

		// Token: 0x0400558A RID: 21898
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400558B RID: 21899
		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x0400558C RID: 21900
		[RequiredField]
		[UIHint(UIHint.FsmString)]
		[Tooltip("The Key value for that hash set")]
		public FsmString key;

		// Token: 0x0400558D RID: 21901
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		public FsmVar result;

		// Token: 0x0400558E RID: 21902
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger when key is found")]
		public FsmEvent KeyFoundEvent;

		// Token: 0x0400558F RID: 21903
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger when key is not found")]
		public FsmEvent KeyNotFoundEvent;
	}
}
