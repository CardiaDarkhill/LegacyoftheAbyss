using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DA8 RID: 3496
	public class WalkLeftRight : FsmStateAction
	{
		// Token: 0x17000BC6 RID: 3014
		// (get) Token: 0x06006580 RID: 25984 RVA: 0x0020061B File Offset: 0x001FE81B
		private float Direction
		{
			get
			{
				if (this.target)
				{
					return Mathf.Sign(this.target.transform.localScale.x) * (float)(this.spriteFacesLeft ? -1 : 1);
				}
				return 0f;
			}
		}

		// Token: 0x06006581 RID: 25985 RVA: 0x00200658 File Offset: 0x001FE858
		public override void OnEnter()
		{
			this.cycle = 0;
			this.boundsVer = -1;
			this.UpdateIfTargetChanged();
			this.SetupStartingDirection();
			this.walkRoutine = base.StartCoroutine(this.Walk());
			if (this.walkAnimName.Value == "")
			{
				this.walkAnimName.Value = "Walk";
			}
		}

		// Token: 0x06006582 RID: 25986 RVA: 0x002006B8 File Offset: 0x001FE8B8
		public override void OnExit()
		{
			if (this.walkRoutine != null)
			{
				base.StopCoroutine(this.walkRoutine);
				this.walkRoutine = null;
			}
			if (this.turnRoutine != null)
			{
				base.StopCoroutine(this.turnRoutine);
				this.turnRoutine = null;
			}
		}

		// Token: 0x06006583 RID: 25987 RVA: 0x002006F0 File Offset: 0x001FE8F0
		private void UpdateIfTargetChanged()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != this.target)
			{
				this.target = ownerDefaultTarget;
				this.body = this.target.GetComponent<Rigidbody2D>();
				this.spriteAnimator = this.target.GetComponent<tk2dSpriteAnimator>();
				this.collider = this.target.GetComponent<Collider2D>();
			}
		}

		// Token: 0x06006584 RID: 25988 RVA: 0x00200757 File Offset: 0x001FE957
		private IEnumerator Walk()
		{
			if (this.spriteAnimator)
			{
				this.spriteAnimator.Play(this.walkAnimName.Value);
			}
			for (;;)
			{
				if (this.body)
				{
					Vector2 linearVelocity = this.body.linearVelocity;
					linearVelocity.x = this.walkSpeed * this.Direction;
					this.body.linearVelocity = linearVelocity;
					this.cycle++;
					if (this.shouldTurn || (Time.timeAsDouble >= this.nextTurnTime && this.CheckIsGrounded() && (this.CheckWall() || this.CheckFloor())))
					{
						this.shouldTurn = false;
						this.nextTurnTime = Time.timeAsDouble + (double)this.turnDelay;
						yield return this.Turn();
					}
				}
				yield return new WaitForFixedUpdate();
			}
			yield break;
		}

		// Token: 0x06006585 RID: 25989 RVA: 0x00200766 File Offset: 0x001FE966
		private IEnumerator Turn()
		{
			if (!this.moveWhileTurning.Value)
			{
				Vector2 linearVelocity = this.body.linearVelocity;
				linearVelocity.x = 0f;
				this.body.linearVelocity = linearVelocity;
				if (this.flipBeforeTurn.Value)
				{
					this.FlipScale();
				}
				tk2dSpriteAnimationClip clipByName = this.spriteAnimator.GetClipByName(this.turnAnimName.Value);
				if (clipByName != null)
				{
					float seconds = (float)clipByName.frames.Length / clipByName.fps;
					this.spriteAnimator.Play(clipByName);
					yield return new WaitForSeconds(seconds);
				}
				if (!this.flipBeforeTurn.Value)
				{
					this.FlipScale();
				}
				if (this.spriteAnimator)
				{
					this.spriteAnimator.Play(this.walkAnimName.Value);
				}
				this.turnRoutine = null;
			}
			else
			{
				tk2dSpriteAnimationClip clipByName2 = this.spriteAnimator.GetClipByName(this.turnAnimName.Value);
				Vector3 localScale = this.target.transform.localScale;
				localScale.x *= -1f;
				this.target.transform.localScale = localScale;
				Vector2 linearVelocity2 = this.body.linearVelocity;
				linearVelocity2.x = -linearVelocity2.x;
				this.body.linearVelocity = linearVelocity2;
				float seconds2 = (float)clipByName2.frames.Length / clipByName2.fps;
				this.spriteAnimator.Play(clipByName2);
				yield return new WaitForSeconds(seconds2);
				if (this.spriteAnimator)
				{
					this.spriteAnimator.Play(this.walkAnimName.Value);
				}
				this.turnRoutine = null;
			}
			yield break;
		}

		// Token: 0x06006586 RID: 25990 RVA: 0x00200778 File Offset: 0x001FE978
		private void FlipScale()
		{
			Vector3 localScale = this.target.transform.localScale;
			localScale.x *= -1f;
			this.target.transform.localScale = localScale;
		}

		// Token: 0x06006587 RID: 25991 RVA: 0x002007B7 File Offset: 0x001FE9B7
		private void UpdateBounds()
		{
			if (this.boundsVer != this.cycle)
			{
				this.boundsVer = this.cycle;
				this.bounds = this.collider.bounds;
			}
		}

		// Token: 0x06006588 RID: 25992 RVA: 0x002007E4 File Offset: 0x001FE9E4
		private bool CheckWall()
		{
			this.UpdateBounds();
			Vector2 origin = this.bounds.center + new Vector2(0f, -(this.bounds.size.y / 2f) + 0.5f);
			Vector2 direction = Vector2.right * this.Direction;
			float distance = this.bounds.size.x / 2f + 0.1f + this.wallLookAhead;
			RaycastHit2D raycastHit2D = Helper.Raycast2D(origin, direction, distance, 33024);
			return raycastHit2D.collider != null && !raycastHit2D.collider.isTrigger;
		}

		// Token: 0x06006589 RID: 25993 RVA: 0x00200898 File Offset: 0x001FEA98
		private bool CheckFloor()
		{
			this.UpdateBounds();
			return !(Helper.Raycast2D(this.bounds.center + new Vector2((this.bounds.size.x / 2f + 0.1f) * this.Direction, -(this.bounds.size.y / 2f) + 0.5f), Vector2.down, 1f, 256).collider != null);
		}

		// Token: 0x0600658A RID: 25994 RVA: 0x0020092C File Offset: 0x001FEB2C
		private bool CheckIsGrounded()
		{
			this.UpdateBounds();
			return Helper.Raycast2D(this.bounds.center + new Vector2(0f, -(this.bounds.size.y / 2f) + 0.5f), Vector2.down, 1f, 256).collider != null;
		}

		// Token: 0x0600658B RID: 25995 RVA: 0x002009A4 File Offset: 0x001FEBA4
		private void SetupStartingDirection()
		{
			if (this.target.transform.localScale.x < 0f)
			{
				if (!this.spriteFacesLeft && this.startRight.Value)
				{
					this.shouldTurn = true;
				}
				if (this.spriteFacesLeft && this.startLeft.Value)
				{
					this.shouldTurn = true;
				}
			}
			else
			{
				if (this.spriteFacesLeft && this.startRight.Value)
				{
					this.shouldTurn = true;
				}
				if (!this.spriteFacesLeft && this.startLeft.Value)
				{
					this.shouldTurn = true;
				}
			}
			if (!this.startLeft.Value && !this.startRight.Value && !this.keepDirection.Value && Random.Range(0f, 100f) <= 50f)
			{
				this.shouldTurn = true;
			}
			this.startLeft.Value = false;
			this.startRight.Value = false;
		}

		// Token: 0x04006483 RID: 25731
		public FsmOwnerDefault gameObject;

		// Token: 0x04006484 RID: 25732
		public float walkSpeed = 4f;

		// Token: 0x04006485 RID: 25733
		public bool spriteFacesLeft;

		// Token: 0x04006486 RID: 25734
		public FsmString groundLayer = "Terrain";

		// Token: 0x04006487 RID: 25735
		public float turnDelay = 1f;

		// Token: 0x04006488 RID: 25736
		public float wallLookAhead;

		// Token: 0x04006489 RID: 25737
		private double nextTurnTime;

		// Token: 0x0400648A RID: 25738
		[Header("Animation")]
		public FsmString walkAnimName;

		// Token: 0x0400648B RID: 25739
		public FsmString turnAnimName;

		// Token: 0x0400648C RID: 25740
		public FsmBool moveWhileTurning;

		// Token: 0x0400648D RID: 25741
		public FsmBool startLeft;

		// Token: 0x0400648E RID: 25742
		public FsmBool startRight;

		// Token: 0x0400648F RID: 25743
		public FsmBool keepDirection;

		// Token: 0x04006490 RID: 25744
		public FsmBool flipBeforeTurn;

		// Token: 0x04006491 RID: 25745
		private float scaleX_pos;

		// Token: 0x04006492 RID: 25746
		private float scaleX_neg;

		// Token: 0x04006493 RID: 25747
		private const float wallRayHeight = 0.5f;

		// Token: 0x04006494 RID: 25748
		private const float wallRayLength = 0.1f;

		// Token: 0x04006495 RID: 25749
		private const float groundRayLength = 1f;

		// Token: 0x04006496 RID: 25750
		private GameObject target;

		// Token: 0x04006497 RID: 25751
		private Rigidbody2D body;

		// Token: 0x04006498 RID: 25752
		private tk2dSpriteAnimator spriteAnimator;

		// Token: 0x04006499 RID: 25753
		private Collider2D collider;

		// Token: 0x0400649A RID: 25754
		private Coroutine walkRoutine;

		// Token: 0x0400649B RID: 25755
		private Coroutine turnRoutine;

		// Token: 0x0400649C RID: 25756
		private bool shouldTurn;

		// Token: 0x0400649D RID: 25757
		private Bounds bounds;

		// Token: 0x0400649E RID: 25758
		private int cycle;

		// Token: 0x0400649F RID: 25759
		private int boundsVer;
	}
}
