using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C31 RID: 3121
	[ActionCategory("Enemy AI")]
	[Tooltip("Object A will flip to face Object B horizontally.")]
	public class FaceObject : FsmStateAction
	{
		// Token: 0x06005EEE RID: 24302 RVA: 0x001E0D2C File Offset: 0x001DEF2C
		public override void Reset()
		{
			this.objectA = null;
			this.objectB = null;
			this.newAnimationClip = null;
			this.spriteFacesRight = false;
			this.everyFrame = false;
			this.resetFrame = false;
			this.playNewAnimation = false;
		}

		// Token: 0x06005EEF RID: 24303 RVA: 0x001E0D64 File Offset: 0x001DEF64
		public override void OnEnter()
		{
			this._sprite = this.objectA.Value.GetComponent<tk2dSpriteAnimator>();
			if (this._sprite == null)
			{
				base.Finish();
			}
			this.xScale = this.objectA.Value.transform.localScale.x;
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

		// Token: 0x06005EF0 RID: 24304 RVA: 0x001E0DEE File Offset: 0x001DEFEE
		public override void OnUpdate()
		{
			this.DoFace();
		}

		// Token: 0x06005EF1 RID: 24305 RVA: 0x001E0DF8 File Offset: 0x001DEFF8
		private void DoFace()
		{
			Vector3 localScale = this.objectA.Value.transform.localScale;
			if (this.objectB.Value == null || this.objectB.IsNone)
			{
				base.Finish();
			}
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
				}
			}
			this.objectA.Value.transform.localScale = new Vector3(localScale.x, this.objectA.Value.transform.localScale.y, this.objectA.Value.transform.localScale.z);
		}

		// Token: 0x04005BA2 RID: 23458
		[RequiredField]
		public FsmGameObject objectA;

		// Token: 0x04005BA3 RID: 23459
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject objectB;

		// Token: 0x04005BA4 RID: 23460
		[Tooltip("Does object A's sprite face right?")]
		public FsmBool spriteFacesRight;

		// Token: 0x04005BA5 RID: 23461
		public bool playNewAnimation;

		// Token: 0x04005BA6 RID: 23462
		public FsmString newAnimationClip;

		// Token: 0x04005BA7 RID: 23463
		public bool resetFrame = true;

		// Token: 0x04005BA8 RID: 23464
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04005BA9 RID: 23465
		private float xScale;

		// Token: 0x04005BAA RID: 23466
		private FsmVector3 vector;

		// Token: 0x04005BAB RID: 23467
		private tk2dSpriteAnimator _sprite;
	}
}
