using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001144 RID: 4420
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the source image sprite of a UI Image component.")]
	public class UiImageGetSprite : ComponentAction<Image>
	{
		// Token: 0x060076F6 RID: 30454 RVA: 0x0024414E File Offset: 0x0024234E
		public override void Reset()
		{
			this.gameObject = null;
			this.sprite = null;
		}

		// Token: 0x060076F7 RID: 30455 RVA: 0x00244160 File Offset: 0x00242360
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.image = this.cachedComponent;
			}
			this.DoSetImageSourceValue();
			base.Finish();
		}

		// Token: 0x060076F8 RID: 30456 RVA: 0x002441A0 File Offset: 0x002423A0
		private void DoSetImageSourceValue()
		{
			if (this.image != null)
			{
				this.sprite.Value = this.image.sprite;
			}
		}

		// Token: 0x04007773 RID: 30579
		[RequiredField]
		[CheckForComponent(typeof(Image))]
		[Tooltip("The GameObject with the UI Image component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007774 RID: 30580
		[RequiredField]
		[Tooltip("The source sprite of the UI Image component.")]
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(Sprite))]
		public FsmObject sprite;

		// Token: 0x04007775 RID: 30581
		private Image image;
	}
}
