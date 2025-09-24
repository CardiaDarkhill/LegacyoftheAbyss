using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.Localization;
using TMProOld;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200062F RID: 1583
public class DialogueYesNoBox : YesNoBox
{
	// Token: 0x1700066C RID: 1644
	// (get) Token: 0x0600386D RID: 14445 RVA: 0x000F91EC File Offset: 0x000F73EC
	protected override string InactiveYesText
	{
		get
		{
			if (this.willGetItem && !this.willGetItem.CanGetMore())
			{
				return this.atMaxText;
			}
			if (this.requiredItems.Count > 0)
			{
				for (int i = 0; i < this.requiredItems.Count; i++)
				{
					SavedItem savedItem = this.requiredItems[i];
					int num = this.requiredItemAmounts[i];
					if (savedItem.GetSavedAmount() < num)
					{
						return this.notEnoughText;
					}
				}
			}
			if (this.requiredCurrencyType == null)
			{
				return string.Empty;
			}
			if (CurrencyManager.GetCurrencyAmount(this.requiredCurrencyType.Value) >= this.requiredCurrencyAmount)
			{
				return string.Empty;
			}
			return this.notEnoughText;
		}
	}

	// Token: 0x1700066D RID: 1645
	// (get) Token: 0x0600386E RID: 14446 RVA: 0x000F92AC File Offset: 0x000F74AC
	protected override bool ShouldHideHud
	{
		get
		{
			return this.requiredItemAmounts.Count <= 0 && this.requiredCurrencyAmount <= 0;
		}
	}

