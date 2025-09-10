using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C30 RID: 3120
	[ActionCategory("Enemy AI")]
	public class FaceDirectionV2 : FsmStateAction
	{
		// Token: 0x06005EE6 RID: 24294 RVA: 0x001E0A0A File Offset: 0x001DEC0A
		public override void Reset()
		{
			this.ObjectA = null;
			this.NewAnimationClip = null;
			this.SpriteFacesRight = false;
			this.EveryFrame = false;
			this.ResetFrame = false;
			this.PauseBetweenTurns = 0.5f;
			this.StoreDuration = null;
		}

		// Token: 0x06005EE7 RID: 24295 RVA: 0x001E0A46 File Offset: 0x001DEC46
		public override string ErrorCheck()
		{
			this.Validate();
			return base.ErrorCheck();
		}

		// Token: 0x06005EE8 RID: 24296 RVA: 0x001E0A54 File Offset: 0x001DEC54
		private void Validate()
		{
			if (this.Direction.Value == 0f || this.Direction.IsNone)
			{
				this.Direction.Value = 1f;
				return;
			}
			this.Direction.Value = Mathf.Sign(this.Direction.Value);
		}

		// Token: 0x06005EE9 RID: 24297 RVA: 0x001E0AAC File Offset: 0x001DECAC
		public override void OnEnter()
		{
			this.objectA_object = base.Fsm.GetOwnerDefaultTarget(this.ObjectA);
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
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005EEA RID: 24298 RVA: 0x001E0B4E File Offset: 0x001DED4E
		public override void OnUpdate()
		{
			if (this.pauseTimer <= 0f)
			{
				this.DoFace();
				this.pauseTimer = this.PauseBetweenTurns;
				return;
			}
			this.pauseTimer -= Time.deltaTime;
		}

		// Token: 0x06005EEB RID: 24299 RVA: 0x001E0B84 File Offset: 0x001DED84
		private void DoFace()
		{
			this.Validate();
			Vector3 localScale = this.objectA_object.transform.localScale;
			this.StoreDuration.Value = 0f;
			if (this.Direction.Value > 0f)
			{
				if (this.SpriteFacesRight.Value)
				{
					if (localScale.x != this.xScale)
					{
						localScale.x = this.xScale;
						this.UpdateAnimation();
					}
				}
				else if (localScale.x != -this.xScale)
				{
					localScale.x = -this.xScale;
					this.UpdateAnimation();
				}
			}
			else if (this.SpriteFacesRight.Value)
			{
				if (localScale.x != -this.xScale)
				{
					localScale.x = -this.xScale;
					this.UpdateAnimation();
				}
			}
			else if (localScale.x != this.xScale)
			{
				localScale.x = this.xScale;
				this.UpdateAnimation();
			}
			this.objectA_object.transform.localScale = new Vector3(localScale.x, this.objectA_object.transform.localScale.y, this.objectA_object.transform.localScale.z);
		}

		// Token: 0x06005EEC RID: 24300 RVA: 0x001E0CBC File Offset: 0x001DEEBC
		private void UpdateAnimation()
		{
			if (this.ResetFrame)
			{
				this._sprite.PlayFromFrame(0);
			}
			if (!this.NewAnimationClip.IsNone && !string.IsNullOrEmpty(this.NewAnimationClip.Value))
			{
				this.StoreDuration.Value = this._sprite.PlayAnimGetTime(this.NewAnimationClip.Value);
			}
		}

		// Token: 0x04005B95 RID: 23445
		[RequiredField]
		public FsmOwnerDefault ObjectA;

		// Token: 0x04005B96 RID: 23446
		[RequiredField]
		public FsmFloat Direction;

		// Token: 0x04005B97 RID: 23447
		[Tooltip("Does object A's sprite face right?")]
		public FsmBool SpriteFacesRight;

		// Token: 0x04005B98 RID: 23448
		public FsmString NewAnimationClip;

		// Token: 0x04005B99 RID: 23449
		public bool ResetFrame = true;

		// Token: 0x04005B9A RID: 23450
		public float PauseBetweenTurns;

		// Token: 0x04005B9B RID: 23451
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreDuration;

		// Token: 0x04005B9C RID: 23452
		public bool EveryFrame;

		// Token: 0x04005B9D RID: 23453
		private float xScale;

		// Token: 0x04005B9E RID: 23454
		private FsmVector3 vector;

		// Token: 0x04005B9F RID: 23455
		private tk2dSpriteAnimator _sprite;

		// Token: 0x04005BA0 RID: 23456
		private GameObject objectA_object;

		// Token: 0x04005BA1 RID: 23457
		private float pauseTimer;
	}
}
