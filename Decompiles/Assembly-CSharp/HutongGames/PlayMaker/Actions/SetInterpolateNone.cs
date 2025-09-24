using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D30 RID: 3376
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Set rigidbody 2D interpolation mode to Extrapolate")]
	public class SetInterpolateNone : ComponentAction<Rigidbody2D>
	{
		// Token: 0x0600635A RID: 25434 RVA: 0x001F5FC9 File Offset: 0x001F41C9
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x0600635B RID: 25435 RVA: 0x001F5FD2 File Offset: 0x001F41D2
		public override void OnEnter()
		{
			this.DoSetExtrapolate();
			base.Finish();
		}

		// Token: 0x0600635C RID: 25436 RVA: 0x001F5FE0 File Offset: 0x001F41E0
		private void DoSetExtrapolate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			base.rigidbody2d.interpolation = RigidbodyInterpolation2D.None;
		}

		// Token: 0x040061BA RID: 25018
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;
	}
}
