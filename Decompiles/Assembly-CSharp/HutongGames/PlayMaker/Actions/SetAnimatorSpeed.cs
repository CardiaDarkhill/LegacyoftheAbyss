using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E13 RID: 3603
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets the playback speed of the Animator. 1 is normal playback speed")]
	public class SetAnimatorSpeed : ComponentAction<Animator>
	{
		// Token: 0x060067B8 RID: 26552 RVA: 0x0020AEEB File Offset: 0x002090EB
		public override void Reset()
		{
			this.gameObject = null;
			this.speed = null;
			this.everyFrame = false;
		}

		// Token: 0x060067B9 RID: 26553 RVA: 0x0020AF02 File Offset: 0x00209102
		public override void OnEnter()
		{
			this.DoPlaybackSpeed();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060067BA RID: 26554 RVA: 0x0020AF18 File Offset: 0x00209118
		public override void OnUpdate()
		{
			this.DoPlaybackSpeed();
		}

		// Token: 0x060067BB RID: 26555 RVA: 0x0020AF20 File Offset: 0x00209120
		private void DoPlaybackSpeed()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.cachedComponent.speed = this.speed.Value;
			}
		}

		// Token: 0x040066F9 RID: 26361
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066FA RID: 26362
		[Tooltip("The playback speed.")]
		public FsmFloat speed;

		// Token: 0x040066FB RID: 26363
		[Tooltip("Repeat every frame. Useful for changing over time.")]
		public bool everyFrame;
	}
}
