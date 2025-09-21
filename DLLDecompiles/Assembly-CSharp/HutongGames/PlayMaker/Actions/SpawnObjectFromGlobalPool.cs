using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D6D RID: 3437
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Spawns a prefab Game Object from the Global Object Pool on the Game Manager.")]
	public class SpawnObjectFromGlobalPool : FsmStateAction
	{
		// Token: 0x06006463 RID: 25699 RVA: 0x001FA224 File Offset: 0x001F8424
		public override void Awake()
		{
			base.Awake();
			GameObject value = this.gameObject.Value;
			if (value)
			{
				PersonalObjectPool.EnsurePooledInScene(base.Fsm.Owner.gameObject, value, 1, true, false, false);
			}
		}

		// Token: 0x06006464 RID: 25700 RVA: 0x001FA265 File Offset: 0x001F8465
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

		// Token: 0x06006465 RID: 25701 RVA: 0x001FA2A0 File Offset: 0x001F84A0
		public override void OnEnter()
		{
			GameObject value = this.gameObject.Value;
			if (value != null)
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
				GameObject gameObject = value.Spawn(a);
				gameObject.transform.eulerAngles = eulerAngles;
				this.storeObject.Value = gameObject;
				BlackThreadState.HandleDamagerSpawn(base.Owner, gameObject);
			}
			base.Finish();
		}

		// Token: 0x040062F7 RID: 25335
		[RequiredField]
		[Tooltip("GameObject to create. Usually a Prefab.")]
		public FsmGameObject gameObject;

		// Token: 0x040062F8 RID: 25336
		[Tooltip("Optional Spawn Point.")]
		public FsmGameObject spawnPoint;

		// Token: 0x040062F9 RID: 25337
		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		// Token: 0x040062FA RID: 25338
		[Tooltip("Rotation. NOTE: Overrides the rotation of the Spawn Point.")]
		public FsmVector3 rotation;

		// Token: 0x040062FB RID: 25339
		[UIHint(UIHint.Variable)]
		[Tooltip("Optionally store the created object.")]
		public FsmGameObject storeObject;
	}
}
