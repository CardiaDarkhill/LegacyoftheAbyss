using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B4B RID: 2891
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Revert a PlayMaker HashTable Proxy component to the prefill data, either defined at runtime or when the action HashTableTakeSnapShot was used.")]
	public class HashTableRevertSnapShot : HashTableActions
	{
		// Token: 0x06005A21 RID: 23073 RVA: 0x001C8281 File Offset: 0x001C6481
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
		}

		// Token: 0x06005A22 RID: 23074 RVA: 0x001C8291 File Offset: 0x001C6491
		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.DoHashTableRevertToSnapShot();
			}
			base.Finish();
		}

		// Token: 0x06005A23 RID: 23075 RVA: 0x001C82C3 File Offset: 0x001C64C3
		public void DoHashTableRevertToSnapShot()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.RevertToSnapShot();
		}

		// Token: 0x040055B2 RID: 21938
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040055B3 RID: 21939
		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;
	}
}
