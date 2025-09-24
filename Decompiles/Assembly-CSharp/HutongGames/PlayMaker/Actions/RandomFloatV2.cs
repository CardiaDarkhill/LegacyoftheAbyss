using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CE7 RID: 3303
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets a Float Variable to a random value between Min/Max.")]
	public class RandomFloatV2 : FsmStateAction
	{
		// Token: 0x0600622E RID: 25134 RVA: 0x001F0926 File Offset: 0x001EEB26
		public override void Reset()
		{
			this.min = 0f;
			this.max = 1f;
			this.storeResult = null;
		}

		// Token: 0x0600622F RID: 25135 RVA: 0x001F094F File Offset: 0x001EEB4F
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006230 RID: 25136 RVA: 0x001F095D File Offset: 0x001EEB5D
		public override void OnEnter()
		{
			this.Randomise();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006231 RID: 25137 RVA: 0x001F0973 File Offset: 0x001EEB73
		public override void OnFixedUpdate()
		{
			this.Randomise();
		}

		// Token: 0x06006232 RID: 25138 RVA: 0x001F097B File Offset: 0x001EEB7B
		private void Randomise()
		{
			this.storeResult.Value = Random.Range(this.min.Value, this.max.Value);
		}

		// Token: 0x04006048 RID: 24648
		[RequiredField]
		public FsmFloat min;

		// Token: 0x04006049 RID: 24649
		[RequiredField]
		public FsmFloat max;

		// Token: 0x0400604A RID: 24650
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeResult;

		// Token: 0x0400604B RID: 24651
		public bool everyFrame;
	}
}
