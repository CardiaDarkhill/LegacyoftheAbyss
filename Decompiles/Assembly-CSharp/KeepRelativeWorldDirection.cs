using System;
using UnityEngine;

// Token: 0x02000246 RID: 582
public class KeepRelativeWorldDirection : MonoBehaviour
{
	// Token: 0x06001538 RID: 5432 RVA: 0x0006035E File Offset: 0x0005E55E
	private void Awake()
	{
		if (!this.InitParent())
		{
			return;
		}
		this.referenceOffset = base.transform.localPosition;
	}

	// Token: 0x06001539 RID: 5433 RVA: 0x0006037A File Offset: 0x0005E57A
	private bool InitParent()
	{
		if (!this.hasParent)
		{
			this.parent = base.transform.parent;
			this.hasParent = (this.parent != null);
			if (!this.hasParent)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600153A RID: 5434 RVA: 0x000603B4 File Offset: 0x0005E5B4
	private void LateUpdate()
	{
		if (!this.hasParent)
		{
			return;
		}
		Vector3 normalized = this.parent.InverseTransformDirection(this.referenceDirection).normalized;
		Vector3 position = Quaternion.LookRotation(base.transform.forward, normalized) * this.referenceOffset;
		base.transform.position = this.parent.TransformPoint(position);
	}

	// Token: 0x0600153B RID: 5435 RVA: 0x00060418 File Offset: 0x0005E618
	private void OnTransformParentChanged()
	{
		if (!this.InitParent())
		{
			return;
		}
		if (this.parent == base.transform.parent)
		{
			return;
		}
		this.referenceOffset = base.transform.localPosition;
	}

	// Token: 0x0600153C RID: 5436 RVA: 0x00060450 File Offset: 0x0005E650
	[ContextMenu("Record Current World Direction and Offset")]
	private void RecordCurrentWorldDirection()
	{
		if (this.parent == null)
		{
			Debug.LogError("Parent is not assigned. Cannot record world direction and offset.");
			return;
		}
		this.referenceDirection = this.parent.InverseTransformDirection((base.transform.position - this.parent.position).normalized);
		Debug.Log(string.Format("Recorded current world direction as: {0} and offset as: {1}", this.referenceDirection, this.referenceOffset));
	}

	// Token: 0x040013DB RID: 5083
	[SerializeField]
	private Vector3 referenceDirection = Vector3.up;

	// Token: 0x040013DC RID: 5084
	private Vector3 referenceOffset;

	// Token: 0x040013DD RID: 5085
	private Transform parent;

	// Token: 0x040013DE RID: 5086
	private bool hasParent;
}
