using System;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000410 RID: 1040
public class VibrationLocalisation : MonoBehaviour
{
	// Token: 0x06002338 RID: 9016 RVA: 0x000A10EC File Offset: 0x0009F2EC
	private void Awake()
	{
		this.hasTextField = (this.textField != null);
		if (!this.hasTextField)
		{
			this.textField = base.gameObject.GetComponent<Text>();
			this.hasTextField = (this.textField != null);
			if (!this.hasTextField)
			{
				base.enabled = false;
				return;
			}
		}
		this.textAligner = base.GetComponent<FixVerticalAlign>();
		if (this.textAligner)
		{
			this.hasTextAligner = true;
		}
	}

	// Token: 0x06002339 RID: 9017 RVA: 0x000A1166 File Offset: 0x0009F366
	private void OnEnable()
	{
		this.gm = GameManager.instance;
		if (this.gm)
		{
			this.gm.RefreshLanguageText += this.UpdateText;
		}
		this.UpdateText();
	}

	// Token: 0x0600233A RID: 9018 RVA: 0x000A119D File Offset: 0x0009F39D
	private void OnDisable()
	{
		if (this.gm != null)
		{
			this.gm.RefreshLanguageText -= this.UpdateText;
		}
	}

	// Token: 0x0600233B RID: 9019 RVA: 0x000A11C4 File Offset: 0x0009F3C4
	private void UpdateText()
	{
		if (this.hasTextField)
		{
			LocalisedString s = this.vibrationLocalisation;
			this.textField.text = s;
			if (this.hasTextAligner)
			{
				this.textAligner.AlignText();
			}
		}
	}

	// Token: 0x040021DF RID: 8671
	[SerializeField]
	[Tooltip("UI Text component to place text.")]
	private Text textField;

	// Token: 0x040021E0 RID: 8672
	[SerializeField]
	private LocalisedString rumbleLocalisation = new LocalisedString
	{
		Sheet = "MainMenu",
		Key = "GAME_CONTROLLER_RUMBLE"
	};

	// Token: 0x040021E1 RID: 8673
	[SerializeField]
	private LocalisedString vibrationLocalisation = new LocalisedString
	{
		Sheet = "MainMenu",
		Key = "GAME_CONTROLLER_VIBRATION"
	};

	// Token: 0x040021E2 RID: 8674
	private GameManager gm;

	// Token: 0x040021E3 RID: 8675
	private FixVerticalAlign textAligner;

	// Token: 0x040021E4 RID: 8676
	private bool hasTextAligner;

	// Token: 0x040021E5 RID: 8677
	private bool hasTextField;
}
