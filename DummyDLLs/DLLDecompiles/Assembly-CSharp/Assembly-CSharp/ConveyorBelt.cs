using System;
using UnityEngine;

// Token: 0x020004C1 RID: 1217
public class ConveyorBelt : MonoBehaviour
{
	// Token: 0x1400008A RID: 138
	// (add) Token: 0x06002BEC RID: 11244 RVA: 0x000C07E8 File Offset: 0x000BE9E8
	// (remove) Token: 0x06002BED RID: 11245 RVA: 0x000C0820 File Offset: 0x000BEA20
	public event Action<HeroController> CapturedHero;

	// Token: 0x17000517 RID: 1303
	// (get) Token: 0x06002BEE RID: 11246 RVA: 0x000C0855 File Offset: 0x000BEA55
	// (set) Token: 0x06002BEF RID: 11247 RVA: 0x000C085D File Offset: 0x000BEA5D
	public float SpeedMultiplier
	{
		get
		{
			return this.speedMultiplier;
		}
		set
		{
			this.speedMultiplier = value;
			this.UpdateHero();
		}
	}

	// Token: 0x06002BF0 RID: 11248 RVA: 0x000C086C File Offset: 0x000BEA6C
	private void OnCollisionEnter2D(Collision2D collision)
	{
		float c_xSpeed = this.speed * this.speedMultiplier;
		if (collision.gameObject.GetComponent<ConveyorMovement>())
		{
			collision.gameObject.GetComponent<ConveyorMovement>().StartConveyorMove(c_xSpeed, 0f);
		}
		if (collision.gameObject.GetComponent<DropCrystal>())
		{
			collision.gameObject.GetComponent<DropCrystal>().StartConveyorMove(c_xSpeed, 0f);
		}
		HeroController component = collision.gameObject.GetComponent<HeroController>();
		if (component)
		{
			this.hc = component;
			this.hcConveyorMove = this.hc.GetComponent<ConveyorMovementHero>();
			this.UpdateHero();
			if (this.CapturedHero != null)
			{
				this.CapturedHero(this.hc);
			}
		}
	}

	// Token: 0x06002BF1 RID: 11249 RVA: 0x000C0924 File Offset: 0x000BEB24
	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.GetComponent<ConveyorMovement>())
		{
			collision.gameObject.GetComponent<ConveyorMovement>().StopConveyorMove();
		}
		if (collision.gameObject.GetComponent<DropCrystal>())
		{
			collision.gameObject.GetComponent<DropCrystal>().StopConveyorMove();
		}
		HeroController component = collision.gameObject.GetComponent<HeroController>();
		if (component != null && component == this.hc)
		{
			this.hcConveyorMove.StopConveyorMove();
			this.hc.cState.onConveyor = false;
			this.hc.cState.onConveyorV = false;
			this.hc = null;
			this.hcConveyorMove = null;
			if (this.CapturedHero != null)
			{
				this.CapturedHero(null);
			}
		}
	}

	// Token: 0x06002BF2 RID: 11250 RVA: 0x000C09E8 File Offset: 0x000BEBE8
	private void UpdateHero()
	{
		if (!this.hc)
		{
			return;
		}
		float num = this.speed * this.speedMultiplier;
		if (this.vertical)
		{
			this.hcConveyorMove.StartConveyorMove(0f, num);
			this.hc.cState.onConveyorV = true;
			return;
		}
		this.hc.SetConveyorSpeed(num);
		this.hc.cState.onConveyor = true;
	}

	// Token: 0x04002D4F RID: 11599
	[SerializeField]
	private float speed;

	// Token: 0x04002D50 RID: 11600
	[SerializeField]
	private bool vertical;

	// Token: 0x04002D51 RID: 11601
	private float speedMultiplier = 1f;

	// Token: 0x04002D52 RID: 11602
	private HeroController hc;

	// Token: 0x04002D53 RID: 11603
	private ConveyorMovementHero hcConveyorMove;
}
