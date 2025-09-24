using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B34 RID: 2868
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Takes a PlayMaker ArrayList Proxy component snapshot, use action ArrayListRevertToSnapShot was used. A Snapshot is taken by default at the beginning for the prefill data")]
	public class ArrayListTakeSnapShot : ArrayListActions
	{
		// Token: 0x060059C5 RID: 22981 RVA: 0x001C6E1C File Offset: 0x001C501C
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
		}

		// Token: 0x060059C6 RID: 22982 RVA: 0x001C6E2C File Offset: 0x001C502C
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.DoArrayListTakeSnapShot();
			}
			base.Finish();
		}

		// Token: 0x060059C7 RID: 22983 RVA: 0x001C6E5E File Offset: 0x001C505E
		public void DoArrayListTakeSnapShot()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.TakeSnapShot();
		}

		// Token: 0x0400553F RID: 21823
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005540 RID: 21824
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;
	}
}
