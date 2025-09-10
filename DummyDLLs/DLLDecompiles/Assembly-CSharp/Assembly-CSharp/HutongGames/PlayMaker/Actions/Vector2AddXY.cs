using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001188 RID: 4488
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Adds a XY values to Vector2 Variable.")]
	public class Vector2AddXY : FsmStateAction
	{
		// Token: 0x06007849 RID: 30793 RVA: 0x00247564 File Offset: 0x00245764
		public override void Reset()
		{
			this.vector2Variable = null;
			this.addX = 0f;
			this.addY = 0f;
			this.everyFrame = false;
			this.perSecond = false;
		}

		// Token: 0x0600784A RID: 30794 RVA: 0x0024759B File Offset: 0x0024579B
		public override void OnEnter()
		{
			this.DoVector2AddXYZ();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600784B RID: 30795 RVA: 0x002475B1 File Offset: 0x002457B1
		public override void OnUpdate()
		{
			this.DoVector2AddXYZ();
		}

		// Token: 0x0600784C RID: 30796 RVA: 0x002475BC File Offset: 0x002457BC
		private void DoVector2AddXYZ()
		{
			Vector2 vector = new Vector2(this.addX.Value, this.addY.Value);
			if (this.perSecond)
			{
				this.vector2Variable.Value += vector * Time.deltaTime;
				return;
			}
			this.vector2Variable.Value += vector;
		}

		// Token: 0x040078BB RID: 30907
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The vector2 target")]
		public FsmVector2 vector2Variable;

		// Token: 0x040078BC RID: 30908
		[Tooltip("The x component to add")]
		public FsmFloat addX;

		// Token: 0x040078BD RID: 30909
		[Tooltip("The y component to add")]
		public FsmFloat addY;

		// Token: 0x040078BE RID: 30910
		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		// Token: 0x040078BF RID: 30911
		[Tooltip("Add the value on a per second bases.")]
		public bool perSecond;
	}
}
