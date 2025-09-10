using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020006EA RID: 1770
public class MenuScreen : MonoBehaviour
{
	// Token: 0x17000747 RID: 1863
	// (get) Token: 0x06003F96 RID: 16278 RVA: 0x00118669 File Offset: 0x00116869
	public CanvasGroup ScreenCanvasGroup
	{
		get
		{
			return base.GetComponent<CanvasGroup>();
		}
	}

	// Token: 0x17000748 RID: 1864
	// (get) Token: 0x06003F97 RID: 16279 RVA: 0x00118671 File Offset: 0x00116871
	public MenuScreen.HighlightDefaultBehaviours HighlightBehaviour
	{
		get
		{
			return this.highlightBehaviour;
		}
	}

	// Token: 0x06003F98 RID: 16280 RVA: 0x0011867C File Offset: 0x0011687C
	public void HighlightDefault()
	{
		EventSystem current = EventSystem.current;
		if (this.defaultHighlight == null)
		{
			return;
		}
		if (current.currentSelectedGameObject != null)
		{
			return;
		}
		Selectable firstInteractable = this.defaultHighlight.GetFirstInteractable();
		if (!firstInteractable)
		{
			return;
		}
		UIManager.HighlightSelectableNoSound(firstInteractable);
		foreach (object obj in this.defaultHighlight.transform)
		{
			Animator component = ((Transform)obj).GetComponent<Animator>();
			if (!(component == null))
			{
				component.ResetTrigger(MenuScreen._hidePropId);
				component.SetTrigger(MenuScreen._showPropId);
				break;
			}
		}
	}

	// Token: 0x06003F99 RID: 16281 RVA: 0x0011873C File Offset: 0x0011693C
	public bool GoBack()
	{
		if (!this.backButton)
		{
			return false;
		}
		this.backButton.SkipNextFlashEffect = true;
		this.backButton.SkipNextSubmitSound = true;
		return ExecuteEvents.Execute<ISubmitHandler>(this.backButton.gameObject, null, ExecuteEvents.submitHandler);
	}

	// Token: 0x04004140 RID: 16704
	public Animator topFleur;

	// Token: 0x04004141 RID: 16705
	public Animator bottomFleur;

	// Token: 0x04004142 RID: 16706
	public MenuButton backButton;

	// Token: 0x04004143 RID: 16707
	public Selectable defaultHighlight;

	// Token: 0x04004144 RID: 16708
	[SerializeField]
	private MenuScreen.HighlightDefaultBehaviours highlightBehaviour;

	// Token: 0x04004145 RID: 16709
	private static readonly int _hidePropId = Animator.StringToHash("hide");

	// Token: 0x04004146 RID: 16710
	private static readonly int _showPropId = Animator.StringToHash("show");

	// Token: 0x020019DB RID: 6619
	public enum HighlightDefaultBehaviours
	{
		// Token: 0x04009761 RID: 38753
		AfterFade,
		// Token: 0x04009762 RID: 38754
		BeforeFade
	}
}
