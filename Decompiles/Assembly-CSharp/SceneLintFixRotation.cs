using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000368 RID: 872
public class SceneLintFixRotation : MonoBehaviour, ISceneLintUpgrader
{
	// Token: 0x06001E04 RID: 7684 RVA: 0x0008AB58 File Offset: 0x00088D58
	private void Reset()
	{
		this.readRotationOf = base.transform;
	}

	// Token: 0x06001E05 RID: 7685 RVA: 0x0008AB66 File Offset: 0x00088D66
	private void Awake()
	{
		this.OnSceneLintUpgrade(true);
	}

	// Token: 0x06001E06 RID: 7686 RVA: 0x0008AB70 File Offset: 0x00088D70
	[ContextMenu("Do Upgrade")]
	private void DoUpgrade()
	{
		this.OnSceneLintUpgrade(true);
	}

	// Token: 0x06001E07 RID: 7687 RVA: 0x0008AB7C File Offset: 0x00088D7C
	public string OnSceneLintUpgrade(bool doUpgrade)
	{
		Vector3 eulerAngles = this.readRotationOf.eulerAngles;
		if (Mathf.Abs(eulerAngles.z) < Mathf.Epsilon)
		{
			return null;
		}
		if (!doUpgrade)
		{
			return string.Format("Rotation on parent of {0} degrees.", eulerAngles.z);
		}
		Transform[] array = this.fixChildrenOf;
		for (int i = 0; i < array.Length; i++)
		{
			foreach (object obj in array[i])
			{
				Transform transform = (Transform)obj;
				this.copiedChildren.Add(new SceneLintFixRotation.TransformRecord(transform));
			}
		}
		Vector3 eulerAngles2 = this.readRotationOf.eulerAngles;
		eulerAngles2.z = 0f;
		this.readRotationOf.eulerAngles = eulerAngles2;
		foreach (SceneLintFixRotation.TransformRecord transformRecord in this.copiedChildren)
		{
			transformRecord.Restore();
		}
		this.copiedChildren.Clear();
		return string.Format("Fixed rotation on parent of {0} degrees, fixed children.", eulerAngles.z);
	}

	// Token: 0x04001D20 RID: 7456
	[SerializeField]
	private Transform readRotationOf;

	// Token: 0x04001D21 RID: 7457
	[SerializeField]
	private Transform[] fixChildrenOf;

	// Token: 0x04001D22 RID: 7458
	private readonly List<SceneLintFixRotation.TransformRecord> copiedChildren = new List<SceneLintFixRotation.TransformRecord>();

	// Token: 0x0200161B RID: 5659
	private readonly struct TransformRecord
	{
		// Token: 0x060088F0 RID: 35056 RVA: 0x0027BA64 File Offset: 0x00279C64
		public TransformRecord(Transform transform)
		{
			this.transform = transform;
			this.position = transform.position;
			this.rotation = transform.rotation;
		}

		// Token: 0x060088F1 RID: 35057 RVA: 0x0027BA85 File Offset: 0x00279C85
		public void Restore()
		{
			this.transform.position = this.position;
			this.transform.rotation = this.rotation;
		}

		// Token: 0x040089B5 RID: 35253
		private readonly Transform transform;

		// Token: 0x040089B6 RID: 35254
		private readonly Vector3 position;

		// Token: 0x040089B7 RID: 35255
		private readonly Quaternion rotation;
	}
}
