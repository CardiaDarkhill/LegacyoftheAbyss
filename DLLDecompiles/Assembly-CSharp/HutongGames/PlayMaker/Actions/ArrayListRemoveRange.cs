using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B2C RID: 2860
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Remove the specified range of elements from a PlayMaker ArrayList Proxy component")]
	public class ArrayListRemoveRange : ArrayListActions
	{
		// Token: 0x060059A4 RID: 22948 RVA: 0x001C6848 File Offset: 0x001C4A48
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.index = null;
			this.count = null;
			this.failureEvent = null;
		}

		// Token: 0x060059A5 RID: 22949 RVA: 0x001C686D File Offset: 0x001C4A6D
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.doArrayListRemoveRange();
			}
			base.Finish();
		}

		// Token: 0x060059A6 RID: 22950 RVA: 0x001C68A0 File Offset: 0x001C4AA0
		public void doArrayListRemoveRange()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			try
			{
				this.proxy.arrayList.RemoveRange(this.index.Value, this.count.Value);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				base.Fsm.Event(this.failureEvent);
			}
		}

		// Token: 0x04005523 RID: 21795
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005524 RID: 21796
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x04005525 RID: 21797
		[UIHint(UIHint.FsmInt)]
		[Tooltip("The zero-based index of the first element of the range of elements to remove. This value is between 0 and the array.count minus count (inclusive)")]
		public FsmInt index;

		// Token: 0x04005526 RID: 21798
		[UIHint(UIHint.FsmInt)]
		[Tooltip("The number of elements to remove. This value is between 0 and the difference between the array.count minus the index ( inclusive )")]
		public FsmInt count;

		// Token: 0x04005527 RID: 21799
		[ActionSection("Result")]
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the removeRange throw errors")]
		public FsmEvent failureEvent;
	}
}
