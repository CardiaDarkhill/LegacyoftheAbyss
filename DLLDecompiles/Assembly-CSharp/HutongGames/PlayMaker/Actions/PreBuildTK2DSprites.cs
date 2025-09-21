using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CE1 RID: 3297
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Prevent hitching at runtime by pre-building TK2D sprites on disabled objects.")]
	public class PreBuildTK2DSprites : FsmStateAction
	{
		// Token: 0x06006211 RID: 25105 RVA: 0x001F007F File Offset: 0x001EE27F
		public override void Reset()
		{
			this.gameObject = null;
			this.useChildren = true;
		}

		// Token: 0x06006212 RID: 25106 RVA: 0x001F0090 File Offset: 0x001EE290
		public override void OnEnter()
		{
			GameObject value = this.gameObject.Value;
			if (value != null)
			{
				tk2dSprite[] array = this.useChildren ? value.GetComponentsInChildren<tk2dSprite>(true) : value.GetComponents<tk2dSprite>();
				for (int i = 0; i < array.Length; i++)
				{
					array[i].ForceBuild();
				}
			}
			base.Finish();
		}

		// Token: 0x04006023 RID: 24611
		[RequiredField]
		public FsmGameObject gameObject;

		// Token: 0x04006024 RID: 24612
		public bool useChildren;
	}
}
