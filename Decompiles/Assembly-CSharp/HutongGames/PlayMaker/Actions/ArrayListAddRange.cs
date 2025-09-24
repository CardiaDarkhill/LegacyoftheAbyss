using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B1A RID: 2842
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Add several items to a PlayMaker Array List Proxy component")]
	public class ArrayListAddRange : ArrayListActions
	{
		// Token: 0x0600595C RID: 22876 RVA: 0x001C51A0 File Offset: 0x001C33A0
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.variables = new FsmVar[2];
		}

		// Token: 0x0600595D RID: 22877 RVA: 0x001C51BC File Offset: 0x001C33BC
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.DoArrayListAddRange();
			}
			base.Finish();
		}

		// Token: 0x0600595E RID: 22878 RVA: 0x001C51F0 File Offset: 0x001C33F0
		public void DoArrayListAddRange()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			foreach (FsmVar fsmVar in this.variables)
			{
				this.proxy.Add(PlayMakerUtils.GetValueFromFsmVar(base.Fsm, fsmVar), fsmVar.Type.ToString(), true);
			}
		}

		// Token: 0x040054BB RID: 21691
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040054BC RID: 21692
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component (necessary if several component coexists on the same GameObject)")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;

		// Token: 0x040054BD RID: 21693
		[ActionSection("Data")]
		[RequiredField]
		[Tooltip("The variables to add.")]
		public FsmVar[] variables;
	}
}
