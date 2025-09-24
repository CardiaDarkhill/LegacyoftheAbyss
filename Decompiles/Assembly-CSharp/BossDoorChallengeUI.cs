using System;
using System.Collections;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020003A6 RID: 934
public class BossDoorChallengeUI : MonoBehaviour
{
	// Token: 0x1400005E RID: 94
	// (add) Token: 0x06001F6F RID: 8047 RVA: 0x0008F9FC File Offset: 0x0008DBFC
	// (remove) Token: 0x06001F70 RID: 8048 RVA: 0x0008FA34 File Offset: 0x0008DC34
	public event BossDoorChallengeUI.HideEvent OnHidden;

	// Token: 0x1400005F RID: 95
	// (add) Token: 0x06001F71 RID: 8049 RVA: 0x0008FA6C File Offset: 0x0008DC6C
	// (remove) Token: 0x06001F72 RID: 8050 RVA: 0x0008FAA4 File Offset: 0x0008DCA4
	public event BossDoorChallengeUI.BeginEvent OnBegin;

	// Token: 0x06001F73 RID: 8051 RVA: 0x0008FAD9 File Offset: 0x0008DCD9
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.canvas = base.GetComponent<Canvas>();
		this.group = base.GetComponent<CanvasGroup>();
	}

	// Token: 0x06001F74 RID: 8052 RVA: 0x0008FB00 File Offset: 0x0008DD00
	private void Start()
	{
		this.canvas.worldCamera = GameCameras.instance.hudCamera;
		this.buttons = new BossDoorChallengeUIBindingButton[4];
		this.buttons[0] = this.boundNailButton;
		this.buttons[1] = this.boundHeartButton;
		this.buttons[2] = this.boundCharmsButton;
		this.buttons[3] = this.boundSoulButton;
		foreach (BossDoorChallengeUIBindingButton bossDoorChallengeUIBindingButton in this.buttons)
		{
			bossDoorChallengeUIBindingButton.OnButtonSelected += this.UpdateAllButtons;
			bossDoorChallengeUIBindingButton.OnButtonCancelled += this.Hide;
			bossDoorChallengeUIBindingButton.Reset();
		}
		this.group.alpha = 0f;
	}

	// Token: 0x06001F75 RID: 8053 RVA: 0x0008FBB8 File Offset: 0x0008DDB8
	private void OnEnable()
	{
		if (this.buttons != null)
		{
			BossDoorChallengeUIBindingButton[] array = this.buttons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Reset();
			}
			this.allPreviouslySelected = false;
		}
	}

	// Token: 0x06001F76 RID: 8054 RVA: 0x0008FBF4 File Offset: 0x0008DDF4
	public void Setup(BossSequenceDoor door)
	{
		this.door = door;
		if (this.titleTextSuper)
		{
			this.titleTextSuper.text = Language.Get(door.titleSuperKey, door.titleSuperSheet);
		}
		if (this.titleTextMain)
		{
			this.titleTextMain.text = Language.Get(door.titleMainKey, door.titleMainSheet);
		}
		if (this.descriptionText)
		{
			this.descriptionText.text = Language.Get(door.descriptionKey, door.descriptionSheet);
		}
	}

	// Token: 0x06001F77 RID: 8055 RVA: 0x0008FC84 File Offset: 0x0008DE84
	private void UpdateAllButtons()
	{
		bool flag = true;
		BossDoorChallengeUIBindingButton[] array = this.buttons;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].Selected)
			{
				flag = false;
				break;
			}
		}
		if (flag || this.allPreviouslySelected)
		{
			array = this.buttons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetAllSelected(flag);
			}
		}
		if (flag)
		{
			GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
			this.allSelectedSound.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
			if (this.allSelectedEffect)
			{
				this.allSelectedEffect.SetActive(false);
				this.allSelectedEffect.SetActive(true);
				this.allSelectedEffect.SetActiveChildren(true);
			}
		}
		this.allPreviouslySelected = flag;
	}

	// Token: 0x06001F78 RID: 8056 RVA: 0x0008FD4C File Offset: 0x0008DF4C
	public void Show()
	{
		base.gameObject.SetActive(true);
		base.StartCoroutine(this.ShowSequence());
		GameCameras.instance.HUDOut();
	}

	// Token: 0x06001F79 RID: 8057 RVA: 0x0008FD71 File Offset: 0x0008DF71
	private IEnumerator ShowSequence()
	{
		this.group.interactable = false;
		EventSystem.current.SetSelectedGameObject(null);
		yield return null;
		if (this.animator)
		{
			this.animator.Play("Open");
			yield return null;
			yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
		}
		this.group.interactable = true;
		if (this.buttons.Length != 0)
		{
			EventSystem.current.SetSelectedGameObject(this.buttons[0].gameObject);
		}
		ManagerSingleton<InputHandler>.Instance.StartUIInput();
		yield break;
	}

	// Token: 0x06001F7A RID: 8058 RVA: 0x0008FD80 File Offset: 0x0008DF80
	public void Hide()
	{
		base.StartCoroutine(this.HideSequence(true));
	}

	// Token: 0x06001F7B RID: 8059 RVA: 0x0008FD90 File Offset: 0x0008DF90
	private IEnumerator HideSequence(bool sendEvent)
	{
		GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
		if (currentSelectedGameObject)
		{
			MenuButton component = currentSelectedGameObject.GetComponent<MenuButton>();
			if (component)
			{
				component.ForceDeselect();
			}
		}
		if (this.animator)
		{
			this.animator.Play("Close");
			yield return null;
			yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
		}
		if (sendEvent && this.OnHidden != null)
		{
			this.OnHidden();
		}
		GameCameras.instance.HUDIn();
		if (this.allSelectedEffect)
		{
			this.allSelectedEffect.SetActive(false);
		}
		base.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06001F7C RID: 8060 RVA: 0x0008FDA8 File Offset: 0x0008DFA8
	public void Begin()
	{
		base.StartCoroutine(this.HideSequence(false));
		GameManager.instance.playerData.bossReturnEntryGate = this.door.dreamReturnGate.name;
		BossSequenceController.ChallengeBindings challengeBindings = BossSequenceController.ChallengeBindings.None;
		if (this.boundNailButton.Selected)
		{
			challengeBindings |= BossSequenceController.ChallengeBindings.Nail;
		}
		if (this.boundHeartButton.Selected)
		{
			challengeBindings |= BossSequenceController.ChallengeBindings.Shell;
		}
		if (this.boundCharmsButton.Selected)
		{
			challengeBindings |= BossSequenceController.ChallengeBindings.Charms;
		}
		if (this.boundSoulButton.Selected)
		{
			challengeBindings |= BossSequenceController.ChallengeBindings.Soul;
		}
		BossSequenceController.SetupNewSequence(this.door.bossSequence, challengeBindings, this.door.playerDataString);
		if (this.OnBegin != null)
		{
			this.OnBegin();
		}
	}

	// Token: 0x04001E78 RID: 7800
	public Text titleTextSuper;

	// Token: 0x04001E79 RID: 7801
	public Text titleTextMain;

	// Token: 0x04001E7A RID: 7802
	public Text descriptionText;

	// Token: 0x04001E7B RID: 7803
	public BossDoorChallengeUIBindingButton boundNailButton;

	// Token: 0x04001E7C RID: 7804
	public BossDoorChallengeUIBindingButton boundHeartButton;

	// Token: 0x04001E7D RID: 7805
	public BossDoorChallengeUIBindingButton boundCharmsButton;

	// Token: 0x04001E7E RID: 7806
	public BossDoorChallengeUIBindingButton boundSoulButton;

	// Token: 0x04001E7F RID: 7807
	private BossDoorChallengeUIBindingButton[] buttons;

	// Token: 0x04001E80 RID: 7808
	private bool allPreviouslySelected;

	// Token: 0x04001E81 RID: 7809
	public AudioSource audioPlayerPrefab;

	// Token: 0x04001E82 RID: 7810
	public AudioEvent allSelectedSound;

	// Token: 0x04001E83 RID: 7811
	public GameObject allSelectedEffect;

	// Token: 0x04001E84 RID: 7812
	private BossSequenceDoor door;

	// Token: 0x04001E85 RID: 7813
	private Animator animator;

	// Token: 0x04001E86 RID: 7814
	private Canvas canvas;

	// Token: 0x04001E87 RID: 7815
	private CanvasGroup group;

	// Token: 0x02001658 RID: 5720
	// (Invoke) Token: 0x060089CA RID: 35274
	public delegate void HideEvent();

	// Token: 0x02001659 RID: 5721
	// (Invoke) Token: 0x060089CE RID: 35278
	public delegate void BeginEvent();
}
