using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D5C RID: 3420
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Sets the 2d Velocity of a Game Object. To leave any axis unchanged, set variable to 'None'. NOTE: Game object must have a rigidbody 2D.")]
	public class SetVelocity2dBool : ComponentAction<Rigidbody2D>
	{
		// Token: 0x0600640D RID: 25613 RVA: 0x001F865C File Offset: 0x001F685C
		public override void Reset()
		{
			this.gameObject = null;
			this.vector = null;
			this.activeBool = new FsmBool
			{
				UseVariable = true
			};
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

		// Token: 0x0600640E RID: 25614 RVA: 0x001F86B4 File Offset: 0x001F68B4
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600640F RID: 25615 RVA: 0x001F86C2 File Offset: 0x001F68C2
		public override void OnEnter()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006410 RID: 25616 RVA: 0x001F86D8 File Offset: 0x001F68D8
		public override void OnFixedUpdate()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006411 RID: 25617 RVA: 0x001F86F0 File Offset: 0x001F68F0
		private void DoSetVelocity()
		{
			if (this.activeBool.Value || this.activeBool.IsNone)
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
		}

		// Token: 0x0400627B RID: 25211
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400627C RID: 25212
		public FsmBool activeBool;

		// Token: 0x0400627D RID: 25213
		[Tooltip("A Vector2 value for the velocity")]
		public FsmVector2 vector;

		// Token: 0x0400627E RID: 25214
		[Tooltip("The y value of the velocity. Overrides 'Vector' x value if set")]
		public FsmFloat x;

		// Token: 0x0400627F RID: 25215
		[Tooltip("The y value of the velocity. Overrides 'Vector' y value if set")]
		public FsmFloat y;

		// Token: 0x04006280 RID: 25216
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
