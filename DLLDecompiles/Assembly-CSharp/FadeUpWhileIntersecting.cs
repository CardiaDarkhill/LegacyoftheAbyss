using System;
using System.Linq;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000230 RID: 560
public class FadeUpWhileIntersecting : MonoBehaviour
{
	// Token: 0x17000245 RID: 581
	// (get) Token: 0x0600149A RID: 5274 RVA: 0x0005CCBC File Offset: 0x0005AEBC
	// (set) Token: 0x0600149B RID: 5275 RVA: 0x0005CCC4 File Offset: 0x0005AEC4
	public bool IsTargetIntersecting { get; private set; }

	// Token: 0x0600149C RID: 5276 RVA: 0x0005CCCD File Offset: 0x0005AECD
	private void Awake()
	{
		if (this.fadeGroup)
		{
			this.initialAlpha = this.fadeGroup.AlphaSelf;
		}
	}

	// Token: 0x0600149D RID: 5277 RVA: 0x0005CCED File Offset: 0x0005AEED
	private void OnEnable()
	{
		if (this.fadeGroup)
		{
			this.fadeGroup.AlphaSelf = 0f;
		}
	}

	// Token: 0x0600149E RID: 5278 RVA: 0x0005CD0C File Offset: 0x0005AF0C
	private void LateUpdate()
	{
		Transform transform = base.transform;
		Transform target = this.GetTarget();
		Vector2 vector;
		float time;
		if (target)
		{
			Vector2 b = transform.position;
			vector = target.position - b;
			float magnitude = vector.magnitude;
			this.IsTargetIntersecting = this.fadeRange.IsInRange(magnitude);
			time = this.fadeRange.GetTBetween(magnitude);
		}
		else
		{
			this.IsTargetIntersecting = false;
			time = 1f;
			vector = Vector2.zero;
		}
		float num = this.initialAlpha;
		if (this.fadeGroup)
		{
			num = this.fadeCurve.Evaluate(time) * this.initialAlpha;
			if (num <= 0.001f)
			{
				num = 0f;
			}
			this.fadeGroup.AlphaSelf = ((Math.Abs(this.fadeSpeed) <= Mathf.Epsilon) ? num : Mathf.Lerp(this.fadeGroup.AlphaSelf, num, this.fadeSpeed * Time.deltaTime));
		}
		if (!target)
		{
			return;
		}
		float z = vector.normalized.DirectionToAngle();
		if (Math.Abs(num) <= 0.001f)
		{
			transform.rotation = Quaternion.Euler(0f, 0f, z);
			return;
		}
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, z), this.rotateSpeed * Time.deltaTime);
	}

	// Token: 0x0600149F RID: 5279 RVA: 0x0005CE74 File Offset: 0x0005B074
	private Transform GetTarget()
	{
		switch (this.rotateTowards)
		{
		case FadeUpWhileIntersecting.RotateTowardsTargets.None:
			return null;
		case FadeUpWhileIntersecting.RotateTowardsTargets.Hero:
			return HeroController.instance.transform;
		case FadeUpWhileIntersecting.RotateTowardsTargets.ClosestEnemy:
		{
			HealthManager healthManager = HealthManager.EnumerateActiveEnemies().FirstOrDefault<HealthManager>();
			if (!(healthManager != null))
			{
				return null;
			}
			return healthManager.transform;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x040012F3 RID: 4851
	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	// Token: 0x040012F4 RID: 4852
	[SerializeField]
	private MinMaxFloat fadeRange;

	// Token: 0x040012F5 RID: 4853
	[SerializeField]
	private AnimationCurve fadeCurve;

	// Token: 0x040012F6 RID: 4854
	[SerializeField]
	private float fadeSpeed;

	// Token: 0x040012F7 RID: 4855
	[Space]
	[SerializeField]
	private FadeUpWhileIntersecting.RotateTowardsTargets rotateTowards;

	// Token: 0x040012F8 RID: 4856
	[SerializeField]
	private float rotateSpeed;

	// Token: 0x040012F9 RID: 4857
	private float initialAlpha;

	// Token: 0x02001545 RID: 5445
	private enum RotateTowardsTargets
	{
		// Token: 0x0400868F RID: 34447
		None,
		// Token: 0x04008690 RID: 34448
		Hero,
		// Token: 0x04008691 RID: 34449
		ClosestEnemy
	}
}
