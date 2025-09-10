using System;
using UnityEngine;

// Token: 0x020003F7 RID: 1015
public class JitterSelfSimple : MonoBehaviour
{
	// Token: 0x060022A3 RID: 8867 RVA: 0x0009F44E File Offset: 0x0009D64E
	private void Start()
	{
		this.Init();
	}

	// Token: 0x060022A4 RID: 8868 RVA: 0x0009F456 File Offset: 0x0009D656
	private void OnEnable()
	{
		this.Init();
	}

	// Token: 0x060022A5 RID: 8869 RVA: 0x0009F460 File Offset: 0x0009D660
	private void Init()
	{
		this.startX = base.transform.position.x;
		this.startY = base.transform.position.y;
		if (this.startActive)
		{
			this.jittering = true;
			return;
		}
		this.jittering = false;
	}

	// Token: 0x060022A6 RID: 8870 RVA: 0x0009F4B0 File Offset: 0x0009D6B0
	private void Update()
	{
		if (this.jittering)
		{
			if (this.timer < this.frameTime)
			{
				this.timer += Time.deltaTime;
				return;
			}
			if (this.up)
			{
				base.transform.position = new Vector3(this.startX + this.jitterX.x, this.startY + this.jitterY.x, base.transform.position.z);
				this.up = false;
			}
			else
			{
				base.transform.position = new Vector3(this.startX + this.jitterX.y, this.startY + this.jitterY.y, base.transform.position.z);
				this.up = true;
			}
			this.timer -= this.frameTime;
		}
	}

	// Token: 0x060022A7 RID: 8871 RVA: 0x0009F59C File Offset: 0x0009D79C
	public void StartJitter()
	{
		this.jittering = true;
	}

	// Token: 0x060022A8 RID: 8872 RVA: 0x0009F5A5 File Offset: 0x0009D7A5
	public void StopJitter()
	{
		base.transform.position = new Vector3(this.startX, this.startY, base.transform.position.z);
		this.jittering = false;
	}

	// Token: 0x04002177 RID: 8567
	public float frameTime = 0.1f;

	// Token: 0x04002178 RID: 8568
	public Vector2 jitterX;

	// Token: 0x04002179 RID: 8569
	public Vector2 jitterY;

	// Token: 0x0400217A RID: 8570
	public bool startActive;

	// Token: 0x0400217B RID: 8571
	private float startX;

	// Token: 0x0400217C RID: 8572
	private float startY;

	// Token: 0x0400217D RID: 8573
	private bool up;

	// Token: 0x0400217E RID: 8574
	private float timer;

	// Token: 0x0400217F RID: 8575
	private bool jittering;
}
