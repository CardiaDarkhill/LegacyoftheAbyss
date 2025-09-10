using System;
using System.Collections;
using UnityEngine;

// Token: 0x020003A9 RID: 937
public class BossDoorLockUI : MonoBehaviour
{
	// Token: 0x06001F8F RID: 8079 RVA: 0x000903EF File Offset: 0x0008E5EF
	private void Awake()
	{
		this.group = base.GetComponent<CanvasGroup>();
		this.animator = base.GetComponent<Animator>();
		this.bossIcons = (this.iconParent ? this.iconParent.GetComponentsInChildren<BossDoorLockUIIcon>() : new BossDoorLockUIIcon[0]);
	}

	// Token: 0x06001F90 RID: 8080 RVA: 0x00090430 File Offset: 0x0008E630
	public void Show(BossSequenceDoor door)
	{
		this.group.alpha = 0f;
		base.gameObject.SetActive(true);
		if (door && door.bossSequence)
		{
			BossSequenceDoor.Completion currentCompletion = door.CurrentCompletion;
			foreach (BossDoorLockUIIcon bossDoorLockUIIcon in this.bossIcons)
			{
				bossDoorLockUIIcon.bossIcon.enabled = false;
				bossDoorLockUIIcon.SetUnlocked(false, false, 0);
			}
			int count = door.bossSequence.Count;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (int j = 0; j < count; j++)
			{
				if (door.bossSequence.IsSceneHidden(j))
				{
					num--;
				}
				else
				{
					BossScene bossScene = door.bossSequence.GetBossScene(j);
					string sceneObjectName = door.bossSequence.GetSceneObjectName(j);
					int num4 = j + num;
					num3 = num4;
					if (num4 < this.bossIcons.Length && this.bossIcons[num4])
					{
						this.bossIcons[num4].gameObject.SetActive(true);
						if (bossScene.DisplayIcon)
						{
							this.bossIcons[num4].bossIcon.enabled = true;
							this.bossIcons[num4].bossIcon.sprite = bossScene.DisplayIcon;
							this.bossIcons[num4].bossIcon.SetNativeSize();
						}
						if (bossScene.IsUnlocked(BossSceneCheckSource.Sequence))
						{
							if (currentCompletion.viewedBossSceneCompletions.Contains(sceneObjectName))
							{
								this.bossIcons[num4].SetUnlocked(true, false, 0);
							}
							else
							{
								this.bossIcons[num4].SetUnlocked(true, true, num2);
								num2++;
								currentCompletion.viewedBossSceneCompletions.Add(sceneObjectName);
							}
						}
					}
				}
			}
			for (int k = num3 + 1; k < this.bossIcons.Length; k++)
			{
				this.bossIcons[k].gameObject.SetActive(false);
			}
			door.CurrentCompletion = currentCompletion;
		}
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
		}
		this.fadeRoutine = base.StartCoroutine(this.ShowRoutine());
		GameCameras.instance.HUDOut();
	}

	// Token: 0x06001F91 RID: 8081 RVA: 0x00090650 File Offset: 0x0008E850
	public void Hide()
	{
		BossDoorLockUIIcon[] array = this.bossIcons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].StopAllCoroutines();
		}
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
		}
		this.fadeRoutine = base.StartCoroutine(this.HideRoutine());
	}

	// Token: 0x06001F92 RID: 8082 RVA: 0x000906A0 File Offset: 0x0008E8A0
	private IEnumerator ShowRoutine()
	{
		if (this.buttonPrompts)
		{
			this.buttonPrompts.alpha = 0f;
		}
		if (this.animator)
		{
			this.animator.Play("Open");
			yield return null;
			yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
		}
		this.group.alpha = 1f;
		if (this.fadeButtonRoutine != null)
		{
			base.StopCoroutine(this.fadeButtonRoutine);
		}
		this.fadeButtonRoutine = base.StartCoroutine(this.FadeButtonPrompts(1f, this.buttonPromptFadeTime));
		yield return this.fadeButtonRoutine;
		this.fadeRoutine = null;
		yield break;
	}

	// Token: 0x06001F93 RID: 8083 RVA: 0x000906AF File Offset: 0x0008E8AF
	private IEnumerator HideRoutine()
	{
		if (this.animator)
		{
			this.animator.Play("Close");
			yield return null;
			float length = this.animator.GetCurrentAnimatorStateInfo(0).length;
			if (this.fadeButtonRoutine != null)
			{
				base.StopCoroutine(this.fadeButtonRoutine);
			}
			this.fadeButtonRoutine = base.StartCoroutine(this.FadeButtonPrompts(0f, length));
			yield return new WaitForSeconds(length);
		}
		GameCameras.instance.HUDIn();
		base.gameObject.SetActive(false);
		this.fadeRoutine = null;
		yield break;
	}

	// Token: 0x06001F94 RID: 8084 RVA: 0x000906BE File Offset: 0x0008E8BE
	private IEnumerator FadeButtonPrompts(float toAlpha, float time)
	{
		if (this.buttonPrompts)
		{
			float startAlpha = this.buttonPrompts.alpha;
			for (float elapsed = 0f; elapsed < time; elapsed += Time.deltaTime)
			{
				this.buttonPrompts.alpha = Mathf.Lerp(startAlpha, toAlpha, elapsed / time);
				yield return null;
			}
			this.buttonPrompts.alpha = toAlpha;
		}
		this.fadeButtonRoutine = null;
		yield break;
	}

	// Token: 0x04001E9F RID: 7839
	public GameObject iconParent;

	// Token: 0x04001EA0 RID: 7840
	private BossDoorLockUIIcon[] bossIcons;

	// Token: 0x04001EA1 RID: 7841
	public CanvasGroup buttonPrompts;

	// Token: 0x04001EA2 RID: 7842
	public float buttonPromptFadeTime = 2f;

	// Token: 0x04001EA3 RID: 7843
	private Coroutine fadeRoutine;

	// Token: 0x04001EA4 RID: 7844
	private Coroutine fadeButtonRoutine;

	// Token: 0x04001EA5 RID: 7845
	private CanvasGroup group;

	// Token: 0x04001EA6 RID: 7846
	private Animator animator;
}
