using System;
using TeamCherry.SharedUtils;
using TMProOld;
using UnityEngine;

// Token: 0x02000643 RID: 1603
public class DisplayItemAmount : MonoBehaviour
{
	// Token: 0x06003989 RID: 14729 RVA: 0x000FCB86 File Offset: 0x000FAD86
	private void OnEnable()
	{
		if (this.playerData == null)
		{
			this.playerData = PlayerData.instance;
		}
		this.Refresh();
	}

	// Token: 0x0600398A RID: 14730 RVA: 0x000FCBA4 File Offset: 0x000FADA4
	private void Refresh()
	{
		string text = this.playerData.GetVariable(this.playerDataInt).ToString();
		this.textObject.text = text;
	}

	// Token: 0x04003C43 RID: 15427
	[PlayerDataField(typeof(int), true)]
	[SerializeField]
	private string playerDataInt;

	// Token: 0x04003C44 RID: 15428
	[SerializeField]
	private TextMeshPro textObject;

	// Token: 0x04003C45 RID: 15429
	private PlayerData playerData;
}
