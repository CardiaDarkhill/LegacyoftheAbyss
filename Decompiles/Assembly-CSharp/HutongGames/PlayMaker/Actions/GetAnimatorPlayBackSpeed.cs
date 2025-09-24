using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DFF RID: 3583
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the playback speed of the Animator. 1 is normal playback speed")]
	public class GetAnimatorPlayBackSpeed : ComponentAction<Animator>
	{
		// Token: 0x06006750 RID: 26448 RVA: 0x00209D1F File Offset: 0x00207F1F
		public override void Reset()
		{
			this.gameObject = null;
			this.playBackSpeed = null;
			this.everyFrame = false;
		}

		// Token: 0x06006751 RID: 26449 RVA: 0x00209D36 File Offset: 0x00207F36
		public override void OnEnter()
		{
			this.GetPlayBackSpeed();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006752 RID: 26450 RVA: 0x00209D4C File Offset: 0x00207F4C
		public override void OnUpdate()
		{
			this.GetPlayBackSpeed();
		}

		// Token: 0x06006753 RID: 26451 RVA: 0x00209D54 File Offset: 0x00207F54
		private void GetPlayBackSpeed()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.playBackSpeed.Value = this.cachedComponent.speed;
			}
		}

		// Token: 0x0400669F RID: 26271
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066A0 RID: 26272
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The playBack speed of the animator. 1 is normal playback speed")]
		public FsmFloat playBackSpeed;

		// Token: 0x040066A1 RID: 26273
		[Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;
	}
}
