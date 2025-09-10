using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x02000664 RID: 1636
public class GameCameraTextureDisplay : MonoBehaviour
{
	// Token: 0x06003A46 RID: 14918 RVA: 0x000FF60A File Offset: 0x000FD80A
	private void Awake()
	{
		if (!GameCameraTextureDisplay.Instance)
		{
			GameCameraTextureDisplay.Instance = this;
		}
		SceneManager.activeSceneChanged += this.OnActiveSceneChanged;
	}

	// Token: 0x06003A47 RID: 14919 RVA: 0x000FF62F File Offset: 0x000FD82F
	private void Start()
	{
		this.UpdateActiveImage();
	}

	// Token: 0x06003A48 RID: 14920 RVA: 0x000FF638 File Offset: 0x000FD838
	private void OnDestroy()
	{
		SceneManager.activeSceneChanged -= this.OnActiveSceneChanged;
		if (GameCameraTextureDisplay.Instance == this)
		{
			GameCameraTextureDisplay.Instance = null;
		}
		if (this.texture != null)
		{
			this.texture.Release();
			Object.Destroy(this.texture);
			this.texture = null;
		}
	}

	// Token: 0x06003A49 RID: 14921 RVA: 0x000FF694 File Offset: 0x000FD894
	private void OnActiveSceneChanged(Scene arg0, Scene scene)
	{
		this.UpdateActiveImage();
	}

	// Token: 0x06003A4A RID: 14922 RVA: 0x000FF69C File Offset: 0x000FD89C
	private void UpdateActiveImage()
	{
		if (GameManager.instance && GameManager.instance.sceneName != "Menu_Title")
		{
			this.image.enabled = true;
			this.altImage.enabled = false;
			if (this.texture == null)
			{
				this.image.color = Color.black;
				return;
			}
		}
		else
		{
			this.image.enabled = false;
			this.altImage.enabled = true;
		}
	}

	// Token: 0x06003A4B RID: 14923 RVA: 0x000FF71C File Offset: 0x000FD91C
	public void UpdateDisplay(RenderTexture source, Material material)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.texture != null && (this.texture.width != source.width || this.texture.height != source.height))
		{
			this.image.texture = null;
			this.texture.Release();
			this.texture = null;
		}
		if (this.texture == null)
		{
			this.texture = new RenderTexture(source.width, source.height, source.depth);
			this.texture.name = "GameCameraTextureDisplay" + base.GetInstanceID().ToString();
			this.image.texture = this.texture;
			this.image.color = Color.white;
			float num = (float)source.width / (float)source.height;
			RectTransform rectTransform = this.image.rectTransform;
			Vector2 sizeDelta = rectTransform.sizeDelta;
			sizeDelta.x = sizeDelta.y * num;
			rectTransform.sizeDelta = sizeDelta;
		}
		Graphics.Blit(source, this.texture, material);
	}

	// Token: 0x06003A4C RID: 14924 RVA: 0x000FF83F File Offset: 0x000FDA3F
	public void UpdateBrightness(float brightness)
	{
		this.altImage.material.SetFloat(GameCameraTextureDisplay._brightnessProp, brightness);
	}

	// Token: 0x04003CCF RID: 15567
	public static GameCameraTextureDisplay Instance;

	// Token: 0x04003CD0 RID: 15568
	private RenderTexture texture;

	// Token: 0x04003CD1 RID: 15569
	public RawImage image;

	// Token: 0x04003CD2 RID: 15570
	public Image altImage;

	// Token: 0x04003CD3 RID: 15571
	private static readonly int _brightnessProp = Shader.PropertyToID("_Brightness");
}
