using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E48 RID: 3656
	[ActionCategory(ActionCategory.Camera)]
	[Tooltip("Fade from a fullscreen Color. NOTE: Uses OnGUI so requires a PlayMakerGUI component in the scene.")]
	public class CameraFadeIn : FsmStateAction
	{
		// Token: 0x06006892 RID: 26770 RVA: 0x0020D8EF File Offset: 0x0020BAEF
		public override void Reset()
		{
			this.color = Color.black;
			this.time = 1f;
			this.finishEvent = null;
		}

		// Token: 0x06006893 RID: 26771 RVA: 0x0020D918 File Offset: 0x0020BB18
		public override void OnEnter()
		{
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.currentTime = 0f;
			this.colorLerp = this.color.Value;
		}

		// Token: 0x06006894 RID: 26772 RVA: 0x0020D944 File Offset: 0x0020BB44
		public override void OnUpdate()
		{
			if (this.realTime)
			{
				this.currentTime = FsmTime.RealtimeSinceStartup - this.startTime;
			}
			else
			{
				this.currentTime += Time.deltaTime;
			}
			this.colorLerp = Color.Lerp(this.color.Value, Color.clear, this.currentTime / this.time.Value);
			if (this.currentTime > this.time.Value)
			{
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
				base.Finish();
			}
		}

		// Token: 0x06006895 RID: 26773 RVA: 0x0020D9DE File Offset: 0x0020BBDE
		public override void OnGUI()
		{
			Color color = GUI.color;
			GUI.color = this.colorLerp;
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), ActionHelpers.WhiteTexture);
			GUI.color = color;
		}

		// Token: 0x040067BE RID: 26558
		[RequiredField]
		[Tooltip("Color to fade from. E.g., Fade up from black.")]
		public FsmColor color;

		// Token: 0x040067BF RID: 26559
		[RequiredField]
		[HasFloatSlider(0f, 10f)]
		[Tooltip("Fade in time in seconds.")]
		public FsmFloat time;

		// Token: 0x040067C0 RID: 26560
		[Tooltip("Optional Event to send when finished.")]
		public FsmEvent finishEvent;

		// Token: 0x040067C1 RID: 26561
		[Tooltip("Ignore TimeScale. Useful if the game is paused.")]
		public bool realTime;

		// Token: 0x040067C2 RID: 26562
		private float startTime;

		// Token: 0x040067C3 RID: 26563
		private float currentTime;

		// Token: 0x040067C4 RID: 26564
		private Color colorLerp;
	}
}
