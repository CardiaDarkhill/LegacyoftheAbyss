using System;
using System.Collections;
using TMProOld;
using UnityEngine;

// Token: 0x0200017B RID: 379
public class ColorFader : MonoBehaviour
{
	// Token: 0x1400001C RID: 28
	// (add) Token: 0x06000C69 RID: 3177 RVA: 0x00036AFC File Offset: 0x00034CFC
	// (remove) Token: 0x06000C6A RID: 3178 RVA: 0x00036B34 File Offset: 0x00034D34
	public event ColorFader.FadeEndEvent OnFadeEnd;

	// Token: 0x06000C6B RID: 3179 RVA: 0x00036B6C File Offset: 0x00034D6C
	private void Reset()
	{
		foreach (PlayMakerFSM playMakerFSM in base.GetComponents<PlayMakerFSM>())
		{
			if ((playMakerFSM.FsmTemplate ? playMakerFSM.FsmTemplate.name : playMakerFSM.FsmName) == "color_fader")
			{
				this.downColour = playMakerFSM.FsmVariables.GetFsmColor("Down Colour").Value;
				this.downTime = playMakerFSM.FsmVariables.GetFsmFloat("Down Time").Value;
				this.upColour = playMakerFSM.FsmVariables.GetFsmColor("Up Colour").Value;
				this.upDelay = playMakerFSM.FsmVariables.GetFsmFloat("Up Delay").Value;
				this.upTime = playMakerFSM.FsmVariables.GetFsmFloat("Up Time").Value;
				return;
			}
		}
	}

	// Token: 0x06000C6C RID: 3180 RVA: 0x00036C4F File Offset: 0x00034E4F
	private void Start()
	{
		this.Setup();
	}

	// Token: 0x06000C6D RID: 3181 RVA: 0x00036C58 File Offset: 0x00034E58
	private void Setup()
	{
		if (!this.setup)
		{
			this.setup = true;
			if (!this.spriteRenderer)
			{
				this.spriteRenderer = base.GetComponent<SpriteRenderer>();
			}
			this.hasSpriteRenderer = (this.spriteRenderer != null);
			if (this.hasSpriteRenderer)
			{
				this.initialColour = (this.useInitialColour ? this.spriteRenderer.color : Color.white);
				this.spriteRenderer.color = this.downColour * this.initialColour;
				return;
			}
			if (!this.textRenderer)
			{
				this.textRenderer = base.GetComponent<TextMeshPro>();
			}
			this.hasTextRenderer = (this.textRenderer != null);
			if (this.hasTextRenderer)
			{
				this.initialColour = (this.useInitialColour ? this.textRenderer.color : Color.white);
				this.textRenderer.color = this.downColour * this.initialColour;
				return;
			}
			if (!this.tk2dSprite)
			{
				this.tk2dSprite = base.GetComponent<tk2dSprite>();
			}
			this.hasTk2dSprite = (this.tk2dSprite != null);
			if (this.hasTk2dSprite)
			{
				this.initialColour = (this.useInitialColour ? this.tk2dSprite.color : Color.white);
				this.tk2dSprite.color = this.downColour * this.initialColour;
			}
		}
	}

	// Token: 0x06000C6E RID: 3182 RVA: 0x00036DC8 File Offset: 0x00034FC8
	public void Fade(bool up)
	{
		this.Setup();
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
		}
		if (up)
		{
			this.fadeRoutine = base.StartCoroutine(this.Fade(this.upColour, this.upTime, this.upDelay));
			return;
		}
		this.fadeRoutine = base.StartCoroutine(this.Fade(this.downColour, this.downTime, 0f));
	}

	// Token: 0x06000C6F RID: 3183 RVA: 0x00036E3A File Offset: 0x0003503A
	private IEnumerator Fade(Color to, float time, float delay)
	{
		Color from = this.hasSpriteRenderer ? this.spriteRenderer.color : (this.hasTextRenderer ? this.textRenderer.color : (this.hasTk2dSprite ? this.tk2dSprite.color : Color.white));
		if (delay > 0f)
		{
			yield return new WaitForSeconds(this.upDelay);
		}
		for (float elapsed = 0f; elapsed < time; elapsed += Time.deltaTime)
		{
			Color color = Color.Lerp(from, to, elapsed / time) * this.initialColour;
			if (this.hasSpriteRenderer)
			{
				this.spriteRenderer.color = color;
			}
			else if (this.hasTextRenderer)
			{
				this.textRenderer.color = color;
			}
			else if (this.hasTk2dSprite)
			{
				this.tk2dSprite.color = color;
			}
			yield return null;
		}
		if (this.hasSpriteRenderer)
		{
			this.spriteRenderer.color = to * this.initialColour;
		}
		else if (this.hasTextRenderer)
		{
			this.textRenderer.color = to * this.initialColour;
		}
		else if (this.hasTk2dSprite)
		{
			this.tk2dSprite.color = to * this.initialColour;
		}
		if (this.OnFadeEnd != null)
		{
			this.OnFadeEnd(to == this.upColour);
		}
		yield break;
	}

	// Token: 0x06000C70 RID: 3184 RVA: 0x00036E5E File Offset: 0x0003505E
	public void SetUpTime(float newUpTime)
	{
		this.upTime = newUpTime;
	}

	// Token: 0x06000C71 RID: 3185 RVA: 0x00036E67 File Offset: 0x00035067
	public void SetUpDelay(float newUpDelay)
	{
		this.upDelay = newUpDelay;
	}

	// Token: 0x04000BE1 RID: 3041
	public Color downColour = new Color(1f, 1f, 1f, 0f);

	// Token: 0x04000BE2 RID: 3042
	public float downTime = 0.5f;

	// Token: 0x04000BE3 RID: 3043
	public Color upColour = new Color(1f, 1f, 1f, 1f);

	// Token: 0x04000BE4 RID: 3044
	public float upDelay;

	// Token: 0x04000BE5 RID: 3045
	public float upTime = 0.4f;

	// Token: 0x04000BE6 RID: 3046
	private Color initialColour;

	// Token: 0x04000BE7 RID: 3047
	public bool useInitialColour = true;

	// Token: 0x04000BE8 RID: 3048
	private SpriteRenderer spriteRenderer;

	// Token: 0x04000BE9 RID: 3049
	private TextMeshPro textRenderer;

	// Token: 0x04000BEA RID: 3050
	private tk2dSprite tk2dSprite;

	// Token: 0x04000BEB RID: 3051
	private bool setup;

	// Token: 0x04000BEC RID: 3052
	private bool hasSpriteRenderer;

	// Token: 0x04000BED RID: 3053
	private bool hasTextRenderer;

	// Token: 0x04000BEE RID: 3054
	private bool hasTk2dSprite;

	// Token: 0x04000BEF RID: 3055
	private Coroutine fadeRoutine;

	// Token: 0x020014AC RID: 5292
	// (Invoke) Token: 0x0600843F RID: 33855
	public delegate void FadeEndEvent(bool up);
}
