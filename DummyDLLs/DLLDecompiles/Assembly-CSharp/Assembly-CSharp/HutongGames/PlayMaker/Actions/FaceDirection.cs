using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C2E RID: 3118
	[ActionCategory("Enemy AI")]
	[Tooltip("Object will flip to face the direction it is moving on X Axis.")]
	public class FaceDirection : RigidBody2dActionBase
	{
		// Token: 0x06005EDC RID: 24284 RVA: 0x001E0422 File Offset: 0x001DE622
		public override void Reset()
		{
			this.gameObject = null;
			this.spriteFacesRight = false;
			this.everyFrame = false;
			this.playNewAnimation = false;
			this.newAnimationClip = null;
		}

		// Token: 0x06005EDD RID: 24285 RVA: 0x001E044C File Offset: 0x001DE64C
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
			this.DoFace();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005EDE RID: 24286 RVA: 0x001E0515 File Offset: 0x001DE715
		public override void OnUpdate()
		{
			this.DoFace();
		}

		// Token: 0x06005EDF RID: 24287 RVA: 0x001E0520 File Offset: 0x001DE720
		private void DoFace()
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
			float x = vector.x;
			if (this.pauseTimer <= 0f || !this.pauseBetweenTurns)
			{
				if (x > 0f)
				{
					if (this.spriteFacesRight.Value)
					{
						if (localScale.x != this.xScale)
						{
							this.pauseTimer = this.pauseTime.Value;
							localScale.x = this.xScale;
							if (this.playNewAnimation)
							{
								this._sprite.Play(this.newAnimationClip.Value);
								this._sprite.PlayFromFrame(0);
							}
						}
					}
					else if (localScale.x != -this.xScale)
					{
						this.pauseTimer = this.pauseTime.Value;
						localScale.x = -this.xScale;
						if (this.playNewAnimation)
						{
							this._sprite.Play(this.newAnimationClip.Value);
							this._sprite.PlayFromFrame(0);
						}
					}
				}
				else if (x < 0f)
				{
					if (this.spriteFacesRight.Value)
					{
						if (localScale.x != -this.xScale)
						{
							this.pauseTimer = this.pauseTime.Value;
							localScale.x = -this.xScale;
							if (this.playNewAnimation)
							{
								this._sprite.Play(this.newAnimationClip.Value);
								this._sprite.PlayFromFrame(0);
							}
						}
					}
					else if (localScale.x != this.xScale)
					{
						this.pauseTimer = this.pauseTime.Value;
						localScale.x = this.xScale;
						if (this.playNewAnimation)
						{
							this._sprite.Play(this.newAnimationClip.Value);
							this._sprite.PlayFromFrame(0);
						}
					}
				}
			}
			else
			{
				this.pauseTimer -= Time.deltaTime;
			}
			this.target.Value.transform.localScale = new Vector3(localScale.x, this.target.Value.transform.localScale.y, this.target.Value.transform.localScale.z);
		}

		// Token: 0x04005B7E RID: 23422
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005B7F RID: 23423
		[Tooltip("Does the target's sprite face right?")]
		public FsmBool spriteFacesRight;

		// Token: 0x04005B80 RID: 23424
		public bool playNewAnimation;

		// Token: 0x04005B81 RID: 23425
		public FsmString newAnimationClip;

		// Token: 0x04005B82 RID: 23426
		public bool everyFrame;

		// Token: 0x04005B83 RID: 23427
		public bool pauseBetweenTurns;

		// Token: 0x04005B84 RID: 23428
		public FsmFloat pauseTime;

		// Token: 0x04005B85 RID: 23429
		private FsmGameObject target;

		// Token: 0x04005B86 RID: 23430
		private tk2dSpriteAnimator _sprite;

		// Token: 0x04005B87 RID: 23431
		private float xScale;

		// Token: 0x04005B88 RID: 23432
		private Vector2 previousPos;

		// Token: 0x04005B89 RID: 23433
		private float pauseTimer;
	}
}
