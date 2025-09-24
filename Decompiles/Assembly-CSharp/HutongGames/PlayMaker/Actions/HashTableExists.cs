using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B43 RID: 2883
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Check if an HashTable Proxy component exists.")]
	public class HashTableExists : ArrayListActions
	{
		// Token: 0x06005A02 RID: 23042 RVA: 0x001C7ACF File Offset: 0x001C5CCF
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.doesExists = null;
			this.doesExistsEvent = null;
			this.doesNotExistsEvent = null;
		}

		// Token: 0x06005A03 RID: 23043 RVA: 0x001C7AF4 File Offset: 0x001C5CF4
		public override void OnEnter()
		{
			bool flag = base.GetHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value, true) != null;
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

		// Token: 0x04005585 RID: 21893
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005586 RID: 21894
		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x04005587 RID: 21895
		[ActionSection("Result")]
		[Tooltip("Store in a bool wether it exists or not")]
		[UIHint(UIHint.Variable)]
		public FsmBool doesExists;

		// Token: 0x04005588 RID: 21896
		[Tooltip("Event sent if this HashTable exists ")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent doesExistsEvent;

		// Token: 0x04005589 RID: 21897
		[Tooltip("Event sent if this HashTable does not exists")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent doesNotExistsEvent;
	}
}
