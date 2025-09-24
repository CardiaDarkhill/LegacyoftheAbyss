using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200006F RID: 111
public class CheckpointSprite : MonoBehaviour
{
	// Token: 0x060002F4 RID: 756 RVA: 0x0000FF71 File Offset: 0x0000E171
	protected void Awake()
	{
		this.image = base.GetComponent<Image>();
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x0000FF8C File Offset: 0x0000E18C
	protected void OnEnable()
	{
		this.state = CheckpointSprite.States.NotStarted;
		this.image.enabled = false;
		this.Update(0f);
		GameManager instance = GameManager.instance;
		if (instance)
		{
			instance.NextSceneWillActivate += this.StopAudio;
		}
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x0000FFD8 File Offset: 0x0000E1D8
	private void OnDisable()
	{
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if (unsafeInstance)
		{
			unsafeInstance.NextSceneWillActivate -= this.StopAudio;
		}
	}

	// Token: 0x060002F7 RID: 759 RVA: 0x00010005 File Offset: 0x0000E205
	private void StopAudio()
	{
		if (this.audioSource)
		{
			this.audioSource.Stop();
		}
	}

	// Token: 0x060002F8 RID: 760 RVA: 0x0001001F File Offset: 0x0000E21F
	[ContextMenu("Show")]
	public void Show()
	{
		this.isShowing = true;
		if (base.isActiveAndEnabled)
		{
			this.Update(0f);
		}
	}

	// Token: 0x060002F9 RID: 761 RVA: 0x0001003B File Offset: 0x0000E23B
	[ContextMenu("Hide")]
	public void Hide()
	{
		this.isShowing = false;
	}

	// Token: 0x060002FA RID: 762 RVA: 0x00010044 File Offset: 0x0000E244
	protected void Update()
	{
		this.Update(Mathf.Min(0.016666668f, Time.unscaledDeltaTime));
	}

	// Token: 0x060002FB RID: 763 RVA: 0x0001005C File Offset: 0x0000E25C
	private void Update(float deltaTime)
	{
		this.frameTimer += deltaTime * this.framesPerSecond;
		if (this.state == CheckpointSprite.States.NotStarted && this.isShowing)
		{
			this.frameTimer = 0f;
			this.state = CheckpointSprite.States.Starting;
			this.audioSource.Play();
			this.image.enabled = true;
		}
		if (this.state == CheckpointSprite.States.Starting)
		{
			int num = (int)this.frameTimer;
			if (num < this.startSprites.Length)
			{
				this.SetImageSprite(this.startSprites[num]);
			}
			else
			{
				this.frameTimer -= (float)this.startSprites.Length;
				if (this.isShowing)
				{
					this.state = CheckpointSprite.States.Looping;
				}
				else
				{
					this.state = CheckpointSprite.States.Ending;
				}
			}
		}
		if (this.state == CheckpointSprite.States.Looping)
		{
			int num2 = (int)this.frameTimer;
			if (num2 >= this.loopSprites.Length)
			{
				this.frameTimer -= (float)(this.loopSprites.Length * (num2 / this.loopSprites.Length));
				if (!this.isShowing)
				{
					this.state = CheckpointSprite.States.Ending;
				}
				else
				{
					this.SetImageSprite(this.loopSprites[num2 % this.loopSprites.Length]);
				}
			}
			else
			{
				this.SetImageSprite(this.loopSprites[num2]);
			}
		}
		if (this.state == CheckpointSprite.States.Ending)
		{
			int num3 = (int)this.frameTimer;
			if (num3 < this.endSprites.Length)
			{
				this.SetImageSprite(this.endSprites[num3]);
				return;
			}
			this.image.enabled = false;
			this.state = CheckpointSprite.States.NotStarted;
		}
	}

	// Token: 0x060002FC RID: 764 RVA: 0x000101C4 File Offset: 0x0000E3C4
	private void SetImageSprite(Sprite sprite)
	{
		if (this.image.sprite != sprite)
		{
			this.image.sprite = sprite;
		}
	}

	// Token: 0x0400027C RID: 636
	private Image image;

	// Token: 0x0400027D RID: 637
	private AudioSource audioSource;

	// Token: 0x0400027E RID: 638
	[SerializeField]
	private Sprite[] startSprites;

	// Token: 0x0400027F RID: 639
	[SerializeField]
	private Sprite[] loopSprites;

	// Token: 0x04000280 RID: 640
	[SerializeField]
	private Sprite[] endSprites;

	// Token: 0x04000281 RID: 641
	[SerializeField]
	private float framesPerSecond;

	// Token: 0x04000282 RID: 642
	private bool isShowing;

	// Token: 0x04000283 RID: 643
	private CheckpointSprite.States state;

	// Token: 0x04000284 RID: 644
	private float frameTimer;

	// Token: 0x020013E5 RID: 5093
	private enum States
	{
		// Token: 0x04008111 RID: 33041
		NotStarted,
		// Token: 0x04008112 RID: 33042
		Starting,
		// Token: 0x04008113 RID: 33043
		Looping,
		// Token: 0x04008114 RID: 33044
		Ending
	}
}
