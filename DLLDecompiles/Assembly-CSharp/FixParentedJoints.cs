using System;
using System.Collections;
using UnityEngine;

// Token: 0x020004DF RID: 1247
public class FixParentedJoints : MonoBehaviour
{
	// Token: 0x06002CCC RID: 11468 RVA: 0x000C3EBC File Offset: 0x000C20BC
	private void Awake()
	{
		if (!this.isActive)
		{
			return;
		}
		this.topParent = this.selfParent.parent;
		if (!this.topParent)
		{
			return;
		}
		Rigidbody2D rigidbody2D;
		Vector2 connectedAnchor;
		if (this.existingTargetBody)
		{
			rigidbody2D = this.existingTargetBody;
			connectedAnchor = this.existingTargetBody.position - this.kinematicBody.position;
		}
		else
		{
			GameObject gameObject = new GameObject("Fixed Joint - " + this.selfParent.gameObject.name);
			rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
			gameObject.transform.position = this.kinematicBody.transform.position;
			gameObject.transform.SetParent(this.topParent, true);
			connectedAnchor = Vector2.zero;
		}
		FixedJoint2D fixedJoint2D = rigidbody2D.gameObject.AddComponent<FixedJoint2D>();
		this.selfParent.transform.SetParent(null, true);
		rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
		fixedJoint2D.autoConfigureConnectedAnchor = false;
		fixedJoint2D.anchor = Vector2.zero;
		fixedJoint2D.connectedAnchor = connectedAnchor;
		fixedJoint2D.connectedBody = this.kinematicBody;
		if (this.delayBodyDynamicFrames <= 0)
		{
			this.kinematicBody.bodyType = RigidbodyType2D.Dynamic;
			return;
		}
		this.setDynamic = true;
	}

	// Token: 0x06002CCD RID: 11469 RVA: 0x000C3FE2 File Offset: 0x000C21E2
	private IEnumerator Start()
	{
		int yieldCount = 2;
		if (this.setDynamic)
		{
			int num;
			for (int i = 0; i < this.delayBodyDynamicFrames; i = num + 1)
			{
				num = yieldCount;
				yieldCount = num - 1;
				yield return null;
				num = i;
			}
			this.kinematicBody.bodyType = RigidbodyType2D.Dynamic;
		}
		if (this.topParent == null)
		{
			yield break;
		}
		while (yieldCount > 0)
		{
			int num = yieldCount;
			yieldCount = num - 1;
			yield return null;
		}
		if (this.topParent != null && !this.topParent.gameObject.activeInHierarchy)
		{
			this.selfParent.gameObject.SetActive(false);
		}
		yield break;
	}

	// Token: 0x04002E6C RID: 11884
	[SerializeField]
	private Transform selfParent;

	// Token: 0x04002E6D RID: 11885
	[SerializeField]
	private Rigidbody2D kinematicBody;

	// Token: 0x04002E6E RID: 11886
	[SerializeField]
	[Tooltip("For some reason child hinges all stretch out if body did not exist at runtime.")]
	private Rigidbody2D existingTargetBody;

	// Token: 0x04002E6F RID: 11887
	[SerializeField]
	private int delayBodyDynamicFrames;

	// Token: 0x04002E70 RID: 11888
	[Space]
	[SerializeField]
	private bool isActive = true;

	// Token: 0x04002E71 RID: 11889
	private bool setDynamic;

	// Token: 0x04002E72 RID: 11890
	private Transform topParent;
}
