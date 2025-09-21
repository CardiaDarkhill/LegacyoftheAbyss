using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200119A RID: 4506
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Get Vector2 Length.")]
	public class GetVectorLength2D : FsmStateAction
	{
		// Token: 0x06007898 RID: 30872 RVA: 0x00248399 File Offset: 0x00246599
		public override void Reset()
		{
			this.Vector2 = null;
			this.StoreLength = null;
			this.EveryFrame = false;
		}

		// Token: 0x06007899 RID: 30873 RVA: 0x002483B0 File Offset: 0x002465B0
		public override void OnEnter()
		{
			this.DoVectorLength();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600789A RID: 30874 RVA: 0x002483C6 File Offset: 0x002465C6
		public override void OnFixedUpdate()
		{
			this.DoVectorLength();
		}

		// Token: 0x0600789B RID: 30875 RVA: 0x002483D0 File Offset: 0x002465D0
		private void DoVectorLength()
		{
			if (this.Vector2 == null)
			{
				return;
			}
			if (this.StoreLength == null)
			{
				return;
			}
			this.StoreLength.Value = this.Vector2.Value.magnitude;
		}

		// Token: 0x04007901 RID: 30977
		public FsmVector2 Vector2;

		// Token: 0x04007902 RID: 30978
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreLength;

		// Token: 0x04007903 RID: 30979
		public bool EveryFrame;
	}
}
