using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DB4 RID: 3508
	[ActionCategory("Physics 2d")]
	[Tooltip("Sets the 2d Velocity of a Game Object. To leave any axis unchanged, set variable to 'None'. NOTE: Game object must have a rigidbody 2D. If the specified bool is true, ignore.")]
	public class SetVelocity2dIfFalse : RigidBody2dActionBase
	{
		// Token: 0x060065C0 RID: 26048 RVA: 0x00201ABC File Offset: 0x001FFCBC
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
			this.checkBool = new FsmBool
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x060065C1 RID: 26049 RVA: 0x00201B14 File Offset: 0x001FFD14
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x060065C2 RID: 26050 RVA: 0x00201B22 File Offset: 0x001FFD22
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060065C3 RID: 26051 RVA: 0x00201B4F File Offset: 0x001FFD4F
		public override void OnFixedUpdate()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060065C4 RID: 26052 RVA: 0x00201B68 File Offset: 0x001FFD68
		private void DoSetVelocity()
		{
			if (this.rb2d == null)
			{
				return;
			}
			if (!this.checkBool.Value)
			{
				Vector2 linearVelocity;
				if (this.vector.IsNone)
				{
					linearVelocity = this.rb2d.linearVelocity;
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
				this.rb2d.linearVelocity = linearVelocity;
				this.rb2d.angularVelocity = 3f;
			}
		}

		// Token: 0x040064E1 RID: 25825
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040064E2 RID: 25826
		[UIHint(UIHint.Variable)]
		public FsmVector2 vector;

		// Token: 0x040064E3 RID: 25827
		public FsmFloat x;

		// Token: 0x040064E4 RID: 25828
		public FsmFloat y;

		// Token: 0x040064E5 RID: 25829
		public FsmBool checkBool;

		// Token: 0x040064E6 RID: 25830
		public bool everyFrame;
	}
}
