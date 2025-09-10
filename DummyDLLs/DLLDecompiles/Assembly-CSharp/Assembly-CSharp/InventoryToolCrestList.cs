using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using TMProOld;
using UnityEngine;

// Token: 0x020006B9 RID: 1721
[DefaultExecutionOrder(0)]
public class InventoryToolCrestList : InventoryItemSelectableDirectional
{
	// Token: 0x17000720 RID: 1824
	// (get) Token: 0x06003E1C RID: 15900 RVA: 0x001112DC File Offset: 0x0010F4DC
	// (set) Token: 0x06003E1D RID: 15901 RVA: 0x001112E4 File Offset: 0x0010F4E4
	public bool IsSwitchingCrests { get; private set; }

	// Token: 0x17000721 RID: 1825
	// (get) Token: 0x06003E1E RID: 15902 RVA: 0x001112ED File Offset: 0x0010F4ED
	// (set) Token: 0x06003E1F RID: 15903 RVA: 0x001112F5 File Offset: 0x0010F4F5
	public bool IsBlocked { get; set; }

	// Token: 0x17000722 RID: 1826
	// (get) Token: 0x06003E20 RID: 15904 RVA: 0x001112FE File Offset: 0x0010F4FE
	// (set) Token: 0x06003E21 RID: 15905 RVA: 0x00111306 File Offset: 0x0010F506
	public bool IsSetupComplete { get; private set; }

	// Token: 0x17000723 RID: 1827
	// (get) Token: 0x06003E22 RID: 15906 RVA: 0x0011130F File Offset: 0x0010F50F
	// (set) Token: 0x06003E23 RID: 15907 RVA: 0x00111317 File Offset: 0x0010F517
	public InventoryToolCrest CurrentCrest { get; private set; }

	// Token: 0x17000724 RID: 1828
	// (get) Token: 0x06003E24 RID: 15908 RVA: 0x00111320 File Offset: 0x0010F520
	public Vector2 HomePosition
	{
		get
		{
			if (this.nudgeIfActive && this.nudgeIfActive.activeInHierarchy)
			{
				return this.initialPosition + this.nudgeOffset;
			}
			return this.initialPosition;
		}
	}

	// Token: 0x06003E25 RID: 15909 RVA: 0x00111354 File Offset: 0x0010F554
	protected override void Awake()
	{
		base.Awake();
		this.manager = base.GetComponentInParent<InventoryItemToolManager>();
		this.pane = base.GetComponentInParent<InventoryPaneBase>();
		this.paneInput = base.GetComponentInParent<InventoryPaneInput>();
		if (this.pane)
		{
			this.pane.OnPaneEnd += delegate()
			{
				this.queuedPaneEnded = true;
				this.IsSwitchingCrests = false;
				this.isWaitingForApply = false;
			};
			this.pane.OnPaneStart += this.Setup;
			this.pane.OnInputLeft += delegate()
			{
				this.SwitchSelectedCrest(-1);
			};
			this.pane.OnInputRight += delegate()
			{
				this.SwitchSelectedCrest(1);
			};
		}
		this.initialPosition = base.transform.localPosition;
		this.inputHandler = GameManager.instance.inputHandler;
		this.Setup();
		if (this.changeCrestButton)
		{
			InventoryItemSelectableButtonEvent inventoryItemSelectableButtonEvent = this.changeCrestButton;
			inventoryItemSelectableButtonEvent.ButtonActivated = (Action)Delegate.Combine(inventoryItemSelectableButtonEvent.ButtonActivated, new Action(delegate()
			{
				this.wasChangeCrestButtonPressed = true;
			}));
		}
	}

