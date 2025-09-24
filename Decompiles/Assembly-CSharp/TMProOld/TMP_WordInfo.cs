using System;

namespace TMProOld
{
	// Token: 0x020007FC RID: 2044
	public struct TMP_WordInfo
	{
		// Token: 0x060047DA RID: 18394 RVA: 0x0014DF7C File Offset: 0x0014C17C
		public string GetWord()
		{
			string text = string.Empty;
			TMP_CharacterInfo[] characterInfo = this.textComponent.textInfo.characterInfo;
			for (int i = this.firstCharacterIndex; i < this.lastCharacterIndex + 1; i++)
			{
				text += characterInfo[i].character.ToString();
			}
			return text;
		}

		// Token: 0x040047FF RID: 18431
		public TMP_Text textComponent;

		// Token: 0x04004800 RID: 18432
		public int firstCharacterIndex;

		// Token: 0x04004801 RID: 18433
		public int lastCharacterIndex;

		// Token: 0x04004802 RID: 18434
		public int characterCount;
	}
}
