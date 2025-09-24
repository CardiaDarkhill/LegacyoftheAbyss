using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003A5 RID: 933
public class BossDoorChallengeCompleteUI : MonoBehaviour
{
	// Token: 0x06001F69 RID: 8041 RVA: 0x0008F887 File Offset: 0x0008DA87
	private void Start()
	{
		base.StartCoroutine(this.Sequence());
		base.StartCoroutine(this.ShowAchievements());
	}

	// Token: 0x06001F6A RID: 8042 RVA: 0x0008F8A3 File Offset: 0x0008DAA3
	private void Update()
	{
		if (this.waitingForInput && (ManagerSingleton<InputHandler>.Instance.gameController.AnyButtonWasPressed || Input.anyKeyDown))
		{
			this.waitingForInput = false;
		}
	}

	// Token: 0x06001F6B RID: 8043 RVA: 0x0008F8CC File Offset: 0x0008DACC
	private IEnumerator ShowAchievements()
	{
		yield return new WaitForSeconds(this.achievementShowDelay);
		GameManager.instance.AwardQueuedAchievements();
		yield break;
	}

	// Token: 0x06001F6C RID: 8044 RVA: 0x0008F8DB File Offset: 0x0008DADB
	private IEnumerator Sequence()
	{
		GameObject[] array = this.coreFlashEffects;
		int j;
		for (j = 0; j < array.Length; j++)
		{
			array[j].SetActive(false);
		}
		BossSequenceDoor.Completion completion = BossSequenceController.IsInSequence ? BossSequenceController.PreviousCompletion : BossSequenceDoor.Completion.None;
		bool boundNail = !BossSequenceController.IsInSequence || BossSequenceController.BoundNail;
		bool boundShell = !BossSequenceController.IsInSequence || BossSequenceController.BoundShell;
		bool boundCharms = !BossSequenceController.IsInSequence || BossSequenceController.BoundCharms;
		bool boundSoul = !BossSequenceController.IsInSequence || BossSequenceController.BoundSoul;
		bool knightDamaged = !BossSequenceController.IsInSequence || BossSequenceController.KnightDamaged;
		if (this.completeCore)
		{
			this.completeCore.SetActive(false);
		}
		if (this.allBindingsCore)
		{
			this.allBindingsCore.SetActive(completion.allBindings);
		}
		if (this.noHitsCore)
		{
			this.noHitsCore.SetActive(completion.noHits && !completion.allBindings);
		}
		if (this.allBindingsNoHitsCore)
		{
			this.allBindingsNoHitsCore.SetActive(completion.noHits && completion.allBindings);
		}
		if (this.timerGroup)
		{
			this.timerGroup.alpha = 0f;
		}
		for (int k = 0; k < 4; k++)
		{
			BossDoorChallengeCompleteUI.BindingIcon bindingIcon = null;
			bool value = false;
			switch (k)
			{
			case 0:
				bindingIcon = this.bindingCapNail;
				value = completion.boundNail;
				break;
			case 1:
				bindingIcon = this.bindingCapShell;
				value = completion.boundShell;
				break;
			case 2:
				bindingIcon = this.bindingCapCharm;
				value = completion.boundCharms;
				break;
			case 3:
				bindingIcon = this.bindingCapSoul;
				value = completion.boundSoul;
				break;
			}
			if (bindingIcon != null)
			{
				bindingIcon.SetAlreadyVisible(value, completion.allBindings);
			}
		}
		yield return new WaitForSeconds(this.musicDelay);
		if (this.musicSource)
		{
			this.musicSource.Play();
		}
		yield return new WaitForSeconds(this.appearAnimDelay - this.musicDelay);
		if (this.animator)
		{
			this.animator.Play("Appear");
			yield return null;
			yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length + this.appearEndWaitTime);
		}
		for (int i = 0; i < 4; i = j + 1)
		{
			BossDoorChallengeCompleteUI.BindingIcon bindingIcon2 = this.GetBindingIcon(i);
			if (bindingIcon2 != null)
			{
				base.StartCoroutine(bindingIcon2.DoAppearAnim(this.bindingCapAppearDelay));
				float num = (float)i * this.bindingAppearPitchIncrease;
				new AudioEvent
				{
					Clip = this.bindingAppearSound.Clip,
					PitchMin = this.bindingAppearSound.PitchMin + num,
					PitchMax = this.bindingAppearSound.PitchMax + num,
					Volume = this.bindingAppearSound.Volume
				}.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
				yield return new WaitForSeconds(this.bindingCapAnimDelay);
			}
			j = i;
		}
		bool allBindings = boundNail && boundShell && boundCharms && boundSoul;
		if (allBindings)
		{
			for (int l = 0; l < 4; l++)
			{
				BossDoorChallengeCompleteUI.BindingIcon bindingIcon3 = this.GetBindingIcon(l);
				if (bindingIcon3 != null)
				{
					base.StartCoroutine(bindingIcon3.DoAllAppearAnim(this.bindingCapAppearDelay));
				}
			}
			this.bindingAllAppearSound.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
		}
		yield return new WaitForSeconds(this.completionCapAppearDelay);
		array = this.coreFlashEffects;
		for (j = 0; j < array.Length; j++)
		{
			array[j].SetActive(true);
		}
		this.coreAppearSound.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
		yield return new WaitForSeconds(this.bindingCapAppearDelay);
		if (this.completeCore)
		{
			this.completeCore.SetActive(!allBindings);
		}
		if (this.allBindingsCore && allBindings)
		{
			this.allBindingsCore.SetActive(true);
		}
		if (this.noHitsCore && !knightDamaged && !allBindings)
		{
			this.noHitsCore.SetActive(true);
		}
		if (this.allBindingsNoHitsCore && (!knightDamaged && allBindings))
		{
			this.allBindingsNoHitsCore.SetActive(true);
		}
		if (this.timerText)
		{
			float timer = BossSequenceController.Timer;
			this.timerText.text = string.Format("{0:00}:{1:00}", timer / 60f, timer % 60f);
			if (this.timerGroup)
			{
				yield return new WaitForSeconds(this.timerFadeDelay);
				for (float elapsed = 0f; elapsed <= this.timerFadeTime; elapsed += Time.deltaTime)
				{
					this.timerGroup.alpha = elapsed / this.timerFadeTime;
					yield return null;
				}
			}
		}
		yield return new WaitForSeconds(this.endAnimDelay);
		this.waitingForInput = true;
		while (this.waitingForInput)
		{
			yield return null;
		}
		if (this.animator)
		{
			this.animator.Play("Disappear");
			yield return null;
			yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
		}
		HeroController.instance.EnterWithoutInput(true);
		StaticVariableList.SetValue("finishedBossReturning", true, 0);
		GameCameras.instance.cameraFadeFSM.SendEventSafe("FADE OUT INSTANT");
		yield return null;
		BossSequenceController.RestoreBindings();
		GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
		{
			SceneName = (BossSequenceController.ShouldUnlockGGMode ? "GG_Unlock" : GameManager.instance.playerData.dreamReturnScene),
			EntryGateName = GameManager.instance.playerData.bossReturnEntryGate,
			EntryDelay = 0f,
			PreventCameraFadeOut = true,
			WaitForSceneTransitionCameraFade = false
		});
		yield break;
	}

	// Token: 0x06001F6D RID: 8045 RVA: 0x0008F8EC File Offset: 0x0008DAEC
	private BossDoorChallengeCompleteUI.BindingIcon GetBindingIcon(int index)
	{
		BossDoorChallengeCompleteUI.BindingIcon result = null;
		switch (index)
		{
		case 0:
			if (BossSequenceController.BoundNail || !BossSequenceController.IsInSequence)
			{
				result = this.bindingCapNail;
			}
			break;
		case 1:
			if (BossSequenceController.BoundShell || !BossSequenceController.IsInSequence)
			{
				result = this.bindingCapShell;
			}
			break;
		case 2:
			if (BossSequenceController.BoundCharms || !BossSequenceController.IsInSequence)
			{
				result = this.bindingCapCharm;
			}
			break;
		case 3:
			if (BossSequenceController.BoundSoul || !BossSequenceController.IsInSequence)
			{
				result = this.bindingCapSoul;
			}
			break;
		}
		return result;
	}

	// Token: 0x04001E58 RID: 7768
	public float achievementShowDelay = 0.5f;

	// Token: 0x04001E59 RID: 7769
	public Animator animator;

	// Token: 0x04001E5A RID: 7770
	public float appearAnimDelay = 2f;

	// Token: 0x04001E5B RID: 7771
	public float appearEndWaitTime = 1f;

	// Token: 0x04001E5C RID: 7772
	public float bindingCapAnimDelay = 0.5f;

	// Token: 0x04001E5D RID: 7773
	public float bindingCapAppearDelay = 0.2f;

	// Token: 0x04001E5E RID: 7774
	public float completionCapAppearDelay = 0.75f;

	// Token: 0x04001E5F RID: 7775
	public float endAnimDelay = 2f;

	// Token: 0x04001E60 RID: 7776
	public AudioSource musicSource;

	// Token: 0x04001E61 RID: 7777
	public float musicDelay = 1f;

	// Token: 0x04001E62 RID: 7778
	[Space]
	public BossDoorChallengeCompleteUI.BindingIcon bindingCapNail;

	// Token: 0x04001E63 RID: 7779
	public BossDoorChallengeCompleteUI.BindingIcon bindingCapShell;

	// Token: 0x04001E64 RID: 7780
	public BossDoorChallengeCompleteUI.BindingIcon bindingCapCharm;

	// Token: 0x04001E65 RID: 7781
	public BossDoorChallengeCompleteUI.BindingIcon bindingCapSoul;

	// Token: 0x04001E66 RID: 7782
	public AudioSource audioSourcePrefab;

	// Token: 0x04001E67 RID: 7783
	public AudioEvent screenAppearSound;

	// Token: 0x04001E68 RID: 7784
	public AudioEvent bindingAppearSound;

	// Token: 0x04001E69 RID: 7785
	public float bindingAppearPitchIncrease = 0.05f;

	// Token: 0x04001E6A RID: 7786
	public AudioEvent bindingAllAppearSound;

	// Token: 0x04001E6B RID: 7787
	public AudioEvent coreAppearSound;

	// Token: 0x04001E6C RID: 7788
	[Space]
	public GameObject[] coreFlashEffects;

	// Token: 0x04001E6D RID: 7789
	public GameObject completeCore;

	// Token: 0x04001E6E RID: 7790
	public GameObject allBindingsCore;

	// Token: 0x04001E6F RID: 7791
	public GameObject noHitsCore;

	// Token: 0x04001E70 RID: 7792
	public GameObject allBindingsNoHitsCore;

	// Token: 0x04001E71 RID: 7793
	[Space]
	public CanvasGroup timerGroup;

	// Token: 0x04001E72 RID: 7794
	public float timerFadeDelay = 1f;

	// Token: 0x04001E73 RID: 7795
	public float timerFadeTime = 2f;

	// Token: 0x04001E74 RID: 7796
	public Text timerText;

	// Token: 0x04001E75 RID: 7797
	private bool waitingForInput;

	// Token: 0x02001655 RID: 5717
	[Serializable]
	public class BindingIcon
	{
		// Token: 0x060089B8 RID: 35256 RVA: 0x0027DA9C File Offset: 0x0027BC9C
		public void SetAlreadyVisible(bool value, bool allUnlocked)
		{
			GameObject[] array = this.flashEffects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			if (this.icon)
			{
				this.icon.enabled = value;
			}
			this.alreadyVisible = value;
			if (allUnlocked)
			{
				this.SetAllUnlocked();
			}
		}

		// Token: 0x060089B9 RID: 35257 RVA: 0x0027DAF0 File Offset: 0x0027BCF0
		public IEnumerator DoAppearAnim(float appearDelay)
		{
			if (!this.alreadyVisible && this.icon)
			{
				this.icon.enabled = false;
			}
			GameObject[] array = this.flashEffects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
			yield return new WaitForSeconds(appearDelay);
			if (this.icon)
			{
				this.icon.enabled = true;
			}
			yield break;
		}

		// Token: 0x060089BA RID: 35258 RVA: 0x0027DB06 File Offset: 0x0027BD06
		public IEnumerator DoAllAppearAnim(float appearDelay)
		{
			foreach (GameObject gameObject in this.flashEffects)
			{
				gameObject.SetActive(false);
				gameObject.SetActive(true);
			}
			yield return new WaitForSeconds(appearDelay);
			this.SetAllUnlocked();
			yield break;
		}

		// Token: 0x060089BB RID: 35259 RVA: 0x0027DB1C File Offset: 0x0027BD1C
		private void SetAllUnlocked()
		{
			if (this.icon && this.allUnlockedSprite)
			{
				this.icon.sprite = this.allUnlockedSprite;
			}
		}

		// Token: 0x04008A6E RID: 35438
		public Image icon;

		// Token: 0x04008A6F RID: 35439
		public Sprite allUnlockedSprite;

		// Token: 0x04008A70 RID: 35440
		public GameObject[] flashEffects;

		// Token: 0x04008A71 RID: 35441
		private bool alreadyVisible;
	}
}