	// Token: 0x06003E26 RID: 15910 RVA: 0x00111454 File Offset: 0x0010F654
	private void Update()
	{
		if (this.IsBlocked || !this.pane || !this.manager || this.manager.IsActionsBlocked || this.isWaitingForApply || !this.paneInput || !this.paneInput.enabled)
		{
			return;
		}
		HeroActions inputActions = this.inputHandler.inputActions;
		Platform.MenuActions menuAction = Platform.Current.GetMenuAction(inputActions, false, false);
		InventoryItemToolManager.EquipStates equipState = this.manager.EquipState;
		if (equipState != InventoryItemToolManager.EquipStates.None)
		{
			if (equipState == InventoryItemToolManager.EquipStates.SwitchCrest)
			{
				if (((menuAction == Platform.MenuActions.Cancel || menuAction == Platform.MenuActions.Submit || InventoryPaneInput.IsInventoryButtonPressed(inputActions)) && this.pane.IsPaneActive) || this.queuedPaneEnded)
				{
					if (this.queuedPaneEnded || menuAction == Platform.MenuActions.Cancel)
					{
						if (this.manager.EndSwitchingCrest())
						{
							this.StopSwitchingCrests(false);
						}
					}
					else if (this.CanApplyCrest())
					{
						this.isWaitingForApply = true;
						if (this.CurrentCrest != this.previousEquippedCrest)
						{
							this.CurrentCrest.DoEquip(new Action(this.ApplyCurrentCrest));
						}
						else
						{
							this.ApplyCurrentCrest();
							this.changeCrestExitAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
						}
					}
				}
			}
		}
		else if ((menuAction == Platform.MenuActions.Super || this.wasChangeCrestButtonPressed) && this.pane.IsPaneActive && this.CanChangeCrests())
		{
			if (this.CurrentCrest && this.CurrentCrest.IsHidden)
			{
				if (this.changeCrestIconAnimator)
				{
					this.changeCrestIconAnimator.SetTrigger(InventoryToolCrestList._failed);
				}
				if (this.manager.ShowingCursedMsg)
				{
					this.manager.HideCursedMsg(false);
				}
				else
				{
					this.manager.ShowCursedMsg(true, ToolItemType.Red);
				}
			}
			else if (this.manager.BeginSwitchingCrest())
			{
				this.StartSwitchingCrests();
			}
			else if (this.CanApplyCrest() && this.manager.EndSwitchingCrest())
			{
				this.StopSwitchingCrests(true);
			}
		}
		this.queuedPaneEnded = false;
		this.wasChangeCrestButtonPressed = false;
	}

	// Token: 0x06003E27 RID: 15911 RVA: 0x0011166E File Offset: 0x0010F86E
	private void ApplyCurrentCrest()
	{
		this.isWaitingForApply = false;
		if (!this.manager.EndSwitchingCrest())
		{
			return;
		}
		this.StopSwitchingCrests(true);
		this.manager.OnAppliedCrest();
	}

	// Token: 0x06003E28 RID: 15912 RVA: 0x00111698 File Offset: 0x0010F898
	private bool CanApplyCrest()
	{
		bool flag = this.CurrentCrest == this.previousEquippedCrest || this.manager.CanChangeEquips();
		if (!flag && this.manager.EquipState == InventoryItemToolManager.EquipStates.SwitchCrest)
		{
			if (this.manager.ShowingCrestMsg)
			{
				this.manager.HideCrestEquipMsg(false);
				return flag;
			}
			this.manager.ShowCrestEquipMsg();
		}
		return flag;
	}

	// Token: 0x06003E29 RID: 15913 RVA: 0x001116FC File Offset: 0x0010F8FC
	private void SetupCrests()
	{
		if (!this.templateCrest)
		{
			return;
		}
		this.templateCrest.gameObject.SetActive(true);
		List<ToolCrest> allCrests = ToolItemManager.GetAllCrests();
		for (int i = allCrests.Count - this.crests.Count; i > 0; i--)
		{
			InventoryToolCrest item = Object.Instantiate<InventoryToolCrest>(this.templateCrest, this.templateCrest.transform.parent);
			this.crests.Add(item);
		}
		for (int j = 0; j < this.crests.Count; j++)
		{
			InventoryItemManager.PropagateSelectables(this, this.crests[j]);
			this.crests[j].Setup((j < allCrests.Count) ? allCrests[j] : null);
		}
		this.templateCrest.gameObject.SetActive(false);
		if (this.changeCrestButton)
		{
			this.changeCrestButton.gameObject.SetActive(this.CanChangeCrests());
		}
	}

	// Token: 0x06003E2A RID: 15914 RVA: 0x001117F3 File Offset: 0x0010F9F3
	public bool CanChangeCrests()
	{
		return this.crests.Count((InventoryToolCrest crest) => crest.IsUnlocked) > 1;
	}

