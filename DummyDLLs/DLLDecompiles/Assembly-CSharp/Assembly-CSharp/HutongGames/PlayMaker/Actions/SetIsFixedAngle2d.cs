using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FDC RID: 4060
	[Obsolete("This action is obsolete; use Constraints instead.")]
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Controls whether the rigidbody 2D should be prevented from rotating")]
	public class SetIsFixedAngle2d : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006FD2 RID: 28626 RVA: 0x0022928E File Offset: 0x0022748E
		public override void Reset()
		{
			this.gameObject = null;
			this.isFixedAngle = false;
			this.everyFrame = false;
		}

		// Token: 0x06006FD3 RID: 28627 RVA: 0x002292AA File Offset: 0x002274AA
		public override void OnEnter()
		{
			this.DoSetIsFixedAngle();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006FD4 RID: 28628 RVA: 0x002292C0 File Offset: 0x002274C0
		public override void OnUpdate()
		{
			this.DoSetIsFixedAngle();
		}

		// Token: 0x06006FD5 RID: 28629 RVA: 0x002292C8 File Offset: 0x002274C8
		private void DoSetIsFixedAngle()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			if (this.isFixedAngle.Value)
			{
				base.rigidbody2d.constraints = (base.rigidbody2d.constraints | RigidbodyConstraints2D.FreezeRotation);
				return;
			}
			base.rigidbody2d.constraints = (base.rigidbody2d.constraints & ~RigidbodyConstraints2D.FreezeRotation);
		}

		// Token: 0x04006FCF RID: 28623
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006FD0 RID: 28624
		[RequiredField]
		[Tooltip("The flag value")]
		public FsmBool isFixedAngle;

		// Token: 0x04006FD1 RID: 28625
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
