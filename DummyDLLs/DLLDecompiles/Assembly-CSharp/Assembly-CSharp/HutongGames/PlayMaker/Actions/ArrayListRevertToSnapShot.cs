using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B2F RID: 2863
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Revert a PlayMaker ArrayList Proxy component to the prefill data, either defined at runtime or when the action ArrayListTakeSnapShot was used. ")]
	public class ArrayListRevertToSnapShot : ArrayListActions
	{
		// Token: 0x060059B0 RID: 22960 RVA: 0x001C6A22 File Offset: 0x001C4C22
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
		}

		// Token: 0x060059B1 RID: 22961 RVA: 0x001C6A32 File Offset: 0x001C4C32
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.DoArrayListRevertToSnapShot();
			}
			base.Finish();
		}

		// Token: 0x060059B2 RID: 22962 RVA: 0x001C6A64 File Offset: 0x001C4C64
		public void DoArrayListRevertToSnapShot()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.RevertToSnapShot();
		}

		// Token: 0x0400552D RID: 21805
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400552E RID: 21806
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;
	}
}
