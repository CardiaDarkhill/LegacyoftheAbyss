using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D8D RID: 3469
	[ActionCategory("Enemy AI")]
	public class Tk2DAnimationByDirection : RigidBody2dActionBase
	{
		// Token: 0x060064F0 RID: 25840 RVA: 0x001FD8F8 File Offset: 0x001FBAF8
		public override void Reset()
		{
			this.gameObject = null;
			this.spriteFacesRight = false;
			this.everyFrame = false;
		}

		// Token: 0x060064F1 RID: 25841 RVA: 0x001FD914 File Offset: 0x001FBB14
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.target = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this._sprite = this.target.Value.GetComponent<tk2dSpriteAnimator>();
			this.xScale = this.target.Value.transform.localScale.x;
			if (this.xScale < 0f)
			{
				this.xScale *= -1f;
			}
			this.previousPos = this.target.Value.transform.position;
			this._sprite.Play(this.forwardClip.Value);
			this.animatingForward = true;
			this.DoAnim();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060064F2 RID: 25842 RVA: 0x001FD9FA File Offset: 0x001FBBFA
		public override void OnUpdate()
		{
			this.DoAnim();
		}

		// Token: 0x060064F3 RID: 25843 RVA: 0x001FDA04 File Offset: 0x001FBC04
		private void DoAnim()
		{
			if (this.pauseTimer <= 0f || !this.pauseBetweenTurns)
			{
				Vector2 vector;
				if (this.rb2d && this.rb2d.linearVelocity.magnitude > Mathf.Epsilon)
				{
					vector = this.rb2d.linearVelocity;
				}
				else
				{
					Vector2 a = this.target.Value.transform.position;
					vector = (a - this.previousPos) / Time.deltaTime;
					this.previousPos = a;
				}
				Vector3 localScale = this.target.Value.transform.localScale;
				bool flag = true;
				if ((this.spriteFacesRight.Value && localScale.x < 0f) || (!this.spriteFacesRight.Value && localScale.x > 0f))
				{
					flag = false;
				}
				bool flag2 = true;
				if (vector.x < 0f)
				{
					flag2 = false;
				}
				bool flag3 = true;
				if ((flag && !flag2) || (!flag && flag2))
				{
					flag3 = false;
				}
				if (this.faceObject)
				{
					bool flag4 = this.objectToFace.Value.transform.position.x > this.target.Value.transform.position.x;
					if ((flag && !flag4) || (!flag && flag4))
					{
						this.target.Value.transform.localScale = new Vector3(-this.target.Value.transform.localScale.x, this.target.Value.transform.localScale.y, this.target.Value.transform.localScale.z);
						this.pauseTimer = this.pauseTime.Value;
						this._sprite.PlayFromFrame(0);
					}
				}
				if (flag3 && !this.animatingForward)
				{
					this._sprite.Play(this.forwardClip.Value);
					this.animatingForward = true;
					this.pauseTimer = this.pauseTime.Value;
				}
				if (!flag3 && this.animatingForward)
				{
					this._sprite.Play(this.backwardClip.Value);
					this.animatingForward = false;
					this.pauseTimer = this.pauseTime.Value;
					return;
				}
			}
			else if (this.pauseBetweenTurns)
			{
				this.pauseTimer -= Time.deltaTime;
			}
		}

		// Token: 0x040063E8 RID: 25576
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040063E9 RID: 25577
		[Tooltip("Does the target's sprite face right?")]
		public FsmBool spriteFacesRight;

		// Token: 0x040063EA RID: 25578
		public FsmString forwardClip;

		// Token: 0x040063EB RID: 25579
		public FsmString backwardClip;

		// Token: 0x040063EC RID: 25580
		public bool faceObject;

		// Token: 0x040063ED RID: 25581
		public FsmGameObject objectToFace;

		// Token: 0x040063EE RID: 25582
		public bool everyFrame;

		// Token: 0x040063EF RID: 25583
		public bool pauseBetweenTurns;

		// Token: 0x040063F0 RID: 25584
		public FsmFloat pauseTime;

		// Token: 0x040063F1 RID: 25585
		private FsmGameObject target;

		// Token: 0x040063F2 RID: 25586
		private tk2dSpriteAnimator _sprite;

		// Token: 0x040063F3 RID: 25587
		private float xScale;

		// Token: 0x040063F4 RID: 25588
		private Vector2 previousPos;

		// Token: 0x040063F5 RID: 25589
		private float pauseTimer;

		// Token: 0x040063F6 RID: 25590
		private bool animatingForward;
	}
}
