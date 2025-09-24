using System;
using System.Collections;
using UnityEngine;

// Token: 0x020003A0 RID: 928
public class BossStatueTrophyPlaque : MonoBehaviour
{
	// Token: 0x06001F48 RID: 8008 RVA: 0x0008F046 File Offset: 0x0008D246
	public void SetDisplay(BossStatueTrophyPlaque.DisplayType type)
	{
		this.SetDisplayObject(type);
	}

	// Token: 0x06001F49 RID: 8009 RVA: 0x0008F050 File Offset: 0x0008D250
	public void DoTierCompleteEffect(BossStatueTrophyPlaque.DisplayType type)
	{
		if (type >= BossStatueTrophyPlaque.DisplayType.Tier1)
		{
			GameObject gameObject = this.tierCompleteEffectPrefabs[(int)type];
			if (gameObject)
			{
				this.spawnedCompleteEffect = Object.Instantiate<GameObject>(gameObject, this.tierCompleteEffectPoint.position, gameObject.transform.rotation);
				this.spawnedCompleteEffect.SetActive(false);
				base.StartCoroutine(this.TierCompleteEffectDelayed());
			}
		}
	}

	// Token: 0x06001F4A RID: 8010 RVA: 0x0008F0AD File Offset: 0x0008D2AD
	private IEnumerator TierCompleteEffectDelayed()
	{
		yield return new WaitForSeconds(this.tierCompleteEffectDelay);
		this.spawnedCompleteEffect.SetActive(true);
		yield break;
	}

	// Token: 0x06001F4B RID: 8011 RVA: 0x0008F0BC File Offset: 0x0008D2BC
	private void SetDisplayObject(BossStatueTrophyPlaque.DisplayType type)
	{
		GameObject[] array = this.displayObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		if (type >= BossStatueTrophyPlaque.DisplayType.Tier1)
		{
			this.displayObjects[(int)type].SetActive(true);
		}
	}

	// Token: 0x06001F4C RID: 8012 RVA: 0x0008F0FC File Offset: 0x0008D2FC
	public static BossStatueTrophyPlaque.DisplayType GetDisplayType(BossStatue.Completion completion)
	{
		BossStatueTrophyPlaque.DisplayType result = BossStatueTrophyPlaque.DisplayType.None;
		if (completion.completedTier3)
		{
			result = BossStatueTrophyPlaque.DisplayType.Tier3;
		}
		else if (completion.completedTier2)
		{
			result = BossStatueTrophyPlaque.DisplayType.Tier2;
		}
		else if (completion.completedTier1)
		{
			result = BossStatueTrophyPlaque.DisplayType.Tier1;
		}
		return result;
	}

	// Token: 0x04001E37 RID: 7735
	[ArrayForEnum(typeof(BossStatueTrophyPlaque.DisplayType))]
	public GameObject[] displayObjects;

	// Token: 0x04001E38 RID: 7736
	[Space]
	public Transform tierCompleteEffectPoint;

	// Token: 0x04001E39 RID: 7737
	public float tierCompleteEffectDelay = 1f;

	// Token: 0x04001E3A RID: 7738
	[ArrayForEnum(typeof(BossStatueTrophyPlaque.DisplayType))]
	public GameObject[] tierCompleteEffectPrefabs;

	// Token: 0x04001E3B RID: 7739
	private GameObject spawnedCompleteEffect;

	// Token: 0x02001648 RID: 5704
	public enum DisplayType
	{
		// Token: 0x04008A4D RID: 35405
		None = -1,
		// Token: 0x04008A4E RID: 35406
		Tier1,
		// Token: 0x04008A4F RID: 35407
		Tier2,
		// Token: 0x04008A50 RID: 35408
		Tier3
	}
}
