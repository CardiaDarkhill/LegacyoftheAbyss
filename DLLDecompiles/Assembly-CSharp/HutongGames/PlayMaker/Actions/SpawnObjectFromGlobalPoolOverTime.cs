using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D6F RID: 3439
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Spawns a prefab Game Object from the Global Object Pool on the Game Manager.")]
	public class SpawnObjectFromGlobalPoolOverTime : FsmStateAction
	{
		// Token: 0x0600646C RID: 25708 RVA: 0x001FA572 File Offset: 0x001F8772
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
			this.frequency = null;
		}

		// Token: 0x0600646D RID: 25709 RVA: 0x001FA5AD File Offset: 0x001F87AD
		public override void OnEnter()
		{
			this.timer = 0f;
			base.OnEnter();
		}

		// Token: 0x0600646E RID: 25710 RVA: 0x001FA5C0 File Offset: 0x001F87C0
		public override void OnUpdate()
		{
			this.timer += Time.deltaTime;
			if (this.timer >= this.frequency.Value)
			{
				this.timer -= this.frequency.Value;
				if (this.gameObject.Value != null)
				{
					Vector3 a = Vector3.zero;
					Vector3 euler = Vector3.up;
					if (this.spawnPoint.Value != null)
					{
						a = this.spawnPoint.Value.transform.position;
						if (!this.position.IsNone)
						{
							a += this.position.Value;
						}
						euler = ((!this.rotation.IsNone) ? this.rotation.Value : this.spawnPoint.Value.transform.eulerAngles);
					}
					else
					{
						if (!this.position.IsNone)
						{
							a = this.position.Value;
						}
						if (!this.rotation.IsNone)
						{
							euler = this.rotation.Value;
						}
					}
					if (this.gameObject != null)
					{
						GameObject spawned = this.gameObject.Value.Spawn(a, Quaternion.Euler(euler));
						BlackThreadState.HandleDamagerSpawn(base.Owner, spawned);
					}
				}
			}
		}

		// Token: 0x04006304 RID: 25348
		[RequiredField]
		[Tooltip("GameObject to create. Usually a Prefab.")]
		public FsmGameObject gameObject;

		// Token: 0x04006305 RID: 25349
		[Tooltip("Optional Spawn Point.")]
		public FsmGameObject spawnPoint;

		// Token: 0x04006306 RID: 25350
		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		// Token: 0x04006307 RID: 25351
		[Tooltip("Rotation. NOTE: Overrides the rotation of the Spawn Point.")]
		public FsmVector3 rotation;

		// Token: 0x04006308 RID: 25352
		[Tooltip("How often, in seconds, spawn occurs.")]
		public FsmFloat frequency;

		// Token: 0x04006309 RID: 25353
		private float timer;
	}
}
