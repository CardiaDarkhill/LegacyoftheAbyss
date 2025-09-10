using System;
using UnityEngine;

// Token: 0x020007D3 RID: 2003
public sealed class SetMappedOnStart : MonoBehaviour
{
	// Token: 0x0600468D RID: 18061 RVA: 0x00131C24 File Offset: 0x0012FE24
	private void Start()
	{
		PlayerData instance = PlayerData.instance;
		if (instance.hasQuill || instance.QuillState > 0)
		{
			instance.scenesMapped.Add(this.sceneName);
		}
	}

	// Token: 0x0600468E RID: 18062 RVA: 0x00131C5C File Offset: 0x0012FE5C
	private void Reset()
	{
		this.sceneName = base.gameObject.scene.name;
	}

	// Token: 0x040046F6 RID: 18166
	[SerializeField]
	private string sceneName;
}
