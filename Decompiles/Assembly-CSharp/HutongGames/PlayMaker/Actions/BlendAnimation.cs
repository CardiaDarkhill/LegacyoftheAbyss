using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DCE RID: 3534
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Blends an Animation towards a Target Weight over a specified Time.\nOptionally sends an Event when finished.")]
	public class BlendAnimation : BaseAnimationAction
	{
		// Token: 0x0600665C RID: 26204 RVA: 0x00206FBC File Offset: 0x002051BC
		public override void Reset()
		{
			this.gameObject = null;
			this.animName = null;
			this.targetWeight = 1f;
			this.time = 0.3f;
			this.finishEvent = null;
		}

		// Token: 0x0600665D RID: 26205 RVA: 0x00206FF3 File Offset: 0x002051F3
		public override void OnEnter()
		{
			this.DoBlendAnimation((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value);
		}

		// Token: 0x0600665E RID: 26206 RVA: 0x00207020 File Offset: 0x00205220
		public override void OnUpdate()
		{
			if (DelayedEvent.WasSent(this.delayedFinishEvent))
			{
				base.Finish();
			}
		}

		// Token: 0x0600665F RID: 26207 RVA: 0x00207038 File Offset: 0x00205238
		private void DoBlendAnimation(GameObject go)
		{
			if (go == null)
			{
				return;
			}
			Animation component = go.GetComponent<Animation>();
			if (component == null)
			{
				base.LogWarning("Missing Animation component on GameObject: " + go.name);
				base.Finish();
				return;
			}
			AnimationState animationState = component[this.animName.Value];
			if (animationState == null)
			{
				base.LogWarning("Missing animation: " + this.animName.Value);
				base.Finish();
				return;
			}
			float value = this.time.Value;
			component.Blend(this.animName.Value, this.targetWeight.Value, value);
			if (this.finishEvent != null)
			{
				this.delayedFinishEvent = base.Fsm.DelayedEvent(this.finishEvent, animationState.length);
				return;
			}
			base.Finish();
		}

		// Token: 0x040065B8 RID: 26040
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		[Tooltip("The GameObject to animate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040065B9 RID: 26041
		[RequiredField]
		[UIHint(UIHint.Animation)]
		[Tooltip("The name of the animation to blend.")]
		public FsmString animName;

		// Token: 0x040065BA RID: 26042
		[RequiredField]
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Target weight to blend to.")]
		public FsmFloat targetWeight;

		// Token: 0x040065BB RID: 26043
		[RequiredField]
		[HasFloatSlider(0f, 5f)]
		[Tooltip("Time it should take to reach the target weight (seconds).")]
		public FsmFloat time;

		// Token: 0x040065BC RID: 26044
		[Tooltip("Event to send when the blend has finished.")]
		public FsmEvent finishEvent;

		// Token: 0x040065BD RID: 26045
		private DelayedEvent delayedFinishEvent;
	}
}
