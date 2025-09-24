using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001299 RID: 4761
	public class SpawnRunEffects : FsmStateAction
	{
		// Token: 0x06007D01 RID: 32001 RVA: 0x002552B0 File Offset: 0x002534B0
		public override void Reset()
		{
			this.SpawnPoint = null;
			this.RunEffectsPrefab = null;
			this.StoreObject = null;
			this.StopOnExit = null;
		}

		// Token: 0x06007D02 RID: 32002 RVA: 0x002552D0 File Offset: 0x002534D0
		public override void OnEnter()
		{
			if (this.RunEffectsPrefab.Value)
			{
				GameObject safe = this.SpawnPoint.GetSafe(this);
				GameObject gameObject = this.RunEffectsPrefab.Value.Spawn();
				this.StoreObject.Value = gameObject;
				if (safe)
				{
					gameObject.transform.SetParent(safe.transform, false);
				}
				RunEffects component = gameObject.GetComponent<RunEffects>();
				if (component)
				{
					component.StartEffect(true, this.DoSprintmasterEffect.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x06007D03 RID: 32003 RVA: 0x0025535C File Offset: 0x0025355C
		public override void OnExit()
		{
			if (!this.StopOnExit.Value)
			{
				return;
			}
			if (this.StoreObject.Value)
			{
				RunEffects component = this.StoreObject.Value.GetComponent<RunEffects>();
				if (component)
				{
					component.Stop();
				}
				this.StoreObject.Value = null;
			}
		}

		// Token: 0x04007D12 RID: 32018
		public FsmOwnerDefault SpawnPoint;

		// Token: 0x04007D13 RID: 32019
		[CheckForComponent(typeof(RunEffects))]
		public FsmGameObject RunEffectsPrefab;

		// Token: 0x04007D14 RID: 32020
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreObject;

		// Token: 0x04007D15 RID: 32021
		public FsmBool DoSprintmasterEffect;

		// Token: 0x04007D16 RID: 32022
		public FsmBool StopOnExit;
	}
}
