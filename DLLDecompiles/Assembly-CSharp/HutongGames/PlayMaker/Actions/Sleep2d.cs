using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FE3 RID: 4067
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Forces a Game Object's Rigid Body 2D to Sleep at least one frame.")]
	public class Sleep2d : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006FF8 RID: 28664 RVA: 0x00229C7B File Offset: 0x00227E7B
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06006FF9 RID: 28665 RVA: 0x00229C84 File Offset: 0x00227E84
		public override void OnEnter()
		{
			this.DoSleep();
			base.Finish();
		}

		// Token: 0x06006FFA RID: 28666 RVA: 0x00229C94 File Offset: 0x00227E94
		private void DoSleep()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			base.rigidbody2d.Sleep();
		}

		// Token: 0x04006FF6 RID: 28662
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with a Rigidbody2d attached")]
		public FsmOwnerDefault gameObject;
	}
}
