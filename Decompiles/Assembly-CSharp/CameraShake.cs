using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000221 RID: 545
public class CameraShake : MonoBehaviour
{
	// Token: 0x0600144A RID: 5194 RVA: 0x0005B55D File Offset: 0x0005975D
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		CameraShake.cameraShakes = new List<CameraShake>();
	}

	// Token: 0x0600144B RID: 5195 RVA: 0x0005B569 File Offset: 0x00059769
	protected void Awake()
	{
		this.cameraShakeFSM = PlayMakerFSM.FindFsmOnGameObject(base.gameObject, "CameraShake");
	}

	// Token: 0x0600144C RID: 5196 RVA: 0x0005B581 File Offset: 0x00059781
	protected void OnEnable()
	{
		CameraShake.cameraShakes.Add(this);
	}

	// Token: 0x0600144D RID: 5197 RVA: 0x0005B58E File Offset: 0x0005978E
	protected void OnDisable()
	{
		CameraShake.cameraShakes.Remove(this);
	}

	// Token: 0x0600144E RID: 5198 RVA: 0x0005B59C File Offset: 0x0005979C
	public void ShakeSingle(CameraShakeCues cue)
	{
		if (this.cameraShakeFSM != null)
		{
			this.cameraShakeFSM.SendEvent(cue.ToString());
		}
	}

	// Token: 0x0600144F RID: 5199 RVA: 0x0005B5C4 File Offset: 0x000597C4
	public static void Shake(CameraShakeCues cue)
	{
		foreach (CameraShake cameraShake in CameraShake.cameraShakes)
		{
			if (cameraShake != null)
			{
				cameraShake.ShakeSingle(cue);
			}
		}
	}

	// Token: 0x04001284 RID: 4740
	private static List<CameraShake> cameraShakes;

	// Token: 0x04001285 RID: 4741
	private PlayMakerFSM cameraShakeFSM;
}
