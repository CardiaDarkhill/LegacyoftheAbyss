using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D70 RID: 3440
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Spawns a prefab Game Object from the Global Object Pool on the Game Manager.")]
	public class SpawnObjectFromGlobalPoolOverTimeV2 : FsmStateAction
	{
		// Token: 0x06006470 RID: 25712 RVA: 0x001FA710 File Offset: 0x001F8910
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
			this.originVariationX = null;
			this.originVariationY = null;
		}

		// Token: 0x06006471 RID: 25713 RVA: 0x001FA764 File Offset: 0x001F8964
		public override void OnUpdate()
		{
			this.timer += Time.deltaTime;
			if (this.timer >= this.frequency.Value)
			{
				this.timer = 0f;
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
						GameObject gameObject = this.gameObject.Value.Spawn(a, Quaternion.Euler(euler));
						if (this.originVariationX != null)
						{
							gameObject.transform.Translate(Random.Range(-this.originVariationX.Value, this.originVariationX.Value), 0f, 0f, Space.World);
						}
						if (this.originVariationY != null)
						{
							gameObject.transform.Translate(0f, Random.Range(-this.originVariationY.Value, this.originVariationY.Value), 0f, Space.World);
						}
						if (this.scaleMin != null && this.scaleMax != null)
						{
							float num = Random.Range(this.scaleMin.Value, this.scaleMax.Value);
							if (Math.Abs(num - 1f) > 0.001f)
							{
								gameObject.transform.localScale = new Vector3(num, num, num);
							}
						}
						BlackThreadState.HandleDamagerSpawn(base.Owner, gameObject);
					}
				}
			}
		}

		// Token: 0x0400630A RID: 25354
		[RequiredField]
		[Tooltip("GameObject to create. Usually a Prefab.")]
		public FsmGameObject gameObject;

		// Token: 0x0400630B RID: 25355
		[Tooltip("Optional Spawn Point.")]
		public FsmGameObject spawnPoint;

		// Token: 0x0400630C RID: 25356
		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		// Token: 0x0400630D RID: 25357
		[Tooltip("Randomises spawn points of objects within this range. Leave as 0 and all objects will spawn at same point.")]
		public FsmFloat originVariationX;

		// Token: 0x0400630E RID: 25358
		public FsmFloat originVariationY;

		// Token: 0x0400630F RID: 25359
		[Tooltip("Rotation. NOTE: Overrides the rotation of the Spawn Point.")]
		public FsmVector3 rotation;

		// Token: 0x04006310 RID: 25360
		[Tooltip("How often, in seconds, spawn occurs.")]
		public FsmFloat frequency;

		// Token: 0x04006311 RID: 25361
		[Tooltip("Minimum scale of clone.")]
		public FsmFloat scaleMin = 1f;

		// Token: 0x04006312 RID: 25362
		[Tooltip("Maximum scale of clone.")]
		public FsmFloat scaleMax = 1f;

		// Token: 0x04006313 RID: 25363
		private float timer;
	}
}
