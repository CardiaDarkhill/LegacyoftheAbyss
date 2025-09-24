using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlobalEnums;
using GlobalSettings;
using TeamCherry.Localization;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using TMProOld;
using UnityEngine;

// Token: 0x020001C3 RID: 451
public class InventoryItemCollectable : InventoryItemUpdateable
{
	// Token: 0x170001D8 RID: 472
	// (get) Token: 0x06001180 RID: 4480 RVA: 0x00051924 File Offset: 0x0004FB24
	public override string DisplayName
	{
		get
		{
			if (!this.item)
			{
				return string.Empty;
			}
			return this.item.GetDisplayName(CollectableItem.ReadSource.Inventory);
		}
	}

	// Token: 0x170001D9 RID: 473
	// (get) Token: 0x06001181 RID: 4481 RVA: 0x00051948 File Offset: 0x0004FB48
	public override string Description
	{
		get
		{
			if (!this.item)
			{
				return string.Empty;
			}
			string description = this.item.GetDescription(CollectableItem.ReadSource.Inventory);
			string[] useResponseDescriptions = this.item.GetUseResponseDescriptions();
			List<FullQuestBase> list = null;
			foreach (FullQuestBase fullQuestBase in QuestManager.GetActiveQuests())
			{
				if (!fullQuestBase.InvItemAppendDesc.IsEmpty)
				{
					using (IEnumerator<FullQuestBase.QuestTarget> enumerator2 = fullQuestBase.Targets.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (!(enumerator2.Current.Counter != this.item))
							{
								if (list == null)
								{
									list = new List<FullQuestBase>();
								}
								list.Add(fullQuestBase);
								break;
							}
						}
					}
				}
			}
			DeliveryQuestItemStandalone deliveryQuestItemStandalone = this.item as DeliveryQuestItemStandalone;
			if (useResponseDescriptions.Length == 0 && list == null && (!deliveryQuestItemStandalone || deliveryQuestItemStandalone.InvItemAppendDesc.IsEmpty))
			{
				return description;
			}
			StringBuilder tempStringBuilder = global::Helper.GetTempStringBuilder(description);
			if (list != null)
			{
				foreach (FullQuestBase fullQuestBase2 in list)
				{
					tempStringBuilder.AppendLine();
					tempStringBuilder.AppendLine();
					tempStringBuilder.AppendLine(fullQuestBase2.InvItemAppendDesc);
				}
			}
			if (deliveryQuestItemStandalone && !deliveryQuestItemStandalone.InvItemAppendDesc.IsEmpty)
			{
				tempStringBuilder.AppendLine();
				tempStringBuilder.AppendLine();
				tempStringBuilder.AppendLine(deliveryQuestItemStandalone.InvItemAppendDesc);
			}
			foreach (string value in useResponseDescriptions)
			{
				tempStringBuilder.AppendLine();
				tempStringBuilder.AppendLine();
				tempStringBuilder.AppendLine(value);
			}
			return tempStringBuilder.ToString();
		}
	}

	// Token: 0x170001DA RID: 474
	// (get) Token: 0x06001182 RID: 4482 RVA: 0x00051B44 File Offset: 0x0004FD44
	public Transform IconTransform
	{
		get
		{
			if (!this.spriteRenderer)
			{
				return base.transform;
			}
			return this.spriteRenderer.transform;
		}
	}

	// Token: 0x170001DB RID: 475
	// (get) Token: 0x06001183 RID: 4483 RVA: 0x00051B65 File Offset: 0x0004FD65
	// (set) Token: 0x06001184 RID: 4484 RVA: 0x00051B6D File Offset: 0x0004FD6D
	public CollectableItem Item
	{
		get
		{
			return this.item;
		}
		set
		{
			this.item = value;
			this.UpdateItemDisplay();
		}
	}

	// Token: 0x170001DC RID: 476
	// (get) Token: 0x06001185 RID: 4485 RVA: 0x00051B7C File Offset: 0x0004FD7C
	// (set) Token: 0x06001186 RID: 4486 RVA: 0x00051B98 File Offset: 0x0004FD98
	protected override bool IsSeen
	{
		get
		{
			return !this.item || this.item.IsSeen;
		}
		set
		{
			if (!this.item)
			{
				return;
			}
			this.item.IsSeen = value;
		}
	}

	// Token: 0x06001187 RID: 4487 RVA: 0x00051BB4 File Offset: 0x0004FDB4
	protected override void Awake()
	{
		base.Awake();
		Transform iconTransform = this.IconTransform;
		this.initialPosition = iconTransform.localPosition;
		this.initialScale = iconTransform.localScale;
		this.manager = base.GetComponentInParent<InventoryItemCollectableManager>();
		if (this.Pane)
		{
			this.Pane.OnPaneStart += delegate()
			{
				if (this.currentCustomDisplay)
				{
					this.currentCustomDisplay.OnPaneStart();
				}
				this.ResetFadeUp();
			};
			this.Pane.OnPrePaneEnd += delegate()
			{
				if (this.currentCustomDisplay)
				{
					this.currentCustomDisplay.OnPrePaneEnd();
				}
			};
			this.Pane.OnPaneEnd += delegate()
			{
				if (this.currentCustomDisplay)
				{
					this.currentCustomDisplay.OnPaneEnd();
				}
				if (this.consumeEffect)
				{
					this.consumeEffect.gameObject.SetActive(false);
				}
				if (this.spawnedExtraConsumeEffect)
				{
					this.spawnedExtraConsumeEffect.Recycle();
					this.spawnedExtraConsumeEffect = null;
				}
			};
		}
		if (this.consumeEffect)
		{
			this.consumeEffect.gameObject.SetActive(false);
		}
		this.paneList = base.GetComponentInParent<InventoryPaneList>();
		this.extraDesc = base.GetComponent<InventoryItemExtraDescription>();
		if (this.extraDesc)
		{
			this.extraDesc.ActivatedDesc += delegate(GameObject obj)
			{
				if (this.item)
				{
					this.item.SetupExtraDescription(obj);
				}
			};
		}
		if (this.consumePrompt)
		{
			this.consumePrompt.SetActive(false);
			this.consumePromptInitialScale = this.consumePrompt.transform.localScale;
		}
	}

	// Token: 0x06001188 RID: 4488 RVA: 0x00051CCD File Offset: 0x0004FECD
	protected override void OnDisable()
	{
		base.OnDisable();
		this.StopConsumeRumble();
	}

	// Token: 0x06001189 RID: 4489 RVA: 0x00051CDC File Offset: 0x0004FEDC
	private void Update()
	{
		if (this.consumeFadeUpDelay > 0f)
		{
			this.consumeFadeUpDelay -= Time.unscaledDeltaTime;
			if (this.consumeFadeUpDelay <= 0f)
			{
				if (this.group)
				{
					this.group.FadeTo(1f, this.breakFadeUpTime, null, true, null);
				}
				this.IconTransform.ScaleTo(this, this.initialScale, this.breakFadeUpTime, 0f, false, true, null);
			}
		}
	}

	// Token: 0x0600118A RID: 4490 RVA: 0x00051D5C File Offset: 0x0004FF5C
	private void UpdateItemDisplay()
	{
		base.gameObject.name = this.item.name;
		if (this.spriteRenderer)
		{
			this.spriteRenderer.sprite = (this.item.CustomInventoryDisplay ? null : this.item.GetIcon(CollectableItem.ReadSource.Inventory));
		}
		if (this.consumePrompt != null && (!this.item.IsConsumable() || !this.item.CanConsumeRightNow()))
		{
			this.consumePrompt.SetActive(false);
		}
		if (this.amountText)
		{
			this.amountText.text = ((this.forceShowAmount || this.item.DisplayAmount) ? this.item.CollectedAmount.ToString() : string.Empty);
			this.amountText.color = (this.item.IsAtMax() ? this.maxAmountTextColor : this.regularAmountTextColor);
		}
		if (this.isSelected)
		{
			this.HidePromptData(true);
			this.DisplayPromptData();
		}
		if (this.currentCustomDisplay)
		{
			if (this.currentCustomDisplay.Owner == this)
			{
				this.currentCustomDisplay.gameObject.SetActive(false);
			}
			this.currentCustomDisplay = null;
		}
		if (this.item.CustomInventoryDisplay)
		{
			if (!InventoryItemCollectable._spawnedCustomDisplays.ContainsKey(this.item) || InventoryItemCollectable._spawnedCustomDisplays[this.item] == null)
			{
				CustomInventoryItemCollectableDisplay customInventoryItemCollectableDisplay = Object.Instantiate<CustomInventoryItemCollectableDisplay>(this.item.CustomInventoryDisplay, this.IconTransform);
				customInventoryItemCollectableDisplay.OnDestroyed += InventoryItemCollectable.OnInventoryItemDestroyed;
				customInventoryItemCollectableDisplay.transform.Reset();
				InventoryItemCollectable._spawnedCustomDisplays[this.item] = customInventoryItemCollectableDisplay;
				this.currentCustomDisplay = customInventoryItemCollectableDisplay;
			}
			else
			{
				this.currentCustomDisplay = InventoryItemCollectable._spawnedCustomDisplays[this.item];
				this.currentCustomDisplay.transform.SetParentReset(this.IconTransform);
				this.currentCustomDisplay.gameObject.SetActive(true);
			}
			this.currentCustomDisplay.transform.localScale = this.item.CustomInventoryDisplay.transform.localScale;
			this.currentCustomDisplay.Owner = this;
		}
		if (this.extraDesc)
		{
			this.extraDesc.ExtraDescPrefab = this.item.ExtraDescriptionSection;
		}
		this.UpdateDisplay();
	}

	// Token: 0x0600118B RID: 4491 RVA: 0x00051FCC File Offset: 0x000501CC
	private static void OnInventoryItemDestroyed(CustomInventoryItemCollectableDisplay display)
	{
		display.OnDestroyed -= InventoryItemCollectable.OnInventoryItemDestroyed;
		IEnumerable<KeyValuePair<CollectableItem, CustomInventoryItemCollectableDisplay>> spawnedCustomDisplays = InventoryItemCollectable._spawnedCustomDisplays;
		Func<KeyValuePair<CollectableItem, CustomInventoryItemCollectableDisplay>, bool> <>9__0;
		Func<KeyValuePair<CollectableItem, CustomInventoryItemCollectableDisplay>, bool> predicate;
		if ((predicate = <>9__0) == null)
		{
			predicate = (<>9__0 = ((KeyValuePair<CollectableItem, CustomInventoryItemCollectableDisplay> kvp) => kvp.Value == display));
		}
		foreach (KeyValuePair<CollectableItem, CustomInventoryItemCollectableDisplay> keyValuePair in spawnedCustomDisplays.Where(predicate).ToArray<KeyValuePair<CollectableItem, CustomInventoryItemCollectableDisplay>>())
		{
			InventoryItemCollectable._spawnedCustomDisplays.Remove(keyValuePair.Key);
		}
	}

	// Token: 0x0600118C RID: 4492 RVA: 0x00052058 File Offset: 0x00050258
	private void DisplayPromptData()
	{
		if (!this.buttonPromptDisplay)
		{
			return;
		}
		InventoryItemButtonPromptData[] array = this.item.GetButtonPromptData();
		if ((array == null || array.Length == 0) && this.item.CanConsumeRightNow())
		{
			array = new InventoryItemButtonPromptData[]
			{
				new InventoryItemButtonPromptData
				{
					Action = HeroActionButton.JUMP,
					UseText = this.consumeItemUsePrompt,
					ResponseText = ((!this.item.UseResponseTextOverride.IsEmpty) ? this.item.UseResponseTextOverride : this.consumeItemResponse),
					IsMenuButton = true
				}
			};
		}
		if (array != null)
		{
			bool forceDisabled = this.item.IsConsumable() && !this.item.CanConsumeRightNow();
			foreach (InventoryItemButtonPromptData promptData in array)
			{
				this.buttonPromptDisplay.Append(promptData, forceDisabled, 0);
			}
			return;
		}
		this.buttonPromptDisplay.Clear();
	}

	// Token: 0x0600118D RID: 4493 RVA: 0x00052154 File Offset: 0x00050354
	private void ShowConsumePrompt()
	{
		if (!this.consumePrompt)
		{
			return;
		}
		if (!this.consumePromptIsVisible)
		{
			this.consumePrompt.SetActive(this.item.IsConsumable() && this.item.CanConsumeRightNow());
			this.consumePrompt.transform.localScale = new Vector3(0f, 0f, 1f);
			this.consumePromptIsVisible = true;
		}
		this.consumePrompt.transform.ScaleTo(this, this.consumePromptInitialScale, 0.1f, 0.1f, false, true, null);
		this.consumePrompt.SendMessage("StopJitter", SendMessageOptions.DontRequireReceiver);
	}

	// Token: 0x0600118E RID: 4494 RVA: 0x00052200 File Offset: 0x00050400
	private void HideConsumePrompt(bool isInstant)
	{
		if (!this.consumePrompt)
		{
			return;
		}
		if (!isInstant && !this.consumePromptIsVisible)
		{
			return;
		}
		if (!isInstant && this.manager.NextSelected == this)
		{
			return;
		}
		if (isInstant)
		{
			this.consumePrompt.SetActive(false);
		}
		else
		{
			this.consumePrompt.transform.ScaleTo(this, new Vector3(0f, 0f, 1f), 0.05f, 0f, false, true, delegate
			{
				this.consumePrompt.SetActive(false);
			});
		}
		this.consumePromptIsVisible = false;
	}

	// Token: 0x0600118F RID: 4495 RVA: 0x00052293 File Offset: 0x00050493
	private void HidePromptData(bool isInstant)
	{
		this.HideConsumePrompt(isInstant);
		if (!this.buttonPromptDisplay)
		{
			return;
		}
		this.buttonPromptDisplay.Clear();
	}

	// Token: 0x06001190 RID: 4496 RVA: 0x000522B5 File Offset: 0x000504B5
	public override void Select(InventoryItemManager.SelectionDirection? direction)
	{
		base.Select(direction);
		this.isSelected = true;
		this.DisplayPromptData();
		this.ShowConsumePrompt();
		if (this.currentCustomDisplay)
		{
			this.currentCustomDisplay.OnSelect();
		}
	}

	// Token: 0x06001191 RID: 4497 RVA: 0x000522E9 File Offset: 0x000504E9
	public override void Deselect()
	{
		base.Deselect();
		this.isSelected = false;
		this.HidePromptData(false);
		if (this.currentCustomDisplay)
		{
			this.currentCustomDisplay.OnDeselect();
		}
	}

	// Token: 0x06001192 RID: 4498 RVA: 0x00052318 File Offset: 0x00050518
	public override bool Submit()
	{
		this.isConsumePressed = true;
		if (!this.item.IsConsumable())
		{
			return false;
		}
		if (this.item.CollectedAmount <= 0)
		{
			Debug.LogError("No items left to consume!");
			return false;
		}
		if (this.manager.ShowingMemoryUseMsg)
		{
			this.manager.HideMemoryUseMsg(false);
			return false;
		}
		if (!this.item.CanConsumeRightNow())
		{
			this.failedAnimator.Play(this.failedAnimId);
			this.failedAudioTable.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, false, 1f, null);
			if (this.currentCustomDisplay)
			{
				this.currentCustomDisplay.OnConsumeBlocked();
			}
			if (GameManager.instance.IsMemoryScene())
			{
				this.manager.ShowMemoryUseMsg();
			}
			return true;
		}
		this.ResetConsume();
		this.consumeRoutine = base.StartCoroutine(this.ConsumeRoutine());
		return true;
	}

	// Token: 0x06001193 RID: 4499 RVA: 0x000523FB File Offset: 0x000505FB
	public override bool SubmitReleased()
	{
		this.isConsumePressed = false;
		this.ResetConsume();
		return true;
	}

	// Token: 0x06001194 RID: 4500 RVA: 0x0005240B File Offset: 0x0005060B
	public override bool Cancel()
	{
		if (this.manager.ShowingMemoryUseMsg)
		{
			this.manager.HideMemoryUseMsg(false);
		}
		return base.Cancel();
	}

	// Token: 0x06001195 RID: 4501 RVA: 0x0005242C File Offset: 0x0005062C
	private void ResetConsume()
	{
		if (this.consumeRoutine == null)
		{
			return;
		}
		base.StopCoroutine(this.consumeRoutine);
		this.consumeRoutine = null;
		if (this.isPendingCloseUsage)
		{
			this.isPendingCloseUsage = false;
			if (this.item != null)
			{
				ManagerSingleton<HeroChargeEffects>.Instance.DoUseBenchItem(this.item);
			}
		}
		this.EndConsume();
	}

	// Token: 0x06001196 RID: 4502 RVA: 0x00052488 File Offset: 0x00050688
	private void EndConsume()
	{
		this.ConsumeBlock(false);
		this.IconTransform.localPosition = this.initialPosition;
		if (this.spawnedUsePlayer)
		{
			this.spawnedUsePlayer.Stop();
			this.spawnedUsePlayer = null;
		}
		if (this.currentCustomDisplay)
		{
			this.currentCustomDisplay.OnConsumeEnd();
		}
		this.StopConsumeRumble();
	}

	// Token: 0x06001197 RID: 4503 RVA: 0x000524EA File Offset: 0x000506EA
	private IEnumerator ConsumeRoutine()
	{
		Transform iconTransform = this.IconTransform;
		bool closeInventoryConsume = this.item.ConsumeClosesInventory(true);
		this.ConsumeBlock(true);
		while (this.isConsumePressed)
		{
			if (this.manager)
			{
				this.manager.IsActionsBlocked = false;
			}
			this.spawnedUsePlayer = this.item.UseSounds.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, delegate(AudioSource source)
			{
				if (this.spawnedUsePlayer == source)
				{
					this.spawnedUsePlayer = null;
				}
			});
			float jitterMagnitude;
			if (this.currentCustomDisplay)
			{
				this.currentCustomDisplay.OnConsumeStart();
				jitterMagnitude = this.currentCustomDisplay.JitterMagnitudeMultiplier;
			}
			else
			{
				jitterMagnitude = 1f;
			}
			WaitForSecondsRealtime wait = new WaitForSecondsRealtime(0.016666668f);
			float holdDuration = this.item.ConsumeClosesInventory(false) ? 1.5f : 0.5f;
			double beforeWaitTime;
			for (float elapsed = 0f; elapsed < holdDuration; elapsed += (float)(Time.unscaledTimeAsDouble - beforeWaitTime))
			{
				this.SetConsumeShakeAmount(elapsed / holdDuration, jitterMagnitude);
				beforeWaitTime = Time.unscaledTimeAsDouble;
				yield return wait;
			}
			if (this.manager)
			{
				this.manager.IsActionsBlocked = true;
			}
			if (this.item.AlwaysPlayInstantUse && this.item.InstantUseSounds.HasClips())
			{
				this.spawnedUsePlayer.Stop();
				this.spawnedUsePlayer = null;
				this.item.InstantUseSounds.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
			}
			else
			{
				this.spawnedUsePlayer = null;
			}
			this.StopConsumeRumble();
			this.PlayConsumeFinalShake();
			if (this.currentCustomDisplay)
			{
				this.currentCustomDisplay.OnConsumeComplete();
			}
			iconTransform.localPosition = this.initialPosition;
			GameObject extraConsumeEffectPrefab = this.item.ExtraUseEffect;
			if (!closeInventoryConsume)
			{
				this.item.ConsumeItemResponse();
			}
			else
			{
				this.isPendingCloseUsage = true;
			}
			if (this.item.TakeItemOnConsume)
			{
				this.item.Take(1, false);
			}
			this.UpdateItemDisplay();
			if (this.consumeEffect)
			{
				InventoryItemCollectable.<>c__DisplayClass71_0 CS$<>8__locals1 = new InventoryItemCollectable.<>c__DisplayClass71_0();
				CS$<>8__locals1.<>4__this = this;
				this.consumeEffect.gameObject.SetActive(false);
				this.consumeEffect.gameObject.SetActive(true);
				CS$<>8__locals1.hitTrigger = false;
				CS$<>8__locals1.temp = null;
				CS$<>8__locals1.temp = delegate()
				{
					CS$<>8__locals1.hitTrigger = true;
					CS$<>8__locals1.<>4__this.consumeEffect.EventFired -= CS$<>8__locals1.temp;
				};
				this.consumeEffect.EventFired += CS$<>8__locals1.temp;
				while (!CS$<>8__locals1.hitTrigger)
				{
					yield return null;
				}
				CS$<>8__locals1 = null;
			}
			if (extraConsumeEffectPrefab)
			{
				this.spawnedExtraConsumeEffect = extraConsumeEffectPrefab.Spawn(base.transform.parent);
				Vector3 position = base.transform.position;
				position.z -= 0.0001f;
				this.spawnedExtraConsumeEffect.transform.position = position;
			}
			this.consumeFadeUpDelay = 0f;
			if (this.group)
			{
				this.group.AlphaSelf = 0f;
			}
			this.IconTransform.ScaleTo(this, new Vector3(this.breakFadeUpScale, this.breakFadeUpScale, this.initialScale.z), 0f, 0f, false, true, null);
			if (this.item.CollectedAmount <= 0 || closeInventoryConsume)
			{
				if (this.group)
				{
					if (closeInventoryConsume)
					{
						yield return new WaitForSecondsRealtime(0.5f);
					}
					else
					{
						yield return new WaitForSecondsRealtime(0.5f);
						this.ResetFadeUp();
					}
				}
				else
				{
					yield return new WaitForSecondsRealtime(0.5f);
				}
				this.HidePromptData(true);
				if (closeInventoryConsume)
				{
					this.ConsumeBlock(false);
					EventRegister.SendEvent(EventRegisterEvents.InventoryCancel, null);
					ManagerSingleton<HeroChargeEffects>.Instance.DoUseBenchItem(this.item);
					this.isPendingCloseUsage = false;
					yield break;
				}
				this.isPendingCloseUsage = false;
				if (this.manager)
				{
					this.manager.SetSelected(null, false);
					int gridSectionIndex = base.GridSectionIndex;
					int gridItemIndex = base.GridItemIndex;
					this.manager.UpdateList();
					if (base.Grid)
					{
						InventoryItemSelectable itemOrFallback = base.Grid.GetItemOrFallback(gridSectionIndex, gridItemIndex);
						this.manager.SetSelected(itemOrFallback ? itemOrFallback : this, null, false);
					}
					else
					{
						Debug.LogError("Grid was null!", this);
					}
				}
				this.ConsumeBlock(false);
				this.HidePromptData(true);
				yield break;
			}
			else
			{
				this.consumeFadeUpDelay = 0.3f - this.breakFadeUpTime;
				yield return new WaitForSecondsRealtime(0.3f);
				if (this.item.PreventUseChaining || this.item.IsConsumeAtMax())
				{
					break;
				}
				HeroActions inputActions = ManagerSingleton<InputHandler>.Instance.inputActions;
				if (Platform.Current.GetMenuAction(inputActions, false, true) != Platform.MenuActions.Submit)
				{
					this.isConsumePressed = false;
				}
				wait = null;
				extraConsumeEffectPrefab = null;
			}
		}
		this.EndConsume();
		this.consumeRoutine = null;
		yield break;
	}

	// Token: 0x06001198 RID: 4504 RVA: 0x000524FC File Offset: 0x000506FC
	private void ConsumeBlock(bool value)
	{
		if (!value && this.manager)
		{
			this.manager.IsActionsBlocked = false;
		}
		if (this.paneList)
		{
			this.paneList.CloseBlocked = value;
		}
		if (this.item.IsConsumable() && !value)
		{
			this.ShowConsumePrompt();
			return;
		}
		this.consumePrompt.transform.ScaleTo(this, new Vector3(this.consumePromptInitialScale.x * 0.8f, this.consumePromptInitialScale.y * 0.8f, this.consumePromptInitialScale.z), 0.05f, 0f, false, true, null);
		this.consumePrompt.SendMessage("StartJitter", SendMessageOptions.DontRequireReceiver);
	}

	// Token: 0x06001199 RID: 4505 RVA: 0x000525B8 File Offset: 0x000507B8
	private void ResetFadeUp()
	{
		this.consumeFadeUpDelay = 0f;
		if (this.group)
		{
			this.group.AlphaSelf = 1f;
		}
		this.IconTransform.ScaleTo(this, this.initialScale, 0f, 0f, false, false, null);
	}

	// Token: 0x0600119A RID: 4506 RVA: 0x0005260C File Offset: 0x0005080C
	public void SetConsumeShakeAmount(float t, float jitterMagnitude)
	{
		if (t <= Mathf.Epsilon)
		{
			this.IconTransform.localPosition = this.initialPosition;
		}
		else
		{
			this.IconTransform.localPosition = this.initialPosition + Random.insideUnitCircle * (this.consumeShakeMagnitude.GetLerpedValue(t) * jitterMagnitude);
		}
		this.UpdateConsumeRumble(t);
	}

	// Token: 0x0600119B RID: 4507 RVA: 0x0005266E File Offset: 0x0005086E
	public void PlayConsumeEffect()
	{
		if (!this.consumeEffect)
		{
			return;
		}
		this.consumeEffect.gameObject.SetActive(false);
		this.consumeEffect.gameObject.SetActive(true);
		this.StopConsumeRumble();
		this.PlayConsumeFinalShake();
	}

	// Token: 0x0600119C RID: 4508 RVA: 0x000526AC File Offset: 0x000508AC
	private void UpdateConsumeRumble(float strength)
	{
		if (this.consumeRumbleEmission == null)
		{
			this.consumeRumbleEmission = VibrationManager.PlayVibrationClipOneShot(this.consumeRumble, null, true, "", true);
		}
		VibrationEmission vibrationEmission = this.consumeRumbleEmission;
		if (vibrationEmission == null)
		{
			return;
		}
		vibrationEmission.SetStrength(strength);
	}

	// Token: 0x0600119D RID: 4509 RVA: 0x000526F8 File Offset: 0x000508F8
	public void StopConsumeRumble()
	{
		VibrationEmission vibrationEmission = this.consumeRumbleEmission;
		if (vibrationEmission != null)
		{
			vibrationEmission.Stop();
		}
		this.consumeRumbleEmission = null;
	}

	// Token: 0x0600119E RID: 4510 RVA: 0x00052714 File Offset: 0x00050914
	private void PlayConsumeFinalShake()
	{
		VibrationManager.PlayVibrationClipOneShot(this.consumeFinalShake, null, false, "", true);
	}

	// Token: 0x0400106B RID: 4203
	private const float CONSUME_HOLD_DURATION = 0.5f;

	// Token: 0x0400106C RID: 4204
	private const float CONSUME_HOLD_END_PAUSE = 0.3f;

	// Token: 0x0400106D RID: 4205
	private const float CONSUME_LAST_PAUSE = 0.5f;

	// Token: 0x0400106E RID: 4206
	private const float CLOSE_CONSUME_HOLD_DURATION = 1.5f;

	// Token: 0x0400106F RID: 4207
	private const float CLOSE_CONSUME_PAUSE = 0.5f;

	// Token: 0x04001070 RID: 4208
	[Space]
	[SerializeField]
	private NestedFadeGroup group;

	// Token: 0x04001071 RID: 4209
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	// Token: 0x04001072 RID: 4210
	[SerializeField]
	private TextMeshPro amountText;

	// Token: 0x04001073 RID: 4211
	[SerializeField]
	private Color regularAmountTextColor;

	// Token: 0x04001074 RID: 4212
	[SerializeField]
	private Color maxAmountTextColor;

	// Token: 0x04001075 RID: 4213
	[Space]
	[SerializeField]
	private CollectableItem item;

	// Token: 0x04001076 RID: 4214
	[SerializeField]
	private bool forceShowAmount;

	// Token: 0x04001077 RID: 4215
	[Space]
	[SerializeField]
	private float breakFadeUpTime;

	// Token: 0x04001078 RID: 4216
	[SerializeField]
	private float breakFadeUpScale = 0.5f;

	// Token: 0x04001079 RID: 4217
	[SerializeField]
	private CaptureAnimationEvent consumeEffect;

	// Token: 0x0400107A RID: 4218
	[SerializeField]
	private AudioSource audioPlayerPrefab;

	// Token: 0x0400107B RID: 4219
	[SerializeField]
	private GameObject consumePrompt;

	// Token: 0x0400107C RID: 4220
	[Space]
	[SerializeField]
	private InventoryItemButtonPromptDisplayList buttonPromptDisplay;

	// Token: 0x0400107D RID: 4221
	[SerializeField]
	private LocalisedString consumeItemUsePrompt;

	// Token: 0x0400107E RID: 4222
	[SerializeField]
	private LocalisedString consumeItemResponse;

	// Token: 0x0400107F RID: 4223
	[SerializeField]
	private MinMaxFloat consumeShakeMagnitude;

	// Token: 0x04001080 RID: 4224
	[SerializeField]
	private RandomAudioClipTable failedAudioTable;

	// Token: 0x04001081 RID: 4225
	[SerializeField]
	private Animator failedAnimator;

	// Token: 0x04001082 RID: 4226
	[Space]
	[SerializeField]
	private VibrationDataAsset consumeRumble;

	// Token: 0x04001083 RID: 4227
	[SerializeField]
	private VibrationDataAsset consumeFinalShake;

	// Token: 0x04001084 RID: 4228
	private static readonly Dictionary<CollectableItem, CustomInventoryItemCollectableDisplay> _spawnedCustomDisplays = new Dictionary<CollectableItem, CustomInventoryItemCollectableDisplay>();

	// Token: 0x04001085 RID: 4229
	private CustomInventoryItemCollectableDisplay currentCustomDisplay;

	// Token: 0x04001086 RID: 4230
	private Vector3 initialPosition;

	// Token: 0x04001087 RID: 4231
	private Vector3 initialScale;

	// Token: 0x04001088 RID: 4232
	private bool consumePromptIsVisible;

	// Token: 0x04001089 RID: 4233
	private Vector3 consumePromptInitialScale;

	// Token: 0x0400108A RID: 4234
	private readonly int failedAnimId = Animator.StringToHash("Failed");

	// Token: 0x0400108B RID: 4235
	private Coroutine consumeRoutine;

	// Token: 0x0400108C RID: 4236
	private bool isSelected;

	// Token: 0x0400108D RID: 4237
	private bool isConsumePressed;

	// Token: 0x0400108E RID: 4238
	private bool isPendingCloseUsage;

	// Token: 0x0400108F RID: 4239
	private float consumeFadeUpDelay;

	// Token: 0x04001090 RID: 4240
	private AudioSource spawnedUsePlayer;

	// Token: 0x04001091 RID: 4241
	private GameObject spawnedExtraConsumeEffect;

	// Token: 0x04001092 RID: 4242
	private InventoryItemExtraDescription extraDesc;

	// Token: 0x04001093 RID: 4243
	private InventoryItemCollectableManager manager;

	// Token: 0x04001094 RID: 4244
	private InventoryPaneList paneList;

	// Token: 0x04001095 RID: 4245
	private VibrationEmission consumeRumbleEmission;
}
