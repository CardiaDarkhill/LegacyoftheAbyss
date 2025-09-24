using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B41 RID: 2881
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Count the number of items ( key/value pairs) in a PlayMaker HashTable Proxy component (PlayMakerHashTableProxy).")]
	public class HashTableCount : HashTableActions
	{
		// Token: 0x060059FA RID: 23034 RVA: 0x001C792A File Offset: 0x001C5B2A
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.count = null;
		}

		// Token: 0x060059FB RID: 23035 RVA: 0x001C7941 File Offset: 0x001C5B41
		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.doHashTableCount();
			}
			base.Finish();
		}

		// Token: 0x060059FC RID: 23036 RVA: 0x001C7973 File Offset: 0x001C5B73
		public void doHashTableCount()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.count.Value = this.proxy.hashTable.Count;
		}

		// Token: 0x0400557C RID: 21884
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400557D RID: 21885
		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x0400557E RID: 21886
		[ActionSection("Result")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The number of items in that hashTable")]
		public FsmInt count;
	}
}
