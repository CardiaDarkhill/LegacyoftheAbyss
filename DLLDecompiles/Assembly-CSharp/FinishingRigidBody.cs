using System;
using UnityEngine;

// Token: 0x02000338 RID: 824
[RequireComponent(typeof(Rigidbody2D))]
public class FinishingRigidBody : MonoBehaviour
{
	// Token: 0x06001CD0 RID: 7376 RVA: 0x000861D6 File Offset: 0x000843D6
	protected void Reset()
	{
		this.waitDuration = 8f;
		this.shrinkDuration = 1f;
		this.conclusion = FinishingRigidBody.Conclusions.Disable;
	}

	// Token: 0x06001CD1 RID: 7377 RVA: 0x000861F5 File Offset: 0x000843F5
	protected void Awake()
	{
		this.rend = base.GetComponent<Renderer>();
		this.body = base.GetComponent<Rigidbody2D>();
	}

	// Token: 0x06001CD2 RID: 7378 RVA: 0x0008620F File Offset: 0x0008440F
	protected void OnEnable()
	{
		this.state = FinishingRigidBody.States.Ready;
		this.timer = 0f;
		this.framesEnabled = 0;
	}

	// Token: 0x06001CD3 RID: 7379 RVA: 0x0008622C File Offset: 0x0008442C
	protected void Update()
	{
		if (this.state == FinishingRigidBody.States.Ready && !this.body.IsAwake())
		{
			this.timer += Time.deltaTime;
			if (this.timer > this.waitDuration)
			{
				this.timer = 0f;
				this.state = FinishingRigidBody.States.Shrinking;
				this.shrinkStartScale = base.transform.localScale;
			}
		}
		if (this.state == FinishingRigidBody.States.Shrinking)
		{
			this.timer += Time.deltaTime;
			if (this.timer > this.shrinkDuration)
			{
				this.Conclude();
				return;
			}
			float d = 1f - Mathf.Clamp01(this.timer / this.shrinkDuration);
			base.transform.localScale = d * this.shrinkStartScale;
		}
		if (!this.persistOffScreen && this.rend != null && !this.rend.isVisible && this.framesEnabled > 10)
		{
			this.Conclude();
			return;
		}
		this.framesEnabled++;
	}

	// Token: 0x06001CD4 RID: 7380 RVA: 0x00086334 File Offset: 0x00084534
	private void Conclude()
	{
		if (this.state == FinishingRigidBody.States.Shrinking)
		{
			base.transform.localScale = this.shrinkStartScale;
		}
		this.state = FinishingRigidBody.States.Concluded;
		if (this.conclusion == FinishingRigidBody.Conclusions.Disable)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (this.conclusion == FinishingRigidBody.Conclusions.Recycle)
		{
			base.gameObject.Recycle();
			return;
		}
		if (this.conclusion == FinishingRigidBody.Conclusions.Destroy)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x04001C2D RID: 7213
	[SerializeField]
	private float waitDuration;

	// Token: 0x04001C2E RID: 7214
	[SerializeField]
	private float shrinkDuration;

	// Token: 0x04001C2F RID: 7215
	[SerializeField]
	private FinishingRigidBody.Conclusions conclusion;

	// Token: 0x04001C30 RID: 7216
	[SerializeField]
	private bool persistOffScreen;

	// Token: 0x04001C31 RID: 7217
	private Renderer rend;

	// Token: 0x04001C32 RID: 7218
	private Rigidbody2D body;

	// Token: 0x04001C33 RID: 7219
	private FinishingRigidBody.States state;

	// Token: 0x04001C34 RID: 7220
	private Vector3 shrinkStartScale;

	// Token: 0x04001C35 RID: 7221
	private float timer;

	// Token: 0x04001C36 RID: 7222
	private int framesEnabled;

	// Token: 0x02001601 RID: 5633
	private enum Conclusions
	{
		// Token: 0x0400896C RID: 35180
		Disable,
		// Token: 0x0400896D RID: 35181
		Recycle,
		// Token: 0x0400896E RID: 35182
		Destroy
	}

	// Token: 0x02001602 RID: 5634
	private enum States
	{
		// Token: 0x04008970 RID: 35184
		Ready,
		// Token: 0x04008971 RID: 35185
		Concluded,
		// Token: 0x04008972 RID: 35186
		Shrinking
	}
}
