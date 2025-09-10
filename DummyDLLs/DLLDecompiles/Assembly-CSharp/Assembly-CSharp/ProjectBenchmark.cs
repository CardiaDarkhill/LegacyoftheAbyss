using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000482 RID: 1154
public static class ProjectBenchmark
{
	// Token: 0x170004F6 RID: 1270
	// (get) Token: 0x060029B1 RID: 10673 RVA: 0x000B585F File Offset: 0x000B3A5F
	// (set) Token: 0x060029B2 RID: 10674 RVA: 0x000B5866 File Offset: 0x000B3A66
	public static bool IsRunning { get; private set; }

	// Token: 0x060029B3 RID: 10675 RVA: 0x000B586E File Offset: 0x000B3A6E
	public static void RunBenchmark(MonoBehaviour runner)
	{
		runner.StartCoroutine(ProjectBenchmark.BenchmarkRoutine(runner));
	}

	// Token: 0x060029B4 RID: 10676 RVA: 0x000B587D File Offset: 0x000B3A7D
	private static IEnumerator BenchmarkRoutine(MonoBehaviour runner)
	{
		HeroController hc = HeroController.instance;
		if (!hc)
		{
			yield break;
		}
		GameManager gm = GameManager.instance;
		HeroActions ia = ManagerSingleton<InputHandler>.Instance.inputActions;
		hc.AddInputBlocker(runner);
		ProjectBenchmark.IsRunning = true;
		CheatManager.InvincibilityStates initialInvincibility = CheatManager.Invincibility;
		CheatManager.Invincibility = CheatManager.InvincibilityStates.FullInvincible;
		try
		{
			Dictionary<string, SceneTeleportMap.SceneInfo> teleportMap = SceneTeleportMap.GetTeleportMap();
			foreach (KeyValuePair<string, SceneTeleportMap.SceneInfo> keyValuePair in teleportMap)
			{
				if (!keyValuePair.Key.IsAny(WorldInfo.MenuScenes) && !keyValuePair.Key.IsAny(WorldInfo.NonGameplayScenes) && !(GameManager.GetBaseSceneName(keyValuePair.Key) != keyValuePair.Key))
				{
					string text = keyValuePair.Value.TransitionGates.FirstOrDefault((string g) => g.StartsWith("left") || g.StartsWith("right") == g.StartsWith("door"));
					if (string.IsNullOrEmpty(text))
					{
						text = keyValuePair.Value.TransitionGates.FirstOrDefault<string>();
						if (string.IsNullOrEmpty(text))
						{
							continue;
						}
					}
					EventRegister.SendEvent(EventRegisterEvents.FsmCancel, null);
					hc.AffectedByGravity(false);
					gm.BeginSceneTransition(new GameManager.SceneLoadInfo
					{
						SceneName = keyValuePair.Key,
						EntryGateName = text,
						AlwaysUnloadUnusedAssets = true,
						EntrySkip = true,
						PreventCameraFadeOut = true
					});
					yield return null;
					while (gm.IsInSceneTransition || !hc.isHeroInPosition || hc.cState.transitioning)
					{
						yield return null;
					}
					yield return new WaitForSeconds(0.5f);
					if (ia.MenuCancel.IsPressed)
					{
						break;
					}
				}
			}
			Dictionary<string, SceneTeleportMap.SceneInfo>.Enumerator enumerator = default(Dictionary<string, SceneTeleportMap.SceneInfo>.Enumerator);
		}
		finally
		{
			ProjectBenchmark.IsRunning = false;
			hc.RemoveInputBlocker(runner);
			CheatManager.Invincibility = initialInvincibility;
		}
		yield break;
		yield break;
	}
}