	// Token: 0x06003E2B RID: 15915 RVA: 0x00111824 File Offset: 0x0010FA24
	private void Setup()
	{
		this.IsSetupComplete = false;
		base.transform.SetLocalPosition2D(this.HomePosition);
		this.SetupCrests();
		foreach (InventoryToolCrest inventoryToolCrest in this.crests)
		{
			inventoryToolCrest.GetEquippedForSlots();
		}
		this.SetupUnlockedCrests();
		InventoryToolCrest inventoryToolCrest2 = null;
		string currentCrestId = GameManager.instance.playerData.CurrentCrestID;
		if (!string.IsNullOrEmpty(currentCrestId))
		{
			inventoryToolCrest2 = this.crests.FirstOrDefault((InventoryToolCrest c) => c.gameObject.name == currentCrestId);
		}
		if (!inventoryToolCrest2 && this.crests.Count > 0)
		{
			inventoryToolCrest2 = this.crests[0];
		}
		if (inventoryToolCrest2)
		{
			this.SetCurrentCrest(inventoryToolCrest2, false, false);
			foreach (InventoryToolCrest inventoryToolCrest3 in this.crests)
			{
				inventoryToolCrest3.UpdateListDisplay(true);
				inventoryToolCrest3.Show(inventoryToolCrest3 == this.CurrentCrest, true);
			}
		}
		this.UpdateEnabledCrests(false);
		if (this.manager)
		{
			this.manager.RefreshTools();
		}
		this.IsSetupComplete = true;
	}

	// Token: 0x06003E2C RID: 15916 RVA: 0x00111988 File Offset: 0x0010FB88
	public override InventoryItemSelectable Get(InventoryItemManager.SelectionDirection? direction)
	{
		List<InventoryToolCrest> list = (from crest in this.crests
		where crest.gameObject.activeSelf
		select crest).ToList<InventoryToolCrest>();
		if (this.CurrentCrest)
		{
			return this.CurrentCrest.Get(direction);
		}
		if (list.Count > 0)
		{
			return list[0].Get(direction);
		}
		return base.Get(direction);
	}

	// Token: 0x06003E2D RID: 15917 RVA: 0x001119FD File Offset: 0x0010FBFD
	public bool CrestHasSlot(ToolItemType type)
	{
		return this.CurrentCrest && this.CurrentCrest.HasSlot(type);
	}

	// Token: 0x06003E2E RID: 15918 RVA: 0x00111A1A File Offset: 0x0010FC1A
	public bool CrestHasAnySlots()
	{
		return this.CurrentCrest && this.CurrentCrest.HasAnySlots();
	}

	// Token: 0x06003E2F RID: 15919 RVA: 0x00111A36 File Offset: 0x0010FC36
	public InventoryToolCrestSlot GetEquippedToolSlot(ToolItem itemData)
	{
		if (this.CurrentCrest)
		{
			return this.CurrentCrest.GetEquippedToolSlot(itemData);
		}
		return null;
	}

	// Token: 0x06003E30 RID: 15920 RVA: 0x00111A53 File Offset: 0x0010FC53
	public IEnumerable<InventoryToolCrestSlot> GetSlots()
	{
		if (this.CurrentCrest)
		{
			return this.CurrentCrest.GetSlots();
		}
		return Enumerable.Empty<InventoryToolCrestSlot>();
	}

