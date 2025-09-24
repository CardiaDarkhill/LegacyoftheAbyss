using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001147 RID: 4423
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the source image sprite of a UI Image component.")]
	public class UiImageSetSprite : ComponentAction<Image>
	{
		// Token: 0x06007704 RID: 30468 RVA: 0x00244311 File Offset: 0x00242511
		public override void Reset()
		{
			this.gameObject = null;
			this.resetOnExit = false;
		}

		// Token: 0x06007705 RID: 30469 RVA: 0x00244328 File Offset: 0x00242528
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.image = this.cachedComponent;
			}
			this.originalSprite = this.image.sprite;
			this.DoSetImageSourceValue();
			base.Finish();
		}

		// Token: 0x06007706 RID: 30470 RVA: 0x00244379 File Offset: 0x00242579
		private void DoSetImageSourceValue()
		{
			if (this.image == null)
			{
				return;
			}
			this.image.sprite = (this.sprite.Value as Sprite);
		}

		// Token: 0x06007707 RID: 30471 RVA: 0x002443A5 File Offset: 0x002425A5
		public override void OnExit()
		{
			if (this.image == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.image.sprite = this.originalSprite;
			}
		}

		// Token: 0x0400777E RID: 30590
		[RequiredField]
		[CheckForComponent(typeof(Image))]
		[Tooltip("The GameObject with the Image UI component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400777F RID: 30591
		[RequiredField]
		[Tooltip("The source sprite of the UI Image component.")]
		[ObjectType(typeof(Sprite))]
		public FsmObject sprite;

		// Token: 0x04007780 RID: 30592
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x04007781 RID: 30593
		private Image image;

		// Token: 0x04007782 RID: 30594
		private Sprite originalSprite;
	}
}
