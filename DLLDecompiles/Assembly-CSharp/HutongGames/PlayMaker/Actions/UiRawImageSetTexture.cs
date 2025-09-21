using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001161 RID: 4449
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the texture of a UI RawImage component.")]
	public class UiRawImageSetTexture : ComponentAction<RawImage>
	{
		// Token: 0x06007785 RID: 30597 RVA: 0x00245749 File Offset: 0x00243949
		public override void Reset()
		{
			this.gameObject = null;
			this.texture = null;
			this.resetOnExit = null;
		}

		// Token: 0x06007786 RID: 30598 RVA: 0x00245760 File Offset: 0x00243960
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this._texture = this.cachedComponent;
			}
			this._originalTexture = this._texture.texture;
			this.DoSetValue();
			base.Finish();
		}

		// Token: 0x06007787 RID: 30599 RVA: 0x002457B1 File Offset: 0x002439B1
		private void DoSetValue()
		{
			if (this._texture != null)
			{
				this._texture.texture = this.texture.Value;
			}
		}

		// Token: 0x06007788 RID: 30600 RVA: 0x002457D7 File Offset: 0x002439D7
		public override void OnExit()
		{
			if (this._texture == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this._texture.texture = this._originalTexture;
			}
		}

		// Token: 0x04007804 RID: 30724
		[RequiredField]
		[CheckForComponent(typeof(RawImage))]
		[Tooltip("The GameObject with the UI RawImage component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007805 RID: 30725
		[RequiredField]
		[Tooltip("The texture of the UI RawImage component.")]
		public FsmTexture texture;

		// Token: 0x04007806 RID: 30726
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x04007807 RID: 30727
		private RawImage _texture;

		// Token: 0x04007808 RID: 30728
		private Texture _originalTexture;
	}
}
