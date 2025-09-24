using System;
using UnityEngine;

// Token: 0x02000336 RID: 822
public class FaceAngleSimple : MonoBehaviour
{
	// Token: 0x06001CC1 RID: 7361 RVA: 0x00085EDA File Offset: 0x000840DA
	private void Awake()
	{
		this.yScale = base.transform.GetScaleY();
	}

	// Token: 0x06001CC2 RID: 7362 RVA: 0x00085EED File Offset: 0x000840ED
	private void OnEnable()
	{
		this.rb2d = base.GetComponent<Rigidbody2D>();
		this.DoAngle();
	}

	// Token: 0x06001CC3 RID: 7363 RVA: 0x00085F01 File Offset: 0x00084101
	private void Update()
	{
		if (this.everyFrame)
		{
			this.DoAngle();
		}
	}

	// Token: 0x06001CC4 RID: 7364 RVA: 0x00085F14 File Offset: 0x00084114
	private void DoAngle()
	{
		Vector2 linearVelocity = this.rb2d.linearVelocity;
		if (linearVelocity.x != 0f && linearVelocity.y != 0f)
		{
			float num = Mathf.Atan2(linearVelocity.y, linearVelocity.x) * 57.295776f + this.angleOffset;
			if (num < 0f)
			{
				num += 360f;
			}
			if (num > 360f)
			{
				num -= 360f;
			}
			if (this.reflectY)
			{
				if (num > 90f && num < 270f)
				{
					if (base.transform.localScale.y != -this.yScale)
					{
						base.transform.SetScaleY(-this.yScale);
					}
				}
				else if (base.transform.localScale.y != this.yScale)
				{
					base.transform.SetScaleY(this.yScale);
				}
			}
			base.transform.localEulerAngles = new Vector3(0f, 0f, num);
		}
	}

	// Token: 0x04001C21 RID: 7201
	public float angleOffset;

	// Token: 0x04001C22 RID: 7202
	public bool everyFrame;

	// Token: 0x04001C23 RID: 7203
	public bool reflectY;

	// Token: 0x04001C24 RID: 7204
	private Rigidbody2D rb2d;

	// Token: 0x04001C25 RID: 7205
	private float yScale;
}
