using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000309 RID: 777
[ActionCategory("Hollow Knight")]
public class GetCanSeeHeroDelayed : FsmStateAction
{
	// Token: 0x06001B98 RID: 7064 RVA: 0x00080C26 File Offset: 0x0007EE26
	public override void Reset()
	{
		this.LineOfSightDetector = new FsmGameObject
		{
			UseVariable = true
		};
		this.StoreResult = null;
		this.InRangeEvent = null;
		this.OutOfRangeEvent = null;
		this.OutOfRangeDelay = null;
	}

	// Token: 0x06001B99 RID: 7065 RVA: 0x00080C56 File Offset: 0x0007EE56
	public override void OnEnter()
	{
		this.detector = this.LineOfSightDetector.Value.GetComponent<LineOfSightDetector>();
	}

	// Token: 0x06001B9A RID: 7066 RVA: 0x00080C6E File Offset: 0x0007EE6E
	public override void OnUpdate()
	{
		this.DoAction();
	}

	// Token: 0x06001B9B RID: 7067 RVA: 0x00080C78 File Offset: 0x0007EE78
	private void DoAction()
	{
		bool flag = this.detector.CanSeeHero;
		this.outOfRangeTimer -= Time.deltaTime;
		if (flag)
		{
			this.outOfRangeTimer = this.OutOfRangeDelay.Value;
		}
		else if (this.outOfRangeTimer > 0f)
		{
			flag = true;
		}
		if (this.detector != null)
		{
			if (!this.StoreResult.IsNone)
			{
				this.StoreResult.Value = flag;
			}
			base.Fsm.Event(flag ? this.InRangeEvent : this.OutOfRangeEvent);
			return;
		}
		if (!this.StoreResult.IsNone)
		{
			this.StoreResult.Value = false;
		}
	}

	// Token: 0x04001A99 RID: 6809
	[CheckForComponent(typeof(LineOfSightDetector))]
	public FsmGameObject LineOfSightDetector;

	// Token: 0x04001A9A RID: 6810
	[UIHint(UIHint.Variable)]
	public FsmBool StoreResult;

	// Token: 0x04001A9B RID: 6811
	public FsmEvent InRangeEvent;

	// Token: 0x04001A9C RID: 6812
	public FsmEvent OutOfRangeEvent;

	// Token: 0x04001A9D RID: 6813
	public FsmFloat OutOfRangeDelay;

	// Token: 0x04001A9E RID: 6814
	private float outOfRangeTimer;

	// Token: 0x04001A9F RID: 6815
	private LineOfSightDetector detector;
}
