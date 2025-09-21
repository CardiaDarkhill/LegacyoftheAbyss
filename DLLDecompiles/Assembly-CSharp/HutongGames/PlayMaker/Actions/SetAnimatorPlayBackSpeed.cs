using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E11 RID: 3601
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets the playback speed of the Animator. 1 is normal playback speed")]
	public class SetAnimatorPlayBackSpeed : ComponentAction<Animator>
	{
		// Token: 0x060067AE RID: 26542 RVA: 0x0020AE0F File Offset: 0x0020900F
		public override void Reset()
		{
			this.gameObject = null;
			this.playBackSpeed = null;
			this.everyFrame = false;
		}

		// Token: 0x060067AF RID: 26543 RVA: 0x0020AE26 File Offset: 0x00209026
		public override void OnEnter()
		{
			this.DoPlayBackSpeed();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060067B0 RID: 26544 RVA: 0x0020AE3C File Offset: 0x0020903C
		public override void OnUpdate()
		{
			this.DoPlayBackSpeed();
		}

		// Token: 0x060067B1 RID: 26545 RVA: 0x0020AE44 File Offset: 0x00209044
		private void DoPlayBackSpeed()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.cachedComponent.speed = this.playBackSpeed.Value;
			}
		}

		// Token: 0x040066F3 RID: 26355
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066F4 RID: 26356
		[Tooltip("If true, automatically stabilize feet during transition and blending")]
		public FsmFloat playBackSpeed;

		// Token: 0x040066F5 RID: 26357
		[Tooltip("Repeat every frame. Useful for changing over time.")]
		public bool everyFrame;
	}
}
