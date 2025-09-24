using System;
using UnityEngine;

// Token: 0x020000A6 RID: 166
public class RecycleAfter2dtkAnimation : MonoBehaviour
{
	// Token: 0x06000507 RID: 1287 RVA: 0x0001A17C File Offset: 0x0001837C
	private void OnEnable()
	{
		this.timer = 0f;
		if (this.spriteAnimator == null)
		{
			this.spriteAnimator = base.GetComponent<tk2dSpriteAnimator>();
		}
		if (this.randomiseRotation)
		{
			base.transform.eulerAngles = new Vector3(base.transform.rotation.x, base.transform.rotation.y, (float)Random.Range(0, 360));
		}
		if (!this.spriteAnimator)
		{
			return;
		}
		if (this.spriteAnimator.CurrentClip != null || this.spriteAnimator.DefaultClip != null)
		{
			this.spriteAnimator.PlayFromFrame(0);
		}
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x0001A226 File Offset: 0x00018426
	private void Update()
	{
		if (this.timer > 0.1f)
		{
			this.timer -= Time.deltaTime;
			return;
		}
		if (!this.spriteAnimator.Playing)
		{
			base.gameObject.Recycle();
		}
	}

	// Token: 0x040004E1 RID: 1249
	public tk2dSpriteAnimator spriteAnimator;

	// Token: 0x040004E2 RID: 1250
	public bool randomiseRotation;

	// Token: 0x040004E3 RID: 1251
	private float timer;
}
