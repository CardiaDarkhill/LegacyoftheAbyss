using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E8D RID: 3725
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Projects the location found with Get Location Info to a 2d map using common projections.")]
	public class ProjectLocationToMap : FsmStateAction
	{
		// Token: 0x060069D4 RID: 27092 RVA: 0x002118DC File Offset: 0x0020FADC
		public override void Reset()
		{
			this.GPSLocation = new FsmVector3
			{
				UseVariable = true
			};
			this.mapProjection = ProjectLocationToMap.MapProjection.EquidistantCylindrical;
			this.minLongitude = -180f;
			this.maxLongitude = 180f;
			this.minLatitude = -90f;
			this.maxLatitude = 90f;
			this.minX = 0f;
			this.minY = 0f;
			this.width = 1f;
			this.height = 1f;
			this.normalized = true;
			this.projectedX = null;
			this.projectedY = null;
			this.everyFrame = false;
		}

		// Token: 0x060069D5 RID: 27093 RVA: 0x002119A3 File Offset: 0x0020FBA3
		public override void OnEnter()
		{
			if (this.GPSLocation.IsNone)
			{
				base.Finish();
				return;
			}
			this.DoProjectGPSLocation();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060069D6 RID: 27094 RVA: 0x002119CD File Offset: 0x0020FBCD
		public override void OnUpdate()
		{
			this.DoProjectGPSLocation();
		}

		// Token: 0x060069D7 RID: 27095 RVA: 0x002119D8 File Offset: 0x0020FBD8
		private void DoProjectGPSLocation()
		{
			this.x = Mathf.Clamp(this.GPSLocation.Value.x, this.minLongitude.Value, this.maxLongitude.Value);
			this.y = Mathf.Clamp(this.GPSLocation.Value.y, this.minLatitude.Value, this.maxLatitude.Value);
			ProjectLocationToMap.MapProjection mapProjection = this.mapProjection;
			if (mapProjection != ProjectLocationToMap.MapProjection.EquidistantCylindrical)
			{
				if (mapProjection == ProjectLocationToMap.MapProjection.Mercator)
				{
					this.DoMercatorProjection();
				}
			}
			else
			{
				this.DoEquidistantCylindrical();
			}
			this.x *= this.width.Value;
			this.y *= this.height.Value;
			this.projectedX.Value = (this.normalized.Value ? (this.minX.Value + this.x) : (this.minX.Value + this.x * (float)Screen.width));
			this.projectedY.Value = (this.normalized.Value ? (this.minY.Value + this.y) : (this.minY.Value + this.y * (float)Screen.height));
		}

		// Token: 0x060069D8 RID: 27096 RVA: 0x00211B20 File Offset: 0x0020FD20
		private void DoEquidistantCylindrical()
		{
			this.x = (this.x - this.minLongitude.Value) / (this.maxLongitude.Value - this.minLongitude.Value);
			this.y = (this.y - this.minLatitude.Value) / (this.maxLatitude.Value - this.minLatitude.Value);
		}

		// Token: 0x060069D9 RID: 27097 RVA: 0x00211B90 File Offset: 0x0020FD90
		private void DoMercatorProjection()
		{
			this.x = (this.x - this.minLongitude.Value) / (this.maxLongitude.Value - this.minLongitude.Value);
			float num = ProjectLocationToMap.LatitudeToMercator(this.minLatitude.Value);
			float num2 = ProjectLocationToMap.LatitudeToMercator(this.maxLatitude.Value);
			this.y = (ProjectLocationToMap.LatitudeToMercator(this.GPSLocation.Value.y) - num) / (num2 - num);
		}

		// Token: 0x060069DA RID: 27098 RVA: 0x00211C10 File Offset: 0x0020FE10
		private static float LatitudeToMercator(float latitudeInDegrees)
		{
			float num = Mathf.Clamp(latitudeInDegrees, -85f, 85f);
			num = 0.017453292f * num;
			return Mathf.Log(Mathf.Tan(num / 2f + 0.7853982f));
		}

		// Token: 0x04006907 RID: 26887
		[Tooltip("Location vector in degrees longitude and latitude. Typically returned by the {{Get Location Info}} action.")]
		public FsmVector3 GPSLocation;

		// Token: 0x04006908 RID: 26888
		[Tooltip("The projection used by the map.")]
		public ProjectLocationToMap.MapProjection mapProjection;

		// Token: 0x04006909 RID: 26889
		[ActionSection("Map Region")]
		[HasFloatSlider(-180f, 180f)]
		[Tooltip("The minimum Longitude shown on the map.")]
		public FsmFloat minLongitude;

		// Token: 0x0400690A RID: 26890
		[HasFloatSlider(-180f, 180f)]
		[Tooltip("The maximum Longitude show on the map.")]
		public FsmFloat maxLongitude;

		// Token: 0x0400690B RID: 26891
		[HasFloatSlider(-90f, 90f)]
		[Tooltip("The minimum Latitude shown on the map.")]
		public FsmFloat minLatitude;

		// Token: 0x0400690C RID: 26892
		[HasFloatSlider(-90f, 90f)]
		[Tooltip("The maximum Latitude shown on the map.")]
		public FsmFloat maxLatitude;

		// Token: 0x0400690D RID: 26893
		[ActionSection("Screen Region")]
		[Tooltip("The screen coordinate of the left edge of the map image.")]
		public FsmFloat minX;

		// Token: 0x0400690E RID: 26894
		[Tooltip("The screen coordinate of the top edge of the map image.")]
		public FsmFloat minY;

		// Token: 0x0400690F RID: 26895
		[Tooltip("The width of the map image in screen coordinates.")]
		public FsmFloat width;

		// Token: 0x04006910 RID: 26896
		[Tooltip("The height of the map in screen coordinates.")]
		public FsmFloat height;

		// Token: 0x04006911 RID: 26897
		[ActionSection("Projection")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the projected X coordinate in a Float Variable. Use this to display a marker on the map.")]
		public FsmFloat projectedX;

		// Token: 0x04006912 RID: 26898
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the projected Y coordinate in a Float Variable. Use this to display a marker on the map.")]
		public FsmFloat projectedY;

		// Token: 0x04006913 RID: 26899
		[Tooltip("If true all coordinates in this action are normalized (0-1); otherwise coordinates are in pixels.")]
		public FsmBool normalized;

		// Token: 0x04006914 RID: 26900
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04006915 RID: 26901
		private float x;

		// Token: 0x04006916 RID: 26902
		private float y;

		// Token: 0x02001BA7 RID: 7079
		public enum MapProjection
		{
			// Token: 0x04009E12 RID: 40466
			EquidistantCylindrical,
			// Token: 0x04009E13 RID: 40467
			Mercator
		}
	}
}
