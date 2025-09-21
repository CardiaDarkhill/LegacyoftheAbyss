using System;
using System.Collections;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003A3 RID: 931
public class BossChallengeUI : MonoBehaviour
{
	// Token: 0x1400005C RID: 92
	// (add) Token: 0x06001F55 RID: 8021 RVA: 0x0008F264 File Offset: 0x0008D464
	// (remove) Token: 0x06001F56 RID: 8022 RVA: 0x0008F29C File Offset: 0x0008D49C
	public event BossChallengeUI.HideEvent OnCancel;

	// Token: 0x1400005D RID: 93
	// (add) Token: 0x06001F57 RID: 8023 RVA: 0x0008F2D4 File Offset: 0x0008D4D4
	// (remove) Token: 0x06001F58 RID: 8024 RVA: 0x0008F30C File Offset: 0x0008D50C
	public event BossChallengeUI.LevelSelectedEvent OnLevelSelected;

	// Token: 0x06001F59 RID: 8025 RVA: 0x0008F344 File Offset: 0x0008D544
	private void Awake()
	{
		this.canvas = base.GetComponent<Canvas>();
		this.animator = base.GetComponent<Animator>();
		this.group = base.GetComponent<CanvasGroup>();
		if (this.group)
		{
			this.group.alpha = 0f;
		}
	}

	// Token: 0x06001F5A RID: 8026 RVA: 0x0008F394 File Offset: 0x0008D594
	private void Start()
	{
		if (this.canvas && GameCameras.instance && GameCameras.instance.hudCamera)
		{
			this.canvas.worldCamera = GameCameras.instance.hudCamera;
		}
	}

	// Token: 0x06001F5B RID: 8027 RVA: 0x0008F3E0 File Offset: 0x0008D5E0
	public void Setup(BossStatue bossStatue, string bossNameSheet, string bossNameKey, string descriptionSheet, string descriptionKey)
	{
		this.bossStatue = bossStatue;
		if (this.bossNameText)
		{
			this.bossNameText.text = Language.Get(bossNameKey, bossNameSheet);
		}
		if (this.descriptionText)
		{
			this.descriptionText.text = Language.Get(descriptionKey, descriptionSheet);
		}
		if (!bossStatue.hasNoTiers)
		{
			BossStatue.Completion completion = bossStatue.UsingDreamVersion ? bossStatue.DreamStatueState : bossStatue.StatueState;
			this.tier1Button.SetState(completion.completedTier1);
			this.tier2Button.SetState(completion.completedTier2);
			this.tier3Button.SetState(completion.completedTier3);
			this.tier1Button.SetupNavigation(true, completion.completedTier2 ? this.tier3Button : this.tier2Button, this.tier2Button);
			this.tier2Button.SetupNavigation(true, this.tier1Button, completion.completedTier2 ? this.tier3Button : this.tier1Button);
			this.tier3Button.SetupNavigation(completion.completedTier2, this.tier2Button, this.tier1Button);
			if (this.tier3UnlockEffect && completion.completedTier2 && !completion.seenTier3Unlock)
			{
				base.StartCoroutine(this.ShowUnlockEffect());
			}
			base.StartCoroutine(this.SetFirstSelected());
			return;
		}
		this.LoadBoss(0, false);
	}

	// Token: 0x06001F5C RID: 8028 RVA: 0x0008F536 File Offset: 0x0008D736
	private IEnumerator ShowUnlockEffect()
	{
		BossStatue.Completion state = this.bossStatue.UsingDreamVersion ? this.bossStatue.DreamStatueState : this.bossStatue.StatueState;
		yield return new WaitForSeconds(this.tier3UnlockEffectDelay);
		this.tier3UnlockEffect.SetActive(true);
		state.seenTier3Unlock = true;
		if (this.bossStatue.UsingDreamVersion)
		{
			this.bossStatue.DreamStatueState = state;
		}
		else
		{
			this.bossStatue.StatueState = state;
		}
		yield break;
	}

