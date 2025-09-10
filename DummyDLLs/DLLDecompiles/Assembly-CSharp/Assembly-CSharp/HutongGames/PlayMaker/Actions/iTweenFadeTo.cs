using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C94 RID: 3220
	[ActionCategory("iTween")]
	[Tooltip("Changes a GameObject's opacity over time.")]
	public class iTweenFadeTo : iTweenFsmAction
	{
		// Token: 0x060060BA RID: 24762 RVA: 0x001EA6E8 File Offset: 0x001E88E8
		public override void Reset()
		{
			base.Reset();
			this.id = new FsmString
			{
				UseVariable = true
			};
			this.alpha = 0f;
			this.includeChildren = true;
			this.namedValueColor = "_Color";
			this.time = 1f;
			this.delay = 0f;
		}

		// Token: 0x060060BB RID: 24763 RVA: 0x001EA759 File Offset: 0x001E8959
		public override void OnEnter()
		{
			base.OnEnteriTween(this.gameObject);
			if (this.loopType != iTween.LoopType.none)
			{
				base.IsLoop(true);
			}
			this.DoiTween();
		}

		// Token: 0x060060BC RID: 24764 RVA: 0x001EA77C File Offset: 0x001E897C
		public override void OnExit()
		{
			base.OnExitiTween(this.gameObject);
		}

		// Token: 0x060060BD RID: 24765 RVA: 0x001EA78C File Offset: 0x001E898C
		private void DoiTween()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.itweenType = "fade";
			iTween.FadeTo(ownerDefaultTarget, iTween.Hash(new object[]
			{
				"name",
				this.id.IsNone ? "" : this.id.Value,
				"alpha",
				this.alpha.Value,
				"includechildren",
				this.includeChildren.IsNone || this.includeChildren.Value,
				"NamedValueColor",
				this.namedValueColor.Value,
				"time",
				this.time.Value,
				"delay",
				this.delay.IsNone ? 0f : this.delay.Value,
				"easetype",
				this.easeType,
				"looptype",
				this.loopType,
				"oncomplete",
				"iTweenOnComplete",
				"oncompleteparams",
				this.itweenID,
				"onstart",
				"iTweenOnStart",
				"onstartparams",
				this.itweenID,
				"ignoretimescale",
				!this.realTime.IsNone && this.realTime.Value
			}));
		}

		// Token: 0x04005E3F RID: 24127
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005E40 RID: 24128
		[Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
		public FsmString id;

		// Token: 0x04005E41 RID: 24129
		[Tooltip("The end alpha value of the animation.")]
		public FsmFloat alpha;

		// Token: 0x04005E42 RID: 24130
		[Tooltip("Whether or not to include children of this GameObject. True by default.")]
		public FsmBool includeChildren;

		// Token: 0x04005E43 RID: 24131
		[Tooltip("Which color of a shader to use. Uses '_Color' by default.")]
		public FsmString namedValueColor;

		// Token: 0x04005E44 RID: 24132
		[Tooltip("The time in seconds the animation will take to complete.")]
		public FsmFloat time;

		// Token: 0x04005E45 RID: 24133
		[Tooltip("The time in seconds the animation will wait before beginning.")]
		public FsmFloat delay;

		// Token: 0x04005E46 RID: 24134
		[Tooltip("The shape of the easing curve applied to the animation.")]
		public iTween.EaseType easeType = iTween.EaseType.linear;

		// Token: 0x04005E47 RID: 24135
		[Tooltip("The type of loop to apply once the animation has completed.")]
		public iTween.LoopType loopType;
	}
}
