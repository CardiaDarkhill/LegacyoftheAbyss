using System;
using UnityEngine;

// Token: 0x020006CA RID: 1738
public class LoadingCanvas : MonoBehaviour
{
	// Token: 0x06003ED0 RID: 16080 RVA: 0x0011465B File Offset: 0x0011285B
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<LoadingSpinner>(ref this.visualizationContainers, typeof(GameManager.SceneLoadVisualizations));
	}

	// Token: 0x06003ED1 RID: 16081 RVA: 0x00114672 File Offset: 0x00112872
	private void Awake()
	{
		this.OnValidate();
	}

	// Token: 0x06003ED2 RID: 16082 RVA: 0x0011467C File Offset: 0x0011287C
	protected void Start()
	{
		foreach (LoadingSpinner loadingSpinner in this.visualizationContainers)
		{
			if (!(loadingSpinner == null))
			{
				loadingSpinner.SetActive(false, true);
			}
		}
	}

	// Token: 0x06003ED3 RID: 16083 RVA: 0x001146B4 File Offset: 0x001128B4
	protected void Update()
	{
		GameManager silentInstance = GameManager.SilentInstance;
		if (silentInstance == null)
		{
			return;
		}
		if (this.isLoading == silentInstance.IsLoadingSceneTransition)
		{
			return;
		}
		this.isLoading = silentInstance.IsLoadingSceneTransition;
		if (this.isLoading)
		{
			LoadingSpinner loadingSpinner = this.defaultLoadingSpinner;
			GameManager.SceneLoadVisualizations loadVisualization = silentInstance.LoadVisualization;
			float displayDelayAdjustment;
			if (loadVisualization != GameManager.SceneLoadVisualizations.ContinueFromSave)
			{
				if (loadVisualization != GameManager.SceneLoadVisualizations.ThreadMemory)
				{
					displayDelayAdjustment = 0f;
				}
				else
				{
					displayDelayAdjustment = this.threadMemoryDelayAdjustment;
				}
			}
			else
			{
				displayDelayAdjustment = this.continueFromSaveDelayAdjustment;
			}
			loadingSpinner.DisplayDelayAdjustment = displayDelayAdjustment;
		}
		LoadingSpinner y = null;
		if (this.isLoading && silentInstance.LoadVisualization >= GameManager.SceneLoadVisualizations.Default && silentInstance.LoadVisualization < (GameManager.SceneLoadVisualizations)this.visualizationContainers.Length)
		{
			y = this.visualizationContainers[(int)silentInstance.LoadVisualization];
		}
		foreach (LoadingSpinner loadingSpinner2 in this.visualizationContainers)
		{
			if (!(loadingSpinner2 == null))
			{
				loadingSpinner2.SetActive(loadingSpinner2 == y, false);
			}
		}
	}

	// Token: 0x0400406A RID: 16490
	[SerializeField]
	[ArrayForEnum(typeof(GameManager.SceneLoadVisualizations))]
	private LoadingSpinner[] visualizationContainers;

	// Token: 0x0400406B RID: 16491
	[SerializeField]
	private LoadingSpinner defaultLoadingSpinner;

	// Token: 0x0400406C RID: 16492
	[SerializeField]
	private float continueFromSaveDelayAdjustment;

	// Token: 0x0400406D RID: 16493
	[SerializeField]
	private float threadMemoryDelayAdjustment;

	// Token: 0x0400406E RID: 16494
	private bool isLoading;

	// Token: 0x0400406F RID: 16495
	private GameManager.SceneLoadVisualizations loadingVisualization;
}
