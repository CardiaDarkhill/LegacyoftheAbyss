using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001372 RID: 4978
	public sealed class AddScenesMapped : FsmStateAction
	{
		// Token: 0x06008040 RID: 32832 RVA: 0x0025DDDE File Offset: 0x0025BFDE
		public override void Reset()
		{
			this.scenesArray = null;
			this.scenesMapped = null;
			this.requireQuill = null;
			this.doMapUpdate = null;
			this.queueMapUpdate = null;
		}

		// Token: 0x06008041 RID: 32833 RVA: 0x0025DE04 File Offset: 0x0025C004
		public override void OnEnter()
		{
			PlayerData instance = PlayerData.instance;
			if (this.requireQuill.Value && (!instance.hasQuill || instance.QuillState == 0))
			{
				base.Finish();
				return;
			}
			bool flag = false;
			if (this.scenesArray != null && this.scenesArray.stringValues != null)
			{
				foreach (string text in this.scenesArray.stringValues)
				{
					if (!string.IsNullOrEmpty(text) && instance.scenesMapped.Add(text))
					{
						flag = true;
					}
				}
			}
			if (this.scenesMapped != null)
			{
				for (int j = 0; j < this.scenesMapped.Length; j++)
				{
					FsmString fsmString = this.scenesMapped[j];
					if (fsmString != null)
					{
						string value = fsmString.Value;
						if (!string.IsNullOrEmpty(value) && instance.scenesMapped.Add(value))
						{
							flag = true;
						}
					}
				}
			}
			if (this.doMapUpdate.Value)
			{
				GameManager.instance.UpdateGameMap();
			}
			else if (this.queueMapUpdate.Value && flag)
			{
				instance.mapUpdateQueued = true;
			}
			base.Finish();
		}

		// Token: 0x04007FA7 RID: 32679
		[ArrayEditor(VariableType.String, "", 0, 0, 65536)]
		public FsmArray scenesArray;

		// Token: 0x04007FA8 RID: 32680
		public FsmString[] scenesMapped;

		// Token: 0x04007FA9 RID: 32681
		public FsmBool requireQuill;

		// Token: 0x04007FAA RID: 32682
		public FsmBool doMapUpdate;

		// Token: 0x04007FAB RID: 32683
		public FsmBool queueMapUpdate;
	}
}
