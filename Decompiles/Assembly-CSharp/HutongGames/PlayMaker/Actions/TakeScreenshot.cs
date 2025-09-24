using System;
using System.IO;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E1D RID: 3613
	[ActionCategory(ActionCategory.Application)]
	[Tooltip("Saves a Screenshot. NOTE: Does nothing in Web Player. On Android, the resulting screenshot is available some time later.")]
	public class TakeScreenshot : FsmStateAction
	{
		// Token: 0x060067E0 RID: 26592 RVA: 0x0020B2A6 File Offset: 0x002094A6
		public override void Reset()
		{
			this.destination = TakeScreenshot.Destination.MyPictures;
			this.filename = "";
			this.autoNumber = null;
			this.superSize = null;
			this.debugLog = null;
		}

		// Token: 0x060067E1 RID: 26593 RVA: 0x0020B2D4 File Offset: 0x002094D4
		public override void OnEnter()
		{
			if (string.IsNullOrEmpty(this.filename.Value))
			{
				return;
			}
			string text;
			switch (this.destination)
			{
			case TakeScreenshot.Destination.MyPictures:
				text = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
				break;
			case TakeScreenshot.Destination.PersistentDataPath:
				text = Application.persistentDataPath;
				break;
			case TakeScreenshot.Destination.CustomPath:
				text = this.customPath.Value;
				break;
			default:
				text = "";
				break;
			}
			text = text.Replace("\\", "/") + "/";
			string text2 = text + this.filename.Value + ".png";
			if (this.autoNumber.Value)
			{
				while (File.Exists(text2))
				{
					this.screenshotCount++;
					text2 = text + this.filename.Value + this.screenshotCount.ToString() + ".png";
				}
			}
			if (this.debugLog.Value)
			{
				Debug.Log("TakeScreenshot: " + text2);
			}
			ScreenCapture.CaptureScreenshot(text2, this.superSize.Value);
			base.Finish();
		}

		// Token: 0x04006710 RID: 26384
		[Tooltip("Where to save the screenshot.")]
		public TakeScreenshot.Destination destination;

		// Token: 0x04006711 RID: 26385
		[Tooltip("Path used with Custom Path Destination option.")]
		public FsmString customPath;

		// Token: 0x04006712 RID: 26386
		[RequiredField]
		[Tooltip("The filename for the screenshot.")]
		public FsmString filename;

		// Token: 0x04006713 RID: 26387
		[Tooltip("Add an auto-incremented number to the filename.")]
		public FsmBool autoNumber;

		// Token: 0x04006714 RID: 26388
		[Tooltip("Factor by which to increase resolution.")]
		public FsmInt superSize;

		// Token: 0x04006715 RID: 26389
		[Tooltip("Log saved file info in Unity console.")]
		public FsmBool debugLog;

		// Token: 0x04006716 RID: 26390
		private int screenshotCount;

		// Token: 0x02001B9B RID: 7067
		public enum Destination
		{
			// Token: 0x04009DE0 RID: 40416
			MyPictures,
			// Token: 0x04009DE1 RID: 40417
			PersistentDataPath,
			// Token: 0x04009DE2 RID: 40418
			CustomPath
		}
	}
}
