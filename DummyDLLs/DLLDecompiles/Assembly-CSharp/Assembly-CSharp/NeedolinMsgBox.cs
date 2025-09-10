using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.Localization;
using TeamCherry.NestedFadeGroup;
using TMProOld;
using UnityEngine;

// Token: 0x02000634 RID: 1588
public class NeedolinMsgBox : MonoBehaviour
{
	// Token: 0x060038A7 RID: 14503 RVA: 0x000FA1A4 File Offset: 0x000F83A4
	private void Awake()
	{
		if (!NeedolinMsgBox._instance)
		{
			NeedolinMsgBox._instance = this;
		}
		if (this.boxFade)
		{
			this.boxFade.AlphaSelf = 0f;
		}
		if (this.animator)
		{
			this.animator.enabled = false;
		}
		this.gm = GameManager.instance;
		this.gm.NextSceneWillActivate += this.ClearAllText;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "DIALOGUE BOX APPEARING").ReceivedEvent += this.HideNeedolinMsgBox;
	}

	// Token: 0x060038A8 RID: 14504 RVA: 0x000FA23C File Offset: 0x000F843C
	private void OnDestroy()
	{
		if (NeedolinMsgBox._instance == this)
		{
			NeedolinMsgBox._instance = null;
		}
		if (!this.gm)
		{
			return;
		}
		this.gm.NextSceneWillActivate -= this.ClearAllText;
		this.gm = null;
	}

	// Token: 0x060038A9 RID: 14505 RVA: 0x000FA288 File Offset: 0x000F8488
	public static void AddText(ILocalisedTextCollection text, bool skipStartDelay = false, bool maxPriority = false)
	{
		if (!NeedolinMsgBox._instance)
		{
			return;
		}
		NeedolinMsgBox._instance.InternalAddText(text, skipStartDelay, maxPriority);
	}

	// Token: 0x060038AA RID: 14506 RVA: 0x000FA2A4 File Offset: 0x000F84A4
	public static void RemoveText(ILocalisedTextCollection text)
	{
		if (!NeedolinMsgBox._instance)
		{
			return;
		}
		NeedolinMsgBox._instance.InternalRemoveText(text);
	}

	// Token: 0x060038AB RID: 14507 RVA: 0x000FA2BE File Offset: 0x000F84BE
	public static void AddBlocker(object blocker)
	{
		if (!NeedolinMsgBox._instance)
		{
			return;
		}
		NeedolinMsgBox._instance.blockers.Add(blocker);
		NeedolinMsgBox._instance.RemoveTextShared();
	}

	// Token: 0x060038AC RID: 14508 RVA: 0x000FA2E8 File Offset: 0x000F84E8
	public static void RemoveBlocker(object blocker)
	{
		if (!NeedolinMsgBox._instance)
		{
			return;
		}
		NeedolinMsgBox._instance.blockers.Remove(blocker);
		if (NeedolinMsgBox._instance.blockers.Count > 0)
		{
			return;
		}
		NeedolinMsgBox._instance.willSkipStartDelay = false;
		NeedolinMsgBox._instance.AddTextShared();
	}

	// Token: 0x17000670 RID: 1648
	// (get) Token: 0x060038AD RID: 14509 RVA: 0x000FA33B File Offset: 0x000F853B
	public static bool IsBlocked
	{
		get
		{
			return NeedolinMsgBox._instance && NeedolinMsgBox._instance.blockers.Count > 0;
		}
	}

	// Token: 0x060038AE RID: 14510 RVA: 0x000FA360 File Offset: 0x000F8560
	private void InternalAddText(ILocalisedTextCollection text, bool skipStartDelay, bool maxPriority)
	{
		if (!text.IsActive)
		{
			return;
		}
		if (maxPriority)
		{
			this.maxPriorityTexts.Add(text);
		}
		else
		{
			this.currentTexts.Add(text);
		}
		if (NeedolinMsgBox._instance.blockers.Count > 0)
		{
			return;
		}
		if (this.cycleTextsRoutine != null)
		{
			this.queuedTextCollection = text;
		}
		this.willSkipStartDelay = skipStartDelay;
		this.AddTextShared();
	}

	// Token: 0x060038AF RID: 14511 RVA: 0x000FA3C2 File Offset: 0x000F85C2
	private void AddTextShared()
	{
		if (this.cycleTextsRoutine == null && this.currentTexts.Count + this.maxPriorityTexts.Count > 0)
		{
			this.cycleTextsRoutine = base.StartCoroutine(this.CycleTexts());
		}
	}

	// Token: 0x060038B0 RID: 14512 RVA: 0x000FA3F8 File Offset: 0x000F85F8
	private void InternalRemoveText(ILocalisedTextCollection text)
	{
		this.currentTexts.Remove(text);
		this.maxPriorityTexts.Remove(text);
		this.RemoveTextShared();
	}

	// Token: 0x060038B1 RID: 14513 RVA: 0x000FA41C File Offset: 0x000F861C
	private void RemoveTextShared()
	{
		if (this.currentTexts.Count + this.maxPriorityTexts.Count != 0 && this.blockers.Count <= 0)
		{
			return;
		}
		if (this.cycleTextsRoutine != null)
		{
			base.StopCoroutine(this.cycleTextsRoutine);
			this.cycleTextsRoutine = null;
		}
		if (!this.hasAppeared)
		{
			return;
		}
		this.hasAppeared = false;
		if (this.hideRoutine == null)
		{
			this.hideRoutine = base.StartCoroutine(this.Hide(false));
		}
	}

	// Token: 0x060038B2 RID: 14514 RVA: 0x000FA498 File Offset: 0x000F8698
	private void HideNeedolinMsgBox()
	{
		if (this.cycleTextsRoutine != null)
		{
			base.StopCoroutine(this.cycleTextsRoutine);
			this.cycleTextsRoutine = null;
		}
		if (this.isHidden)
		{
			return;
		}
		if (this.hideRoutine != null)
		{
			base.StopCoroutine(this.hideRoutine);
			this.hideRoutine = null;
		}
		this.currentTexts.Clear();
		this.maxPriorityTexts.Clear();
		this.animator.SetFloat(NeedolinMsgBox._speedId, this.hideInstantSpeedModifier);
		this.hideRoutine = base.StartCoroutine(this.Hide(false));
		this.hasAppeared = false;
		this.isHidden = true;
		this.queuedTextCollection = null;
		this.previousText = default(LocalisedString);
		this.previousCollection = null;
	}

	// Token: 0x060038B3 RID: 14515 RVA: 0x000FA54C File Offset: 0x000F874C
	private void ClearAllText()
	{
		this.currentTexts.Clear();
		this.maxPriorityTexts.Clear();
		this.blockers.Clear();
		if (this.cycleTextsRoutine != null)
		{
			base.StopCoroutine(this.cycleTextsRoutine);
			this.cycleTextsRoutine = null;
		}
		if (this.hideRoutine != null)
		{
			base.StopCoroutine(this.hideRoutine);
			this.hideRoutine = null;
		}
		if (this.hasAppeared)
		{
			this.animator.Play(NeedolinMsgBox._disappearId, 0, 1f);
			this.animator.Update(0f);
			this.hasAppeared = false;
			this.isHidden = true;
		}
		if (this.boxFade)
		{
			this.boxFade.AlphaSelf = 0f;
		}
		if (this.animator)
		{
			this.animator.enabled = false;
		}
		this.queuedTextCollection = null;
		this.previousText = default(LocalisedString);
		this.previousCollection = null;
	}

	// Token: 0x060038B4 RID: 14516 RVA: 0x000FA63C File Offset: 0x000F883C
	private IEnumerator CycleTexts()
	{
		if (!this.willSkipStartDelay)
		{
			yield return new WaitForSecondsInterruptable(this.appearPause, () => this.willSkipStartDelay, false);
		}
		while (this.hideRoutine != null)
		{
			yield return null;
		}
		LocalisedString localisedString;
		NeedolinTextConfig needolinTextConfig;
		this.GetNewText(true, out localisedString, out needolinTextConfig);
		LocalisedString s;
		NeedolinTextConfig config;
		this.GetNewText(false, out s, out config);
		string text = s.IsEmpty ? string.Empty : s;
		this.primaryText.text = text;
		if (this.backboardSprites.Length != 0)
		{
			this.primaryBackboard.sprite = this.backboardSprites[this.currentBackboardSpriteIndex];
		}
		if (this.animator && !this.animator.enabled)
		{
			this.animator.enabled = true;
		}
		this.animator.SetFloat(NeedolinMsgBox._speedId, config.SpeedMultiplier);
		this.isHidden = false;
		this.hasAppeared = true;
		bool didPlayAppear;
		if (!string.IsNullOrEmpty(text))
		{
			didPlayAppear = true;
			this.animator.Play(NeedolinMsgBox._appearId, 0, 0f);
			yield return null;
			yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
		}
		else
		{
			didPlayAppear = false;
		}
		for (;;)
		{
			yield return new WaitForSeconds(config.HoldDuration);
			this.animator.SetFloat(NeedolinMsgBox._speedId, config.SpeedMultiplier);
			if (didPlayAppear)
			{
				this.GetNewText(false, out s, out config);
				text = (s.IsEmpty ? string.Empty : s);
			}
			if (!didPlayAppear || string.IsNullOrEmpty(text))
			{
				float disappearLength;
				if (didPlayAppear)
				{
					this.animator.Play(NeedolinMsgBox._disappearId, 0, 0f);
					yield return null;
					disappearLength = this.animator.GetCurrentAnimatorStateInfo(0).length;
					yield return new WaitForSeconds(disappearLength + config.HoldDuration);
				}
				else
				{
					disappearLength = 0f;
				}
				for (;;)
				{
					this.GetNewText(false, out s, out config);
					text = (s.IsEmpty ? string.Empty : s);
					if (!string.IsNullOrEmpty(text))
					{
						break;
					}
					yield return new WaitForSeconds(disappearLength + config.HoldDuration);
				}
				this.primaryText.text = text;
				didPlayAppear = true;
				this.animator.Play(NeedolinMsgBox._appearId, 0, 0f);
				yield return null;
				yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
			}
			else
			{
				this.secondaryText.text = this.primaryText.text;
				this.secondaryBackboard.sprite = this.primaryBackboard.sprite;
				this.primaryText.text = text;
				if (this.backboardSprites.Length != 0)
				{
					this.currentBackboardSpriteIndex++;
					if (this.currentBackboardSpriteIndex >= this.backboardSprites.Length)
					{
						this.currentBackboardSpriteIndex = 0;
					}
				}
				this.primaryBackboard.sprite = this.backboardSprites[this.currentBackboardSpriteIndex];
				this.animator.Play(NeedolinMsgBox._swapTextId, 0, 0f);
				yield return null;
				yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
			}
		}
		yield break;
	}

	// Token: 0x060038B5 RID: 14517 RVA: 0x000FA64B File Offset: 0x000F884B
	private IEnumerator Hide(bool skipPause = false)
	{
		if (this.animator.GetCurrentAnimatorStateInfo(0).shortNameHash != NeedolinMsgBox._disappearId)
		{
			while (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
			{
				yield return null;
			}
		}
		if (!skipPause)
		{
			yield return new WaitForSeconds(this.hidePause);
		}
		this.isHidden = true;
		this.gapsLeft.Clear();
		if (this.animator.GetCurrentAnimatorStateInfo(0).shortNameHash != NeedolinMsgBox._disappearId)
		{
			this.animator.Play(NeedolinMsgBox._disappearId, 0, 0f);
			yield return null;
			yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
		}
		this.hideRoutine = null;
		yield break;
	}

	// Token: 0x060038B6 RID: 14518 RVA: 0x000FA664 File Offset: 0x000F8864
	private void GetNewText(bool isFirst, out LocalisedString text, out NeedolinTextConfig config)
	{
		ILocalisedTextCollection localisedTextCollection;
		if (this.maxPriorityTexts.Count > 0)
		{
			localisedTextCollection = this.maxPriorityTexts[Random.Range(0, this.maxPriorityTexts.Count)];
		}
		else if (this.queuedTextCollection != null && this.queuedTextCollection != this.previousCollection)
		{
			localisedTextCollection = this.queuedTextCollection;
			this.queuedTextCollection = null;
		}
		else
		{
			if (this.currentTexts.Count <= 0)
			{
				text = default(LocalisedString);
				config = this.defaultConfig;
				this.previousText = default(LocalisedString);
				this.previousCollection = null;
				return;
			}
			localisedTextCollection = null;
			this.currentTexts.Shuffle<ILocalisedTextCollection>();
			int num;
			try
			{
				foreach (ILocalisedTextCollection localisedTextCollection2 in this.currentTexts)
				{
					NeedolinTextConfig config2 = localisedTextCollection2.GetConfig();
					if (config2 && config2.EmptyStartGap > 0)
					{
						this.uniqueTextsTempLow.Add(localisedTextCollection2);
					}
					else
					{
						this.uniqueTextsTempHigh.Add(localisedTextCollection2);
					}
				}
			}
			finally
			{
				num = ((this.uniqueTextsTempHigh.Count > 0) ? this.uniqueTextsTempHigh.Count : this.uniqueTextsTempLow.Count);
				this.uniqueTextsTempLow.Clear();
				this.uniqueTextsTempHigh.Clear();
			}
			foreach (ILocalisedTextCollection localisedTextCollection3 in this.currentTexts)
			{
				int num2;
				if (this.gapsLeft.TryGetValue(localisedTextCollection3, out num2))
				{
					num2--;
					if (num2 > 0)
					{
						this.gapsLeft[localisedTextCollection3] = num2;
					}
					else
					{
						this.gapsLeft.Remove(localisedTextCollection3);
					}
				}
				else if (this.previousCollection == null || num <= 1 || localisedTextCollection3 != this.previousCollection)
				{
					localisedTextCollection = localisedTextCollection3;
				}
			}
			if (localisedTextCollection == null)
			{
				text = default(LocalisedString);
				config = this.defaultConfig;
				return;
			}
		}
		text = localisedTextCollection.GetRandom(this.previousText);
		config = localisedTextCollection.GetConfig();
		this.previousText = text;
		this.previousCollection = localisedTextCollection;
		if (!config)
		{
			config = this.defaultConfig;
		}
		int num3 = config.EmptyGap.GetRandomValue(true);
		if (isFirst)
		{
			num3 += config.EmptyStartGap;
		}
		if (num3 > 0)
		{
			this.gapsLeft[localisedTextCollection] = num3;
		}
	}

	// Token: 0x04003BA2 RID: 15266
	[SerializeField]
	private NestedFadeGroupBase boxFade;

	// Token: 0x04003BA3 RID: 15267
	[SerializeField]
	private Animator animator;

	// Token: 0x04003BA4 RID: 15268
	[SerializeField]
	private TMP_Text primaryText;

	// Token: 0x04003BA5 RID: 15269
	[SerializeField]
	private SpriteRenderer primaryBackboard;

	// Token: 0x04003BA6 RID: 15270
	[SerializeField]
	private TMP_Text secondaryText;

	// Token: 0x04003BA7 RID: 15271
	[SerializeField]
	private SpriteRenderer secondaryBackboard;

	// Token: 0x04003BA8 RID: 15272
	[SerializeField]
	private Sprite[] backboardSprites;

	// Token: 0x04003BA9 RID: 15273
	[SerializeField]
	private float appearPause;

	// Token: 0x04003BAA RID: 15274
	[SerializeField]
	private float hidePause;

	// Token: 0x04003BAB RID: 15275
	[SerializeField]
	private float hideInstantSpeedModifier = 2.5f;

	// Token: 0x04003BAC RID: 15276
	[Space]
	[SerializeField]
	[AssetPickerDropdown]
	private NeedolinTextConfig defaultConfig;

	// Token: 0x04003BAD RID: 15277
	private readonly List<ILocalisedTextCollection> currentTexts = new List<ILocalisedTextCollection>();

	// Token: 0x04003BAE RID: 15278
	private readonly List<ILocalisedTextCollection> maxPriorityTexts = new List<ILocalisedTextCollection>();

	// Token: 0x04003BAF RID: 15279
	private readonly List<object> blockers = new List<object>();

	// Token: 0x04003BB0 RID: 15280
	private readonly HashSet<ILocalisedTextCollection> uniqueTextsTempLow = new HashSet<ILocalisedTextCollection>();

	// Token: 0x04003BB1 RID: 15281
	private readonly HashSet<ILocalisedTextCollection> uniqueTextsTempHigh = new HashSet<ILocalisedTextCollection>();

	// Token: 0x04003BB2 RID: 15282
	private LocalisedString previousText;

	// Token: 0x04003BB3 RID: 15283
	private ILocalisedTextCollection previousCollection;

	// Token: 0x04003BB4 RID: 15284
	private ILocalisedTextCollection queuedTextCollection;

	// Token: 0x04003BB5 RID: 15285
	private Dictionary<ILocalisedTextCollection, int> gapsLeft = new Dictionary<ILocalisedTextCollection, int>();

	// Token: 0x04003BB6 RID: 15286
	private Coroutine cycleTextsRoutine;

	// Token: 0x04003BB7 RID: 15287
	private Coroutine hideRoutine;

	// Token: 0x04003BB8 RID: 15288
	private bool willSkipStartDelay;

	// Token: 0x04003BB9 RID: 15289
	private int currentBackboardSpriteIndex;

	// Token: 0x04003BBA RID: 15290
	private bool hasAppeared;

	// Token: 0x04003BBB RID: 15291
	private GameManager gm;

	// Token: 0x04003BBC RID: 15292
	private static NeedolinMsgBox _instance;

	// Token: 0x04003BBD RID: 15293
	private static readonly int _speedId = Animator.StringToHash("Speed");

	// Token: 0x04003BBE RID: 15294
	private static readonly int _appearId = Animator.StringToHash("Appear");

	// Token: 0x04003BBF RID: 15295
	private static readonly int _swapTextId = Animator.StringToHash("Swap Text");

	// Token: 0x04003BC0 RID: 15296
	private static readonly int _disappearId = Animator.StringToHash("Disappear");

	// Token: 0x04003BC1 RID: 15297
	private bool isHidden = true;
}
