using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001199 RID: 4505
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Get Vector3 Length.")]
	public class GetVectorLength : FsmStateAction
	{
		// Token: 0x06007894 RID: 30868 RVA: 0x00248333 File Offset: 0x00246533
		public override void Reset()
		{
			this.vector3 = null;
			this.storeLength = null;
		}

		// Token: 0x06007895 RID: 30869 RVA: 0x00248343 File Offset: 0x00246543
		public override void OnEnter()
		{
			this.DoVectorLength();
			base.Finish();
		}

		// Token: 0x06007896 RID: 30870 RVA: 0x00248354 File Offset: 0x00246554
		private void DoVectorLength()
		{
			if (this.vector3 == null)
			{
				return;
			}
			if (this.storeLength == null)
			{
				return;
			}
			this.storeLength.Value = this.vector3.Value.magnitude;
		}

		// Token: 0x040078FF RID: 30975
		public FsmVector3 vector3;

		// Token: 0x04007900 RID: 30976
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeLength;
	}
}
