using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EB8 RID: 3768
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets a Game Object's Tag and stores it in a String Variable.")]
	public class GetTag : FsmStateAction
	{
		// Token: 0x06006A94 RID: 27284 RVA: 0x00214538 File Offset: 0x00212738
		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006A95 RID: 27285 RVA: 0x0021454F File Offset: 0x0021274F
		public override void OnEnter()
		{
			this.DoGetTag();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006A96 RID: 27286 RVA: 0x00214565 File Offset: 0x00212765
		public override void OnUpdate()
		{
			this.DoGetTag();
		}

		// Token: 0x06006A97 RID: 27287 RVA: 0x0021456D File Offset: 0x0021276D
		private void DoGetTag()
		{
			if (this.gameObject.Value == null)
			{
				return;
			}
			this.storeResult.Value = this.gameObject.Value.tag;
		}

		// Token: 0x040069E0 RID: 27104
		[RequiredField]
		[Tooltip("The Game Object.")]
		public FsmGameObject gameObject;

		// Token: 0x040069E1 RID: 27105
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Tag in a String Variable.")]
		public FsmString storeResult;

		// Token: 0x040069E2 RID: 27106
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
