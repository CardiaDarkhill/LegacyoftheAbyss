using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001151 RID: 4433
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the text value of a UI InputField component as a float.")]
	public class UiInputFieldGetTextAsFloat : ComponentAction<InputField>
	{
		// Token: 0x06007733 RID: 30515 RVA: 0x002449D2 File Offset: 0x00242BD2
		public override void Reset()
		{
			this.value = null;
			this.isFloat = null;
			this.isFloatEvent = null;
			this.isNotFloatEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x06007734 RID: 30516 RVA: 0x002449F8 File Offset: 0x00242BF8
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.inputField = this.cachedComponent;
			}
			this.DoGetTextValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007735 RID: 30517 RVA: 0x00244A40 File Offset: 0x00242C40
		public override void OnUpdate()
		{
			this.DoGetTextValue();
		}

		// Token: 0x06007736 RID: 30518 RVA: 0x00244A48 File Offset: 0x00242C48
		private void DoGetTextValue()
		{
			if (this.inputField == null)
			{
				return;
			}
			this._success = float.TryParse(this.inputField.text, out this._value);
			this.value.Value = this._value;
			this.isFloat.Value = this._success;
			base.Fsm.Event(this._success ? this.isFloatEvent : this.isNotFloatEvent);
		}

		// Token: 0x040077AB RID: 30635
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040077AC RID: 30636
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The text value as a float of the UI InputField component.")]
		public FsmFloat value;

		// Token: 0x040077AD RID: 30637
		[UIHint(UIHint.Variable)]
		[Tooltip("true if text resolves to a float")]
		public FsmBool isFloat;

		// Token: 0x040077AE RID: 30638
		[Tooltip("true if text resolves to a float")]
		public FsmEvent isFloatEvent;

		// Token: 0x040077AF RID: 30639
		[Tooltip("Event sent if text does not resolves to a float")]
		public FsmEvent isNotFloatEvent;

		// Token: 0x040077B0 RID: 30640
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040077B1 RID: 30641
		private InputField inputField;

		// Token: 0x040077B2 RID: 30642
		private float _value;

		// Token: 0x040077B3 RID: 30643
		private bool _success;
	}
}
