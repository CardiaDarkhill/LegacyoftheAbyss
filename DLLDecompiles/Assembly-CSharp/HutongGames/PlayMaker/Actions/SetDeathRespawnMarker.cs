using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001202 RID: 4610
	public class SetDeathRespawnMarker : FSMUtility.GetComponentFsmStateAction<RespawnMarker>
	{
		// Token: 0x06007AAA RID: 31402 RVA: 0x0024D020 File Offset: 0x0024B220
		protected override void DoAction(RespawnMarker marker)
		{
			HeroController instance = HeroController.instance;
			GameManager instance2 = GameManager.instance;
			bool flag = marker.GetComponent<RestBench>();
			instance.SetBenchRespawn(marker, instance2.GetSceneNameString(), flag ? 1 : 0);
		}
	}
}
