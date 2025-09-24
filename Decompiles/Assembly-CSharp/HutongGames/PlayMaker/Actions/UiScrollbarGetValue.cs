using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001164 RID: 4452
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the value of a UI Scrollbar component.")]
	public class UiScrollbarGetValue : ComponentAction<Scrollbar>
	{
		// Token: 0x06007794 RID: 30612 RVA: 0x00245943 File Offset: 0x00243B43
		public override void Reset()
		{
			this.gameObject = null;
			this.value = null;
			this.everyFrame = false;
		}

		// Token: 0x06007795 RID: 30613 RVA: 0x0024595C File Offset: 0x00243B5C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.scrollbar = this.cachedComponent;
			}
			this.DoGetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007796 RID: 30614 RVA: 0x002459A4 File Offset: 0x00243BA4
		public override void OnUpdate()
		{
			this.DoGetValue();
		}

		// Token: 0x06007797 RID: 30615 RVA: 0x002459AC File Offset: 0x00243BAC
		private void DoGetValue()
		{
			if (this.scrollbar != null)
			{
				this.value.Value = this.scrollbar.value;
			}
		}

		// Token: 0x04007811 RID: 30737
		[RequiredField]
		[CheckForComponent(typeof(Scrollbar))]
		[Tooltip("The GameObject with the UI Scrollbar component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007812 RID: 30738
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The position value of the UI Scrollbar.")]
		public FsmFloat value;

		// Token: 0x04007813 RID: 30739
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x04007814 RID: 30740
		private Scrollbar scrollbar;
	}
}
