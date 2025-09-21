using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DCC RID: 3532
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Applies animation settings to the specified animation. Note: Settings are applied once, on entering the state, not continuously. Use\u00a0{{Set Animation Speed}},\u00a0{{Set Animation Time}}\u00a0etc. if you need to update those animation\u00a0settings every frame.\\nSee\u00a0<a href=\"https://docs.unity3d.com/Manual/AnimationScripting.html\" rel =\"nofollow\" target=\"_blank\">Unity Animation Docs</a>\u00a0for detailed descriptions of Wrap Mode, Blend Mode, Speed and Layer settings.")]
	public class AnimationSettings : BaseAnimationAction
	{
		// Token: 0x06006656 RID: 26198 RVA: 0x00206E57 File Offset: 0x00205057
		public override void Reset()
		{
			this.gameObject = null;
			this.animName = null;
			this.wrapMode = WrapMode.Loop;
			this.blendMode = AnimationBlendMode.Blend;
			this.speed = 1f;
			this.layer = 0;
		}

		// Token: 0x06006657 RID: 26199 RVA: 0x00206E91 File Offset: 0x00205091
		public override void OnEnter()
		{
			this.DoAnimationSettings();
			base.Finish();
		}

		// Token: 0x06006658 RID: 26200 RVA: 0x00206EA0 File Offset: 0x002050A0
		private void DoAnimationSettings()
		{
			if (string.IsNullOrEmpty(this.animName.Value))
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			AnimationState animationState = base.animation[this.animName.Value];
			if (animationState == null)
			{
				base.LogWarning("Missing animation: " + this.animName.Value);
				return;
			}
			animationState.wrapMode = this.wrapMode;
			animationState.blendMode = this.blendMode;
			if (!this.layer.IsNone)
			{
				animationState.layer = this.layer.Value;
			}
			if (!this.speed.IsNone)
			{
				animationState.speed = this.speed.Value;
			}
		}

		// Token: 0x040065B2 RID: 26034
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		[Tooltip("A GameObject with an Animation Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040065B3 RID: 26035
		[RequiredField]
		[UIHint(UIHint.Animation)]
		[Tooltip("The name of the animation. Use the browse button to select from animations on the Game Object (if available).")]
		public FsmString animName;

		// Token: 0x040065B4 RID: 26036
		[Tooltip("Set how the animation wraps (Loop, PingPong etc.). NOTE: Because of the way WrapMode is defined by Unity you cannot select Once, but Clamp is the same as Once.")]
		public WrapMode wrapMode;

		// Token: 0x040065B5 RID: 26037
		[Tooltip("How the animation is blended with other animations on the Game Object.")]
		public AnimationBlendMode blendMode;

		// Token: 0x040065B6 RID: 26038
		[HasFloatSlider(0f, 5f)]
		[Tooltip("Speed up or slow down the animation. 1 is normal speed, 0.5 is half speed...")]
		public FsmFloat speed;

		// Token: 0x040065B7 RID: 26039
		[Tooltip("You can play animations on different layers to combine them into a final animation. See the Unity Animation docs for more details.")]
		public FsmInt layer;
	}
}
