using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C82 RID: 3202
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Gets the 2d Velocity of a Game Object and stores it in a Vector2 Variable or each Axis in a Float Variable. Ignores very low speeds.")]
	public class GetVelocity2dNotZeroV2 : ComponentAction<Rigidbody2D>
	{
		// Token: 0x0600605C RID: 24668 RVA: 0x001E7E38 File Offset: 0x001E6038
		public override void Awake()
		{
			base.Awake();
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600605D RID: 24669 RVA: 0x001E7E4C File Offset: 0x001E604C
		public override void OnPreprocess()
		{
			base.OnPreprocess();
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600605E RID: 24670 RVA: 0x001E7E60 File Offset: 0x001E6060
		public override void Reset()
		{
			this.gameObject = null;
			this.vector = null;
			this.x = null;
			this.y = null;
			this.space = Space.World;
			this.everyFrame = false;
		}

		// Token: 0x0600605F RID: 24671 RVA: 0x001E7E8C File Offset: 0x001E608C
		public override void OnEnter()
		{
			this.DoGetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006060 RID: 24672 RVA: 0x001E7EA2 File Offset: 0x001E60A2
		public override void OnFixedUpdate()
		{
			base.OnFixedUpdate();
			this.DoGetVelocity();
		}

		// Token: 0x06006061 RID: 24673 RVA: 0x001E7EB0 File Offset: 0x001E60B0
		private void DoGetVelocity()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			Vector2 value = this.vector.Value;
			Vector2 vector = base.rigidbody2d.linearVelocity;
			if (this.space == Space.Self)
			{
				vector = base.rigidbody2d.transform.InverseTransformDirection(vector);
			}
			if (vector.x > 0.1f || vector.x < -0.1f)
			{
				this.x.Value = vector.x;
				value.x = vector.x;
			}
			if (vector.y > 0.1f || vector.y < -0.1f)
			{
				this.y.Value = vector.y;
				value.y = vector.y;
			}
			this.vector.Value = value;
		}

		// Token: 0x04005DB1 RID: 23985
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005DB2 RID: 23986
		[UIHint(UIHint.Variable)]
		[Tooltip("The velocity")]
		public FsmVector2 vector;

		// Token: 0x04005DB3 RID: 23987
		[UIHint(UIHint.Variable)]
		[Tooltip("The x value of the velocity")]
		public FsmFloat x;

		// Token: 0x04005DB4 RID: 23988
		[UIHint(UIHint.Variable)]
		[Tooltip("The y value of the velocity")]
		public FsmFloat y;

		// Token: 0x04005DB5 RID: 23989
		[Tooltip("The space reference to express the velocity")]
		public Space space;

		// Token: 0x04005DB6 RID: 23990
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
