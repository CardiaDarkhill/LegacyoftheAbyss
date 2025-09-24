using System;
using TeamCherry.NestedFadeGroup;
using TMProOld;
using UnityEngine;

// Token: 0x0200064D RID: 1613
[ExecuteInEditMode]
[RequireComponent(typeof(TextMeshPro))]
[NestedFadeGroupBridge(new Type[]
{
	typeof(TextMeshPro)
})]
public class NestedFadeGroupTextMeshPro : NestedFadeGroupBase
{
	// Token: 0x1700068D RID: 1677
	// (get) Token: 0x060039BC RID: 14780 RVA: 0x000FD493 File Offset: 0x000FB693
	// (set) Token: 0x060039BD RID: 14781 RVA: 0x000FD4B9 File Offset: 0x000FB6B9
	public Color Color
	{
		get
		{
			this.GetMissingReferences();
			if (!this.textMesh)
			{
				return Color.black;
			}
			return this.textMesh.color;
		}
		set
		{
			this.GetMissingReferences();
			base.AlphaSelf = value.a;
		}
	}

	// Token: 0x1700068E RID: 1678
	// (get) Token: 0x060039BE RID: 14782 RVA: 0x000FD4CD File Offset: 0x000FB6CD
	// (set) Token: 0x060039BF RID: 14783 RVA: 0x000FD4F3 File Offset: 0x000FB6F3
	public string Text
	{
		get
		{
			this.GetMissingReferences();
			if (!this.textMesh)
			{
				return string.Empty;
			}
			return this.textMesh.text;
		}
		set
		{
			this.GetMissingReferences();
			if (this.textMesh)
			{
				this.textMesh.text = value;
			}
		}
	}

	// Token: 0x060039C0 RID: 14784 RVA: 0x000FD514 File Offset: 0x000FB714
	protected override void GetMissingReferences()
	{
		if (!this.textMesh)
		{
			this.textMesh = base.GetComponent<TextMeshPro>();
			this.textMesh.enabled = true;
		}
		if (!this.meshRenderer)
		{
			this.meshRenderer = base.GetComponent<MeshRenderer>();
		}
	}

	// Token: 0x060039C1 RID: 14785 RVA: 0x000FD554 File Offset: 0x000FB754
	protected override void OnComponentAdded()
	{
		if (this.textMesh)
		{
			base.AlphaSelf = this.textMesh.color.a;
		}
	}

	// Token: 0x060039C2 RID: 14786 RVA: 0x000FD57C File Offset: 0x000FB77C
	protected override void OnAlphaChanged(float alpha)
	{
		Color color = this.textMesh.color;
		color.a = alpha;
		this.textMesh.color = color;
		this.meshRenderer.enabled = (alpha > Mathf.Epsilon);
	}

	// Token: 0x04003C6E RID: 15470
	private TextMeshPro textMesh;

	// Token: 0x04003C6F RID: 15471
	private MeshRenderer meshRenderer;
}
