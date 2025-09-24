using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003B2 RID: 946
[ExecuteInEditMode]
public class RadialLayoutUI : TransformLayout
{
	// Token: 0x1700032F RID: 815
	// (get) Token: 0x06001FBC RID: 8124 RVA: 0x00090FFE File Offset: 0x0008F1FE
	// (set) Token: 0x06001FBD RID: 8125 RVA: 0x00091006 File Offset: 0x0008F206
	public bool ElementOffset
	{
		get
		{
			return this.elementOffset;
		}
		set
		{
			this.elementOffset = value;
			this.UpdatePositions();
		}
	}

	// Token: 0x06001FBE RID: 8126 RVA: 0x00091018 File Offset: 0x0008F218
	public override void UpdatePositions()
	{
		float num = this.radius * this.scale;
		float num2 = this.splitX * this.scale;
		float num3 = this.splitY * this.scale;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if (!this.ignoreInactive || child.gameObject.activeSelf)
			{
				this.elements.Add(child);
			}
		}
		float num4 = this.elementOffset ? (360f / (float)this.elements.Count / 2f) : 0f;
		int num5 = 0;
		foreach (Transform transform in this.elements)
		{
			if (transform.gameObject.activeSelf || !this.ignoreInactive)
			{
				float num6 = (float)(num5 + 1) / (float)this.elements.Count * 360f - num4;
				if (this.counterClockwise)
				{
					num6 = 360f - num6;
				}
				if (this.rotateElements)
				{
					transform.SetLocalRotation2D(360f - num6);
				}
				num6 *= 0.017453292f;
				Vector3 vector = new Vector3(num * Mathf.Sin(num6), num * Mathf.Cos(num6), 0f);
				vector.x += ((vector.x > 0f) ? num2 : (-num2));
				vector.y += ((vector.y > 0f) ? num3 : (-num3));
				transform.localPosition = vector;
				num5++;
			}
		}
		this.elements.Clear();
	}

	// Token: 0x04001EC7 RID: 7879
	[SerializeField]
	private bool ignoreInactive;

	// Token: 0x04001EC8 RID: 7880
	[SerializeField]
	private float scale = 1f;

	// Token: 0x04001EC9 RID: 7881
	[SerializeField]
	private float radius = 1f;

	// Token: 0x04001ECA RID: 7882
	[SerializeField]
	private bool elementOffset;

	// Token: 0x04001ECB RID: 7883
	[SerializeField]
	private float splitX;

	// Token: 0x04001ECC RID: 7884
	[SerializeField]
	private float splitY;

	// Token: 0x04001ECD RID: 7885
	[SerializeField]
	private bool counterClockwise;

	// Token: 0x04001ECE RID: 7886
	[SerializeField]
	private bool rotateElements;

	// Token: 0x04001ECF RID: 7887
	private readonly List<Transform> elements = new List<Transform>();
}
