using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B36 RID: 2870
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Destroys a PlayMakerArrayListProxy Component of a Game Object.")]
	public class DestroyArrayList : ArrayListActions
	{
		// Token: 0x060059CE RID: 22990 RVA: 0x001C6F5C File Offset: 0x001C515C
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.successEvent = null;
			this.notFoundEvent = null;
		}

		// Token: 0x060059CF RID: 22991 RVA: 0x001C6F7C File Offset: 0x001C517C
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.DoDestroyArrayList();
			}
			else
			{
				base.Fsm.Event(this.notFoundEvent);
			}
			base.Finish();
		}

		// Token: 0x060059D0 RID: 22992 RVA: 0x001C6FCC File Offset: 0x001C51CC
		private void DoDestroyArrayList()
		{
			foreach (PlayMakerArrayListProxy playMakerArrayListProxy in this.proxy.GetComponents<PlayMakerArrayListProxy>())
			{
				if (playMakerArrayListProxy.referenceName == this.reference.Value)
				{
					Object.Destroy(playMakerArrayListProxy);
					base.Fsm.Event(this.successEvent);
					return;
				}
			}
			base.Fsm.Event(this.notFoundEvent);
		}

		// Token: 0x04005546 RID: 21830
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005547 RID: 21831
		[Tooltip("Author defined Reference of the PlayMaker ArrayList proxy component ( necessary if several component coexists on the same GameObject")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;

		// Token: 0x04005548 RID: 21832
		[ActionSection("Result")]
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the ArrayList proxy component is destroyed")]
		public FsmEvent successEvent;

		// Token: 0x04005549 RID: 21833
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the ArrayList proxy component was not found")]
		public FsmEvent notFoundEvent;
	}
}