	// Token: 0x06001F5D RID: 8029 RVA: 0x0008F545 File Offset: 0x0008D745
	private IEnumerator SetFirstSelected()
	{
		MenuSelectable select = this.firstSelected;
		if (BossChallengeUI.lastSelectedButton >= 0)
		{
			switch (BossChallengeUI.lastSelectedButton)
			{
			case 0:
				if (this.tier1Button.button && this.tier1Button.button.interactable)
				{
					select = this.tier1Button.button;
				}
				break;
			case 1:
				if (this.tier2Button.button && this.tier2Button.button.interactable)
				{
					select = this.tier2Button.button;
				}
				break;
			case 2:
				if (this.tier3Button.button && this.tier3Button.button.interactable)
				{
					select = this.tier3Button.button;
				}
				break;
			}
		}
		if (select)
		{
			select.ForceDeselect();
			yield return null;
			UIManager.HighlightSelectableNoSound(select);
			ManagerSingleton<InputHandler>.Instance.StartUIInput();
		}
		yield break;
	}

	// Token: 0x06001F5E RID: 8030 RVA: 0x0008F554 File Offset: 0x0008D754
	public void Hide()
	{
		this.Hide(true);
	}

	// Token: 0x06001F5F RID: 8031 RVA: 0x0008F55D File Offset: 0x0008D75D
	public void Hide(bool doAnim)
	{
		if (doAnim)
		{
			base.StartCoroutine(this.HideAnim());
			return;
		}
		if (this.OnCancel != null)
		{
			this.OnCancel();
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001F60 RID: 8032 RVA: 0x0008F58F File Offset: 0x0008D78F
	private IEnumerator HideAnim()
	{
		if (this.animator)
		{
			this.animator.Play(this.closeStateName);
			AnimatorClipInfo[] currentAnimatorClipInfo = this.animator.GetCurrentAnimatorClipInfo(0);
			yield return new WaitForSeconds(currentAnimatorClipInfo[0].clip.length);
		}
		if (this.tier3UnlockEffect)
		{
			this.tier3UnlockEffect.SetActive(false);
		}
		if (this.OnCancel != null)
		{
			this.OnCancel();
		}
		base.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06001F61 RID: 8033 RVA: 0x0008F59E File Offset: 0x0008D79E
	public void LoadBoss(int level)
	{
		this.LoadBoss(level, true);
	}

	// Token: 0x06001F62 RID: 8034 RVA: 0x0008F5A8 File Offset: 0x0008D7A8
	public void LoadBoss(int level, bool doHideAnim)
	{
		BossScene bossScene = this.bossStatue.UsingDreamVersion ? this.bossStatue.dreamBossScene : this.bossStatue.bossScene;
		string text = bossScene.sceneName;
		switch (level)
		{
		case 0:
			text = bossScene.Tier1Scene;
			break;
		case 1:
			text = bossScene.Tier2Scene;
			break;
		case 2:
			text = bossScene.Tier3Scene;
			break;
		}
		if (!Application.CanStreamedLevelBeLoaded(text))
		{
			this.Hide(doHideAnim);
			Debug.LogError(string.Format("Could not start boss scene. Scene: \"{0}\" does not exist!", text));
			return;
		}
		StaticVariableList.SetValue("bossSceneToLoad", text, 0);
		this.OnCancel = null;
		this.Hide(doHideAnim);
		GameManager.instance.playerData.bossReturnEntryGate = this.bossStatue.dreamReturnGate.name;
		Action <>9__1;
		BossSceneController.SetupEvent = delegate(BossSceneController self)
		{
			self.BossLevel = level;
			self.DreamReturnEvent = "DREAM RETURN";
			BossSceneController self2 = self;
			Action value;
			if ((value = <>9__1) == null)
			{
				value = (<>9__1 = delegate()
				{
					string fieldName = this.bossStatue.UsingDreamVersion ? this.bossStatue.dreamStatueStatePD : this.bossStatue.statueStatePD;
					BossStatue.Completion playerDataVariable = GameManager.instance.GetPlayerDataVariable<BossStatue.Completion>(fieldName);
					switch (level)
					{
					case 0:
						playerDataVariable.completedTier1 = true;
						break;
					case 1:
						playerDataVariable.completedTier2 = true;
						break;
					case 2:
						playerDataVariable.completedTier3 = true;
						break;
					}
					GameManager.instance.SetPlayerDataVariable<BossStatue.Completion>(fieldName, playerDataVariable);
					GameManager.instance.playerData.currentBossStatueCompletionKey = (this.bossStatue.UsingDreamVersion ? this.bossStatue.dreamStatueStatePD : this.bossStatue.statueStatePD);
					GameManager.instance.playerData.bossStatueTargetLevel = level;
				});
			}
			self2.OnBossesDead += value;
			self.OnBossSceneComplete += delegate()
			{
				self.DoDreamReturn();
			};
		};
		if (this.OnLevelSelected != null)
		{
			this.OnLevelSelected();
		}
	}

	// Token: 0x06001F63 RID: 8035 RVA: 0x0008F6A7 File Offset: 0x0008D8A7
	public void RecordLastSelected(int index)
	{
		BossChallengeUI.lastSelectedButton = index;
	}

	// Token: 0x04001E43 RID: 7747
	private BossStatue bossStatue;

	// Token: 0x04001E44 RID: 7748
	public Text bossNameText;

	// Token: 0x04001E45 RID: 7749
	public Text descriptionText;

	// Token: 0x04001E46 RID: 7750
	[Space]
	public MenuSelectable firstSelected;

	// Token: 0x04001E47 RID: 7751
	public string closeStateName = "GG_Challenge_Close";

	// Token: 0x04001E48 RID: 7752
	public BossChallengeUI.ButtonDisplay tier1Button;

	// Token: 0x04001E49 RID: 7753
	public BossChallengeUI.ButtonDisplay tier2Button;

	// Token: 0x04001E4A RID: 7754
	public BossChallengeUI.ButtonDisplay tier3Button;

	// Token: 0x04001E4B RID: 7755
	public GameObject tier3UnlockEffect;

	// Token: 0x04001E4C RID: 7756
	public float tier3UnlockEffectDelay = 0.5f;

	// Token: 0x04001E4D RID: 7757
	private static int lastSelectedButton = -1;

	// Token: 0x04001E4E RID: 7758
	private Canvas canvas;

	// Token: 0x04001E4F RID: 7759
	private Animator animator;

	// Token: 0x04001E50 RID: 7760
	private CanvasGroup group;

	// Token: 0x0200164A RID: 5706
	// (Invoke) Token: 0x06008992 RID: 35218
	public delegate void HideEvent();

	// Token: 0x0200164B RID: 5707
	// (Invoke) Token: 0x06008996 RID: 35222
	public delegate void LevelSelectedEvent();

	// Token: 0x0200164C RID: 5708
	[Serializable]
	public class ButtonDisplay
	{
		// Token: 0x06008999 RID: 35225 RVA: 0x0027D470 File Offset: 0x0027B670
		public void SetupNavigation(bool isActive, BossChallengeUI.ButtonDisplay selectOnUp, BossChallengeUI.ButtonDisplay selectOnDown)
		{
			this.button.interactable = isActive;
			Navigation navigation = this.button.navigation;
			navigation.selectOnUp = selectOnUp.button;
			navigation.selectOnDown = selectOnDown.button;
			this.button.navigation = navigation;
			CanvasGroup component = this.button.GetComponent<CanvasGroup>();
			if (component)
			{
				component.alpha = (isActive ? this.enabledAlpha : this.disabledAlpha);
			}
		}

		// Token: 0x0600899A RID: 35226 RVA: 0x0027D4E8 File Offset: 0x0027B6E8
		public void SetState(bool isComplete)
		{
			if (this.completeImage)
			{
				this.completeImage.gameObject.SetActive(isComplete);
			}
			if (this.incompleteImage)
			{
				this.incompleteImage.gameObject.SetActive(!isComplete);
			}
		}

		// Token: 0x04008A54 RID: 35412
		public Image completeImage;

		// Token: 0x04008A55 RID: 35413
		public Image incompleteImage;

		// Token: 0x04008A56 RID: 35414
		public MenuSelectable button;

		// Token: 0x04008A57 RID: 35415
		public float enabledAlpha = 1f;

		// Token: 0x04008A58 RID: 35416
		public float disabledAlpha = 0.5f;
	}
}
