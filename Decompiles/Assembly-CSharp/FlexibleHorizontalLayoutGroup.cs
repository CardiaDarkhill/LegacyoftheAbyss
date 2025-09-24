using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000663 RID: 1635
public class FlexibleHorizontalLayoutGroup : HorizontalLayoutGroup
{
	// Token: 0x06003A40 RID: 14912 RVA: 0x000FF4FC File Offset: 0x000FD6FC
	private void DoLayoutMethod(Action method)
	{
		bool childForceExpandWidth = this.m_ChildForceExpandWidth;
		this.m_ChildForceExpandWidth = (base.rectChildren.Count > this.targetRectChildren);
		float spacing = this.m_Spacing;
		if (base.rectChildren.Count > this.targetRectChildren)
		{
			this.m_Spacing = 0f;
		}
		method();
		this.m_ChildForceExpandWidth = childForceExpandWidth;
		this.m_Spacing = spacing;
	}

	// Token: 0x06003A41 RID: 14913 RVA: 0x000FF562 File Offset: 0x000FD762
	public override void CalculateLayoutInputHorizontal()
	{
		if (this.doCalculateLayoutInputHorizontal == null)
		{
			this.doCalculateLayoutInputHorizontal = new Action(base.CalculateLayoutInputHorizontal);
		}
		this.DoLayoutMethod(this.doCalculateLayoutInputHorizontal);
	}

	// Token: 0x06003A42 RID: 14914 RVA: 0x000FF58A File Offset: 0x000FD78A
	public override void CalculateLayoutInputVertical()
	{
		if (this.doCalculateLayoutInputVertical == null)
		{
			this.doCalculateLayoutInputVertical = new Action(base.CalculateLayoutInputVertical);
		}
		this.DoLayoutMethod(this.doCalculateLayoutInputVertical);
	}

	// Token: 0x06003A43 RID: 14915 RVA: 0x000FF5B2 File Offset: 0x000FD7B2
	public override void SetLayoutHorizontal()
	{
		if (this.doSetLayoutHorizontal == null)
		{
			this.doSetLayoutHorizontal = new Action(base.SetLayoutHorizontal);
		}
		this.DoLayoutMethod(this.doSetLayoutHorizontal);
	}

	// Token: 0x06003A44 RID: 14916 RVA: 0x000FF5DA File Offset: 0x000FD7DA
	public override void SetLayoutVertical()
	{
		if (this.doSetLayoutVertical == null)
		{
			this.doSetLayoutVertical = new Action(base.SetLayoutVertical);
		}
		this.DoLayoutMethod(this.doSetLayoutVertical);
	}

	// Token: 0x04003CCA RID: 15562
	[SerializeField]
	private int targetRectChildren;

	// Token: 0x04003CCB RID: 15563
	private Action doCalculateLayoutInputHorizontal;

	// Token: 0x04003CCC RID: 15564
	private Action doCalculateLayoutInputVertical;

	// Token: 0x04003CCD RID: 15565
	private Action doSetLayoutHorizontal;

	// Token: 0x04003CCE RID: 15566
	private Action doSetLayoutVertical;
}
