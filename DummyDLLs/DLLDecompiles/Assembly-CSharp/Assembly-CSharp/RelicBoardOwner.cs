using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x020001F0 RID: 496
public class RelicBoardOwner : MonoBehaviour
{
	// Token: 0x17000225 RID: 549
	// (get) Token: 0x0600131B RID: 4891 RVA: 0x00057DEF File Offset: 0x00055FEF
	public CollectableRelicBoard RelicBoard
	{
		get
		{
			return this.relicBoard;
		}
	}

	// Token: 0x17000226 RID: 550
	// (get) Token: 0x0600131C RID: 4892 RVA: 0x00057DF7 File Offset: 0x00055FF7
	public bool HasGramaphone
	{
		get
		{
			return this.gramaphone && this.gramaphone.gameObject.activeInHierarchy;
		}
	}

	// Token: 0x0600131D RID: 4893 RVA: 0x00057E18 File Offset: 0x00056018
	private void Awake()
	{
		this.didStartLoad = true;
		foreach (CollectableRelic collectableRelic in this.GetRelics())
		{
			collectableRelic.LoadClips();
		}
	}

	// Token: 0x0600131E RID: 4894 RVA: 0x00057E6C File Offset: 0x0005606C
	private void OnDestroy()
	{
		if (!this.didStartLoad)
		{
			return;
		}
		this.didStartLoad = false;
		foreach (CollectableRelic collectableRelic in this.GetRelics())
		{
			collectableRelic.FreeClips();
		}
	}

	// Token: 0x0600131F RID: 4895 RVA: 0x00057EC8 File Offset: 0x000560C8
	public void PlayOnGramaphone(CollectableRelic playingRelic)
	{
		this.StopPlayingRelic();
		this.gramaphone.Play(playingRelic, false, this);
	}

	// Token: 0x06001320 RID: 4896 RVA: 0x00057EDE File Offset: 0x000560DE
	public void StopPlayingRelic()
	{
		this.gramaphone.Stop();
	}

	// Token: 0x06001321 RID: 4897 RVA: 0x00057EEC File Offset: 0x000560EC
	[UsedImplicitly]
	public bool IsAllRelicsDeposited()
	{
		foreach (CollectableItemRelicType collectableItemRelicType in this.list)
		{
			using (IEnumerator<CollectableRelic> enumerator2 = collectableItemRelicType.Relics.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (!enumerator2.Current.SavedData.IsDeposited)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	// Token: 0x06001322 RID: 4898 RVA: 0x00057F78 File Offset: 0x00056178
	[UsedImplicitly]
	public bool IsAnyRelicsToDeposit()
	{
		foreach (CollectableItemRelicType collectableItemRelicType in this.list)
		{
			foreach (CollectableRelic collectableRelic in collectableItemRelicType.Relics)
			{
				CollectableRelicsData.Data savedData = collectableRelic.SavedData;
				if (savedData.IsCollected && !savedData.IsDeposited)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001323 RID: 4899 RVA: 0x00058010 File Offset: 0x00056210
	[UsedImplicitly]
	public void DepositCollectedRelics()
	{
		PlayerData instance = PlayerData.instance;
		int num = 0;
		int num2 = 0;
		foreach (CollectableItemRelicType collectableItemRelicType in this.list)
		{
			foreach (CollectableRelic collectableRelic in collectableItemRelicType.Relics)
			{
				num2++;
				CollectableRelicsData.Data savedData = collectableRelic.SavedData;
				if (savedData.IsDeposited)
				{
					num++;
				}
				else if (savedData.IsCollected)
				{
					savedData.IsDeposited = true;
					collectableRelic.SavedData = savedData;
					num++;
				}
			}
		}
		if (num == num2 && !string.IsNullOrEmpty(this.completedBool))
		{
			instance.SetBool(this.completedBool, true);
		}
	}

	// Token: 0x06001324 RID: 4900 RVA: 0x000580F4 File Offset: 0x000562F4
	public IEnumerable<CollectableRelic> GetRelics()
	{
		return this.list.SelectMany((CollectableItemRelicType type) => type.Relics);
	}

	// Token: 0x06001325 RID: 4901 RVA: 0x00058120 File Offset: 0x00056320
	public CollectableRelic GetPlayingRelic()
	{
		if (!this.gramaphone)
		{
			return null;
		}
		return this.gramaphone.PlayingRelic;
	}

	// Token: 0x06001326 RID: 4902 RVA: 0x0005813C File Offset: 0x0005633C
	public IEnumerable<CollectableRelic> GetRelicsToDeposit()
	{
		foreach (CollectableItemRelicType collectableItemRelicType in this.list)
		{
			foreach (CollectableRelic collectableRelic in collectableItemRelicType.Relics)
			{
				CollectableRelicsData.Data savedData = collectableRelic.SavedData;
				if (savedData.IsCollected && !savedData.IsDeposited)
				{
					yield return collectableRelic;
				}
			}
			IEnumerator<CollectableRelic> enumerator2 = null;
		}
		IEnumerator<CollectableItemRelicType> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x040011A9 RID: 4521
	[SerializeField]
	private CollectableRelicBoard relicBoard;

	// Token: 0x040011AA RID: 4522
	[Space]
	[SerializeField]
	private CollectableRelicTypeList list;

	// Token: 0x040011AB RID: 4523
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string completedBool;

	// Token: 0x040011AC RID: 4524
	[Space]
	[SerializeField]
	private CollectionGramaphone gramaphone;

	// Token: 0x040011AD RID: 4525
	private bool didStartLoad;
}
