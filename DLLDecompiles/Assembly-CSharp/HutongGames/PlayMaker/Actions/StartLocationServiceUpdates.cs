using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E8E RID: 3726
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Starts location service updates. Last location coordinates can be retrieved with {{GetLocationInfo}}.")]
	public class StartLocationServiceUpdates : FsmStateAction
	{
		// Token: 0x060069DC RID: 27100 RVA: 0x00211C55 File Offset: 0x0020FE55
		public override void Reset()
		{
			this.maxWait = 20f;
			this.desiredAccuracy = 10f;
			this.updateDistance = 10f;
			this.successEvent = null;
			this.failedEvent = null;
		}

		// Token: 0x060069DD RID: 27101 RVA: 0x00211C95 File Offset: 0x0020FE95
		public override void OnEnter()
		{
			base.Finish();
		}

		// Token: 0x060069DE RID: 27102 RVA: 0x00211C9D File Offset: 0x0020FE9D
		public override void OnUpdate()
		{
		}

		// Token: 0x04006917 RID: 26903
		[Tooltip("Maximum time to wait in seconds before failing.")]
		public FsmFloat maxWait;

		// Token: 0x04006918 RID: 26904
		[Tooltip("The desired accuracy in meters.")]
		public FsmFloat desiredAccuracy;

		// Token: 0x04006919 RID: 26905
		[Tooltip("Distance between updates in meters.")]
		public FsmFloat updateDistance;

		// Token: 0x0400691A RID: 26906
		[Tooltip("Event to send when the location services have started.")]
		public FsmEvent successEvent;

		// Token: 0x0400691B RID: 26907
		[Tooltip("Event to send if the location services fail to start.")]
		public FsmEvent failedEvent;
	}
}
