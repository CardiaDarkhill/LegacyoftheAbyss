using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003E6 RID: 998
public class MemoryOrbSource : MonoBehaviour
{
	// Token: 0x1700038C RID: 908
	// (get) Token: 0x06002230 RID: 8752 RVA: 0x0009DACA File Offset: 0x0009BCCA
	protected virtual bool IsActive
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002231 RID: 8753 RVA: 0x0009DAD0 File Offset: 0x0009BCD0
	private void Awake()
	{
		if (this.needolinScreenEdgePrefab)
		{
			PersonalObjectPool.EnsurePooledInScene(base.gameObject, this.needolinScreenEdgePrefab, 2, false, false, false);
		}
		if (this.needolinOnScreenPrefab)
		{
			PersonalObjectPool.EnsurePooledInScene(base.gameObject, this.needolinOnScreenPrefab, 2, false, false, false);
		}
		PersonalObjectPool.EnsurePooledInSceneFinished(base.gameObject);
	}

	// Token: 0x06002232 RID: 8754 RVA: 0x0009DB2C File Offset: 0x0009BD2C
	private void OnEnable()
	{
		HeroPerformanceRegion.StartedPerforming += this.OnNeedolinStart;
		HeroPerformanceRegion.StoppedPerforming += this.OnNeedolinStop;
	}

	// Token: 0x06002233 RID: 8755 RVA: 0x0009DB50 File Offset: 0x0009BD50
	private void OnDisable()
	{
		this.OnNeedolinStop();
		HeroPerformanceRegion.StartedPerforming -= this.OnNeedolinStart;
		HeroPerformanceRegion.StoppedPerforming -= this.OnNeedolinStop;
	}

	// Token: 0x06002234 RID: 8756 RVA: 0x0009DB7C File Offset: 0x0009BD7C
	private void OnNeedolinStart()
	{
		if (this.spawnedEffect)
		{
			return;
		}
		if (NeedolinMsgBox.IsBlocked)
		{
			return;
		}
		if (!this.IsActive)
		{
			return;
		}
		this.spawnedEffect = MemoryOrbSource.SpawnScreenEdgeEffect(this.needolinScreenEdgePrefab, this.needolinOnScreenPrefab, base.transform.position, this.screenEdgePadding);
	}

	// Token: 0x06002235 RID: 8757 RVA: 0x0009DBD8 File Offset: 0x0009BDD8
	private void OnNeedolinStop()
	{
		if (!this.spawnedEffect)
		{
			return;
		}
		if (this.temp == null)
		{
			this.temp = new List<ParticleSystem>();
		}
		this.spawnedEffect.GetComponentsInChildren<ParticleSystem>(this.temp);
		foreach (ParticleSystem particleSystem in this.temp)
		{
			particleSystem.Stop(true);
		}
		if (this.temp.Count == 0)
		{
			this.spawnedEffect.Recycle();
		}
		this.temp.Clear();
		this.spawnedEffect = null;
	}

	// Token: 0x06002236 RID: 8758 RVA: 0x0009DC88 File Offset: 0x0009BE88
	public static GameObject SpawnScreenEdgeEffect(GameObject edgePrefab, GameObject onScreenPrefab, Vector2 orbPos, float screenEdgePadding)
	{
		Transform transform = GameCameras.instance.mainCamera.transform;
		Vector2 vector = transform.position;
		Vector2 vector2 = new Vector2(8.3f * ForceCameraAspect.CurrentViewportAspect, 8.3f);
		Vector2 vector3 = vector - vector2;
		Vector2 vector4 = vector + vector2;
		Vector2 normalized = (orbPos - vector).normalized;
		if (orbPos.x >= vector3.x && orbPos.x <= vector4.x && orbPos.y >= vector3.y && orbPos.y <= vector4.y)
		{
			if (!onScreenPrefab)
			{
				return null;
			}
			return onScreenPrefab.Spawn(orbPos.ToVector3(onScreenPrefab.transform.localScale.z), Quaternion.Euler(0f, 0f, normalized.DirectionToAngle()));
		}
		else
		{
			Vector2 vector5 = vector + normalized * vector2.x;
			if (vector5.x < vector3.x)
			{
				vector5.x = vector3.x;
			}
			else if (vector5.x > vector4.x)
			{
				vector5.x = vector4.x;
			}
			if (vector5.y < vector3.y)
			{
				vector5.y = vector3.y;
			}
			else if (vector5.y > vector4.y)
			{
				vector5.y = vector4.y;
			}
			vector5 += normalized * screenEdgePadding;
			if (!edgePrefab)
			{
				return null;
			}
			GameObject gameObject = edgePrefab.Spawn(vector5.ToVector3(edgePrefab.transform.localScale.z), Quaternion.Euler(0f, 0f, normalized.DirectionToAngle()));
			gameObject.transform.SetParent(transform);
			return gameObject;
		}
	}

	// Token: 0x04002103 RID: 8451
	[SerializeField]
	private GameObject needolinScreenEdgePrefab;

	// Token: 0x04002104 RID: 8452
	[SerializeField]
	private GameObject needolinOnScreenPrefab;

	// Token: 0x04002105 RID: 8453
	[SerializeField]
	private float screenEdgePadding;

	// Token: 0x04002106 RID: 8454
	private GameObject spawnedEffect;

	// Token: 0x04002107 RID: 8455
	private List<ParticleSystem> temp;
}
