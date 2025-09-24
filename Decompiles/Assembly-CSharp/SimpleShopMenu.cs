using System;
using System.Collections;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using TMProOld;
using UnityEngine;

// Token: 0x0200072B RID: 1835
public class SimpleShopMenu : MonoBehaviour
{
	// Token: 0x06004183 RID: 16771 RVA: 0x00120475 File Offset: 0x0011E675
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<NestedFadeGroupBase>(ref this.stateFaders, typeof(SimpleShopMenu.State));
	}

	// Token: 0x06004184 RID: 16772 RVA: 0x0012048C File Offset: 0x0011E68C
	private void Awake()
	{
		this.OnValidate();
		this.pane = base.GetComponent<InventoryPaneStandalone>();
		this.pane.OnPaneStart += this.OnPaneStart;
		this.pane.PaneOpenedAnimEnd += this.OnPaneOpenedAnimEnd;
		this.pane.OnPaneEnd += this.OnPaneEnd;
		this.pane.OnInputUp += this.OnInputUp;
		this.pane.OnInputDown += this.OnInputDown;
		this.paneInput = base.GetComponent<InventoryPaneInput>();
		if (this.templateItem)
		{
			this.templateItem.gameObject.SetActive(false);
		}
		if (this.confirmList)
		{
			this.confirmList.gameObject.SetActive(false);
		}
		base.transform.SetPosition2D(this.screenPos);
	}

	// Token: 0x06004185 RID: 16773 RVA: 0x00120578 File Offset: 0x0011E778
	private void Update()
	{
		if (this.state != SimpleShopMenu.State.ItemList)
		{
			return;
		}
		HeroActions inputActions = ManagerSingleton<InputHandler>.Instance.inputActions;
		Platform.MenuActions menuAction = Platform.Current.GetMenuAction(inputActions, false, false);
		if (menuAction == Platform.MenuActions.Submit)
		{
			this.OnSubmitPressed();
			return;
		}
		if (menuAction != Platform.MenuActions.Cancel)
		{
			if (InventoryPaneInput.IsInventoryButtonPressed(inputActions))
			{
				this.OnCancelPressed();
			}
			return;
		}
		this.OnCancelPressed();
	}

	// Token: 0x06004186 RID: 16774 RVA: 0x001205D0 File Offset: 0x0011E7D0
	public void SetStock(SimpleShopMenuOwner newOwner, List<ISimpleShopItem> newShopItems)
	{
		this.owner = newOwner;
		if (this.titleText)
		{
			this.titleText.text = this.owner.ShopTitle;
		}
		if (this.purchaseText)
		{
			this.purchaseText.text = this.owner.PurchaseText;
		}
		if (!this.templateItem)
		{
			Debug.LogError("No templateItem assigned!", this);
			return;
		}
		if (!this.itemList)
		{
			Debug.LogError("No itemList assigned!", this);
			return;
		}
		this.shopItems = newShopItems;
		this.activeItemCount = this.shopItems.Count;
		foreach (SimpleShopItemDisplay simpleShopItemDisplay in this.spawnedItemDisplays)
		{
			simpleShopItemDisplay.gameObject.SetActive(false);
		}
		for (int i = this.activeItemCount - this.spawnedItemDisplays.Count; i > 0; i--)
		{
			SimpleShopItemDisplay item = Object.Instantiate<SimpleShopItemDisplay>(this.templateItem, this.itemList);
			this.spawnedItemDisplays.Add(item);
		}
		for (int j = 0; j < this.activeItemCount; j++)
		{
			SimpleShopItemDisplay simpleShopItemDisplay2 = this.spawnedItemDisplays[j];
			simpleShopItemDisplay2.SetItem(newShopItems[j]);
			simpleShopItemDisplay2.gameObject.SetActive(true);
		}
	}

	// Token: 0x06004187 RID: 16775 RVA: 0x00120730 File Offset: 0x0011E930
	public void Activate()
	{
		this.pane.PaneStart();
	}

	// Token: 0x06004188 RID: 16776 RVA: 0x00120740 File Offset: 0x0011E940
	private void OnPaneStart()
	{
		this.state = SimpleShopMenu.State.ItemList;
		this.openTime = Time.time + 1f;
		if (this.confirmList)
		{
			this.confirmList.SetActive(false);
		}
		this.stateFaders[1].AlphaSelf = 0f;
		this.stateFaders[0].AlphaSelf = 1f;
		this.selectedIndex = -1;
		this.purchasedIndex = -1;
		this.didPurchase = false;
		this.ScrollTo(0, true);
		CurrencyCounter.Show(CurrencyType.Money, false);
	}

	// Token: 0x06004189 RID: 16777 RVA: 0x001207C6 File Offset: 0x0011E9C6
	private void OnPaneOpenedAnimEnd()
	{
		this.openTime = 0f;
	}

	// Token: 0x0600418A RID: 16778 RVA: 0x001207D3 File Offset: 0x0011E9D3
	private void OnPaneEnd()
	{
		this.owner.ClosedMenu(this.didPurchase, this.purchasedIndex);
		this.state = SimpleShopMenu.State.Inactive;
		this.owner = null;
		CurrencyCounter.Hide(CurrencyType.Money);
	}

	// Token: 0x0600418B RID: 16779 RVA: 0x00120800 File Offset: 0x0011EA00
	private void OnInputUp()
	{
		if (this.openTime > Time.time)
		{
			return;
		}
		if (this.state != SimpleShopMenu.State.ItemList)
		{
			return;
		}
		this.ScrollTo(this.selectedIndex - 1, false);
		if (this.upArrow)
		{
			this.upArrow.StartAnimation();
		}
	}

	// Token: 0x0600418C RID: 16780 RVA: 0x00120840 File Offset: 0x0011EA40
	private void OnInputDown()
	{
		if (this.openTime > Time.time)
		{
			return;
		}
		if (this.state != SimpleShopMenu.State.ItemList)
		{
			return;
		}
		this.ScrollTo(this.selectedIndex + 1, false);
		if (this.downArrow)
		{
			this.downArrow.StartAnimation();
		}
	}

	// Token: 0x0600418D RID: 16781 RVA: 0x00120880 File Offset: 0x0011EA80
	private void OnSubmitPressed()
	{
		if (this.openTime > Time.time)
		{
			return;
		}
		if (this.state != SimpleShopMenu.State.ItemList)
		{
			return;
		}
		ISimpleShopItem currentSelectedItem = this.GetCurrentSelectedItem();
		if (currentSelectedItem == null)
		{
			return;
		}
		if (PlayerData.instance.geo < currentSelectedItem.GetCost())
		{
			this.notEnoughSubmitSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
			if (this.selectorJitter)
			{
				this.selectorJitter.StartTimedJitter();
			}
			return;
		}
		this.submitSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
		if (currentSelectedItem.DelayPurchase())
		{
			this.state = SimpleShopMenu.State.Transitioning;
			this.didPurchase = true;
			this.purchasedIndex = this.selectedIndex;
			this.pane.PaneEnd();
			return;
		}
		if (this.confirmList)
		{
			this.confirmList.gameObject.SetActive(true);
			this.confirmList.SetActive(false);
			this.TransitionState(SimpleShopMenu.State.Confirm, false);
			return;
		}
		this.ConfirmYes();
	}

	// Token: 0x0600418E RID: 16782 RVA: 0x0012097C File Offset: 0x0011EB7C
	private void OnCancelPressed()
	{
		if (this.openTime > Time.time)
		{
			return;
		}
		if (this.state != SimpleShopMenu.State.ItemList)
		{
			return;
		}
		this.state = SimpleShopMenu.State.Inactive;
		this.cancelSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
		this.pane.PaneEnd();
	}

	// Token: 0x0600418F RID: 16783 RVA: 0x001209D0 File Offset: 0x0011EBD0
	public void ConfirmYes()
	{
		this.didPurchase = true;
		this.purchasedIndex = this.selectedIndex;
		ISimpleShopItem currentSelectedItem = this.GetCurrentSelectedItem();
		if (currentSelectedItem != null)
		{
			CurrencyManager.TakeCurrency(currentSelectedItem.GetCost(), CurrencyType.Money, true);
		}
		if (this.owner.ClosePaneOnPurchase)
		{
			this.pane.PaneEnd();
			return;
		}
		this.owner.PurchaseNoClose(this.purchasedIndex);
		this.purchasedIndex = -1;
		if (this.owner.HasStockLeft())
		{
			this.owner.RefreshStock();
			this.ScrollTo(0, true);
			this.ConfirmNo(true);
			return;
		}
		this.pane.PaneEnd();
	}

	// Token: 0x06004190 RID: 16784 RVA: 0x00120A6B File Offset: 0x0011EC6B
	public void ConfirmNo()
	{
		this.confirmList.SetActive(false);
		this.TransitionState(SimpleShopMenu.State.ItemList, false);
	}

	// Token: 0x06004191 RID: 16785 RVA: 0x00120A81 File Offset: 0x0011EC81
	public void ConfirmNo(bool waitFrame)
	{
		this.confirmList.SetActive(false);
		this.TransitionState(SimpleShopMenu.State.ItemList, waitFrame);
	}

	// Token: 0x06004192 RID: 16786 RVA: 0x00120A98 File Offset: 0x0011EC98
	private void ScrollTo(int index, bool isInstant = false)
	{
		SimpleShopMenu.<>c__DisplayClass53_0 CS$<>8__locals1 = new SimpleShopMenu.<>c__DisplayClass53_0();
		CS$<>8__locals1.<>4__this = this;
		int num = 0;
		if (index >= this.activeItemCount)
		{
			index = this.activeItemCount - 1;
			num = 1;
		}
		else if (index < 0)
		{
			index = 0;
			num = -1;
		}
		CS$<>8__locals1.previousIndex = this.selectedIndex;
		this.selectedIndex = index;
		CS$<>8__locals1.targetPosition = this.baseItemSpacing * (float)(index * -1);
		if (this.moveRoutine != null)
		{
			base.StopCoroutine(this.moveRoutine);
		}
		if (num != 0)
		{
			if (!isInstant)
			{
				this.UpdateItemPositions(-1, index, 1f);
				this.itemList.SetLocalPosition2D(CS$<>8__locals1.targetPosition);
				Vector2 moveTargetPosition = CS$<>8__locals1.targetPosition + this.failOffset * (float)num;
				this.moveRoutine = this.StartTimerRoutine(0f, this.failDuration, delegate(float time)
				{
					Vector2 position = Vector2.Lerp(CS$<>8__locals1.targetPosition, moveTargetPosition, CS$<>8__locals1.<>4__this.failCurve.Evaluate(time));
					CS$<>8__locals1.<>4__this.itemList.SetLocalPosition2D(position);
				}, null, null, false);
			}
			this.paneInput.CancelRepeat();
			return;
		}
		if (isInstant)
		{
			this.itemList.localPosition = CS$<>8__locals1.targetPosition;
			this.UpdateItemPositions(-1, index, 1f);
			return;
		}
		this.moveSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
		Vector2 initialPos = this.itemList.localPosition;
		this.moveRoutine = this.StartTimerRoutine(0f, this.moveToDuration, delegate(float time)
		{
			time = CS$<>8__locals1.<>4__this.moveToCurve.Evaluate(time);
			Vector2 position = Vector2.Lerp(initialPos, CS$<>8__locals1.targetPosition, time);
			CS$<>8__locals1.<>4__this.itemList.SetLocalPosition2D(position);
			CS$<>8__locals1.<>4__this.UpdateItemPositions(CS$<>8__locals1.previousIndex, CS$<>8__locals1.<>4__this.selectedIndex, time);
		}, null, null, false);
	}

	// Token: 0x06004193 RID: 16787 RVA: 0x00120C2C File Offset: 0x0011EE2C
	private void UpdateItemPositions(int previousItemIndex, int currentItemIndex, float blend)
	{
		for (int i = 0; i < this.activeItemCount; i++)
		{
			float b = 0f;
			float a = 0f;
			if (currentItemIndex >= 0)
			{
				if (i > currentItemIndex)
				{
					b = 1f;
				}
				else if (i < currentItemIndex)
				{
					b = -1f;
				}
			}
			if (previousItemIndex >= 0)
			{
				if (i > previousItemIndex)
				{
					a = 1f;
				}
				else if (i < previousItemIndex)
				{
					a = -1f;
				}
			}
			float d = Mathf.Lerp(a, b, blend);
			this.spawnedItemDisplays[i].transform.SetLocalPosition2D(this.baseItemSpacing * (float)i + d * this.selectedItemSpacing);
		}
	}

	// Token: 0x06004194 RID: 16788 RVA: 0x00120CD0 File Offset: 0x0011EED0
	private void TransitionState(SimpleShopMenu.State newState, bool waitFrame)
	{
		if (this.transitionStateRoutine != null)
		{
			Debug.LogError("Already transitioning");
			return;
		}
		if (this.state == SimpleShopMenu.State.Inactive || newState == SimpleShopMenu.State.Inactive)
		{
			Debug.LogError("Can't transition from or to inactive");
			return;
		}
		this.transitionStateRoutine = base.StartCoroutine(this.DoTransitionState(this.state, newState, waitFrame));
	}

	// Token: 0x06004195 RID: 16789 RVA: 0x00120D22 File Offset: 0x0011EF22
	private IEnumerator DoTransitionState(SimpleShopMenu.State previousState, SimpleShopMenu.State newState, bool waitFrame)
	{
		if (waitFrame)
		{
			yield return null;
		}
		Debug.LogFormat(this, "Transitioning from {0} to {1}", new object[]
		{
			previousState.ToString(),
			newState.ToString()
		});
		this.state = SimpleShopMenu.State.Transitioning;
		SimpleShopMenu.State state = newState;
		if (state != SimpleShopMenu.State.ItemList)
		{
			if (state == SimpleShopMenu.State.Confirm)
			{
				ISimpleShopItem currentSelectedItem = this.GetCurrentSelectedItem();
				if (this.confirmCostText)
				{
					this.confirmCostText.text = ((currentSelectedItem != null) ? currentSelectedItem.GetCost().ToString() : null);
				}
				if (this.confirmNameText)
				{
					this.confirmNameText.text = ((currentSelectedItem != null) ? currentSelectedItem.GetDisplayName() : null);
				}
			}
		}
		else
		{
			for (int i = 0; i < this.activeItemCount; i++)
			{
				this.spawnedItemDisplays[i].SetItem(this.shopItems[i]);
			}
		}
		NestedFadeGroupBase nestedFadeGroupBase = this.stateFaders[(int)previousState];
		NestedFadeGroupBase newStateFader = this.stateFaders[(int)newState];
		float fadeTime = this.stateFadeDuration * 0.5f;
		if (nestedFadeGroupBase)
		{
			yield return new WaitForSeconds(nestedFadeGroupBase.FadeTo(0f, fadeTime, null, false, null));
		}
		if (newStateFader)
		{
			yield return new WaitForSeconds(newStateFader.FadeTo(1f, fadeTime, null, false, null));
		}
		state = newState;
		if (state != SimpleShopMenu.State.ItemList)
		{
			if (state == SimpleShopMenu.State.Confirm && this.confirmList)
			{
				this.confirmList.SetActive(true);
			}
		}
		else if (this.confirmList)
		{
			this.confirmList.gameObject.SetActive(false);
			this.confirmList.SetActive(false);
		}
		this.state = newState;
		this.transitionStateRoutine = null;
		yield break;
	}

	// Token: 0x06004196 RID: 16790 RVA: 0x00120D46 File Offset: 0x0011EF46
	private ISimpleShopItem GetCurrentSelectedItem()
	{
		if (this.selectedIndex < 0)
		{
			return null;
		}
		List<ISimpleShopItem> list = this.shopItems;
		if (list == null)
		{
			return null;
		}
		return list[this.selectedIndex];
	}

	// Token: 0x04004303 RID: 17155
	[SerializeField]
	private Vector2 screenPos;

	// Token: 0x04004304 RID: 17156
	[Space]
	[SerializeField]
	private TMP_Text titleText;

	// Token: 0x04004305 RID: 17157
	[SerializeField]
	[ArrayForEnum(typeof(SimpleShopMenu.State))]
	private NestedFadeGroupBase[] stateFaders;

	// Token: 0x04004306 RID: 17158
	[SerializeField]
	private float stateFadeDuration;

	// Token: 0x04004307 RID: 17159
	[SerializeField]
	private TMP_Text purchaseText;

	// Token: 0x04004308 RID: 17160
	[Header("Item List")]
	[SerializeField]
	private Transform itemList;

	// Token: 0x04004309 RID: 17161
	[SerializeField]
	private SimpleShopItemDisplay templateItem;

	// Token: 0x0400430A RID: 17162
	[SerializeField]
	private Vector2 baseItemSpacing;

	// Token: 0x0400430B RID: 17163
	[SerializeField]
	private Vector2 selectedItemSpacing;

	// Token: 0x0400430C RID: 17164
	[Space]
	[SerializeField]
	private AnimationCurve moveToCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400430D RID: 17165
	[SerializeField]
	private float moveToDuration;

	// Token: 0x0400430E RID: 17166
	[SerializeField]
	private AnimationCurve failCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.5f, 1f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x0400430F RID: 17167
	[SerializeField]
	private Vector2 failOffset;

	// Token: 0x04004310 RID: 17168
	[SerializeField]
	private float failDuration;

	// Token: 0x04004311 RID: 17169
	[Space]
	[SerializeField]
	private BaseAnimator upArrow;

	// Token: 0x04004312 RID: 17170
	[SerializeField]
	private BaseAnimator downArrow;

	// Token: 0x04004313 RID: 17171
	[Space]
	[SerializeField]
	private AudioEvent moveSound;

	// Token: 0x04004314 RID: 17172
	[SerializeField]
	private AudioEvent notEnoughSubmitSound;

	// Token: 0x04004315 RID: 17173
	[SerializeField]
	private JitterSelfForTime selectorJitter;

	// Token: 0x04004316 RID: 17174
	[SerializeField]
	private AudioEvent submitSound;

	// Token: 0x04004317 RID: 17175
	[SerializeField]
	private AudioEvent cancelSound;

	// Token: 0x04004318 RID: 17176
	[Header("Confirm")]
	[SerializeField]
	private UISelectionList confirmList;

	// Token: 0x04004319 RID: 17177
	[SerializeField]
	private TMP_Text confirmNameText;

	// Token: 0x0400431A RID: 17178
	[SerializeField]
	private TMP_Text confirmCostText;

	// Token: 0x0400431B RID: 17179
	private SimpleShopMenuOwner owner;

	// Token: 0x0400431C RID: 17180
	private List<ISimpleShopItem> shopItems;

	// Token: 0x0400431D RID: 17181
	private readonly List<SimpleShopItemDisplay> spawnedItemDisplays = new List<SimpleShopItemDisplay>();

	// Token: 0x0400431E RID: 17182
	private int activeItemCount;

	// Token: 0x0400431F RID: 17183
	private SimpleShopMenu.State state;

	// Token: 0x04004320 RID: 17184
	private Coroutine transitionStateRoutine;

	// Token: 0x04004321 RID: 17185
	private int selectedIndex = -1;

	// Token: 0x04004322 RID: 17186
	private int purchasedIndex = -1;

	// Token: 0x04004323 RID: 17187
	private bool didPurchase;

	// Token: 0x04004324 RID: 17188
	private Coroutine moveRoutine;

	// Token: 0x04004325 RID: 17189
	private float openTime;

	// Token: 0x04004326 RID: 17190
	private InventoryPaneStandalone pane;

	// Token: 0x04004327 RID: 17191
	private InventoryPaneInput paneInput;

	// Token: 0x02001A11 RID: 6673
	private enum State
	{
		// Token: 0x04009856 RID: 38998
		Transitioning = -2,
		// Token: 0x04009857 RID: 38999
		Inactive,
		// Token: 0x04009858 RID: 39000
		ItemList,
		// Token: 0x04009859 RID: 39001
		Confirm
	}
}
