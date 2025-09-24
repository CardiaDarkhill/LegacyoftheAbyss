using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200117A RID: 4474
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the isOn value of a UI Toggle component. Optionally send events")]
	public class UiToggleGetIsOn : ComponentAction<Toggle>
	{
		// Token: 0x0600780A RID: 30730 RVA: 0x00246CBF File Offset: 0x00244EBF
		public override void Reset()
		{
			this.gameObject = null;
			this.value = null;
			this.everyFrame = false;
		}

		// Token: 0x0600780B RID: 30731 RVA: 0x00246CD8 File Offset: 0x00244ED8
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this._toggle = this.cachedComponent;
			}
			this.DoGetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600780C RID: 30732 RVA: 0x00246D20 File Offset: 0x00244F20
		public override void OnUpdate()
		{
			this.DoGetValue();
		}

		// Token: 0x0600780D RID: 30733 RVA: 0x00246D28 File Offset: 0x00244F28
		private void DoGetValue()
		{
			if (this._toggle == null)
			{
				return;
			}
			this.value.Value = this._toggle.isOn;
			base.Fsm.Event(this._toggle.isOn ? this.isOnEvent : this.isOffEvent);
		}

		// Token: 0x04007889 RID: 30857
		[RequiredField]
		[CheckForComponent(typeof(Toggle))]
		[Tooltip("The GameObject with the UI Toggle component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400788A RID: 30858
		[UIHint(UIHint.Variable)]
		[Tooltip("The isOn Value of the UI Toggle component.")]
		public FsmBool value;

		// Token: 0x0400788B RID: 30859
		[Tooltip("Event sent when isOn Value is true.")]
		public FsmEvent isOnEvent;

		// Token: 0x0400788C RID: 30860
		[Tooltip("Event sent when isOn Value is false.")]
		public FsmEvent isOffEvent;

		// Token: 0x0400788D RID: 30861
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x0400788E RID: 30862
		private Toggle _toggle;
	}
}
