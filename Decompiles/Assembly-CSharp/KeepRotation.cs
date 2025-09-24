using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020002FC RID: 764
public class KeepRotation : MonoBehaviour
{
	// Token: 0x06001B40 RID: 6976 RVA: 0x0007E92E File Offset: 0x0007CB2E
	private void Awake()
	{
		this.originalScale = base.transform.lossyScale;
	}

	// Token: 0x06001B41 RID: 6977 RVA: 0x0007E944 File Offset: 0x0007CB44
	private void Start()
	{
		this.tf = base.transform;
		this.hasTransform = true;
		if (!this.useParentAngle)
		{
			this.rotation = new Vector3(0f, 0f, this.angle);
			this.UpdateScale();
		}
		this.parentBody = base.GetComponentInParent<Rigidbody2D>();
		this.hasParentBody = (this.parentBody != null);
		this.UpdateParent();
		this.started = true;
		ComponentSingleton<KeepRotationCallbackHooks>.Instance.OnLateUpdate += this.OnLateUpdate;
	}

	// Token: 0x06001B42 RID: 6978 RVA: 0x0007E9D0 File Offset: 0x0007CBD0
	private void OnEnable()
	{
		if (this.started)
		{
			ComponentSingleton<KeepRotationCallbackHooks>.Instance.OnLateUpdate += this.OnLateUpdate;
		}
		if (this.useParentAngle)
		{
			this.angle = base.transform.parent.transform.localEulerAngles.z;
			this.rotation = new Vector3(0f, 0f, this.angle);
			this.UpdateScale();
		}
	}

	// Token: 0x06001B43 RID: 6979 RVA: 0x0007EA44 File Offset: 0x0007CC44
	private void OnDisable()
	{
		ComponentSingleton<KeepRotationCallbackHooks>.Instance.OnLateUpdate -= this.OnLateUpdate;
	}

	// Token: 0x06001B44 RID: 6980 RVA: 0x0007EA5C File Offset: 0x0007CC5C
	private void OnLateUpdate()
	{
		if (this.hasParentBody && !this.parentBody.IsAwake() && !this.forceEveryFrame)
		{
			return;
		}
		if (this.hasTransform)
		{
			if (this.worldSpace)
			{
				this.tf.eulerAngles = this.rotation;
			}
			else
			{
				this.tf.localEulerAngles = this.rotation;
			}
			this.UpdateScale();
		}
	}

	// Token: 0x06001B45 RID: 6981 RVA: 0x0007EAC4 File Offset: 0x0007CCC4
	private void UpdateScale()
	{
		if (!this.keepWorldScale)
		{
			return;
		}
		if (this.hasParent)
		{
			return;
		}
		Vector3 lossyScale = this.tf.parent.lossyScale;
		this.tf.localScale = new Vector3(this.originalScale.x / lossyScale.x, this.originalScale.y / lossyScale.y, this.originalScale.z / lossyScale.z);
	}

	// Token: 0x06001B46 RID: 6982 RVA: 0x0007EB3A File Offset: 0x0007CD3A
	private void UpdateParent()
	{
		if (this.hasTransform)
		{
			this.hasParent = (this.tf.parent != null);
		}
	}

	// Token: 0x06001B47 RID: 6983 RVA: 0x0007EB5B File Offset: 0x0007CD5B
	private void OnTransformParentChanged()
	{
		this.UpdateParent();
	}

	// Token: 0x04001A2F RID: 6703
	[SerializeField]
	private float angle;

	// Token: 0x04001A30 RID: 6704
	[SerializeField]
	private bool worldSpace;

	// Token: 0x04001A31 RID: 6705
	[SerializeField]
	private bool useParentAngle;

	// Token: 0x04001A32 RID: 6706
	[SerializeField]
	private bool forceEveryFrame;

	// Token: 0x04001A33 RID: 6707
	[SerializeField]
	private bool keepWorldScale;

	// Token: 0x04001A34 RID: 6708
	private bool hasTransform;

	// Token: 0x04001A35 RID: 6709
	private Transform tf;

	// Token: 0x04001A36 RID: 6710
	private Vector3 rotation;

	// Token: 0x04001A37 RID: 6711
	private Rigidbody2D parentBody;

	// Token: 0x04001A38 RID: 6712
	private bool hasParentBody;

	// Token: 0x04001A39 RID: 6713
	private Vector3 originalScale;

	// Token: 0x04001A3A RID: 6714
	private bool hasParent;

	// Token: 0x04001A3B RID: 6715
	private bool started;
}
