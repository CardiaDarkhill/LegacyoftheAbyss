using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B20 RID: 2848
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Check if an ArrayList Proxy component exists.")]
	public class ArrayListExists : ArrayListActions
	{
		// Token: 0x06005974 RID: 22900 RVA: 0x001C58B7 File Offset: 0x001C3AB7
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.doesExists = null;
			this.doesExistsEvent = null;
			this.doesNotExistsEvent = null;
		}

		// Token: 0x06005975 RID: 22901 RVA: 0x001C58DC File Offset: 0x001C3ADC
		public override void OnEnter()
		{
			bool flag = base.GetArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value, true) != null;
			this.doesExists.Value = flag;
			if (flag)
			{
				base.Fsm.Event(this.doesExistsEvent);
			}
			else
			{
				base.Fsm.Event(this.doesNotExistsEvent);
			}
			base.Finish();
		}

		// Token: 0x040054D3 RID: 21715
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040054D4 RID: 21716
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component (necessary if several component coexists on the same GameObject)")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;

		// Token: 0x040054D5 RID: 21717
		[ActionSection("Result")]
		[Tooltip("Store in a bool wether it exists or not")]
		[UIHint(UIHint.Variable)]
		public FsmBool doesExists;

		// Token: 0x040054D6 RID: 21718
		[Tooltip("Event sent if this arrayList exists ")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent doesExistsEvent;

		// Token: 0x040054D7 RID: 21719
		[Tooltip("Event sent if this arrayList does not exists")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent doesNotExistsEvent;
	}
}
