using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005DB RID: 1499
public abstract class RadialHudIcon : MonoBehaviour
{
	// Token: 0x06003531 RID: 13617 RVA: 0x000EBE5A File Offset: 0x000EA05A
	private void Start()
	{
		if (!this.updated)
		{
			this.UpdateDisplay();
		}
	}

	// Token: 0x06003532 RID: 13618 RVA: 0x000EBE6C File Offset: 0x000EA06C
	private void Update()
	{
		if (this.radialLerpRoutine != null)
		{
			return;
		}
		float targetFillAmount = this.GetTargetFillAmount();
		if (Math.Abs(targetFillAmount - this.previousFillAmount) <= Mathf.Epsilon)
		{
			return;
		}
		this.SetFillAmount(targetFillAmount);
	}

	// Token: 0x06003533 RID: 13619 RVA: 0x000EBEA8 File Offset: 0x000EA0A8
	private float GetTargetFillAmount()
	{
		if (this.storageAmount <= 0)
		{
			return 1f;
		}
		float num = (float)this.amountLeft / (float)this.storageAmount;
		float midProgress = this.GetMidProgress();
		if (midProgress <= Mathf.Epsilon)
		{
			return num;
		}
		float b = (float)Mathf.Clamp(this.amountLeft - 1, 0, this.storageAmount) / (float)this.storageAmount;
		return Mathf.Lerp(num, b, midProgress);
	}

	// Token: 0x06003534 RID: 13620 RVA: 0x000EBF0C File Offset: 0x000EA10C
	private void SetFillAmount(float value)
	{
		if (this.radialImage)
		{
			this.radialImage.fillAmount = value;
		}
		if (this.radialImageBg)
		{
			this.radialImageBg.fillAmount = 1f - value;
		}
		this.previousFillAmount = value;
	}

	// Token: 0x06003535 RID: 13621 RVA: 0x000EBF58 File Offset: 0x000EA158
	protected virtual void OnPreUpdateDisplay()
	{
	}

	// Token: 0x06003536 RID: 13622
	protected abstract bool GetIsActive();

	// Token: 0x06003537 RID: 13623
	protected abstract void GetAmounts(out int amountLeft, out int totalCount);

	// Token: 0x06003538 RID: 13624
	protected abstract bool TryGetHudSprite(out Sprite sprite);

	// Token: 0x06003539 RID: 13625
	public abstract bool GetIsEmpty();

	// Token: 0x0600353A RID: 13626
	protected abstract bool HasTargetChanged();

	// Token: 0x0600353B RID: 13627 RVA: 0x000EBF5A File Offset: 0x000EA15A
	protected virtual bool TryGetBarColour(out Color color)
	{
		color = Color.black;
		return false;
	}

	// Token: 0x0600353C RID: 13628 RVA: 0x000EBF68 File Offset: 0x000EA168
	protected virtual float GetMidProgress()
	{
		return 0f;
	}

	// Token: 0x0600353D RID: 13629 RVA: 0x000EBF70 File Offset: 0x000EA170
	protected void UpdateDisplay()
	{
		this.updated = true;
		this.OnPreUpdateDisplay();
		if (!this.GetIsActive())
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.gameObject.SetActive(true);
		this.GetAmounts(out this.amountLeft, out this.storageAmount);
		bool isEmpty = this.GetIsEmpty();
		if (this.icon)
		{
			Sprite sprite;
			if (this.TryGetHudSprite(out sprite))
			{
				this.icon.sprite = sprite;
				this.icon.transform.localScale = new Vector3(1.4f, 1.4f, 1f);
			}
			else
			{
				this.icon.sprite = sprite;
				this.icon.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
			}
			this.SetIconColour(this.icon, isEmpty ? this.inactiveColor : this.activeColor);
		}
		if (this.radialImage)
		{
			Color color;
			if (!this.TryGetBarColour(out color))
			{
				color = Color.white;
			}
			float targetFillAmount;
			if (this.storageAmount > 0)
			{
				targetFillAmount = this.GetTargetFillAmount();
			}
			else
			{
				targetFillAmount = 1f;
				if (isEmpty)
				{
					color = color.MultiplyElements(this.inactiveColor);
				}
			}
			this.radialImage.color = color;
			if (this.radialLerpRoutine != null)
			{
				base.StopCoroutine(this.radialLerpRoutine);
			}
			if (!this.HasTargetChanged())
			{
				float initialFillAmount = this.radialImage.fillAmount;
				float duration = (targetFillAmount > initialFillAmount) ? this.radialLerpUpTime : this.radialLerpDownTime;
				this.radialLerpRoutine = this.StartTimerRoutine(0f, duration, delegate(float time)
				{
					this.SetFillAmount(Mathf.Lerp(initialFillAmount, targetFillAmount, this.radialLerpCurve.Evaluate(time)));
				}, null, delegate
				{
					this.radialLerpRoutine = null;
				}, false);
			}
			else
			{
				this.SetFillAmount(targetFillAmount);
			}
		}
		if (!this.templateNotch)
		{
			return;
		}
		this.templateNotch.SetActive(false);
		for (int i = this.storageAmount - this.notches.Count; i > 0; i--)
		{
			GameObject item = Object.Instantiate<GameObject>(this.templateNotch, this.templateNotch.transform.parent);
			this.notches.Add(item);
		}
		for (int j = 0; j < this.notches.Count; j++)
		{
			this.notches[j].SetActive(j < this.storageAmount);
		}
	}

	// Token: 0x0600353E RID: 13630 RVA: 0x000EC1E9 File Offset: 0x000EA3E9
	protected virtual void SetIconColour(SpriteRenderer spriteRenderer, Color color)
	{
		spriteRenderer.color = color;
	}

	// Token: 0x04003899 RID: 14489
	[SerializeField]
	private SpriteRenderer icon;

	// Token: 0x0400389A RID: 14490
	[SerializeField]
	protected Color activeColor;

	// Token: 0x0400389B RID: 14491
	[SerializeField]
	protected Color inactiveColor;

	// Token: 0x0400389C RID: 14492
	[Space]
	[SerializeField]
	private Image radialImage;

	// Token: 0x0400389D RID: 14493
	[SerializeField]
	private Image radialImageBg;

	// Token: 0x0400389E RID: 14494
	[SerializeField]
	private float radialLerpDownTime;

	// Token: 0x0400389F RID: 14495
	[SerializeField]
	private float radialLerpUpTime;

	// Token: 0x040038A0 RID: 14496
	[SerializeField]
	private AnimationCurve radialLerpCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040038A1 RID: 14497
	[SerializeField]
	private GameObject templateNotch;

	// Token: 0x040038A2 RID: 14498
	private readonly List<GameObject> notches = new List<GameObject>();

	// Token: 0x040038A3 RID: 14499
	private Coroutine radialLerpRoutine;

	// Token: 0x040038A4 RID: 14500
	private int amountLeft;

	// Token: 0x040038A5 RID: 14501
	private int storageAmount;

	// Token: 0x040038A6 RID: 14502
	private float previousFillAmount;

	// Token: 0x040038A7 RID: 14503
	private bool updated;
}
