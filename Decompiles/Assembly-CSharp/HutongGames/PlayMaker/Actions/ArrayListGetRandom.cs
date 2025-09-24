using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B24 RID: 2852
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Gets a random item from a PlayMaker ArrayList Proxy component")]
	public class ArrayListGetRandom : ArrayListActions
	{
		// Token: 0x06005985 RID: 22917 RVA: 0x001C5F10 File Offset: 0x001C4110
		public override void Reset()
		{
			this.gameObject = null;
			this.failureEvent = null;
			this.randomItem = null;
			this.randomIndex = null;
		}

		// Token: 0x06005986 RID: 22918 RVA: 0x001C5F2E File Offset: 0x001C412E
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.GetRandomItem();
			}
			base.Finish();
		}

		// Token: 0x06005987 RID: 22919 RVA: 0x001C5F60 File Offset: 0x001C4160
		public void GetRandomItem()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			int num = Random.Range(0, this.proxy.arrayList.Count);
			object value = null;
			try
			{
				value = this.proxy.arrayList[num];
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.Message);
				base.Fsm.Event(this.failureEvent);
				return;
			}
			this.randomIndex.Value = num;
			if (!PlayMakerUtils.ApplyValueToFsmVar(base.Fsm, this.randomItem, value))
			{
				Debug.LogWarning("ApplyValueToFsmVar failed");
				base.Fsm.Event(this.failureEvent);
				return;
			}
		}

		// Token: 0x040054F4 RID: 21748
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040054F5 RID: 21749
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x040054F6 RID: 21750
		[ActionSection("Result")]
		[Tooltip("The random item data picked from the array")]
		[UIHint(UIHint.Variable)]
		public FsmVar randomItem;

		// Token: 0x040054F7 RID: 21751
		[Tooltip("The random item index picked from the array")]
		[UIHint(UIHint.Variable)]
		public FsmInt randomIndex;

		// Token: 0x040054F8 RID: 21752
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the action fails ( likely and index is out of range exception)")]
		public FsmEvent failureEvent;
	}
}
