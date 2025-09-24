using System;
using UnityEngine;

// Token: 0x0200047D RID: 1149
[ExecuteInEditMode]
public class PositionRelativeTo : MonoBehaviour
{
	// Token: 0x170004F4 RID: 1268
	// (get) Token: 0x06002998 RID: 10648 RVA: 0x000B5149 File Offset: 0x000B3349
	// (set) Token: 0x06002999 RID: 10649 RVA: 0x000B5151 File Offset: 0x000B3351
	public Vector3 Offset
	{
		get
		{
			return this.offset;
		}
		set
		{
			this.offset = value;
			this.Reposition();
		}
	}

	// Token: 0x0600299A RID: 10650 RVA: 0x000B5160 File Offset: 0x000B3360
	private void Awake()
	{
		this.previousPos = base.transform.position;
	}

	// Token: 0x0600299B RID: 10651 RVA: 0x000B5173 File Offset: 0x000B3373
	private void Update()
	{
		this.Reposition();
	}

	// Token: 0x0600299C RID: 10652 RVA: 0x000B517C File Offset: 0x000B337C
	private void Reposition()
	{
		Vector3 vector = this.target ? this.target.position : base.transform.position;
		if (this.inSpace)
		{
			vector = this.inSpace.InverseTransformPoint(vector);
		}
		if (this.target)
		{
			vector += this.offset;
		}
		else
		{
			vector = this.offset;
		}
		foreach (PositionRelativeTo.ExtensionPair extensionPair in this.extensions)
		{
			if (extensionPair.Target && extensionPair.Target.activeInHierarchy)
			{
				vector += extensionPair.AddOffset;
			}
		}
		Vector3 vector2 = base.transform.position;
		if (this.inSpace)
		{
			vector2 = this.inSpace.InverseTransformPoint(vector2);
		}
		if (this.positionX)
		{
			vector2.x = vector.x;
		}
		if (this.positionY)
		{
			vector2.y = vector.y;
		}
		if (this.positionZ)
		{
			vector2.z = vector.z;
		}
		if (this.inSpace)
		{
			vector2 = this.inSpace.TransformPoint(vector2);
		}
		if (Vector3.Distance(vector2, this.previousPos) <= Mathf.Epsilon)
		{
			return;
		}
		this.previousPos = vector2;
		base.transform.position = vector2;
	}

	// Token: 0x04002A2C RID: 10796
	[SerializeField]
	private Transform inSpace;

	// Token: 0x04002A2D RID: 10797
	[SerializeField]
	private Transform target;

	// Token: 0x04002A2E RID: 10798
	[SerializeField]
	private PositionRelativeTo.ExtensionPair[] extensions;

	// Token: 0x04002A2F RID: 10799
	[SerializeField]
	private bool positionX;

	// Token: 0x04002A30 RID: 10800
	[SerializeField]
	private bool positionY;

	// Token: 0x04002A31 RID: 10801
	[SerializeField]
	private bool positionZ;

	// Token: 0x04002A32 RID: 10802
	[SerializeField]
	private Vector3 offset;

	// Token: 0x04002A33 RID: 10803
	private Vector3 previousPos;

	// Token: 0x0200178B RID: 6027
	[Serializable]
	private struct ExtensionPair
	{
		// Token: 0x04008E68 RID: 36456
		public GameObject Target;

		// Token: 0x04008E69 RID: 36457
		public Vector3 AddOffset;
	}
}
