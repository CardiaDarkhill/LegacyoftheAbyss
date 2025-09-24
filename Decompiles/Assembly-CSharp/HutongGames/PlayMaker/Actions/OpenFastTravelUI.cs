using System;
using GlobalEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200136A RID: 4970
	public class OpenFastTravelUI : FsmStateAction
	{
		// Token: 0x06008024 RID: 32804 RVA: 0x0025D803 File Offset: 0x0025BA03
		public override void Reset()
		{
			this.SpawnedUI = null;
			this.ClosedEvent = null;
			this.StoreLocation = null;
			this.ThisLocation = null;
			this.StoreDestinationTune = null;
		}

		// Token: 0x06008025 RID: 32805 RVA: 0x0025D828 File Offset: 0x0025BA28
		public override void OnEnter()
		{
			if (this.SpawnedUI.Value)
			{
				this.spawnedMap = this.SpawnedUI.Value.GetComponent<FastTravelMap>();
			}
			if (this.spawnedMap)
			{
				this.spawnedMap.LocationConfirmed += this.OnLocationConfirmed;
				this.spawnedMap.PaneClosed += this.OnPaneClosed;
				this.spawnedMap.AutoSelectLocation = (FastTravelLocations)this.ThisLocation.Value;
				this.spawnedMap.Open();
				return;
			}
			base.Finish();
		}

		// Token: 0x06008026 RID: 32806 RVA: 0x0025D8C5 File Offset: 0x0025BAC5
		private void OnLocationConfirmed(FastTravelLocations targetLocation)
		{
			this.spawnedMap.LocationConfirmed -= this.OnLocationConfirmed;
			this.StoreLocation.Value = targetLocation;
			this.StoreDestinationTune.Value = null;
		}

		// Token: 0x06008027 RID: 32807 RVA: 0x0025D8FB File Offset: 0x0025BAFB
		private void OnPaneClosed()
		{
			this.spawnedMap.PaneClosed -= this.OnPaneClosed;
			base.Fsm.Event(this.ClosedEvent);
			base.Finish();
		}

		// Token: 0x04007F8B RID: 32651
		public FsmGameObject SpawnedUI;

		// Token: 0x04007F8C RID: 32652
		public FsmEvent ClosedEvent;

		// Token: 0x04007F8D RID: 32653
		[ObjectType(typeof(FastTravelLocations))]
		[UIHint(UIHint.Variable)]
		public FsmEnum StoreLocation;

		// Token: 0x04007F8E RID: 32654
		[ObjectType(typeof(FastTravelLocations))]
		public FsmEnum ThisLocation;

		// Token: 0x04007F8F RID: 32655
		[ObjectType(typeof(AudioClip))]
		[UIHint(UIHint.Variable)]
		public FsmObject StoreDestinationTune;

		// Token: 0x04007F90 RID: 32656
		private FastTravelMap spawnedMap;
	}
}
