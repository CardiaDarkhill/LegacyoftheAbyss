using System;
using UnityEngine;

// Token: 0x020004FC RID: 1276
[RequireComponent(typeof(Collider2D))]
public class HeroTouchForce : MonoBehaviour
{
	// Token: 0x06002DA4 RID: 11684 RVA: 0x000C78DD File Offset: 0x000C5ADD
	private void Awake()
	{
		this.collider = base.GetComponent<Collider2D>();
		if (this.collider && !base.transform.IsOnHeroPlane())
		{
			this.collider.enabled = false;
			Object.Destroy(this);
		}
	}

	// Token: 0x06002DA5 RID: 11685 RVA: 0x000C7917 File Offset: 0x000C5B17
	private void Start()
	{
		this.hero = HeroController.instance.transform;
	}

	// Token: 0x06002DA6 RID: 11686 RVA: 0x000C7929 File Offset: 0x000C5B29
	private void OnCollisionEnter2D()
	{
		if (this.disableTimeLeft <= 0f)
		{
			this.disableTimeLeft = this.touchTime;
		}
	}

	// Token: 0x06002DA7 RID: 11687 RVA: 0x000C7944 File Offset: 0x000C5B44
	private void Update()
	{
		if (this.collider.enabled)
		{
			if (this.disableTimeLeft > 0f)
			{
				this.disableTimeLeft -= Time.deltaTime;
				if (this.disableTimeLeft <= 0f)
				{
					this.collider.enabled = false;
					return;
				}
			}
		}
		else
		{
			if (Mathf.Abs(this.hero.position.x - base.transform.position.x) < this.playerRangeEnable)
			{
				this.enableTimeLeft = this.playerRangeEnableDelay;
				return;
			}
			if (this.enableTimeLeft > 0f)
			{
				this.enableTimeLeft -= Time.deltaTime;
				if (this.enableTimeLeft <= 0f)
				{
					this.collider.enabled = true;
				}
			}
		}
	}

	// Token: 0x04002F7D RID: 12157
	[SerializeField]
	private float touchTime;

	// Token: 0x04002F7E RID: 12158
	[SerializeField]
	private float playerRangeEnable;

	// Token: 0x04002F7F RID: 12159
	[SerializeField]
	private float playerRangeEnableDelay;

	// Token: 0x04002F80 RID: 12160
	private float disableTimeLeft;

	// Token: 0x04002F81 RID: 12161
	private float enableTimeLeft;

	// Token: 0x04002F82 RID: 12162
	private Transform hero;

	// Token: 0x04002F83 RID: 12163
	private Collider2D collider;
}
