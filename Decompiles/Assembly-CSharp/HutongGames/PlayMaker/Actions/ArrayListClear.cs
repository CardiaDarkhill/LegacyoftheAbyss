using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B1B RID: 2843
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Removes all elements from a PlayMaker ArrayList Proxy component")]
	public class ArrayListClear : ArrayListActions
	{
		// Token: 0x06005960 RID: 22880 RVA: 0x001C5253 File Offset: 0x001C3453
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
		}

		// Token: 0x06005961 RID: 22881 RVA: 0x001C5263 File Offset: 0x001C3463
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.ClearArrayList();
			}
			base.Finish();
		}

		// Token: 0x06005962 RID: 22882 RVA: 0x001C5295 File Offset: 0x001C3495
		public void ClearArrayList()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.arrayList.Clear();
		}

		// Token: 0x040054BE RID: 21694
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040054BF RID: 21695
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;
	}
}
