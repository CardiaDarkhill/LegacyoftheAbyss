using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E8A RID: 3722
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Gets Location Info from a mobile device. NOTE: Use StartLocationService before trying to get location info.")]
	public class GetLocationInfo : FsmStateAction
	{
		// Token: 0x060069C6 RID: 27078 RVA: 0x002115FC File Offset: 0x0020F7FC
		public override void Reset()
		{
			this.longitude = null;
			this.latitude = null;
			this.altitude = null;
			this.horizontalAccuracy = null;
			this.verticalAccuracy = null;
			this.errorEvent = null;
		}

		// Token: 0x060069C7 RID: 27079 RVA: 0x00211628 File Offset: 0x0020F828
		public override void OnEnter()
		{
			this.DoGetLocationInfo();
			base.Finish();
		}

		// Token: 0x060069C8 RID: 27080 RVA: 0x00211636 File Offset: 0x0020F836
		private void DoGetLocationInfo()
		{
		}

		// Token: 0x040068F1 RID: 26865
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the location in a Vector3 Variable.")]
		public FsmVector3 vectorPosition;

		// Token: 0x040068F2 RID: 26866
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Longitude in a Float Variable.")]
		public FsmFloat longitude;

		// Token: 0x040068F3 RID: 26867
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Latitude in a Float Variable.")]
		public FsmFloat latitude;

		// Token: 0x040068F4 RID: 26868
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Altitude in a Float Variable.")]
		public FsmFloat altitude;

		// Token: 0x040068F5 RID: 26869
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the horizontal accuracy of the location.")]
		public FsmFloat horizontalAccuracy;

		// Token: 0x040068F6 RID: 26870
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the vertical accuracy of the location.")]
		public FsmFloat verticalAccuracy;

		// Token: 0x040068F7 RID: 26871
		[Tooltip("Event to send if the location cannot be queried.")]
		public FsmEvent errorEvent;
	}
}
