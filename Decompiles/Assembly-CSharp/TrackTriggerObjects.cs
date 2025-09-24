using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020005F9 RID: 1529
public class TrackTriggerObjects : DebugDrawColliderRuntimeAdder
{
	// Token: 0x140000AB RID: 171
	// (add) Token: 0x06003686 RID: 13958 RVA: 0x000F08A8 File Offset: 0x000EEAA8
	// (remove) Token: 0x06003687 RID: 13959 RVA: 0x000F08E0 File Offset: 0x000EEAE0
	public event Action<bool> InsideStateChanged;

	// Token: 0x17000648 RID: 1608
	// (get) Token: 0x06003688 RID: 13960 RVA: 0x000F0918 File Offset: 0x000EEB18
	public int InsideCount
	{
		get
		{
			int num = 0;
			foreach (GameObject gameObject in this.insideGameObjects)
			{
				if (gameObject && this.IsCounted(gameObject))
				{
					num++;
				}
			}
			return num;
		}
	}

	// Token: 0x17000649 RID: 1609
	// (get) Token: 0x06003689 RID: 13961 RVA: 0x000F097C File Offset: 0x000EEB7C
	public bool IsInside
	{
		get
		{
			return this.InsideCount > 0;
		}
	}

	// Token: 0x1700064A RID: 1610
	// (get) Token: 0x0600368A RID: 13962 RVA: 0x000F0987 File Offset: 0x000EEB87
	public IEnumerable<GameObject> InsideGameObjects
	{
		get
		{
			if (!this.gottenOverlappedColliders)
			{
				this.GetOverlappedColliders(false);
			}
			return this.insideGameObjects;
		}
	}

	// Token: 0x1700064B RID: 1611
	// (get) Token: 0x0600368B RID: 13963 RVA: 0x000F099E File Offset: 0x000EEB9E
	public List<GameObject> insideObjectsList
	{
		get
		{
			return this.insideGameObjects;
		}
	}

	// Token: 0x1700064C RID: 1612
	// (get) Token: 0x0600368C RID: 13964 RVA: 0x000F09A6 File Offset: 0x000EEBA6
	protected virtual bool RequireEnabled
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600368D RID: 13965 RVA: 0x000F09AC File Offset: 0x000EEBAC
	protected virtual void OnEnable()
	{
		if (this.layerMask < 0)
		{
			this.layerMask = Helper.GetCollidingLayerMaskForLayer(base.gameObject.layer);
		}
		HeroController silentInstance = HeroController.SilentInstance;
		if (silentInstance)
		{
			silentInstance.heroInPosition += this.OnHeroInPosition;
			if (silentInstance.isHeroInPosition)
			{
				this.GetOverlappedColliders(false);
			}
		}
	}

	// Token: 0x0600368E RID: 13966 RVA: 0x000F0A08 File Offset: 0x000EEC08
	protected virtual void OnDisable()
	{
		HeroController silentInstance = HeroController.SilentInstance;
		if (silentInstance)
		{
			silentInstance.heroInPosition -= this.OnHeroInPosition;
		}
		foreach (GameObject obj in this.insideGameObjects)
		{
			this.ExitNotify(obj);
		}
		this.insideGameObjects.Clear();
		this.CallInsideStateChanged(false);
		this.gottenOverlappedColliders = false;
	}

	// Token: 0x0600368F RID: 13967 RVA: 0x000F0A94 File Offset: 0x000EEC94
	private void OnHeroInPosition(bool forceDirect)
	{
		if (!this)
		{
			Debug.LogError("TrackTriggerObjects native Object was destroyed! This should not happen...", this);
			return;
		}
		this.GetOverlappedColliders(true);
	}

	// Token: 0x06003690 RID: 13968 RVA: 0x000F0AB1 File Offset: 0x000EECB1
	protected void Refresh()
	{
		this.GetOverlappedColliders(true);
	}

