using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x020006E2 RID: 1762
public class MenuButtonList : MonoBehaviour
{
	// Token: 0x06003F77 RID: 16247 RVA: 0x001180EC File Offset: 0x001162EC
	private void Awake()
	{
		MenuScreen component = base.GetComponent<MenuScreen>();
		if (component)
		{
			component.defaultHighlight = null;
		}
	}

	// Token: 0x06003F78 RID: 16248 RVA: 0x0011810F File Offset: 0x0011630F
	protected void Start()
	{
		this.SetupActive();
		this.DoSelect();
		this.started = true;
		Platform.OnSaveStoreStateChanged += this.OnSaveStoreStateChanged;
	}

	// Token: 0x06003F79 RID: 16249 RVA: 0x00118135 File Offset: 0x00116335
	private void OnEnable()
	{
		if (this.started)
		{
			if (this.isDirty)
			{
				this.SetupActive();
			}
			this.DoSelect();
		}
	}

	// Token: 0x06003F7A RID: 16250 RVA: 0x00118153 File Offset: 0x00116353
	private void OnSaveStoreStateChanged(bool mounted)
	{
		if (mounted)
		{
			this.isDirty = true;
		}
	}

	// Token: 0x06003F7B RID: 16251 RVA: 0x00118160 File Offset: 0x00116360
	public void SetupActive()
	{
		MenuButtonList._menuButtonLists.Add(this);
		if (this.activeSelectables == null)
		{
			this.activeSelectables = new List<Selectable>();
		}
		else
		{
			this.activeSelectables.Clear();
		}
		foreach (MenuButtonList.Entry entry in this.entries)
		{
			Selectable selectable = entry.Selectable;
			MenuButtonListCondition condition = entry.Condition;
			bool flag = this.skipDisabled && !entry.ForceEnable;
			if (condition == null || condition.IsFulfilledAllComponents())
			{
				if (!flag)
				{
					selectable.gameObject.SetActive(true);
					if (condition)
					{
						condition.OnActiveStateSet(true);
					}
					selectable.interactable = true;
					if (entry.AlsoAffectParent)
					{
						selectable.transform.parent.gameObject.SetActive(true);
					}
				}
				if (!flag || (selectable.gameObject.activeInHierarchy && selectable.interactable))
				{
					this.activeSelectables.Add(selectable);
				}
			}
			else
			{
				condition.OnActiveStateSet(false);
				if (!condition.AlwaysVisible())
				{
					selectable.gameObject.SetActive(false);
				}
				selectable.interactable = false;
				if (entry.AlsoAffectParent)
				{
					selectable.transform.parent.gameObject.SetActive(false);
				}
			}
		}
		for (int j = 0; j < this.activeSelectables.Count; j++)
		{
			Selectable selectable2 = this.activeSelectables[j];
			Selectable selectOnUp = this.activeSelectables[(j + this.activeSelectables.Count - 1) % this.activeSelectables.Count];
			Selectable selectOnDown = this.activeSelectables[(j + 1) % this.activeSelectables.Count];
			Navigation navigation = selectable2.navigation;
			if (navigation.mode == Navigation.Mode.Explicit)
			{
				navigation.selectOnUp = selectOnUp;
				navigation.selectOnDown = selectOnDown;
				selectable2.navigation = navigation;
			}
			if (this.isTopLevelMenu)
			{
				CancelAction cancelAction = (!Platform.Current.WillDisplayQuitButton) ? CancelAction.DoNothing : CancelAction.GoToExitPrompt;
				MenuButton menuButton = selectable2 as MenuButton;
				if (menuButton != null)
				{
					menuButton.cancelAction = cancelAction;
				}
			}
		}
		foreach (Selectable selectable3 in this.activeSelectables)
		{
			MenuSelectable menuSelectable = (MenuSelectable)selectable3;
			menuSelectable.OnSelected += delegate(MenuSelectable self)
			{
				if (!this.isTopLevelMenu)
				{
					Object menuSelectable = menuSelectable;
					List<Selectable> list = this.activeSelectables;
					if (!(menuSelectable != (MenuSelectable)list[list.Count - 1]))
					{
						return;
					}
				}
				this.lastSelected = self;
			};
		}
	}

