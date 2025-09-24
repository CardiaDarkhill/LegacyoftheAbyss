using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GlobalSettings;
using TeamCherry.Localization;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x020006B8 RID: 1720
[DefaultExecutionOrder(1)]
public class InventoryToolCrest : InventoryItemSelectableDirectional
{
	// Token: 0x1700071B RID: 1819
	// (get) Token: 0x06003E00 RID: 15872 RVA: 0x00110496 File Offset: 0x0010E696
	public override string DisplayName
	{
		get
		{
			return this.displayName;
		}
	}

	// Token: 0x1700071C RID: 1820
	// (get) Token: 0x06003E01 RID: 15873 RVA: 0x001104A3 File Offset: 0x0010E6A3
	public override string Description
	{
		get
		{
			return this.description;
		}
	}

	// Token: 0x1700071D RID: 1821
	// (get) Token: 0x06003E02 RID: 15874 RVA: 0x001104B0 File Offset: 0x0010E6B0
	public bool IsUnlocked
	{
		get
		{
			return this.CrestData && this.CrestData.IsVisible;
		}
	}

	// Token: 0x1700071E RID: 1822
	// (get) Token: 0x06003E03 RID: 15875 RVA: 0x001104CC File Offset: 0x0010E6CC
	public bool IsHidden
	{
		get
		{
			return this.CrestData && this.CrestData.IsHidden;
		}
	}

	// Token: 0x1700071F RID: 1823
	// (get) Token: 0x06003E04 RID: 15876 RVA: 0x001104E8 File Offset: 0x0010E6E8
	// (set) Token: 0x06003E05 RID: 15877 RVA: 0x001104F0 File Offset: 0x0010E6F0
	public ToolCrest CrestData { get; private set; }

	// Token: 0x06003E06 RID: 15878 RVA: 0x001104F9 File Offset: 0x0010E6F9
	protected override void OnValidate()
	{
		base.OnValidate();
		ArrayForEnumAttribute.EnsureArraySize<InventoryToolCrestSlot>(ref this.templateSlots, typeof(ToolItemType));
	}

	// Token: 0x06003E07 RID: 15879 RVA: 0x00110516 File Offset: 0x0010E716
	protected override void Awake()
	{
		base.Awake();
		this.manager = base.GetComponentInParent<InventoryItemToolManager>();
		if (this.newIndicator)
		{
			this.newIndicatorInitialScale = this.newIndicator.transform.localScale;
		}
	}

	// Token: 0x06003E08 RID: 15880 RVA: 0x0011054D File Offset: 0x0010E74D
	protected override void OnDisable()
	{
		base.OnDisable();
		if (this.equipRoutine != null)
		{
			base.StopCoroutine(this.equipRoutine);
		}
	}

