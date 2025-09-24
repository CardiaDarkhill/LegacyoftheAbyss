using System;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000162 RID: 354
[ExecuteInEditMode]
public class CameraRenderScaled : MonoBehaviour
{
	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x06000AFF RID: 2815 RVA: 0x00031C7D File Offset: 0x0002FE7D
	// (set) Token: 0x06000B00 RID: 2816 RVA: 0x00031C85 File Offset: 0x0002FE85
	public bool ForceFullResolution { get; set; }

	// Token: 0x06000B01 RID: 2817 RVA: 0x00031C8E File Offset: 0x0002FE8E
	public static void Clear()
	{
		CameraRenderScaled.Resolution = new ScreenRes(0, 0);
	}

	// Token: 0x06000B02 RID: 2818 RVA: 0x00031C9C File Offset: 0x0002FE9C
	private void Awake()
	{
		this.camera = base.GetComponent<Camera>();
		SceneManager.activeSceneChanged += this.OnActiveSceneChanged;
		Scene activeScene = SceneManager.GetActiveScene();
		this.OnActiveSceneChanged(activeScene, activeScene);
	}

	// Token: 0x06000B03 RID: 2819 RVA: 0x00031CD4 File Offset: 0x0002FED4
	private void OnDestroy()
	{
		SceneManager.activeSceneChanged -= this.OnActiveSceneChanged;
		this.ClearRenderTexture();
	}

	// Token: 0x06000B04 RID: 2820 RVA: 0x00031CED File Offset: 0x0002FEED
	private void ClearRenderTexture()
	{
		this.camera.targetTexture = null;
		if (this.renderTex != null)
		{
			this.renderTex.Release();
			Object.Destroy(this.renderTex);
			this.renderTex = null;
		}
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x00031D28 File Offset: 0x0002FF28
	private void OnPreCull()
	{
		if (!Application.isPlaying || !this.applyTo || !this.applyTo.IsActive)
		{
			this.ClearRenderTexture();
			return;
		}
		ScreenRes resolution = CameraRenderScaled.Resolution;
		int width = resolution.Width;
		int height = resolution.Height;
		bool flag = this.ForceFullResolution || CameraRenderScaled.forceFullResolutionV2 || width <= 0 || height <= 0;
		if (flag)
		{
			width = Screen.width;
			height = Screen.height;
		}
		if (this.renderTex != null && (flag || width != this.renderTex.width || height != this.renderTex.height))
		{
			Object.Destroy(this.renderTex);
			this.renderTex = null;
			if (flag && this.applyTo)
			{
				this.applyTo.Texture = null;
				this.applyTo.SourceCamera = null;
			}
		}
		if (flag)
		{
			this.ClearRenderTexture();
			return;
		}
		if (this.renderTex != null)
		{
			return;
		}
		this.renderTex = new RenderTexture(width, height, 32, RenderTextureFormat.Default)
		{
			hideFlags = HideFlags.HideAndDontSave,
			name = "CameraRenderScaled" + base.GetInstanceID().ToString()
		};
		this.camera.targetTexture = this.renderTex;
		if (this.applyTo)
		{
			this.applyTo.Texture = this.renderTex;
			this.applyTo.SourceCamera = this.camera;
		}
	}

	// Token: 0x06000B06 RID: 2822 RVA: 0x00031E94 File Offset: 0x00030094
	private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
	{
		string name = newScene.name;
		bool forceFullResolution = name.IsAny(WorldInfo.MenuScenes) || name.IsAny(WorldInfo.NonGameplayScenes);
		CameraRenderScaled.fullResotionSources.RemoveWhere((MonoBehaviour o) => o == null);
		this.ForceFullResolution = forceFullResolution;
	}

	// Token: 0x06000B07 RID: 2823 RVA: 0x00031EF6 File Offset: 0x000300F6
	public static void AddForceFullResolution(MonoBehaviour source)
	{
		if (source == null)
		{
			return;
		}
		if (CameraRenderScaled.fullResotionSources.Add(source))
		{
			CameraRenderScaled.forceFullResolutionV2 = true;
		}
	}

	// Token: 0x06000B08 RID: 2824 RVA: 0x00031F15 File Offset: 0x00030115
	public static void RemoveForceFullResolution(MonoBehaviour source)
	{
		if (CameraRenderScaled.fullResotionSources.Remove(source) && CameraRenderScaled.fullResotionSources.Count == 0)
		{
			CameraRenderScaled.forceFullResolutionV2 = false;
		}
	}

	// Token: 0x04000A81 RID: 2689
	[SerializeField]
	private CameraRenderScaledApply applyTo;

	// Token: 0x04000A82 RID: 2690
	private RenderTexture renderTex;

	// Token: 0x04000A83 RID: 2691
	private Camera camera;

	// Token: 0x04000A84 RID: 2692
	public static ScreenRes Resolution;

	// Token: 0x04000A86 RID: 2694
	private static bool forceFullResolutionV2;

	// Token: 0x04000A87 RID: 2695
	private static HashSet<MonoBehaviour> fullResotionSources = new HashSet<MonoBehaviour>();
}
