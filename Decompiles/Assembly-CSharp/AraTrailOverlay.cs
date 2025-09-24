using System;
using Ara;
using UnityEngine;

// Token: 0x020000CD RID: 205
public class AraTrailOverlay : MonoBehaviour
{
	// Token: 0x0600068F RID: 1679 RVA: 0x000213CF File Offset: 0x0001F5CF
	private void Awake()
	{
		this.Refresh();
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x000213D8 File Offset: 0x0001F5D8
	private void OnDestroy()
	{
		if (this.materials != null)
		{
			foreach (Material material in this.materials)
			{
				if (material)
				{
					Object.Destroy(material);
				}
			}
			this.materials = null;
		}
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x0002141C File Offset: 0x0001F61C
	[ContextMenu("Refresh")]
	public void Refresh()
	{
		this.trailRenderer = base.GetComponent<AraTrail>();
		if (this.trailRenderer)
		{
			if (this.materials != null)
			{
				foreach (Material material in this.materials)
				{
					if (material)
					{
						Object.Destroy(material);
					}
				}
			}
			this.materials = new Material[this.trailRenderer.materials.Length];
			for (int j = 0; j < this.materials.Length; j++)
			{
				if (this.trailRenderer.materials[j])
				{
					this.materials[j] = new Material(this.trailRenderer.materials[j]);
					this.materials[j].renderQueue = 4000;
				}
			}
			this.trailRenderer.materials = this.materials;
		}
	}

	// Token: 0x04000661 RID: 1633
	private AraTrail trailRenderer;

	// Token: 0x04000662 RID: 1634
	private Material[] materials;
}
