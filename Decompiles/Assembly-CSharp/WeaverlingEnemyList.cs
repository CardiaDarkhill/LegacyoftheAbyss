using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020003F1 RID: 1009
public class WeaverlingEnemyList : MonoBehaviour
{
	// Token: 0x06002271 RID: 8817 RVA: 0x0009E9AA File Offset: 0x0009CBAA
	private void OnEnable()
	{
		this.enemyList.Clear();
	}

	// Token: 0x06002272 RID: 8818 RVA: 0x0009E9B7 File Offset: 0x0009CBB7
	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		this.enemyList.Add(otherCollider.gameObject);
	}

	// Token: 0x06002273 RID: 8819 RVA: 0x0009E9CA File Offset: 0x0009CBCA
	private void OnTriggerExit2D(Collider2D otherCollider)
	{
		this.enemyList.Remove(otherCollider.gameObject);
	}

	// Token: 0x06002274 RID: 8820 RVA: 0x0009E9DE File Offset: 0x0009CBDE
	public GameObject GetTarget()
	{
		if (this.enemyList.Count > 0)
		{
			return this.enemyList[Random.Range(0, this.enemyList.Count)];
		}
		return null;
	}

	// Token: 0x04002142 RID: 8514
	public List<GameObject> enemyList;

	// Token: 0x02001693 RID: 5779
	[ActionCategory("Hollow Knight")]
	public class GetEnemyTarget : FsmStateAction
	{
		// Token: 0x06008A8F RID: 35471 RVA: 0x0028098C File Offset: 0x0027EB8C
		public override void Reset()
		{
			this.listHolder = new FsmOwnerDefault();
			this.storeTarget = new FsmGameObject();
		}

		// Token: 0x06008A90 RID: 35472 RVA: 0x002809A4 File Offset: 0x0027EBA4
		public override void OnEnter()
		{
			GameObject gameObject = (this.listHolder.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.listHolder.GameObject.Value;
			if (gameObject != null)
			{
				WeaverlingEnemyList component = gameObject.GetComponent<WeaverlingEnemyList>();
				if (component != null)
				{
					this.storeTarget.Value = component.GetTarget();
				}
			}
			base.Finish();
		}

		// Token: 0x04008B73 RID: 35699
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault listHolder;

		// Token: 0x04008B74 RID: 35700
		public FsmGameObject storeTarget;
	}
}
