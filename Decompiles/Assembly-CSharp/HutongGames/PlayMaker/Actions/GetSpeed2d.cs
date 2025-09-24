using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FC9 RID: 4041
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Gets the 2d Speed of a Game Object and stores it in a Float Variable. NOTE: The Game Object must have a rigid body 2D.")]
	public class GetSpeed2d : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006F73 RID: 28531 RVA: 0x0022777E File Offset: 0x0022597E
		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006F74 RID: 28532 RVA: 0x00227795 File Offset: 0x00225995
		public override void OnEnter()
		{
			this.DoGetSpeed();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006F75 RID: 28533 RVA: 0x002277AB File Offset: 0x002259AB
		public override void OnUpdate()
		{
			this.DoGetSpeed();
		}

		// Token: 0x06006F76 RID: 28534 RVA: 0x002277B4 File Offset: 0x002259B4
		private void DoGetSpeed()
		{
			if (this.storeResult.IsNone)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			this.storeResult.Value = base.rigidbody2d.linearVelocity.magnitude;
		}

		// Token: 0x04006F43 RID: 28483
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006F44 RID: 28484
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The speed, or in technical terms: velocity magnitude")]
		public FsmFloat storeResult;

		// Token: 0x04006F45 RID: 28485
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
