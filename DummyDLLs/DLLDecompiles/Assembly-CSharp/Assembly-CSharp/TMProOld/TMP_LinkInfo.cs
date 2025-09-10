using System;

namespace TMProOld
{
	// Token: 0x020007FB RID: 2043
	public struct TMP_LinkInfo
	{
		// Token: 0x060047D7 RID: 18391 RVA: 0x0014DEB0 File Offset: 0x0014C0B0
		internal void SetLinkID(char[] text, int startIndex, int length)
		{
			if (this.linkID == null || this.linkID.Length < length)
			{
				this.linkID = new char[length];
			}
			for (int i = 0; i < length; i++)
			{
				this.linkID[i] = text[startIndex + i];
			}
		}

		// Token: 0x060047D8 RID: 18392 RVA: 0x0014DEF8 File Offset: 0x0014C0F8
		public string GetLinkText()
		{
			string text = string.Empty;
			TMP_TextInfo textInfo = this.textComponent.textInfo;
			for (int i = this.linkTextfirstCharacterIndex; i < this.linkTextfirstCharacterIndex + this.linkTextLength; i++)
			{
				text += textInfo.characterInfo[i].character.ToString();
			}
			return text;
		}

		// Token: 0x060047D9 RID: 18393 RVA: 0x0014DF52 File Offset: 0x0014C152
		public string GetLinkID()
		{
			if (this.textComponent == null)
			{
				return string.Empty;
			}
			return new string(this.linkID, 0, this.linkIdLength);
		}

		// Token: 0x040047F8 RID: 18424
		public TMP_Text textComponent;

		// Token: 0x040047F9 RID: 18425
		public int hashCode;

		// Token: 0x040047FA RID: 18426
		public int linkIdFirstCharacterIndex;

		// Token: 0x040047FB RID: 18427
		public int linkIdLength;

		// Token: 0x040047FC RID: 18428
		public int linkTextfirstCharacterIndex;

		// Token: 0x040047FD RID: 18429
		public int linkTextLength;

		// Token: 0x040047FE RID: 18430
		internal char[] linkID;
	}
}
