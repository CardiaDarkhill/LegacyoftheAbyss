using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001152 RID: 4434
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the text value of a UI InputField component as an Int.")]
	public class UiInputFieldGetTextAsInt : ComponentAction<InputField>
	{
		// Token: 0x06007738 RID: 30520 RVA: 0x00244ACB File Offset: 0x00242CCB
		public override void Reset()
		{
			this.value = null;
			this.isInt = null;
			this.isIntEvent = null;
			this.isNotIntEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x06007739 RID: 30521 RVA: 0x00244AF0 File Offset: 0x00242CF0
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

		// Token: 0x0600773A RID: 30522 RVA: 0x00244B38 File Offset: 0x00242D38
		public override void OnUpdate()
		{
			this.DoGetTextValue();
		}

		// Token: 0x0600773B RID: 30523 RVA: 0x00244B40 File Offset: 0x00242D40
		private void DoGetTextValue()
		{
			if (this.inputField == null)
			{
				return;
			}
			this._success = int.TryParse(this.inputField.text, out this._value);
			this.value.Value = this._value;
			this.isInt.Value = this._success;
			base.Fsm.Event(this._success ? this.isIntEvent : this.isNotIntEvent);
		}

		// Token: 0x040077B4 RID: 30644
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040077B5 RID: 30645
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the text value as an int.")]
		public FsmInt value;

		// Token: 0x040077B6 RID: 30646
		[UIHint(UIHint.Variable)]
		[Tooltip("True if text resolves to an int")]
		public FsmBool isInt;

		// Token: 0x040077B7 RID: 30647
		[Tooltip("Event to send if text resolves to an int")]
		public FsmEvent isIntEvent;

		// Token: 0x040077B8 RID: 30648
		[Tooltip("Event to send if text does NOT resolve to an int")]
		public FsmEvent isNotIntEvent;

		// Token: 0x040077B9 RID: 30649
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040077BA RID: 30650
		private InputField inputField;

		// Token: 0x040077BB RID: 30651
		private int _value;

		// Token: 0x040077BC RID: 30652
		private bool _success;
	}
}
