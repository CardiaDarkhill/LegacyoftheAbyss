using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x0200073E RID: 1854
public class ButtonSkin
{
	// Token: 0x06004232 RID: 16946 RVA: 0x00124A13 File Offset: 0x00122C13
	public ButtonSkin(Sprite startSprite, string startSymbol, ButtonSkinType startSkinType)
	{
		this.sprite = startSprite;
		this.symbol = startSymbol;
		this.skinType = startSkinType;
	}

	// Token: 0x040043D3 RID: 17363
	public Sprite sprite;

	// Token: 0x040043D4 RID: 17364
	public string symbol;

	// Token: 0x040043D5 RID: 17365
	public ButtonSkinType skinType;
}