	// Token: 0x06003E31 RID: 15921 RVA: 0x00111A74 File Offset: 0x0010FC74
	private void SetCurrentCrest(InventoryToolCrest crest, bool doScroll, bool doSave)
	{
		this.previousSelectedCrest = this.CurrentCrest;
		this.CurrentCrest = crest;
		if (this.previousSelectedCrest)
		{
			this.previousSelectedCrest.UpdateListDisplay(!doScroll);
		}
		if (this.CurrentCrest)
		{
			this.CurrentCrest.UpdateListDisplay(!doScroll);
		}
		this.ScrollToCrest(crest, doScroll ? this.scrollTime : 0f);
		if (Application.isPlaying)
		{
			if (doSave)
			{
				ToolItemManager.SetEquippedCrest(crest.gameObject.name);
			}
			if (this.manager)
			{
				this.manager.RefreshTools(true, false);
			}
			foreach (TextMeshPro textMeshPro in this.crestNameDisplays)
			{
				if (textMeshPro)
				{
					textMeshPro.text = crest.DisplayName;
				}
			}
			if (this.crestDescriptionDisplay)
			{
				this.crestDescriptionDisplay.text = crest.Description;
			}
			ToolCrest crestData = crest.CrestData;
			if (crestData.HasCustomAction)
			{
				this.comboButtonPromptDisplay.Show(crestData.CustomButtonCombo);
			}
			else
			{
				this.comboButtonPromptDisplay.Hide();
			}
		}
		if (this.CurrentCrest == this.previousSelectedCrest)
		{
			return;
		}
		if (this.CurrentCrest)
		{
			foreach (InventoryToolCrestSlot inventoryToolCrestSlot in this.CurrentCrest.GetSlots())
			{
				inventoryToolCrestSlot.SetIsVisible(true);
			}
		}
		if (this.previousSelectedCrest)
		{
			foreach (InventoryToolCrestSlot inventoryToolCrestSlot2 in this.previousSelectedCrest.GetSlots())
			{
				inventoryToolCrestSlot2.SetIsVisible(false);
			}
		}
	}

	// Token: 0x06003E32 RID: 15922 RVA: 0x00111C4C File Offset: 0x0010FE4C
	private void SetupUnlockedCrests()
	{
		this.unlockedCrests.Clear();
		foreach (InventoryToolCrest inventoryToolCrest in this.crests)
		{
			if (inventoryToolCrest.IsUnlocked)
			{
				this.unlockedCrests.Add(inventoryToolCrest);
			}
		}
	}

	// Token: 0x06003E33 RID: 15923 RVA: 0x00111CB8 File Offset: 0x0010FEB8
	private void UpdateEnabledCrests(bool setAllEnabled)
	{
		foreach (InventoryToolCrest inventoryToolCrest in this.crests)
		{
			inventoryToolCrest.gameObject.SetActive(setAllEnabled || inventoryToolCrest == this.CurrentCrest);
		}
	}

	// Token: 0x06003E34 RID: 15924 RVA: 0x00111D24 File Offset: 0x0010FF24
	private void StartSwitchingCrests()
	{
		this.IsSwitchingCrests = true;
		this.previousEquippedCrest = this.CurrentCrest;
		this.UpdateEnabledCrests(true);
		this.scrollLeftArrowGroup.AlphaSelf = 0f;
		this.scrollRightArrowGroup.AlphaSelf = 0f;
		this.CurrentCrest.UpdateListDisplay(false);
		this.ScrollToCrest(this.CurrentCrest, 0f);
		foreach (InventoryToolCrestSlot inventoryToolCrestSlot in this.CurrentCrest.GetSlots())
		{
			inventoryToolCrestSlot.Deselect();
		}
		this.changeCrestEnterAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
		if (this.crestSwitchSequenceRoutine != null)
		{
			base.StopCoroutine(this.crestSwitchSequenceRoutine);
		}
		this.crestSwitchSequenceRoutine = base.StartCoroutine(this.ModeSwitchSequence(true));
	}

	// Token: 0x06003E35 RID: 15925 RVA: 0x00111E10 File Offset: 0x00110010
	public void StopSwitchingCrests(bool keepNewSelection)
	{
		if (this.IsSwitchingCrests)
		{
			if (keepNewSelection)
			{
				this.SetCurrentCrest(this.CurrentCrest, true, true);
			}
			else
			{
				this.SetCurrentCrest(this.previousEquippedCrest, false, true);
				this.changeCrestExitAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
			}
		}
		this.IsSwitchingCrests = false;
		this.previousEquippedCrest = null;
		if (this.crestSwitchSequenceRoutine != null)
		{
			base.StopCoroutine(this.crestSwitchSequenceRoutine);
		}
		this.crestSwitchSequenceRoutine = base.StartCoroutine(this.ModeSwitchSequence(false));
	}

