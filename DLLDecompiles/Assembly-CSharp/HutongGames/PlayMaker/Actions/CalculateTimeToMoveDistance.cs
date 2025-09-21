using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BD8 RID: 3032
	[ActionCategory(ActionCategory.Math)]
	public class CalculateTimeToMoveDistance : FsmStateAction
	{
		// Token: 0x06005CF5 RID: 23797 RVA: 0x001D356D File Offset: 0x001D176D
		public override void Reset()
		{
			this.Speed = null;
			this.Distance = new FsmFloat
			{
				UseVariable = true
			};
			this.FromPositon = null;
			this.ToPositon = null;
			this.Time = null;
		}

		// Token: 0x06005CF6 RID: 23798 RVA: 0x001D359D File Offset: 0x001D179D
		public bool IsDistanceSpecified()
		{
			return !this.Distance.IsNone;
		}

		// Token: 0x06005CF7 RID: 23799 RVA: 0x001D35B0 File Offset: 0x001D17B0
		public override void OnEnter()
		{
			float num = (!this.Distance.IsNone) ? this.Distance.Value : Vector3.Distance(this.FromPositon.Value, this.ToPositon.Value);
			this.Time.Value = num / this.Speed.Value;
			base.Finish();
		}

		// Token: 0x04005890 RID: 22672
		public FsmFloat Speed;

		// Token: 0x04005891 RID: 22673
		public FsmFloat Distance;

		// Token: 0x04005892 RID: 22674
		[HideIf("IsDistanceSpecified")]
		public FsmVector3 FromPositon;

		// Token: 0x04005893 RID: 22675
		[HideIf("IsDistanceSpecified")]
		public FsmVector3 ToPositon;

		// Token: 0x04005894 RID: 22676
		[UIHint(UIHint.Variable)]
		public FsmFloat Time;
	}
}
