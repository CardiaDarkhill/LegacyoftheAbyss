using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001198 RID: 4504
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Get the XYZ channels of a Vector3 Variable and store them in Float Variables.")]
	public class GetVector3XYZ : FsmStateAction
	{
		// Token: 0x0600788F RID: 30863 RVA: 0x00248267 File Offset: 0x00246467
		public override void Reset()
		{
			this.vector3Variable = null;
			this.storeX = null;
			this.storeY = null;
			this.storeZ = null;
			this.everyFrame = false;
		}

		// Token: 0x06007890 RID: 30864 RVA: 0x0024828C File Offset: 0x0024648C
		public override void OnEnter()
		{
			this.DoGetVector3XYZ();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007891 RID: 30865 RVA: 0x002482A2 File Offset: 0x002464A2
		public override void OnUpdate()
		{
			this.DoGetVector3XYZ();
		}

		// Token: 0x06007892 RID: 30866 RVA: 0x002482AC File Offset: 0x002464AC
		private void DoGetVector3XYZ()
		{
			if (this.vector3Variable == null)
			{
				return;
			}
			if (this.storeX != null)
			{
				this.storeX.Value = this.vector3Variable.Value.x;
			}
			if (this.storeY != null)
			{
				this.storeY.Value = this.vector3Variable.Value.y;
			}
			if (this.storeZ != null)
			{
				this.storeZ.Value = this.vector3Variable.Value.z;
			}
		}

		// Token: 0x040078FA RID: 30970
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Vector3 variable to examine.")]
		public FsmVector3 vector3Variable;

		// Token: 0x040078FB RID: 30971
		[UIHint(UIHint.Variable)]
		[Tooltip("Store X value in a float variable.")]
		public FsmFloat storeX;

		// Token: 0x040078FC RID: 30972
		[UIHint(UIHint.Variable)]
		[Tooltip("Store Y value in a float variable.")]
		public FsmFloat storeY;

		// Token: 0x040078FD RID: 30973
		[UIHint(UIHint.Variable)]
		[Tooltip("Store Z value in a float variable.")]
		public FsmFloat storeZ;

		// Token: 0x040078FE RID: 30974
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
