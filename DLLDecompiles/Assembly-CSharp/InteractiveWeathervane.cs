using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200050D RID: 1293
public class InteractiveWeathervane : MonoBehaviour
{
	// Token: 0x06002E2B RID: 11819 RVA: 0x000CAB97 File Offset: 0x000C8D97
	private void Awake()
	{
		if (this.tinkEffect)
		{
			this.tinkEffect.HitInDirection += this.OnHitInDirection;
		}
	}

	// Token: 0x06002E2C RID: 11820 RVA: 0x000CABC0 File Offset: 0x000C8DC0
	private void OnHitInDirection(GameObject source, HitInstance.HitDirection direction)
	{
		float num;
		switch (direction)
		{
		case HitInstance.HitDirection.Left:
			num = -1f;
			break;
		case HitInstance.HitDirection.Right:
			num = 1f;
			break;
		case HitInstance.HitDirection.Up:
		case HitInstance.HitDirection.Down:
			num = (float)((source.transform.position.x > base.transform.position.x) ? 1 : -1);
			break;
		default:
			throw new NotImplementedException();
		}
		int stateNameHash = (num > 0f) ? InteractiveWeathervane._hitRightAnim : InteractiveWeathervane._hitLeftAnim;
		if (this.rotationAnimator)
		{
			this.rotationAnimator.Play(stateNameHash, 0, 0f);
		}
		if (this.poleAnimator)
		{
			this.poleAnimator.Play(stateNameHash, 0, 0f);
		}
		this.OnHit.Invoke();
	}

	// Token: 0x04003066 RID: 12390
	private static readonly int _hitLeftAnim = Animator.StringToHash("Hit Left");

	// Token: 0x04003067 RID: 12391
	private static readonly int _hitRightAnim = Animator.StringToHash("Hit Right");

	// Token: 0x04003068 RID: 12392
	[SerializeField]
	private Animator rotationAnimator;

	// Token: 0x04003069 RID: 12393
	[SerializeField]
	private Animator poleAnimator;

	// Token: 0x0400306A RID: 12394
	[SerializeField]
	private TinkEffect tinkEffect;

	// Token: 0x0400306B RID: 12395
	[Space]
	[SerializeField]
	private UnityEvent OnHit;
}
