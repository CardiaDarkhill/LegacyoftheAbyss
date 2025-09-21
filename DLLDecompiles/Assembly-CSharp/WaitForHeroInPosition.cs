using System;
using HutongGames.PlayMaker;

// Token: 0x0200003D RID: 61
[ActionCategory("Hollow Knight")]
public class WaitForHeroInPosition : FsmStateAction
{
	// Token: 0x060001A0 RID: 416 RVA: 0x00008D0B File Offset: 0x00006F0B
	public override void Reset()
	{
		this.sendEvent = null;
		this.skipIfAlreadyPositioned = new FsmBool(false);
	}

	// Token: 0x060001A1 RID: 417 RVA: 0x00008D28 File Offset: 0x00006F28
	public override void OnEnter()
	{
		if (HeroController.instance && !HeroController.instance.isHeroInPosition)
		{
			HeroController.instance.heroInPosition += this.DoHeroInPosition;
			this.subscribed = true;
			return;
		}
		if (this.skipIfAlreadyPositioned.Value)
		{
			base.Fsm.Event(this.sendEvent);
		}
		base.Finish();
	}

	// Token: 0x060001A2 RID: 418 RVA: 0x00008D90 File Offset: 0x00006F90
	public override void OnExit()
	{
		if (this.subscribed)
		{
			HeroController silentInstance = HeroController.SilentInstance;
			if (silentInstance)
			{
				silentInstance.heroInPosition -= this.DoHeroInPosition;
			}
			this.subscribed = false;
		}
	}

	// Token: 0x060001A3 RID: 419 RVA: 0x00008DCC File Offset: 0x00006FCC
	private void DoHeroInPosition(bool forceDirect)
	{
		base.Fsm.Event(this.sendEvent);
		base.Finish();
		HeroController.instance.heroInPosition -= this.DoHeroInPosition;
		this.subscribed = false;
	}

	// Token: 0x04000174 RID: 372
	public FsmEvent sendEvent;

	// Token: 0x04000175 RID: 373
	public FsmBool skipIfAlreadyPositioned;

	// Token: 0x04000176 RID: 374
	private bool subscribed;
}
