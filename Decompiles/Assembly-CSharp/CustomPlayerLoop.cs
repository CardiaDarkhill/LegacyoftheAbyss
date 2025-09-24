using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

// Token: 0x020001A2 RID: 418
public static class CustomPlayerLoop
{
	// Token: 0x17000195 RID: 405
	// (get) Token: 0x06001039 RID: 4153 RVA: 0x0004E4D9 File Offset: 0x0004C6D9
	// (set) Token: 0x0600103A RID: 4154 RVA: 0x0004E4E0 File Offset: 0x0004C6E0
	public static int FixedUpdateCycle { get; private set; }

	// Token: 0x0600103B RID: 4155 RVA: 0x0004E4E8 File Offset: 0x0004C6E8
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void SetupCustomPlayerLoop()
	{
		PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
		int num;
		PlayerLoopSystem directSubSystem = CustomPlayerLoop.GetDirectSubSystem<FixedUpdate>(currentPlayerLoop, out num);
		List<PlayerLoopSystem> list = directSubSystem.subSystemList.ToList<PlayerLoopSystem>();
		list.Add(CustomPlayerLoop.LateFixedUpdate.Create(CustomPlayerLoop._lateFixedUpdateList, CustomPlayerLoop._superLateFixedUpdateList));
		directSubSystem.subSystemList = list.ToArray();
		currentPlayerLoop.subSystemList[num] = directSubSystem;
		PlayerLoop.SetPlayerLoop(currentPlayerLoop);
	}

	// Token: 0x0600103C RID: 4156 RVA: 0x0004E544 File Offset: 0x0004C744
	private static PlayerLoopSystem GetDirectSubSystem<T>(PlayerLoopSystem def, out int index)
	{
		index = -1;
		if (def.subSystemList == null)
		{
			return default(PlayerLoopSystem);
		}
		for (int i = 0; i < def.subSystemList.Length; i++)
		{
			PlayerLoopSystem playerLoopSystem = def.subSystemList[i];
			if (!(playerLoopSystem.type != typeof(T)))
			{
				index = i;
				return playerLoopSystem;
			}
		}
		return default(PlayerLoopSystem);
	}

	// Token: 0x0600103D RID: 4157 RVA: 0x0004E5AB File Offset: 0x0004C7AB
	public static void RegisterLateFixedUpdate(CustomPlayerLoop.ILateFixedUpdate obj)
	{
		CustomPlayerLoop._lateFixedUpdateList.Add(obj);
	}

	// Token: 0x0600103E RID: 4158 RVA: 0x0004E5B8 File Offset: 0x0004C7B8
	public static void UnregisterLateFixedUpdate(CustomPlayerLoop.ILateFixedUpdate obj)
	{
		CustomPlayerLoop._lateFixedUpdateList.Remove(obj);
	}

	// Token: 0x0600103F RID: 4159 RVA: 0x0004E5C6 File Offset: 0x0004C7C6
	public static void RegisterSuperLateFixedUpdate(CustomPlayerLoop.ILateFixedUpdate obj)
	{
		CustomPlayerLoop._superLateFixedUpdateList.Add(obj);
	}

	// Token: 0x06001040 RID: 4160 RVA: 0x0004E5D3 File Offset: 0x0004C7D3
	public static void UnregisterSuperLateFixedUpdate(CustomPlayerLoop.ILateFixedUpdate obj)
	{
		CustomPlayerLoop._superLateFixedUpdateList.Remove(obj);
	}

	// Token: 0x04000FC0 RID: 4032
	private static readonly List<CustomPlayerLoop.ILateFixedUpdate> _lateFixedUpdateList = new List<CustomPlayerLoop.ILateFixedUpdate>();

	// Token: 0x04000FC1 RID: 4033
	private static readonly List<CustomPlayerLoop.ILateFixedUpdate> _superLateFixedUpdateList = new List<CustomPlayerLoop.ILateFixedUpdate>();

	// Token: 0x020014E0 RID: 5344
	public interface ILateFixedUpdate
	{
		// Token: 0x17000D3A RID: 3386
		// (get) Token: 0x06008520 RID: 34080
		bool isActiveAndEnabled { get; }

		// Token: 0x06008521 RID: 34081
		void LateFixedUpdate();
	}

	// Token: 0x020014E1 RID: 5345
	private struct LateFixedUpdate
	{
		// Token: 0x06008522 RID: 34082 RVA: 0x0026EE30 File Offset: 0x0026D030
		public static PlayerLoopSystem Create(List<CustomPlayerLoop.ILateFixedUpdate> updateList, List<CustomPlayerLoop.ILateFixedUpdate> laterUpdateList)
		{
			return new PlayerLoopSystem
			{
				type = typeof(CustomPlayerLoop.LateFixedUpdate),
				updateDelegate = delegate()
				{
					CustomPlayerLoop.LateFixedUpdate.Update(updateList);
					CustomPlayerLoop.LateFixedUpdate.Update(laterUpdateList);
					CustomPlayerLoop.FixedUpdateCycle++;
				}
			};
		}

		// Token: 0x06008523 RID: 34083 RVA: 0x0026EE80 File Offset: 0x0026D080
		private static void Update(List<CustomPlayerLoop.ILateFixedUpdate> list)
		{
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				CustomPlayerLoop.ILateFixedUpdate lateFixedUpdate = list[i];
				if (lateFixedUpdate != null)
				{
					if (lateFixedUpdate.isActiveAndEnabled)
					{
						lateFixedUpdate.LateFixedUpdate();
					}
					list[num++] = lateFixedUpdate;
				}
			}
			if (num < list.Count)
			{
				list.RemoveRange(num, list.Count - num);
			}
		}
	}
}
