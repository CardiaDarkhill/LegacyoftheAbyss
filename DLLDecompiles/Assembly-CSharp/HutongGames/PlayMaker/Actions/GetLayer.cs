using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EB0 RID: 3760
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets a Game Object's Layer and stores it in an Int Variable.")]
	public class GetLayer : FsmStateAction
	{
		// Token: 0x06006A73 RID: 27251 RVA: 0x0021409F File Offset: 0x0021229F
		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006A74 RID: 27252 RVA: 0x002140B6 File Offset: 0x002122B6
		public override void OnEnter()
		{
			this.DoGetLayer();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006A75 RID: 27253 RVA: 0x002140CC File Offset: 0x002122CC
		public override void OnUpdate()
		{
			this.DoGetLayer();
		}

		// Token: 0x06006A76 RID: 27254 RVA: 0x002140D4 File Offset: 0x002122D4
		private void DoGetLayer()
		{
			if (this.gameObject.Value == null)
			{
				return;
			}
			this.storeResult.Value = this.gameObject.Value.layer;
		}

		// Token: 0x040069C9 RID: 27081
		[RequiredField]
		[Tooltip("The Game Object to examine.")]
		public FsmGameObject gameObject;

		// Token: 0x040069CA RID: 27082
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Layer in an Int Variable.")]
		public FsmInt storeResult;

		// Token: 0x040069CB RID: 27083
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
