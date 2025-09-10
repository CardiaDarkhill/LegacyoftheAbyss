using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DD9 RID: 3545
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Create a dynamic transition between the current state and the destination state. Both states have to be on the same layer. Note: You cannot change the current state on a synchronized layer, you need to change it on the referenced layer.")]
	public class AnimatorCrossFade : ComponentAction<Animator>
	{
		// Token: 0x17000BC7 RID: 3015
		// (get) Token: 0x06006699 RID: 26265 RVA: 0x00207E80 File Offset: 0x00206080
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x0600669A RID: 26266 RVA: 0x00207E88 File Offset: 0x00206088
		public override void Reset()
		{
			this.gameObject = null;
			this.stateName = null;
			this.transitionDuration = 1f;
			this.layer = new FsmInt
			{
				UseVariable = true
			};
			this.normalizedTime = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x0600669B RID: 26267 RVA: 0x00207ED8 File Offset: 0x002060D8
		public override void OnEnter()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			if (this.animator != null)
			{
				int num = this.layer.IsNone ? -1 : this.layer.Value;
				float normalizedTimeOffset = this.normalizedTime.IsNone ? float.NegativeInfinity : this.normalizedTime.Value;
				this.animator.CrossFade(this.stateName.Value, this.transitionDuration.Value, num, normalizedTimeOffset);
			}
			base.Finish();
		}

		// Token: 0x040065EF RID: 26095
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040065F0 RID: 26096
		[Tooltip("The name of the state that will be played.")]
		public FsmString stateName;

		// Token: 0x040065F1 RID: 26097
		[Tooltip("The duration of the transition. Value is in source state normalized time.")]
		public FsmFloat transitionDuration;

		// Token: 0x040065F2 RID: 26098
		[Tooltip("Layer index containing the destination state. Leave to none to ignore")]
		public FsmInt layer;

		// Token: 0x040065F3 RID: 26099
		[Tooltip("Start time of the current destination state. Value is in source state normalized time, should be between 0 and 1.")]
		public FsmFloat normalizedTime;
	}
}
