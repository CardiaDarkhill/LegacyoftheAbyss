using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001182 RID: 4482
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Get Vector2 Length.")]
	public class GetVector2Length : FsmStateAction
	{
		// Token: 0x0600782D RID: 30765 RVA: 0x00247188 File Offset: 0x00245388
		public override void Reset()
		{
			this.vector2 = null;
			this.storeLength = null;
			this.everyFrame = false;
		}

		// Token: 0x0600782E RID: 30766 RVA: 0x0024719F File Offset: 0x0024539F
		public override void OnEnter()
		{
			this.DoVectorLength();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600782F RID: 30767 RVA: 0x002471B5 File Offset: 0x002453B5
		public override void OnUpdate()
		{
			this.DoVectorLength();
		}

		// Token: 0x06007830 RID: 30768 RVA: 0x002471C0 File Offset: 0x002453C0
		private void DoVectorLength()
		{
			if (this.vector2 == null)
			{
				return;
			}
			if (this.storeLength == null)
			{
				return;
			}
			this.storeLength.Value = this.vector2.Value.magnitude;
		}

		// Token: 0x040078A5 RID: 30885
		[Tooltip("The Vector2 to get the length from")]
		public FsmVector2 vector2;

		// Token: 0x040078A6 RID: 30886
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Vector2 the length")]
		public FsmFloat storeLength;

		// Token: 0x040078A7 RID: 30887
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
