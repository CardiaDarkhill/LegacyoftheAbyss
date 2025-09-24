using System;
using GlobalSettings;
using TMProOld;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200072D RID: 1837
public class SimpleCounter : MonoBehaviour
{
	// Token: 0x060041A8 RID: 16808 RVA: 0x00121010 File Offset: 0x0011F210
	private void Awake()
	{
		if (this.appearEvent)
		{
			this.appearEvent.ReceivedEvent += this.Appear;
		}
		if (this.disappearEvent)
		{
			this.disappearEvent.ReceivedEvent += this.Disappear;
		}
		if (this.incrementEvent)
		{
			this.incrementEvent.ReceivedEvent += this.Increment;
		}
		if (this.hitTargetEffect)
		{
			this.hitTargetEffect.SetActive(false);
		}
	}

	// Token: 0x060041A9 RID: 16809 RVA: 0x001210A2 File Offset: 0x0011F2A2
	private void Start()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x060041AA RID: 16810 RVA: 0x001210B0 File Offset: 0x0011F2B0
	public void Appear()
	{
		this.count = 0;
		this.UpdateText();
		base.gameObject.SetActive(true);
	}

	// Token: 0x060041AB RID: 16811 RVA: 0x001210CB File Offset: 0x0011F2CB
	public void Disappear()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x060041AC RID: 16812 RVA: 0x001210D9 File Offset: 0x0011F2D9
	public void Increment()
	{
		this.count++;
		this.UpdateText();
		this.OnIncrement.Invoke();
	}

	// Token: 0x060041AD RID: 16813 RVA: 0x001210FA File Offset: 0x0011F2FA
	public void SetCurrent(int value)
	{
		this.count = value;
		this.UpdateText();
	}

	// Token: 0x060041AE RID: 16814 RVA: 0x00121109 File Offset: 0x0011F309
	public void SetCap(int value)
	{
		this.cap = value;
		this.UpdateText();
	}

	// Token: 0x060041AF RID: 16815 RVA: 0x00121118 File Offset: 0x0011F318
	public void SetTarget(int value)
	{
		this.target = value;
		this.UpdateText();
	}

	// Token: 0x060041B0 RID: 16816 RVA: 0x00121128 File Offset: 0x0011F328
	private void UpdateText()
	{
		this.counterText.text = ((this.cap > 0) ? string.Format("{0}/{1}", this.count, this.cap) : this.count.ToString());
		bool flag = this.target > 0 && this.count > this.target;
		Color color = flag ? UI.MaxItemsTextColor : Color.white;
		this.counterText.color = color;
		if (this.icon)
		{
			this.icon.color = color;
		}
		if (flag && !this.wasAboveTarget && this.hitTargetEffect)
		{
			this.hitTargetEffect.SetActive(true);
		}
		this.wasAboveTarget = flag;
	}

	// Token: 0x0400432E RID: 17198
	[SerializeField]
	private EventRegister appearEvent;

	// Token: 0x0400432F RID: 17199
	[SerializeField]
	private EventRegister disappearEvent;

	// Token: 0x04004330 RID: 17200
	[SerializeField]
	private EventRegister incrementEvent;

	// Token: 0x04004331 RID: 17201
	[SerializeField]
	private TMP_Text counterText;

	// Token: 0x04004332 RID: 17202
	[SerializeField]
	private SpriteRenderer icon;

	// Token: 0x04004333 RID: 17203
	[SerializeField]
	private GameObject hitTargetEffect;

	// Token: 0x04004334 RID: 17204
	[Space]
	public UnityEvent OnIncrement;

	// Token: 0x04004335 RID: 17205
	private int count;

	// Token: 0x04004336 RID: 17206
	private int cap;

	// Token: 0x04004337 RID: 17207
	private int target;

	// Token: 0x04004338 RID: 17208
	private bool wasAboveTarget;
}