	// Token: 0x06003E09 RID: 15881 RVA: 0x0011056C File Offset: 0x0010E76C
	public void Setup(ToolCrest newCrestData)
	{
		this.CrestData = newCrestData;
		base.gameObject.name = (newCrestData ? newCrestData.name : "Spare Crest");
		if (this.crestSubmitAnimator && this.crestSubmitAnimator.isActiveAndEnabled)
		{
			this.crestSubmitAnimator.Play(InventoryToolCrest._inertAnim);
		}
		if (newCrestData)
		{
			this.displayName = newCrestData.DisplayName;
			this.description = newCrestData.Description;
			if (this.crestSprite)
			{
				this.crestSprite.Sprite = newCrestData.CrestSprite;
			}
			if (this.crestSilhouette)
			{
				this.crestSilhouette.Sprite = newCrestData.CrestSilhouette;
			}
			if (this.crestGlowSprite)
			{
				this.crestGlowSprite.sprite = newCrestData.CrestGlow;
			}
			GameObject displayPrefab = newCrestData.DisplayPrefab;
			if (displayPrefab)
			{
				GameObject gameObject;
				GameObject gameObject2;
				if (this.spawnedDisplayObjects.TryGetValue(displayPrefab, out gameObject))
				{
					gameObject2 = gameObject;
				}
				else
				{
					gameObject2 = Object.Instantiate<GameObject>(displayPrefab, base.transform);
					gameObject2.transform.localPosition = Vector3.zero;
					this.spawnedDisplayObjects[displayPrefab] = gameObject2;
				}
				if (this.activeDisplayObject && this.activeDisplayObject != gameObject2)
				{
					this.activeDisplayObject.SetActive(false);
				}
				gameObject2.SetActive(true);
				this.activeDisplayObject = gameObject2;
			}
			if (this.spawnedSlots == null)
			{
				this.spawnedSlots = new Dictionary<ToolItemType, List<InventoryToolCrestSlot>>();
				foreach (ToolItemType key in InventoryToolCrest.TOOL_TYPES)
				{
					this.spawnedSlots[key] = new List<InventoryToolCrestSlot>();
				}
			}
			if (this.spawnedSlotsRemaining == null)
			{
				this.spawnedSlotsRemaining = new Dictionary<ToolItemType, Queue<InventoryToolCrestSlot>>();
				foreach (ToolItemType key2 in InventoryToolCrest.TOOL_TYPES)
				{
					this.spawnedSlotsRemaining[key2] = new Queue<InventoryToolCrestSlot>();
				}
			}
			ToolItemType[] tool_TYPES = InventoryToolCrest.TOOL_TYPES;
			for (int i = 0; i < tool_TYPES.Length; i++)
			{
				ToolItemType type = tool_TYPES[i];
				int num = newCrestData.Slots.Count((ToolCrest.SlotInfo slotData) => slotData.Type == type);
				int count = this.spawnedSlots[type].Count;
				int j = num - count;
				InventoryToolCrestSlot inventoryToolCrestSlot = this.templateSlots[(int)type];
				inventoryToolCrestSlot.gameObject.SetActive(false);
				while (j > 0)
				{
					InventoryToolCrestSlot inventoryToolCrestSlot2 = Object.Instantiate<InventoryToolCrestSlot>(inventoryToolCrestSlot, inventoryToolCrestSlot.transform.parent);
					this.spawnedSlots[type].Add(inventoryToolCrestSlot2);
					inventoryToolCrestSlot2.OnSetEquipSaved += this.SaveEquips;
					j--;
				}
				this.spawnedSlotsRemaining[type].Clear();
				foreach (InventoryToolCrestSlot inventoryToolCrestSlot3 in this.spawnedSlots[type])
				{
					inventoryToolCrestSlot3.gameObject.SetActive(false);
					this.spawnedSlotsRemaining[type].Enqueue(inventoryToolCrestSlot3);
				}
			}
			this.activeSlots.Clear();
			this.activeSlotsData.Clear();
			for (int k = 0; k < newCrestData.Slots.Length; k++)
			{
				ToolCrest.SlotInfo slotInfo = newCrestData.Slots[k];
				InventoryToolCrestSlot inventoryToolCrestSlot4 = this.spawnedSlotsRemaining[slotInfo.Type].Dequeue();
				inventoryToolCrestSlot4.gameObject.SetActive(true);
				inventoryToolCrestSlot4.SetCrestInfo(this, k, null, null);
				inventoryToolCrestSlot4.transform.SetLocalPosition2D(slotInfo.Position);
				this.activeSlots.Add(inventoryToolCrestSlot4);
				this.activeSlotsData.Add(slotInfo);
			}
		}
		for (int l = 0; l < this.activeSlots.Count; l++)
		{
			InventoryToolCrestSlot inventoryToolCrestSlot5 = this.activeSlots[l];
			ToolCrest.SlotInfo slotInfo2 = this.activeSlotsData[l];
			inventoryToolCrestSlot5.Selectables[0] = this.GetActiveSlot(slotInfo2.NavUpIndex);
			inventoryToolCrestSlot5.Selectables[1] = this.GetActiveSlot(slotInfo2.NavDownIndex);
			inventoryToolCrestSlot5.Selectables[2] = this.GetActiveSlot(slotInfo2.NavLeftIndex);
			inventoryToolCrestSlot5.Selectables[3] = this.GetActiveSlot(slotInfo2.NavRightIndex);
			this.SetListSlotIndex(inventoryToolCrestSlot5.FallbackSelectables[0].Selectables, slotInfo2.NavUpFallbackIndex);
			this.SetListSlotIndex(inventoryToolCrestSlot5.FallbackSelectables[1].Selectables, slotInfo2.NavDownFallbackIndex);
			this.SetListSlotIndex(inventoryToolCrestSlot5.FallbackSelectables[2].Selectables, slotInfo2.NavLeftFallbackIndex);
			this.SetListSlotIndex(inventoryToolCrestSlot5.FallbackSelectables[3].Selectables, slotInfo2.NavRightFallbackIndex);
			inventoryToolCrestSlot5.SlotInfo = slotInfo2;
		}
		foreach (InventoryToolCrestSlot target in this.activeSlots)
		{
			InventoryItemManager.PropagateSelectables(this, target);
		}
		this.isNew = (!newCrestData.IsHidden && newCrestData.SaveData.DisplayNewIndicator);
	}

