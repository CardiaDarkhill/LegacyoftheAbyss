using System;
using System.Linq;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200133D RID: 4925
	public class StoreTrackTriggerListAsArray : FsmStateAction
	{
		// Token: 0x06007F54 RID: 32596 RVA: 0x0025B138 File Offset: 0x00259338
		public override void Reset()
		{
			this.TrackTrigger = null;
			this.Array = null;
		}

		// Token: 0x06007F55 RID: 32597 RVA: 0x0025B148 File Offset: 0x00259348
		public override void OnEnter()
		{
			TrackTriggerObjects component = this.TrackTrigger.Value.GetComponent<TrackTriggerObjects>();
			FsmArray array = this.Array;
			object[] values = component.InsideGameObjects.ToArray<GameObject>();
			array.Values = values;
			base.Finish();
		}

		// Token: 0x04007EDD RID: 32477
		[CheckForComponent(typeof(TrackTriggerObjects))]
		public FsmGameObject TrackTrigger;

		// Token: 0x04007EDE RID: 32478
		[UIHint(UIHint.Variable)]
		public FsmArray Array;

		// Token: 0x04007EDF RID: 32479
		private TrackTriggerObjects track;
	}
}
