using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DDC RID: 3548
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Plays a state. This could be used to synchronize your animation with audio or synchronize an Animator over the network.")]
	public class AnimatorPlay : ComponentAction<Animator>
	{
		// Token: 0x17000BC9 RID: 3017
		// (get) Token: 0x060066A6 RID: 26278 RVA: 0x002081E7 File Offset: 0x002063E7
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x060066A7 RID: 26279 RVA: 0x002081EF File Offset: 0x002063EF
		public override void Reset()
		{
			this.gameObject = null;
			this.stateName = null;
			this.layer = new FsmInt
			{
				UseVariable = true
			};
			this.normalizedTime = new FsmFloat
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x060066A8 RID: 26280 RVA: 0x0020822A File Offset: 0x0020642A
		public override void OnEnter()
		{
			this.DoAnimatorPlay();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060066A9 RID: 26281 RVA: 0x00208240 File Offset: 0x00206440
		public override void OnUpdate()
		{
			this.DoAnimatorPlay();
		}

		// Token: 0x060066AA RID: 26282 RVA: 0x00208248 File Offset: 0x00206448
		private void DoAnimatorPlay()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			int num = this.layer.IsNone ? -1 : this.layer.Value;
			float num2 = this.normalizedTime.IsNone ? float.NegativeInfinity : this.normalizedTime.Value;
			this.animator.Play(this.stateName.Value, num, num2);
		}

		// Token: 0x04006603 RID: 26115
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006604 RID: 26116
		[Tooltip("The name of the state that will be played.")]
		public FsmString stateName;

		// Token: 0x04006605 RID: 26117
		[Tooltip("The layer where the state is.")]
		public FsmInt layer;

		// Token: 0x04006606 RID: 26118
		[Tooltip("The normalized time at which the state will play")]
		public FsmFloat normalizedTime;

		// Token: 0x04006607 RID: 26119
		[Tooltip("Repeat every frame. Useful when using normalizedTime to manually control the animation.")]
		public bool everyFrame;
	}
}
