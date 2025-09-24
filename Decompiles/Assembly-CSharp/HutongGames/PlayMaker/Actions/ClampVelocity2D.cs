using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BF7 RID: 3063
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Sets the 2d Velocity of a Game Object. To leave any axis unchanged, set variable to 'None'. NOTE: Game object must have a rigidbody 2D.")]
	public class ClampVelocity2D : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06005DB1 RID: 23985 RVA: 0x001D8BD3 File Offset: 0x001D6DD3
		public override void Reset()
		{
			this.gameObject = null;
			this.xMin = null;
			this.xMax = null;
			this.yMin = null;
			this.yMax = null;
			this.everyFrame = false;
		}

		// Token: 0x06005DB2 RID: 23986 RVA: 0x001D8BFF File Offset: 0x001D6DFF
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005DB3 RID: 23987 RVA: 0x001D8C0D File Offset: 0x001D6E0D
		public override void OnEnter()
		{
			this.DoClampVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005DB4 RID: 23988 RVA: 0x001D8C23 File Offset: 0x001D6E23
		public override void OnFixedUpdate()
		{
			this.DoClampVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005DB5 RID: 23989 RVA: 0x001D8C3C File Offset: 0x001D6E3C
		private void DoClampVelocity()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			Vector2 linearVelocity = base.rigidbody2d.linearVelocity;
			if (!this.xMin.IsNone && linearVelocity.x < this.xMin.Value)
			{
				linearVelocity.x = this.xMin.Value;
			}
			else if (!this.xMax.IsNone && linearVelocity.x > this.xMax.Value)
			{
				linearVelocity.x = this.xMax.Value;
			}
			if (!this.yMin.IsNone && linearVelocity.y < this.yMin.Value)
			{
				linearVelocity.y = this.yMin.Value;
			}
			else if (!this.yMax.IsNone && linearVelocity.y > this.yMax.Value)
			{
				linearVelocity.y = this.yMax.Value;
			}
			base.rigidbody2d.linearVelocity = linearVelocity;
		}

		// Token: 0x04005A03 RID: 23043
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005A04 RID: 23044
		public FsmFloat xMin;

		// Token: 0x04005A05 RID: 23045
		public FsmFloat xMax;

		// Token: 0x04005A06 RID: 23046
		public FsmFloat yMin;

		// Token: 0x04005A07 RID: 23047
		public FsmFloat yMax;

		// Token: 0x04005A08 RID: 23048
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
