using System;
using TMProOld;
using UnityEngine;

// Token: 0x0200066D RID: 1645
[ExecuteInEditMode]
public class TextMeshProLineAnchor : MonoBehaviour
{
	// Token: 0x06003B14 RID: 15124 RVA: 0x001046BE File Offset: 0x001028BE
	private void OnValidate()
	{
		if (Math.Abs(this.offset - this.previousOffset) > 0.001f)
		{
			this.previousOffset = this.offset;
			this.DoLayout(true);
		}
	}

	// Token: 0x06003B15 RID: 15125 RVA: 0x001046EC File Offset: 0x001028EC
	private void LateUpdate()
	{
		this.DoLayout(false);
	}

	// Token: 0x06003B16 RID: 15126 RVA: 0x001046F8 File Offset: 0x001028F8
	private void DoLayout(bool force)
	{
		if (!this.text)
		{
			return;
		}
		TextMeshProLineAnchor.Axis axis = this.axis;
		if (axis != TextMeshProLineAnchor.Axis.Vertical)
		{
			if (axis != TextMeshProLineAnchor.Axis.Horizontal)
			{
				throw new ArgumentOutOfRangeException();
			}
			float preferredWidth = this.text.preferredWidth;
			if (force || Math.Abs(preferredWidth - this.previous) > 0.001f)
			{
				this.previous = preferredWidth;
				base.transform.SetPositionX(this.text.transform.position.y - preferredWidth * this.offsetDirection + this.offset);
				return;
			}
		}
		else
		{
			float preferredHeight = this.text.preferredHeight;
			if (force || Math.Abs(preferredHeight - this.previous) > 0.001f)
			{
				this.previous = preferredHeight;
				base.transform.SetPositionY(this.text.transform.position.y - preferredHeight * this.offsetDirection + this.offset);
				return;
			}
		}
	}

	// Token: 0x04003D64 RID: 15716
	[SerializeField]
	private TextMeshPro text;

	// Token: 0x04003D65 RID: 15717
	[SerializeField]
	private TextMeshProLineAnchor.Axis axis;

	// Token: 0x04003D66 RID: 15718
	[SerializeField]
	private float offsetDirection = 1f;

	// Token: 0x04003D67 RID: 15719
	[SerializeField]
	private float offset;

	// Token: 0x04003D68 RID: 15720
	private float previous;

	// Token: 0x04003D69 RID: 15721
	private float previousOffset;

	// Token: 0x02001980 RID: 6528
	private enum Axis
	{
		// Token: 0x04009611 RID: 38417
		Vertical,
		// Token: 0x04009612 RID: 38418
		Horizontal
	}
}
