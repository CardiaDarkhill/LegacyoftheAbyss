using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B2A RID: 2858
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Remove an element of a PlayMaker Array List Proxy component")]
	public class ArrayListRemove : ArrayListActions
	{
		// Token: 0x0600599C RID: 22940 RVA: 0x001C66D3 File Offset: 0x001C48D3
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.notFoundEvent = null;
			this.variable = null;
		}

		// Token: 0x0600599D RID: 22941 RVA: 0x001C66F1 File Offset: 0x001C48F1
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.DoRemoveFromArrayList();
			}
			base.Finish();
		}

		// Token: 0x0600599E RID: 22942 RVA: 0x001C6724 File Offset: 0x001C4924
		public void DoRemoveFromArrayList()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			if (!this.proxy.Remove(PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.variable), this.variable.Type.ToString(), false))
			{
				base.Fsm.Event(this.notFoundEvent);
			}
		}

		// Token: 0x0400551B RID: 21787
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400551C RID: 21788
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component (necessary if several component coexists on the same GameObject)")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;

		// Token: 0x0400551D RID: 21789
		[ActionSection("Data")]
		[Tooltip("The type of Variable to remove.")]
		public FsmVar variable;

		// Token: 0x0400551E RID: 21790
		[ActionSection("Result")]
		[Tooltip("Event sent if this arraList does not contains that element ( described below)")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent notFoundEvent;
	}
}
