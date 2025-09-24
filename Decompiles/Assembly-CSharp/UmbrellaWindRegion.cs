using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000EF RID: 239
public class UmbrellaWindRegion : WindRegion
{
	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x06000792 RID: 1938 RVA: 0x00024C54 File Offset: 0x00022E54
	public float SpeedX
	{
		get
		{
			return this.speedX * this.SpeedMultiplier;
		}
	}

	// Token: 0x06000793 RID: 1939 RVA: 0x00024C64 File Offset: 0x00022E64
	protected override void Awake()
	{
		base.Awake();
		GameObject gameObject = new GameObject("WindZone", new Type[]
		{
			typeof(WindZone)
		});
		Transform transform = gameObject.transform;
		transform.eulerAngles = new Vector3(0f, (float)((this.speedX > 0f) ? 90 : -90), 0f);
		transform.SetParent(base.transform, true);
		transform.localPosition = Vector3.zero;
		this.childWindZone = gameObject.GetComponent<WindZone>();
		this.childWindZone.mode = WindZoneMode.Directional;
		this.childWindZone.windMain = Mathf.Abs(this.speedX);
		this.childWindZone.gameObject.SetActive(false);
	}

	// Token: 0x06000794 RID: 1940 RVA: 0x00024D1A File Offset: 0x00022F1A
	protected override void OnEnable()
	{
		base.OnEnable();
		UmbrellaWindRegion._activeRegions.Add(this);
	}

	// Token: 0x06000795 RID: 1941 RVA: 0x00024D2D File Offset: 0x00022F2D
	protected override void OnDisable()
	{
		base.OnDisable();
		UmbrellaWindRegion._activeRegions.Remove(this);
	}

	// Token: 0x06000796 RID: 1942 RVA: 0x00024D41 File Offset: 0x00022F41
	protected override void OnInsideStateChanged(bool isInside)
	{
		base.OnInsideStateChanged(isInside);
		this.childWindZone.gameObject.SetActive(isInside);
	}

	// Token: 0x06000797 RID: 1943 RVA: 0x00024D5B File Offset: 0x00022F5B
	public static IEnumerable<UmbrellaWindRegion> EnumerateActiveRegions()
	{
		foreach (UmbrellaWindRegion umbrellaWindRegion in UmbrellaWindRegion._activeRegions)
		{
			yield return umbrellaWindRegion;
		}
		List<UmbrellaWindRegion>.Enumerator enumerator = default(List<UmbrellaWindRegion>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x04000764 RID: 1892
	[Space]
	[SerializeField]
	private float speedX;

	// Token: 0x04000765 RID: 1893
	public float SpeedMultiplier = 1f;

	// Token: 0x04000766 RID: 1894
	private WindZone childWindZone;

	// Token: 0x04000767 RID: 1895
	private static readonly List<UmbrellaWindRegion> _activeRegions = new List<UmbrellaWindRegion>();
}
