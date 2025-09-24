using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EB3 RID: 3763
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets the Game Object that owns the FSM and stores it in a Game Object variable.")]
	public class GetOwner : FsmStateAction
	{
		// Token: 0x06006A81 RID: 27265 RVA: 0x002142DD File Offset: 0x002124DD
		public override void Reset()
		{
			this.storeGameObject = null;
		}

		// Token: 0x06006A82 RID: 27266 RVA: 0x002142E6 File Offset: 0x002124E6
		public override void OnEnter()
		{
			this.storeGameObject.Value = base.Owner;
			base.Finish();
		}

		// Token: 0x040069D6 RID: 27094
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Owner in a Game Object variable.")]
		public FsmGameObject storeGameObject;
	}
}
