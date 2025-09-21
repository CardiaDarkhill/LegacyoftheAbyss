using System;
using UnityEngine;

// Token: 0x020005B0 RID: 1456
public class RandomlyFlipScale : MonoBehaviour
{
	// Token: 0x0600345C RID: 13404 RVA: 0x000E8C46 File Offset: 0x000E6E46
	private void Start()
	{
		if (!this.didScale)
		{
			this.ApplyScale();
		}
	}

	// Token: 0x0600345D RID: 13405 RVA: 0x000E8C56 File Offset: 0x000E6E56
	private void OnEnable()
	{
		if (this.doOnEnable)
		{
			this.ApplyScale();
		}
	}

	// Token: 0x0600345E RID: 13406 RVA: 0x000E8C68 File Offset: 0x000E6E68
	public void ApplyScale()
	{
		if ((float)Random.Range(1, 100) < this.flipChance)
		{
			if (this.flipX)
			{
				base.transform.localScale = new Vector3(-base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
			}
			if (this.flipY)
			{
				base.transform.localScale = new Vector3(base.transform.localScale.x, -base.transform.localScale.y, base.transform.localScale.z);
			}
		}
		this.didScale = true;
	}

	// Token: 0x040037DA RID: 14298
	[SerializeField]
	private bool flipX;

	// Token: 0x040037DB RID: 14299
	[SerializeField]
	private bool flipY;

	// Token: 0x040037DC RID: 14300
	[SerializeField]
	private float flipChance = 50f;

	// Token: 0x040037DD RID: 14301
	[SerializeField]
	private bool doOnEnable;

	// Token: 0x040037DE RID: 14302
	private bool didScale;
}