	// Token: 0x06003E36 RID: 15926 RVA: 0x00111E98 File Offset: 0x00110098
	private IEnumerator ModeSwitchSequence(bool isSwitching)
	{
		if (this.crestSwitchMoveRoutine != null)
		{
			base.StopCoroutine(this.crestSwitchMoveRoutine);
		}
		if (isSwitching)
		{
			yield return new WaitForSecondsRealtime(this.manager.FadeToolGroup(false));
			this.manager.RefreshTools();
			this.crestSwitchMoveRoutine = base.StartCoroutine(this.CrestListMove(this.initialPosition + this.crestModeSwitchOffset));
			yield return this.crestSwitchMoveRoutine;
			foreach (InventoryToolCrest inventoryToolCrest in this.unlockedCrests)
			{
				if (inventoryToolCrest != this.CurrentCrest)
				{
					inventoryToolCrest.Show(true, false);
				}
			}
			this.manager.FadeCrestGroup(true);
		}
		else
		{
			float num = this.manager.FadeCrestGroup(false);
			foreach (InventoryToolCrest inventoryToolCrest2 in this.unlockedCrests)
			{
				if (inventoryToolCrest2 != this.CurrentCrest)
				{
					num = Mathf.Max(num, inventoryToolCrest2.Show(false, false));
				}
			}
			yield return new WaitForSecondsRealtime(num);
			this.UpdateEnabledCrests(false);
			this.CurrentCrest.GetEquippedForSlots();
			this.crestSwitchMoveRoutine = base.StartCoroutine(this.CrestListMove(this.HomePosition));
			yield return this.crestSwitchMoveRoutine;
			this.manager.FadeToolGroup(true);
			this.manager.RefreshTools();
		}
		if (this.CurrentCrest)
		{
			this.CurrentCrest.UpdateListDisplay(false);
		}
		this.crestSwitchSequenceRoutine = null;
		yield break;
	}

	// Token: 0x06003E37 RID: 15927 RVA: 0x00111EB0 File Offset: 0x001100B0
	public void PaneMovePrevented()
	{
		HeroActions inputActions = ManagerSingleton<InputHandler>.Instance.inputActions;
		if (inputActions.PaneLeft.IsPressed)
		{
			this.SwitchSelectedCrest(-1);
			return;
		}
		if (inputActions.PaneRight.IsPressed)
		{
			this.SwitchSelectedCrest(1);
		}
	}

	// Token: 0x06003E38 RID: 15928 RVA: 0x00111EF4 File Offset: 0x001100F4
	private void SwitchSelectedCrest(int direction)
	{
		if (!this.IsSwitchingCrests || direction == 0 || this.isWaitingForApply || this.crestSwitchSequenceRoutine != null)
		{
			return;
		}
		this.manager.HideCrestEquipMsg(true);
		direction = (int)Mathf.Sign((float)direction);
		int num = this.unlockedCrests.IndexOf(this.CurrentCrest);
		num += direction;
		if (num < 0 || num >= this.unlockedCrests.Count)
		{
			return;
		}
		BaseAnimator baseAnimator = (direction > 0) ? this.scrollRightArrow : this.scrollLeftArrow;
		if (baseAnimator)
		{
			baseAnimator.StartAnimation();
		}
		this.SetCurrentCrest(this.unlockedCrests[num], true, false);
	}

	// Token: 0x06003E39 RID: 15929 RVA: 0x00111F94 File Offset: 0x00110194
	private void ScrollToCrest(InventoryToolCrest crest, float duration)
	{
		if (this.scrollRoutine != null)
		{
			base.StopCoroutine(this.scrollRoutine);
			this.scrollRoutine = null;
		}
		if (this.onScrollEnd != null)
		{
			this.onScrollEnd();
		}
		if (base.isActiveAndEnabled)
		{
			this.scrollRoutine = base.StartCoroutine(this.ScrollToCrestRoutine(crest, duration));
			return;
		}
		this.ScrollToCrestRoutine(crest, 0f).MoveNext();
	}

