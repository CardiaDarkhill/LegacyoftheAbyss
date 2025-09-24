using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001371 RID: 4977
	public sealed class AddScenesVisited : FsmStateAction
	{
		// Token: 0x0600803D RID: 32829 RVA: 0x0025DCCC File Offset: 0x0025BECC
		public override void Reset()
		{
			this.scenesArray = null;
			this.scenesVisited = null;
			this.doMapUpdate = null;
			this.queueMapUpdate = null;
		}

		// Token: 0x0600803E RID: 32830 RVA: 0x0025DCEC File Offset: 0x0025BEEC
		public override void OnEnter()
		{
			PlayerData instance = PlayerData.instance;
			bool flag = false;
			if (this.scenesArray != null && this.scenesArray.stringValues != null)
			{
				foreach (string text in this.scenesArray.stringValues)
				{
					if (!string.IsNullOrEmpty(text) && instance.scenesVisited.Add(text))
					{
						flag = true;
					}
				}
			}
			if (this.scenesVisited != null)
			{
				for (int j = 0; j < this.scenesVisited.Length; j++)
				{
					FsmString fsmString = this.scenesVisited[j];
					if (fsmString != null)
					{
						string value = fsmString.Value;
						if (!string.IsNullOrEmpty(value) && instance.scenesVisited.Add(value))
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

		// Token: 0x04007FA3 RID: 32675
		[ArrayEditor(VariableType.String, "", 0, 0, 65536)]
		public FsmArray scenesArray;

		// Token: 0x04007FA4 RID: 32676
		public FsmString[] scenesVisited;

		// Token: 0x04007FA5 RID: 32677
		public FsmBool doMapUpdate;

		// Token: 0x04007FA6 RID: 32678
		public FsmBool queueMapUpdate;
	}
}
