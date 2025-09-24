using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001163 RID: 4451
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the direction of a UI Scrollbar component.")]
	public class UiScrollbarGetDirection : ComponentAction<Scrollbar>
	{
		// Token: 0x0600778F RID: 30607 RVA: 0x002458A9 File Offset: 0x00243AA9
		public override void Reset()
		{
			this.gameObject = null;
			this.direction = null;
			this.everyFrame = false;
		}

		// Token: 0x06007790 RID: 30608 RVA: 0x002458C0 File Offset: 0x00243AC0
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

		// Token: 0x06007791 RID: 30609 RVA: 0x00245908 File Offset: 0x00243B08
		public override void OnUpdate()
		{
			this.DoGetValue();
		}

		// Token: 0x06007792 RID: 30610 RVA: 0x00245910 File Offset: 0x00243B10
		private void DoGetValue()
		{
			if (this.scrollbar != null)
			{
				this.direction.Value = this.scrollbar.direction;
			}
		}

		// Token: 0x0400780D RID: 30733
		[RequiredField]
		[CheckForComponent(typeof(Scrollbar))]
		[Tooltip("The GameObject with the UI Scrollbar component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400780E RID: 30734
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the direction of the UI Scrollbar.")]
		[ObjectType(typeof(Scrollbar.Direction))]
		public FsmEnum direction;

		// Token: 0x0400780F RID: 30735
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x04007810 RID: 30736
		private Scrollbar scrollbar;
	}
}
