using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FEB RID: 4075
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Checks the current contacts of a Rigidbody2D and sends an FSM event if a specific contact is met.")]
	public sealed class CheckContacts2D : FsmStateAction
	{
		// Token: 0x06007058 RID: 28760 RVA: 0x0022B6A1 File Offset: 0x002298A1
		public override void Reset()
		{
			this.gameObject = null;
			this.useRB2D = null;
			this.tagFilter = "";
			this.layerFilter = null;
			this.includeTriggers = true;
			this.contactEvent = null;
		}

		// Token: 0x06007059 RID: 28761 RVA: 0x0022B6DC File Offset: 0x002298DC
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			this.hasRb2d = false;
			if (this.useRB2D.Value)
			{
				this.rigidbody2D = ownerDefaultTarget.GetComponent<Rigidbody2D>();
				this.hasRb2d = (this.rigidbody2D != null);
				bool flag = this.hasRb2d;
			}
			if (!this.hasRb2d)
			{
				this.collider2D = ownerDefaultTarget.GetComponent<Collider2D>();
				if (this.collider2D == null)
				{
					base.Finish();
					return;
				}
			}
			if (this.CheckContacts())
			{
				base.Finish();
			}
		}

		// Token: 0x0600705A RID: 28762 RVA: 0x0022B77B File Offset: 0x0022997B
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600705B RID: 28763 RVA: 0x0022B789 File Offset: 0x00229989
		public override void OnFixedUpdate()
		{
			this.CheckContacts();
			base.Finish();
		}

		// Token: 0x0600705C RID: 28764 RVA: 0x0022B798 File Offset: 0x00229998
		private bool CheckContacts()
		{
			ContactFilter2D contactFilter = default(ContactFilter2D);
			if (!this.layerFilter.IsNone)
			{
				int intVal = 1 << this.layerFilter.Value;
				contactFilter.SetLayerMask(intVal);
			}
			contactFilter.useTriggers = this.includeTriggers.Value;
			int num = this.hasRb2d ? this.rigidbody2D.GetContacts(contactFilter, CheckContacts2D.contacts) : this.collider2D.GetContacts(contactFilter, CheckContacts2D.contacts);
			if (num == 0)
			{
				contactFilter = default(ContactFilter2D);
				num = (this.hasRb2d ? this.rigidbody2D.GetContacts(contactFilter, CheckContacts2D.contacts) : this.collider2D.GetContacts(contactFilter, CheckContacts2D.contacts));
				if (num == 0)
				{
					CheckContacts2D.contacts.EmptyArray<ContactPoint2D>();
					return false;
				}
			}
			bool flag = !string.IsNullOrEmpty(this.tagFilter.Value);
			for (int i = 0; i < num; i++)
			{
				ContactPoint2D contactPoint2D = CheckContacts2D.contacts[i];
				Collider2D collider = contactPoint2D.collider;
				if (!flag || collider.CompareTag(this.tagFilter.Value))
				{
					CheckContacts2D.contacts.EmptyArray<ContactPoint2D>();
					base.Fsm.Event(this.contactEvent);
					return true;
				}
			}
			CheckContacts2D.contacts.EmptyArray<ContactPoint2D>();
			return false;
		}

		// Token: 0x04007038 RID: 28728
		[RequiredField]
		[Tooltip("The GameObject with the Rigidbody2D.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007039 RID: 28729
		public FsmBool useRB2D;

		// Token: 0x0400703A RID: 28730
		[UIHint(UIHint.Tag)]
		[Tooltip("Optional tag to filter contacts.")]
		public FsmString tagFilter;

		// Token: 0x0400703B RID: 28731
		[UIHint(UIHint.Layer)]
		[Tooltip("Optional layer to filter contacts.")]
		public FsmInt layerFilter;

		// Token: 0x0400703C RID: 28732
		public FsmBool includeTriggers;

		// Token: 0x0400703D RID: 28733
		[Tooltip("The event to send if a specific contact is met.")]
		public FsmEvent contactEvent;

		// Token: 0x0400703E RID: 28734
		private Rigidbody2D rigidbody2D;

		// Token: 0x0400703F RID: 28735
		private Collider2D collider2D;

		// Token: 0x04007040 RID: 28736
		private bool hasRb2d;

		// Token: 0x04007041 RID: 28737
		private static ContactPoint2D[] contacts = new ContactPoint2D[10];
	}
}
