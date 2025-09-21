using System;
using UnityEngine;

// Token: 0x02000149 RID: 329
[RequireComponent(typeof(BoxCollider2D))]
public class BoxCollider2DConfigs : MonoBehaviour
{
	// Token: 0x06000A10 RID: 2576 RVA: 0x0002DA6C File Offset: 0x0002BC6C
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		if (this.configs == null)
		{
			return;
		}
		foreach (BoxCollider2DConfigs.Config config in this.configs)
		{
			Gizmos.DrawWireCube(config.Offset, config.Size);
		}
	}

	// Token: 0x06000A11 RID: 2577 RVA: 0x0002DACA File Offset: 0x0002BCCA
	private void Awake()
	{
		this.GetBox();
	}

	// Token: 0x06000A12 RID: 2578 RVA: 0x0002DAD4 File Offset: 0x0002BCD4
	private void GetBox()
	{
		if (this.box)
		{
			return;
		}
		this.box = base.GetComponent<BoxCollider2D>();
		this.initialConfig = new BoxCollider2DConfigs.Config
		{
			Offset = this.box.offset,
			Size = this.box.size
		};
	}

	// Token: 0x06000A13 RID: 2579 RVA: 0x0002DB30 File Offset: 0x0002BD30
	public void SetConfig(int index)
	{
		this.GetBox();
		BoxCollider2DConfigs.Config config;
		if (index < 0 || index >= this.configs.Length)
		{
			config = this.initialConfig;
		}
		else
		{
			config = this.configs[index];
		}
		this.box.offset = config.Offset;
		this.box.size = config.Size;
	}

	// Token: 0x040009A1 RID: 2465
	[SerializeField]
	private BoxCollider2DConfigs.Config[] configs;

	// Token: 0x040009A2 RID: 2466
	private BoxCollider2DConfigs.Config initialConfig;

	// Token: 0x040009A3 RID: 2467
	private BoxCollider2D box;

	// Token: 0x0200147E RID: 5246
	[Serializable]
	private struct Config
	{
		// Token: 0x0400836D RID: 33645
		public Vector2 Offset;

		// Token: 0x0400836E RID: 33646
		public Vector2 Size;
	}
}
