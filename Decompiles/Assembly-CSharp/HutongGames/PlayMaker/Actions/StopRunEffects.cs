using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200129A RID: 4762
	public class StopRunEffects : FsmStateAction
	{
		// Token: 0x06007D05 RID: 32005 RVA: 0x002553BC File Offset: 0x002535BC
		public override void Reset()
		{
			this.SpawnedObject = null;
		}

		// Token: 0x06007D06 RID: 32006 RVA: 0x002553C8 File Offset: 0x002535C8
		public override void OnEnter()
		{
			if (this.SpawnedObject.Value)
			{
				RunEffects component = this.SpawnedObject.Value.GetComponent<RunEffects>();
				if (component)
				{
					component.Stop();
				}
				this.SpawnedObject.Value = null;
			}
			base.Finish();
		}

		// Token: 0x04007D17 RID: 32023
		[CheckForComponent(typeof(RunEffects))]
		public FsmGameObject SpawnedObject;
	}
}
