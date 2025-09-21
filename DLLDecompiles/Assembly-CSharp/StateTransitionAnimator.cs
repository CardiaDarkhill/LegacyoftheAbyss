using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000BE RID: 190
public class StateTransitionAnimator : MonoBehaviour
{
	// Token: 0x06000607 RID: 1543 RVA: 0x0001EF40 File Offset: 0x0001D140
	private void SetSprite(Sprite sprite)
	{
		SpriteRenderer[] array = this.spriteRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].sprite = sprite;
		}
	}

	// Token: 0x06000608 RID: 1544 RVA: 0x0001EF6C File Offset: 0x0001D16C
	private void OnEnable()
	{
		if (this.transforms.Length != 0)
		{
			this.initialPositions = new Vector3[this.transforms.Length];
			for (int i = 0; i < this.initialPositions.Length; i++)
			{
				this.initialPositions[i] = this.transforms[i].localPosition;
			}
			this.SetTransformsOffset(Vector3.zero);
		}
		if (this.sprites.Length != 0)
		{
			this.SetSprite(this.sprites[0]);
		}
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x0001EFE3 File Offset: 0x0001D1E3
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x0600060A RID: 1546 RVA: 0x0001EFEB File Offset: 0x0001D1EB
	private IEnumerator AnimateSpritesSit()
	{
		WaitForSeconds wait = new WaitForSeconds(1f / this.framerate);
		foreach (Sprite sprite in this.sprites)
		{
			this.SetSprite(sprite);
			yield return wait;
		}
		Sprite[] array = null;
		yield break;
	}

	// Token: 0x0600060B RID: 1547 RVA: 0x0001EFFA File Offset: 0x0001D1FA
	private IEnumerator AnimateSpritesGetOff()
	{
		WaitForSeconds wait = new WaitForSeconds(1f / this.framerate);
		int num;
		for (int i = this.sprites.Length - 1; i >= 0; i = num - 1)
		{
			this.SetSprite(this.sprites[i]);
			yield return wait;
			num = i;
		}
		yield break;
	}

	// Token: 0x0600060C RID: 1548 RVA: 0x0001F009 File Offset: 0x0001D209
	private IEnumerator AnimateTransformsSit()
	{
		for (float elapsed = 0f; elapsed < this.moveTime; elapsed += Time.deltaTime)
		{
			this.SetTransformsOffset(this.localOffset * (elapsed / this.moveTime));
			yield return null;
		}
		this.SetTransformsOffset(this.localOffset);
		yield break;
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x0001F018 File Offset: 0x0001D218
	private IEnumerator AnimateTransformsGetOff()
	{
		for (float elapsed = 0f; elapsed < this.moveTime; elapsed += Time.deltaTime)
		{
			this.SetTransformsOffset(this.localOffset * (1f - elapsed / this.moveTime));
			yield return null;
		}
		this.SetTransformsOffset(Vector3.zero);
		yield break;
	}

	// Token: 0x0600060E RID: 1550 RVA: 0x0001F028 File Offset: 0x0001D228
	private void SetTransformsOffset(Vector3 offset)
	{
		for (int i = 0; i < this.transforms.Length; i++)
		{
			this.transforms[i].localPosition = this.initialPositions[i] + offset;
		}
	}

	// Token: 0x0600060F RID: 1551 RVA: 0x0001F068 File Offset: 0x0001D268
	public void SetState(bool value, bool isInstant)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (this.state == value)
		{
			return;
		}
		this.state = value;
		if (this.spriteRoutine != null)
		{
			base.StopCoroutine(this.spriteRoutine);
		}
		if (this.sprites.Length != 0)
		{
			if (isInstant)
			{
				Sprite sprite;
				if (!value)
				{
					sprite = this.sprites[0];
				}
				else
				{
					Sprite[] array = this.sprites;
					sprite = array[array.Length - 1];
				}
				this.SetSprite(sprite);
			}
			else
			{
				this.spriteRoutine = base.StartCoroutine(value ? this.AnimateSpritesSit() : this.AnimateSpritesGetOff());
			}
		}
		if (this.transformsRoutine != null)
		{
			base.StopCoroutine(this.transformsRoutine);
		}
		if (this.transforms.Length != 0)
		{
			if (isInstant)
			{
				this.SetTransformsOffset(value ? this.localOffset : Vector3.zero);
			}
			else
			{
				this.transformsRoutine = base.StartCoroutine(value ? this.AnimateTransformsSit() : this.AnimateTransformsGetOff());
			}
		}
		if (!isInstant)
		{
			if (value)
			{
				this.sitOnAudio.SpawnAndPlayOneShot(base.transform.position, null);
				return;
			}
			this.getOffAudio.SpawnAndPlayOneShot(base.transform.position, null);
		}
	}

	// Token: 0x040005D2 RID: 1490
	[SerializeField]
	private SpriteRenderer[] spriteRenderers;

	// Token: 0x040005D3 RID: 1491
	[SerializeField]
	private Sprite[] sprites;

	// Token: 0x040005D4 RID: 1492
	[SerializeField]
	private float framerate = 18f;

	// Token: 0x040005D5 RID: 1493
	[Space]
	[SerializeField]
	private Transform[] transforms;

	// Token: 0x040005D6 RID: 1494
	[SerializeField]
	private Vector3 localOffset;

	// Token: 0x040005D7 RID: 1495
	[SerializeField]
	private float moveTime;

	// Token: 0x040005D8 RID: 1496
	[SerializeField]
	private AudioEvent sitOnAudio;

	// Token: 0x040005D9 RID: 1497
	[SerializeField]
	private AudioEvent getOffAudio;

	// Token: 0x040005DA RID: 1498
	private bool state;

	// Token: 0x040005DB RID: 1499
	private Vector3[] initialPositions;

	// Token: 0x040005DC RID: 1500
	private Coroutine spriteRoutine;

	// Token: 0x040005DD RID: 1501
	private Coroutine transformsRoutine;
}
