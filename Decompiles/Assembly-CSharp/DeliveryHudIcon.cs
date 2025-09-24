using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005D4 RID: 1492
public class DeliveryHudIcon : RadialHudIcon
{
	// Token: 0x060034FA RID: 13562 RVA: 0x000EB214 File Offset: 0x000E9414
	private void Awake()
	{
		this.gm = GameManager.instance;
		this.gm.SceneInit += base.UpdateDisplay;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "DELIVERY HUD REFRESH").ReceivedEvent += base.UpdateDisplay;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "DELIVERY HUD HIT").ReceivedEvent += this.SpawnHitEffect;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "DELIVERY HUD BREAK").ReceivedEvent += this.SpawnBreakEffect;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "TOOLS APPEAR").ReceivedEvent += this.StartUp;
	}

	// Token: 0x060034FB RID: 13563 RVA: 0x000EB2C7 File Offset: 0x000E94C7
	private void OnEnable()
	{
		if (this.burstAppearChild)
		{
			this.burstAppearChild.SetActive(false);
		}
	}

	// Token: 0x060034FC RID: 13564 RVA: 0x000EB2E2 File Offset: 0x000E94E2
	private void OnDestroy()
	{
		if (this.gm)
		{
			this.gm.SceneInit -= base.UpdateDisplay;
		}
	}

	// Token: 0x060034FD RID: 13565 RVA: 0x000EB308 File Offset: 0x000E9508
	private void SpawnHitEffect()
	{
		this.OnHit.Invoke();
		Vector3 position = base.transform.position;
		if (this.hitEffectPrefab)
		{
			this.hitEffectPrefab.Spawn(position);
		}
		if (this.hasCustomHitEffect)
		{
			this.customHitEffectPrefab.Spawn(position);
		}
	}

	// Token: 0x060034FE RID: 13566 RVA: 0x000EB35C File Offset: 0x000E955C
	private void SpawnBreakEffect()
	{
		this.OnBreak.Invoke();
		this.StopLoopEffect();
		this.CleanLoopEffect();
		if (!this.currentItem)
		{
			return;
		}
		GameObject breakUIEffect = this.currentItem.BreakUIEffect;
		if (!breakUIEffect)
		{
			return;
		}
		breakUIEffect.Spawn(base.transform.position);
	}

	// Token: 0x060034FF RID: 13567 RVA: 0x000EB3B5 File Offset: 0x000E95B5
	private void StartUp()
	{
		this.started = true;
		this.isHudOut = false;
		base.UpdateDisplay();
		if (base.gameObject.activeSelf && this.burstAppearChild)
		{
			this.burstAppearChild.SetActive(true);
		}
	}

	// Token: 0x06003500 RID: 13568 RVA: 0x000EB3F1 File Offset: 0x000E95F1
	public void HudOut()
	{
		this.isHudOut = true;
		this.StopLoopEffect();
	}

	// Token: 0x06003501 RID: 13569 RVA: 0x000EB400 File Offset: 0x000E9600
	public void HudIn()
	{
		this.isHudOut = false;
		if (this.queuedAppear && this.started)
		{
			base.UpdateDisplay();
			this.StartLoopEffect();
			if (this.burstAppearChild)
			{
				this.burstAppearChild.SetActive(true);
			}
		}
	}

	// Token: 0x06003502 RID: 13570 RVA: 0x000EB440 File Offset: 0x000E9640
	protected override void OnPreUpdateDisplay()
	{
		this.previousItem = this.currentItem;
		this.currentItem = null;
		this.currentQuest = null;
		using (IEnumerator<DeliveryQuestItem.ActiveItem> enumerator = DeliveryQuestItem.GetActiveItems().GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				DeliveryQuestItem.ActiveItem activeItem = enumerator.Current;
				this.currentItem = activeItem.Item;
				this.currentQuest = activeItem.Quest;
				this.maxItemCount = activeItem.MaxCount;
			}
		}
		bool flag = this.currentItem != null;
		if (this.currentItem != this.previousItem)
		{
			this.hasCustomHitEffect = false;
			this.customHitEffectPrefab = null;
			if (this.previousItem != null)
			{
				this.CleanLoopEffect();
			}
			if (flag)
			{
				this.customHitEffectPrefab = this.currentItem.HitUIEffect;
				this.hasCustomHitEffect = (this.customHitEffectPrefab != null);
				this.SpawnLoopEffect();
			}
		}
		if (this.isHudOut && flag)
		{
			this.queuedAppear = true;
			return;
		}
		this.queuedAppear = false;
		if (flag)
		{
			this.StartLoopEffect();
		}
	}

	// Token: 0x06003503 RID: 13571 RVA: 0x000EB558 File Offset: 0x000E9758
	protected override bool GetIsActive()
	{
		return this.started && !this.queuedAppear && this.currentItem && DeliveryQuestItem.CanTakeHit();
	}

	// Token: 0x06003504 RID: 13572 RVA: 0x000EB582 File Offset: 0x000E9782
	protected override void GetAmounts(out int amountLeft, out int totalCount)
	{
		amountLeft = (this.currentQuest ? this.currentQuest.Counters.FirstOrDefault<int>() : this.currentItem.CollectedAmount);
		totalCount = this.maxItemCount;
	}

	// Token: 0x06003505 RID: 13573 RVA: 0x000EB5B8 File Offset: 0x000E97B8
	protected override bool TryGetHudSprite(out Sprite sprite)
	{
		sprite = this.currentItem.GetIcon(CollectableItem.ReadSource.Tiny);
		if (sprite)
		{
			return true;
		}
		sprite = this.currentItem.GetIcon(CollectableItem.ReadSource.Inventory);
		return false;
	}

	// Token: 0x06003506 RID: 13574 RVA: 0x000EB5E2 File Offset: 0x000E97E2
	public override bool GetIsEmpty()
	{
		return false;
	}

	// Token: 0x06003507 RID: 13575 RVA: 0x000EB5E5 File Offset: 0x000E97E5
	protected override bool HasTargetChanged()
	{
		return this.currentItem != this.previousItem;
	}

	// Token: 0x06003508 RID: 13576 RVA: 0x000EB5F8 File Offset: 0x000E97F8
	protected override bool TryGetBarColour(out Color color)
	{
		if (!this.currentItem)
		{
			color = Color.white;
			return false;
		}
		color = this.currentItem.BarColour;
		return true;
	}

	// Token: 0x06003509 RID: 13577 RVA: 0x000EB628 File Offset: 0x000E9828
	protected override float GetMidProgress()
	{
		foreach (HeroController.DeliveryTimer deliveryTimer in HeroController.instance.GetDeliveryTimers())
		{
			if (!(deliveryTimer.Item.Item != this.currentItem))
			{
				float timeLeft = deliveryTimer.TimeLeft;
				float chunkDuration = deliveryTimer.Item.Item.GetChunkDuration(deliveryTimer.Item.MaxCount);
				return (chunkDuration - timeLeft) / chunkDuration;
			}
		}
		return 0f;
	}

	// Token: 0x0600350A RID: 13578 RVA: 0x000EB6C0 File Offset: 0x000E98C0
	private void SpawnLoopEffect()
	{
		if (this.hasLoopEffect)
		{
			if (this.loopEffectObject != null)
			{
				Object.Destroy(this.loopEffectObject);
			}
			this.hasLoopEffect = false;
		}
		if (this.currentItem != this.previousItem && this.currentItem != null && this.currentItem.UILoopEffect != null)
		{
			this.loopEffectObject = this.currentItem.UILoopEffect.Spawn(base.transform, Vector3.zero);
			this.hasLoopEffect = (this.loopEffectObject != null);
			if (this.hasLoopEffect)
			{
				this.loopEffectObject.SetActive(false);
			}
		}
	}

	// Token: 0x0600350B RID: 13579 RVA: 0x000EB770 File Offset: 0x000E9970
	private void StartLoopEffect()
	{
		if (this.hasLoopEffect)
		{
			this.loopEffectObject.gameObject.SetActive(true);
		}
	}

	// Token: 0x0600350C RID: 13580 RVA: 0x000EB78B File Offset: 0x000E998B
	private void StopLoopEffect()
	{
		if (this.hasLoopEffect)
		{
			this.loopEffectObject.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600350D RID: 13581 RVA: 0x000EB7A6 File Offset: 0x000E99A6
	private void CleanLoopEffect()
	{
		if (this.hasLoopEffect)
		{
			Object.Destroy(this.loopEffectObject);
			this.hasLoopEffect = false;
		}
	}

	// Token: 0x04003868 RID: 14440
	[Space]
	[SerializeField]
	private GameObject hitEffectPrefab;

	// Token: 0x04003869 RID: 14441
	[SerializeField]
	private GameObject burstAppearChild;

	// Token: 0x0400386A RID: 14442
	[Space]
	public UnityEvent OnHit;

	// Token: 0x0400386B RID: 14443
	public UnityEvent OnBreak;

	// Token: 0x0400386C RID: 14444
	private DeliveryQuestItem previousItem;

	// Token: 0x0400386D RID: 14445
	private DeliveryQuestItem currentItem;

	// Token: 0x0400386E RID: 14446
	private int maxItemCount;

	// Token: 0x0400386F RID: 14447
	private FullQuestBase currentQuest;

	// Token: 0x04003870 RID: 14448
	private bool isHudOut;

	// Token: 0x04003871 RID: 14449
	private bool queuedAppear;

	// Token: 0x04003872 RID: 14450
	private GameManager gm;

	// Token: 0x04003873 RID: 14451
	private bool started;

	// Token: 0x04003874 RID: 14452
	private bool hasLoopEffect;

	// Token: 0x04003875 RID: 14453
	private GameObject loopEffectObject;

	// Token: 0x04003876 RID: 14454
	private bool hasCustomHitEffect;

	// Token: 0x04003877 RID: 14455
	private GameObject customHitEffectPrefab;
}
