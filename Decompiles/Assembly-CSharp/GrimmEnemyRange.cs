using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000010 RID: 16
public class GrimmEnemyRange : MonoBehaviour
{
	// Token: 0x06000071 RID: 113 RVA: 0x0000418D File Offset: 0x0000238D
	private void OnEnable()
	{
		this.ClearEnemyList();
	}

	// Token: 0x06000072 RID: 114 RVA: 0x00004195 File Offset: 0x00002395
	public bool IsEnemyInRange()
	{
		return this.enemyList.Count != 0;
	}

	// Token: 0x06000073 RID: 115 RVA: 0x000041A8 File Offset: 0x000023A8
	public GameObject GetTarget()
	{
		GameObject result = null;
		float num = 99999f;
		if (this.enemyList.Count > 0)
		{
			for (int i = this.enemyList.Count - 1; i > -1; i--)
			{
				if (this.enemyList[i] == null || !this.enemyList[i].activeSelf)
				{
					this.enemyList.RemoveAt(i);
				}
			}
			foreach (GameObject gameObject in this.enemyList)
			{
				if (!Physics2D.Linecast(base.transform.position, gameObject.transform.position, 256))
				{
					float sqrMagnitude = (base.transform.position - gameObject.transform.position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						result = gameObject;
						num = sqrMagnitude;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06000074 RID: 116 RVA: 0x000042C0 File Offset: 0x000024C0
	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		this.enemyList.Add(otherCollider.gameObject);
	}

	// Token: 0x06000075 RID: 117 RVA: 0x000042D3 File Offset: 0x000024D3
	private void OnTriggerExit2D(Collider2D otherCollider)
	{
		this.enemyList.Remove(otherCollider.gameObject);
	}

	// Token: 0x06000076 RID: 118 RVA: 0x000042E7 File Offset: 0x000024E7
	public void ClearEnemyList()
	{
		this.enemyList.Clear();
	}

	// Token: 0x0400005B RID: 91
	public List<GameObject> enemyList;
}
