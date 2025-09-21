using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200072F RID: 1839
[DisallowMultipleComponent]
public sealed class SliderRightStickInput : MonoBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler
{
	// Token: 0x060041B8 RID: 16824 RVA: 0x0012141C File Offset: 0x0011F61C
	private void Start()
	{
		if (!this.isSelected)
		{
			this.SetSelected(EventSystem.current.currentSelectedGameObject == base.gameObject);
		}
		bool flag;
		ManagerSingleton<InputHandler>.Instance.GetSticksInput(out flag);
	}

	// Token: 0x060041B9 RID: 16825 RVA: 0x00121459 File Offset: 0x0011F659
	private void OnValidate()
	{
		if (this.slider == null)
		{
			this.slider = base.GetComponent<Slider>();
		}
	}

	// Token: 0x060041BA RID: 16826 RVA: 0x00121478 File Offset: 0x0011F678
	private void Update()
	{
		if (this.hasInputHandler)
		{
			bool flag;
			Vector2 sticksInput = this.ih.GetSticksInput(out flag);
			if (flag && Mathf.Abs(sticksInput.x) >= this.threshold)
			{
				if (sticksInput.x < 0f)
				{
					this.slider.value = this.slider.minValue;
					return;
				}
				this.slider.value = this.slider.maxValue;
			}
		}
	}

	// Token: 0x060041BB RID: 16827 RVA: 0x001214EB File Offset: 0x0011F6EB
	private void OnDisable()
	{
		this.isSelected = false;
	}

	// Token: 0x060041BC RID: 16828 RVA: 0x001214F4 File Offset: 0x0011F6F4
	private void SetSelected(bool isSelected)
	{
		this.isSelected = isSelected;
		if (isSelected)
		{
			this.ih = ManagerSingleton<InputHandler>.Instance;
			this.hasInputHandler = (this.ih != null);
		}
	}

	// Token: 0x060041BD RID: 16829 RVA: 0x0012151D File Offset: 0x0011F71D
	public void OnSelect(BaseEventData eventData)
	{
		this.SetSelected(true);
	}

	// Token: 0x060041BE RID: 16830 RVA: 0x00121526 File Offset: 0x0011F726
	public void OnDeselect(BaseEventData eventData)
	{
		this.SetSelected(false);
	}

	// Token: 0x04004346 RID: 17222
	[SerializeField]
	private Slider slider;

	// Token: 0x04004347 RID: 17223
	[SerializeField]
	private float threshold = 0.7f;

	// Token: 0x04004348 RID: 17224
	private bool isSelected;

	// Token: 0x04004349 RID: 17225
	private bool hasInputHandler;

	// Token: 0x0400434A RID: 17226
	private InputHandler ih;
}
