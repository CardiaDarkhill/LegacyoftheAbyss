using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B30 RID: 2864
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Set an item at a specified index to a PlayMaker array List component")]
	public class ArrayListSet : ArrayListActions
	{
		// Token: 0x060059B4 RID: 22964 RVA: 0x001C6A82 File Offset: 0x001C4C82
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.variable = null;
			this.everyFrame = false;
		}

		// Token: 0x060059B5 RID: 22965 RVA: 0x001C6AA0 File Offset: 0x001C4CA0
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.SetToArrayList();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060059B6 RID: 22966 RVA: 0x001C6ADA File Offset: 0x001C4CDA
		public override void OnUpdate()
		{
			this.SetToArrayList();
		}

		// Token: 0x060059B7 RID: 22967 RVA: 0x001C6AE4 File Offset: 0x001C4CE4
		public void SetToArrayList()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.Set(this.atIndex.Value, PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.variable), this.variable.Type.ToString());
		}

		// Token: 0x0400552F RID: 21807
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005530 RID: 21808
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component (necessary if several component coexists on the same GameObject)")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;

		// Token: 0x04005531 RID: 21809
		[Tooltip("The index of the Data in the ArrayList")]
		[UIHint(UIHint.FsmString)]
		public FsmInt atIndex;

		// Token: 0x04005532 RID: 21810
		public bool everyFrame;

		// Token: 0x04005533 RID: 21811
		[ActionSection("Data")]
		[Tooltip("The variable to add.")]
		public FsmVar variable;
	}
}
