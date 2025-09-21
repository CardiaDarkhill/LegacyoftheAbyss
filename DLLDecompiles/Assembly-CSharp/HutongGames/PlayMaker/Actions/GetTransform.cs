using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EBA RID: 3770
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets a Game Object's Transform and stores it in an Object Variable.")]
	public class GetTransform : FsmStateAction
	{
		// Token: 0x06006A9C RID: 27292 RVA: 0x00214608 File Offset: 0x00212808
		public override void Reset()
		{
			this.gameObject = new FsmGameObject
			{
				UseVariable = true
			};
			this.storeTransform = null;
			this.everyFrame = false;
		}

		// Token: 0x06006A9D RID: 27293 RVA: 0x0021462A File Offset: 0x0021282A
		public override void OnEnter()
		{
			this.DoGetGameObjectName();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006A9E RID: 27294 RVA: 0x00214640 File Offset: 0x00212840
		public override void OnUpdate()
		{
			this.DoGetGameObjectName();
		}

		// Token: 0x06006A9F RID: 27295 RVA: 0x00214648 File Offset: 0x00212848
		private void DoGetGameObjectName()
		{
			GameObject value = this.gameObject.Value;
			this.storeTransform.Value = ((value != null) ? value.transform : null);
		}

		// Token: 0x040069E5 RID: 27109
		[RequiredField]
		[Tooltip("The Game Object.")]
		public FsmGameObject gameObject;

		// Token: 0x040069E6 RID: 27110
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(Transform))]
		[Tooltip("Store the GameObject's Transform in an Object variable of type UnityEngine.Transform.")]
		public FsmObject storeTransform;

		// Token: 0x040069E7 RID: 27111
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
