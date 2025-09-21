using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000545 RID: 1349
public class RosaryCacheShrine : RosaryCache
{
	// Token: 0x0600304B RID: 12363 RVA: 0x000D539E File Offset: 0x000D359E
	protected override float GetHitSourceY(float sourceHeight)
	{
		return base.transform.position.y;
	}

	// Token: 0x0600304C RID: 12364 RVA: 0x000D53B0 File Offset: 0x000D35B0
	protected override void RespondToHit(HitInstance damageInstance, Vector2 hitPos)
	{
		Vector2 force = this.flingDishForce;
		float num = this.flingDishTorque;
		if (this.hitReactAnimator)
		{
			HitInstance.HitDirection hitDirection = damageInstance.GetHitDirection(HitInstance.TargetType.Regular);
			if (hitDirection != HitInstance.HitDirection.Left)
			{
				if (hitDirection != HitInstance.HitDirection.Right)
				{
					this.hitReactAnimator.StartAnimation(Random.Range(0, 2) > 0);
					force.x = 0f;
					num = Random.Range(-num, num);
				}
				else
				{
					this.hitReactAnimator.StartAnimation(false);
				}
			}
			else
			{
				this.hitReactAnimator.StartAnimation(true);
				force.x *= -1f;
				num *= -1f;
			}
		}
		if (base.State >= base.StateCount - 1)
		{
			if (this.regularDish)
			{
				this.regularDish.gameObject.SetActive(false);
			}
			if (this.flingDish)
			{
				this.flingDish.gameObject.SetActive(true);
				this.flingDish.AddForce(force, ForceMode2D.Impulse);
				this.flingDish.AddTorque(num, ForceMode2D.Impulse);
			}
			if (this.OnCompleted != null)
			{
				this.OnCompleted.Invoke();
			}
		}
	}

	// Token: 0x0600304D RID: 12365 RVA: 0x000D54C4 File Offset: 0x000D36C4
	protected override void SetCompletedReturning()
	{
		base.SetCompletedReturning();
		if (this.regularDish)
		{
			this.regularDish.gameObject.SetActive(false);
		}
		if (this.flingDish)
		{
			this.flingDish.gameObject.SetActive(false);
		}
		if (this.brokenDish)
		{
			this.brokenDish.gameObject.SetActive(true);
		}
		if (this.OnCompleted != null)
		{
			this.OnCompleted.Invoke();
		}
	}

	// Token: 0x0400332E RID: 13102
	[SerializeField]
	private Rigidbody2D flingDish;

	// Token: 0x0400332F RID: 13103
	[SerializeField]
	private Vector2 flingDishForce;

	// Token: 0x04003330 RID: 13104
	[SerializeField]
	private float flingDishTorque;

	// Token: 0x04003331 RID: 13105
	[SerializeField]
	private GameObject regularDish;

	// Token: 0x04003332 RID: 13106
	[SerializeField]
	private VectorCurveAnimator hitReactAnimator;

	// Token: 0x04003333 RID: 13107
	[SerializeField]
	private GameObject brokenDish;

	// Token: 0x04003334 RID: 13108
	[Space]
	public UnityEvent OnCompleted;
}
