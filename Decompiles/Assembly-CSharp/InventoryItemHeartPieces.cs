using System;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x02000690 RID: 1680
public class InventoryItemHeartPieces : InventoryItemSelectableDirectional
{
	// Token: 0x170006CA RID: 1738
	// (get) Token: 0x06003BFE RID: 15358 RVA: 0x001082C8 File Offset: 0x001064C8
	public override string DisplayName
	{
		get
		{
			return this.currentState.DisplayName;
		}
	}

	// Token: 0x170006CB RID: 1739
	// (get) Token: 0x06003BFF RID: 15359 RVA: 0x001082DA File Offset: 0x001064DA
	public override string Description
	{
		get
		{
			return this.currentState.Description;
		}
	}

	// Token: 0x06003C00 RID: 15360 RVA: 0x001082EC File Offset: 0x001064EC
	protected override void Awake()
	{
		base.Awake();
		InventoryPane componentInParent = base.GetComponentInParent<InventoryPane>();
		if (componentInParent)
		{
			componentInParent.OnPaneStart += this.UpdateState;
		}
	}

	// Token: 0x06003C01 RID: 15361 RVA: 0x00108320 File Offset: 0x00106520
	protected override void Start()
	{
		base.Start();
		this.UpdateState();
	}

	// Token: 0x06003C02 RID: 15362 RVA: 0x00108330 File Offset: 0x00106530
	private void UpdateState()
	{
		PlayerData instance = PlayerData.instance;
		int num = instance.heartPieces;
		if (num <= 0 && instance.maxHealthBase <= 5)
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
			}
			return;
		}
		bool flag = false;
		if (instance.maxHealthBase >= 10)
		{
			num = 4;
			flag = true;
		}
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
		}
		InventoryItemHeartPieces.DisplayState[] array = this.displayStates;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].DisplayObject.SetActive(false);
		}
		int num2 = this.displayStates.Length - 1;
		for (int j = 0; j < this.displayStates.Length; j++)
		{
			InventoryItemHeartPieces.DisplayState displayState = this.displayStates[j];
			if (!flag)
			{
				if (j <= num)
				{
					displayState.DisplayObject.SetActive(true);
				}
			}
			else if (j == num2)
			{
				displayState.DisplayObject.SetActive(true);
				this.currentState = displayState;
			}
			if (j == num)
			{
				this.currentState = displayState;
			}
			displayState.DisplayObject.transform.localScale = ((num == 1) ? new Vector3(-1f, 1f, 1f) : Vector3.one);
		}
	}

	// Token: 0x04003E22 RID: 15906
	[Space]
	[SerializeField]
	private InventoryItemHeartPieces.DisplayState[] displayStates;

	// Token: 0x04003E23 RID: 15907
	private InventoryItemHeartPieces.DisplayState currentState;

	// Token: 0x02001996 RID: 6550
	[Serializable]
	private struct DisplayState
	{
		// Token: 0x04009656 RID: 38486
		public GameObject DisplayObject;

		// Token: 0x04009657 RID: 38487
		public LocalisedString DisplayName;

		// Token: 0x04009658 RID: 38488
		public LocalisedString Description;
	}
}
