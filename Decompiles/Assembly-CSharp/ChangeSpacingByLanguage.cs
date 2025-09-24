using System;
using TMProOld;
using UnityEngine;

// Token: 0x02000409 RID: 1033
public sealed class ChangeSpacingByLanguage : ChangeByLanguageOld<ChangeSpacingByLanguage.SpaceOverride>
{
	// Token: 0x06002318 RID: 8984 RVA: 0x000A0A1E File Offset: 0x0009EC1E
	protected override void DoAwake()
	{
		this.hasText = (this.text != null);
	}

	// Token: 0x06002319 RID: 8985 RVA: 0x000A0A32 File Offset: 0x0009EC32
	protected override void OnValidate()
	{
		base.OnValidate();
		if (this.text == null)
		{
			this.text = base.GetComponent<TextMeshPro>();
		}
		this.hasText = (this.text != null);
	}

	// Token: 0x0600231A RID: 8986 RVA: 0x000A0A66 File Offset: 0x0009EC66
	protected override void RecordOriginalValues()
	{
		if (this.recorded)
		{
			return;
		}
		if (this.hasText)
		{
			this.originalValue = this.text.lineSpacing;
			this.recorded = true;
		}
	}

	// Token: 0x0600231B RID: 8987 RVA: 0x000A0A91 File Offset: 0x0009EC91
	protected override void DoRevertValues()
	{
		if (!this.hasText)
		{
			return;
		}
		this.text.lineSpacing = this.originalValue;
	}

	// Token: 0x0600231C RID: 8988 RVA: 0x000A0AAD File Offset: 0x0009ECAD
	protected override void ApplySetting(ChangeSpacingByLanguage.SpaceOverride setting)
	{
		if (!this.hasText)
		{
			return;
		}
		this.text.lineSpacing = setting.lineSpacing;
	}

	// Token: 0x040021C8 RID: 8648
	[SerializeField]
	private TextMeshPro text;

	// Token: 0x040021C9 RID: 8649
	private float originalValue;

	// Token: 0x040021CA RID: 8650
	private bool hasText;

	// Token: 0x020016A0 RID: 5792
	[Serializable]
	public sealed class SpaceOverride : ChangeByLanguageOld<ChangeSpacingByLanguage.SpaceOverride>.LanguageOverride
	{
		// Token: 0x04008B8F RID: 35727
		public float lineSpacing;
	}
}
