using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200119F RID: 4511
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Adds a XYZ values to Vector3 Variable.")]
	public class Vector3AddXYZ : FsmStateAction
	{
		// Token: 0x060078AF RID: 30895 RVA: 0x00248714 File Offset: 0x00246914
		public override void Reset()
		{
			this.vector3Variable = null;
			this.addX = 0f;
			this.addY = 0f;
			this.addZ = 0f;
			this.everyFrame = false;
			this.perSecond = false;
		}

		// Token: 0x060078B0 RID: 30896 RVA: 0x00248766 File Offset: 0x00246966
		public override void OnEnter()
		{
			this.DoVector3AddXYZ();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060078B1 RID: 30897 RVA: 0x0024877C File Offset: 0x0024697C
		public override void OnUpdate()
		{
			this.DoVector3AddXYZ();
		}

		// Token: 0x060078B2 RID: 30898 RVA: 0x00248784 File Offset: 0x00246984
		private void DoVector3AddXYZ()
		{
			Vector3 vector = new Vector3(this.addX.Value, this.addY.Value, this.addZ.Value);
			if (this.perSecond)
			{
				this.vector3Variable.Value += vector * Time.deltaTime;
				return;
			}
			this.vector3Variable.Value += vector;
		}

		// Token: 0x04007914 RID: 30996
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Vector3 Variable to add to.")]
		public FsmVector3 vector3Variable;

		// Token: 0x04007915 RID: 30997
		[Tooltip("Add to the X axis.")]
		public FsmFloat addX;

		// Token: 0x04007916 RID: 30998
		[Tooltip("Add to the Y axis.")]
		public FsmFloat addY;

		// Token: 0x04007917 RID: 30999
		[Tooltip("Add to the Z axis.")]
		public FsmFloat addZ;

		// Token: 0x04007918 RID: 31000
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007919 RID: 31001
		[Tooltip("Add over one second (multiplies values by Time.deltaTime). Note: Needs Every Frame checked.")]
		public bool perSecond;
	}
}
