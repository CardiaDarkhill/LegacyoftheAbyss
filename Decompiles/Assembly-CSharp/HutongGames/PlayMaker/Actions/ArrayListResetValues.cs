using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B2D RID: 2861
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Sets all element to to a given value of a PlayMaker ArrayList Proxy component")]
	public class ArrayListResetValues : ArrayListActions
	{
		// Token: 0x060059A8 RID: 22952 RVA: 0x001C6914 File Offset: 0x001C4B14
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.resetValue = null;
		}

		// Token: 0x060059A9 RID: 22953 RVA: 0x001C692B File Offset: 0x001C4B2B
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.ResetArrayList();
			}
			base.Finish();
		}

		// Token: 0x060059AA RID: 22954 RVA: 0x001C6960 File Offset: 0x001C4B60
		public void ResetArrayList()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			object valueFromFsmVar = PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.resetValue);
			for (int i = 0; i < this.proxy.arrayList.Count; i++)
			{
				this.proxy.arrayList[i] = valueFromFsmVar;
			}
		}

		// Token: 0x04005528 RID: 21800
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005529 RID: 21801
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x0400552A RID: 21802
		[Tooltip("The value to reset all the arrayList with")]
		public FsmVar resetValue;
	}
}
