using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200006A RID: 106
public class BoneSegment : MonoBehaviour
{
	// Token: 0x060002B6 RID: 694 RVA: 0x0000F1E0 File Offset: 0x0000D3E0
	private void OnTriggerEnter2D(Collider2D collision)
	{
		int layer = collision.gameObject.layer;
		bool flag = layer == 9;
		if (!flag && layer != 11 && layer != 26)
		{
			return;
		}
		this.touchingObjects.AddIfNotPresent(collision.gameObject);
		if (!this.depressed)
		{
			this.Depress(flag);
		}
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x0000F22E File Offset: 0x0000D42E
	private void OnTriggerExit2D(Collider2D collision)
	{
		this.touchingObjects.Remove(collision.gameObject);
		if (this.touchingObjects.Count <= 0)
		{
			this.Release();
		}
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x0000F258 File Offset: 0x0000D458
	private void Depress(bool vibrate)
	{
		if (this.depressed)
		{
			return;
		}
		this.depressed = true;
		if (this.particle != null)
		{
			this.particle.Play();
		}
		this.sprite.transform.SetLocalPosition2D(new Vector2(0f, -0.05f));
		Vector3 b = new Vector3(0f, -0.05f, 0f);
		foreach (Transform transform in this.depressOthers)
		{
			if (transform)
			{
				transform.localPosition += b;
			}
		}
		this.depressSound.SpawnAndPlayOneShot(base.transform.position, vibrate, null);
	}

	// Token: 0x060002B9 RID: 697 RVA: 0x0000F310 File Offset: 0x0000D510
	private void Release()
	{
		if (!this.depressed)
		{
			return;
		}
		this.depressed = false;
		this.sprite.transform.SetLocalPosition2D(new Vector3(0f, 0f));
		Vector3 b = new Vector3(0f, 0.05f, 0f);
		foreach (Transform transform in this.depressOthers)
		{
			if (transform)
			{
				transform.localPosition += b;
			}
		}
	}

	// Token: 0x04000250 RID: 592
	[SerializeField]
	private GameObject sprite;

	// Token: 0x04000251 RID: 593
	[SerializeField]
	private ParticleSystem particle;

	// Token: 0x04000252 RID: 594
	[SerializeField]
	private AudioEventRandom depressSound;

	// Token: 0x04000253 RID: 595
	[SerializeField]
	private Transform[] depressOthers;

	// Token: 0x04000254 RID: 596
	private readonly List<GameObject> touchingObjects = new List<GameObject>();

	// Token: 0x04000255 RID: 597
	private bool depressed;
}
