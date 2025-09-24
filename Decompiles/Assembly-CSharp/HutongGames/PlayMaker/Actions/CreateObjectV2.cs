using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EA2 RID: 3746
	[ActionCategory(ActionCategory.GameObject)]
	[ActionTarget(typeof(GameObject), "gameObject", true)]
	[Tooltip("Creates a Game Object, usually using a Prefab.")]
	public class CreateObjectV2 : FsmStateAction
	{
		// Token: 0x06006A37 RID: 27191 RVA: 0x00213364 File Offset: 0x00211564
		public override void Awake()
		{
			base.Awake();
			GameObject value = this.gameObject.Value;
			if (value != null)
			{
				this.preSpawnedObject = Object.Instantiate<GameObject>(value);
				this.preSpawnedObject.SetActive(false);
			}
		}

		// Token: 0x06006A38 RID: 27192 RVA: 0x002133A4 File Offset: 0x002115A4
		public override void Reset()
		{
			this.gameObject = null;
			this.canPreSpawn = true;
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

		// Token: 0x06006A39 RID: 27193 RVA: 0x002133F8 File Offset: 0x002115F8
		public override void OnEnter()
		{
			GameObject value = this.gameObject.Value;
			if (value != null)
			{
				Vector3 a = Vector3.zero;
				Vector3 euler = Vector3.zero;
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
				GameObject gameObject;
				if (this.preSpawnedObject)
				{
					gameObject = this.preSpawnedObject;
					gameObject.transform.position = a;
					gameObject.transform.rotation = Quaternion.Euler(euler);
					gameObject.gameObject.SetActive(true);
					this.preSpawnedObject = null;
				}
				else
				{
					gameObject = Object.Instantiate<GameObject>(value, a, Quaternion.Euler(euler));
				}
				this.storeObject.Value = gameObject;
			}
			base.Finish();
		}

		// Token: 0x04006992 RID: 27026
		[RequiredField]
		[Tooltip("GameObject to create. Usually a Prefab.")]
		public FsmGameObject gameObject;

		// Token: 0x04006993 RID: 27027
		public FsmBool canPreSpawn;

		// Token: 0x04006994 RID: 27028
		[Tooltip("Optional Spawn Point.")]
		public FsmGameObject spawnPoint;

		// Token: 0x04006995 RID: 27029
		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		// Token: 0x04006996 RID: 27030
		[Tooltip("Rotation. NOTE: Overrides the rotation of the Spawn Point.")]
		public FsmVector3 rotation;

		// Token: 0x04006997 RID: 27031
		[UIHint(UIHint.Variable)]
		[Tooltip("Optionally store the created object.")]
		public FsmGameObject storeObject;

		// Token: 0x04006998 RID: 27032
		private GameObject preSpawnedObject;
	}
}