	// Token: 0x06003691 RID: 13969 RVA: 0x000F0ABC File Offset: 0x000EECBC
	private void GetOverlappedColliders(bool isRefresh)
	{
		if (!base.enabled || !base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.gottenOverlappedColliders && !isRefresh)
		{
			return;
		}
		this.gottenOverlappedColliders = true;
		Collider2D[] array = base.GetComponents<Collider2D>();
		for (int i = 0; i < array.Length; i++)
		{
			int num = array[i].Overlap(new ContactFilter2D
			{
				useTriggers = true,
				useLayerMask = true,
				layerMask = this.layerMask
			}, TrackTriggerObjects._tempResults);
			if (num > 0)
			{
				for (int j = 0; j < Mathf.Min(num, TrackTriggerObjects._tempResults.Length); j++)
				{
					this.OnTriggerEnter2D(TrackTriggerObjects._tempResults[j]);
				}
			}
		}
		if (isRefresh)
		{
			TrackTriggerObjects._refreshTemp.AddRange(this.insideGameObjects);
			foreach (GameObject gameObject in TrackTriggerObjects._refreshTemp)
			{
				bool flag = false;
				foreach (Collider2D collider2D in TrackTriggerObjects._tempResults)
				{
					if (collider2D && collider2D.gameObject == gameObject)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this.OnExit(gameObject);
				}
			}
			TrackTriggerObjects._refreshTemp.Clear();
		}
		for (int k = 0; k < TrackTriggerObjects._tempResults.Length; k++)
		{
			TrackTriggerObjects._tempResults[k] = null;
		}
	}

