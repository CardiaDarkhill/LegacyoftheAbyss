using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E8C RID: 3724
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Gets info on a touch event.")]
	public class GetTouchInfo : FsmStateAction
	{
		// Token: 0x060069CF RID: 27087 RVA: 0x00211688 File Offset: 0x0020F888
		public override void Reset()
		{
			this.fingerId = new FsmInt
			{
				UseVariable = true
			};
			this.normalize = true;
			this.storePosition = null;
			this.storeDeltaPosition = null;
			this.storeDeltaTime = null;
			this.storeTapCount = null;
			this.everyFrame = true;
		}

		// Token: 0x060069D0 RID: 27088 RVA: 0x002116D6 File Offset: 0x0020F8D6
		public override void OnEnter()
		{
			this.screenWidth = (float)Screen.width;
			this.screenHeight = (float)Screen.height;
			this.DoGetTouchInfo();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060069D1 RID: 27089 RVA: 0x00211704 File Offset: 0x0020F904
		public override void OnUpdate()
		{
			this.DoGetTouchInfo();
		}

		// Token: 0x060069D2 RID: 27090 RVA: 0x0021170C File Offset: 0x0020F90C
		private void DoGetTouchInfo()
		{
			if (Input.touchCount > 0)
			{
				foreach (Touch touch in Input.touches)
				{
					if (this.fingerId.IsNone || touch.fingerId == this.fingerId.Value)
					{
						float num = (!this.normalize.Value) ? touch.position.x : (touch.position.x / this.screenWidth);
						float num2 = (!this.normalize.Value) ? touch.position.y : (touch.position.y / this.screenHeight);
						if (!this.storePosition.IsNone)
						{
							this.storePosition.Value = new Vector3(num, num2, 0f);
						}
						this.storeX.Value = num;
						this.storeY.Value = num2;
						float num3 = (!this.normalize.Value) ? touch.deltaPosition.x : (touch.deltaPosition.x / this.screenWidth);
						float num4 = (!this.normalize.Value) ? touch.deltaPosition.y : (touch.deltaPosition.y / this.screenHeight);
						if (!this.storeDeltaPosition.IsNone)
						{
							this.storeDeltaPosition.Value = new Vector3(num3, num4, 0f);
						}
						this.storeDeltaX.Value = num3;
						this.storeDeltaY.Value = num4;
						this.storeDeltaTime.Value = touch.deltaTime;
						this.storeTapCount.Value = touch.tapCount;
					}
				}
			}
		}

		// Token: 0x040068FA RID: 26874
		[Tooltip("Filter by a Finger ID. You can store a Finger ID in other Touch actions, e.g., Touch Event.")]
		public FsmInt fingerId;

		// Token: 0x040068FB RID: 26875
		[Tooltip("If true, all screen coordinates are returned normalized (0-1), otherwise in pixels.")]
		public FsmBool normalize;

		// Token: 0x040068FC RID: 26876
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the position of the touch in a Vector3 Variable.")]
		public FsmVector3 storePosition;

		// Token: 0x040068FD RID: 26877
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the X position \u00a0in a Float Variable.")]
		public FsmFloat storeX;

		// Token: 0x040068FE RID: 26878
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Y position \u00a0in a Float Variable.")]
		public FsmFloat storeY;

		// Token: 0x040068FF RID: 26879
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the movement of the touch in a Vector3 Variable.")]
		public FsmVector3 storeDeltaPosition;

		// Token: 0x04006900 RID: 26880
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the X movement in a Float Variable.")]
		public FsmFloat storeDeltaX;

		// Token: 0x04006901 RID: 26881
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Y movement in a Float Variable.")]
		public FsmFloat storeDeltaY;

		// Token: 0x04006902 RID: 26882
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the time between touch events in a Float Variable.")]
		public FsmFloat storeDeltaTime;

		// Token: 0x04006903 RID: 26883
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the number of tap count of the touch (e.g. 2 = double tap).")]
		public FsmInt storeTapCount;

		// Token: 0x04006904 RID: 26884
		[Tooltip("Repeat every frame.")]
		public bool everyFrame = true;

		// Token: 0x04006905 RID: 26885
		private float screenWidth;

		// Token: 0x04006906 RID: 26886
		private float screenHeight;
	}
}
