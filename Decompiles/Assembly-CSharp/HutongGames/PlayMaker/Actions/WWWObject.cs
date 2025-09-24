using System;
using UnityEngine.Networking;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011F1 RID: 4593
	[ActionCategory("WWW")]
	[Tooltip("Gets data from a url and store it in variables. See Unity WWW docs for more details.")]
	public class WWWObject : FsmStateAction
	{
		// Token: 0x06007A6F RID: 31343 RVA: 0x0024C61E File Offset: 0x0024A81E
		public override void Reset()
		{
			this.url = null;
			this.storeText = null;
			this.storeTexture = null;
			this.errorString = null;
			this.progress = null;
			this.isDone = null;
		}

		// Token: 0x06007A70 RID: 31344 RVA: 0x0024C64C File Offset: 0x0024A84C
		public override void OnEnter()
		{
			if (string.IsNullOrEmpty(this.url.Value))
			{
				base.Finish();
				return;
			}
			if (!this.storeTexture.IsNone)
			{
				this.uwr = UnityWebRequestTexture.GetTexture(this.url.Value);
			}
			else
			{
				this.uwr = new UnityWebRequest(this.url.Value);
				this.d = new DownloadHandlerBuffer();
				this.uwr.downloadHandler = this.d;
			}
			this.uwr.SendWebRequest();
		}

		// Token: 0x06007A71 RID: 31345 RVA: 0x0024C6D8 File Offset: 0x0024A8D8
		public override void OnUpdate()
		{
			if (this.uwr == null)
			{
				this.errorString.Value = "Unity Web Request is Null!";
				base.Finish();
				return;
			}
			this.errorString.Value = this.uwr.error;
			if (!string.IsNullOrEmpty(this.uwr.error))
			{
				this.uwr.Dispose();
				base.Finish();
				base.Fsm.Event(this.isError);
				return;
			}
			this.progress.Value = this.uwr.downloadProgress;
			if (this.progress.Value.Equals(1f) && this.uwr.isDone)
			{
				if (!this.storeText.IsNone)
				{
					this.storeText.Value = this.uwr.downloadHandler.text;
				}
				if (!this.storeTexture.IsNone)
				{
					this.storeTexture.Value = ((DownloadHandlerTexture)this.uwr.downloadHandler).texture;
				}
				this.errorString.Value = this.uwr.error;
				this.uwr.Dispose();
				base.Fsm.Event(string.IsNullOrEmpty(this.errorString.Value) ? this.isDone : this.isError);
				base.Finish();
			}
		}

		// Token: 0x04007AB4 RID: 31412
		[RequiredField]
		[Tooltip("Url to download data from.")]
		public FsmString url;

		// Token: 0x04007AB5 RID: 31413
		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Gets text from the url.")]
		public FsmString storeText;

		// Token: 0x04007AB6 RID: 31414
		[UIHint(UIHint.Variable)]
		[Tooltip("Gets a Texture from the url.")]
		public FsmTexture storeTexture;

		// Token: 0x04007AB7 RID: 31415
		[UIHint(UIHint.Variable)]
		[Tooltip("Error message if there was an error during the download.")]
		public FsmString errorString;

		// Token: 0x04007AB8 RID: 31416
		[UIHint(UIHint.Variable)]
		[Tooltip("How far the download progressed (0-1).")]
		public FsmFloat progress;

		// Token: 0x04007AB9 RID: 31417
		[ActionSection("Events")]
		[Tooltip("Event to send when the data has finished loading (progress = 1).")]
		public FsmEvent isDone;

		// Token: 0x04007ABA RID: 31418
		[Tooltip("Event to send if there was an error.")]
		public FsmEvent isError;

		// Token: 0x04007ABB RID: 31419
		private UnityWebRequest uwr;

		// Token: 0x04007ABC RID: 31420
		private DownloadHandlerBuffer d;
	}
}
