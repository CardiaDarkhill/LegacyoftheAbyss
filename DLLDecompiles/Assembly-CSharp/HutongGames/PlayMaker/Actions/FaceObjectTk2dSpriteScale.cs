using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C33 RID: 3123
	[ActionCategory("Enemy AI")]
	public class FaceObjectTk2dSpriteScale : FsmStateAction
	{
		// Token: 0x06005EF8 RID: 24312 RVA: 0x001E119C File Offset: 0x001DF39C
		public override void Reset()
		{
			this.ObjectA = null;
			this.ObjectB = null;
			this.NewAnimationClip = null;
			this.SpriteFacesRight = false;
			this.EveryFrame = false;
			this.ResetFrame = false;
			this.PauseBetweenTurns = 0.5f;
			this.StoreDuration = null;
		}

		// Token: 0x06005EF9 RID: 24313 RVA: 0x001E11EC File Offset: 0x001DF3EC
		public override void OnEnter()
		{
			this.obj = base.Fsm.GetOwnerDefaultTarget(this.ObjectA);
			this.sprite = this.obj.GetComponent<tk2dSprite>();
			this.animator = this.obj.GetComponent<tk2dSpriteAnimator>();
			this.pauseTimer = 0f;
			if (this.animator == null)
			{
				base.Finish();
			}
			this.xScale = this.sprite.scale.x * this.obj.transform.localScale.x;
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

		// Token: 0x06005EFA RID: 24314 RVA: 0x001E12B0 File Offset: 0x001DF4B0
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

		// Token: 0x06005EFB RID: 24315 RVA: 0x001E12E4 File Offset: 0x001DF4E4
		private void DoFace()
		{
			float num = this.sprite.scale.x * this.obj.transform.localScale.x;
			this.StoreDuration.Value = 0f;
			if (this.ObjectB.Value == null || this.ObjectB.IsNone)
			{
				base.Finish();
			}
			if (this.obj.transform.position.x < this.ObjectB.Value.transform.position.x)
			{
				if (this.SpriteFacesRight.Value)
				{
					if (!Mathf.Approximately(num, this.xScale))
					{
						num = this.xScale;
						this.UpdateAnimation();
					}
				}
				else if (!Mathf.Approximately(num, -this.xScale))
				{
					num = -this.xScale;
					this.UpdateAnimation();
				}
			}
			else if (this.SpriteFacesRight.Value)
			{
				if (!Mathf.Approximately(num, -this.xScale))
				{
					num = -this.xScale;
					this.UpdateAnimation();
				}
			}
			else if (!Mathf.Approximately(num, this.xScale))
			{
				num = this.xScale;
				this.UpdateAnimation();
			}
			this.sprite.scale = new Vector3(num / this.obj.transform.localScale.x, this.sprite.scale.y, this.sprite.scale.z);
		}

		// Token: 0x06005EFC RID: 24316 RVA: 0x001E1458 File Offset: 0x001DF658
		private void UpdateAnimation()
		{
			if (this.ResetFrame)
			{
				this.animator.PlayFromFrame(0);
			}
			if (!this.NewAnimationClip.IsNone && !string.IsNullOrEmpty(this.NewAnimationClip.Value))
			{
				this.StoreDuration.Value = this.animator.PlayAnimGetTime(this.NewAnimationClip.Value);
			}
			if (this.turnAudioClipTable != null)
			{
				this.turnAudioClipTable.SpawnAndPlayOneShot(this.obj.transform.position, false);
			}
		}

		// Token: 0x04005BB5 RID: 23477
		[RequiredField]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault ObjectA;

		// Token: 0x04005BB6 RID: 23478
		[RequiredField]
		public FsmGameObject ObjectB;

		// Token: 0x04005BB7 RID: 23479
		[Tooltip("Does object A's sprite face right?")]
		public FsmBool SpriteFacesRight;

		// Token: 0x04005BB8 RID: 23480
		public FsmString NewAnimationClip;

		// Token: 0x04005BB9 RID: 23481
		public bool ResetFrame = true;

		// Token: 0x04005BBA RID: 23482
		public float PauseBetweenTurns;

		// Token: 0x04005BBB RID: 23483
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreDuration;

		// Token: 0x04005BBC RID: 23484
		public RandomAudioClipTable turnAudioClipTable;

		// Token: 0x04005BBD RID: 23485
		public bool EveryFrame;

		// Token: 0x04005BBE RID: 23486
		private float xScale;

		// Token: 0x04005BBF RID: 23487
		private FsmVector3 vector;

		// Token: 0x04005BC0 RID: 23488
		private GameObject obj;

		// Token: 0x04005BC1 RID: 23489
		private tk2dSprite sprite;

		// Token: 0x04005BC2 RID: 23490
		private tk2dSpriteAnimator animator;

		// Token: 0x04005BC3 RID: 23491
		private float pauseTimer;
	}
}
