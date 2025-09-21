using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E15 RID: 3605
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets an AvatarTarget and a targetNormalizedTime for the current state")]
	public class SetAnimatorTarget : ComponentAction<Animator>
	{
		// Token: 0x060067C0 RID: 26560 RVA: 0x0020AFA8 File Offset: 0x002091A8
		public override void Reset()
		{
			this.gameObject = null;
			this.avatarTarget = AvatarTarget.Body;
			this.targetNormalizedTime = null;
			this.everyFrame = false;
		}

		// Token: 0x060067C1 RID: 26561 RVA: 0x0020AFC6 File Offset: 0x002091C6
		public override void OnPreprocess()
		{
			base.Fsm.HandleAnimatorMove = true;
		}

		// Token: 0x060067C2 RID: 26562 RVA: 0x0020AFD4 File Offset: 0x002091D4
		public override void OnEnter()
		{
			this.SetTarget();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060067C3 RID: 26563 RVA: 0x0020AFEA File Offset: 0x002091EA
		public override void DoAnimatorMove()
		{
			this.SetTarget();
		}

		// Token: 0x060067C4 RID: 26564 RVA: 0x0020AFF2 File Offset: 0x002091F2
		private void SetTarget()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.cachedComponent.SetTarget(this.avatarTarget, this.targetNormalizedTime.Value);
			}
		}

		// Token: 0x040066FE RID: 26366
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with the Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066FF RID: 26367
		[Tooltip("The avatar target")]
		public AvatarTarget avatarTarget;

		// Token: 0x04006700 RID: 26368
		[Tooltip("The current state Time that is queried")]
		public FsmFloat targetNormalizedTime;

		// Token: 0x04006701 RID: 26369
		[Tooltip("Repeat every frame during OnAnimatorMove. Useful when changing over time.")]
		public bool everyFrame;
	}
}
