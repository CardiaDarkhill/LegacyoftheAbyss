using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200018E RID: 398
public class HeroCorpseMarker : MonoBehaviour
{
	// Token: 0x17000181 RID: 385
	// (get) Token: 0x06000F7A RID: 3962 RVA: 0x0004AB5E File Offset: 0x00048D5E
	public Vector2 Position
	{
		get
		{
			return base.transform.position + this.cocoonOffset;
		}
	}

	// Token: 0x17000182 RID: 386
	// (get) Token: 0x06000F7B RID: 3963 RVA: 0x0004AB7B File Offset: 0x00048D7B
	public Guid Guid
	{
		get
		{
			if (!this.guidComponent)
			{
				return Guid.Empty;
			}
			return this.guidComponent.GetGuid();
		}
	}

	// Token: 0x06000F7C RID: 3964 RVA: 0x0004AB9C File Offset: 0x00048D9C
	private void OnDrawGizmos()
	{
		if (this.cocoonOffset.magnitude < 0.01f)
		{
			return;
		}
		Gizmos.color = Color.magenta;
		Vector2 vector = base.transform.position;
		Vector2 v = vector + this.cocoonOffset;
		Gizmos.DrawLine(vector, v);
		Gizmos.DrawWireSphere(v, 0.5f);
	}

	// Token: 0x06000F7D RID: 3965 RVA: 0x0004AC03 File Offset: 0x00048E03
	private void OnEnable()
	{
		HeroCorpseMarker._activeMarkers.AddIfNotPresent(this);
	}

	// Token: 0x06000F7E RID: 3966 RVA: 0x0004AC11 File Offset: 0x00048E11
	private void OnDisable()
	{
		HeroCorpseMarker._activeMarkers.Remove(this);
	}

	// Token: 0x06000F7F RID: 3967 RVA: 0x0004AC20 File Offset: 0x00048E20
	public static HeroCorpseMarker GetClosest(Vector2 toPosition)
	{
		HeroCorpseMarker result = null;
		float num = float.MaxValue;
		foreach (HeroCorpseMarker heroCorpseMarker in HeroCorpseMarker._activeMarkers)
		{
			float num2 = Vector2.Distance(heroCorpseMarker.transform.position, toPosition);
			if (num2 < num)
			{
				num = num2;
				result = heroCorpseMarker;
			}
		}
		return result;
	}

	// Token: 0x06000F80 RID: 3968 RVA: 0x0004AC98 File Offset: 0x00048E98
	public static HeroCorpseMarker GetByGuid(byte[] guid)
	{
		if (guid == null || guid.Length == 0)
		{
			return null;
		}
		return HeroCorpseMarker.GetByGuid(new Guid(guid));
	}

	// Token: 0x06000F81 RID: 3969 RVA: 0x0004ACB0 File Offset: 0x00048EB0
	public static HeroCorpseMarker GetByGuid(Guid guid)
	{
		if (guid == Guid.Empty)
		{
			return null;
		}
		foreach (HeroCorpseMarker heroCorpseMarker in HeroCorpseMarker._activeMarkers)
		{
			if (heroCorpseMarker.guidComponent && heroCorpseMarker.guidComponent.GetGuid() == guid)
			{
				return heroCorpseMarker;
			}
		}
		return null;
	}

	// Token: 0x06000F82 RID: 3970 RVA: 0x0004AD34 File Offset: 0x00048F34
	public static HeroCorpseMarker GetRandom()
	{
		if (HeroCorpseMarker._activeMarkers.Count != 0)
		{
			return HeroCorpseMarker._activeMarkers[Random.Range(0, HeroCorpseMarker._activeMarkers.Count)];
		}
		return null;
	}

	// Token: 0x04000F1D RID: 3869
	[SerializeField]
	private GuidComponent guidComponent;

	// Token: 0x04000F1E RID: 3870
	[SerializeField]
	[FormerlySerializedAs("corpseOffset")]
	private Vector2 cocoonOffset;

	// Token: 0x04000F1F RID: 3871
	private static readonly List<HeroCorpseMarker> _activeMarkers = new List<HeroCorpseMarker>();
}
