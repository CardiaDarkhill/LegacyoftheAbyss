using System;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200066E RID: 1646
public class GenericMessageCanvas : MonoBehaviour
{
	// Token: 0x170006AD RID: 1709
	// (get) Token: 0x06003B18 RID: 15128 RVA: 0x001047F8 File Offset: 0x001029F8
	public static bool IsActive
	{
		get
		{
			return GenericMessageCanvas._instance && GenericMessageCanvas._instance.isActive;
		}
	}

	// Token: 0x06003B19 RID: 15129 RVA: 0x00104812 File Offset: 0x00102A12
	protected void Awake()
	{
		if (!GenericMessageCanvas._instance)
		{
			GenericMessageCanvas._instance = this;
		}
		this.rootGroup.alpha = 0f;
		this.rootGroup.interactable = false;
		this.rootGroup.blocksRaycasts = false;
	}

	// Token: 0x06003B1A RID: 15130 RVA: 0x0010484E File Offset: 0x00102A4E
	private void OnDestroy()
	{
		if (GenericMessageCanvas._instance == this)
		{
			GenericMessageCanvas._instance = null;
		}
	}

	// Token: 0x06003B1B RID: 15131 RVA: 0x00104863 File Offset: 0x00102A63
	public static void Show(string errorKey, Action onOk)
	{
		GenericMessageCanvas._instance.ShowInternal(errorKey, onOk);
	}

	// Token: 0x06003B1C RID: 15132 RVA: 0x00104874 File Offset: 0x00102A74
	private void ShowInternal(string errorKey, Action onOk)
	{
		this.isActive = true;
		HeroController hc = HeroController.SilentInstance;
		if (hc)
		{
			hc.AddInputBlocker(this);
		}
		InputHandler ih = ManagerSingleton<InputHandler>.Instance;
		bool wasInputActive = ih.acceptingInput;
		GameObject previouslySelected = EventSystem.current.currentSelectedGameObject;
		this.okButtonCallback = delegate()
		{
			EventSystem.current.SetSelectedGameObject(null);
			this.Hide();
			if (!wasInputActive)
			{
				ih.StopUIInput();
			}
			if (previouslySelected)
			{
				EventSystem.current.SetSelectedGameObject(previouslySelected);
			}
			Action onOk2 = onOk;
			if (onOk2 != null)
			{
				onOk2();
			}
			if (hc)
			{
				hc.RemoveInputBlocker(this);
			}
			this.isActive = false;
		};
		this.labelText.text = new LocalisedString("Error", errorKey + "_TITLE");
		this.descText.text = new LocalisedString("Error", errorKey + "_DESC");
		this.rootGroup.alpha = 1f;
		this.rootGroup.interactable = true;
		this.rootGroup.blocksRaycasts = true;
		ih.StartUIInput();
		EventSystem.current.SetSelectedGameObject(this.okButton);
	}

	// Token: 0x06003B1D RID: 15133 RVA: 0x0010498B File Offset: 0x00102B8B
	private void Hide()
	{
		this.rootGroup.alpha = 0f;
		this.rootGroup.interactable = false;
		this.rootGroup.blocksRaycasts = false;
	}

	// Token: 0x06003B1E RID: 15134 RVA: 0x001049B5 File Offset: 0x00102BB5
	public void OkButtonClicked()
	{
		if (this.okButtonCallback == null)
		{
			return;
		}
		this.okButtonCallback();
		this.okButtonCallback = null;
	}

	// Token: 0x04003D6A RID: 15722
	[SerializeField]
	private CanvasGroup rootGroup;

	// Token: 0x04003D6B RID: 15723
	[SerializeField]
	private Text labelText;

	// Token: 0x04003D6C RID: 15724
	[SerializeField]
	private Text descText;

	// Token: 0x04003D6D RID: 15725
	[SerializeField]
	private GameObject okButton;

	// Token: 0x04003D6E RID: 15726
	private Action okButtonCallback;

	// Token: 0x04003D6F RID: 15727
	private bool isActive;

	// Token: 0x04003D70 RID: 15728
	private static GenericMessageCanvas _instance;
}
