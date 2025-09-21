using System;
using UnityEngine;

// Token: 0x020001FF RID: 511
public class DeactivateOnHeroRespawnPoint : MonoBehaviour
{
	// Token: 0x0600136B RID: 4971 RVA: 0x000589FC File Offset: 0x00056BFC
	private void OnEnable()
	{
		PlayerData instance = PlayerData.instance;
		string respawnMarkerName = instance.respawnMarkerName;
		string respawnScene = instance.respawnScene;
		if (string.IsNullOrEmpty(respawnMarkerName) || string.IsNullOrEmpty(respawnScene))
		{
			return;
		}
		if (respawnMarkerName == this.respawnPointName && respawnScene == this.respawnSceneName)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x040011D8 RID: 4568
	[SerializeField]
	private string respawnPointName;

	// Token: 0x040011D9 RID: 4569
	[SerializeField]
	private string respawnSceneName;
}
