using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C34 RID: 3124
	[ActionCategory("Enemy AI")]
	[Tooltip("Object A will flip to face Object B horizontally.")]
	public class FaceObjectV2 : FsmStateAction
	{
		// Token: 0x06005EFE RID: 24318 RVA: 0x001E14F4 File Offset: 0x001DF6F4
		public override void Reset()
		{
			this.objectA = null;
			this.objectB = null;
			this.newAnimationClip = null;
			this.spriteFacesRight = false;
			this.everyFrame = false;
			this.resetFrame = false;
			this.playNewAnimation = false;
			this.pauseBetweenTurns = 0.5f;
		}

		// Token: 0x06005EFF RID: 24319 RVA: 0x001E1544 File Offset: 0x001DF744
		public override void OnEnter()
		{
			this.objectA_object = this.objectA.GetSafe(this);
			if (this.objectA_object == null)
			{
				base.Finish();
				return;
			}
			if (this.objectB.Value == null)
			{
				base.Finish();
				return;
			}
			this._sprite = this.objectA_object.GetComponent<tk2dSpriteAnimator>();
			this.pauseTimer = 0f;
			if (this._sprite == null)
			{
				base.Finish();
			}
			this.xScale = this.objectA_object.transform.localScale.x;
			if (this.xScale < 0f)
			{
				this.xScale *= -1f;
			}
			this.DoFace();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005F00 RID: 24320 RVA: 0x001E1610 File Offset: 0x001DF810
		public override void OnUpdate()
		{
			if (this.pauseTimer <= 0f)
			{
				this.DoFace();
				return;
			}
			this.pauseTimer -= Time.deltaTime;
		}

		// Token: 0x06005F01 RID: 24321 RVA: 0x001E1638 File Offset: 0x001DF838
		private void DoFace()
		{
			Vector3 localScale = this.objectA_object.transform.localScale;
			bool flag = false;
			if (this.objectB.Value == null || this.objectB.IsNone)
			{
				base.Finish();
				return;
			}
			if (this.objectA_object.transform.position.x < this.objectB.Value.transform.position.x)
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
						}
						flag = true;
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
					}
					flag = true;
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
					}
					flag = true;
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
				}
				flag = true;
			}
			if (flag)
			{
				this.pauseTimer = this.pauseBetweenTurns;
			}
			this.objectA_object.transform.localScale = new Vector3(localScale.x, this.objectA_object.transform.localScale.y, this.objectA_object.transform.localScale.z);
		}

		// Token: 0x04005BC4 RID: 23492
		[RequiredField]
		public FsmOwnerDefault objectA;

		// Token: 0x04005BC5 RID: 23493
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject objectB;

		// Token: 0x04005BC6 RID: 23494
		[Tooltip("Does object A's sprite face right?")]
		public FsmBool spriteFacesRight;

		// Token: 0x04005BC7 RID: 23495
		public bool playNewAnimation;

		// Token: 0x04005BC8 RID: 23496
		public FsmString newAnimationClip;

		// Token: 0x04005BC9 RID: 23497
		public bool resetFrame = true;

		// Token: 0x04005BCA RID: 23498
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04005BCB RID: 23499
		public float pauseBetweenTurns;

		// Token: 0x04005BCC RID: 23500
		private float xScale;

		// Token: 0x04005BCD RID: 23501
		private FsmVector3 vector;

		// Token: 0x04005BCE RID: 23502
		private tk2dSpriteAnimator _sprite;

		// Token: 0x04005BCF RID: 23503
		private GameObject objectA_object;

		// Token: 0x04005BD0 RID: 23504
		private float pauseTimer;
	}
}
