using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B2B RID: 2859
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Remove item at a specified index of a PlayMaker ArrayList Proxy component")]
	public class ArrayListRemoveAt : ArrayListActions
	{
		// Token: 0x060059A0 RID: 22944 RVA: 0x001C678B File Offset: 0x001C498B
		public override void Reset()
		{
			this.gameObject = null;
			this.failureEvent = null;
			this.reference = null;
			this.index = null;
		}

		// Token: 0x060059A1 RID: 22945 RVA: 0x001C67A9 File Offset: 0x001C49A9
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.doArrayListRemoveAt();
			}
			base.Finish();
		}

		// Token: 0x060059A2 RID: 22946 RVA: 0x001C67DC File Offset: 0x001C49DC
		public void doArrayListRemoveAt()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			try
			{
				this.proxy.arrayList.RemoveAt(this.index.Value);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				base.Fsm.Event(this.failureEvent);
			}
		}

		// Token: 0x0400551F RID: 21791
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005520 RID: 21792
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x04005521 RID: 21793
		[UIHint(UIHint.FsmInt)]
		[Tooltip("The index to remove at")]
		public FsmInt index;

		// Token: 0x04005522 RID: 21794
		[ActionSection("Result")]
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the removeAt throw errors")]
		public FsmEvent failureEvent;
	}
}
