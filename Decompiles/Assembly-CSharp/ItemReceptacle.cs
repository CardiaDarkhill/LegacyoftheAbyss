using System;
using System.Collections;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using TMProOld;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000632 RID: 1586
public class ItemReceptacle : NPCControlBase
{
	// Token: 0x140000B5 RID: 181
	// (add) Token: 0x0600388A RID: 14474 RVA: 0x000F9AA8 File Offset: 0x000F7CA8
	// (remove) Token: 0x0600388B RID: 14475 RVA: 0x000F9AE0 File Offset: 0x000F7CE0
	public event Action Unlocked;

	// Token: 0x140000B6 RID: 182
	// (add) Token: 0x0600388C RID: 14476 RVA: 0x000F9B18 File Offset: 0x000F7D18
	// (remove) Token: 0x0600388D RID: 14477 RVA: 0x000F9B50 File Offset: 0x000F7D50
	public event Action StartedUnlocked;

	// Token: 0x1700066F RID: 1647
	// (get) Token: 0x0600388E RID: 14478 RVA: 0x000F9B85 File Offset: 0x000F7D85
	public override bool AutoEnd
	{
		get
		{
			return !this.inspectEventTarget || string.IsNullOrEmpty(this.inspectEndEvent);
		}
	}

	// Token: 0x0600388F RID: 14479 RVA: 0x000F9BA4 File Offset: 0x000F7DA4
	private bool? IsEventValid(string eventName)
	{
		if (this.inspectEventTarget)
		{
			return this.inspectEventTarget.IsEventValid(eventName, true);
		}
		return null;
	}

	// Token: 0x06003890 RID: 14480 RVA: 0x000F9BD8 File Offset: 0x000F7DD8
	protected override void Awake()
	{
		base.Awake();
		if (this.persistent && string.IsNullOrEmpty(this.playerDataBool))
		{
			this.persistent.OnGetSaveState += delegate(out bool value)
			{
				value = this.isActivated;
			};
			this.persistent.OnSetSaveState += delegate(bool value)
			{
				this.isActivated = value;
				if (this.isActivated)
				{
					this.StartedActivated();
				}
			};
		}
	}

	// Token: 0x06003891 RID: 14481 RVA: 0x000F9C34 File Offset: 0x000F7E34
	protected override void Start()
	{
		if (this.animator && !this.animatorStartActive)
		{
			this.animator.enabled = true;
			this.animator.Play(ItemReceptacle._unlockAnim, 0, 0f);
			this.animator.Update(0f);
			this.animator.enabled = false;
		}
		if (!string.IsNullOrEmpty(this.playerDataBool))
		{
			this.isActivated = PlayerData.instance.GetVariable(this.playerDataBool);
			if (this.isActivated)
			{
				this.StartedActivated();
			}
		}
	}

	// Token: 0x06003892 RID: 14482 RVA: 0x000F9CC8 File Offset: 0x000F7EC8
	private void StartedActivated()
	{
		if (this.animator)
		{
			this.animator.enabled = true;
			this.animator.Play(ItemReceptacle._unlockAnim, 0, 1f);
			this.animator.Update(0f);
			this.animator.enabled = false;
		}
		if (this.unlock)
		{
			this.unlock.Opened();
		}
		this.onStartUnlocked.Invoke();
		Action startedUnlocked = this.StartedUnlocked;
		if (startedUnlocked != null)
		{
			startedUnlocked();
		}
		base.Deactivate(false);
	}

	// Token: 0x06003893 RID: 14483 RVA: 0x000F9D5C File Offset: 0x000F7F5C
	protected override void OnStartDialogue()
	{
		ItemReceptacle.<>c__DisplayClass36_0 CS$<>8__locals1 = new ItemReceptacle.<>c__DisplayClass36_0();
		CS$<>8__locals1.<>4__this = this;
		base.DisableInteraction();
		this.yesNoOpened = false;
		CS$<>8__locals1.hasItem = (this.requiredItem && this.requiredItem.CollectedAmount >= this.requiredItemCount);
		if (CS$<>8__locals1.hasItem && this.skipInspectOnUse)
		{
			CS$<>8__locals1.<OnStartDialogue>g__OpenYesNo|0();
			return;
		}
		DialogueBox.StartConversation(this.inspectDialogue, this, false, new DialogueBox.DisplayOptions
		{
			ShowDecorators = true,
			Alignment = TextAlignmentOptions.Top,
			TextColor = Color.white
		}, delegate()
		{
			if (!CS$<>8__locals1.hasItem)
			{
				return;
			}
			base.<OnStartDialogue>g__OpenYesNo|0();
		});
	}

	// Token: 0x06003894 RID: 14484 RVA: 0x000F9E04 File Offset: 0x000F8004
	public override void OnDialogueBoxEnded()
	{
		if (!this.AutoEnd)
		{
			DialogueBox.EndConversation(true, null);
		}
	}

	// Token: 0x06003895 RID: 14485 RVA: 0x000F9E15 File Offset: 0x000F8015
	protected override void OnEndDialogue()
	{
		if (this.yesNoOpened)
		{
			return;
		}
		base.EnableInteraction();
		if (this.inspectEventTarget)
		{
			this.inspectEventTarget.SendEvent(this.inspectEndEvent);
		}
	}

