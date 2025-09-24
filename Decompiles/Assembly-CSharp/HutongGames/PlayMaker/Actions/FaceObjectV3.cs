using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C35 RID: 3125
	[ActionCategory("Enemy AI")]
	public class FaceObjectV3 : FsmStateAction
	{
		// Token: 0x06005F03 RID: 24323 RVA: 0x001E1888 File Offset: 0x001DFA88
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

		// Token: 0x06005F04 RID: 24324 RVA: 0x001E18D8 File Offset: 0x001DFAD8
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

		// Token: 0x06005F05 RID: 24325 RVA: 0x001E198B File Offset: 0x001DFB8B
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

		// Token: 0x06005F06 RID: 24326 RVA: 0x001E19C0 File Offset: 0x001DFBC0
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

		// Token: 0x06005F07 RID: 24327 RVA: 0x001E1B38 File Offset: 0x001DFD38
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
			if (this.hc)
			{
				this.hc.cState.facingRight = (this.objectA_object.transform.localScale.x < 1f);
			}
		}

		// Token: 0x04005BD1 RID: 23505
		[RequiredField]
		public FsmOwnerDefault ObjectA;

		// Token: 0x04005BD2 RID: 23506
		[RequiredField]
		public FsmGameObject ObjectB;

		// Token: 0x04005BD3 RID: 23507
		[Tooltip("Does object A's sprite face right?")]
		public FsmBool SpriteFacesRight;

		// Token: 0x04005BD4 RID: 23508
		public FsmString NewAnimationClip;

		// Token: 0x04005BD5 RID: 23509
		public bool ResetFrame = true;

		// Token: 0x04005BD6 RID: 23510
		public float PauseBetweenTurns;

		// Token: 0x04005BD7 RID: 23511
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreDuration;

		// Token: 0x04005BD8 RID: 23512
		public bool EveryFrame;

		// Token: 0x04005BD9 RID: 23513
		private float xScale;

		// Token: 0x04005BDA RID: 23514
		private FsmVector3 vector;

		// Token: 0x04005BDB RID: 23515
		private tk2dSpriteAnimator _sprite;

		// Token: 0x04005BDC RID: 23516
		private GameObject objectA_object;

		// Token: 0x04005BDD RID: 23517
		private float pauseTimer;

		// Token: 0x04005BDE RID: 23518
		private HeroController hc;
	}
}
