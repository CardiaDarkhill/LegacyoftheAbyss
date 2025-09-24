using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001136 RID: 4406
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the interactable flag of a UI Selectable component.")]
	public class UiSetIsInteractable : FsmStateAction
	{
		// Token: 0x060076B5 RID: 30389 RVA: 0x002432FB File Offset: 0x002414FB
		public override void Reset()
		{
			this.gameObject = null;
			this.isInteractable = null;
			this.resetOnExit = false;
		}

		// Token: 0x060076B6 RID: 30390 RVA: 0x00243318 File Offset: 0x00241518
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				this._selectable = ownerDefaultTarget.GetComponent<Selectable>();
			}
			if (this._selectable != null && this.resetOnExit.Value)
			{
				this._originalState = this._selectable.IsInteractable();
			}
			this.DoSetValue();
			base.Finish();
		}

		// Token: 0x060076B7 RID: 30391 RVA: 0x00243384 File Offset: 0x00241584
		private void DoSetValue()
		{
			if (this._selectable != null)
			{
				this._selectable.interactable = this.isInteractable.Value;
			}
		}

		// Token: 0x060076B8 RID: 30392 RVA: 0x002433AA File Offset: 0x002415AA
		public override void OnExit()
		{
			if (this._selectable == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this._selectable.interactable = this._originalState;
			}
		}

		// Token: 0x04007725 RID: 30501
		[RequiredField]
		[CheckForComponent(typeof(Selectable))]
		[Tooltip("The GameObject with the UI Selectable component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007726 RID: 30502
		[Tooltip("The Interactable value")]
		public FsmBool isInteractable;

		// Token: 0x04007727 RID: 30503
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x04007728 RID: 30504
		private Selectable _selectable;

		// Token: 0x04007729 RID: 30505
		private bool _originalState;
	}
}