	// Token: 0x0600386F RID: 14447 RVA: 0x000F92CA File Offset: 0x000F74CA
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<GameObject>(ref this.currencyDisplays, typeof(CurrencyType));
	}

	// Token: 0x06003870 RID: 14448 RVA: 0x000F92E1 File Offset: 0x000F74E1
	protected override void Awake()
	{
		base.Awake();
		this.OnValidate();
		if (!DialogueYesNoBox._instance)
		{
			DialogueYesNoBox._instance = this;
		}
	}

	// Token: 0x06003871 RID: 14449 RVA: 0x000F9301 File Offset: 0x000F7501
	private void OnDestroy()
	{
		if (DialogueYesNoBox._instance == this)
		{
			DialogueYesNoBox._instance = null;
		}
	}

	// Token: 0x06003872 RID: 14450 RVA: 0x000F9318 File Offset: 0x000F7518
	public static void Open(Action yes, Action no, bool returnHud, string text, SavedItem willGetItem = null)
	{
		if (!DialogueYesNoBox._instance)
		{
			return;
		}
		if (DialogueYesNoBox._instance.animator)
		{
			DialogueYesNoBox._instance.animator.SetBool(DialogueYesNoBox._textFinishedPropId, false);
		}
		if (DialogueYesNoBox._instance.instantiatedItems != null)
		{
			foreach (SavedItemDisplay savedItemDisplay in DialogueYesNoBox._instance.instantiatedItems)
			{
				savedItemDisplay.gameObject.SetActive(false);
			}
		}
		DialogueYesNoBox._instance.PreOpen(willGetItem, text);
		DialogueYesNoBox._instance.InternalOpen(yes, no, returnHud);
	}

	// Token: 0x06003873 RID: 14451 RVA: 0x000F93CC File Offset: 0x000F75CC
	public static void Open(Action yes, Action no, bool returnHud, SavedItem item, int amount, bool displayHudPopup = true, bool consumeCurrency = false, SavedItem willGetItem = null)
	{
		string format = Language.Get("GIVE_ITEM_PROMPT", "UI");
		format = string.Format(format, item.GetPopupName());
		DialogueYesNoBox.Open(yes, no, returnHud, format, CurrencyType.Money, 0, new SavedItem[]
		{
			item
		}, new int[]
		{
			amount
		}, displayHudPopup, consumeCurrency, willGetItem, TakeItemTypes.Silent);
	}

	// Token: 0x06003874 RID: 14452 RVA: 0x000F9420 File Offset: 0x000F7620
	public static void Open(Action yes, Action no, bool returnHud, string formatText, SavedItem item, int amount, bool displayHudPopup = true, bool consumeCurrency = false, SavedItem willGetItem = null)
	{
		string text;
		try
		{
			CollectableItem collectableItem = item as CollectableItem;
			text = string.Format(formatText, collectableItem ? collectableItem.GetDisplayName(CollectableItem.ReadSource.TakePopup) : item.GetPopupName());
		}
		catch (FormatException)
		{
			text = formatText;
		}
		DialogueYesNoBox.Open(yes, no, returnHud, text, CurrencyType.Money, 0, new SavedItem[]
		{
			item
		}, new int[]
		{
			amount
		}, displayHudPopup, consumeCurrency, willGetItem, TakeItemTypes.Silent);
	}

	// Token: 0x06003875 RID: 14453 RVA: 0x000F9494 File Offset: 0x000F7694
	private void PreOpen(SavedItem willGet, string textValue)
	{
		GameObject[] array = this.currencyDisplays;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		this.requiredCurrencyType = null;
		this.requiredCurrencyAmount = 0;
		this.requiredItems.Clear();
		this.requiredItemAmounts.Clear();
		this.currencyParent.SetActive(false);
		this.itemsLayout.gameObject.SetActive(false);
		DialogueYesNoBox._instance.itemTemplate.gameObject.SetActive(false);
		this.willGetItem = willGet;
		base.StartCoroutine(DialogueYesNoBox._instance.AnimateOut(textValue));
	}

	// Token: 0x06003876 RID: 14454 RVA: 0x000F9534 File Offset: 0x000F7734
	public static void Open(Action yes, Action no, bool returnHud, string text, CurrencyType currencyType, int amount, bool displayHudPopup = true, bool consumeCurrency = true, SavedItem willGetItem = null)
	{
		DialogueYesNoBox.Open(yes, no, returnHud, text, currencyType, amount, null, null, displayHudPopup, consumeCurrency, willGetItem, TakeItemTypes.Silent);
	}

	// Token: 0x06003877 RID: 14455 RVA: 0x000F9558 File Offset: 0x000F7758
	public static void Open(Action yes, Action no, bool returnHud, string text, IReadOnlyList<SavedItem> items, IReadOnlyList<int> amounts, bool displayHudPopup, bool consumeCurrency, SavedItem willGetItem)
	{
		DialogueYesNoBox.Open(yes, no, returnHud, text, CurrencyType.Money, 0, items, amounts, displayHudPopup, consumeCurrency, willGetItem, TakeItemTypes.Silent);
	}

	// Token: 0x06003878 RID: 14456 RVA: 0x000F957C File Offset: 0x000F777C
	public static void Open(Action yes, Action no, bool returnHud, string text, CurrencyType currencyType, int currencyAmount, IReadOnlyList<SavedItem> items, IReadOnlyList<int> amounts, bool displayHudPopup, bool consumeCurrency, SavedItem willGetItem, TakeItemTypes takeItemType = TakeItemTypes.Silent)
	{
		if (!DialogueYesNoBox._instance)
		{
			return;
		}
		if (string.IsNullOrEmpty(text))
		{
			text = Language.Get("GIVE_ITEMS_PROMPT", "UI");
		}
		DialogueYesNoBox._instance.PreOpen(willGetItem, text);
		if (currencyAmount > 0)
		{
			string text2 = (currencyAmount > 1) ? currencyAmount.ToString() : string.Empty;
			DialogueYesNoBox._instance.currencyDisplays[(int)currencyType].SetActive(true);
			if (DialogueYesNoBox._instance.currencyText)
			{
				DialogueYesNoBox._instance.currencyText.text = text2;
			}
			DialogueYesNoBox._instance.currencyParent.SetActive(true);
			DialogueYesNoBox._instance.requiredCurrencyAmount = currencyAmount;
			DialogueYesNoBox._instance.requiredCurrencyType = new CurrencyType?(currencyType);
			if (displayHudPopup)
			{
				CurrencyCounter.Show(currencyType, false);
			}
		}
		SavedItem item;
		CollectableItem collectableItem;
		if (items != null && items.Count > 0)
		{
			while (DialogueYesNoBox._instance.instantiatedItems.Count < items.Count)
			{
				DialogueYesNoBox._instance.instantiatedItems.Add(Object.Instantiate<SavedItemDisplay>(DialogueYesNoBox._instance.itemTemplate, DialogueYesNoBox._instance.itemTemplate.transform.parent));
			}
			for (int i = 0; i < DialogueYesNoBox._instance.instantiatedItems.Count; i++)
			{
				SavedItemDisplay savedItemDisplay = DialogueYesNoBox._instance.instantiatedItems[i];
				if (i < items.Count)
				{
					savedItemDisplay.gameObject.SetActive(true);
					item = items[i];
					int amount = amounts[i];
					savedItemDisplay.Setup(item, amount);
				}
				else
				{
					savedItemDisplay.gameObject.SetActive(false);
				}
			}
			DialogueYesNoBox._instance.itemsLayout.gameObject.SetActive(true);
			DialogueYesNoBox._instance.itemsLayout.ForceUpdateLayoutNoCanvas();
			DialogueYesNoBox._instance.requiredItemAmounts.AddRange(amounts);
			DialogueYesNoBox._instance.requiredItems.AddRange(items);
			if (displayHudPopup && takeItemType == TakeItemTypes.Silent)
			{
				foreach (SavedItem savedItem in items)
				{
					collectableItem = (savedItem as CollectableItem);
					if (collectableItem != null)
					{
						ItemCurrencyCounter.Show(collectableItem);
					}
				}
			}
		}
		Action yes2 = delegate()
		{
			bool flag = false;
			if (consumeCurrency)
			{
				int waitingCount = 0;
				if (currencyAmount > 0)
				{
					CurrencyManager.TakeCurrency(currencyAmount, currencyType, displayHudPopup);
					if (yes != null & displayHudPopup)
					{
						int waitingCount3 = waitingCount;
						waitingCount = waitingCount3 + 1;
						CurrencyCounterTyped<CurrencyType>.RegisterTempCounterStateChangedHandler(delegate(CurrencyType type, CurrencyCounterBase.StateEvents state)
						{
							if (type != currencyType || state != CurrencyCounterBase.StateEvents.FadeDelayElapsed)
							{
								return false;
							}
							CurrencyCounter.Hide(currencyType);
							int waitingCount2 = waitingCount;
							waitingCount = waitingCount2 - 1;
							if (waitingCount == 0)
							{
								yes();
							}
							return true;
						});
						flag = true;
					}
				}
				if (items != null)
				{
					for (int j = 0; j < items.Count; j++)
					{
						SavedItem item = items[j];
						ToolItem toolItem = item as ToolItem;
						if (toolItem != null)
						{
							toolItem.Lock();
						}
						else
						{
							CollectableItem collectableItem = item as CollectableItem;
							if (collectableItem != null)
							{
								int amount2 = amounts[j];
								bool flag2;
								if (takeItemType == TakeItemTypes.Silent)
								{
									collectableItem.Take(amount2, displayHudPopup);
									flag2 = displayHudPopup;
								}
								else
								{
									CollectableUIMsg.ShowTakeMsg(collectableItem, takeItemType);
									collectableItem.Take(amount2, false);
									flag2 = false;
								}
								if (yes != null && flag2)
								{
									int waitingCount3 = waitingCount;
									waitingCount = waitingCount3 + 1;
									CurrencyCounterTyped<CollectableItem>.RegisterTempCounterStateChangedHandler(delegate(CollectableItem type, CurrencyCounterBase.StateEvents state)
									{
										if (type != item || state != CurrencyCounterBase.StateEvents.FadeDelayElapsed)
										{
											return false;
										}
										ItemCurrencyCounter.Hide(collectableItem);
										int waitingCount2 = waitingCount;
										waitingCount = waitingCount2 - 1;
										if (waitingCount == 0)
										{
											yes();
										}
										return true;
									});
									flag = true;
								}
							}
						}
					}
				}
			}
			else
			{
				if (currencyAmount > 0)
				{
					CurrencyCounter.Hide(currencyType);
				}
				if (items != null && takeItemType == TakeItemTypes.Silent)
				{
					foreach (SavedItem savedItem2 in items)
					{
						CollectableItem collectableItem2 = savedItem2 as CollectableItem;
						if (collectableItem2 != null)
						{
							ItemCurrencyCounter.Hide(collectableItem2);
						}
					}
				}
			}
			if (!flag && yes != null)
			{
				yes();
			}
		};
		Action no2 = delegate()
		{
			if (displayHudPopup)
			{
				if (currencyAmount > 0)
				{
					CurrencyCounter.Hide(currencyType);
				}
				if (items != null && takeItemType == TakeItemTypes.Silent)
				{
					foreach (SavedItem savedItem2 in items)
					{
						CollectableItem collectableItem4 = savedItem2 as CollectableItem;
						if (collectableItem4 != null)
						{
							ItemCurrencyCounter.Hide(collectableItem4);
						}
					}
				}
			}
			Action no3 = no;
			if (no3 == null)
			{
				return;
			}
			no3();
		};
		DialogueYesNoBox._instance.InternalOpen(yes2, no2, returnHud);
	}

	// Token: 0x06003879 RID: 14457 RVA: 0x000F9878 File Offset: 0x000F7A78
	private IEnumerator AnimateOut(string textValue)
	{
		base.OnAppearing();
		if (this.text)
		{
			this.text.text = textValue;
			this.text.maxVisibleCharacters = 0;
			yield return new WaitForSeconds(this.textRevealWait);
			float visibleCharacters = 0f;
			while (this.text.maxVisibleCharacters < textValue.Length)
			{
				yield return null;
				visibleCharacters += this.textRevealSpeed * Time.deltaTime;
				this.text.maxVisibleCharacters = Mathf.RoundToInt(visibleCharacters);
			}
		}
		yield return null;
		if (this.animator)
		{
			this.animator.SetBool(DialogueYesNoBox._textFinishedPropId, true);
		}
		if (this.appearedEventDelay > 0f)
		{
			yield return new WaitForSeconds(this.appearedEventDelay);
		}
		base.OnAppeared();
		yield break;
	}

	// Token: 0x0600387A RID: 14458 RVA: 0x000F988E File Offset: 0x000F7A8E
	public static void ForceClose()
	{
		if (DialogueYesNoBox._instance)
		{
			DialogueYesNoBox._instance.DoEnd();
		}
	}

	// Token: 0x04003B5C RID: 15196
	[Space]
	[SerializeField]
	private Animator animator;

	// Token: 0x04003B5D RID: 15197
	[Space]
	[SerializeField]
	private float appearedEventDelay = 0.26666668f;

	// Token: 0x04003B5E RID: 15198
	[SerializeField]
	private TMP_Text text;

	// Token: 0x04003B5F RID: 15199
	[SerializeField]
	private float textRevealWait;

	// Token: 0x04003B60 RID: 15200
	[SerializeField]
	private float textRevealSpeed = 20f;

	// Token: 0x04003B61 RID: 15201
	[SerializeField]
	private GameObject currencyParent;

	// Token: 0x04003B62 RID: 15202
	[SerializeField]
	private TMP_Text currencyText;

	// Token: 0x04003B63 RID: 15203
	[SerializeField]
	[ArrayForEnum(typeof(CurrencyType))]
	private GameObject[] currencyDisplays;

	// Token: 0x04003B64 RID: 15204
	[SerializeField]
	private SavedItemDisplay itemTemplate;

	// Token: 0x04003B65 RID: 15205
	[SerializeField]
	private LayoutGroup itemsLayout;

	// Token: 0x04003B66 RID: 15206
	[Space]
	[SerializeField]
	private LocalisedString notEnoughText;

	// Token: 0x04003B67 RID: 15207
	[SerializeField]
	private LocalisedString atMaxText;

	// Token: 0x04003B68 RID: 15208
	private CurrencyType? requiredCurrencyType;

	// Token: 0x04003B69 RID: 15209
	private int requiredCurrencyAmount;

	// Token: 0x04003B6A RID: 15210
	private readonly List<SavedItem> requiredItems = new List<SavedItem>();

	// Token: 0x04003B6B RID: 15211
	private readonly List<int> requiredItemAmounts = new List<int>();

	// Token: 0x04003B6C RID: 15212
	private SavedItem willGetItem;

	// Token: 0x04003B6D RID: 15213
	private readonly List<SavedItemDisplay> instantiatedItems = new List<SavedItemDisplay>();

	// Token: 0x04003B6E RID: 15214
	private static DialogueYesNoBox _instance;

	// Token: 0x04003B6F RID: 15215
	private static readonly int _textFinishedPropId = Animator.StringToHash("Text Finished");
}
