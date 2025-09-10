using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D72 RID: 3442
	public class SpawnProjectile : FsmStateAction
	{
		// Token: 0x06006476 RID: 25718 RVA: 0x001FAB61 File Offset: 0x001F8D61
		public bool IsNoSpawnPointSpecified()
		{
			return this.SpawnPoint.OwnerOption == OwnerDefaultOption.SpecifyGameObject && this.SpawnPoint.GameObject.IsNone;
		}

		// Token: 0x06006477 RID: 25719 RVA: 0x001FAB84 File Offset: 0x001F8D84
		public override void Reset()
		{
			this.SpawnPoint = new FsmOwnerDefault();
			this.Prefab = new FsmGameObject();
			this.Position = new FsmVector3
			{
				UseVariable = true
			};
			this.Rotation = new FsmQuaternion
			{
				UseVariable = true
			};
			this.ImpulseForce = new FsmVector2();
			this.Space = Space.Self;
			this.StoreSpawned = new FsmGameObject();
		}

		// Token: 0x06006478 RID: 25720 RVA: 0x001FABE8 File Offset: 0x001F8DE8
		public override void OnEnter()
		{
			Vector3 vector = this.Position.Value;
			Quaternion rotation = this.Rotation.Value;
			GameObject safe = this.SpawnPoint.GetSafe(this);
			if (safe)
			{
				vector += safe.transform.position;
				rotation = safe.transform.rotation;
			}
			if (this.Prefab.Value)
			{
				GameObject gameObject = this.Prefab.Value.Spawn(vector, rotation);
				this.StoreSpawned.Value = gameObject;
				Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
				if (component)
				{
					Vector2 force = (this.Space == Space.World || safe == null) ? this.ImpulseForce.Value : safe.transform.TransformVector(this.ImpulseForce.Value);
					component.AddForce(force, ForceMode2D.Impulse);
				}
			}
			base.Finish();
		}

		// Token: 0x0400631B RID: 25371
		public FsmOwnerDefault SpawnPoint;

		// Token: 0x0400631C RID: 25372
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmGameObject Prefab;

		// Token: 0x0400631D RID: 25373
		public FsmVector3 Position;

		// Token: 0x0400631E RID: 25374
		public FsmQuaternion Rotation;

		// Token: 0x0400631F RID: 25375
		public FsmVector2 ImpulseForce;

		// Token: 0x04006320 RID: 25376
		[HideIf("IsNoSpawnPointSpecified")]
		public Space Space;

		// Token: 0x04006321 RID: 25377
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreSpawned;
	}
}
