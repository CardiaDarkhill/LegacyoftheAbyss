using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004BB RID: 1211
public class CogCorpseReaction : MonoBehaviour
{
	// Token: 0x06002BB3 RID: 11187 RVA: 0x000BF804 File Offset: 0x000BDA04
	private void Update()
	{
		if (this.corpses.Count <= 0)
		{
			return;
		}
		this.timer += Time.deltaTime;
		if (this.timer < this.jitterFrameTime)
		{
			return;
		}
		for (int i = 0; i < this.corpses.Count; i++)
		{
			Transform transform = this.corpses[i];
			Vector3 position = transform.position;
			transform.position = new Vector3(position.x + Random.Range(-this.jitterX, this.jitterX), position.y + Random.Range(-this.jitterY, this.jitterY), position.z);
		}
		this.timer -= this.jitterFrameTime;
	}

	// Token: 0x06002BB4 RID: 11188 RVA: 0x000BF8C0 File Offset: 0x000BDAC0
	private void OnTriggerEnter2D(Collider2D collision)
	{
		GameObject gameObject = collision.gameObject;
		CogCorpseItem cogCorpseItem;
		if (!this.ShouldGrab(gameObject, out cogCorpseItem))
		{
			return;
		}
		this.corpses.Add(gameObject.transform);
		Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
		if (component)
		{
			component.linearVelocity = new Vector2(0f, 0f);
			component.angularVelocity = Random.Range(-1000f, 1000f);
			GameObject grindParticle = this.grindParticlePrefab.Spawn(gameObject.transform, Vector3.zero, Quaternion.identity);
			RecycleResetHandler.Add(gameObject, delegate()
			{
				grindParticle.Recycle();
			});
		}
		if (cogCorpseItem)
		{
			cogCorpseItem.EnteredCogs();
			cogCorpseItem.AddTrackedRegion(this);
			return;
		}
		Collider2D component2 = gameObject.GetComponent<Collider2D>();
		if (component2)
		{
			component2.isTrigger = true;
		}
	}

	// Token: 0x06002BB5 RID: 11189 RVA: 0x000BF994 File Offset: 0x000BDB94
	private void OnTriggerExit2D(Collider2D collision)
	{
		CogCorpseItem cogCorpseItem;
		if (!this.ShouldGrab(collision.gameObject, out cogCorpseItem))
		{
			return;
		}
		this.corpses.Remove(collision.gameObject.transform);
		if (cogCorpseItem)
		{
			cogCorpseItem.ExitedCogs();
			cogCorpseItem.RemoveTrackedRegion(this);
		}
	}

	// Token: 0x06002BB6 RID: 11190 RVA: 0x000BF9DE File Offset: 0x000BDBDE
	public void RemoveCorpse(Transform transform)
	{
		this.corpses.Remove(transform);
	}

	// Token: 0x06002BB7 RID: 11191 RVA: 0x000BF9ED File Offset: 0x000BDBED
	private bool ShouldGrab(GameObject obj, out CogCorpseItem item)
	{
		item = obj.GetComponent<CogCorpseItem>();
		return obj.layer == 26 || (item && !item.IsBroken);
	}

	// Token: 0x04002CFF RID: 11519
	[SerializeField]
	private float jitterFrameTime = 0.025f;

	// Token: 0x04002D00 RID: 11520
	[SerializeField]
	private float jitterX = 0.2f;

	// Token: 0x04002D01 RID: 11521
	[SerializeField]
	private float jitterY = 0.2f;

	// Token: 0x04002D02 RID: 11522
	[SerializeField]
	private GameObject grindParticlePrefab;

	// Token: 0x04002D03 RID: 11523
	private readonly List<Transform> corpses = new List<Transform>();

	// Token: 0x04002D04 RID: 11524
	private float timer;
}
