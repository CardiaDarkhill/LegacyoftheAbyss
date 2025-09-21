using System;
using UnityEngine;

// Token: 0x02000628 RID: 1576
public class CursorHighlight : MonoBehaviour
{
	// Token: 0x0600382A RID: 14378 RVA: 0x000F82DC File Offset: 0x000F64DC
	public void Awake()
	{
		this.myRect = base.GetComponent<RectTransform>();
	}

	// Token: 0x0600382B RID: 14379 RVA: 0x000F82EA File Offset: 0x000F64EA
	private void Start()
	{
		this.lerpTimer = this.lerpTime;
		this.cooldownTimer = 0f;
		this.startPos = this.myRect.anchoredPosition;
		this.targetPos = this.startPos;
		this.coolingDown = false;
	}

	// Token: 0x0600382C RID: 14380 RVA: 0x000F8328 File Offset: 0x000F6528
	private void Update()
	{
		if (!this.coolingDown)
		{
			if (this.lerpTimer > this.lerpTime)
			{
				this.lerpTimer = this.lerpTime;
				this.coolingDown = true;
			}
			else if (this.lerpTimer < this.lerpTime)
			{
				this.lerpTimer += Time.deltaTime;
			}
			float num = this.lerpTimer / this.lerpTime;
			num = num * num * (3f - 2f * num);
			this.myRect.anchoredPosition = Vector2.Lerp(this.startPos, this.targetPos, num);
			return;
		}
		if (this.cooldownTimer > this.cursorCooldown)
		{
			this.coolingDown = false;
			this.cooldownTimer = 0f;
			return;
		}
		this.cooldownTimer += Time.deltaTime;
	}

	// Token: 0x0600382D RID: 14381 RVA: 0x000F83F1 File Offset: 0x000F65F1
	public void MoveCursor(RectTransform buttonRect)
	{
		if (!this.coolingDown)
		{
			this.startPos = this.myRect.anchoredPosition;
			this.targetPos = buttonRect.anchoredPosition;
			this.lerpTimer = 0f;
		}
	}

	// Token: 0x04003B1F RID: 15135
	private RectTransform myRect;

	// Token: 0x04003B20 RID: 15136
	private Vector2 startPos;

	// Token: 0x04003B21 RID: 15137
	private Vector2 targetPos;

	// Token: 0x04003B22 RID: 15138
	[Tooltip("The time it takes for the cursor to move from one option to another.")]
	public float lerpTime = 1f;

	// Token: 0x04003B23 RID: 15139
	[Tooltip("The wait period between the cursor moving from one option to another.")]
	public float cursorCooldown = 0.1f;

	// Token: 0x04003B24 RID: 15140
	private float lerpTimer;

	// Token: 0x04003B25 RID: 15141
	private float cooldownTimer;

	// Token: 0x04003B26 RID: 15142
	private bool coolingDown;
}
