using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DB1 RID: 3505
	[ActionCategory("Physics 2d")]
	[Tooltip("Gets the 2d Velocity of a Game Object and stores it in a Vector2 Variable or each Axis in a Float Variable. NOTE: The Game Object must have a Rigid Body 2D.")]
	public class GetVelocityAsAngle : RigidBody2dActionBase
	{
		// Token: 0x060065B5 RID: 26037 RVA: 0x00201811 File Offset: 0x001FFA11
		public override void Reset()
		{
			this.storeAngle = null;
			this.everyFrame = false;
		}

		// Token: 0x060065B6 RID: 26038 RVA: 0x00201821 File Offset: 0x001FFA21
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.DoGetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060065B7 RID: 26039 RVA: 0x0020184E File Offset: 0x001FFA4E
		public override void OnUpdate()
		{
			this.DoGetVelocity();
		}

		// Token: 0x060065B8 RID: 26040 RVA: 0x00201858 File Offset: 0x001FFA58
		private void DoGetVelocity()
		{
			if (this.rb2d == null)
			{
				return;
			}
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			float num = Mathf.Atan2(linearVelocity.x, -linearVelocity.y) * 180f / 3.1415927f - 90f;
			if (num < 0f)
			{
				num += 360f;
			}
			this.storeAngle.Value = num;
		}

		// Token: 0x040064DA RID: 25818
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040064DB RID: 25819
		[UIHint(UIHint.Variable)]
		public FsmFloat storeAngle;

		// Token: 0x040064DC RID: 25820
		public bool everyFrame;
	}
}
