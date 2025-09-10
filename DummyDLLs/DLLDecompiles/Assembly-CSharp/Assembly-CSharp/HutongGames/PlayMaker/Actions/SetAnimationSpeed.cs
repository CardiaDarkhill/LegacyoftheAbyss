using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DD5 RID: 3541
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Sets the Speed of an Animation. Check Every Frame to update the animation time continuously, e.g., if you're manipulating a variable that controls animation speed.")]
	public class SetAnimationSpeed : BaseAnimationAction
	{
		// Token: 0x06006686 RID: 26246 RVA: 0x00207AA9 File Offset: 0x00205CA9
		public override void Reset()
		{
			this.gameObject = null;
			this.animName = null;
			this.speed = 1f;
			this.everyFrame = false;
		}

		// Token: 0x06006687 RID: 26247 RVA: 0x00207AD0 File Offset: 0x00205CD0
		public override void OnEnter()
		{
			this.DoSetAnimationSpeed((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value);
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006688 RID: 26248 RVA: 0x00207B0B File Offset: 0x00205D0B
		public override void OnUpdate()
		{
			this.DoSetAnimationSpeed((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value);
		}

		// Token: 0x06006689 RID: 26249 RVA: 0x00207B38 File Offset: 0x00205D38
		private void DoSetAnimationSpeed(GameObject go)
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
			animationState.speed = this.speed.Value;
		}

		// Token: 0x040065E0 RID: 26080
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		[Tooltip("The Game Object playing the animation.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040065E1 RID: 26081
		[RequiredField]
		[UIHint(UIHint.Animation)]
		[Tooltip("The name of the animation.")]
		public FsmString animName;

		// Token: 0x040065E2 RID: 26082
		[Tooltip("The desired animation speed. 1= normal, 0.5 = half speed, 2 = double speed.")]
		public FsmFloat speed = 1f;

		// Token: 0x040065E3 RID: 26083
		[Tooltip("Update the speed every frame. Useful if you're using a variable to set Speed.")]
		public bool everyFrame;
	}
}
