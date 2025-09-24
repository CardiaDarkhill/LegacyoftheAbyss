using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D29 RID: 3369
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Set rigidbody 2D interpolation mode to Extrapolate")]
	public class SetExtrapolate : ComponentAction<Rigidbody2D>
	{
		// Token: 0x0600633E RID: 25406 RVA: 0x001F5BAA File Offset: 0x001F3DAA
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x0600633F RID: 25407 RVA: 0x001F5BB3 File Offset: 0x001F3DB3
		public override void OnEnter()
		{
			this.DoSetExtrapolate();
			base.Finish();
		}

		// Token: 0x06006340 RID: 25408 RVA: 0x001F5BC4 File Offset: 0x001F3DC4
		private void DoSetExtrapolate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			base.rigidbody2d.interpolation = RigidbodyInterpolation2D.Extrapolate;
		}

		// Token: 0x040061A8 RID: 25000
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;
	}
}
