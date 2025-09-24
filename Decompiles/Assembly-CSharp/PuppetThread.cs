using System;
using UnityEngine;

// Token: 0x020000A1 RID: 161
public class PuppetThread : MonoBehaviour
{
	// Token: 0x060004F5 RID: 1269 RVA: 0x00019C8D File Offset: 0x00017E8D
	private void Awake()
	{
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		this.timer = Random.Range(2f, 4f);
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x00019CB0 File Offset: 0x00017EB0
	private void Update()
	{
		if (this.parented)
		{
			if (this.parent == null)
			{
				base.gameObject.SetActive(false);
			}
			if (this.timer < 0f)
			{
				float num = (float)Random.Range(1, 90);
				if (num < 31f)
				{
					this.animator.PlayFromFrame("String 1", 0);
				}
				else if (num < 61f)
				{
					this.animator.PlayFromFrame("String 2", 0);
				}
				else
				{
					this.animator.PlayFromFrame("String 3", 0);
				}
				this.timer = Random.Range(2f, 4f);
			}
			else
			{
				this.timer -= Time.deltaTime;
			}
			this.DoTilt();
			this.DoPosition();
		}
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x00019D75 File Offset: 0x00017F75
	public void SetParent(GameObject newParent, float new_xOffset, float new_yOffset)
	{
		this.parent = newParent;
		this.parent_rb = newParent.GetComponent<Rigidbody2D>();
		this.parent_transform = newParent.GetComponent<Transform>();
		this.xOffset = new_xOffset;
		this.yOffset = new_yOffset;
		this.parented = true;
	}

	// Token: 0x060004F8 RID: 1272 RVA: 0x00019DAC File Offset: 0x00017FAC
	private void DoTilt()
	{
		if (this.parent_rb != null)
		{
			float num = this.parent_rb.linearVelocity.x * -10f;
			if (num > 10f)
			{
				num = 10f;
			}
			if (num < -10f)
			{
				num = -10f;
			}
			float num2 = num - base.transform.localEulerAngles.z;
			bool flag;
			if (num2 < 0f)
			{
				flag = (num2 < -180f);
			}
			else
			{
				flag = (num2 <= 180f);
			}
			if (flag)
			{
				base.transform.Rotate(0f, 0f, 45f * Time.deltaTime);
				if (base.transform.localEulerAngles.z > num)
				{
					base.transform.localEulerAngles = new Vector3(base.transform.rotation.x, base.transform.rotation.y, num);
					return;
				}
			}
			else
			{
				base.transform.Rotate(0f, 0f, -45f * Time.deltaTime);
				if (base.transform.localEulerAngles.z < num)
				{
					base.transform.localEulerAngles = new Vector3(base.transform.rotation.x, base.transform.rotation.y, num);
				}
			}
		}
	}

	// Token: 0x060004F9 RID: 1273 RVA: 0x00019F08 File Offset: 0x00018108
	private void DoPosition()
	{
		if (this.parent_transform != null)
		{
			base.transform.position = new Vector3(this.parent_transform.position.x + this.xOffset, this.parent_transform.position.y + this.yOffset, 0.009123f);
		}
	}

	// Token: 0x040004C8 RID: 1224
	private tk2dSpriteAnimator animator;

	// Token: 0x040004C9 RID: 1225
	private float timer;

	// Token: 0x040004CA RID: 1226
	private GameObject parent;

	// Token: 0x040004CB RID: 1227
	private Transform parent_transform;

	// Token: 0x040004CC RID: 1228
	private Rigidbody2D parent_rb;

	// Token: 0x040004CD RID: 1229
	private bool parented;

	// Token: 0x040004CE RID: 1230
	private float xOffset;

	// Token: 0x040004CF RID: 1231
	private float yOffset;

	// Token: 0x040004D0 RID: 1232
	private const float tiltMax = 10f;

	// Token: 0x040004D1 RID: 1233
	private const float tiltFactor = -10f;

	// Token: 0x040004D2 RID: 1234
	private const float rotationSpeed = 45f;
}
