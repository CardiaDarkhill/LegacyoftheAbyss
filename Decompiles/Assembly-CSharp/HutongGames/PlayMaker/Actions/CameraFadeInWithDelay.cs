using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BDB RID: 3035
	[ActionCategory(ActionCategory.Camera)]
	[Tooltip("Fade from a fullscreen Color. NOTE: Uses OnGUI so requires a PlayMakerGUI component in the scene.")]
	public class CameraFadeInWithDelay : FsmStateAction
	{
		// Token: 0x06005D04 RID: 23812 RVA: 0x001D3CEC File Offset: 0x001D1EEC
		public override void Reset()
		{
			this.color = Color.black;
			this.time = 1f;
			this.delay = 1f;
			this.finishEvent = null;
			this.delayPassed = false;
		}

		// Token: 0x06005D05 RID: 23813 RVA: 0x001D3D2C File Offset: 0x001D1F2C
		public override void OnEnter()
		{
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.currentTime = 0f;
			this.colorLerp = this.color.Value;
		}

		// Token: 0x06005D06 RID: 23814 RVA: 0x001D3D58 File Offset: 0x001D1F58
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
			if (!this.delayPassed && this.currentTime > this.delay.Value)
			{
				this.currentTime = 0f;
				this.delayPassed = true;
			}
			if (this.delayPassed)
			{
				this.colorLerp = Color.Lerp(this.color.Value, Color.clear, this.currentTime / this.time.Value);
				if (this.currentTime > this.time.Value)
				{
					if (this.finishEvent != null)
					{
						base.Fsm.Event(this.finishEvent);
					}
					this.delayPassed = false;
					base.Finish();
				}
			}
		}

		// Token: 0x06005D07 RID: 23815 RVA: 0x001D3E2E File Offset: 0x001D202E
		public override void OnGUI()
		{
			Color color = GUI.color;
			GUI.color = this.colorLerp;
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), ActionHelpers.WhiteTexture);
			GUI.color = color;
		}

		// Token: 0x040058AD RID: 22701
		[RequiredField]
		[Tooltip("Color to fade from. E.g., Fade up from black.")]
		public FsmColor color;

		// Token: 0x040058AE RID: 22702
		[RequiredField]
		[HasFloatSlider(0f, 10f)]
		[Tooltip("Delay time in seconds before starting the fade in.")]
		public FsmFloat delay;

		// Token: 0x040058AF RID: 22703
		[RequiredField]
		[HasFloatSlider(0f, 10f)]
		[Tooltip("Fade in time in seconds.")]
		public FsmFloat time;

		// Token: 0x040058B0 RID: 22704
		[Tooltip("Event to send when finished.")]
		public FsmEvent finishEvent;

		// Token: 0x040058B1 RID: 22705
		[Tooltip("Ignore TimeScale. Useful if the game is paused.")]
		public bool realTime;

		// Token: 0x040058B2 RID: 22706
		private float startTime;

		// Token: 0x040058B3 RID: 22707
		private float currentTime;

		// Token: 0x040058B4 RID: 22708
		private Color colorLerp;

		// Token: 0x040058B5 RID: 22709
		private bool delayPassed;
	}
}
