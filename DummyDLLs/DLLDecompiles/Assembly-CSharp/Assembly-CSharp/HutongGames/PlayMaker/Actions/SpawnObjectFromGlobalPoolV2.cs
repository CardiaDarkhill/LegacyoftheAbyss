using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D71 RID: 3441
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Spawns a prefab Game Object from the Global Object Pool on the Game Manager.")]
	public class SpawnObjectFromGlobalPoolV2 : FsmStateAction
	{
		// Token: 0x06006473 RID: 25715 RVA: 0x001FA990 File Offset: 0x001F8B90
		public override void Reset()
		{
			this.gameObject = null;
			this.spawnPoint = null;
			this.amount = 1;
			this.position = new FsmVector3
			{
				UseVariable = true
			};
			this.rotation = new FsmVector3
			{
				UseVariable = true
			};
			this.storeObject = null;
		}

		// Token: 0x06006474 RID: 25716 RVA: 0x001FA9E4 File Offset: 0x001F8BE4
		public override void OnEnter()
		{
			if (this.gameObject.Value != null)
			{
				Vector3 a = Vector3.zero;
				Vector3 eulerAngles = Vector3.zero;
				if (this.spawnPoint.Value != null)
				{
					a = this.spawnPoint.Value.transform.position;
					if (!this.position.IsNone)
					{
						a += this.position.Value;
					}
					eulerAngles = ((!this.rotation.IsNone) ? this.rotation.Value : this.spawnPoint.Value.transform.eulerAngles);
				}
				else
				{
					if (!this.position.IsNone)
					{
						a = this.position.Value;
					}
					if (!this.rotation.IsNone)
					{
						eulerAngles = this.rotation.Value;
					}
				}
				if (this.gameObject != null)
				{
					int value = this.amount.Value;
					for (int i = 1; i <= value; i++)
					{
						GameObject gameObject = this.gameObject.Value.Spawn(a);
						gameObject.transform.eulerAngles = eulerAngles;
						this.storeObject.Value = gameObject;
						BlackThreadState.HandleDamagerSpawn(base.Owner, gameObject);
						if (!this.setParent.IsNone)
						{
							GameObject value2 = this.setParent.Value;
							if (value2 != null)
							{
								gameObject.transform.parent = value2.transform;
							}
						}
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04006314 RID: 25364
		[RequiredField]
		[Tooltip("GameObject to create. Usually a Prefab.")]
		public FsmGameObject gameObject;

		// Token: 0x04006315 RID: 25365
		[Tooltip("Optional Spawn Point.")]
		public FsmGameObject spawnPoint;

		// Token: 0x04006316 RID: 25366
		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		// Token: 0x04006317 RID: 25367
		[Tooltip("Rotation. NOTE: Overrides the rotation of the Spawn Point.")]
		public FsmVector3 rotation;

		// Token: 0x04006318 RID: 25368
		public FsmInt amount;

		// Token: 0x04006319 RID: 25369
		[UIHint(UIHint.Variable)]
		[Tooltip("Optionally store the created object.")]
		public FsmGameObject storeObject;

		// Token: 0x0400631A RID: 25370
		public FsmGameObject setParent;
	}
}
