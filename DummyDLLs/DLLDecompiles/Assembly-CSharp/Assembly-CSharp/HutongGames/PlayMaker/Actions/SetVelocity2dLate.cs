using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D5D RID: 3421
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Sets the 2d Velocity of a Game Object. To leave any axis unchanged, set variable to 'None'. NOTE: Game object must have a rigidbody 2D.")]
	public class SetVelocity2dLate : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006413 RID: 25619 RVA: 0x001F87AF File Offset: 0x001F69AF
		public override void Reset()
		{
			this.gameObject = null;
			this.vector = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x06006414 RID: 25620 RVA: 0x001F87EA File Offset: 0x001F69EA
		public override void OnLateUpdate()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006415 RID: 25621 RVA: 0x001F8800 File Offset: 0x001F6A00
		private void DoSetVelocity()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			Vector2 linearVelocity;
			if (this.vector.IsNone)
			{
				linearVelocity = base.rigidbody2d.linearVelocity;
			}
			else
			{
				linearVelocity = this.vector.Value;
			}
			if (!this.x.IsNone)
			{
				linearVelocity.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				linearVelocity.y = this.y.Value;
			}
			base.rigidbody2d.linearVelocity = linearVelocity;
		}

		// Token: 0x04006281 RID: 25217
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006282 RID: 25218
		[Tooltip("A Vector2 value for the velocity")]
		public FsmVector2 vector;

		// Token: 0x04006283 RID: 25219
		[Tooltip("The y value of the velocity. Overrides 'Vector' x value if set")]
		public FsmFloat x;

		// Token: 0x04006284 RID: 25220
		[Tooltip("The y value of the velocity. Overrides 'Vector' y value if set")]
		public FsmFloat y;

		// Token: 0x04006285 RID: 25221
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
