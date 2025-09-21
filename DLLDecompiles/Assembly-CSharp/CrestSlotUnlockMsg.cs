using System;
using System.Collections;
using GlobalSettings;
using UnityEngine;

// Token: 0x020005D3 RID: 1491
public class CrestSlotUnlockMsg : UIMsgPopupBase<CrestSlotUnlockMsg.SlotUnlockPopupInfo, CrestSlotUnlockMsg>
{
	// Token: 0x060034F4 RID: 13556 RVA: 0x000EB078 File Offset: 0x000E9278
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<CrestSlotUnlockMsg.SlotIcons>(ref this.slotTypeIcons, typeof(ToolItemType));
		for (int i = 0; i < this.slotTypeIcons.Length; i++)
		{
			CrestSlotUnlockMsg.SlotIcons slotIcons = this.slotTypeIcons[i];
			ArrayForEnumAttribute.EnsureArraySize<RuntimeAnimatorController>(ref slotIcons.Icons, typeof(AttackToolBinding));
			this.slotTypeIcons[i] = slotIcons;
		}
	}

	// Token: 0x060034F5 RID: 13557 RVA: 0x000EB0DD File Offset: 0x000E92DD
	private void Awake()
	{
		this.OnValidate();
	}

	// Token: 0x060034F6 RID: 13558 RVA: 0x000EB0E8 File Offset: 0x000E92E8
	public static void Spawn(ToolItemType toolType, AttackToolBinding attackBinding)
	{
		CrestSlotUnlockMsg crestSlotUnlockMsgPrefab = UI.CrestSlotUnlockMsgPrefab;
		if (!crestSlotUnlockMsgPrefab)
		{
			return;
		}
		UIMsgPopupBase<CrestSlotUnlockMsg.SlotUnlockPopupInfo, CrestSlotUnlockMsg>.SpawnInternal(crestSlotUnlockMsgPrefab, new CrestSlotUnlockMsg.SlotUnlockPopupInfo
		{
			SlotType = toolType,
			AttackBinding = (toolType.IsAttackType() ? attackBinding : AttackToolBinding.Neutral)
		}, null, false);
	}

	// Token: 0x060034F7 RID: 13559 RVA: 0x000EB134 File Offset: 0x000E9334
	protected override void UpdateDisplay(CrestSlotUnlockMsg.SlotUnlockPopupInfo item)
	{
		if (this.icon)
		{
			this.icon.color = UI.GetToolTypeColor(item.SlotType);
		}
		if (this.animator)
		{
			RuntimeAnimatorController runtimeAnimatorController = this.slotTypeIcons[(int)item.SlotType].Icons[(int)item.AttackBinding];
			this.animator.runtimeAnimatorController = runtimeAnimatorController;
			if (this.animateRoutine != null)
			{
				base.StopCoroutine(this.animateRoutine);
				this.animateRoutine = null;
			}
			this.animator.enabled = true;
			this.animator.Play("Unequip", 0, 0f);
			if (this.iconAnimateDelay > 0f)
			{
				this.animateRoutine = base.StartCoroutine(this.AnimateDelayed());
			}
		}
	}

	// Token: 0x060034F8 RID: 13560 RVA: 0x000EB1FD File Offset: 0x000E93FD
	private IEnumerator AnimateDelayed()
	{
		yield return null;
		this.animator.enabled = false;
		yield return new WaitForSeconds(this.iconAnimateDelay);
		this.animator.enabled = true;
		this.animator.Play("Unequip", 0, 0f);
		this.animateRoutine = null;
		yield break;
	}

	// Token: 0x04003863 RID: 14435
	[Space]
	[SerializeField]
	private SpriteRenderer icon;

	// Token: 0x04003864 RID: 14436
	[SerializeField]
	private Animator animator;

	// Token: 0x04003865 RID: 14437
	[SerializeField]
	private float iconAnimateDelay;

	// Token: 0x04003866 RID: 14438
	[SerializeField]
	[ArrayForEnum(typeof(ToolItemType))]
	private CrestSlotUnlockMsg.SlotIcons[] slotTypeIcons;

	// Token: 0x04003867 RID: 14439
	private Coroutine animateRoutine;

	// Token: 0x020018E0 RID: 6368
	public struct SlotUnlockPopupInfo : IUIMsgPopupItem
	{
		// Token: 0x17001048 RID: 4168
		// (get) Token: 0x06009293 RID: 37523 RVA: 0x0029C3B8 File Offset: 0x0029A5B8
		// (set) Token: 0x06009294 RID: 37524 RVA: 0x0029C3C0 File Offset: 0x0029A5C0
		public ToolItemType SlotType { readonly get; set; }

		// Token: 0x17001049 RID: 4169
		// (get) Token: 0x06009295 RID: 37525 RVA: 0x0029C3C9 File Offset: 0x0029A5C9
		// (set) Token: 0x06009296 RID: 37526 RVA: 0x0029C3D1 File Offset: 0x0029A5D1
		public AttackToolBinding AttackBinding { readonly get; set; }

		// Token: 0x06009297 RID: 37527 RVA: 0x0029C3DA File Offset: 0x0029A5DA
		public Object GetRepresentingObject()
		{
			return null;
		}
	}

	// Token: 0x020018E1 RID: 6369
	[Serializable]
	private struct SlotIcons
	{
		// Token: 0x04009395 RID: 37781
		[ArrayForEnum(typeof(AttackToolBinding))]
		public RuntimeAnimatorController[] Icons;
	}
}
