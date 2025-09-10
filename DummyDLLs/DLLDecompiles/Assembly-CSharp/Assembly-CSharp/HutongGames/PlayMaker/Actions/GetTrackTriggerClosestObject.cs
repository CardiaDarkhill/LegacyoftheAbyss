using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200133C RID: 4924
	public class GetTrackTriggerClosestObject : FsmStateAction
	{
		// Token: 0x06007F51 RID: 32593 RVA: 0x0025B054 File Offset: 0x00259254
		public override void Reset()
		{
			this.ClosestTo = null;
			this.TrackTrigger = null;
			this.StoreObject = null;
			this.ExcludedObject = null;
		}

		// Token: 0x06007F52 RID: 32594 RVA: 0x0025B074 File Offset: 0x00259274
		public override void OnEnter()
		{
			TrackTriggerObjects component = this.TrackTrigger.Value.GetComponent<TrackTriggerObjects>();
			if (component.IsInside)
			{
				if (this.excludeWrapper == null)
				{
					this.excludeWrapper = new List<GameObject>(1);
				}
				else
				{
					this.excludeWrapper.Clear();
				}
				if (this.ExcludedObject.Value)
				{
					this.excludeWrapper.Add(this.ExcludedObject.Value);
				}
				Vector2 toPos = this.ClosestTo.GetSafe(this).transform.position;
				GameObject closestInside = component.GetClosestInside(toPos, this.excludeWrapper);
				this.StoreObject.Value = closestInside;
			}
			else
			{
				this.StoreObject.Value = null;
			}
			base.Finish();
		}

		// Token: 0x04007ED8 RID: 32472
		public FsmOwnerDefault ClosestTo;

		// Token: 0x04007ED9 RID: 32473
		[CheckForComponent(typeof(TrackTriggerObjects))]
		public FsmGameObject TrackTrigger;

		// Token: 0x04007EDA RID: 32474
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreObject;

		// Token: 0x04007EDB RID: 32475
		[UIHint(UIHint.Variable)]
		public FsmGameObject ExcludedObject;

		// Token: 0x04007EDC RID: 32476
		private List<GameObject> excludeWrapper;
	}
}
