using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B4E RID: 2894
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Takes a PlayMaker HashTable Proxy component snapshot, use action HashTableRevertToSnapShot was used. A Snapshot is taken by default at the beginning for the prefill data")]
	public class HashTableTakeSnapShot : HashTableActions
	{
		// Token: 0x06005A2D RID: 23085 RVA: 0x001C8421 File Offset: 0x001C6621
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
		}

		// Token: 0x06005A2E RID: 23086 RVA: 0x001C8431 File Offset: 0x001C6631
		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.DoHashTableTakeSnapShot();
			}
			base.Finish();
		}

		// Token: 0x06005A2F RID: 23087 RVA: 0x001C8463 File Offset: 0x001C6663
		public void DoHashTableTakeSnapShot()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.TakeSnapShot();
		}

		// Token: 0x040055BC RID: 21948
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040055BD RID: 21949
		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;
	}
}
