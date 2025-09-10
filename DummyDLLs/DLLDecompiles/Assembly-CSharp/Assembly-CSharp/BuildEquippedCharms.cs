using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200060F RID: 1551
public class BuildEquippedCharms : MonoBehaviour
{
	// Token: 0x06003764 RID: 14180 RVA: 0x000F4581 File Offset: 0x000F2781
	private void Start()
	{
	}

	// Token: 0x06003765 RID: 14181 RVA: 0x000F4584 File Offset: 0x000F2784
	private void BuildCharmList()
	{
		if (this.gm == null)
		{
			this.gm = GameManager.instance;
		}
		if (this.pd == null)
		{
			this.pd = PlayerData.instance;
		}
		this.uiItems = 0;
		this.equippedAmount = 0;
		int num = 0;
		float num2;
		if (num < 9)
		{
			num2 = this.CHARM_DISTANCE_X;
		}
		else if (num == 9)
		{
			num2 = 1.7f;
		}
		else if (num == 10)
		{
			num2 = 1.5f;
		}
		else
		{
			num2 = 1.4f;
		}
		this.instanceList = new List<GameObject>();
		float num3 = this.START_X;
		for (int i = 0; i < this.equippedCharms.Count; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.gameObjectList[this.equippedCharms[i] - 1]);
			gameObject.transform.position = new Vector3(num3, this.START_Y, -10f);
			gameObject.transform.SetParent(this.charmsFolder.transform, false);
			gameObject.transform.localScale = new Vector3(this.CHARM_SCALE, this.CHARM_SCALE, this.CHARM_SCALE);
			gameObject.name = this.equippedCharms[i].ToString();
			gameObject.GetComponent<CharmItem>().listNumber = i + 1;
			this.instanceList.Add(gameObject);
			num3 += num2;
		}
		this.uiItems = this.instanceList.Count;
		this.uiItems++;
		this.instanceList.Add(this.nextDot);
		this.nextDot.transform.localPosition = new Vector3(num3, this.START_Y, -6f);
		this.nextDot.GetComponent<CharmItem>().listNumber = this.instanceList.Count + 1;
		this.UpdateNotches();
	}

	// Token: 0x06003766 RID: 14182 RVA: 0x000F4751 File Offset: 0x000F2951
	public void UpdateNotches()
	{
	}

	// Token: 0x06003767 RID: 14183 RVA: 0x000F4753 File Offset: 0x000F2953
	public GameObject GetObjectAt(int listNumber)
	{
		return this.instanceList[listNumber - 1];
	}

	// Token: 0x06003768 RID: 14184 RVA: 0x000F4763 File Offset: 0x000F2963
	public int GetUICount()
	{
		return this.uiItems;
	}

	// Token: 0x06003769 RID: 14185 RVA: 0x000F476B File Offset: 0x000F296B
	public string GetItemName(int itemNum)
	{
		return this.instanceList[itemNum - 1].name;
	}

	// Token: 0x04003A45 RID: 14917
	public Color notchFullColor;

	// Token: 0x04003A46 RID: 14918
	public Color notchOverColor;

	// Token: 0x04003A47 RID: 14919
	public List<int> equippedCharms;

	// Token: 0x04003A48 RID: 14920
	public List<GameObject> gameObjectList;

	// Token: 0x04003A49 RID: 14921
	public List<GameObject> instanceList;

	// Token: 0x04003A4A RID: 14922
	private PlayerData pd;

	// Token: 0x04003A4B RID: 14923
	private GameObject textNotches;

	// Token: 0x04003A4C RID: 14924
	public GameObject nextDot;

	// Token: 0x04003A4D RID: 14925
	public GameObject charmsFolder;

	// Token: 0x04003A4E RID: 14926
	private GameManager gm;

	// Token: 0x04003A4F RID: 14927
	public int charmSlots;

	// Token: 0x04003A50 RID: 14928
	public int charmSlotsFilled;

	// Token: 0x04003A51 RID: 14929
	public int equippedAmount;

	// Token: 0x04003A52 RID: 14930
	public int uiItems;

	// Token: 0x04003A53 RID: 14931
	private float START_X = -7.28f;

	// Token: 0x04003A54 RID: 14932
	private float START_Y = -3.86f;

	// Token: 0x04003A55 RID: 14933
	private float CHARM_SCALE = 1.15f;

	// Token: 0x04003A56 RID: 14934
	private float CHARM_DISTANCE_X = 1.76f;
}
