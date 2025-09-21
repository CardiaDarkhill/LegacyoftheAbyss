using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EA1 RID: 3745
	[ActionCategory(ActionCategory.GameObject)]
	[ActionTarget(typeof(GameObject), "gameObject", true)]
	[Tooltip("Creates a Game Object, usually using a Prefab.")]
	public class CreateObject : FsmStateAction
	{
		// Token: 0x06006A34 RID: 27188 RVA: 0x0021321E File Offset: 0x0021141E
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

		// Token: 0x06006A35 RID: 27189 RVA: 0x0021325C File Offset: 0x0021145C
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
				GameObject value2 = Object.Instantiate<GameObject>(value, a, Quaternion.Euler(euler));
				this.storeObject.Value = value2;
			}
			base.Finish();
		}

		// Token: 0x0400698D RID: 27021
		[RequiredField]
		[Tooltip("GameObject to create. Usually a Prefab.")]
		public FsmGameObject gameObject;

		// Token: 0x0400698E RID: 27022
		[Tooltip("Optional Spawn Point.")]
		public FsmGameObject spawnPoint;

		// Token: 0x0400698F RID: 27023
		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		// Token: 0x04006990 RID: 27024
		[Tooltip("Rotation. NOTE: Overrides the rotation of the Spawn Point.")]
		public FsmVector3 rotation;

		// Token: 0x04006991 RID: 27025
		[UIHint(UIHint.Variable)]
		[Tooltip("Optionally store the created object.")]
		public FsmGameObject storeObject;
	}
}
