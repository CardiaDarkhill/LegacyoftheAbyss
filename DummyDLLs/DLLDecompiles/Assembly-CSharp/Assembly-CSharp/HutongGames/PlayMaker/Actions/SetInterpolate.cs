using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D2F RID: 3375
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Set rigidbody 2D interpolation mode to Interpolate")]
	public class SetInterpolate : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006356 RID: 25430 RVA: 0x001F5F72 File Offset: 0x001F4172
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06006357 RID: 25431 RVA: 0x001F5F7B File Offset: 0x001F417B
		public override void OnEnter()
		{
			this.DoSetInterpolate();
			base.Finish();
		}

		// Token: 0x06006358 RID: 25432 RVA: 0x001F5F8C File Offset: 0x001F418C
		private void DoSetInterpolate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			base.rigidbody2d.interpolation = RigidbodyInterpolation2D.Interpolate;
		}

		// Token: 0x040061B9 RID: 25017
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;
	}
}
