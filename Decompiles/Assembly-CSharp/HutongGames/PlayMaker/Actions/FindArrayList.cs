using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B37 RID: 2871
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Finds an ArrayList by reference. Warning: this function can be very slow.")]
	public class FindArrayList : CollectionsActions
	{
		// Token: 0x060059D2 RID: 22994 RVA: 0x001C7040 File Offset: 0x001C5240
		public override void Reset()
		{
			this.ArrayListReference = "";
			this.store = null;
			this.foundEvent = null;
			this.notFoundEvent = null;
		}

		// Token: 0x060059D3 RID: 22995 RVA: 0x001C7068 File Offset: 0x001C5268
		public override void OnEnter()
		{
			foreach (PlayMakerArrayListProxy playMakerArrayListProxy in Object.FindObjectsOfType(typeof(PlayMakerArrayListProxy)) as PlayMakerArrayListProxy[])
			{
				if (playMakerArrayListProxy.referenceName == this.ArrayListReference.Value)
				{
					this.store.Value = playMakerArrayListProxy.gameObject;
					base.Fsm.Event(this.foundEvent);
					return;
				}
			}
			this.store.Value = null;
			base.Fsm.Event(this.notFoundEvent);
			base.Finish();
		}

		// Token: 0x0400554A RID: 21834
		[ActionSection("Set up")]
		[RequiredField]
		[UIHint(UIHint.FsmString)]
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component")]
		public FsmString ArrayListReference;

		// Token: 0x0400554B RID: 21835
		[ActionSection("Result")]
		[RequiredField]
		[Tooltip("Store the GameObject hosting the PlayMaker ArrayList Proxy component here")]
		public FsmGameObject store;

		// Token: 0x0400554C RID: 21836
		public FsmEvent foundEvent;

		// Token: 0x0400554D RID: 21837
		public FsmEvent notFoundEvent;
	}
}
