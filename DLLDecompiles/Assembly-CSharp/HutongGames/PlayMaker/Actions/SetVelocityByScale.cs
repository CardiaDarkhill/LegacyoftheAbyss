using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D5F RID: 3423
	[ActionCategory(ActionCategory.Physics2D)]
	public class SetVelocityByScale : ComponentAction<Rigidbody2D>
	{
		// Token: 0x0600641E RID: 25630 RVA: 0x001F89BE File Offset: 0x001F6BBE
		public override void Reset()
		{
			this.gameObject = null;
			this.speed = null;
			this.ySpeed = new FsmFloat
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x0600641F RID: 25631 RVA: 0x001F89E7 File Offset: 0x001F6BE7
		public override void OnEnter()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006420 RID: 25632 RVA: 0x001F89FD File Offset: 0x001F6BFD
		public override void OnUpdate()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006421 RID: 25633 RVA: 0x001F8A14 File Offset: 0x001F6C14
		private void DoSetVelocity()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			Vector2 linearVelocity = base.rigidbody2d.linearVelocity;
			if (ownerDefaultTarget.transform.localScale.x > 0f)
			{
				linearVelocity.x = this.speed.Value;
			}
			else
			{
				linearVelocity.x = -this.speed.Value;
			}
			if (!this.ySpeed.IsNone)
			{
				linearVelocity.y = this.ySpeed.Value;
			}
			base.rigidbody2d.linearVelocity = linearVelocity;
		}

		// Token: 0x0400628A RID: 25226
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400628B RID: 25227
		public FsmFloat speed;

		// Token: 0x0400628C RID: 25228
		public FsmFloat ySpeed;

		// Token: 0x0400628D RID: 25229
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
