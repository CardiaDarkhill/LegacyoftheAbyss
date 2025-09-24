using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D73 RID: 3443
	public class SpawnProjectileV2 : FsmStateAction
	{
		// Token: 0x0600647A RID: 25722 RVA: 0x001FACDA File Offset: 0x001F8EDA
		[UsedImplicitly]
		public bool IsNoSpawnPointSpecified()
		{
			return this.SpawnPoint.OwnerOption == OwnerDefaultOption.SpecifyGameObject && this.SpawnPoint.GameObject.IsNone;
		}

		// Token: 0x0600647B RID: 25723 RVA: 0x001FACFC File Offset: 0x001F8EFC
		public override void Reset()
		{
			this.SpawnPoint = null;
			this.Prefab = null;
			this.Position = new FsmVector3
			{
				UseVariable = true
			};
			this.Rotation = new FsmQuaternion
			{
				UseVariable = true
			};
			this.FlipScaleFireLeft = null;
			this.Speed = null;
			this.MinAngle = null;
			this.MaxAngle = null;
			this.Space = Space.Self;
			this.StoreSpawned = null;
		}

		// Token: 0x0600647C RID: 25724 RVA: 0x001FAD68 File Offset: 0x001F8F68
		public override void OnEnter()
		{
			Vector3 position = this.Position.Value;
			Quaternion value = this.Rotation.Value;
			GameObject safe = this.SpawnPoint.GetSafe(this);
			if (safe)
			{
				position = safe.transform.TransformPoint(position);
			}
			if (this.Prefab.Value)
			{
				GameObject gameObject = this.Prefab.Value.Spawn(position, value);
				this.StoreSpawned.Value = gameObject;
				gameObject.transform.localScale = this.Prefab.Value.transform.localScale;
				Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
				if (component)
				{
					float value2 = this.Speed.Value;
					float num = Random.Range(this.MinAngle.Value, this.MaxAngle.Value);
					float x = value2 * Mathf.Cos(num * 0.017453292f);
					float y = value2 * Mathf.Sin(num * 0.017453292f);
					Vector2 vector = new Vector2(x, y);
					if (this.Space == Space.Self && safe != null)
					{
						vector = safe.transform.TransformVector(vector);
					}
					HeroController componentInParent = safe.GetComponentInParent<HeroController>();
					if (componentInParent && componentInParent.cState.wallSliding)
					{
						vector.x *= -1f;
					}
					if (vector.x < 0f && this.FlipScaleFireLeft.Value)
					{
						gameObject.transform.FlipLocalScale(true, false, false);
					}
					component.linearVelocity = vector;
					IProjectile component2 = gameObject.GetComponent<IProjectile>();
					if (component2 != null)
					{
						component2.VelocityWasSet();
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04006322 RID: 25378
		public FsmOwnerDefault SpawnPoint;

		// Token: 0x04006323 RID: 25379
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmGameObject Prefab;

		// Token: 0x04006324 RID: 25380
		public FsmVector3 Position;

		// Token: 0x04006325 RID: 25381
		public FsmQuaternion Rotation;

		// Token: 0x04006326 RID: 25382
		public FsmBool FlipScaleFireLeft;

		// Token: 0x04006327 RID: 25383
		public FsmFloat Speed;

		// Token: 0x04006328 RID: 25384
		public FsmFloat MinAngle;

		// Token: 0x04006329 RID: 25385
		public FsmFloat MaxAngle;

		// Token: 0x0400632A RID: 25386
		[HideIf("IsNoSpawnPointSpecified")]
		public Space Space;

		// Token: 0x0400632B RID: 25387
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreSpawned;
	}
}
