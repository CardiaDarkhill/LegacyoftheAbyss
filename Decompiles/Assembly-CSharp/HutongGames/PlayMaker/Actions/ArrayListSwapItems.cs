using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B33 RID: 2867
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Swap two items at a specified indexes of a PlayMaker ArrayList Proxy component")]
	public class ArrayListSwapItems : ArrayListActions
	{
		// Token: 0x060059C1 RID: 22977 RVA: 0x001C6D07 File Offset: 0x001C4F07
		public override void Reset()
		{
			this.gameObject = null;
			this.failureEvent = null;
			this.reference = null;
			this.index1 = null;
			this.index2 = null;
		}

		// Token: 0x060059C2 RID: 22978 RVA: 0x001C6D2C File Offset: 0x001C4F2C
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.doArrayListSwap();
			}
			base.Finish();
		}

		// Token: 0x060059C3 RID: 22979 RVA: 0x001C6D60 File Offset: 0x001C4F60
		public void doArrayListSwap()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			try
			{
				object value = this.proxy.arrayList[this.index2.Value];
				this.proxy.arrayList[this.index2.Value] = this.proxy.arrayList[this.index1.Value];
				this.proxy.arrayList[this.index1.Value] = value;
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				base.Fsm.Event(this.failureEvent);
			}
		}

		// Token: 0x0400553A RID: 21818
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400553B RID: 21819
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x0400553C RID: 21820
		[UIHint(UIHint.FsmInt)]
		[Tooltip("The first index to swap")]
		public FsmInt index1;

		// Token: 0x0400553D RID: 21821
		[UIHint(UIHint.FsmInt)]
		[Tooltip("The second index to swap")]
		public FsmInt index2;

		// Token: 0x0400553E RID: 21822
		[ActionSection("Result")]
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the removeAt throw errors")]
		public FsmEvent failureEvent;
	}
}
