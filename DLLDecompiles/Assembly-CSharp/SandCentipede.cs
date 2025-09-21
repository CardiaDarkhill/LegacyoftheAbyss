using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000548 RID: 1352
public class SandCentipede : MonoBehaviour
{
	// Token: 0x0600305A RID: 12378 RVA: 0x000D58AC File Offset: 0x000D3AAC
	private void Awake()
	{
		this.animationIds = new int[this.animations.Length];
		for (int i = 0; i < this.animationIds.Length; i++)
		{
			this.animationIds[i] = Animator.StringToHash(this.animations[i]);
		}
		this.minPos = base.transform.position;
		this.maxPos = base.transform.TransformPoint(this.rangePos);
		this.hasEverWarmedUp = false;
	}

	// Token: 0x0600305B RID: 12379 RVA: 0x000D5932 File Offset: 0x000D3B32
	private void OnEnable()
	{
		this.animator.enabled = false;
		this.sprite.enabled = false;
		this.animRoutine = base.StartCoroutine(this.Anim());
	}

	// Token: 0x0600305C RID: 12380 RVA: 0x000D595E File Offset: 0x000D3B5E
	private void OnDisable()
	{
		base.StopCoroutine(this.animRoutine);
	}

	// Token: 0x0600305D RID: 12381 RVA: 0x000D596C File Offset: 0x000D3B6C
	private IEnumerator Anim()
	{
		if (!this.hasEverWarmedUp)
		{
			this.animator.enabled = true;
			AnimatorCullingMode previous = this.animator.cullingMode;
			if (previous != AnimatorCullingMode.AlwaysAnimate)
			{
				this.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			}
			yield return null;
			if (previous != AnimatorCullingMode.AlwaysAnimate)
			{
				this.animator.cullingMode = previous;
			}
			this.animator.enabled = false;
			this.hasEverWarmedUp = true;
		}
		for (;;)
		{
			float initialWaitTimeLeft = this.waitTime.GetRandomValue();
			while (initialWaitTimeLeft > 0f)
			{
				initialWaitTimeLeft -= Time.deltaTime;
				yield return null;
			}
			Vector2 position = Vector2.Lerp(this.minPos, this.maxPos, Random.Range(0f, 1f));
			if (CameraInfoCache.IsWithinBounds(position, SandCentipede.BOUNDS_BUFFER))
			{
				base.transform.SetPosition2D(position);
				this.animator.enabled = true;
				this.sprite.enabled = true;
				if (Random.Range(0, 2) == 0)
				{
					base.transform.FlipLocalScale(true, false, false);
				}
				this.animator.Play(this.animationIds.GetRandomElement<int>());
				yield return null;
				float waitTimeLeft = this.animator.GetCurrentAnimatorStateInfo(0).length;
				while (waitTimeLeft > 0f)
				{
					waitTimeLeft -= Time.deltaTime;
					yield return null;
				}
				this.animator.enabled = false;
				this.sprite.enabled = false;
			}
		}
		yield break;
	}

	// Token: 0x0400333B RID: 13115
	[SerializeField]
	private Animator animator;

	// Token: 0x0400333C RID: 13116
	[SerializeField]
	private SpriteRenderer sprite;

	// Token: 0x0400333D RID: 13117
	[SerializeField]
	private string[] animations;

	// Token: 0x0400333E RID: 13118
	[SerializeField]
	private MinMaxFloat waitTime;

	// Token: 0x0400333F RID: 13119
	[SerializeField]
	private Vector2 rangePos;

	// Token: 0x04003340 RID: 13120
	private Vector2 minPos;

	// Token: 0x04003341 RID: 13121
	private Vector2 maxPos;

	// Token: 0x04003342 RID: 13122
	private int[] animationIds;

	// Token: 0x04003343 RID: 13123
	private Coroutine animRoutine;

	// Token: 0x04003344 RID: 13124
	private bool hasEverWarmedUp;

	// Token: 0x04003345 RID: 13125
	private static readonly Vector2 BOUNDS_BUFFER = new Vector2(3f, 5f);
}
