using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B19 RID: 2841
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Add an item to a PlayMaker Array List Proxy component")]
	public class ArrayListAdd : ArrayListActions
	{
		// Token: 0x06005958 RID: 22872 RVA: 0x001C5100 File Offset: 0x001C3300
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.variable = null;
		}

		// Token: 0x06005959 RID: 22873 RVA: 0x001C5117 File Offset: 0x001C3317
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.AddToArrayList();
			}
			base.Finish();
		}

		// Token: 0x0600595A RID: 22874 RVA: 0x001C514C File Offset: 0x001C334C
		public void AddToArrayList()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.Add(PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.variable), this.variable.Type.ToString(), false);
		}

		// Token: 0x040054B8 RID: 21688
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040054B9 RID: 21689
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component (necessary if several component coexists on the same GameObject)")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;

		// Token: 0x040054BA RID: 21690
		[ActionSection("Data")]
		[RequiredField]
		[Tooltip("The variable to add.")]
		public FsmVar variable;
	}
}
