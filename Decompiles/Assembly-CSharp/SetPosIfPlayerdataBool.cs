using System;
using UnityEngine;

// Token: 0x0200071B RID: 1819
public class SetPosIfPlayerdataBool : MonoBehaviour
{
	// Token: 0x060040B2 RID: 16562 RVA: 0x0011C528 File Offset: 0x0011A728
	private void OnEnable()
	{
		if (!this.hasSet || !this.onceOnly)
		{
			if (this.playerData == null)
			{
				this.playerData = PlayerData.instance;
			}
			if (this.playerData.GetBool(this.playerDataBool))
			{
				if (this.setX)
				{
					base.transform.localPosition = new Vector3(this.XPos, base.transform.localPosition.y, base.transform.localPosition.z);
				}
				if (this.setY)
				{
					base.transform.localPosition = new Vector3(base.transform.localPosition.x, this.YPos, base.transform.localPosition.z);
				}
				this.hasSet = true;
			}
		}
	}

	// Token: 0x04004233 RID: 16947
	[PlayerDataField(typeof(bool), true)]
	public string playerDataBool;

	// Token: 0x04004234 RID: 16948
	public bool setX;

	// Token: 0x04004235 RID: 16949
	public float XPos;

	// Token: 0x04004236 RID: 16950
	public bool setY;

	// Token: 0x04004237 RID: 16951
	public float YPos;

	// Token: 0x04004238 RID: 16952
	public bool onceOnly;

	// Token: 0x04004239 RID: 16953
	private bool hasSet;

	// Token: 0x0400423A RID: 16954
	private PlayerData playerData;
}
