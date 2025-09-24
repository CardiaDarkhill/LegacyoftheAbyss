using System;
using HutongGames.PlayMaker;

// Token: 0x02000307 RID: 775
[ActionCategory("Hollow Knight")]
public class GetCanSeeHero : FsmStateAction
{
	// Token: 0x06001B8E RID: 7054 RVA: 0x00080A71 File Offset: 0x0007EC71
	public override void Reset()
	{
		this.lineOfSightDetector = new FsmObject();
		this.storeResult = new FsmBool();
	}

	// Token: 0x06001B8F RID: 7055 RVA: 0x00080A89 File Offset: 0x0007EC89
	public override void OnEnter()
	{
		this.Apply();
		if (!this.everyFrame)
		{
			base.Finish();
		}
	}

	// Token: 0x06001B90 RID: 7056 RVA: 0x00080A9F File Offset: 0x0007EC9F
	public override void OnUpdate()
	{
		this.Apply();
	}

	// Token: 0x06001B91 RID: 7057 RVA: 0x00080AA8 File Offset: 0x0007ECA8
	private void Apply()
	{
		LineOfSightDetector lineOfSightDetector = this.lineOfSightDetector.Value as LineOfSightDetector;
		if (lineOfSightDetector != null)
		{
			if (!this.storeResult.IsNone)
			{
				this.storeResult.Value = lineOfSightDetector.CanSeeHero;
			}
			if (lineOfSightDetector.CanSeeHero)
			{
				base.Fsm.Event(this.eventTarget, this.sendEvent.Value);
				return;
			}
		}
		else if (!this.storeResult.IsNone)
		{
			this.storeResult.Value = false;
		}
	}

	// Token: 0x04001A8D RID: 6797
	[UIHint(UIHint.Variable)]
	[ObjectType(typeof(LineOfSightDetector))]
	public FsmObject lineOfSightDetector;

	// Token: 0x04001A8E RID: 6798
	[UIHint(UIHint.Variable)]
	public FsmBool storeResult;

	// Token: 0x04001A8F RID: 6799
	public FsmString sendEvent;

	// Token: 0x04001A90 RID: 6800
	public FsmEventTarget eventTarget;

	// Token: 0x04001A91 RID: 6801
	public bool everyFrame;
}
