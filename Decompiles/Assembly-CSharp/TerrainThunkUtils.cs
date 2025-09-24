using System;
using GlobalSettings;
using UnityEngine;

// Token: 0x0200078F RID: 1935
public static class TerrainThunkUtils
{
	// Token: 0x06004489 RID: 17545 RVA: 0x0012C1EF File Offset: 0x0012A3EF
	public static Vector2 GenerateTerrainThunk(Collision2D collision, ContactPoint2D[] storeContacts, TerrainThunkUtils.SlashDirection slashDirection, Vector2 recoilOrigin, out int recoilDir, out int surfaceDir, TerrainThunkUtils.TerrainThunkConditionDelegate conditionDelegate = null)
	{
		return TerrainThunkUtils.GenerateTerrainThunk(collision.GetContacts(storeContacts), storeContacts, slashDirection, recoilOrigin, out recoilDir, out surfaceDir, conditionDelegate);
	}

	// Token: 0x0600448A RID: 17546 RVA: 0x0012C208 File Offset: 0x0012A408
	public static Vector2 GenerateTerrainThunk(int contactCount, ContactPoint2D[] storeContacts, TerrainThunkUtils.SlashDirection slashDirection, Vector2 recoilOrigin, out int recoilDir, out int surfaceDir, TerrainThunkUtils.TerrainThunkConditionDelegate conditionDelegate = null)
	{
		recoilDir = -1;
		surfaceDir = -1;
		GameObject enemyNailTerrainThunk = Effects.EnemyNailTerrainThunk;
		if (!enemyNailTerrainThunk)
		{
			Debug.LogError("No terrain thunk prefab to spawn!");
			return Vector2.zero;
		}
		bool flag = false;
		float num = float.MaxValue;
		Vector2 vector = Vector2.zero;
		bool flag2 = false;
		for (int i = 0; i < contactCount; i++)
		{
			ContactPoint2D contactPoint2D = storeContacts[i];
			Collider2D collider = contactPoint2D.collider;
			if (!collider.isTrigger)
			{
				Vector2 point = contactPoint2D.point;
				Vector2 vector2 = point - recoilOrigin;
				switch (slashDirection)
				{
				case TerrainThunkUtils.SlashDirection.Left:
					if (vector2.x > 0f)
					{
						goto IL_233;
					}
					break;
				case TerrainThunkUtils.SlashDirection.Right:
					if (vector2.x < 0f)
					{
						goto IL_233;
					}
					break;
				case TerrainThunkUtils.SlashDirection.Up:
					if (vector2.y < 0f)
					{
						goto IL_233;
					}
					break;
				case TerrainThunkUtils.SlashDirection.Down:
					if (vector2.y > 0f)
					{
						goto IL_233;
					}
					break;
				}
				bool flag3 = true;
				bool flag4;
				TerrainThunkUtils.GetThunkProperties(collider.gameObject, out flag4, ref flag3);
				Vector2 vector3 = recoilOrigin;
				if (slashDirection - TerrainThunkUtils.SlashDirection.Left > 1)
				{
					if (slashDirection - TerrainThunkUtils.SlashDirection.Up <= 1)
					{
						vector3.x = point.x;
					}
				}
				else
				{
					vector3.y = point.y;
				}
				Vector2 vector4 = point - vector3;
				Vector2 normalized = vector4.normalized;
				float distance = vector4.magnitude + 1f;
				RaycastHit2D raycastHit2D = Helper.Raycast2D(vector3, normalized, distance, 1 << collider.gameObject.layer);
				if (!(raycastHit2D.collider == null))
				{
					float num2 = Vector2.Distance(raycastHit2D.point, recoilOrigin);
					if (num2 <= num)
					{
						num = num2;
						flag = true;
						vector = raycastHit2D.point;
						if (flag4)
						{
							flag2 = true;
						}
						Vector2 normal = raycastHit2D.normal;
						float degrees = 57.29578f * Mathf.Atan2(normal.y, normal.x);
						surfaceDir = DirectionUtils.GetCardinalDirection(degrees);
						if (flag3)
						{
							if (slashDirection == TerrainThunkUtils.SlashDirection.None)
							{
								Vector2 vector5 = -vector2.normalized;
								float degrees2 = 57.29578f * Mathf.Atan2(vector5.y, vector5.x);
								recoilDir = DirectionUtils.GetCardinalDirection(degrees2);
							}
							else
							{
								switch (slashDirection)
								{
								case TerrainThunkUtils.SlashDirection.Left:
									recoilDir = 0;
									break;
								case TerrainThunkUtils.SlashDirection.Right:
									recoilDir = 2;
									break;
								case TerrainThunkUtils.SlashDirection.Up:
									recoilDir = 3;
									break;
								case TerrainThunkUtils.SlashDirection.Down:
									recoilDir = 1;
									break;
								}
							}
						}
					}
				}
			}
			IL_233:;
		}
		if (!flag)
		{
			return Vector2.zero;
		}
		if (conditionDelegate != null)
		{
			TerrainThunkUtils.TerrainThunkConditionArgs args = new TerrainThunkUtils.TerrainThunkConditionArgs
			{
				RecoilDirection = recoilDir,
				ThunkPos = vector
			};
			if (!conditionDelegate(args))
			{
				return Vector2.zero;
			}
		}
		if (flag2)
		{
			enemyNailTerrainThunk.Spawn(vector, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
		}
		return vector;
	}

	// Token: 0x0600448B RID: 17547 RVA: 0x0012C4C8 File Offset: 0x0012A6C8
	public static void GetThunkProperties(GameObject target, out bool shouldThunk, ref bool doRecoil)
	{
		if (target.GetComponent<TinkEffect>())
		{
			shouldThunk = false;
			doRecoil = false;
			return;
		}
		NonThunker component = target.GetComponent<NonThunker>();
		if (!(component != null))
		{
			shouldThunk = true;
			return;
		}
		if (component.active)
		{
			shouldThunk = false;
			doRecoil = component.doRecoil;
			return;
		}
		shouldThunk = true;
	}

	// Token: 0x02001A6A RID: 6762
	public enum SlashDirection
	{
		// Token: 0x04009965 RID: 39269
		None,
		// Token: 0x04009966 RID: 39270
		Left,
		// Token: 0x04009967 RID: 39271
		Right,
		// Token: 0x04009968 RID: 39272
		Up,
		// Token: 0x04009969 RID: 39273
		Down
	}

	// Token: 0x02001A6B RID: 6763
	public struct TerrainThunkConditionArgs
	{
		// Token: 0x0400996A RID: 39274
		public int RecoilDirection;

		// Token: 0x0400996B RID: 39275
		public Vector2 ThunkPos;
	}

	// Token: 0x02001A6C RID: 6764
	// (Invoke) Token: 0x060096D5 RID: 38613
	public delegate bool TerrainThunkConditionDelegate(TerrainThunkUtils.TerrainThunkConditionArgs args);
}
