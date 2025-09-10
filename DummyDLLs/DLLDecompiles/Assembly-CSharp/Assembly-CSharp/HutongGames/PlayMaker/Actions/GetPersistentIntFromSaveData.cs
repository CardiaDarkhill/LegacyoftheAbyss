using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020013AD RID: 5037
	public class GetPersistentIntFromSaveData : FsmStateAction
	{
		// Token: 0x06008115 RID: 33045 RVA: 0x00260434 File Offset: 0x0025E634
		public bool ShouldHideDirect()
		{
			return this.Target.OwnerOption == OwnerDefaultOption.UseOwner || (this.Target.GameObject != null && this.Target.GameObject.Value != null);
		}

		// Token: 0x06008116 RID: 33046 RVA: 0x0026046A File Offset: 0x0025E66A
		public override void Reset()
		{
			this.Target = new FsmOwnerDefault
			{
				OwnerOption = OwnerDefaultOption.SpecifyGameObject,
				GameObject = null
			};
			this.SceneName = null;
			this.ID = null;
			this.StoreValue = null;
		}

		// Token: 0x06008117 RID: 33047 RVA: 0x0026049C File Offset: 0x0025E69C
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			string sceneName;
			string id;
			if (safe != null)
			{
				PersistentItemData<int> itemData = safe.GetComponent<PersistentIntItem>().ItemData;
				sceneName = itemData.SceneName;
				id = itemData.ID;
			}
			else
			{
				sceneName = this.SceneName.Value;
				id = this.ID.Value;
			}
			PersistentItemData<int> persistentItemData;
			if (SceneData.instance.PersistentInts.TryGetValue(sceneName, id, out persistentItemData))
			{
				this.StoreValue.Value = persistentItemData.Value;
			}
			else
			{
				this.StoreValue.Value = 0;
			}
			base.Finish();
		}

		// Token: 0x04008050 RID: 32848
		[CheckForComponent(typeof(PersistentIntItem))]
		public FsmOwnerDefault Target;

		// Token: 0x04008051 RID: 32849
		[HideIf("ShouldHideDirect")]
		public FsmString SceneName;

		// Token: 0x04008052 RID: 32850
		[HideIf("ShouldHideDirect")]
		public FsmString ID;

		// Token: 0x04008053 RID: 32851
		[UIHint(UIHint.Variable)]
		public FsmInt StoreValue;
	}
}
