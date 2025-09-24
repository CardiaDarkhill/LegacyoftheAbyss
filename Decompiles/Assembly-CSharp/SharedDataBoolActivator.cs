using System;
using UnityEngine;

// Token: 0x0200046A RID: 1130
public class SharedDataBoolActivator : MonoBehaviour
{
	// Token: 0x06002876 RID: 10358 RVA: 0x000B2786 File Offset: 0x000B0986
	private void Awake()
	{
		this.platform = Platform.Current;
	}

	// Token: 0x06002877 RID: 10359 RVA: 0x000B2794 File Offset: 0x000B0994
	private void OnEnable()
	{
		bool value = this.platform.RoamingSharedData.GetBool(this.key, false) == this.targetValue;
		this.gameObjects.SetAllActive(value);
	}

	// Token: 0x040024A0 RID: 9376
	[SerializeField]
	private string key;

	// Token: 0x040024A1 RID: 9377
	[SerializeField]
	private bool targetValue = true;

	// Token: 0x040024A2 RID: 9378
	[SerializeField]
	private GameObject[] gameObjects;

	// Token: 0x040024A3 RID: 9379
	private Platform platform;
}
