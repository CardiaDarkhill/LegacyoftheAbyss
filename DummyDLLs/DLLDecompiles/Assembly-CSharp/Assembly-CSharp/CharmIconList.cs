using System;
using UnityEngine;

// Token: 0x02000614 RID: 1556
public class CharmIconList : MonoBehaviour
{
	// Token: 0x0600377C RID: 14204 RVA: 0x000F4BA6 File Offset: 0x000F2DA6
	private void Awake()
	{
		CharmIconList.Instance = this;
	}

	// Token: 0x0600377D RID: 14205 RVA: 0x000F4BAE File Offset: 0x000F2DAE
	private void Start()
	{
		this.playerData = PlayerData.instance;
	}

	// Token: 0x0600377E RID: 14206 RVA: 0x000F4BBB File Offset: 0x000F2DBB
	public Sprite GetSprite(int id)
	{
		this.playerData = PlayerData.instance;
		return this.spriteList[id];
	}

	// Token: 0x04003A6E RID: 14958
	public static CharmIconList Instance;

	// Token: 0x04003A6F RID: 14959
	public Sprite[] spriteList;

	// Token: 0x04003A70 RID: 14960
	public Sprite unbreakableHeart;

	// Token: 0x04003A71 RID: 14961
	public Sprite unbreakableGreed;

	// Token: 0x04003A72 RID: 14962
	public Sprite unbreakableStrength;

	// Token: 0x04003A73 RID: 14963
	public Sprite grimmchildLevel1;

	// Token: 0x04003A74 RID: 14964
	public Sprite grimmchildLevel2;

	// Token: 0x04003A75 RID: 14965
	public Sprite grimmchildLevel3;

	// Token: 0x04003A76 RID: 14966
	public Sprite grimmchildLevel4;

	// Token: 0x04003A77 RID: 14967
	public Sprite nymmCharm;

	// Token: 0x04003A78 RID: 14968
	private PlayerData playerData;
}
