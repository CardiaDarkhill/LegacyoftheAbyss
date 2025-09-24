using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FDF RID: 4063
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Sets the 2d Velocity of a Game Object. To leave any axis unchanged, set variable to 'None'. NOTE: Game object must have a rigidbody 2D.")]
	public class SetVelocity2d : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006FDF RID: 28639 RVA: 0x00229413 File Offset: 0x00227613
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

		// Token: 0x06006FE0 RID: 28640 RVA: 0x0022944E File Offset: 0x0022764E
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006FE1 RID: 28641 RVA: 0x0022945C File Offset: 0x0022765C
		public override void OnEnter()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006FE2 RID: 28642 RVA: 0x00229472 File Offset: 0x00227672
		public override void OnFixedUpdate()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006FE3 RID: 28643 RVA: 0x00229488 File Offset: 0x00227688
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

		// Token: 0x04006FD6 RID: 28630
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006FD7 RID: 28631
		[Tooltip("A Vector2 value for the velocity")]
		public FsmVector2 vector;

		// Token: 0x04006FD8 RID: 28632
		[Tooltip("The y value of the velocity. Overrides 'Vector' x value if set")]
		public FsmFloat x;

		// Token: 0x04006FD9 RID: 28633
		[Tooltip("The y value of the velocity. Overrides 'Vector' y value if set")]
		public FsmFloat y;

		// Token: 0x04006FDA RID: 28634
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
