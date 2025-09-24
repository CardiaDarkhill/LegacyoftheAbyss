using System;
using UnityEngine;

// Token: 0x020006ED RID: 1773
public class MenuStyleUnlock : MonoBehaviour
{
	// Token: 0x06003FAF RID: 16303 RVA: 0x00118FA6 File Offset: 0x001171A6
	private void Start()
	{
		MenuStyleUnlock.Unlock(this.unlockKey, this.forceChange);
		if (GameManager.instance.GetPlayerDataInt("permadeathMode") == 1)
		{
			MenuStyleUnlock.Unlock("COMPLETED_STEEL", false);
		}
	}

	// Token: 0x06003FB0 RID: 16304 RVA: 0x00118FD8 File Offset: 0x001171D8
	public static void Unlock(string key, bool forceChange)
	{
		if (string.IsNullOrEmpty(key))
		{
			return;
		}
		if (!GameManager.CanUnlockMenuStyle(key))
		{
			return;
		}
		Platform platform = Platform.Current;
		if (forceChange || !platform.RoamingSharedData.GetBool(key, false))
		{
			platform.RoamingSharedData.SetString("unlockedMenuStyle", key);
		}
		platform.RoamingSharedData.SetBool(key, true);
		platform.RoamingSharedData.Save();
	}

	// Token: 0x04004158 RID: 16728
	[SerializeField]
	private string unlockKey;

	// Token: 0x04004159 RID: 16729
	[SerializeField]
	private bool forceChange = true;
}
