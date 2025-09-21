using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C8C RID: 3212
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Adds a value to an Integer Variable.")]
	public class IntAddDelay : FsmStateAction
	{
		// Token: 0x06006095 RID: 24725 RVA: 0x001EA100 File Offset: 0x001E8300
		public override void Reset()
		{
			this.intVariable = null;
			this.add = null;
			this.timer = 0f;
		}

		// Token: 0x06006096 RID: 24726 RVA: 0x001EA11B File Offset: 0x001E831B
		public override void OnEnter()
		{
			this.timer = 0f;
		}

		// Token: 0x06006097 RID: 24727 RVA: 0x001EA128 File Offset: 0x001E8328
		public override void OnUpdate()
		{
			if (this.timer < this.delay)
			{
				this.timer += Time.deltaTime;
				return;
			}
			this.intVariable.Value += this.add.Value;
			if (this.repeat)
			{
				this.timer -= this.delay;
				return;
			}
			base.Finish();
		}

		// Token: 0x04005E15 RID: 24085
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt intVariable;

		// Token: 0x04005E16 RID: 24086
		[RequiredField]
		public FsmInt add;

		// Token: 0x04005E17 RID: 24087
		public float delay;

		// Token: 0x04005E18 RID: 24088
		public bool repeat;

		// Token: 0x04005E19 RID: 24089
		private float timer;
	}
}
