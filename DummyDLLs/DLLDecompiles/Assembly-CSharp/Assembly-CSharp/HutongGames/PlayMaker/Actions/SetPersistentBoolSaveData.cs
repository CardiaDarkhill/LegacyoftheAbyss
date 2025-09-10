using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020013A8 RID: 5032
	public class SetPersistentBoolSaveData : FsmStateAction
	{
		// Token: 0x06008106 RID: 33030 RVA: 0x002601C3 File Offset: 0x0025E3C3
		public override void Reset()
		{
			this.SceneName = null;
			this.ID = null;
			this.SetValue = null;
		}

		// Token: 0x06008107 RID: 33031 RVA: 0x002601DC File Offset: 0x0025E3DC
		public override void OnEnter()
		{
			PersistentItemData<bool> persistentItemData;
			if (SceneData.instance.PersistentBools.TryGetValue(this.SceneName.Value, this.ID.Value, out persistentItemData))
			{
				persistentItemData.Value = this.SetValue.Value;
			}
			else
			{
				persistentItemData = new PersistentItemData<bool>
				{
					SceneName = this.SceneName.Value,
					ID = this.ID.Value,
					Value = this.SetValue.Value
				};
			}
			SceneData.instance.PersistentBools.SetValue(persistentItemData);
			base.Finish();
		}

		// Token: 0x04008045 RID: 32837
		public FsmString SceneName;

		// Token: 0x04008046 RID: 32838
		public FsmString ID;

		// Token: 0x04008047 RID: 32839
		public FsmBool SetValue;
	}
}