	// Token: 0x06003E0A RID: 15882 RVA: 0x00110AB4 File Offset: 0x0010ECB4
	private InventoryToolCrestSlot GetActiveSlot(int index)
	{
		if (index < 0)
		{
			return null;
		}
		if (index >= this.activeSlots.Count)
		{
			Debug.LogError("Crest slot index out of range!", this);
			return null;
		}
		return this.activeSlots[index];
	}

	// Token: 0x06003E0B RID: 15883 RVA: 0x00110AE3 File Offset: 0x0010ECE3
	private void SetListSlotIndex(List<InventoryItemSelectable> selectables, int slotIndex)
	{
		selectables.Clear();
		if (slotIndex < 0)
		{
			return;
		}
		if (slotIndex >= this.activeSlots.Count)
		{
			Debug.LogError("Crest slot index out of range!", this);
			return;
		}
		selectables.Add(this.activeSlots[slotIndex]);
	}

	// Token: 0x06003E0C RID: 15884 RVA: 0x00110B1C File Offset: 0x0010ED1C
	public void GetEquippedForSlots()
	{
		List<ToolItem> equippedToolsForCrest = ToolItemManager.GetEquippedToolsForCrest(base.gameObject.name);
		if (equippedToolsForCrest == null)
		{
			return;
		}
		for (int i = 0; i < Mathf.Min(equippedToolsForCrest.Count, this.activeSlots.Count); i++)
		{
			this.activeSlots[i].SetEquipped(equippedToolsForCrest[i], false, false);
		}
	}

	// Token: 0x06003E0D RID: 15885 RVA: 0x00110B7C File Offset: 0x0010ED7C
	private void SaveEquips()
	{
		ToolItemManager.SetEquippedTools(base.gameObject.name, this.activeSlots.Select(delegate(InventoryToolCrestSlot slot)
		{
			if (!slot.EquippedItem)
			{
				return null;
			}
			return slot.EquippedItem.name;
		}).ToList<string>());
	}

	// Token: 0x06003E0E RID: 15886 RVA: 0x00110BC8 File Offset: 0x0010EDC8
	public override InventoryItemSelectable Get(InventoryItemManager.SelectionDirection? direction)
	{
		if (this.activeSlots.Count <= 0)
		{
			return null;
		}
		if (direction == null)
		{
			return this.activeSlots[0].Get(null);
		}
		if (this.manager && this.manager.CurrentSelected)
		{
			InventoryToolCrestSlot closestOnAxis = InventoryItemNavigationHelper.GetClosestOnAxis<InventoryToolCrestSlot>(direction.Value, this.manager.CurrentSelected, this.activeSlots);
			if (closestOnAxis)
			{
				return closestOnAxis.Get(direction);
			}
		}
		return this.activeSlots[0].Get(direction);
	}

