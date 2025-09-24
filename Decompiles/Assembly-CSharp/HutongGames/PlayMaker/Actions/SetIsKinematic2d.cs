using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FDD RID: 4061
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Controls whether 2D physics affects the Game Object.")]
	public class SetIsKinematic2d : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006FD7 RID: 28631 RVA: 0x00229338 File Offset: 0x00227538
		public override void Reset()
		{
			this.gameObject = null;
			this.isKinematic = false;
		}

		// Token: 0x06006FD8 RID: 28632 RVA: 0x0022934D File Offset: 0x0022754D
		public override void OnEnter()
		{
			this.DoSetIsKinematic();
			base.Finish();
		}

		// Token: 0x06006FD9 RID: 28633 RVA: 0x0022935C File Offset: 0x0022755C
		private void DoSetIsKinematic()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			base.rigidbody2d.isKinematic = this.isKinematic.Value;
		}

		// Token: 0x04006FD2 RID: 28626
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006FD3 RID: 28627
		[RequiredField]
		[Tooltip("The isKinematic value")]
		public FsmBool isKinematic;
	}
}
