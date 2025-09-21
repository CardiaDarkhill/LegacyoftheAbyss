using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001399 RID: 5017
	public class GetGuidReferenceHolderGameObject : FsmStateAction
	{
		// Token: 0x060080C2 RID: 32962 RVA: 0x0025F117 File Offset: 0x0025D317
		public override void Reset()
		{
			this.Target = null;
			this.StoreGameObject = null;
		}

		// Token: 0x060080C3 RID: 32963 RVA: 0x0025F128 File Offset: 0x0025D328
		public override void OnEnter()
		{
			GuidReferenceHolder component = this.Target.GetSafe(this).GetComponent<GuidReferenceHolder>();
			this.StoreGameObject.Value = component.ReferencedGameObject;
			base.Finish();
		}

		// Token: 0x04007FFF RID: 32767
		public FsmOwnerDefault Target;

		// Token: 0x04008000 RID: 32768
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreGameObject;
	}
}