	// Token: 0x06003692 RID: 13970 RVA: 0x000F0C34 File Offset: 0x000EEE34
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!this.gottenOverlappedColliders)
		{
			return;
		}
		if (this.RequireEnabled && !base.isActiveAndEnabled)
		{
			return;
		}
		GameObject gameObject = collision.gameObject;
		if (this.IsIgnored(gameObject))
		{
			return;
		}
		List<string> list = this.tagIncludeList;
		if (list != null && list.Count > 0)
		{
			bool flag = false;
			foreach (string tag in this.tagIncludeList)
			{
				if (gameObject.CompareTag(tag))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
		}
		list = this.tagExcludeList;
		if (list != null && list.Count > 0)
		{
			foreach (string tag2 in this.tagExcludeList)
			{
				if (gameObject.CompareTag(tag2))
				{
					return;
				}
			}
		}
		HeroController component = collision.GetComponent<HeroController>();
		if (component && component.cState.isTriggerEventsPaused)
		{
			return;
		}
		if (this.insideGameObjects.Contains(gameObject))
		{
			return;
		}
		this.insideGameObjects.Add(gameObject);
		ITrackTriggerObject component2 = gameObject.GetComponent<ITrackTriggerObject>();
		if (component2 != null)
		{
			component2.OnTrackTriggerEntered(this);
		}
		if (this.insideGameObjects.Count == 1)
		{
			this.CallInsideStateChanged(true);
		}
	}

	// Token: 0x06003693 RID: 13971 RVA: 0x000F0D90 File Offset: 0x000EEF90
	private void OnTriggerExit2D(Collider2D collision)
	{
		HeroController component = collision.GetComponent<HeroController>();
		if (component && component.cState.isTriggerEventsPaused)
		{
			return;
		}
		GameObject gameObject = collision.gameObject;
		this.OnExit(gameObject);
	}

	// Token: 0x06003694 RID: 13972 RVA: 0x000F0DC8 File Offset: 0x000EEFC8
	private void OnExit(GameObject obj)
	{
		if (this.insideGameObjects.Remove(obj))
		{
			this.ExitNotify(obj);
			if (this.insideGameObjects.Count == 0)
			{
				this.CallInsideStateChanged(false);
			}
		}
	}

	// Token: 0x06003695 RID: 13973 RVA: 0x000F0DF3 File Offset: 0x000EEFF3
	private void ExitNotify(GameObject obj)
	{
		if (!obj)
		{
			return;
		}
		ITrackTriggerObject component = obj.GetComponent<ITrackTriggerObject>();
		if (component == null)
		{
			return;
		}
		component.OnTrackTriggerExited(this);
	}

	// Token: 0x06003696 RID: 13974 RVA: 0x000F0E0F File Offset: 0x000EF00F
	private void CallInsideStateChanged(bool isInside)
	{
		Action<bool> insideStateChanged = this.InsideStateChanged;
		if (insideStateChanged != null)
		{
			insideStateChanged(isInside);
		}
		this.OnInsideStateChanged(isInside);
	}

	// Token: 0x06003697 RID: 13975 RVA: 0x000F0E2C File Offset: 0x000EF02C
	private bool IsIgnored(GameObject obj)
	{
		int layer = obj.layer;
		int num = 1 << layer;
		return (this.ignoreLayers.value & num) == num;
	}

	// Token: 0x06003698 RID: 13976 RVA: 0x000F0E57 File Offset: 0x000EF057
	protected virtual void OnInsideStateChanged(bool isInside)
	{
	}

	// Token: 0x06003699 RID: 13977 RVA: 0x000F0E5C File Offset: 0x000EF05C
	public GameObject GetClosestInside(Vector2 toPos, List<GameObject> excludeObjects)
	{
		float num = float.MaxValue;
		GameObject result = null;
		foreach (GameObject gameObject in this.InsideGameObjects)
		{
			if (!excludeObjects.Contains(gameObject))
			{
				float sqrMagnitude = (gameObject.transform.position - toPos).sqrMagnitude;
				if (sqrMagnitude <= num)
				{
					num = sqrMagnitude;
					result = gameObject;
				}
			}
		}
		return result;
	}

	// Token: 0x0600369A RID: 13978 RVA: 0x000F0EE0 File Offset: 0x000EF0E0
	public GameObject GetClosestInsideLineOfSight(Vector2 originPos, HashSet<GameObject> excludeObjects)
	{
		return this.GetClosestInsideLineOfSight(originPos, excludeObjects, PhysicsConstants.ALL_TERRAIN_LAYER);
	}

	// Token: 0x0600369B RID: 13979 RVA: 0x000F0EF0 File Offset: 0x000EF0F0
	public GameObject GetClosestInsideLineOfSight(Vector2 originPos, HashSet<GameObject> excludeObjects, int obstacleLayerMask)
	{
		float num = float.MaxValue;
		GameObject result = null;
		foreach (GameObject gameObject in this.InsideGameObjects)
		{
			if (!excludeObjects.Contains(gameObject))
			{
				Vector2 vector = gameObject.transform.position - originPos;
				float sqrMagnitude = vector.sqrMagnitude;
				if (sqrMagnitude < num)
				{
					float distance = Mathf.Sqrt(sqrMagnitude);
					RaycastHit2D raycastHit2D = Physics2D.Raycast(originPos, vector.normalized, distance, obstacleLayerMask);
					if (!(raycastHit2D.collider != null) || !(raycastHit2D.collider.gameObject != gameObject))
					{
						num = sqrMagnitude;
						result = gameObject;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x0600369C RID: 13980 RVA: 0x000F0FB4 File Offset: 0x000EF1B4
	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.Region, false);
	}

	// Token: 0x0600369D RID: 13981 RVA: 0x000F0FC3 File Offset: 0x000EF1C3
	protected virtual bool IsCounted(GameObject obj)
	{
		return true;
	}

	// Token: 0x04003965 RID: 14693
	[SerializeField]
	private LayerMask ignoreLayers;

	// Token: 0x04003966 RID: 14694
	[SerializeField]
	[TagSelector]
	[FormerlySerializedAs("tagFilter")]
	private List<string> tagIncludeList;

	// Token: 0x04003967 RID: 14695
	[SerializeField]
	[TagSelector]
	private List<string> tagExcludeList;

	// Token: 0x04003968 RID: 14696
	private List<GameObject> insideGameObjects = new List<GameObject>();

	// Token: 0x04003969 RID: 14697
	private int layerMask = -1;

	// Token: 0x0400396A RID: 14698
	private bool gottenOverlappedColliders;

	// Token: 0x0400396B RID: 14699
	private static readonly Collider2D[] _tempResults = new Collider2D[10];

	// Token: 0x0400396C RID: 14700
	private static readonly List<GameObject> _refreshTemp = new List<GameObject>();
}
