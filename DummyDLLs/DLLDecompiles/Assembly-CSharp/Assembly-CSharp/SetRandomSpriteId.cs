using System;
using UnityEngine;

// Token: 0x020005C0 RID: 1472
public class SetRandomSpriteId : MonoBehaviour, IExternalDebris
{
	// Token: 0x06003499 RID: 13465 RVA: 0x000E9B29 File Offset: 0x000E7D29
	protected void Awake()
	{
		this.sprite = base.GetComponent<tk2dSprite>();
	}

	// Token: 0x0600349A RID: 13466 RVA: 0x000E9B38 File Offset: 0x000E7D38
	public void Init()
	{
		if (this.sprite != null)
		{
			tk2dSpriteCollectionData collection = this.sprite.Collection;
			if (collection != null)
			{
				this.sprite.SetSprite(collection, Random.Range(0, collection.Count));
			}
		}
	}

	// Token: 0x0600349B RID: 13467 RVA: 0x000E9B80 File Offset: 0x000E7D80
	void IExternalDebris.InitExternalDebris()
	{
		this.Init();
	}

	// Token: 0x0400380D RID: 14349
	private tk2dSprite sprite;
}
