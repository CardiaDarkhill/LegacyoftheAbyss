using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C41 RID: 3137
	[ActionCategory(ActionCategory.GameObject)]
	public class FlingObjectsFromGlobalPoolV2 : RigidBody2dActionBase
	{
		// Token: 0x06005F40 RID: 24384 RVA: 0x001E3B38 File Offset: 0x001E1D38
		public override void Awake()
		{
			base.Awake();
			if (Application.isPlaying && this.GameObjects != null)
			{
				GameObject owner = base.Owner;
				int value = this.SpawnMax.Value;
				bool flag = false;
				foreach (GameObject gameObject in this.GameObjects.Values)
				{
					if (!(gameObject == null))
					{
						flag = true;
						PersonalObjectPool.EnsurePooledInScene(owner, gameObject, value, false, false, false);
					}
				}
				if (flag)
				{
					PersonalObjectPool.CreateIfRequired(owner, false);
				}
			}
		}

		// Token: 0x06005F41 RID: 24385 RVA: 0x001E3BBC File Offset: 0x001E1DBC
		public override void Reset()
		{
			this.GameObjects = null;
			this.SpawnPoint = null;
			this.Position = new FsmVector3
			{
				UseVariable = true
			};
			this.SpawnMin = null;
			this.SpawnMax = null;
			this.SpeedMin = null;
			this.SpeedMax = null;
			this.AngleMin = null;
			this.AngleMax = null;
			this.OriginVariationX = null;
			this.OriginVariationY = null;
		}

		// Token: 0x06005F42 RID: 24386 RVA: 0x001E3C24 File Offset: 0x001E1E24
		public override void OnEnter()
		{
			Vector3 vector = Vector3.zero;
			Vector3 zero = Vector3.zero;
			if (this.SpawnPoint.Value != null)
			{
				vector = this.SpawnPoint.Value.transform.position;
				if (!this.Position.IsNone)
				{
					vector += this.Position.Value;
				}
			}
			else if (!this.Position.IsNone)
			{
				vector = this.Position.Value;
			}
			this.gameObjects.Clear();
			foreach (GameObject gameObject in this.GameObjects.Values)
			{
				if (!(gameObject == null))
				{
					this.gameObjects.Add(gameObject);
				}
			}
			int num = Random.Range(this.SpawnMin.Value, this.SpawnMax.Value + 1);
			if (num > 0)
			{
				float value = this.SpeedMin.Value;
				float value2 = this.SpeedMax.Value;
				float value3 = this.AngleMin.Value;
				float value4 = this.AngleMax.Value;
				bool flag = this.OriginVariationX != null;
				float num2 = flag ? this.OriginVariationX.Value : 0f;
				bool flag2 = this.OriginVariationY != null;
				float num3 = flag2 ? this.OriginVariationY.Value : 0f;
				for (int j = 1; j <= num; j++)
				{
					int index = (this.gameObjects.Count > 1) ? Random.Range(0, this.gameObjects.Count) : 0;
					GameObject prefab = this.gameObjects[index];
					float x = 0f;
					float y = 0f;
					if (flag)
					{
						x = Random.Range(-num2, num2);
						this.originAdjusted = true;
					}
					if (flag2)
					{
						y = Random.Range(-num3, num3);
						this.originAdjusted = true;
					}
					Vector3 vector2 = vector;
					if (this.originAdjusted)
					{
						vector2 += new Vector3(x, y, 0f);
					}
					GameObject gameObject2 = prefab.Spawn(vector2, Quaternion.Euler(zero));
					BlackThreadState.HandleDamagerSpawn(base.Owner, gameObject2);
					this.rb2d = gameObject2.GetComponent<Rigidbody2D>();
					float num4 = Random.Range(value, value2);
					float num5 = Random.Range(value3, value4);
					this.vectorX = num4 * Mathf.Cos(num5 * 0.017453292f);
					this.vectorY = num4 * Mathf.Sin(num5 * 0.017453292f);
					Vector2 linearVelocity;
					linearVelocity.x = this.vectorX;
					linearVelocity.y = this.vectorY;
					this.rb2d.linearVelocity = linearVelocity;
				}
			}
			base.Finish();
		}

		// Token: 0x04005C67 RID: 23655
		[RequiredField]
		[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
		public FsmArray GameObjects;

		// Token: 0x04005C68 RID: 23656
		public FsmGameObject SpawnPoint;

		// Token: 0x04005C69 RID: 23657
		public FsmVector3 Position;

		// Token: 0x04005C6A RID: 23658
		public FsmInt SpawnMin;

		// Token: 0x04005C6B RID: 23659
		public FsmInt SpawnMax;

		// Token: 0x04005C6C RID: 23660
		public FsmFloat SpeedMin;

		// Token: 0x04005C6D RID: 23661
		public FsmFloat SpeedMax;

		// Token: 0x04005C6E RID: 23662
		public FsmFloat AngleMin;

		// Token: 0x04005C6F RID: 23663
		public FsmFloat AngleMax;

		// Token: 0x04005C70 RID: 23664
		public FsmFloat OriginVariationX;

		// Token: 0x04005C71 RID: 23665
		public FsmFloat OriginVariationY;

		// Token: 0x04005C72 RID: 23666
		private float vectorX;

		// Token: 0x04005C73 RID: 23667
		private float vectorY;

		// Token: 0x04005C74 RID: 23668
		private bool originAdjusted;

		// Token: 0x04005C75 RID: 23669
		private List<GameObject> gameObjects = new List<GameObject>();
	}
}