	// Token: 0x06003896 RID: 14486 RVA: 0x000F9E44 File Offset: 0x000F8044
	private void AcceptedPrompt()
	{
		base.StartCoroutine(this.UnlockSequence());
	}

	// Token: 0x06003897 RID: 14487 RVA: 0x000F9E53 File Offset: 0x000F8053
	private void CanceledPrompt()
	{
		base.EnableInteraction();
	}

	// Token: 0x06003898 RID: 14488 RVA: 0x000F9E5B File Offset: 0x000F805B
	private IEnumerator UnlockSequence()
	{
		HeroController hc = HeroController.instance;
		tk2dSpriteAnimator heroAnimator = hc.GetComponent<tk2dSpriteAnimator>();
		HeroAnimationController heroAnim = hc.GetComponent<HeroAnimationController>();
		hc.StopAnimationControl();
		heroAnimator.Play(heroAnim.GetClip("Collect Stand 1"));
		yield return new WaitForSeconds(0.75f);
		heroAnimator.Play(heroAnim.GetClip("Collect Stand 2"));
		this.requiredItem.Take(this.requiredItemCount, false);
		this.isActivated = true;
		if (!string.IsNullOrEmpty(this.playerDataBool))
		{
			PlayerData.instance.SetVariable(this.playerDataBool, true);
		}
		if (this.unlockEffectPrefab)
		{
			Transform transform = this.unlockEffectPoint ? this.unlockEffectPoint : base.transform;
			this.unlockEffectPrefab.Spawn(transform.position);
		}
		this.unlockSound.SpawnAndPlayOneShot(base.transform.position, null);
		this.onUnlockEffect.Invoke();
		yield return new WaitForSeconds(0.5f);
		yield return base.StartCoroutine(heroAnimator.PlayAnimWait(heroAnim.GetClip("Collect Stand 3"), null));
		hc.StartAnimationControl();
		base.EnableInteraction();
		base.Deactivate(false);
		yield return new WaitForSeconds(this.unlockDelay);
		this.onPreUnlock.Invoke();
		if (this.animator)
		{
			this.animator.enabled = true;
			this.animator.Play(ItemReceptacle._unlockAnim, 0, 0f);
			yield return null;
			yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
		}
		yield return new WaitForSeconds(this.unlockEventDelay);
		this.onUnlock.Invoke();
		if (this.unlock)
		{
			this.unlock.Open();
		}
		Action unlocked = this.Unlocked;
		if (unlocked != null)
		{
			unlocked();
		}
		yield break;
	}

	// Token: 0x04003B7A RID: 15226
	[Space]
	[SerializeField]
	private LocalisedString inspectDialogue;

	// Token: 0x04003B7B RID: 15227
	[SerializeField]
	private LocalisedString promptFormatText;

	// Token: 0x04003B7C RID: 15228
	[SerializeField]
	private CollectableItem requiredItem;

	// Token: 0x04003B7D RID: 15229
	[SerializeField]
	private int requiredItemCount;

	// Token: 0x04003B7E RID: 15230
	[SerializeField]
	private bool skipInspectOnUse;

	// Token: 0x04003B7F RID: 15231
	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string playerDataBool;

	// Token: 0x04003B80 RID: 15232
	[SerializeField]
	[ModifiableProperty]
	[Conditional("playerDataBool", false, false, false)]
	private PersistentBoolItem persistent;

	// Token: 0x04003B81 RID: 15233
	[Space]
	[SerializeField]
	private AudioEvent unlockSound;

	// Token: 0x04003B82 RID: 15234
	[SerializeField]
	private Transform unlockEffectPoint;

	// Token: 0x04003B83 RID: 15235
	[SerializeField]
	private GameObject unlockEffectPrefab;

	// Token: 0x04003B84 RID: 15236
	[SerializeField]
	private float unlockDelay;

	// Token: 0x04003B85 RID: 15237
	[SerializeField]
	private Animator animator;

	// Token: 0x04003B86 RID: 15238
	[SerializeField]
	private bool animatorStartActive;

	// Token: 0x04003B87 RID: 15239
	[SerializeField]
	private float unlockEventDelay;

	// Token: 0x04003B88 RID: 15240
	[Space]
	[SerializeField]
	private UnlockablePropBase unlock;

	// Token: 0x04003B89 RID: 15241
	[Space]
	[SerializeField]
	private UnityEvent onUnlockEffect;

	// Token: 0x04003B8A RID: 15242
	[SerializeField]
	private UnityEvent onPreUnlock;

	// Token: 0x04003B8B RID: 15243
	[SerializeField]
	private UnityEvent onUnlock;

	// Token: 0x04003B8C RID: 15244
	[SerializeField]
	private UnityEvent onStartUnlocked;

	// Token: 0x04003B8D RID: 15245
	[Space]
	[SerializeField]
	private PlayMakerFSM inspectEventTarget;

	// Token: 0x04003B8E RID: 15246
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsEventValid")]
	private string inspectEndEvent;

	// Token: 0x04003B8F RID: 15247
	private static readonly int _unlockAnim = Animator.StringToHash("Unlock");

	// Token: 0x04003B90 RID: 15248
	private bool isActivated;

	// Token: 0x04003B91 RID: 15249
	private bool yesNoOpened;
}
