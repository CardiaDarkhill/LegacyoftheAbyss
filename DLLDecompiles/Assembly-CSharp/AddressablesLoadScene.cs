using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

// Token: 0x02000412 RID: 1042
public class AddressablesLoadScene : MonoBehaviour
{
	// Token: 0x1700039E RID: 926
	// (get) Token: 0x0600234F RID: 9039 RVA: 0x000A1672 File Offset: 0x0009F872
	private bool HasSceneRef
	{
		get
		{
			return !string.IsNullOrEmpty(this.loadScene.AssetGUID);
		}
	}

	// Token: 0x06002350 RID: 9040 RVA: 0x000A1687 File Offset: 0x0009F887
	private void Start()
	{
		if (this.HasSceneRef)
		{
			Addressables.LoadSceneAsync(this.loadScene, LoadSceneMode.Single, true, 100, SceneReleaseMode.ReleaseSceneWhenSceneUnloaded);
			return;
		}
		Addressables.LoadSceneAsync(this.address, LoadSceneMode.Single, true, 100, SceneReleaseMode.ReleaseSceneWhenSceneUnloaded);
	}

	// Token: 0x040021EC RID: 8684
	[SerializeField]
	private AssetReference loadScene;

	// Token: 0x040021ED RID: 8685
	[SerializeField]
	[ModifiableProperty]
	[Conditional("HasSceneRef", false, false, false)]
	private string address;
}
