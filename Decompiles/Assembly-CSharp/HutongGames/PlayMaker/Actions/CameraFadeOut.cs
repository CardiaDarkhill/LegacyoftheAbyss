using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E49 RID: 3657
	[ActionCategory(ActionCategory.Camera)]
	[Tooltip("Fade to a fullscreen Color. NOTE: Uses OnGUI so requires a PlayMakerGUI component in the scene.")]
	public class CameraFadeOut : FsmStateAction
	{
		// Token: 0x06006897 RID: 26775 RVA: 0x0020DA22 File Offset: 0x0020BC22
		public override void Reset()
		{
			this.color = Color.black;
			this.time = 1f;
			this.finishEvent = null;
		}

		// Token: 0x06006898 RID: 26776 RVA: 0x0020DA4B File Offset: 0x0020BC4B
		public override void OnEnter()
		{
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.currentTime = 0f;
			this.colorLerp = Color.clear;
		}

		// Token: 0x06006899 RID: 26777 RVA: 0x0020DA70 File Offset: 0x0020BC70
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
			this.colorLerp = Color.Lerp(Color.clear, this.color.Value, this.currentTime / this.time.Value);
			if (this.currentTime > this.time.Value && this.finishEvent != null)
			{
				base.Fsm.Event(this.finishEvent);
			}
		}

		// Token: 0x0600689A RID: 26778 RVA: 0x0020DB04 File Offset: 0x0020BD04
		public override void OnGUI()
		{
			Color color = GUI.color;
			GUI.color = this.colorLerp;
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), ActionHelpers.WhiteTexture);
			GUI.color = color;
		}

		// Token: 0x040067C5 RID: 26565
		[RequiredField]
		[Tooltip("Color to fade to. E.g., Fade to black.")]
		public FsmColor color;

		// Token: 0x040067C6 RID: 26566
		[RequiredField]
		[HasFloatSlider(0f, 10f)]
		[Tooltip("Fade out time in seconds.")]
		public FsmFloat time;

		// Token: 0x040067C7 RID: 26567
		[Tooltip("Optional Event to send when finished.")]
		public FsmEvent finishEvent;

		// Token: 0x040067C8 RID: 26568
		[Tooltip("Ignore TimeScale. Useful if the game is paused.")]
		public bool realTime;

		// Token: 0x040067C9 RID: 26569
		private float startTime;

		// Token: 0x040067CA RID: 26570
		private float currentTime;

		// Token: 0x040067CB RID: 26571
		private Color colorLerp;
	}
}
