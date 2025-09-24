using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B2E RID: 2862
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Reverses the sequence of elements in a PlayMaker ArrayList Proxy component")]
	public class ArrayListReverse : ArrayListActions
	{
		// Token: 0x060059AC RID: 22956 RVA: 0x001C69BD File Offset: 0x001C4BBD
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
		}

		// Token: 0x060059AD RID: 22957 RVA: 0x001C69CD File Offset: 0x001C4BCD
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.DoArrayListReverse();
			}
			base.Finish();
		}

		// Token: 0x060059AE RID: 22958 RVA: 0x001C69FF File Offset: 0x001C4BFF
		public void DoArrayListReverse()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.arrayList.Reverse();
		}

		// Token: 0x0400552B RID: 21803
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400552C RID: 21804
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;
	}
}
