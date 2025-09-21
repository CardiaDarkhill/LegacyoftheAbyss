using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C32 RID: 3122
	[ActionCategory("Enemy AI")]
	[Tooltip("Play a different anim clip depending on target direction")]
	public class FaceObjectAnim : FsmStateAction
	{
		// Token: 0x06005EF3 RID: 24307 RVA: 0x001E104A File Offset: 0x001DF24A
		public override void Reset()
		{
			this.turningObject = null;
			this.target = null;
			this.clipL = null;
			this.clipR = null;
			this.startFacingRight = false;
			this.everyFrame = false;
		}

		// Token: 0x06005EF4 RID: 24308 RVA: 0x001E1078 File Offset: 0x001DF278
		public override void OnEnter()
		{
			this.turner = base.Fsm.GetOwnerDefaultTarget(this.turningObject);
			this._sprite = this.turner.GetComponent<tk2dSpriteAnimator>();
			this.facingRight = this.startFacingRight;
			if (this._sprite == null)
			{
				base.Finish();
			}
			this.DoFace();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005EF5 RID: 24309 RVA: 0x001E10E1 File Offset: 0x001DF2E1
		public override void OnUpdate()
		{
			this.DoFace();
		}

		// Token: 0x06005EF6 RID: 24310 RVA: 0x001E10EC File Offset: 0x001DF2EC
		private void DoFace()
		{
			bool flag = this.target.Value.gameObject.transform.position.x > this.turner.transform.position.x;
			if (!this.facingRight && flag)
			{
				this._sprite.PlayFromFrame(this.clipR.Value, 0);
				this.facingRight = true;
				return;
			}
			if (this.facingRight && !flag)
			{
				this._sprite.PlayFromFrame(this.clipL.Value, 0);
				this.facingRight = false;
			}
		}

		// Token: 0x04005BAC RID: 23468
		[RequiredField]
		public FsmOwnerDefault turningObject;

		// Token: 0x04005BAD RID: 23469
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		// Token: 0x04005BAE RID: 23470
		public FsmString clipL;

		// Token: 0x04005BAF RID: 23471
		public FsmString clipR;

		// Token: 0x04005BB0 RID: 23472
		public bool startFacingRight = true;

		// Token: 0x04005BB1 RID: 23473
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04005BB2 RID: 23474
		private bool facingRight;

		// Token: 0x04005BB3 RID: 23475
		private tk2dSpriteAnimator _sprite;

		// Token: 0x04005BB4 RID: 23476
		private GameObject turner;
	}
}