	// Token: 0x06003E0F RID: 15887 RVA: 0x00110C68 File Offset: 0x0010EE68
	public bool HasSlot(ToolItemType type)
	{
		foreach (InventoryToolCrestSlot inventoryToolCrestSlot in this.activeSlots)
		{
			if (inventoryToolCrestSlot.Type == type && !inventoryToolCrestSlot.IsLocked)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003E10 RID: 15888 RVA: 0x00110CCC File Offset: 0x0010EECC
	public bool HasAnySlots()
	{
		using (List<InventoryToolCrestSlot>.Enumerator enumerator = this.activeSlots.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.IsLocked)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06003E11 RID: 15889 RVA: 0x00110D28 File Offset: 0x0010EF28
	public bool HasSlot(InventoryToolCrestSlot otherSlot)
	{
		using (List<InventoryToolCrestSlot>.Enumerator enumerator = this.activeSlots.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == otherSlot)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06003E12 RID: 15890 RVA: 0x00110D84 File Offset: 0x0010EF84
	public InventoryToolCrestSlot GetEquippedToolSlot(ToolItem toolItem)
	{
		return this.activeSlots.FirstOrDefault((InventoryToolCrestSlot slot) => slot.EquippedItem == toolItem);
	}

	// Token: 0x06003E13 RID: 15891 RVA: 0x00110DB5 File Offset: 0x0010EFB5
	public IEnumerable<InventoryToolCrestSlot> GetSlots()
	{
		return this.activeSlots;
	}

	// Token: 0x06003E14 RID: 15892 RVA: 0x00110DC0 File Offset: 0x0010EFC0
	public float Show(bool value, bool isInstant)
	{
		if (!this.fadeGroup)
		{
			this.fadeGroup = base.GetComponent<NestedFadeGroupBase>();
		}
		float result = isInstant ? 0f : this.fadeTime;
		if (!this.fadeGroup)
		{
			return result;
		}
		return this.fadeGroup.FadeTo((float)(value ? 1 : 0), result, null, true, null);
	}

	// Token: 0x06003E15 RID: 15893 RVA: 0x00110E20 File Offset: 0x0010F020
	public void UpdateListDisplay(bool isInstant = false)
	{
		if (!this.crestList)
		{
			this.crestList = base.GetComponentInParent<InventoryToolCrestList>();
			this.defaultScale = base.transform.localScale;
		}
		bool flag = this.crestList.CurrentCrest == this;
		if (!this.IsUnlocked)
		{
			if (flag)
			{
				if (!base.gameObject.activeSelf)
				{
					base.gameObject.SetActive(true);
				}
			}
			else if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
			}
		}
		if (this.crestList)
		{
			Color newColor;
			switch (this.manager.EquipState)
			{
			case InventoryItemToolManager.EquipStates.None:
				newColor = InventoryToolCrest.DeselectedColor;
				break;
			case InventoryItemToolManager.EquipStates.PlaceTool:
			case InventoryItemToolManager.EquipStates.SelectTool:
				newColor = InventoryToolCrestSlot.InvalidItemColor;
				break;
			case InventoryItemToolManager.EquipStates.SwitchCrest:
				newColor = ((this.crestList.CurrentCrest == this) ? Color.white : InventoryToolCrest.DeselectedColor);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			Vector3 newScale = (this.crestList.CurrentCrest == this) ? this.defaultScale : this.deselectedScale;
			newScale.z = 1f;
			if (this.transitionRoutine != null)
			{
				base.StopCoroutine(this.transitionRoutine);
			}
			if (!isInstant)
			{
				this.transitionRoutine = base.StartCoroutine(this.TransitionDisplayState(newColor, newScale, flag, false));
			}
			else
			{
				this.TransitionDisplayState(newColor, newScale, flag, true).MoveNext();
			}
		}
		if (this.isNew)
		{
			if (flag)
			{
				if (this.crestList.IsSwitchingCrests)
				{
					ToolCrestsData.Data saveData = this.CrestData.SaveData;
					saveData.DisplayNewIndicator = false;
					this.CrestData.SaveData = saveData;
					this.isNew = false;
					if (this.newIndicator && this.newIndicator.activeSelf)
					{
						this.newIndicator.transform.ScaleTo(this, Vector3.zero, UI.NewDotScaleTime, 0f, false, true, null);
						return;
					}
				}
				else if (this.newIndicator)
				{
					this.newIndicator.SetActive(false);
					return;
				}
			}
			else if (this.newIndicator)
			{
				this.newIndicator.SetActive(true);
				this.newIndicator.transform.localScale = this.newIndicatorInitialScale;
				return;
			}
		}
		else if (this.newIndicator)
		{
			this.newIndicator.SetActive(false);
		}
	}

	// Token: 0x06003E16 RID: 15894 RVA: 0x00111079 File Offset: 0x0010F279
	private IEnumerator TransitionDisplayState(Color newColor, Vector3 newScale, bool isCurrentCrest, bool isInstant)
	{
		InventoryToolCrest.<>c__DisplayClass61_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.newColor = newColor;
		CS$<>8__locals1.newScale = newScale;
		CS$<>8__locals1.startColor = (this.crestSprite ? this.crestSprite.Color : Color.white);
		CS$<>8__locals1.startScale = base.transform.localScale;
		CS$<>8__locals1.oldFlashAmount = (float)(this.wasCurrentCrest ? 0 : 1);
		CS$<>8__locals1.newFlashAmount = (float)(isCurrentCrest ? 0 : 1);
		CS$<>8__locals1.slotScaleStart = (this.wasCurrentCrest ? InventoryToolCrest._slotScaleCurrent : InventoryToolCrest._slotScaleOther);
		CS$<>8__locals1.slotScaleEnd = (isCurrentCrest ? InventoryToolCrest._slotScaleCurrent : InventoryToolCrest._slotScaleOther);
		this.wasCurrentCrest = isCurrentCrest;
		if (!isInstant)
		{
			for (float elapsed = 0f; elapsed < this.lerpTime; elapsed += Time.unscaledDeltaTime)
			{
				this.<TransitionDisplayState>g__SetLerpedValues|61_0(elapsed / this.lerpTime, ref CS$<>8__locals1);
				yield return null;
			}
		}
		this.<TransitionDisplayState>g__SetLerpedValues|61_0(1f, ref CS$<>8__locals1);
		yield break;
	}

	// Token: 0x06003E17 RID: 15895 RVA: 0x001110A5 File Offset: 0x0010F2A5
	public void DoEquip(Action onEquip)
	{
		if (!this.crestSubmitAnimator || !this.crestSubmitAnimator.isActiveAndEnabled)
		{
			onEquip();
			return;
		}
		this.equipRoutine = base.StartCoroutine(this.DoEquipAnim(onEquip));
	}

	// Token: 0x06003E18 RID: 15896 RVA: 0x001110DB File Offset: 0x0010F2DB
	private IEnumerator DoEquipAnim(Action onEquip)
	{
		this.crestSubmitAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
		this.crestSubmitAnimator.Play(InventoryToolCrest._burstAnim, 0, 0f);
		yield return null;
		if (this.crestSubmitAnimator.updateMode == AnimatorUpdateMode.UnscaledTime)
		{
			yield return new WaitForSecondsRealtime(this.crestSubmitAnimator.GetCurrentAnimatorStateInfo(0).length);
		}
		else
		{
			yield return new WaitForSeconds(this.crestSubmitAnimator.GetCurrentAnimatorStateInfo(0).length);
		}
		onEquip();
		yield break;
	}

	// Token: 0x06003E1B RID: 15899 RVA: 0x001111E8 File Offset: 0x0010F3E8
	[CompilerGenerated]
	private void <TransitionDisplayState>g__SetLerpedValues|61_0(float time, ref InventoryToolCrest.<>c__DisplayClass61_0 A_2)
	{
		float num = Mathf.Lerp(A_2.oldFlashAmount, A_2.newFlashAmount, time);
		float a = 1f - num;
		if (this.crestSprite)
		{
			Color color = Color.Lerp(A_2.startColor, A_2.newColor, time);
			color.a = a;
			this.crestSprite.Color = color;
		}
		if (this.crestSilhouette)
		{
			this.crestSilhouette.AlphaSelf = num;
		}
		base.transform.localScale = Vector3.Lerp(A_2.startScale, A_2.newScale, time);
		foreach (InventoryToolCrestSlot inventoryToolCrestSlot in this.activeSlots)
		{
			inventoryToolCrestSlot.ItemFlashAmount = num;
			inventoryToolCrestSlot.transform.localScale = Vector3.Lerp(A_2.slotScaleStart, A_2.slotScaleEnd, time);
		}
	}

	// Token: 0x04003F9F RID: 16287
	private static readonly Vector3 _slotScaleCurrent = new Vector3(1f, 1f, 1f);

	// Token: 0x04003FA0 RID: 16288
	private static readonly Vector3 _slotScaleOther = new Vector3(0.7f, 0.7f, 1f);

	// Token: 0x04003FA1 RID: 16289
	public static readonly Color DeselectedColor = new Color(0.5f, 0.5f, 0.5f, 1f);

	// Token: 0x04003FA2 RID: 16290
	[Space]
	[SerializeField]
	private LocalisedString displayName;

	// Token: 0x04003FA3 RID: 16291
	[SerializeField]
	private LocalisedString description;

	// Token: 0x04003FA4 RID: 16292
	[Space]
	[SerializeField]
	private NestedFadeGroupSpriteRenderer crestSprite;

	// Token: 0x04003FA5 RID: 16293
	[SerializeField]
	private NestedFadeGroupSpriteRenderer crestSilhouette;

	// Token: 0x04003FA6 RID: 16294
	[SerializeField]
	private float lerpTime = 0.2f;

	// Token: 0x04003FA7 RID: 16295
	[SerializeField]
	private float fadeTime = 0.2f;

	// Token: 0x04003FA8 RID: 16296
	private NestedFadeGroupBase fadeGroup;

	// Token: 0x04003FA9 RID: 16297
	[SerializeField]
	private Vector2 deselectedScale = new Vector2(0.5f, 0.5f);

	// Token: 0x04003FAA RID: 16298
	private Vector2 defaultScale;

	// Token: 0x04003FAB RID: 16299
	[Space]
	[SerializeField]
	private SpriteRenderer crestGlowSprite;

	// Token: 0x04003FAC RID: 16300
	[SerializeField]
	private Animator crestSubmitAnimator;

	// Token: 0x04003FAD RID: 16301
	[SerializeField]
	private AudioEvent crestSubmitAudio;

	// Token: 0x04003FAE RID: 16302
	[Space]
	[SerializeField]
	[ArrayForEnum(typeof(ToolItemType))]
	private InventoryToolCrestSlot[] templateSlots;

	// Token: 0x04003FAF RID: 16303
	[SerializeField]
	private GameObject newIndicator;

	// Token: 0x04003FB0 RID: 16304
	private Dictionary<ToolItemType, List<InventoryToolCrestSlot>> spawnedSlots;

	// Token: 0x04003FB1 RID: 16305
	private Dictionary<ToolItemType, Queue<InventoryToolCrestSlot>> spawnedSlotsRemaining;

	// Token: 0x04003FB2 RID: 16306
	private readonly List<ToolCrest.SlotInfo> activeSlotsData = new List<ToolCrest.SlotInfo>();

	// Token: 0x04003FB3 RID: 16307
	private readonly List<InventoryToolCrestSlot> activeSlots = new List<InventoryToolCrestSlot>();

	// Token: 0x04003FB4 RID: 16308
	private Coroutine transitionRoutine;

	// Token: 0x04003FB5 RID: 16309
	private bool wasCurrentCrest;

	// Token: 0x04003FB6 RID: 16310
	private Vector3 newIndicatorInitialScale;

	// Token: 0x04003FB7 RID: 16311
	private bool isNew;

	// Token: 0x04003FB8 RID: 16312
	private GameObject activeDisplayObject;

	// Token: 0x04003FB9 RID: 16313
	private readonly Dictionary<GameObject, GameObject> spawnedDisplayObjects = new Dictionary<GameObject, GameObject>();

	// Token: 0x04003FBA RID: 16314
	private InventoryToolCrestList crestList;

	// Token: 0x04003FBB RID: 16315
	private InventoryItemToolManager manager;

	// Token: 0x04003FBC RID: 16316
	private Coroutine equipRoutine;

	// Token: 0x04003FBD RID: 16317
	private static readonly int _inertAnim = Animator.StringToHash("Inert");

	// Token: 0x04003FBE RID: 16318
	private static readonly int _burstAnim = Animator.StringToHash("Burst");

	// Token: 0x04003FBF RID: 16319
	private static ToolItemType[] TOOL_TYPES = (ToolItemType[])Enum.GetValues(typeof(ToolItemType));
}
