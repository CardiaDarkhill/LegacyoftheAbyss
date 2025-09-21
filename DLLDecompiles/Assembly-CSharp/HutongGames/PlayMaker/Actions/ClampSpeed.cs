using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BF6 RID: 3062
	[ActionCategory(ActionCategory.Physics2D)]
	public class ClampSpeed : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06005DAC RID: 23980 RVA: 0x001D8AC4 File Offset: 0x001D6CC4
		public override void Reset()
		{
			this.gameObject = null;
			this.speedMin = new FsmFloat
			{
				UseVariable = true
			};
			this.speedMax = new FsmFloat
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x06005DAD RID: 23981 RVA: 0x001D8AF8 File Offset: 0x001D6CF8
		public override void OnEnter()
		{
			this.DoClampSpeed();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005DAE RID: 23982 RVA: 0x001D8B0E File Offset: 0x001D6D0E
		public override void OnUpdate()
		{
			this.DoClampSpeed();
		}

		// Token: 0x06005DAF RID: 23983 RVA: 0x001D8B18 File Offset: 0x001D6D18
		private void DoClampSpeed()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			Vector2 linearVelocity = base.rigidbody2d.linearVelocity;
			if (!this.speedMin.IsNone && linearVelocity.magnitude < this.speedMin.Value)
			{
				linearVelocity = linearVelocity.normalized * this.speedMin.Value;
			}
			if (!this.speedMax.IsNone && linearVelocity.magnitude > this.speedMax.Value)
			{
				linearVelocity = linearVelocity.normalized * this.speedMax.Value;
			}
			base.rigidbody2d.linearVelocity = linearVelocity;
		}

		// Token: 0x040059FF RID: 23039
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005A00 RID: 23040
		public FsmFloat speedMin;

		// Token: 0x04005A01 RID: 23041
		public FsmFloat speedMax;

		// Token: 0x04005A02 RID: 23042
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
