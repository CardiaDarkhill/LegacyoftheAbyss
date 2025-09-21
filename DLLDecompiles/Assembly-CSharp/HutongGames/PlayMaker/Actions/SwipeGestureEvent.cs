using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E90 RID: 3728
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Sends an event when a swipe is detected.")]
	public class SwipeGestureEvent : FsmStateAction
	{
		// Token: 0x060069E3 RID: 27107 RVA: 0x00211CB9 File Offset: 0x0020FEB9
		public override void Reset()
		{
			this.minSwipeDistance = 0.1f;
			this.swipeLeftEvent = null;
			this.swipeRightEvent = null;
			this.swipeUpEvent = null;
			this.swipeDownEvent = null;
		}

		// Token: 0x060069E4 RID: 27108 RVA: 0x00211CE7 File Offset: 0x0020FEE7
		public override void OnEnter()
		{
			this.screenDiagonalSize = Mathf.Sqrt((float)(Screen.width * Screen.width + Screen.height * Screen.height));
			this.minSwipeDistancePixels = this.minSwipeDistance.Value * this.screenDiagonalSize;
		}

		// Token: 0x060069E5 RID: 27109 RVA: 0x00211D24 File Offset: 0x0020FF24
		public override void OnUpdate()
		{
			if (Input.touchCount > 0)
			{
				Touch touch = Input.touches[0];
				switch (touch.phase)
				{
				case TouchPhase.Began:
					this.touchStarted = true;
					this.touchStartPos = touch.position;
					return;
				case TouchPhase.Moved:
				case TouchPhase.Stationary:
					break;
				case TouchPhase.Ended:
					if (this.touchStarted)
					{
						this.TestForSwipeGesture(touch.position);
						this.touchStarted = false;
						return;
					}
					break;
				case TouchPhase.Canceled:
					this.touchStarted = false;
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x060069E6 RID: 27110 RVA: 0x00211DA4 File Offset: 0x0020FFA4
		private void TestForSwipeGesture(Vector2 touchPosition)
		{
			if (Vector2.Distance(touchPosition, this.touchStartPos) > this.minSwipeDistancePixels)
			{
				float x = touchPosition.y - this.touchStartPos.y;
				float y = touchPosition.x - this.touchStartPos.x;
				float num = 57.29578f * Mathf.Atan2(y, x);
				num = (360f + num - 45f) % 360f;
				if (num < 90f)
				{
					base.Fsm.Event(this.swipeRightEvent);
					return;
				}
				if (num < 180f)
				{
					base.Fsm.Event(this.swipeDownEvent);
					return;
				}
				if (num < 270f)
				{
					base.Fsm.Event(this.swipeLeftEvent);
					return;
				}
				base.Fsm.Event(this.swipeUpEvent);
			}
		}

		// Token: 0x0400691C RID: 26908
		[Tooltip("How far a touch has to travel to be considered a swipe. Uses normalized distance (e.g. 1 = 1 screen diagonal distance). Should generally be a very small number.")]
		public FsmFloat minSwipeDistance;

		// Token: 0x0400691D RID: 26909
		[Tooltip("Event to send when swipe left detected.")]
		public FsmEvent swipeLeftEvent;

		// Token: 0x0400691E RID: 26910
		[Tooltip("Event to send when swipe right detected.")]
		public FsmEvent swipeRightEvent;

		// Token: 0x0400691F RID: 26911
		[Tooltip("Event to send when swipe up detected.")]
		public FsmEvent swipeUpEvent;

		// Token: 0x04006920 RID: 26912
		[Tooltip("Event to send when swipe down detected.")]
		public FsmEvent swipeDownEvent;

		// Token: 0x04006921 RID: 26913
		private float screenDiagonalSize;

		// Token: 0x04006922 RID: 26914
		private float minSwipeDistancePixels;

		// Token: 0x04006923 RID: 26915
		private bool touchStarted;

		// Token: 0x04006924 RID: 26916
		private Vector2 touchStartPos;
	}
}
