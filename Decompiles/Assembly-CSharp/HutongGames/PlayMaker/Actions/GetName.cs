using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EB1 RID: 3761
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets the name of a Game Object and stores it in a String Variable.")]
	public class GetName : FsmStateAction
	{
		// Token: 0x06006A78 RID: 27256 RVA: 0x0021410D File Offset: 0x0021230D
		public override void Reset()
		{
			this.gameObject = new FsmGameObject
			{
				UseVariable = true
			};
			this.storeName = null;
			this.everyFrame = false;
		}

		// Token: 0x06006A79 RID: 27257 RVA: 0x0021412F File Offset: 0x0021232F
		public override void OnEnter()
		{
			this.DoGetGameObjectName();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006A7A RID: 27258 RVA: 0x00214145 File Offset: 0x00212345
		public override void OnUpdate()
		{
			this.DoGetGameObjectName();
		}

		// Token: 0x06006A7B RID: 27259 RVA: 0x00214150 File Offset: 0x00212350
		private void DoGetGameObjectName()
		{
			GameObject value = this.gameObject.Value;
			this.storeName.Value = ((value != null) ? value.name : "");
		}

		// Token: 0x040069CC RID: 27084
		[RequiredField]
		[Tooltip("The Game Object target.")]
		public FsmGameObject gameObject;

		// Token: 0x040069CD RID: 27085
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Game Object name in a String Variable.")]
		public FsmString storeName;

		// Token: 0x040069CE RID: 27086
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
