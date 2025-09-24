using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DD7 RID: 3543
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Sets the Blend Weight of an Animation. Check Every Frame to update the weight continuously, e.g., if you're manipulating a variable that controls the weight.")]
	public class SetAnimationWeight : BaseAnimationAction
	{
		// Token: 0x06006690 RID: 26256 RVA: 0x00207CF5 File Offset: 0x00205EF5
		public override void Reset()
		{
			this.gameObject = null;
			this.animName = null;
			this.weight = 1f;
			this.everyFrame = false;
		}

		// Token: 0x06006691 RID: 26257 RVA: 0x00207D1C File Offset: 0x00205F1C
		public override void OnEnter()
		{
			this.DoSetAnimationWeight((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value);
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006692 RID: 26258 RVA: 0x00207D57 File Offset: 0x00205F57
		public override void OnUpdate()
		{
			this.DoSetAnimationWeight((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value);
		}

		// Token: 0x06006693 RID: 26259 RVA: 0x00207D84 File Offset: 0x00205F84
		private void DoSetAnimationWeight(GameObject go)
		{
			if (!base.UpdateCache(go))
			{
				return;
			}
			AnimationState animationState = base.animation[this.animName.Value];
			if (animationState == null)
			{
				base.LogWarning("Missing animation: " + this.animName.Value);
				return;
			}
			animationState.weight = this.weight.Value;
		}

		// Token: 0x040065E9 RID: 26089
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		[Tooltip("The Game Object playing the animation.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040065EA RID: 26090
		[RequiredField]
		[Tooltip("The name of the animation.")]
		[UIHint(UIHint.Animation)]
		public FsmString animName;

		// Token: 0x040065EB RID: 26091
		[Tooltip("The weight to set the animation to.")]
		public FsmFloat weight = 1f;

		// Token: 0x040065EC RID: 26092
		[Tooltip("Perform this action every frame. Useful if Weight is a variable.")]
		public bool everyFrame;
	}
}
