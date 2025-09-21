using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020013AE RID: 5038
	public class SetPersistentIntSaveData : FsmStateAction
	{
		// Token: 0x06008119 RID: 33049 RVA: 0x00260533 File Offset: 0x0025E733
		public override void Reset()
		{
			this.SceneName = null;
			this.ID = null;
			this.SetValue = null;
		}

		// Token: 0x0600811A RID: 33050 RVA: 0x0026054C File Offset: 0x0025E74C
		public override void OnEnter()
		{
			PersistentItemData<int> persistentItemData;
			if (SceneData.instance.PersistentInts.TryGetValue(this.SceneName.Value, this.ID.Value, out persistentItemData))
			{
				persistentItemData.Value = this.SetValue.Value;
			}
			else
			{
				persistentItemData = new PersistentItemData<int>
				{
					SceneName = this.SceneName.Value,
					ID = this.ID.Value,
					Value = this.SetValue.Value
				};
			}
			SceneData.instance.PersistentInts.SetValue(persistentItemData);
			base.Finish();
		}

		// Token: 0x04008054 RID: 32852
		public FsmString SceneName;

		// Token: 0x04008055 RID: 32853
		public FsmString ID;

		// Token: 0x04008056 RID: 32854
		public FsmInt SetValue;
	}
}
