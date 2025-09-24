using System;
using UnityEngine;

// Token: 0x0200006B RID: 107
public class BossThreadAnimator : MonoBehaviour
{
	// Token: 0x060002BB RID: 699 RVA: 0x0000F3AD File Offset: 0x0000D5AD
	private void OnEnable()
	{
		this.animTimer = 0f;
		this.startTimer = 0f;
		this.cyclesRemaining = 11;
		this.startPause = Random.Range(0f, 0.2f);
		this.animating = true;
	}

	// Token: 0x060002BC RID: 700 RVA: 0x0000F3EC File Offset: 0x0000D5EC
	private void Update()
	{
		if (this.startTimer < this.startPause)
		{
			this.startTimer += Time.deltaTime;
		}
		else if (this.animating)
		{
			this.animTimer -= Time.deltaTime;
			if (this.animTimer <= 0f)
			{
				this.cyclesRemaining--;
				this.threadObject.SetActive(false);
				float x = 1f;
				if (Random.Range(1, 100) >= 50)
				{
					x = -1f;
				}
				this.threadObject.transform.localScale = new Vector3(x, 1f, 1f);
				base.gameObject.transform.localEulerAngles = new Vector3(0f, 0f, (float)Random.Range(0, 360));
				this.threadObject.SetActive(true);
				this.animTimer += 0.3f;
			}
		}
		if (this.cyclesRemaining <= 0)
		{
			this.threadObject.SetActive(false);
			this.animating = false;
		}
	}

	// Token: 0x04000256 RID: 598
	private const int CYCLES_TOTAL = 11;

	// Token: 0x04000257 RID: 599
	private const float THREAD_ANIM_TIME = 0.3f;

	// Token: 0x04000258 RID: 600
	public GameObject threadObject;

	// Token: 0x04000259 RID: 601
	private bool animating;

	// Token: 0x0400025A RID: 602
	private int cyclesRemaining;

	// Token: 0x0400025B RID: 603
	private float animTimer;

	// Token: 0x0400025C RID: 604
	private float startPause;

	// Token: 0x0400025D RID: 605
	private float startTimer;
}
