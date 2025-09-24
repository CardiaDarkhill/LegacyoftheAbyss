using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FCB RID: 4043
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Gets the 2d Velocity of a Game Object and stores it in a Vector2 Variable or each Axis in a Float Variable. NOTE: The Game Object must have a Rigid Body 2D.")]
	public class GetVelocity2d : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006F7C RID: 28540 RVA: 0x002278D4 File Offset: 0x00225AD4
		public override void Reset()
		{
			this.gameObject = null;
			this.vector = null;
			this.x = null;
			this.y = null;
			this.space = Space.World;
			this.everyFrame = false;
		}

		// Token: 0x06006F7D RID: 28541 RVA: 0x00227900 File Offset: 0x00225B00
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006F7E RID: 28542 RVA: 0x0022790E File Offset: 0x00225B0E
		public override void OnEnter()
		{
			this.DoGetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006F7F RID: 28543 RVA: 0x00227924 File Offset: 0x00225B24
		public override void OnUpdate()
		{
			this.DoGetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006F80 RID: 28544 RVA: 0x0022793A File Offset: 0x00225B3A
		public override void OnFixedUpdate()
		{
			this.DoGetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006F81 RID: 28545 RVA: 0x00227950 File Offset: 0x00225B50
		private void DoGetVelocity()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			Vector2 vector = base.rigidbody2d.linearVelocity;
			if (this.space == Space.Self)
			{
				vector = base.rigidbody2d.transform.InverseTransformDirection(vector);
			}
			this.vector.Value = vector;
			this.x.Value = vector.x;
			this.y.Value = vector.y;
		}

		// Token: 0x04006F49 RID: 28489
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006F4A RID: 28490
		[UIHint(UIHint.Variable)]
		[Tooltip("The velocity")]
		public FsmVector2 vector;

		// Token: 0x04006F4B RID: 28491
		[UIHint(UIHint.Variable)]
		[Tooltip("The x value of the velocity")]
		public FsmFloat x;

		// Token: 0x04006F4C RID: 28492
		[UIHint(UIHint.Variable)]
		[Tooltip("The y value of the velocity")]
		public FsmFloat y;

		// Token: 0x04006F4D RID: 28493
		[Tooltip("The space reference to express the velocity")]
		public Space space;

		// Token: 0x04006F4E RID: 28494
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
