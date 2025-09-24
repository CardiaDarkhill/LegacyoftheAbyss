using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x020006D2 RID: 1746
public class MapNextAreaDisplay : MonoBehaviour
{
	// Token: 0x06003F0C RID: 16140 RVA: 0x001161AE File Offset: 0x001143AE
	private void Reset()
	{
		this.gameMap = base.GetComponentInParent<GameMap>();
	}

	// Token: 0x06003F0D RID: 16141 RVA: 0x001161BC File Offset: 0x001143BC
	private void Awake()
	{
		this.gameMap.UpdateQuickMapDisplay += this.Refresh;
	}

	// Token: 0x06003F0E RID: 16142 RVA: 0x001161D5 File Offset: 0x001143D5
	private void OnDestroy()
	{
		this.gameMap.UpdateQuickMapDisplay -= this.Refresh;
	}

	// Token: 0x06003F0F RID: 16143 RVA: 0x001161F0 File Offset: 0x001143F0
	private void Refresh(bool display, MapZone _)
	{
		if (this.pd == null)
		{
			this.pd = GameManager.instance.playerData;
		}
		bool flag = string.IsNullOrEmpty(this.visitedString);
		if (!flag)
		{
			flag = this.pd.GetBool(this.visitedString);
		}
		if (flag && !this.visibleCondition.IsFulfilled)
		{
			flag = false;
		}
		if (flag && !string.IsNullOrEmpty(this.sceneVisited) && !this.pd.scenesVisited.Contains(this.sceneVisited))
		{
			flag = false;
		}
		if (flag && display)
		{
			GameMapScene componentInParent = base.gameObject.GetComponentInParent<GameMapScene>(true);
			if (componentInParent)
			{
				if (!componentInParent.IsMapped)
				{
					flag = false;
				}
			}
			else
			{
				Debug.LogError(string.Concat(new string[]
				{
					"Next area display \"",
					base.gameObject.name,
					"\" in \"",
					base.transform.parent.name,
					"\" did not have map scene parent."
				}));
			}
		}
		if (base.gameObject.activeSelf)
		{
			if (!flag || !display)
			{
				base.gameObject.SetActive(false);
				return;
			}
		}
		else if (flag && display)
		{
			base.gameObject.SetActive(true);
		}
	}

	// Token: 0x040040D2 RID: 16594
	[SerializeField]
	private GameMap gameMap;

	// Token: 0x040040D3 RID: 16595
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string visitedString;

	// Token: 0x040040D4 RID: 16596
	[SerializeField]
	private MapPinConditional.Condition visibleCondition;

	// Token: 0x040040D5 RID: 16597
	[SerializeField]
	private string sceneVisited;

	// Token: 0x040040D6 RID: 16598
	private PlayerData pd;
}
