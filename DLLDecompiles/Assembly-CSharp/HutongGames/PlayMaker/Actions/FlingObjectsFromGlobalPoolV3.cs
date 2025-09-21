using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C42 RID: 3138
	[ActionCategory(ActionCategory.GameObject)]
	public sealed class FlingObjectsFromGlobalPoolV3 : RigidBody2dActionBase
	{
		// Token: 0x06005F44 RID: 24388 RVA: 0x001E3EDC File Offset: 0x001E20DC
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

		// Token: 0x06005F45 RID: 24389 RVA: 0x001E3F60 File Offset: 0x001E2160
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
			this.spreadFrames = null;
		}

		// Token: 0x06005F46 RID: 24390 RVA: 0x001E3FCC File Offset: 0x001E21CC
		public override void OnEnter()
		{
			this.spawnPosition = Vector3.zero;
			this.spawnRotation = Vector3.zero;
			if (this.SpawnPoint.Value != null)
			{
				this.spawnPosition = this.SpawnPoint.Value.transform.position;
				if (!this.Position.IsNone)
				{
					this.spawnPosition += this.Position.Value;
				}
			}
			else if (!this.Position.IsNone)
			{
				this.spawnPosition = this.Position.Value;
			}
			this.gameObjects.Clear();
			foreach (GameObject gameObject in this.GameObjects.Values)
			{
				if (!(gameObject == null))
				{
					this.gameObjects.Add(gameObject);
				}
			}
			this.spawns = Random.Range(this.SpawnMin.Value, this.SpawnMax.Value + 1);
			if (this.spawns > 0)
			{
				int value = this.spreadFrames.Value;
				if (value <= 1)
				{
					this.DoSpawn(this.spawns);
					this.spawns = 0;
				}
				else
				{
					this.spawnRate = Mathf.CeilToInt((float)this.spawns / (float)value);
				}
			}
			if (this.spawns <= 0)
			{
				base.Finish();
			}
		}

		// Token: 0x06005F47 RID: 24391 RVA: 0x001E411E File Offset: 0x001E231E
		public override void OnUpdate()
		{
			this.DoStaggeredSpawn();
			if (this.spawns <= 0)
			{
				base.Finish();
			}
		}

		// Token: 0x06005F48 RID: 24392 RVA: 0x001E4138 File Offset: 0x001E2338
		private void DoSpawn(int spawns)
		{
			if (spawns > 0)
			{
				this.spawns -= spawns;
				float value = this.SpeedMin.Value;
				float value2 = this.SpeedMax.Value;
				float value3 = this.AngleMin.Value;
				float value4 = this.AngleMax.Value;
				bool flag = this.OriginVariationX != null;
				float num = flag ? this.OriginVariationX.Value : 0f;
				bool flag2 = this.OriginVariationY != null;
				float num2 = flag2 ? this.OriginVariationY.Value : 0f;
				for (int i = 1; i <= spawns; i++)
				{
					int index = (this.gameObjects.Count > 1) ? Random.Range(0, this.gameObjects.Count) : 0;
					GameObject prefab = this.gameObjects[index];
					float x = 0f;
					float y = 0f;
					if (flag)
					{
						x = Random.Range(-num, num);
						this.originAdjusted = true;
					}
					if (flag2)
					{
						y = Random.Range(-num2, num2);
						this.originAdjusted = true;
					}
					Vector3 vector = this.spawnPosition;
					if (this.originAdjusted)
					{
						vector += new Vector3(x, y, 0f);
					}
					GameObject gameObject = prefab.Spawn(vector, Quaternion.Euler(this.spawnRotation));
					BlackThreadState.HandleDamagerSpawn(base.Owner, gameObject);
					this.rb2d = gameObject.GetComponent<Rigidbody2D>();
					float num3 = Random.Range(value, value2);
					float num4 = Random.Range(value3, value4);
					this.vectorX = num3 * Mathf.Cos(num4 * 0.017453292f);
					this.vectorY = num3 * Mathf.Sin(num4 * 0.017453292f);
					Vector2 linearVelocity;
					linearVelocity.x = this.vectorX;
					linearVelocity.y = this.vectorY;
					this.rb2d.linearVelocity = linearVelocity;
				}
			}
		}

		// Token: 0x06005F49 RID: 24393 RVA: 0x001E430D File Offset: 0x001E250D
		private void DoStaggeredSpawn()
		{
			if (this.spawnRate > 0)
			{
				this.DoSpawn(Mathf.Min(this.spawnRate, this.spawns));
			}
		}

		// Token: 0x04005C76 RID: 23670
		[RequiredField]
		[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
		public FsmArray GameObjects;

		// Token: 0x04005C77 RID: 23671
		public FsmGameObject SpawnPoint;

		// Token: 0x04005C78 RID: 23672
		public FsmVector3 Position;

		// Token: 0x04005C79 RID: 23673
		public FsmInt SpawnMin;

		// Token: 0x04005C7A RID: 23674
		public FsmInt SpawnMax;

		// Token: 0x04005C7B RID: 23675
		public FsmFloat SpeedMin;

		// Token: 0x04005C7C RID: 23676
		public FsmFloat SpeedMax;

		// Token: 0x04005C7D RID: 23677
		public FsmFloat AngleMin;

		// Token: 0x04005C7E RID: 23678
		public FsmFloat AngleMax;

		// Token: 0x04005C7F RID: 23679
		public FsmFloat OriginVariationX;

		// Token: 0x04005C80 RID: 23680
		public FsmFloat OriginVariationY;

		// Token: 0x04005C81 RID: 23681
		public FsmInt spreadFrames;

		// Token: 0x04005C82 RID: 23682
		private float vectorX;

		// Token: 0x04005C83 RID: 23683
		private float vectorY;

		// Token: 0x04005C84 RID: 23684
		private bool originAdjusted;

		// Token: 0x04005C85 RID: 23685
		private List<GameObject> gameObjects = new List<GameObject>();

		// Token: 0x04005C86 RID: 23686
		private int spawns;

		// Token: 0x04005C87 RID: 23687
		private int spawnRate;

		// Token: 0x04005C88 RID: 23688
		private Vector3 spawnPosition;

		// Token: 0x04005C89 RID: 23689
		private Vector3 spawnRotation;
	}
}
