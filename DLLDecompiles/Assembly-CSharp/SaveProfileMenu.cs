using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000710 RID: 1808
public class SaveProfileMenu : MonoBehaviour
{
	// Token: 0x06004083 RID: 16515 RVA: 0x0011BCDE File Offset: 0x00119EDE
	private void Start()
	{
		this.ui = UIManager.instance;
	}

	// Token: 0x06004084 RID: 16516 RVA: 0x0011BCEB File Offset: 0x00119EEB
	public void BackAction()
	{
		if (RestoreSaveButton.GoBack())
		{
			return;
		}
		this.ui.UIGoToMainMenu();
	}

	// Token: 0x04004217 RID: 16919
	private UIManager ui;
}
