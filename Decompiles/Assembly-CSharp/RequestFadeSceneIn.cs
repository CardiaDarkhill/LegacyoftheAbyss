using System;
using System.Collections;
using GlobalEnums;
using UnityEngine;

// Token: 0x0200016E RID: 366
public class RequestFadeSceneIn : MonoBehaviour
{
	// Token: 0x06000BB4 RID: 2996 RVA: 0x000355F7 File Offset: 0x000337F7
	private IEnumerator Start()
	{
		yield return new WaitForSeconds(this.waitBeforeFade);
		if (this.fadeInSpeed == CameraFadeInType.SLOW)
		{
			GameCameras.instance.cameraFadeFSM.SendEventSafe("FADE SCENE IN SLOWLY");
		}
		else if (this.fadeInSpeed == CameraFadeInType.NORMAL)
		{
			GameCameras.instance.cameraFadeFSM.SendEventSafe("FADE SCENE IN");
		}
		else if (this.fadeInSpeed == CameraFadeInType.INSTANT)
		{
			GameCameras.instance.cameraFadeFSM.SendEventSafe("FADE SCENE IN INSTANT");
		}
		yield break;
	}

	// Token: 0x04000B49 RID: 2889
	public float waitBeforeFade;

	// Token: 0x04000B4A RID: 2890
	public CameraFadeInType fadeInSpeed;
}
