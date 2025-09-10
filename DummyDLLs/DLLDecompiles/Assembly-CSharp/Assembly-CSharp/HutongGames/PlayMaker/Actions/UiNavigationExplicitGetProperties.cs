using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200112E RID: 4398
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the explicit navigation properties of a UI Selectable component. ")]
	public class UiNavigationExplicitGetProperties : ComponentAction<Selectable>
	{
		// Token: 0x0600768E RID: 30350 RVA: 0x00242582 File Offset: 0x00240782
		public override void Reset()
		{
			this.gameObject = null;
			this.selectOnDown = null;
			this.selectOnUp = null;
			this.selectOnLeft = null;
			this.selectOnRight = null;
		}

		// Token: 0x0600768F RID: 30351 RVA: 0x002425A8 File Offset: 0x002407A8
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				this._selectable = ownerDefaultTarget.GetComponent<Selectable>();
			}
			this.DoGetValue();
			base.Finish();
		}

		// Token: 0x06007690 RID: 30352 RVA: 0x002425E8 File Offset: 0x002407E8
		private void DoGetValue()
		{
			if (this._selectable != null)
			{
				if (!this.selectOnUp.IsNone)
				{
					this.selectOnUp.Value = ((this._selectable.navigation.selectOnUp == null) ? null : this._selectable.navigation.selectOnUp.gameObject);
				}
				if (!this.selectOnDown.IsNone)
				{
					this.selectOnDown.Value = ((this._selectable.navigation.selectOnDown == null) ? null : this._selectable.navigation.selectOnDown.gameObject);
				}
				if (!this.selectOnLeft.IsNone)
				{
					this.selectOnLeft.Value = ((this._selectable.navigation.selectOnLeft == null) ? null : this._selectable.navigation.selectOnLeft.gameObject);
				}
				if (!this.selectOnRight.IsNone)
				{
					this.selectOnRight.Value = ((this._selectable.navigation.selectOnRight == null) ? null : this._selectable.navigation.selectOnRight.gameObject);
				}
			}
		}

		// Token: 0x040076E5 RID: 30437
		[RequiredField]
		[CheckForComponent(typeof(Selectable))]
		[Tooltip("The GameObject with the UI Selectable component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040076E6 RID: 30438
		[Tooltip("The down Selectable.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject selectOnDown;

		// Token: 0x040076E7 RID: 30439
		[Tooltip("The up Selectable.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject selectOnUp;

		// Token: 0x040076E8 RID: 30440
		[Tooltip("The left Selectable.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject selectOnLeft;

		// Token: 0x040076E9 RID: 30441
		[Tooltip("The right Selectable.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject selectOnRight;

		// Token: 0x040076EA RID: 30442
		private Selectable _selectable;
	}
}
