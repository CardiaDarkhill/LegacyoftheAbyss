using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E03 RID: 3587
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the playback speed of the Animator. 1 is normal playback speed")]
	public class GetAnimatorSpeed : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BDD RID: 3037
		// (get) Token: 0x06006766 RID: 26470 RVA: 0x00209FAE File Offset: 0x002081AE
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x06006767 RID: 26471 RVA: 0x00209FB6 File Offset: 0x002081B6
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.speed = null;
			this.everyFrame = false;
		}

		// Token: 0x06006768 RID: 26472 RVA: 0x00209FD3 File Offset: 0x002081D3
		public override void OnEnter()
		{
			this.GetPlaybackSpeed();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006769 RID: 26473 RVA: 0x00209FE9 File Offset: 0x002081E9
		public override void OnActionUpdate()
		{
			this.GetPlaybackSpeed();
		}

		// Token: 0x0600676A RID: 26474 RVA: 0x00209FF1 File Offset: 0x002081F1
		private void GetPlaybackSpeed()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			this.speed.Value = this.animator.speed;
		}

		// Token: 0x040066AE RID: 26286
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066AF RID: 26287
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The playBack speed of the animator. 1 is normal playback speed")]
		public FsmFloat speed;
	}
}
