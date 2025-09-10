using System;
using HutongGames.PlayMaker;

// Token: 0x02000308 RID: 776
[ActionCategory("Hollow Knight")]
public class GetCanSeeHeroV2 : FsmStateAction
{
	// Token: 0x06001B93 RID: 7059 RVA: 0x00080B33 File Offset: 0x0007ED33
	public override void Reset()
	{
		this.lineOfSightDetector = new FsmGameObject();
		this.storeResult = new FsmBool();
	}

	// Token: 0x06001B94 RID: 7060 RVA: 0x00080B4B File Offset: 0x0007ED4B
	public override void OnEnter()
	{
		this.Apply();
		if (!this.everyFrame)
		{
			base.Finish();
		}
		this.detector = this.lineOfSightDetector.Value.GetComponent<LineOfSightDetector>();
	}

	// Token: 0x06001B95 RID: 7061 RVA: 0x00080B77 File Offset: 0x0007ED77
	public override void OnUpdate()
	{
		this.Apply();
	}

	// Token: 0x06001B96 RID: 7062 RVA: 0x00080B80 File Offset: 0x0007ED80
	private void Apply()
	{
		if (!(this.detector != null))
		{
			if (!this.storeResult.IsNone)
			{
				this.storeResult.Value = false;
			}
			return;
		}
		if (!this.storeResult.IsNone)
		{
			this.storeResult.Value = this.detector.CanSeeHero;
		}
		if (this.detector.CanSeeHero)
		{
			base.Fsm.Event(this.eventTarget, this.inRangeEvent.Value);
			return;
		}
		base.Fsm.Event(this.eventTarget, this.outOfRangeEvent.Value);
	}

	// Token: 0x04001A92 RID: 6802
	[UIHint(UIHint.Variable)]
	[ObjectType(typeof(LineOfSightDetector))]
	public FsmGameObject lineOfSightDetector;

	// Token: 0x04001A93 RID: 6803
	[UIHint(UIHint.Variable)]
	public FsmBool storeResult;

	// Token: 0x04001A94 RID: 6804
	public FsmString inRangeEvent;

	// Token: 0x04001A95 RID: 6805
	public FsmString outOfRangeEvent;

	// Token: 0x04001A96 RID: 6806
	public FsmEventTarget eventTarget;

	// Token: 0x04001A97 RID: 6807
	private LineOfSightDetector detector;

	// Token: 0x04001A98 RID: 6808
	public bool everyFrame;
}
