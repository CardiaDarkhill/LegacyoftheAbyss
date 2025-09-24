using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B27 RID: 2855
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Check if an ArrayList Proxy component is empty.")]
	public class ArrayListIsEmpty : ArrayListActions
	{
		// Token: 0x06005991 RID: 22929 RVA: 0x001C62D4 File Offset: 0x001C44D4
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.isEmpty = null;
			this.isNotEmptyEvent = null;
			this.isEmptyEvent = null;
		}

		// Token: 0x06005992 RID: 22930 RVA: 0x001C62FC File Offset: 0x001C44FC
		public override void OnEnter()
		{
			bool flag = base.GetArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value, true).arrayList.Count == 0;
			this.isEmpty.Value = flag;
			if (flag)
			{
				base.Fsm.Event(this.isEmptyEvent);
			}
			else
			{
				base.Fsm.Event(this.isNotEmptyEvent);
			}
			base.Finish();
		}

		// Token: 0x04005507 RID: 21767
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005508 RID: 21768
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component (necessary if several component coexists on the same GameObject)")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;

		// Token: 0x04005509 RID: 21769
		[ActionSection("Result")]
		[Tooltip("Store in a bool wether it is empty or not")]
		[UIHint(UIHint.Variable)]
		public FsmBool isEmpty;

		// Token: 0x0400550A RID: 21770
		[Tooltip("Event sent if this arrayList is empty ")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent isEmptyEvent;

		// Token: 0x0400550B RID: 21771
		[Tooltip("Event sent if this arrayList is not empty")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent isNotEmptyEvent;
	}
}
