using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000098 RID: 152
public class MossClump : MonoBehaviour, ScenePrefabInstanceFix.ICheckFields
{
	// Token: 0x060004BB RID: 1211 RVA: 0x00019523 File Offset: 0x00017723
	private void Awake()
	{
		if (!this.spriteRenderer)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x0001953C File Offset: 0x0001773C
	private void OnTriggerEnter2D(Collider2D collision)
	{
		int layer = collision.gameObject.layer;
		if (layer == 9 || layer == 11 || layer == 26)
		{
			this.touchingObjects.Add(collision.gameObject);
			if (!this.depressed)
			{
				this.depressed = true;
			}
		}
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x00019584 File Offset: 0x00017784
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (this.touchingObjects.Contains(collision.gameObject))
		{
			this.touchingObjects.Remove(collision.gameObject);
			if (this.touchingObjects.Count <= 0)
			{
				this.depressed = false;
			}
		}
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x000195C0 File Offset: 0x000177C0
	private void Update()
	{
		if (this.timer > 0f)
		{
			this.timer -= Time.deltaTime;
			return;
		}
		if (this.depressed && this.currentFrame < this.sprites.Count - 1)
		{
			this.currentFrame++;
			this.ChangeFrame();
		}
		if (!this.depressed && this.currentFrame > 0)
		{
			this.currentFrame--;
			this.ChangeFrame();
		}
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x00019643 File Offset: 0x00017843
	private void ChangeFrame()
	{
		if (!this.spriteRenderer)
		{
			return;
		}
		this.spriteRenderer.sprite = this.sprites[this.currentFrame];
		this.timer = 0.05f;
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x0001967A File Offset: 0x0001787A
	public void OnPrefabInstanceFix()
	{
		if (this.spriteRenderer)
		{
			ScenePrefabInstanceFix.CheckField<SpriteRenderer>(ref this.spriteRenderer);
		}
	}

	// Token: 0x0400048A RID: 1162
	public List<Sprite> sprites;

	// Token: 0x0400048B RID: 1163
	public SpriteRenderer spriteRenderer;

	// Token: 0x0400048C RID: 1164
	public List<GameObject> touchingObjects;

	// Token: 0x0400048D RID: 1165
	private bool depressed;

	// Token: 0x0400048E RID: 1166
	private const float frameTime = 0.05f;

	// Token: 0x0400048F RID: 1167
	private float timer;

	// Token: 0x04000490 RID: 1168
	private int currentFrame;
}
