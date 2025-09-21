using System;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x0200067B RID: 1659
public class CrestSocketUnlockInventoryDescription : MonoBehaviour
{
	// Token: 0x06003B59 RID: 15193 RVA: 0x00105253 File Offset: 0x00103453
	private void Awake()
	{
		this.leftLockInitialPosition = this.leftLock.transform.localPosition;
		this.rightLockInitialPosition = this.rightLock.transform.localPosition;
	}

	// Token: 0x06003B5A RID: 15194 RVA: 0x0010528C File Offset: 0x0010348C
	public void SetSlotSprite(Sprite sprite, Color color)
	{
		this.slotIcon.Sprite = sprite;
		this.slotIcon.Color = color;
		this.leftLock.Color = color;
		this.leftLockGlow.BaseColor = color;
		this.rightLock.Color = color;
		this.rightLockGlow.BaseColor = color;
		this.SetConsumeShakeAmount(0f);
	}

	// Token: 0x06003B5B RID: 15195 RVA: 0x001052EC File Offset: 0x001034EC
	public void StartConsume()
	{
		this.CancelConsume();
		AudioSource spawnedSource = null;
		spawnedSource = (this.spawnedConsumeAudio = this.consumeAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, delegate()
		{
			if (this.spawnedConsumeAudio != spawnedSource)
			{
				return;
			}
			this.spawnedConsumeAudio = null;
		}));
	}

	// Token: 0x06003B5C RID: 15196 RVA: 0x00105349 File Offset: 0x00103549
	public void CancelConsume()
	{
		if (!this.spawnedConsumeAudio)
		{
			return;
		}
		this.spawnedConsumeAudio.Stop();
		this.spawnedConsumeAudio = null;
	}

	// Token: 0x06003B5D RID: 15197 RVA: 0x0010536B File Offset: 0x0010356B
	public void ConsumeCompleted()
	{
		this.spawnedConsumeAudio = null;
	}

	// Token: 0x06003B5E RID: 15198 RVA: 0x00105374 File Offset: 0x00103574
	public void SetConsumeShakeAmount(float t)
	{
		this.leftLockGlow.AlphaSelf = t;
		this.rightLockGlow.AlphaSelf = t;
		Vector2 b = Random.insideUnitCircle * (t * this.lockJitterMagnitude);
		float num = this.lockMoveXCurve.Evaluate(t) * this.lockMoveX;
		this.leftLock.transform.SetLocalPosition2D(this.leftLockInitialPosition + b + new Vector2(-num, 0f));
		this.rightLock.transform.SetLocalPosition2D(this.rightLockInitialPosition + b + new Vector2(num, 0f));
	}

	// Token: 0x04003D9D RID: 15773
	[SerializeField]
	private NestedFadeGroupSpriteRenderer slotIcon;

	// Token: 0x04003D9E RID: 15774
	[SerializeField]
	private NestedFadeGroupSpriteRenderer leftLock;

	// Token: 0x04003D9F RID: 15775
	[SerializeField]
	private NestedFadeGroupSpriteRenderer leftLockGlow;

	// Token: 0x04003DA0 RID: 15776
	[SerializeField]
	private NestedFadeGroupSpriteRenderer rightLock;

	// Token: 0x04003DA1 RID: 15777
	[SerializeField]
	private NestedFadeGroupSpriteRenderer rightLockGlow;

	// Token: 0x04003DA2 RID: 15778
	[Space]
	[SerializeField]
	private float lockMoveX;

	// Token: 0x04003DA3 RID: 15779
	[SerializeField]
	private AnimationCurve lockMoveXCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04003DA4 RID: 15780
	[SerializeField]
	private float lockJitterMagnitude;

	// Token: 0x04003DA5 RID: 15781
	[Space]
	[SerializeField]
	private AudioEvent consumeAudio;

	// Token: 0x04003DA6 RID: 15782
	private AudioSource spawnedConsumeAudio;

	// Token: 0x04003DA7 RID: 15783
	private Vector2 leftLockInitialPosition;

	// Token: 0x04003DA8 RID: 15784
	private Vector2 rightLockInitialPosition;
}
