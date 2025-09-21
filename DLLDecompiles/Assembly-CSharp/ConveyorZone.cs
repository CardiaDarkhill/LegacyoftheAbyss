using System;
using UnityEngine;

// Token: 0x020004C6 RID: 1222
public class ConveyorZone : MonoBehaviour
{
	// Token: 0x06002C02 RID: 11266 RVA: 0x000C0CCC File Offset: 0x000BEECC
	private void Start()
	{
		if (HeroController.instance)
		{
			this.activated = false;
			HeroController.HeroInPosition temp = null;
			temp = delegate(bool b)
			{
				this.activated = true;
				HeroController.instance.heroInPosition -= temp;
			};
			HeroController.instance.heroInPosition += temp;
		}
	}

	// Token: 0x06002C03 RID: 11267 RVA: 0x000C0D24 File Offset: 0x000BEF24
	private void OnDisable()
	{
		if (this.hasEntered)
		{
			if (this.conveyorMovement)
			{
				this.conveyorMovement.StopConveyorMove();
			}
			if (this.hc)
			{
				this.hc.GetComponent<ConveyorMovementHero>().StopConveyorMove();
				this.hc.cState.inConveyorZone = false;
				this.hc.cState.onConveyorV = false;
			}
			this.hasEntered = false;
			this.conveyorMovement = null;
			this.hc = null;
		}
	}

	// Token: 0x06002C04 RID: 11268 RVA: 0x000C0DA8 File Offset: 0x000BEFA8
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!this.activated)
		{
			return;
		}
		this.hasEntered = true;
		this.conveyorMovement = collision.GetComponent<ConveyorMovement>();
		if (this.conveyorMovement)
		{
			this.conveyorMovement.StartConveyorMove(this.speed, 0f);
		}
		this.hc = collision.GetComponent<HeroController>();
		if (this.hc)
		{
			if (this.vertical)
			{
				this.hc.GetComponent<ConveyorMovementHero>().StartConveyorMove(0f, this.speed);
				this.hc.cState.onConveyorV = true;
				return;
			}
			this.hc.SetConveyorSpeed(this.speed);
			this.hc.cState.inConveyorZone = true;
		}
	}

	// Token: 0x06002C05 RID: 11269 RVA: 0x000C0E64 File Offset: 0x000BF064
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (!this.activated)
		{
			return;
		}
		this.hasEntered = false;
		ConveyorMovement component = collision.GetComponent<ConveyorMovement>();
		if (component)
		{
			component.StopConveyorMove();
		}
		HeroController component2 = collision.GetComponent<HeroController>();
		if (component2)
		{
			component2.GetComponent<ConveyorMovementHero>().StopConveyorMove();
			component2.cState.inConveyorZone = false;
			component2.cState.onConveyorV = false;
		}
	}

	// Token: 0x06002C06 RID: 11270 RVA: 0x000C0EC8 File Offset: 0x000BF0C8
	public void SetSpeed(float speed_new)
	{
		this.speed = speed_new;
	}

	// Token: 0x06002C07 RID: 11271 RVA: 0x000C0ED1 File Offset: 0x000BF0D1
	public void SetActivated(bool activated_new)
	{
		this.activated = activated_new;
	}

	// Token: 0x04002D5D RID: 11613
	public float speed;

	// Token: 0x04002D5E RID: 11614
	public bool vertical;

	// Token: 0x04002D5F RID: 11615
	private bool activated = true;

	// Token: 0x04002D60 RID: 11616
	private ConveyorMovement conveyorMovement;

	// Token: 0x04002D61 RID: 11617
	private HeroController hc;

	// Token: 0x04002D62 RID: 11618
	private bool hasEntered;
}