	// Token: 0x06003E3A RID: 15930 RVA: 0x00111FFE File Offset: 0x001101FE
	private IEnumerator ScrollToCrestRoutine(InventoryToolCrest crest, float duration)
	{
		this.UpdateCrestPositions(null, null, 0f);
		float x = -crest.transform.localPosition.x;
		Vector3 localPosition = this.scrollParent.localPosition;
		Vector3 targetPosition = localPosition;
		targetPosition.x = x;
		int? previousCrestIndex = null;
		if (this.previousSelectedCrest)
		{
			previousCrestIndex = new int?(this.unlockedCrests.IndexOf(this.previousSelectedCrest));
		}
		int currentCrestIndex = this.unlockedCrests.IndexOf(this.CurrentCrest);
		if (duration > 0f && previousCrestIndex != null)
		{
			int? previousCrestIndex2 = previousCrestIndex;
			int currentCrestIndex2 = currentCrestIndex;
			if (!(previousCrestIndex2.GetValueOrDefault() == currentCrestIndex2 & previousCrestIndex2 != null))
			{
				this.scrollAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
			}
		}
		this.scrollLeftArrowGroup.FadeTo((float)((currentCrestIndex > 0) ? 1 : 0), this.arrowFadeTime, null, true, null);
		this.scrollRightArrowGroup.FadeTo((float)((currentCrestIndex < this.unlockedCrests.Count - 1) ? 1 : 0), this.arrowFadeTime, null, true, null);
		this.onScrollEnd = delegate()
		{
			this.scrollParent.localPosition = targetPosition;
			this.UpdateCrestPositions(previousCrestIndex, new int?(currentCrestIndex), 1f);
			this.onScrollEnd = null;
		};
		for (float elapsed = 0f; elapsed < duration; elapsed += Time.unscaledDeltaTime)
		{
			float num = elapsed / duration;
			this.scrollParent.localPosition = Vector3.Lerp(localPosition, targetPosition, num);
			this.UpdateCrestPositions(previousCrestIndex, new int?(currentCrestIndex), num);
			yield return null;
		}
		this.onScrollEnd();
		yield break;
	}

	// Token: 0x06003E3B RID: 15931 RVA: 0x0011201B File Offset: 0x0011021B
	private IEnumerator CrestListMove(Vector2 toPosition)
	{
		Vector2 fromPosition = base.transform.localPosition;
		for (float elapsed = 0f; elapsed < this.crestModeSwitchMoveTime; elapsed += Time.unscaledDeltaTime)
		{
			base.transform.SetLocalPosition2D(Vector2.Lerp(fromPosition, toPosition, elapsed / this.crestModeSwitchMoveTime));
			yield return null;
		}
		base.transform.SetLocalPosition2D(toPosition);
		yield break;
	}

	// Token: 0x06003E3C RID: 15932 RVA: 0x00112034 File Offset: 0x00110234
	private void UpdateCrestPositions(int? previousCrestIndex, int? currentCrestIndex, float blend)
	{
		for (int i = 0; i < this.unlockedCrests.Count; i++)
		{
			float b = 0f;
			float a = 0f;
			int num = i;
			int? num2 = currentCrestIndex + 1;
			if (num == num2.GetValueOrDefault() & num2 != null)
			{
				b = 1f;
			}
			else
			{
				int num3 = i;
				num2 = currentCrestIndex - 1;
				if (num3 == num2.GetValueOrDefault() & num2 != null)
				{
					b = -1f;
				}
			}
			int num4 = i;
			num2 = previousCrestIndex + 1;
			if (num4 == num2.GetValueOrDefault() & num2 != null)
			{
				a = 1f;
			}
			else
			{
				int num5 = i;
				num2 = previousCrestIndex - 1;
				if (num5 == num2.GetValueOrDefault() & num2 != null)
				{
					a = -1f;
				}
			}
			float num6 = Mathf.Lerp(a, b, blend);
			this.unlockedCrests[i].transform.SetLocalPositionX(this.crestSpacing * (float)i + num6 * this.adjacentCrestOffset);
		}
	}

