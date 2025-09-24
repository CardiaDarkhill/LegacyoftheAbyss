using System;
using UnityEngine;

// Token: 0x02000195 RID: 405
public class InGameCutsceneInfo : MonoBehaviour
{
	// Token: 0x17000186 RID: 390
	// (get) Token: 0x06000FA0 RID: 4000 RVA: 0x0004B560 File Offset: 0x00049760
	public static bool IsInCutscene
	{
		get
		{
			if (InGameCutsceneInfo._instance == null)
			{
				return false;
			}
			GameManager instance = GameManager.instance;
			return !instance || instance.GetSceneNameString() == InGameCutsceneInfo._instance.gameObject.scene.name;
		}
	}

	// Token: 0x17000187 RID: 391
	// (get) Token: 0x06000FA1 RID: 4001 RVA: 0x0004B5AE File Offset: 0x000497AE
	public static Vector2 CameraPosition
	{
		get
		{
			if (!(InGameCutsceneInfo._instance != null))
			{
				return Vector2.zero;
			}
			return InGameCutsceneInfo._instance.cameraPosition;
		}
	}

	// Token: 0x06000FA2 RID: 4002 RVA: 0x0004B5CD File Offset: 0x000497CD
	private void Awake()
	{
		InGameCutsceneInfo._instance = this;
	}

	// Token: 0x06000FA3 RID: 4003 RVA: 0x0004B5D5 File Offset: 0x000497D5
	private void OnDestroy()
	{
		if (InGameCutsceneInfo._instance == this)
		{
			InGameCutsceneInfo._instance = null;
		}
	}

	// Token: 0x04000F3B RID: 3899
	private static InGameCutsceneInfo _instance;

	// Token: 0x04000F3C RID: 3900
	[SerializeField]
	private Vector2 cameraPosition;
}
