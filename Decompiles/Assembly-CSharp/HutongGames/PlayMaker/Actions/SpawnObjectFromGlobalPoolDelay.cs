using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D6E RID: 3438
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Spawns a prefab Game Object from the Global Object Pool on the Game Manager.")]
	public class SpawnObjectFromGlobalPoolDelay : FsmStateAction
	{
		// Token: 0x06006467 RID: 25703 RVA: 0x001FA3B7 File Offset: 0x001F85B7
		public override void Reset()
		{
			this.gameObject = null;
			this.spawnPoint = null;
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

		// Token: 0x06006468 RID: 25704 RVA: 0x001FA3F2 File Offset: 0x001F85F2
		public override void OnEnter()
		{
			this.timer = Random.Range(this.delayMin.Value, this.delayMax.Value);
		}

		// Token: 0x06006469 RID: 25705 RVA: 0x001FA415 File Offset: 0x001F8615
		public override void OnUpdate()
		{
			if (this.timer > 0f)
			{
				this.timer -= Time.deltaTime;
				return;
			}
			this.SpawnObject();
		}

		// Token: 0x0600646A RID: 25706 RVA: 0x001FA440 File Offset: 0x001F8640
		private void SpawnObject()
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
					GameObject gameObject = this.gameObject.Value.Spawn();
					gameObject.transform.position = a;
					gameObject.transform.eulerAngles = eulerAngles;
					this.storeObject.Value = gameObject;
					BlackThreadState.HandleDamagerSpawn(base.Owner, gameObject);
				}
			}
			base.Finish();
		}

		// Token: 0x040062FC RID: 25340
		[RequiredField]
		[Tooltip("GameObject to create. Usually a Prefab.")]
		public FsmGameObject gameObject;

		// Token: 0x040062FD RID: 25341
		[Tooltip("Optional Spawn Point.")]
		public FsmGameObject spawnPoint;

		// Token: 0x040062FE RID: 25342
		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		// Token: 0x040062FF RID: 25343
		[Tooltip("Rotation. NOTE: Overrides the rotation of the Spawn Point.")]
		public FsmVector3 rotation;

		// Token: 0x04006300 RID: 25344
		[UIHint(UIHint.Variable)]
		[Tooltip("Optionally store the created object.")]
		public FsmGameObject storeObject;

		// Token: 0x04006301 RID: 25345
		public FsmFloat delayMin;

		// Token: 0x04006302 RID: 25346
		public FsmFloat delayMax;

		// Token: 0x04006303 RID: 25347
		private float timer;
	}
}
