using System;
using GlobalEnums;
using InControl;
using UnityEngine;

// Token: 0x02000605 RID: 1541
[RequireComponent(typeof(SpriteRenderer))]
public class ActionButtonIcon : ActionButtonIconBase
{
	// Token: 0x17000653 RID: 1619
	// (get) Token: 0x06003704 RID: 14084 RVA: 0x000F2B3B File Offset: 0x000F0D3B
	public override HeroActionButton Action
	{
		get
		{
			return this.action;
		}
	}

	// Token: 0x06003705 RID: 14085 RVA: 0x000F2B43 File Offset: 0x000F0D43
	protected override void OnEnable()
	{
		base.OnIconUpdate += this.CheckHideIcon;
		base.OnEnable();
	}

	// Token: 0x06003706 RID: 14086 RVA: 0x000F2B5D File Offset: 0x000F0D5D
	protected override void OnDisable()
	{
		base.OnIconUpdate -= this.CheckHideIcon;
		base.OnDisable();
	}

	// Token: 0x06003707 RID: 14087 RVA: 0x000F2B78 File Offset: 0x000F0D78
	private void CheckHideIcon()
	{
		if (this.showForControllerOnly && this.sr)
		{
			if (this.initialScale == null)
			{
				this.initialScale = new Vector3?(base.transform.localScale);
			}
			InputHandler instance = ManagerSingleton<InputHandler>.Instance;
			if (instance.lastActiveController == BindingSourceType.KeyBindingSource || instance.lastActiveController == BindingSourceType.None)
			{
				base.transform.localScale = Vector3.zero;
				return;
			}
			if (instance.lastActiveController == BindingSourceType.DeviceBindingSource)
			{
				base.transform.localScale = this.initialScale.Value;
			}
		}
	}

	// Token: 0x06003708 RID: 14088 RVA: 0x000F2C04 File Offset: 0x000F0E04
	public void SetAction(HeroActionButton action)
	{
		this.action = action;
		base.GetButtonIcon(action);
	}

	// Token: 0x06003709 RID: 14089 RVA: 0x000F2C14 File Offset: 0x000F0E14
	public void SetActionString(string action)
	{
		object obj = Enum.Parse(typeof(HeroActionButton), action);
		if (obj != null)
		{
			this.action = (HeroActionButton)obj;
			base.GetButtonIcon((HeroActionButton)obj);
			return;
		}
		Debug.LogError("SetAction couldn't convert " + action);
	}

	// Token: 0x040039CF RID: 14799
	[SerializeField]
	private HeroActionButton action;

	// Token: 0x040039D0 RID: 14800
	public bool showForControllerOnly;

	// Token: 0x040039D1 RID: 14801
	private Vector3? initialScale;
}
