using System;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000571 RID: 1393
public class BlackThreadStrandSimple : MonoBehaviour
{
	// Token: 0x060031DA RID: 12762 RVA: 0x000DD418 File Offset: 0x000DB618
	private void Awake()
	{
		if (this.rageStartEvent)
		{
			this.rageStartEvent.ReceivedEvent += this.OnRageStart;
		}
		if (this.rageEndEvent)
		{
			this.rageEndEvent.ReceivedEvent += this.OnRageEnd;
		}
		Vector3 lossyScale = base.transform.lossyScale;
		this.scaleAdjust = lossyScale.y / lossyScale.x;
		foreach (object obj in this.creaturesParent)
		{
			Transform transform = (Transform)obj;
			BasicSpriteAnimator component = transform.GetComponent<BasicSpriteAnimator>();
			if (component)
			{
				Vector3 localScale = transform.localScale;
				localScale.x *= this.scaleAdjust;
				transform.localScale = localScale;
				BlackThreadStrandSimple.CreatureTracker tracker = new BlackThreadStrandSimple.CreatureTracker
				{
					Sprite = transform.GetComponent<SpriteRenderer>(),
					Animator = component,
					Transform = transform,
					StartX = transform.localPosition.x
				};
				component.OnAnimEnd.AddListener(delegate()
				{
					this.RepositionAndPlay(tracker);
				});
			}
		}
		this.rageCreatures = new List<ScaleActivation>(this.rageParent.childCount);
		this.rageParent.gameObject.SetActive(true);
		foreach (object obj2 in this.rageParent)
		{
			Transform transform2 = (Transform)obj2;
			transform2.gameObject.SetActive(false);
			Vector3 localScale2 = transform2.localScale;
			localScale2.x *= this.scaleAdjust;
			transform2.localScale = localScale2;
			ScaleActivation component2 = transform2.GetComponent<ScaleActivation>();
			this.rageCreatures.Add(component2);
		}
	}

	// Token: 0x060031DB RID: 12763 RVA: 0x000DD614 File Offset: 0x000DB814
	private void OnRageStart()
	{
		foreach (ScaleActivation scaleActivation in this.rageCreatures)
		{
			scaleActivation.Activate();
		}
	}

	// Token: 0x060031DC RID: 12764 RVA: 0x000DD664 File Offset: 0x000DB864
	private void OnRageEnd()
	{
		foreach (ScaleActivation scaleActivation in this.rageCreatures)
		{
			scaleActivation.Deactivate();
		}
	}

	// Token: 0x060031DD RID: 12765 RVA: 0x000DD6B4 File Offset: 0x000DB8B4
	private void RepositionAndPlay(BlackThreadStrandSimple.CreatureTracker tracker)
	{
		float num = this.creatureReposition.GetRandomValue() * this.scaleAdjust;
		tracker.Transform.SetLocalPositionX(tracker.StartX + num);
		tracker.Sprite.flipX = (Random.Range(0, 2) == 0);
		tracker.Animator.Play();
	}

	// Token: 0x04003550 RID: 13648
	[SerializeField]
	private Transform creaturesParent;

	// Token: 0x04003551 RID: 13649
	[SerializeField]
	private MinMaxFloat creatureReposition;

	// Token: 0x04003552 RID: 13650
	[SerializeField]
	private Transform rageParent;

	// Token: 0x04003553 RID: 13651
	[SerializeField]
	private EventRegister rageStartEvent;

	// Token: 0x04003554 RID: 13652
	[SerializeField]
	private EventRegister rageEndEvent;

	// Token: 0x04003555 RID: 13653
	private float scaleAdjust;

	// Token: 0x04003556 RID: 13654
	private List<ScaleActivation> rageCreatures;

	// Token: 0x02001874 RID: 6260
	private class CreatureTracker
	{
		// Token: 0x0400921F RID: 37407
		public Transform Transform;

		// Token: 0x04009220 RID: 37408
		public SpriteRenderer Sprite;

		// Token: 0x04009221 RID: 37409
		public BasicSpriteAnimator Animator;

		// Token: 0x04009222 RID: 37410
		public float StartX;
	}
}
