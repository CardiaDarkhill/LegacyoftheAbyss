using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000686 RID: 1670
public abstract class InventoryItemButtonPromptBase<TData> : MonoBehaviour, InventoryItemUpdateable.IIsSeenProvider
{
	// Token: 0x170006BD RID: 1725
	// (get) Token: 0x06003BA2 RID: 15266 RVA: 0x0010653B File Offset: 0x0010473B
	// (set) Token: 0x06003BA3 RID: 15267 RVA: 0x0010655C File Offset: 0x0010475C
	public bool IsSeen
	{
		get
		{
			return string.IsNullOrEmpty(this.hasSeenPdBool) || PlayerData.instance.GetVariable(this.hasSeenPdBool);
		}
		set
		{
			if (string.IsNullOrEmpty(this.hasSeenPdBool))
			{
				return;
			}
			PlayerData.instance.SetVariable(this.hasSeenPdBool, value);
		}
	}

	// Token: 0x06003BA4 RID: 15268 RVA: 0x00106580 File Offset: 0x00104780
	private void OnEnable()
	{
		if (!this.appearCondition.IsFulfilled)
		{
			return;
		}
		if (this.extraDescriptionOverride && this.extraDescriptionOverride.WillDisplay)
		{
			return;
		}
		InventoryItemSelectable component = base.GetComponent<InventoryItemSelectable>();
		if (!component)
		{
			return;
		}
		component.OnSelected += this.Show;
		component.OnDeselected += this.Hide;
		InventoryItemUpdateable component2 = base.GetComponent<InventoryItemUpdateable>();
		if (component2)
		{
			component2.RegisterIsSeenProvider(this);
		}
	}

	// Token: 0x06003BA5 RID: 15269 RVA: 0x00106600 File Offset: 0x00104800
	private void OnDisable()
	{
		InventoryItemUpdateable component = base.GetComponent<InventoryItemUpdateable>();
		if (component)
		{
			component.DeregisterIsSeenProvider(this);
		}
		InventoryItemSelectable component2 = base.GetComponent<InventoryItemSelectable>();
		if (!component2)
		{
			return;
		}
		component2.OnSelected -= this.Show;
		component2.OnDeselected -= this.Hide;
		this.Hide(component2);
	}

	// Token: 0x06003BA6 RID: 15270 RVA: 0x0010665E File Offset: 0x0010485E
	private void Hide(InventoryItemSelectable selectable)
	{
		if (this.display)
		{
			this.display.Clear();
		}
	}

	// Token: 0x06003BA7 RID: 15271 RVA: 0x00106678 File Offset: 0x00104878
	private void Show(InventoryItemSelectable selectable)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.display)
		{
			this.OnShow(this.display, this.data);
		}
	}

	// Token: 0x06003BA8 RID: 15272 RVA: 0x001066A2 File Offset: 0x001048A2
	public void SetData(TData newData)
	{
		this.data = newData;
	}

	// Token: 0x06003BA9 RID: 15273
	protected abstract void OnShow(InventoryItemButtonPromptDisplayList displayList, TData data);

	// Token: 0x04003DD7 RID: 15831
	[SerializeField]
	protected InventoryItemButtonPromptDisplayList display;

	// Token: 0x04003DD8 RID: 15832
	[SerializeField]
	private TData data;

	// Token: 0x04003DD9 RID: 15833
	[SerializeField]
	private PlayerDataTest appearCondition;

	// Token: 0x04003DDA RID: 15834
	[SerializeField]
	private InventoryItemExtraDescription extraDescriptionOverride;

	// Token: 0x04003DDB RID: 15835
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string hasSeenPdBool;

	// Token: 0x04003DDC RID: 15836
	[SerializeField]
	protected int order;
}
