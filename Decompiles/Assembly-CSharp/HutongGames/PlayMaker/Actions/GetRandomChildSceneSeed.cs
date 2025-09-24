using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C7E RID: 3198
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets a Random Child of a Game Object.")]
	public class GetRandomChildSceneSeed : FsmStateAction
	{
		// Token: 0x0600604B RID: 24651 RVA: 0x001E7B2F File Offset: 0x001E5D2F
		public override void Reset()
		{
			this.Parent = null;
			this.StoreResult = null;
		}

		// Token: 0x0600604C RID: 24652 RVA: 0x001E7B3F File Offset: 0x001E5D3F
		public override void OnEnter()
		{
			this.DoGetRandomChild();
			base.Finish();
		}

		// Token: 0x0600604D RID: 24653 RVA: 0x001E7B50 File Offset: 0x001E5D50
		private void DoGetRandomChild()
		{
			GameObject safe = this.Parent.GetSafe(this);
			if (safe == null)
			{
				return;
			}
			int childCount = safe.transform.childCount;
			if (childCount == 0)
			{
				return;
			}
			int index = GameManager.instance.SceneSeededRandom.Next(childCount);
			this.StoreResult.Value = safe.transform.GetChild(index).gameObject;
		}

		// Token: 0x04005DA1 RID: 23969
		[RequiredField]
		public FsmOwnerDefault Parent;

		// Token: 0x04005DA2 RID: 23970
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreResult;
	}
}
