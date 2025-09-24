using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C36 RID: 3126
	[ActionCategory("Enemy AI")]
	public class FaceObjectV4 : FsmStateAction
	{
		// Token: 0x06005F09 RID: 24329 RVA: 0x001E1BE4 File Offset: 0x001DFDE4
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

		// Token: 0x06005F0A RID: 24330 RVA: 0x001E1C34 File Offset: 0x001DFE34
		public override void OnEnter()
		{
			this.objectA_object = base.Fsm.GetOwnerDefaultTarget(this.ObjectA);
			this._sprite = this.objectA_object.GetComponent<tk2dSpriteAnimator>();
			this.hc = this.objectA_object.GetComponent<HeroController>();
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

		// Token: 0x06005F0B RID: 24331 RVA: 0x001E1CE7 File Offset: 0x001DFEE7
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

		// Token: 0x06005F0C RID: 24332 RVA: 0x001E1D1C File Offset: 0x001DFF1C
		private void DoFace()
		{
			Vector3 localScale = this.objectA_object.transform.localScale;
			this.StoreDuration.Value = 0f;
			if (this.ObjectB.Value == null || this.ObjectB.IsNone)
			{
				base.Finish();
			}
			if (this.objectA_object.transform.position.x < this.ObjectB.Value.transform.position.x)
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

		// Token: 0x06005F0D RID: 24333 RVA: 0x001E1E94 File Offset: 0x001E0094
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
			if (this.turnAudioClipTable != null)
			{
				this.turnAudioClipTable.SpawnAndPlayOneShot(this.objectA_object.transform.position, false);
			}
			if (this.hc)
			{
				this.hc.cState.facingRight = (this.objectA_object.transform.localScale.x < 1f);
			}
		}

		// Token: 0x04005BDF RID: 23519
		[RequiredField]
		public FsmOwnerDefault ObjectA;

		// Token: 0x04005BE0 RID: 23520
		[RequiredField]
		public FsmGameObject ObjectB;

		// Token: 0x04005BE1 RID: 23521
		[Tooltip("Does object A's sprite face right?")]
		public FsmBool SpriteFacesRight;

		// Token: 0x04005BE2 RID: 23522
		public FsmString NewAnimationClip;

		// Token: 0x04005BE3 RID: 23523
		public bool ResetFrame = true;

		// Token: 0x04005BE4 RID: 23524
		public float PauseBetweenTurns;

		// Token: 0x04005BE5 RID: 23525
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreDuration;

		// Token: 0x04005BE6 RID: 23526
		public RandomAudioClipTable turnAudioClipTable;

		// Token: 0x04005BE7 RID: 23527
		public bool EveryFrame;

		// Token: 0x04005BE8 RID: 23528
		private float xScale;

		// Token: 0x04005BE9 RID: 23529
		private FsmVector3 vector;

		// Token: 0x04005BEA RID: 23530
		private tk2dSpriteAnimator _sprite;

		// Token: 0x04005BEB RID: 23531
		private GameObject objectA_object;

		// Token: 0x04005BEC RID: 23532
		private float pauseTimer;

		// Token: 0x04005BED RID: 23533
		private HeroController hc;
	}
}
