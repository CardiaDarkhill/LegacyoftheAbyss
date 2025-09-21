using System;
using UnityEngine;

// Token: 0x02000083 RID: 131
public class FadeSequence : SkippableSequence
{
	// Token: 0x17000040 RID: 64
	// (get) Token: 0x0600039D RID: 925 RVA: 0x000127DE File Offset: 0x000109DE
	public override bool IsSkipped
	{
		get
		{
			return this.childSequence.IsSkipped;
		}
	}

	// Token: 0x17000041 RID: 65
	// (get) Token: 0x0600039E RID: 926 RVA: 0x000127EB File Offset: 0x000109EB
	public float FadeRate
	{
		get
		{
			return this.fadeRate;
		}
	}

	// Token: 0x17000042 RID: 66
	// (get) Token: 0x0600039F RID: 927 RVA: 0x000127F3 File Offset: 0x000109F3
	// (set) Token: 0x060003A0 RID: 928 RVA: 0x000127FB File Offset: 0x000109FB
	public override float FadeByController
	{
		get
		{
			return this.fadeByController;
		}
		set
		{
			this.fadeByController = value;
			this.UpdateFade();
		}
	}

	// Token: 0x17000043 RID: 67
	// (get) Token: 0x060003A1 RID: 929 RVA: 0x0001280A File Offset: 0x00010A0A
	public override bool IsPlaying
	{
		get
		{
			return this.childSequence.IsPlaying || this.fade > Mathf.Epsilon;
		}
	}

	// Token: 0x060003A2 RID: 930 RVA: 0x00012828 File Offset: 0x00010A28
	protected void Awake()
	{
		this.fade = 0f;
		this.timer = 0f;
		this.fadeByController = 1f;
	}

	// Token: 0x060003A3 RID: 931 RVA: 0x0001284B File Offset: 0x00010A4B
	public override void Begin()
	{
		this.fade = 0f;
		this.timer = 0f;
		this.UpdateFade();
		this.childSequence.Begin();
	}

	// Token: 0x060003A4 RID: 932 RVA: 0x00012874 File Offset: 0x00010A74
	protected void Update()
	{
		if (this.childSequence.IsPlaying)
		{
			this.timer += Time.deltaTime;
		}
		float target;
		if (this.timer >= this.fadeDelay && this.childSequence.IsPlaying)
		{
			target = 1f;
		}
		else
		{
			target = 0f;
		}
		this.fade = Mathf.MoveTowards(this.fade, target, this.fadeRate * Time.unscaledDeltaTime);
		this.UpdateFade();
	}

	// Token: 0x060003A5 RID: 933 RVA: 0x000128ED File Offset: 0x00010AED
	public override void Skip()
	{
		this.childSequence.Skip();
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x000128FA File Offset: 0x00010AFA
	private void UpdateFade()
	{
		this.childSequence.FadeByController = Mathf.Clamp01(this.fade * this.fadeByController);
	}

	// Token: 0x04000347 RID: 839
	[SerializeField]
	private SkippableSequence childSequence;

	// Token: 0x04000348 RID: 840
	private float fade;

	// Token: 0x04000349 RID: 841
	private float fadeByController;

	// Token: 0x0400034A RID: 842
	[SerializeField]
	private float fadeDelay;

	// Token: 0x0400034B RID: 843
	private float timer;

	// Token: 0x0400034C RID: 844
	[SerializeField]
	private float fadeRate;
}
