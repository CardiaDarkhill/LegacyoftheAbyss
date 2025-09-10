using System;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x020007DC RID: 2012
public class WaveEffectControl : MonoBehaviour
{
	// Token: 0x060046AA RID: 18090 RVA: 0x0013B1D8 File Offset: 0x001393D8
	private void Awake()
	{
		this.spriteRenderer = base.GetComponent<SpriteRenderer>();
		this.UpdateEffect();
	}

	// Token: 0x060046AB RID: 18091 RVA: 0x0013B1EC File Offset: 0x001393EC
	private void Start()
	{
		this.groupBridge = this.spriteRenderer.GetComponent<NestedFadeGroupSpriteRenderer>();
		this.hasGroupBridge = (this.groupBridge != null);
		this.started = true;
	}

	// Token: 0x060046AC RID: 18092 RVA: 0x0013B218 File Offset: 0x00139418
	private void OnEnable()
	{
		this.timer = 0f;
		if (this.blackWave)
		{
			this.colour = new Color(0f, 0f, 0f, 1f);
		}
		else if (!this.otherColour)
		{
			this.colour = new Color(1f, 1f, 1f, 1f);
		}
		this.accel = this.accelStart;
		if (!this.doNotPositionZ)
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, 0.1f);
		}
		if (this.started)
		{
			this.UpdateEffect();
		}
	}

	// Token: 0x060046AD RID: 18093 RVA: 0x0013B2D8 File Offset: 0x001394D8
	private void Update()
	{
		this.timer += Time.deltaTime * this.accel;
		this.UpdateEffect();
		if (this.timer > 1f)
		{
			if (!this.doNotRecycle)
			{
				base.gameObject.Recycle();
				return;
			}
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x060046AE RID: 18094 RVA: 0x0013B334 File Offset: 0x00139534
	private void UpdateEffect()
	{
		float num = (1f + this.timer * 4f) * this.scaleMultiplier;
		base.transform.localScale = new Vector3(num, num, num);
		Color color = this.spriteRenderer.color;
		color.a = 1f - this.timer;
		if (this.hasGroupBridge)
		{
			this.groupBridge.Color = color;
			return;
		}
		this.spriteRenderer.color = color;
	}

	// Token: 0x060046AF RID: 18095 RVA: 0x0013B3AE File Offset: 0x001395AE
	private void FixedUpdate()
	{
		this.accel *= 0.95f;
		if (this.accel < 0.5f)
		{
			this.accel = 0.5f;
		}
	}

	// Token: 0x04004709 RID: 18185
	private float timer;

	// Token: 0x0400470A RID: 18186
	public Color colour;

	// Token: 0x0400470B RID: 18187
	public SpriteRenderer spriteRenderer;

	// Token: 0x0400470C RID: 18188
	public float accel;

	// Token: 0x0400470D RID: 18189
	public float accelStart = 5f;

	// Token: 0x0400470E RID: 18190
	public bool doNotRecycle;

	// Token: 0x0400470F RID: 18191
	public bool doNotPositionZ;

	// Token: 0x04004710 RID: 18192
	public bool blackWave;

	// Token: 0x04004711 RID: 18193
	public bool otherColour;

	// Token: 0x04004712 RID: 18194
	public float scaleMultiplier = 1f;

	// Token: 0x04004713 RID: 18195
	private NestedFadeGroupSpriteRenderer groupBridge;

	// Token: 0x04004714 RID: 18196
	private bool started;

	// Token: 0x04004715 RID: 18197
	private bool hasGroupBridge;
}
