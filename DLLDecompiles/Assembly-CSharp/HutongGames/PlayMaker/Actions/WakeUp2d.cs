using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FED RID: 4077
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Forces a Game Object's Rigid Body 2D to wake up.")]
	public class WakeUp2d : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06007064 RID: 28772 RVA: 0x0022B95D File Offset: 0x00229B5D
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06007065 RID: 28773 RVA: 0x0022B966 File Offset: 0x00229B66
		public override void OnEnter()
		{
			this.DoWakeUp();
			base.Finish();
		}

		// Token: 0x06007066 RID: 28774 RVA: 0x0022B974 File Offset: 0x00229B74
		private void DoWakeUp()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			base.rigidbody2d.WakeUp();
		}

		// Token: 0x04007043 RID: 28739
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with a Rigidbody2d attached")]
		public FsmOwnerDefault gameObject;
	}
}
