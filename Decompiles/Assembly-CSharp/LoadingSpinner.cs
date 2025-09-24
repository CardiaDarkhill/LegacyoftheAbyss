using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006CC RID: 1740
public class LoadingSpinner : MonoBehaviour
{
	// Token: 0x17000740 RID: 1856
	// (get) Token: 0x06003ED8 RID: 16088 RVA: 0x0011488D File Offset: 0x00112A8D
	// (set) Token: 0x06003ED9 RID: 16089 RVA: 0x00114895 File Offset: 0x00112A95
	public float DisplayDelayAdjustment { get; set; }

	// Token: 0x17000741 RID: 1857
	// (get) Token: 0x06003EDA RID: 16090 RVA: 0x0011489E File Offset: 0x00112A9E
	public float DisplayDelay
	{
		get
		{
			return this.displayDelay + this.DisplayDelayAdjustment;
		}
	}

	// Token: 0x06003EDB RID: 16091 RVA: 0x001148AD File Offset: 0x00112AAD
	protected void OnEnable()
	{
		this.fadeFactor = 0f;
		if (this.frameRate <= 0f)
		{
			this.frameRate = 12f;
		}
	}

	// Token: 0x06003EDC RID: 16092 RVA: 0x001148D2 File Offset: 0x00112AD2
	protected void Start()
	{
		this.image.color = new Color(1f, 1f, 1f, 0f);
		this.image.enabled = false;
		this.fadeFactor = 0f;
	}

	// Token: 0x06003EDD RID: 16093 RVA: 0x00114910 File Offset: 0x00112B10
	protected void Update()
	{
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		GameManager silentInstance = GameManager.SilentInstance;
		if (silentInstance != null)
		{
			float target;
			if (!this.targetActive)
			{
				target = 0f;
			}
			else if (silentInstance.CurrentLoadDuration > this.DisplayDelay)
			{
				target = 1f;
			}
			else
			{
				target = 0f;
			}
			this.fadeFactor = Mathf.MoveTowards(this.fadeFactor, target, unscaledDeltaTime / this.fadeDuration);
			if (this.fadeFactor < Mathf.Epsilon)
			{
				if (this.image.enabled)
				{
					this.image.enabled = false;
				}
				if (!this.targetActive)
				{
					base.gameObject.SetActive(false);
				}
			}
			else
			{
				if (!this.image.enabled)
				{
					this.image.enabled = true;
				}
				this.image.color = new Color(1f, 1f, 1f, this.fadeFactor * (this.fadeAmount + this.fadeVariance * Mathf.Sin(silentInstance.CurrentLoadDuration * 3.1415927f * 2f / this.fadePulseDuration)));
			}
		}
		if (this.sprites.Length != 0)
		{
			this.frameTimer += unscaledDeltaTime * this.frameRate;
			if (this.frameTimer >= 1f)
			{
				int num = Mathf.FloorToInt(this.frameTimer);
				this.frameTimer -= (float)num;
				this.frameIndex = (this.frameIndex + num) % this.sprites.Length;
				this.SetImage(this.sprites[this.frameIndex]);
			}
		}
	}

	// Token: 0x06003EDE RID: 16094 RVA: 0x00114A92 File Offset: 0x00112C92
	private void SetImage(Sprite sprite)
	{
		if (this.image.sprite != sprite)
		{
			this.image.sprite = sprite;
		}
	}

	// Token: 0x06003EDF RID: 16095 RVA: 0x00114AB3 File Offset: 0x00112CB3
	public void SetActive(bool value, bool isInstant)
	{
		this.targetActive = value;
		if (value)
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
				return;
			}
		}
		else if (isInstant)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x04004077 RID: 16503
	[SerializeField]
	private Image image;

	// Token: 0x04004078 RID: 16504
	[SerializeField]
	private float displayDelay;

	// Token: 0x04004079 RID: 16505
	[SerializeField]
	private float fadeDuration;

	// Token: 0x0400407A RID: 16506
	[SerializeField]
	private float fadeAmount;

	// Token: 0x0400407B RID: 16507
	[SerializeField]
	private float fadeVariance;

	// Token: 0x0400407C RID: 16508
	[SerializeField]
	private float fadePulseDuration;

	// Token: 0x0400407D RID: 16509
	[SerializeField]
	private Sprite[] sprites;

	// Token: 0x0400407E RID: 16510
	[SerializeField]
	private float frameRate;

	// Token: 0x0400407F RID: 16511
	private float fadeFactor;

	// Token: 0x04004080 RID: 16512
	private float frameTimer;

	// Token: 0x04004081 RID: 16513
	private int frameIndex;

	// Token: 0x04004082 RID: 16514
	private bool targetActive;
}
