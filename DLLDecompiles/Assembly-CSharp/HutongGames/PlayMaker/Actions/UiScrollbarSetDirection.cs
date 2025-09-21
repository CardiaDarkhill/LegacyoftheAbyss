using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001166 RID: 4454
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the direction of a UI Scrollbar component.")]
	public class UiScrollbarSetDirection : ComponentAction<Scrollbar>
	{
		// Token: 0x0600779E RID: 30622 RVA: 0x00245AB5 File Offset: 0x00243CB5
		public override void Reset()
		{
			this.gameObject = null;
			this.direction = Scrollbar.Direction.LeftToRight;
			this.includeRectLayouts = new FsmBool
			{
				UseVariable = true
			};
			this.resetOnExit = null;
		}

		// Token: 0x0600779F RID: 30623 RVA: 0x00245AE8 File Offset: 0x00243CE8
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.scrollbar = this.cachedComponent;
			}
			if (this.resetOnExit.Value)
			{
				this.originalValue = this.scrollbar.direction;
			}
			this.DoSetValue();
			base.Finish();
		}

		// Token: 0x060077A0 RID: 30624 RVA: 0x00245B48 File Offset: 0x00243D48
		private void DoSetValue()
		{
			if (this.scrollbar == null)
			{
				return;
			}
			if (this.includeRectLayouts.IsNone)
			{
				this.scrollbar.direction = (Scrollbar.Direction)this.direction.Value;
				return;
			}
			this.scrollbar.SetDirection((Scrollbar.Direction)this.direction.Value, this.includeRectLayouts.Value);
		}

		// Token: 0x060077A1 RID: 30625 RVA: 0x00245BB4 File Offset: 0x00243DB4
		public override void OnExit()
		{
			if (this.scrollbar == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				if (this.includeRectLayouts.IsNone)
				{
					this.scrollbar.direction = this.originalValue;
					return;
				}
				this.scrollbar.SetDirection(this.originalValue, this.includeRectLayouts.Value);
			}
		}

		// Token: 0x0400781A RID: 30746
		[RequiredField]
		[CheckForComponent(typeof(Scrollbar))]
		[Tooltip("The GameObject with the UI Scrollbar component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400781B RID: 30747
		[RequiredField]
		[Tooltip("The direction of the UI Scrollbar.")]
		[ObjectType(typeof(Scrollbar.Direction))]
		public FsmEnum direction;

		// Token: 0x0400781C RID: 30748
		[Tooltip("Include the  RectLayouts. Leave to none for no effect")]
		public FsmBool includeRectLayouts;

		// Token: 0x0400781D RID: 30749
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x0400781E RID: 30750
		private Scrollbar scrollbar;

		// Token: 0x0400781F RID: 30751
		private Scrollbar.Direction originalValue;
	}
}