	// Token: 0x06003F7C RID: 16252 RVA: 0x001183F0 File Offset: 0x001165F0
	private void DoSelect()
	{
		if (this.lastSelected)
		{
			base.StartCoroutine(this.SelectDelayed(this.lastSelected));
			return;
		}
		if (this.activeSelectables != null && this.activeSelectables.Count > 0)
		{
			base.StartCoroutine(this.SelectDelayed(this.activeSelectables[0].GetFirstInteractable()));
		}
	}

	// Token: 0x06003F7D RID: 16253 RVA: 0x00118452 File Offset: 0x00116652
	private void OnDestroy()
	{
		MenuButtonList._menuButtonLists.Remove(this);
		Platform.OnSaveStoreStateChanged -= this.OnSaveStoreStateChanged;
	}

	// Token: 0x06003F7E RID: 16254 RVA: 0x00118471 File Offset: 0x00116671
	private IEnumerator SelectDelayed(Selectable selectable)
	{
		while (!selectable.gameObject.activeInHierarchy)
		{
			yield return null;
		}
		UIManager.HighlightSelectableNoSound(selectable);
		foreach (Animator animator in selectable.GetComponentsInChildren<Animator>())
		{
			if (animator.HasParameter(MenuButtonList._hide, null))
			{
				animator.ResetTrigger(MenuButtonList._hide);
			}
			if (animator.HasParameter(MenuButtonList._show, null))
			{
				animator.SetTrigger(MenuButtonList._show);
			}
		}
		yield break;
	}

	// Token: 0x06003F7F RID: 16255 RVA: 0x00118480 File Offset: 0x00116680
	public void ClearLastSelected()
	{
		this.lastSelected = null;
	}

	// Token: 0x06003F80 RID: 16256 RVA: 0x0011848C File Offset: 0x0011668C
	public static void ClearAllLastSelected()
	{
		foreach (MenuButtonList menuButtonList in MenuButtonList._menuButtonLists)
		{
			menuButtonList.ClearLastSelected();
		}
	}

	// Token: 0x04004132 RID: 16690
	[SerializeField]
	private MenuButtonList.Entry[] entries;

	// Token: 0x04004133 RID: 16691
	[SerializeField]
	private bool isTopLevelMenu;

	// Token: 0x04004134 RID: 16692
	[SerializeField]
	private bool skipDisabled;

	// Token: 0x04004135 RID: 16693
	private MenuSelectable lastSelected;

	// Token: 0x04004136 RID: 16694
	private List<Selectable> activeSelectables;

	// Token: 0x04004137 RID: 16695
	private static readonly HashSet<MenuButtonList> _menuButtonLists = new HashSet<MenuButtonList>();

	// Token: 0x04004138 RID: 16696
	private bool started;

	// Token: 0x04004139 RID: 16697
	private static readonly int _hide = Animator.StringToHash("hide");

	// Token: 0x0400413A RID: 16698
	private static readonly int _show = Animator.StringToHash("show");

	// Token: 0x0400413B RID: 16699
	private bool isDirty;

	// Token: 0x020019D7 RID: 6615
	[Serializable]
	private class Entry
	{
		// Token: 0x170010CC RID: 4300
		// (get) Token: 0x06009549 RID: 38217 RVA: 0x002A5066 File Offset: 0x002A3266
		public Selectable Selectable
		{
			get
			{
				return this.selectable;
			}
		}

		// Token: 0x170010CD RID: 4301
		// (get) Token: 0x0600954A RID: 38218 RVA: 0x002A506E File Offset: 0x002A326E
		public MenuButtonListCondition Condition
		{
			get
			{
				return this.condition;
			}
		}

		// Token: 0x170010CE RID: 4302
		// (get) Token: 0x0600954B RID: 38219 RVA: 0x002A5076 File Offset: 0x002A3276
		public bool ForceEnable
		{
			get
			{
				return this.forceEnable;
			}
		}

		// Token: 0x170010CF RID: 4303
		// (get) Token: 0x0600954C RID: 38220 RVA: 0x002A507E File Offset: 0x002A327E
		public bool AlsoAffectParent
		{
			get
			{
				return this.alsoAffectParent;
			}
		}

		// Token: 0x04009755 RID: 38741
		[SerializeField]
		[FormerlySerializedAs("button")]
		private Selectable selectable;

		// Token: 0x04009756 RID: 38742
		[SerializeField]
		private MenuButtonListCondition condition;

		// Token: 0x04009757 RID: 38743
		[SerializeField]
		private bool alsoAffectParent;

		// Token: 0x04009758 RID: 38744
		[SerializeField]
		private bool forceEnable;
	}
}
