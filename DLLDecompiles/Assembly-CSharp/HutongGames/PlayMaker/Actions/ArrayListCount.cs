using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B1F RID: 2847
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Count items from a PlayMaker ArrayList Proxy component")]
	public class ArrayListCount : ArrayListActions
	{
		// Token: 0x06005970 RID: 22896 RVA: 0x001C5831 File Offset: 0x001C3A31
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.count = null;
		}

		// Token: 0x06005971 RID: 22897 RVA: 0x001C5848 File Offset: 0x001C3A48
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.getArrayListCount();
			}
			base.Finish();
		}

		// Token: 0x06005972 RID: 22898 RVA: 0x001C587C File Offset: 0x001C3A7C
		public void getArrayListCount()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			int value = this.proxy.arrayList.Count;
			this.count.Value = value;
		}

		// Token: 0x040054D0 RID: 21712
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040054D1 RID: 21713
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x040054D2 RID: 21714
		[ActionSection("Result")]
		[UIHint(UIHint.FsmInt)]
		[Tooltip("Store the count value")]
		public FsmInt count;
	}
}
