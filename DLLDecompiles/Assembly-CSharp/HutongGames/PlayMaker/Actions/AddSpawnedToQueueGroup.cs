using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001290 RID: 4752
	public class AddSpawnedToQueueGroup : FsmStateAction
	{
		// Token: 0x06007CDD RID: 31965 RVA: 0x00254997 File Offset: 0x00252B97
		public override void Reset()
		{
			this.Group = null;
			this.Target = null;
		}

		// Token: 0x06007CDE RID: 31966 RVA: 0x002549A8 File Offset: 0x00252BA8
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			SpawnQueueGroup spawnQueueGroup = this.Group.Value as SpawnQueueGroup;
			if (spawnQueueGroup != null)
			{
				spawnQueueGroup.AddSpawned(safe);
			}
			base.Finish();
		}

		// Token: 0x04007CEE RID: 31982
		[RequiredField]
		[ObjectType(typeof(SpawnQueueGroup))]
		public FsmObject Group;

		// Token: 0x04007CEF RID: 31983
		[RequiredField]
		public FsmOwnerDefault Target;
	}
}
