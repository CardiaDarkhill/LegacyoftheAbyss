using System;
using HutongGames.PlayMaker;

// Token: 0x02000271 RID: 625
public class SetSceneAmbientLight : FsmStateAction
{
	// Token: 0x06001658 RID: 5720 RVA: 0x000649A0 File Offset: 0x00062BA0
	public override void Reset()
	{
		this.Intensity = null;
	}

	// Token: 0x06001659 RID: 5721 RVA: 0x000649AC File Offset: 0x00062BAC
	public override void OnEnter()
	{
		SceneColorManager sceneColorManager = GameCameras.instance.sceneColorManager;
		if (sceneColorManager)
		{
			sceneColorManager.AmbientIntensityA = this.Intensity.Value;
		}
		base.Finish();
	}

	// Token: 0x040014CE RID: 5326
	public FsmFloat Intensity;
}
