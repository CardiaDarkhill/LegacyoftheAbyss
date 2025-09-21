using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B75 RID: 2933
	[ActionCategory("2D Toolkit/Sprite")]
	[Tooltip("Get the pixel perfect flag of a sprite. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dClippedSprite)")]
	public class Tk2dSpriteGetPixelPerfect : FsmStateAction
	{
		// Token: 0x06005B0C RID: 23308 RVA: 0x001CBEE1 File Offset: 0x001CA0E1
		public override void OnEnter()
		{
			base.Finish();
		}

		// Token: 0x06005B0D RID: 23309 RVA: 0x001CBEE9 File Offset: 0x001CA0E9
		public override void Reset()
		{
			this.gameObject = null;
			this.pixelPerfect = null;
		}

		// Token: 0x04005694 RID: 22164
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dClippedSprite).")]
		[CheckForComponent(typeof(tk2dBaseSprite))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005695 RID: 22165
		[Tooltip("(Deprecated in 2D Toolkit 2.0) Is the sprite pixelPerfect")]
		[UIHint(UIHint.Variable)]
		public FsmBool pixelPerfect;
	}
}