	// Token: 0x06003E3D RID: 15933 RVA: 0x001121A6 File Offset: 0x001103A6
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(this.crestModeSwitchOffset, 0.25f);
	}

	// Token: 0x04003FC1 RID: 16321
	[SerializeField]
	private TextMeshPro[] crestNameDisplays;

	// Token: 0x04003FC2 RID: 16322
	[SerializeField]
	private TextMeshPro crestDescriptionDisplay;

	// Token: 0x04003FC3 RID: 16323
	[SerializeField]
	private InventoryItemComboButtonPromptDisplay comboButtonPromptDisplay;

	// Token: 0x04003FC4 RID: 16324
	[Space]
	[SerializeField]
	private InventoryToolCrest templateCrest;

	// Token: 0x04003FC5 RID: 16325
	[SerializeField]
	private float crestSpacing = 4f;

	// Token: 0x04003FC6 RID: 16326
	[SerializeField]
	private float adjacentCrestOffset = 1f;

	// Token: 0x04003FC7 RID: 16327
	[SerializeField]
	private Transform scrollParent;

	// Token: 0x04003FC8 RID: 16328
	[SerializeField]
	private float scrollTime = 0.3f;

	// Token: 0x04003FC9 RID: 16329
	[SerializeField]
	private AudioEvent scrollAudio;

	// Token: 0x04003FCA RID: 16330
	[SerializeField]
	private BaseAnimator scrollLeftArrow;

	// Token: 0x04003FCB RID: 16331
	[SerializeField]
	private NestedFadeGroupBase scrollLeftArrowGroup;

	// Token: 0x04003FCC RID: 16332
	[SerializeField]
	private BaseAnimator scrollRightArrow;

	// Token: 0x04003FCD RID: 16333
	[SerializeField]
	private NestedFadeGroupBase scrollRightArrowGroup;

	// Token: 0x04003FCE RID: 16334
	[SerializeField]
	private float arrowFadeTime = 0.3f;

	// Token: 0x04003FCF RID: 16335
	[SerializeField]
	private AudioEvent changeCrestEnterAudio;

	// Token: 0x04003FD0 RID: 16336
	[SerializeField]
	private AudioEvent changeCrestExitAudio;

	// Token: 0x04003FD1 RID: 16337
	[SerializeField]
	private InventoryItemSelectableButtonEvent changeCrestButton;

	// Token: 0x04003FD2 RID: 16338
	[SerializeField]
	private Animator changeCrestIconAnimator;

	// Token: 0x04003FD3 RID: 16339
	[SerializeField]
	private Vector2 crestModeSwitchOffset;

	// Token: 0x04003FD4 RID: 16340
	[Space]
	[SerializeField]
	private float crestModeSwitchMoveTime = 0.2f;

	// Token: 0x04003FD5 RID: 16341
	[SerializeField]
	private GameObject nudgeIfActive;

	// Token: 0x04003FD6 RID: 16342
	[SerializeField]
	private Vector2 nudgeOffset;

	// Token: 0x04003FD7 RID: 16343
	private readonly List<InventoryToolCrest> crests = new List<InventoryToolCrest>();

	// Token: 0x04003FD8 RID: 16344
	private readonly List<InventoryToolCrest> unlockedCrests = new List<InventoryToolCrest>();

	// Token: 0x04003FD9 RID: 16345
	private Coroutine crestSwitchMoveRoutine;

	// Token: 0x04003FDA RID: 16346
	private Coroutine crestSwitchSequenceRoutine;

	// Token: 0x04003FDB RID: 16347
	private Coroutine scrollRoutine;

	// Token: 0x04003FDC RID: 16348
	private Action onScrollEnd;

	// Token: 0x04003FDD RID: 16349
	private Vector2 initialPosition;

	// Token: 0x04003FDE RID: 16350
	private bool wasChangeCrestButtonPressed;

	// Token: 0x04003FDF RID: 16351
	private InventoryPaneBase pane;

	// Token: 0x04003FE0 RID: 16352
	private InventoryPaneInput paneInput;

	// Token: 0x04003FE1 RID: 16353
	private InventoryItemToolManager manager;

	// Token: 0x04003FE2 RID: 16354
	private InputHandler inputHandler;

	// Token: 0x04003FE3 RID: 16355
	private bool queuedPaneEnded;

	// Token: 0x04003FE4 RID: 16356
	private bool isWaitingForApply;

	// Token: 0x04003FE5 RID: 16357
	private InventoryToolCrest previousSelectedCrest;

	// Token: 0x04003FE6 RID: 16358
	private InventoryToolCrest previousEquippedCrest;

	// Token: 0x04003FE7 RID: 16359
	private static readonly int _failed = Animator.StringToHash("Failed");
}
