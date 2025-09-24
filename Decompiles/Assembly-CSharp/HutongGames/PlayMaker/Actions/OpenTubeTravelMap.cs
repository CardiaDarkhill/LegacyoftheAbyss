using System;
using GlobalEnums;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200136D RID: 4973
	public class OpenTubeTravelMap : FsmStateAction
	{
		// Token: 0x0600802F RID: 32815 RVA: 0x0025DA2B File Offset: 0x0025BC2B
		public override void Reset()
		{
			this.SpawnedMap = null;
			this.ClosedEvent = null;
			this.StoreLocation = null;
			this.ThisLocation = null;
		}

		// Token: 0x06008030 RID: 32816 RVA: 0x0025DA4C File Offset: 0x0025BC4C
		public override void OnEnter()
		{
			if (this.SpawnedMap.Value)
			{
				this.spawnedMap = this.SpawnedMap.Value.GetComponent<TubeTravelMap>();
			}
			if (this.spawnedMap)
			{
				this.spawnedMap.LocationConfirmed += this.OnLocationConfirmed;
				this.spawnedMap.PaneClosed += this.OnPaneClosed;
				this.spawnedMap.AutoSelectLocation = (TubeTravelLocations)this.ThisLocation.Value;
				this.spawnedMap.Open();
				return;
			}
			base.Finish();
		}

		// Token: 0x06008031 RID: 32817 RVA: 0x0025DAE9 File Offset: 0x0025BCE9
		private void OnLocationConfirmed(TubeTravelLocations targetLocation)
		{
			this.spawnedMap.LocationConfirmed -= this.OnLocationConfirmed;
			this.StoreLocation.Value = targetLocation;
		}

		// Token: 0x06008032 RID: 32818 RVA: 0x0025DB13 File Offset: 0x0025BD13
		private void OnPaneClosed()
		{
			this.spawnedMap.PaneClosed -= this.OnPaneClosed;
			base.Fsm.Event(this.ClosedEvent);
			base.Finish();
		}

		// Token: 0x04007F97 RID: 32663
		public FsmGameObject SpawnedMap;

		// Token: 0x04007F98 RID: 32664
		public FsmEvent ClosedEvent;

		// Token: 0x04007F99 RID: 32665
		[ObjectType(typeof(TubeTravelLocations))]
		[UIHint(UIHint.Variable)]
		public FsmEnum StoreLocation;

		// Token: 0x04007F9A RID: 32666
		[ObjectType(typeof(TubeTravelLocations))]
		public FsmEnum ThisLocation;

		// Token: 0x04007F9B RID: 32667
		private TubeTravelMap spawnedMap;
	}
}
