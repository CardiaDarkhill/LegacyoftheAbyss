using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B32 RID: 2866
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Sorts the sequence of elements in a PlayMaker ArrayList Proxy component")]
	public class ArrayListSort : ArrayListActions
	{
		// Token: 0x060059BD RID: 22973 RVA: 0x001C6CA2 File Offset: 0x001C4EA2
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
		}

		// Token: 0x060059BE RID: 22974 RVA: 0x001C6CB2 File Offset: 0x001C4EB2
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.DoArrayListSort();
			}
			base.Finish();
		}

		// Token: 0x060059BF RID: 22975 RVA: 0x001C6CE4 File Offset: 0x001C4EE4
		public void DoArrayListSort()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.arrayList.Sort();
		}

		// Token: 0x04005538 RID: 21816
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005539 RID: 21817
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;
	}
}
