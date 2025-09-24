using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C84 RID: 3204
	[ActionCategory("Enemy AI")]
	[Tooltip("Object A will flip to face Object B horizontally.")]
	public class GrimmChildFly : FsmStateAction
	{
		// Token: 0x0600606A RID: 24682 RVA: 0x001E82A0 File Offset: 0x001E64A0
		public override void Reset()
		{
			this.objectA = null;
			this.objectB = null;
			this.newAnimationClip = null;
			this.spriteFacesRight = false;
			this.resetFrame = false;
			this.playNewAnimation = false;
			this.flyingFast = false;
			this.pauseBetweenAnimChange = null;
			this.timer = 0f;
		}

		// Token: 0x0600606B RID: 24683 RVA: 0x001E82F8 File Offset: 0x001E64F8
		public override void OnEnter()
		{
			this._sprite = this.objectA.Value.GetComponent<tk2dSpriteAnimator>();
			this.rb2d = this.objectA.Value.GetComponent<Rigidbody2D>();
			this.xScale = this.objectA.Value.transform.localScale.x;
			if (this.xScale < 0f)
			{
				this.xScale *= -1f;
			}
			this.DoFace();
		}

		// Token: 0x0600606C RID: 24684 RVA: 0x001E8376 File Offset: 0x001E6576
		public override void OnUpdate()
		{
			this.DoFace();
		}

		// Token: 0x0600606D RID: 24685 RVA: 0x001E8380 File Offset: 0x001E6580
		private void DoFace()
		{
			Vector3 localScale = this.objectA.Value.transform.localScale;
			if (this.objectA.Value.transform.position.x < this.objectB.Value.transform.position.x)
			{
				if (this.spriteFacesRight.Value)
				{
					if (localScale.x != this.xScale)
					{
						localScale.x = this.xScale;
						if (this.resetFrame)
						{
							this._sprite.PlayFromFrame(0);
						}
						if (this.playNewAnimation)
						{
							this._sprite.Play(this.newAnimationClip.Value);
							this.flyingFast = false;
						}
					}
				}
				else if (localScale.x != -this.xScale)
				{
					localScale.x = -this.xScale;
					if (this.resetFrame)
					{
						this._sprite.PlayFromFrame(0);
					}
					if (this.playNewAnimation)
					{
						this._sprite.Play(this.newAnimationClip.Value);
						this.flyingFast = false;
					}
				}
			}
			else if (this.spriteFacesRight.Value)
			{
				if (localScale.x != -this.xScale)
				{
					localScale.x = -this.xScale;
					if (this.resetFrame)
					{
						this._sprite.PlayFromFrame(0);
					}
					if (this.playNewAnimation)
					{
						this._sprite.Play(this.newAnimationClip.Value);
						this.flyingFast = false;
					}
				}
			}
			else if (localScale.x != this.xScale)
			{
				localScale.x = this.xScale;
				if (this.resetFrame)
				{
					this._sprite.PlayFromFrame(0);
				}
				if (this.playNewAnimation)
				{
					this._sprite.Play(this.newAnimationClip.Value);
					this.flyingFast = false;
				}
			}
			if (!this.flyingFast && this.timer <= 0f && (this.rb2d.linearVelocity.x > this.fastAnimSpeed.Value || this.rb2d.linearVelocity.x < -this.fastAnimSpeed.Value))
			{
				this.flyingFast = true;
				this._sprite.Play(this.fastAnimationClip.Value);
				this.timer = this.pauseBetweenAnimChange.Value;
			}
			if (this.flyingFast && this.timer <= 0f && this.rb2d.linearVelocity.x < this.fastAnimSpeed.Value && this.rb2d.linearVelocity.x > -this.fastAnimSpeed.Value)
			{
				this.flyingFast = false;
				this._sprite.Play(this.normalAnimationClip.Value);
				this.timer = this.pauseBetweenAnimChange.Value;
			}
			if (this.timer > 0f)
			{
				this.timer -= Time.deltaTime;
			}
			this.objectA.Value.transform.localScale = new Vector3(localScale.x, this.objectA.Value.transform.localScale.y, this.objectA.Value.transform.localScale.z);
		}

		// Token: 0x04005DC4 RID: 24004
		[RequiredField]
		public FsmGameObject objectA;

		// Token: 0x04005DC5 RID: 24005
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject objectB;

		// Token: 0x04005DC6 RID: 24006
		[Tooltip("Does object A's sprite face right?")]
		public FsmBool spriteFacesRight;

		// Token: 0x04005DC7 RID: 24007
		public bool playNewAnimation;

		// Token: 0x04005DC8 RID: 24008
		public FsmString newAnimationClip;

		// Token: 0x04005DC9 RID: 24009
		public bool resetFrame = true;

		// Token: 0x04005DCA RID: 24010
		public FsmFloat fastAnimSpeed;

		// Token: 0x04005DCB RID: 24011
		public FsmString fastAnimationClip;

		// Token: 0x04005DCC RID: 24012
		public FsmString normalAnimationClip;

		// Token: 0x04005DCD RID: 24013
		public FsmFloat pauseBetweenAnimChange;

		// Token: 0x04005DCE RID: 24014
		private float xScale;

		// Token: 0x04005DCF RID: 24015
		public bool flyingFast;

		// Token: 0x04005DD0 RID: 24016
		private FsmVector3 vector;

		// Token: 0x04005DD1 RID: 24017
		private tk2dSpriteAnimator _sprite;

		// Token: 0x04005DD2 RID: 24018
		private Rigidbody2D rb2d;

		// Token: 0x04005DD3 RID: 24019
		private float timer;
	}
}
