using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003AB RID: 939
public class BossSummaryUI : MonoBehaviour
{
	// Token: 0x06001F9A RID: 8090 RVA: 0x0009078C File Offset: 0x0008E98C
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
	}

	// Token: 0x06001F9B RID: 8091 RVA: 0x0009079C File Offset: 0x0008E99C
	private void Start()
	{
		CanvasGroup component = base.GetComponent<CanvasGroup>();
		if (component)
		{
			component.alpha = 0f;
		}
	}

	// Token: 0x06001F9C RID: 8092 RVA: 0x000907C4 File Offset: 0x0008E9C4
	public void SetupUI(List<BossStatue> bossStatues)
	{
		this.listItemTemplate.SetActive(true);
		foreach (BossStatue bossStatue in bossStatues)
		{
			if (bossStatue.gameObject.activeInHierarchy)
			{
				this.CreateListItem(bossStatue, false);
				if (bossStatue && bossStatue.dreamBossScene)
				{
					this.CreateListItem(bossStatue, true);
				}
			}
		}
		this.listItemTemplate.SetActive(false);
	}

	// Token: 0x06001F9D RID: 8093 RVA: 0x00090858 File Offset: 0x0008EA58
	private void CreateListItem(BossStatue bossStatue, bool isAlt = false)
	{
		BossStatue.Completion completion = (bossStatue != null) ? (isAlt ? bossStatue.DreamStatueState : bossStatue.StatueState) : BossStatue.Completion.None;
		if (this.listItemTemplate)
		{
			if (!bossStatue.isHidden || completion.completedTier1 || completion.completedTier2 || completion.completedTier3)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.listItemTemplate, this.listItemTemplate.transform.parent);
				gameObject.name = string.Format("{0} ({1})", this.listItemTemplate.name, bossStatue ? bossStatue.gameObject.name : "null");
				int num = 0;
				if (completion.completedTier3)
				{
					num = 4;
				}
				else if (completion.completedTier2)
				{
					num = 3;
				}
				else if (completion.completedTier1)
				{
					num = 2;
				}
				else if (completion.isUnlocked)
				{
					num = 1;
				}
				Image componentInChildren = gameObject.GetComponentInChildren<Image>();
				if (componentInChildren)
				{
					if (bossStatue.hasNoTiers)
					{
						num = Mathf.Clamp(num, 0, 3);
						componentInChildren.sprite = this.noTierStateSprites[num];
						componentInChildren.SetNativeSize();
					}
					else if (num < this.stateSprites.Length && num >= 0)
					{
						componentInChildren.sprite = this.stateSprites[num];
						componentInChildren.SetNativeSize();
					}
				}
				Text componentInChildren2 = gameObject.GetComponentInChildren<Text>();
				if (componentInChildren2)
				{
					if (num > 0 && bossStatue)
					{
						componentInChildren2.text = Language.Get(isAlt ? bossStatue.dreamBossDetails.nameKey : bossStatue.bossDetails.nameKey, isAlt ? bossStatue.dreamBossDetails.nameSheet : bossStatue.bossDetails.nameSheet).GetProcessed(LocalisationHelper.FontSource.Trajan).ToUpper();
						return;
					}
					componentInChildren2.text = this.defaultName;
					return;
				}
			}
		}
		else
		{
			Debug.LogError("No List Item template assigned!", this);
		}
	}

	// Token: 0x06001F9E RID: 8094 RVA: 0x00090A12 File Offset: 0x0008EC12
	public void Show()
	{
		base.gameObject.SetActive(true);
		GameCameras.instance.HUDOut();
	}

	// Token: 0x06001F9F RID: 8095 RVA: 0x00090A2A File Offset: 0x0008EC2A
	public void Hide()
	{
		base.StartCoroutine(this.Close());
	}

	// Token: 0x06001FA0 RID: 8096 RVA: 0x00090A39 File Offset: 0x0008EC39
	private IEnumerator Close()
	{
		if (this.animator)
		{
			this.animator.Play("Close");
			yield return null;
			yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
		}
		GameCameras.instance.HUDIn();
		base.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x04001EAD RID: 7853
	public GameObject listItemTemplate;

	// Token: 0x04001EAE RID: 7854
	public Sprite[] stateSprites;

	// Token: 0x04001EAF RID: 7855
	public Sprite[] noTierStateSprites;

	// Token: 0x04001EB0 RID: 7856
	public string defaultName = ".....";

	// Token: 0x04001EB1 RID: 7857
	private Animator animator;
}
