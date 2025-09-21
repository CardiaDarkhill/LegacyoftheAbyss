using System;
using UnityEngine;

// Token: 0x020007BB RID: 1979
public class HeroCorpseMarkerProxy : MonoBehaviour
{
	// Token: 0x170007D8 RID: 2008
	// (get) Token: 0x060045C4 RID: 17860 RVA: 0x0012F9A1 File Offset: 0x0012DBA1
	public byte[] TargetGuid
	{
		get
		{
			return this.targetGuid;
		}
	}

	// Token: 0x170007D9 RID: 2009
	// (get) Token: 0x060045C5 RID: 17861 RVA: 0x0012F9A9 File Offset: 0x0012DBA9
	public string TargetSceneName
	{
		get
		{
			return FastTravelScenes.GetSceneName(PlayerData.instance.FastTravelNPCLocation);
		}
	}

	// Token: 0x170007DA RID: 2010
	// (get) Token: 0x060045C6 RID: 17862 RVA: 0x0012F9BA File Offset: 0x0012DBBA
	public Vector2 TargetScenePos
	{
		get
		{
			return StaticVariableList.GetValue<Vector2>(this.readScenePosFromStaticVar);
		}
	}

	// Token: 0x170007DB RID: 2011
	// (get) Token: 0x060045C7 RID: 17863 RVA: 0x0012F9C7 File Offset: 0x0012DBC7
	// (set) Token: 0x060045C8 RID: 17864 RVA: 0x0012F9CE File Offset: 0x0012DBCE
	public static HeroCorpseMarkerProxy Instance { get; private set; }

	// Token: 0x060045C9 RID: 17865 RVA: 0x0012F9D6 File Offset: 0x0012DBD6
	private void OnEnable()
	{
		HeroCorpseMarkerProxy.Instance = this;
	}

	// Token: 0x060045CA RID: 17866 RVA: 0x0012F9DE File Offset: 0x0012DBDE
	private void OnDisable()
	{
		if (HeroCorpseMarkerProxy.Instance == this)
		{
			HeroCorpseMarkerProxy.Instance = null;
		}
	}

	// Token: 0x04004676 RID: 18038
	[SerializeField]
	private byte[] targetGuid;

	// Token: 0x04004677 RID: 18039
	[SerializeField]
	private string readScenePosFromStaticVar;
}
