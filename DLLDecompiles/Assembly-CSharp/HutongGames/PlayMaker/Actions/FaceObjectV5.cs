using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C37 RID: 3127
	public class FaceObjectV5 : FsmStateAction
	{
		// Token: 0x06005F0F RID: 24335 RVA: 0x001E1F68 File Offset: 0x001E0168
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

		// Token: 0x06005F10 RID: 24336 RVA: 0x001E1FB8 File Offset: 0x001E01B8
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

		// Token: 0x06005F11 RID: 24337 RVA: 0x001E206B File Offset: 0x001E026B
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

		// Token: 0x06005F12 RID: 24338 RVA: 0x001E20A0 File Offset: 0x001E02A0
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

		// Token: 0x06005F13 RID: 24339 RVA: 0x001E2218 File Offset: 0x001E0418
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
			if (this.turnAudioClipTables != null)
			{
				foreach (RandomAudioClipTable randomAudioClipTable in this.turnAudioClipTables)
				{
					if (!(randomAudioClipTable == null))
					{
						randomAudioClipTable.SpawnAndPlayOneShot(this.objectA_object.transform.position, false);
					}
				}
			}
			if (this.hc)
			{
				this.hc.cState.facingRight = (this.objectA_object.transform.localScale.x < 1f);
			}
		}

		// Token: 0x04005BEE RID: 23534
		[RequiredField]
		public FsmOwnerDefault ObjectA;

		// Token: 0x04005BEF RID: 23535
		[RequiredField]
		public FsmGameObject ObjectB;

		// Token: 0x04005BF0 RID: 23536
		[Tooltip("Does object A's sprite face right?")]
		public FsmBool SpriteFacesRight;

		// Token: 0x04005BF1 RID: 23537
		public FsmString NewAnimationClip;

		// Token: 0x04005BF2 RID: 23538
		public bool ResetFrame = true;

		// Token: 0x04005BF3 RID: 23539
		public float PauseBetweenTurns;

		// Token: 0x04005BF4 RID: 23540
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreDuration;

		// Token: 0x04005BF5 RID: 23541
		public RandomAudioClipTable[] turnAudioClipTables;

		// Token: 0x04005BF6 RID: 23542
		public bool EveryFrame;

		// Token: 0x04005BF7 RID: 23543
		private float xScale;

		// Token: 0x04005BF8 RID: 23544
		private FsmVector3 vector;

		// Token: 0x04005BF9 RID: 23545
		private tk2dSpriteAnimator _sprite;

		// Token: 0x04005BFA RID: 23546
		private GameObject objectA_object;

		// Token: 0x04005BFB RID: 23547
		private float pauseTimer;

		// Token: 0x04005BFC RID: 23548
		private HeroController hc;
	}
}
