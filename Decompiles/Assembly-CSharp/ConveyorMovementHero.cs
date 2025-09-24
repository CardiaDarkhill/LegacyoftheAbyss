using System;
using UnityEngine;

// Token: 0x020004C4 RID: 1220
public class ConveyorMovementHero : MonoBehaviour
{
	// Token: 0x06002BFB RID: 11259 RVA: 0x000C0B43 File Offset: 0x000BED43
	private void Start()
	{
		this.heroCon = base.GetComponent<HeroController>();
	}

	// Token: 0x06002BFC RID: 11260 RVA: 0x000C0B51 File Offset: 0x000BED51
	public void StartConveyorMove(float cXSpeed, float cYSpeed)
	{
		this.onConveyor = true;
		this.xSpeed = cXSpeed;
		this.ySpeed = cYSpeed;
	}

	// Token: 0x06002BFD RID: 11261 RVA: 0x000C0B68 File Offset: 0x000BED68
	public void StopConveyorMove()
	{
		this.onConveyor = false;
		if (this.gravityOff)
		{
			if (!this.heroCon.cState.superDashing)
			{
				this.heroCon.AffectedByGravity(true);
			}
			this.gravityOff = false;
		}
	}

	// Token: 0x06002BFE RID: 11262 RVA: 0x000C0BA0 File Offset: 0x000BEDA0
	private void LateUpdate()
	{
		if (this.onConveyor)
		{
			if (Math.Abs(this.ySpeed) > 0.001f && (this.heroCon.cState.wallSliding || this.heroCon.cState.superDashOnWall))
			{
				if (this.heroCon.cState.superDashOnWall)
				{
					base.GetComponent<Rigidbody2D>().linearVelocity = new Vector3(0f, 0f);
				}
				base.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(base.GetComponent<Rigidbody2D>().linearVelocity.x, this.ySpeed);
				if (!this.gravityOff)
				{
					this.heroCon.AffectedByGravity(false);
					this.gravityOff = true;
					return;
				}
			}
			else if (this.gravityOff && !this.heroCon.cState.superDashing)
			{
				this.heroCon.AffectedByGravity(true);
				this.gravityOff = false;
			}
		}
	}

	// Token: 0x04002D57 RID: 11607
	[SerializeField]
	private bool gravityOff;

	// Token: 0x04002D58 RID: 11608
	private float xSpeed;

	// Token: 0x04002D59 RID: 11609
	private float ySpeed;

	// Token: 0x04002D5A RID: 11610
	private bool onConveyor;

	// Token: 0x04002D5B RID: 11611
	private HeroController heroCon;
}
