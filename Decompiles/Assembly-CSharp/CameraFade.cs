using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200015D RID: 349
public class CameraFade : MonoBehaviour, IOnGUI
{
	// Token: 0x06000AC2 RID: 2754 RVA: 0x00030EA8 File Offset: 0x0002F0A8
	private void Awake()
	{
		this.fadeTexture = new Texture2D(1, 1);
		this.fadeTexture.name = string.Format("{0} Fade Texture", this);
		this.backgroundStyle.normal.background = this.fadeTexture;
	}

	// Token: 0x06000AC3 RID: 2755 RVA: 0x00030EE3 File Offset: 0x0002F0E3
	private IEnumerator Start()
	{
		if (this.fadeOnStart == CameraFade.FadeTypes.BLACK_TO_CLEAR)
		{
			this.SetScreenOverlayColor(new Color(0f, 0f, 0f, 1f));
		}
		else if (this.fadeOnStart == CameraFade.FadeTypes.CLEAR_TO_BLACK)
		{
			this.SetScreenOverlayColor(new Color(0f, 0f, 0f, 0f));
		}
		if (this.startDelay > 0f)
		{
			yield return new WaitForSeconds(this.startDelay);
		}
		else
		{
			yield return new WaitForEndOfFrame();
		}
		if (this.fadeOnStart == CameraFade.FadeTypes.BLACK_TO_CLEAR)
		{
			this.FadeToTransparent(this.fadeTime);
		}
		else if (this.fadeOnStart == CameraFade.FadeTypes.CLEAR_TO_BLACK)
		{
			this.FadeToBlack(this.fadeTime);
		}
		yield break;
	}

	// Token: 0x06000AC4 RID: 2756 RVA: 0x00030EF2 File Offset: 0x0002F0F2
	private void OnDisable()
	{
		this.ToggleDrawGUI(false);
	}

	// Token: 0x06000AC5 RID: 2757 RVA: 0x00030EFB File Offset: 0x0002F0FB
	private void OnDestroy()
	{
		if (this.fadeTexture != null)
		{
			Object.Destroy(this.fadeTexture);
			this.fadeTexture = null;
		}
	}

	// Token: 0x06000AC6 RID: 2758 RVA: 0x00030F20 File Offset: 0x0002F120
	private void Update()
	{
		if (this.currentScreenOverlayColor != this.targetScreenOverlayColor)
		{
			if (Mathf.Abs(this.currentScreenOverlayColor.a - this.targetScreenOverlayColor.a) < Mathf.Abs(this.deltaColor.a) * Time.deltaTime)
			{
				this.desiredOverlayColor = this.targetScreenOverlayColor;
				this.SetScreenOverlayColor(this.desiredOverlayColor);
				this.deltaColor = new Color(0f, 0f, 0f, 0f);
			}
			else
			{
				this.desiredOverlayColor = this.currentScreenOverlayColor + this.deltaColor * Time.deltaTime;
			}
		}
		if (this.desiredOverlayColor.a > 0f)
		{
			this.ToggleDrawGUI(true);
			return;
		}
		this.ToggleDrawGUI(false);
	}

	// Token: 0x06000AC7 RID: 2759 RVA: 0x00030FF2 File Offset: 0x0002F1F2
	private void ToggleDrawGUI(bool draw)
	{
		if (this.drawGUI != draw)
		{
			if (draw)
			{
				GUIDrawer.AddDrawer(this);
				return;
			}
			GUIDrawer.RemoveDrawer(this);
		}
	}

	// Token: 0x170000EE RID: 238
	// (get) Token: 0x06000AC8 RID: 2760 RVA: 0x0003100D File Offset: 0x0002F20D
	public int GUIDepth
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x06000AC9 RID: 2761 RVA: 0x00031010 File Offset: 0x0002F210
	public void DrawGUI()
	{
		this.SetScreenOverlayColor(this.desiredOverlayColor);
		if (this.currentScreenOverlayColor.a > 0f)
		{
			GUI.depth = this.fadeGUIDepth;
			GUI.Label(new Rect(-10f, -10f, (float)(Screen.width + 10), (float)(Screen.height + 10)), this.fadeTexture, this.backgroundStyle);
		}
	}

	// Token: 0x06000ACA RID: 2762 RVA: 0x00031078 File Offset: 0x0002F278
	public void SetScreenOverlayColor(Color newScreenOverlayColor)
	{
		if (this.currentScreenOverlayColor != newScreenOverlayColor)
		{
			this.currentScreenOverlayColor = newScreenOverlayColor;
			this.fadeTexture.SetPixel(0, 0, this.currentScreenOverlayColor);
			this.fadeTexture.Apply();
		}
	}

	// Token: 0x06000ACB RID: 2763 RVA: 0x000310AD File Offset: 0x0002F2AD
	public void StartFade(Color newScreenOverlayColor, float fadeDuration)
	{
		if (fadeDuration <= 0f)
		{
			this.SetScreenOverlayColor(newScreenOverlayColor);
			return;
		}
		this.targetScreenOverlayColor = newScreenOverlayColor;
		this.deltaColor = (this.targetScreenOverlayColor - this.currentScreenOverlayColor) / (fadeDuration * 2f);
	}

	// Token: 0x06000ACC RID: 2764 RVA: 0x000310EC File Offset: 0x0002F2EC
	public void FadeToBlack(float duration)
	{
		this.SetScreenOverlayColor(new Color(0f, 0f, 0f, 0f));
		this.StartFade(new Color(0f, 0f, 0f, 1f), duration);
	}

	// Token: 0x06000ACD RID: 2765 RVA: 0x00031138 File Offset: 0x0002F338
	public void FadeToTransparent(float duration)
	{
		this.SetScreenOverlayColor(new Color(0f, 0f, 0f, 1f));
		this.StartFade(new Color(0f, 0f, 0f, 0f), duration);
	}

	// Token: 0x04000A49 RID: 2633
	private GUIStyle backgroundStyle = new GUIStyle();

	// Token: 0x04000A4A RID: 2634
	private Texture2D fadeTexture;

	// Token: 0x04000A4B RID: 2635
	private Color currentScreenOverlayColor = new Color(0f, 0f, 0f, 0f);

	// Token: 0x04000A4C RID: 2636
	private Color targetScreenOverlayColor = new Color(0f, 0f, 0f, 1f);

	// Token: 0x04000A4D RID: 2637
	private Color desiredOverlayColor;

	// Token: 0x04000A4E RID: 2638
	private Color deltaColor = new Color(0f, 0f, 0f, 0f);

	// Token: 0x04000A4F RID: 2639
	private int fadeGUIDepth = -1000;

	// Token: 0x04000A50 RID: 2640
	[Header("Fade On Scene Start")]
	[Space(6f)]
	[Tooltip("Type of fade to do on Start.")]
	public CameraFade.FadeTypes fadeOnStart;

	// Token: 0x04000A51 RID: 2641
	[Tooltip("The time in seconds to wait after Start before performing the delay.")]
	public float startDelay;

	// Token: 0x04000A52 RID: 2642
	[Tooltip("Time to perform fade in seconds on Start.")]
	public float fadeTime;

	// Token: 0x04000A53 RID: 2643
	private bool drawGUI;

	// Token: 0x02001492 RID: 5266
	public enum FadeTypes
	{
		// Token: 0x040083BC RID: 33724
		NONE,
		// Token: 0x040083BD RID: 33725
		BLACK_TO_CLEAR,
		// Token: 0x040083BE RID: 33726
		CLEAR_TO_BLACK
	}
}
