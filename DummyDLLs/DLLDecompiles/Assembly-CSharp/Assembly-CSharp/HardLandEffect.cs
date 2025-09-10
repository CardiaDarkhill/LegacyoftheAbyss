using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x02000241 RID: 577
public class HardLandEffect : MonoBehaviour
{
	// Token: 0x0600151C RID: 5404 RVA: 0x0005F8AC File Offset: 0x0005DAAC
	private void OnEnable()
	{
		this.dustObj.SetActive(false);
		this.dustObj.SetActiveChildren(true);
		this.grassObj.SetActive(false);
		this.grassObj.SetActiveChildren(true);
		this.boneObj.SetActive(false);
		this.boneObj.SetActiveChildren(true);
		this.spaObj.SetActive(false);
		this.spaObj.SetActiveChildren(true);
		this.metalObj.SetActive(false);
		this.metalObj.SetActiveChildren(true);
		this.wetObj.SetActive(false);
		this.wetObj.SetActiveChildren(true);
		this.peakPuffObj.SetActive(false);
		this.peakPuffObj.SetActiveChildren(true);
		GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
		this.impactEffect.SetActive(true);
		switch (GameManager.instance.playerData.environmentType)
		{
		case EnvironmentTypes.Dust:
		case EnvironmentTypes.Wood:
			this.dustObj.SetActive(true);
			break;
		case EnvironmentTypes.Grass:
			this.grassObj.SetActive(true);
			break;
		case EnvironmentTypes.Bone:
			this.boneObj.SetActive(true);
			break;
		case EnvironmentTypes.ShallowWater:
			this.spaObj.SetActive(true);
			break;
		case EnvironmentTypes.Metal:
		case EnvironmentTypes.ThinMetal:
			this.metalObj.SetActive(true);
			break;
		case EnvironmentTypes.Moss:
		case EnvironmentTypes.WetMetal:
		case EnvironmentTypes.WetWood:
		case EnvironmentTypes.RunningWater:
			this.wetObj.SetActive(true);
			break;
		case EnvironmentTypes.PeakPuff:
			this.peakPuffObj.SetActive(true);
			break;
		}
		this.recycleTime = Time.timeAsDouble + 1.5;
	}

	// Token: 0x0600151D RID: 5405 RVA: 0x0005FA4E File Offset: 0x0005DC4E
	private void Update()
	{
		if (Time.timeAsDouble > this.recycleTime)
		{
			base.gameObject.Recycle();
		}
	}

	// Token: 0x040013A8 RID: 5032
	public GameObject dustObj;

	// Token: 0x040013A9 RID: 5033
	public GameObject grassObj;

	// Token: 0x040013AA RID: 5034
	public GameObject boneObj;

	// Token: 0x040013AB RID: 5035
	public GameObject spaObj;

	// Token: 0x040013AC RID: 5036
	public GameObject metalObj;

	// Token: 0x040013AD RID: 5037
	public GameObject peakPuffObj;

	// Token: 0x040013AE RID: 5038
	public GameObject wetObj;

	// Token: 0x040013AF RID: 5039
	public GameObject impactEffect;

	// Token: 0x040013B0 RID: 5040
	[Space]
	private double recycleTime;
}
