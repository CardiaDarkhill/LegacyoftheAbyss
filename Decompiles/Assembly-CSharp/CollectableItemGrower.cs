using System;
using System.Collections.Generic;
using System.Linq;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020001AC RID: 428
[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (Grower)")]
public class CollectableItemGrower : CollectableItem
{
	// Token: 0x170001B1 RID: 433
	// (get) Token: 0x060010A1 RID: 4257 RVA: 0x0004F1CD File Offset: 0x0004D3CD
	public override bool DisplayAmount
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170001B2 RID: 434
	// (get) Token: 0x060010A2 RID: 4258 RVA: 0x0004F1D0 File Offset: 0x0004D3D0
	public override bool TakeItemOnConsume
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060010A3 RID: 4259 RVA: 0x0004F1D3 File Offset: 0x0004D3D3
	private void OnValidate()
	{
		if (this.states == null || this.states.Length == 0)
		{
			this.states = new CollectableItemGrower.ItemState[1];
		}
	}

	// Token: 0x060010A4 RID: 4260 RVA: 0x0004F1F2 File Offset: 0x0004D3F2
	private void Awake()
	{
		this.OnValidate();
	}

	// Token: 0x060010A5 RID: 4261 RVA: 0x0004F1FA File Offset: 0x0004D3FA
	public override string GetDisplayName(CollectableItem.ReadSource readSource)
	{
		return this.GetState(readSource).DisplayName;
	}

	// Token: 0x060010A6 RID: 4262 RVA: 0x0004F20D File Offset: 0x0004D40D
	public override string GetDescription(CollectableItem.ReadSource readSource)
	{
		return this.GetState(readSource).Description;
	}

	// Token: 0x060010A7 RID: 4263 RVA: 0x0004F220 File Offset: 0x0004D420
	public override Sprite GetIcon(CollectableItem.ReadSource readSource)
	{
		return this.GetState(readSource).Icon;
	}

	// Token: 0x060010A8 RID: 4264 RVA: 0x0004F230 File Offset: 0x0004D430
	private CollectableItemGrower.ItemState GetState(CollectableItem.ReadSource readSource)
	{
		if (readSource != CollectableItem.ReadSource.GetPopup && readSource != CollectableItem.ReadSource.Shop)
		{
			return this.GetCurrentState();
		}
		if (this.states.Length == 0)
		{
			return default(CollectableItemGrower.ItemState);
		}
		CollectableItemGrower.ItemState[] array = this.states;
		return array[array.Length - 1];
	}

	// Token: 0x060010A9 RID: 4265 RVA: 0x0004F270 File Offset: 0x0004D470
	private CollectableItemGrower.ItemState GetCurrentState()
	{
		if (!Application.isPlaying)
		{
			return this.states[0];
		}
		int num = PlayerData.instance.GetInt(this.growStatePdInt);
		if (num < 0)
		{
			num = 0;
		}
		else if (num >= this.states.Length)
		{
			num = this.states.Length - 1;
		}
		return this.states[num];
	}

	// Token: 0x060010AA RID: 4266 RVA: 0x0004F2CC File Offset: 0x0004D4CC
	protected override IEnumerable<CollectableItem.UseResponse> GetUseResponses()
	{
		return base.GetUseResponses().Concat(this.GetCurrentState().UseResponses);
	}

	// Token: 0x060010AB RID: 4267 RVA: 0x0004F2E4 File Offset: 0x0004D4E4
	public override void ConsumeItemResponse()
	{
		base.ConsumeItemResponse();
		PlayerData.instance.SetInt(this.growStatePdInt, 0);
	}

	// Token: 0x060010AC RID: 4268 RVA: 0x0004F2FD File Offset: 0x0004D4FD
	public override bool IsConsumable()
	{
		return true;
	}

	// Token: 0x060010AD RID: 4269 RVA: 0x0004F300 File Offset: 0x0004D500
	protected override void OnCollected()
	{
		PlayerData.instance.SetInt(this.growStatePdInt, this.states.Length - 1);
	}

	// Token: 0x04000FEA RID: 4074
	[Space]
	[SerializeField]
	[PlayerDataField(typeof(int), true)]
	private string growStatePdInt;

	// Token: 0x04000FEB RID: 4075
	[SerializeField]
	private CollectableItemGrower.ItemState[] states;

	// Token: 0x020014E8 RID: 5352
	[Serializable]
	private struct ItemState
	{
		// Token: 0x0400852E RID: 34094
		public LocalisedString DisplayName;

		// Token: 0x0400852F RID: 34095
		public LocalisedString Description;

		// Token: 0x04008530 RID: 34096
		public Sprite Icon;

		// Token: 0x04008531 RID: 34097
		[SerializeField]
		public CollectableItem.UseResponse[] UseResponses;
	}
}
