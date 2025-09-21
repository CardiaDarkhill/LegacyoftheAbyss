using System;
using TeamCherry.SharedUtils;
using TMProOld;
using UnityEngine;

// Token: 0x02000406 RID: 1030
public class ChangeFontStyleByLanguage : ChangeByLanguageNew<ChangeFontStyleByLanguage.OverrideSetting>
{
	// Token: 0x06002300 RID: 8960 RVA: 0x000A0362 File Offset: 0x0009E562
	protected override void DoAwake()
	{
		base.DoAwake();
		this.hasTextMeshPro = (this.textMeshPro != null);
		if (!this.hasTextMeshPro)
		{
			this.textMeshPro = base.GetComponent<TextMeshPro>();
			this.hasTextMeshPro = (this.textMeshPro != null);
		}
	}

	// Token: 0x06002301 RID: 8961 RVA: 0x000A03A2 File Offset: 0x0009E5A2
	protected override void OnValidate()
	{
		base.OnValidate();
		if (this.textMeshPro == null)
		{
			this.textMeshPro = base.GetComponent<TextMeshPro>();
		}
		this.hasTextMeshPro = (this.textMeshPro != null);
	}

	// Token: 0x06002302 RID: 8962 RVA: 0x000A03D6 File Offset: 0x0009E5D6
	protected override void RecordOriginalValues()
	{
		if (this.recorded)
		{
			return;
		}
		if (this.hasTextMeshPro)
		{
			this.originalFontStyle = this.textMeshPro.fontStyle;
			this.recorded = true;
		}
	}

	// Token: 0x06002303 RID: 8963 RVA: 0x000A0401 File Offset: 0x0009E601
	protected override void DoRevertValues()
	{
		if (this.hasTextMeshPro)
		{
			this.textMeshPro.fontStyle = this.originalFontStyle;
		}
	}

	// Token: 0x06002304 RID: 8964 RVA: 0x000A041C File Offset: 0x0009E61C
	protected override void ApplySetting(ChangeFontStyleByLanguage.OverrideSetting setting)
	{
		if (this.hasTextMeshPro && setting.fontStyle.IsEnabled)
		{
			this.textMeshPro.fontStyle = setting.fontStyle.Value;
		}
	}

	// Token: 0x040021BA RID: 8634
	[SerializeField]
	private TextMeshPro textMeshPro;

	// Token: 0x040021BB RID: 8635
	private bool hasTextMeshPro;

	// Token: 0x040021BC RID: 8636
	private FontStyles originalFontStyle;

	// Token: 0x02001699 RID: 5785
	[Serializable]
	public class OverrideStyle : OverrideMaskValue<FontStyles>
	{
	}

	// Token: 0x0200169A RID: 5786
	[Serializable]
	public class OverrideSetting : ChangeByLanguageNew<ChangeFontStyleByLanguage.OverrideSetting>.OverrideValue
	{
		// Token: 0x04008B7F RID: 35711
		public ChangeFontStyleByLanguage.OverrideStyle fontStyle;
	}
}
