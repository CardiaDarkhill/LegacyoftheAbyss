using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000730 RID: 1840
public class StandaloneLoadingSpinner : MonoBehaviour
{
	// Token: 0x060041C0 RID: 16832 RVA: 0x00121542 File Offset: 0x0011F742
	private void OnValidate()
	{
		if (this.frameRate <= 0f)
		{
			this.frameRate = 12f;
		}
	}

	// Token: 0x060041C1 RID: 16833 RVA: 0x0012155C File Offset: 0x0011F75C
	public void Setup(GameManager lastGameManager)
	{
		this.lastGameManager = lastGameManager;
	}

	// Token: 0x060041C2 RID: 16834 RVA: 0x00121565 File Offset: 0x0011F765
	protected void OnEnable()
	{
		this.fadeFactor = 0f;
		if (this.frameRate <= 0f)
		{
			this.frameRate = 12f;
		}
	}

	// Token: 0x060041C3 RID: 16835 RVA: 0x0012158C File Offset: 0x0011F78C
	protected void Start()
	{
		this.image.color = new Color(1f, 1f, 1f, 0f);
		this.image.enabled = false;
		this.fadeFactor = 0f;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x060041C4 RID: 16836 RVA: 0x001215E0 File Offset: 0x0011F7E0
	protected void LateUpdate()
	{
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if (this.lastGameManager == null && unsafeInstance != null && (this.lastGameManager != unsafeInstance || this.lastGameManager == null) && !this.isComplete)
		{
			this.renderingCamera.enabled = false;
			this.isComplete = true;
		}
		this.timeRunning += Time.unscaledDeltaTime;
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		float target = (this.timeRunning > this.displayDelay && !this.isComplete) ? 1f : 0f;
		this.fadeFactor = Mathf.MoveTowards(this.fadeFactor, target, unscaledDeltaTime / this.fadeDuration);
		if (this.fadeFactor < Mathf.Epsilon)
		{
			if (this.image.enabled)
			{
				this.image.enabled = false;
			}
			if (this.isComplete)
			{
				Object.Destroy(base.gameObject);
			}
		}
		else
		{
			if (!this.image.enabled)
			{
				this.image.enabled = true;
			}
			this.image.color = new Color(1f, 1f, 1f, this.fadeFactor * (this.fadeAmount + this.fadeVariance * Mathf.Sin(this.timeRunning * 3.1415927f * 2f / this.fadePulseDuration)));
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

	// Token: 0x060041C5 RID: 16837 RVA: 0x001217AC File Offset: 0x0011F9AC
	private void SetImage(Sprite sprite)
	{
		if (this.image.sprite != sprite)
		{
			this.image.sprite = sprite;
		}
	}

	// Token: 0x0400434B RID: 17227
	[SerializeField]
	private Camera renderingCamera;

	// Token: 0x0400434C RID: 17228
	[SerializeField]
	private Image backgroundImage;

	// Token: 0x0400434D RID: 17229
	[SerializeField]
	private Image image;

	// Token: 0x0400434E RID: 17230
	[SerializeField]
	private float displayDelay;

	// Token: 0x0400434F RID: 17231
	[SerializeField]
	private float fadeDuration;

	// Token: 0x04004350 RID: 17232
	[SerializeField]
	private float fadeAmount;

	// Token: 0x04004351 RID: 17233
	[SerializeField]
	private float fadeVariance;

	// Token: 0x04004352 RID: 17234
	[SerializeField]
	private float fadePulseDuration;

	// Token: 0x04004353 RID: 17235
	[SerializeField]
	private Sprite[] sprites;

	// Token: 0x04004354 RID: 17236
	[SerializeField]
	private float frameRate;

	// Token: 0x04004355 RID: 17237
	private float fadeFactor;

	// Token: 0x04004356 RID: 17238
	private float frameTimer;

	// Token: 0x04004357 RID: 17239
	private int frameIndex;

	// Token: 0x04004358 RID: 17240
	private float timeRunning;

	// Token: 0x04004359 RID: 17241
	private bool isComplete;

	// Token: 0x0400435A RID: 17242
	private GameManager lastGameManager;
}
