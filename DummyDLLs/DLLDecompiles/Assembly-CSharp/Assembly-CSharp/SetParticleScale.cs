using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020005BD RID: 1469
public class SetParticleScale : MonoBehaviour
{
	// Token: 0x0600348D RID: 13453 RVA: 0x000E9760 File Offset: 0x000E7960
	private void Start()
	{
		if (this.grandParent)
		{
			if (base.transform.parent != null && base.transform.parent.parent != null)
			{
				this.parent = base.transform.parent.gameObject.transform.parent.gameObject;
			}
		}
		else if (this.greatGrandParent)
		{
			if (base.transform.parent != null && base.transform.parent.parent != null && base.transform.parent.parent.parent != null)
			{
				this.parent = base.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject;
			}
		}
		else if (base.transform.parent != null)
		{
			this.parent = base.transform.parent.gameObject;
		}
		this.hasParent = (this.parent != null);
		this.parentBody = base.GetComponentInParent<Rigidbody2D>();
		this.hasParentBody = (this.parentBody != null);
		this.started = true;
		ComponentSingleton<SetParticleScaleCallbackHooks>.Instance.OnUpdate += this.OnUpdate;
	}

	// Token: 0x0600348E RID: 13454 RVA: 0x000E98D2 File Offset: 0x000E7AD2
	private void OnEnable()
	{
		if (this.started)
		{
			ComponentSingleton<SetParticleScaleCallbackHooks>.Instance.OnUpdate += this.OnUpdate;
		}
		this.updated = false;
	}

	// Token: 0x0600348F RID: 13455 RVA: 0x000E98F9 File Offset: 0x000E7AF9
	private void OnDisable()
	{
		ComponentSingleton<SetParticleScaleCallbackHooks>.Instance.OnUpdate -= this.OnUpdate;
	}

	// Token: 0x06003490 RID: 13456 RVA: 0x000E9914 File Offset: 0x000E7B14
	private void OnUpdate()
	{
		if (this.updated)
		{
			return;
		}
		if (this.hasParentBody && !this.parentBody.IsAwake())
		{
			return;
		}
		if (this.hasParent && !this.unparented)
		{
			this.parentXScale = this.parent.transform.localScale.x;
			this.selfXScale = base.transform.localScale.x;
			if ((this.parentXScale < 0f && this.selfXScale > 0f) || (this.parentXScale > 0f && this.selfXScale < 0f))
			{
				this.scaleVector.Set(-base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
				base.transform.localScale = this.scaleVector;
			}
		}
		else
		{
			this.unparented = true;
			this.selfXScale = base.transform.localScale.x;
			if (this.selfXScale < 0f)
			{
				this.scaleVector.Set(-base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
				base.transform.localScale = this.scaleVector;
			}
		}
		if (this.doesNotMove)
		{
			this.updated = true;
		}
	}

	// Token: 0x06003491 RID: 13457 RVA: 0x000E9A96 File Offset: 0x000E7C96
	private void OnTransformParentChanged()
	{
		this.hasParent = (this.parentBody != null);
		this.updated = false;
	}

	// Token: 0x040037FC RID: 14332
	public bool grandParent;

	// Token: 0x040037FD RID: 14333
	public bool greatGrandParent;

	// Token: 0x040037FE RID: 14334
	[SerializeField]
	private bool doesNotMove;

	// Token: 0x040037FF RID: 14335
	private float parentXScale;

	// Token: 0x04003800 RID: 14336
	private float selfXScale;

	// Token: 0x04003801 RID: 14337
	private Vector3 scaleVector;

	// Token: 0x04003802 RID: 14338
	private bool unparented;

	// Token: 0x04003803 RID: 14339
	private bool hasParent;

	// Token: 0x04003804 RID: 14340
	private bool hasParentBody;

	// Token: 0x04003805 RID: 14341
	private GameObject parent;

	// Token: 0x04003806 RID: 14342
	private Rigidbody2D parentBody;

	// Token: 0x04003807 RID: 14343
	private bool updated;

	// Token: 0x04003808 RID: 14344
	private bool started;
}
