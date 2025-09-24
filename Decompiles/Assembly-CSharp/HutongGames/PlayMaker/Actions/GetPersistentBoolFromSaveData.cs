using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020013A7 RID: 5031
	public class GetPersistentBoolFromSaveData : FsmStateAction
	{
		// Token: 0x06008102 RID: 33026 RVA: 0x002600C3 File Offset: 0x0025E2C3
		public bool ShouldHideDirect()
		{
			return this.Target.OwnerOption == OwnerDefaultOption.UseOwner || (this.Target.GameObject != null && this.Target.GameObject.Value != null);
		}

		// Token: 0x06008103 RID: 33027 RVA: 0x002600F9 File Offset: 0x0025E2F9
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

		// Token: 0x06008104 RID: 33028 RVA: 0x0026012C File Offset: 0x0025E32C
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			string sceneName;
			string id;
			if (safe != null)
			{
				PersistentItemData<bool> itemData = safe.GetComponent<PersistentBoolItem>().ItemData;
				sceneName = itemData.SceneName;
				id = itemData.ID;
			}
			else
			{
				sceneName = this.SceneName.Value;
				id = this.ID.Value;
			}
			PersistentItemData<bool> persistentItemData;
			if (SceneData.instance.PersistentBools.TryGetValue(sceneName, id, out persistentItemData))
			{
				this.StoreValue.Value = persistentItemData.Value;
			}
			else
			{
				this.StoreValue.Value = false;
			}
			base.Finish();
		}

		// Token: 0x04008041 RID: 32833
		[CheckForComponent(typeof(PersistentBoolItem))]
		public FsmOwnerDefault Target;

		// Token: 0x04008042 RID: 32834
		[HideIf("ShouldHideDirect")]
		public FsmString SceneName;

		// Token: 0x04008043 RID: 32835
		[HideIf("ShouldHideDirect")]
		public FsmString ID;

		// Token: 0x04008044 RID: 32836
		[UIHint(UIHint.Variable)]
		public FsmBool StoreValue;
	}
}
