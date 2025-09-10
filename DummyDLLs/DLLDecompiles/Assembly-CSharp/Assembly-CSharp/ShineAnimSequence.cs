using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000AE RID: 174
public class ShineAnimSequence : MonoBehaviour
{
	// Token: 0x06000520 RID: 1312 RVA: 0x0001A7AF File Offset: 0x000189AF
	private void OnEnable()
	{
		this.StartShine();
	}

	// Token: 0x06000521 RID: 1313 RVA: 0x0001A7B7 File Offset: 0x000189B7
	private void OnDisable()
	{
		this.StopShine();
	}

	// Token: 0x06000522 RID: 1314 RVA: 0x0001A7C0 File Offset: 0x000189C0
	public void StartShine()
	{
		if (this.shineRoutine != null)
		{
			return;
		}
		ShineAnimSequence.ShineObject[] array = this.shineObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Setup();
		}
		this.shineRoutine = base.StartCoroutine(this.ShineSequence());
	}

	// Token: 0x06000523 RID: 1315 RVA: 0x0001A808 File Offset: 0x00018A08
	public void StopShine()
	{
		if (this.shineRoutine == null)
		{
			return;
		}
		base.StopCoroutine(this.shineRoutine);
		this.shineRoutine = null;
		ShineAnimSequence.ShineObject[] array = this.shineObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ResetSprite();
		}
	}

	// Token: 0x06000524 RID: 1316 RVA: 0x0001A84E File Offset: 0x00018A4E
	private IEnumerator ShineSequence()
	{
		for (;;)
		{
			yield return new WaitForSeconds(Random.Range(this.minDelaySequence, this.maxDelaySequence));
			this.OnSequencePlay.Invoke();
			foreach (ShineAnimSequence.ShineObject shineObject in this.shineObjects)
			{
				if (shineObject.renderer.gameObject.activeInHierarchy)
				{
					base.StartCoroutine(shineObject.ShineAnim());
				}
				yield return new WaitForSeconds(this.delayBetweenObjects);
			}
			ShineAnimSequence.ShineObject[] array = null;
		}
		yield break;
	}

	// Token: 0x040004FC RID: 1276
	[SerializeField]
	private ShineAnimSequence.ShineObject[] shineObjects;

	// Token: 0x040004FD RID: 1277
	[SerializeField]
	private float delayBetweenObjects;

	// Token: 0x040004FE RID: 1278
	[SerializeField]
	private float minDelaySequence = 2f;

	// Token: 0x040004FF RID: 1279
	[SerializeField]
	private float maxDelaySequence = 4f;

	// Token: 0x04000500 RID: 1280
	[Space]
	public UnityEvent OnSequencePlay;

	// Token: 0x04000501 RID: 1281
	private Coroutine shineRoutine;

	// Token: 0x0200141C RID: 5148
	[Serializable]
	private class ShineObject
	{
		// Token: 0x0600828E RID: 33422 RVA: 0x00266066 File Offset: 0x00264266
		public void Setup()
		{
			if (this.renderer)
			{
				this.initialSprite = this.renderer.sprite;
			}
		}

		// Token: 0x0600828F RID: 33423 RVA: 0x00266086 File Offset: 0x00264286
		public IEnumerator ShineAnim()
		{
			if (!this.renderer || this.shineSprites.Length == 0)
			{
				yield break;
			}
			WaitForSeconds wait = new WaitForSeconds(1f / this.fps);
			foreach (Sprite sprite in this.shineSprites)
			{
				this.renderer.sprite = sprite;
				yield return wait;
			}
			Sprite[] array = null;
			this.ResetSprite();
			yield break;
		}

		// Token: 0x06008290 RID: 33424 RVA: 0x00266095 File Offset: 0x00264295
		public void ResetSprite()
		{
			if (!this.renderer)
			{
				return;
			}
			this.renderer.sprite = this.initialSprite;
		}

		// Token: 0x040081EF RID: 33263
		public SpriteRenderer renderer;

		// Token: 0x040081F0 RID: 33264
		public Sprite[] shineSprites;

		// Token: 0x040081F1 RID: 33265
		public float fps = 12f;

		// Token: 0x040081F2 RID: 33266
		private Sprite initialSprite;
	}
}
