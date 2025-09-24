using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

// Token: 0x02000388 RID: 904
public static class BossSequenceController
{
	// Token: 0x17000310 RID: 784
	// (get) Token: 0x06001EB7 RID: 7863 RVA: 0x0008CB20 File Offset: 0x0008AD20
	public static bool BoundNail
	{
		get
		{
			return BossSequenceController.currentData != null && (BossSequenceController.currentData.bindings & BossSequenceController.ChallengeBindings.Nail) == BossSequenceController.ChallengeBindings.Nail;
		}
	}

	// Token: 0x17000311 RID: 785
	// (get) Token: 0x06001EB8 RID: 7864 RVA: 0x0008CB3A File Offset: 0x0008AD3A
	public static bool BoundShell
	{
		get
		{
			return BossSequenceController.currentData != null && (BossSequenceController.currentData.bindings & BossSequenceController.ChallengeBindings.Shell) == BossSequenceController.ChallengeBindings.Shell;
		}
	}

	// Token: 0x17000312 RID: 786
	// (get) Token: 0x06001EB9 RID: 7865 RVA: 0x0008CB54 File Offset: 0x0008AD54
	public static bool BoundCharms
	{
		get
		{
			return BossSequenceController.currentData != null && (BossSequenceController.currentData.bindings & BossSequenceController.ChallengeBindings.Charms) == BossSequenceController.ChallengeBindings.Charms;
		}
	}

	// Token: 0x17000313 RID: 787
	// (get) Token: 0x06001EBA RID: 7866 RVA: 0x0008CB6E File Offset: 0x0008AD6E
	public static bool BoundSoul
	{
		get
		{
			return BossSequenceController.currentData != null && (BossSequenceController.currentData.bindings & BossSequenceController.ChallengeBindings.Soul) == BossSequenceController.ChallengeBindings.Soul;
		}
	}

	// Token: 0x17000314 RID: 788
	// (get) Token: 0x06001EBB RID: 7867 RVA: 0x0008CB88 File Offset: 0x0008AD88
	// (set) Token: 0x06001EBC RID: 7868 RVA: 0x0008CB9D File Offset: 0x0008AD9D
	public static bool KnightDamaged
	{
		get
		{
			return BossSequenceController.currentData != null && BossSequenceController.currentData.knightDamaged;
		}
		set
		{
			if (BossSequenceController.currentData != null)
			{
				BossSequenceController.currentData.knightDamaged = value;
			}
		}
	}

	// Token: 0x17000315 RID: 789
	// (get) Token: 0x06001EBD RID: 7869 RVA: 0x0008CBB1 File Offset: 0x0008ADB1
	public static BossSequenceDoor.Completion PreviousCompletion
	{
		get
		{
			return BossSequenceController.currentData.previousCompletion;
		}
	}

	// Token: 0x17000316 RID: 790
	// (get) Token: 0x06001EBE RID: 7870 RVA: 0x0008CBBD File Offset: 0x0008ADBD
	// (set) Token: 0x06001EBF RID: 7871 RVA: 0x0008CBD6 File Offset: 0x0008ADD6
	public static float Timer
	{
		get
		{
			if (BossSequenceController.currentData == null)
			{
				return 0f;
			}
			return BossSequenceController.currentData.timer;
		}
		set
		{
			if (BossSequenceController.currentData != null)
			{
				BossSequenceController.currentData.timer = value;
			}
		}
	}

	// Token: 0x17000317 RID: 791
	// (get) Token: 0x06001EC0 RID: 7872 RVA: 0x0008CBEA File Offset: 0x0008ADEA
	// (set) Token: 0x06001EC1 RID: 7873 RVA: 0x0008CBF1 File Offset: 0x0008ADF1
	public static bool WasCompleted { get; private set; }

	// Token: 0x17000318 RID: 792
	// (get) Token: 0x06001EC2 RID: 7874 RVA: 0x0008CBF9 File Offset: 0x0008ADF9
	public static bool IsInSequence
	{
		get
		{
			return BossSequenceController.currentData != null && BossSequenceController.currentSequence != null;
		}
	}

	// Token: 0x17000319 RID: 793
	// (get) Token: 0x06001EC3 RID: 7875 RVA: 0x0008CC0F File Offset: 0x0008AE0F
	public static bool IsLastBossScene
	{
		get
		{
			return BossSequenceController.bossIndex >= BossSequenceController.currentSequence.Count - 1;
		}
	}

	// Token: 0x1700031A RID: 794
	// (get) Token: 0x06001EC4 RID: 7876 RVA: 0x0008CC27 File Offset: 0x0008AE27
	public static int BossIndex
	{
		get
		{
			return BossSequenceController.bossIndex;
		}
	}

	// Token: 0x1700031B RID: 795
	// (get) Token: 0x06001EC5 RID: 7877 RVA: 0x0008CC2E File Offset: 0x0008AE2E
	public static int BossCount
	{
		get
		{
			if (!BossSequenceController.currentSequence)
			{
				return 0;
			}
			return BossSequenceController.currentSequence.Count;
		}
	}

	// Token: 0x1700031C RID: 796
	// (get) Token: 0x06001EC6 RID: 7878 RVA: 0x0008CC48 File Offset: 0x0008AE48
	public static bool ShouldUnlockGGMode
	{
		get
		{
			if (GameManager.instance.GetStatusRecordInt("RecBossRushMode") <= 0)
			{
				int num = 0;
				foreach (FieldInfo fieldInfo in typeof(PlayerData).GetFields())
				{
					if (fieldInfo.FieldType == typeof(BossSequenceDoor.Completion) && ((BossSequenceDoor.Completion)fieldInfo.GetValue(GameManager.instance.playerData)).completed)
					{
						num++;
					}
				}
				return num >= 3;
			}
			return false;
		}
	}

	// Token: 0x1700031D RID: 797
	// (get) Token: 0x06001EC7 RID: 7879 RVA: 0x0008CCCC File Offset: 0x0008AECC
	public static int BoundMaxHealth
	{
		get
		{
			int num = 0;
			return BossSequenceController.currentSequence.maxHealth + num;
		}
	}

	// Token: 0x1700031E RID: 798
	// (get) Token: 0x06001EC8 RID: 7880 RVA: 0x0008CCE8 File Offset: 0x0008AEE8
	public static int BoundNailDamage
	{
		get
		{
			int nailDamage = GameManager.instance.playerData.nailDamage;
			if (nailDamage <= BossSequenceController.currentSequence.nailDamage)
			{
				return Mathf.RoundToInt((float)nailDamage * BossSequenceController.currentSequence.lowerNailDamagePercentage);
			}
			return BossSequenceController.currentSequence.nailDamage;
		}
	}

	// Token: 0x06001EC9 RID: 7881 RVA: 0x0008CD2F File Offset: 0x0008AF2F
	public static void Reset()
	{
		BossSequenceController.currentData = null;
		BossSequenceController.currentSequence = null;
		BossSequenceController.bossIndex = 0;
	}

	// Token: 0x06001ECA RID: 7882 RVA: 0x0008CD44 File Offset: 0x0008AF44
	public static void SetupNewSequence(BossSequence sequence, BossSequenceController.ChallengeBindings bindings, string playerData)
	{
		BossSequenceController.currentSequence = sequence;
		StaticVariableList.SetValue("currentBossDoorPD", playerData, 0);
		BossSequenceController.bossIndex = 0;
		BossSequenceController.currentData = new BossSequenceController.BossSequenceData
		{
			bindings = bindings,
			timer = 0f,
			playerData = playerData,
			bossSequenceName = BossSequenceController.currentSequence.name,
			previousCompletion = GameManager.instance.GetPlayerDataVariable<BossSequenceDoor.Completion>(playerData)
		};
		BossSequenceController.WasCompleted = false;
		GameManager.instance.playerData.currentBossSequence = null;
		BossSequenceController.SetupBossScene();
	}

	// Token: 0x06001ECB RID: 7883 RVA: 0x0008CDC8 File Offset: 0x0008AFC8
	public static void CheckLoadSequence(MonoBehaviour caller)
	{
		if (BossSequenceController.currentSequence == null)
		{
			BossSequenceController.LoadCurrentSequence(caller);
		}
	}

	// Token: 0x06001ECC RID: 7884 RVA: 0x0008CDE0 File Offset: 0x0008AFE0
	private static void LoadCurrentSequence(MonoBehaviour caller)
	{
		BossSequenceController.currentData = GameManager.instance.playerData.currentBossSequence;
		if (BossSequenceController.currentData == null || string.IsNullOrEmpty(BossSequenceController.currentData.bossSequenceName))
		{
			Debug.LogError("Cannot load existing boss sequence if there is none!");
			return;
		}
		BossSequenceController.currentSequence = Resources.Load<BossSequence>(Path.Combine("GG", BossSequenceController.currentData.bossSequenceName));
		if (BossSequenceController.currentSequence)
		{
			string name = BossSceneController.Instance.gameObject.scene.name;
			BossSequenceController.bossIndex = -1;
			for (int i = 0; i < BossSequenceController.currentSequence.Count; i++)
			{
				if (BossSequenceController.currentSequence.GetSceneAt(i) == name)
				{
					BossSequenceController.bossIndex = i;
					break;
				}
			}
			if (BossSequenceController.bossIndex < 0)
			{
				Debug.LogError(string.Format("Could not find current scene {0} in boss sequence {1}", name, BossSequenceController.currentSequence.name));
			}
			BossSequenceController.SetupBossScene();
			caller.StartCoroutine(BossSequenceController.ResetBindingDisplay());
			return;
		}
		Debug.LogError("Boss sequence couldn't be loaded by name!");
	}

	// Token: 0x06001ECD RID: 7885 RVA: 0x0008CEE0 File Offset: 0x0008B0E0
	public static void ApplyBindings()
	{
		if (BossSequenceController.BoundNail)
		{
			EventRegister.SendEvent(EventRegisterEvents.ShowBoundNail, null);
		}
		if (BossSequenceController.BoundCharms)
		{
			BossSequenceController.SetCharmsEquipped(false);
			EventRegister.SendEvent(EventRegisterEvents.ShowBoundCharms, null);
		}
		HeroController.instance.CharmUpdate();
		PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
		EventRegister.SendEvent(EventRegisterEvents.UpdateBlueHealth, null);
		PlayMakerFSM.BroadcastEvent("HUD IN");
		if (BossSequenceController.BoundSoul)
		{
			EventRegister.SendEvent(EventRegisterEvents.BindVesselOrb, null);
		}
		GameManager.instance.soulOrb_fsm.SendEvent("MP LOSE");
		GameManager.instance.soulVessel_fsm.SendEvent("MP RESERVE DOWN");
		PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");
	}

	// Token: 0x06001ECE RID: 7886 RVA: 0x0008CF84 File Offset: 0x0008B184
	public static void RestoreBindings()
	{
		if (!GameManager.instance || !HeroController.instance)
		{
			return;
		}
		bool boundCharms = BossSequenceController.BoundCharms;
		HeroController.instance.CharmUpdate();
		if (BossSequenceController.currentData != null)
		{
			BossSequenceController.currentData.bindings = BossSequenceController.ChallengeBindings.None;
		}
		GameManager.instance.playerData.MaxHealth();
		HeroController.instance.UpdateBlueHealth();
		EventRegister.SendEvent(EventRegisterEvents.UpdateBlueHealth, null);
		EventRegister.SendEvent(EventRegisterEvents.HideBoundNail, null);
		PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
		EventRegister.SendEvent(EventRegisterEvents.HideBoundCharms, null);
		GameManager.instance.soulOrb_fsm.SendEvent("MP LOSE");
		EventRegister.SendEvent(EventRegisterEvents.UnbindVesselOrb, null);
		PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");
		PlayMakerFSM.BroadcastEvent("HUD IN");
	}

	// Token: 0x06001ECF RID: 7887 RVA: 0x0008D043 File Offset: 0x0008B243
	private static IEnumerator ResetBindingDisplay()
	{
		while (!GameCameras.instance.hudCamera.gameObject.activeInHierarchy)
		{
			yield return null;
		}
		BossSequenceController.ApplyBindings();
		yield break;
	}

	// Token: 0x06001ED0 RID: 7888 RVA: 0x0008D04B File Offset: 0x0008B24B
	private static void SetupBossScene()
	{
		BossSceneController.SetupEventDelegate setupEvent = null;
		setupEvent = delegate(BossSceneController self)
		{
			self.DreamReturnEvent = "DREAM EXIT";
			bool flag = false;
			if (self.customExitPoint)
			{
				if (BossSequenceController.currentSequence.GetSceneAt(BossSequenceController.bossIndex - 1) != self.gameObject.scene.name)
				{
					BossSequenceController.IncrementBossIndex();
				}
				if (BossSequenceController.bossIndex >= BossSequenceController.currentSequence.Count)
				{
					Debug.LogError("The last Boss Scene in a sequence can not have a custom exit point!");
				}
				else
				{
					string sceneAt = BossSequenceController.currentSequence.GetSceneAt(BossSequenceController.bossIndex);
					string entryPoint = "door_dreamEnter";
					self.customExitPoint.SetTargetScene(sceneAt);
					self.customExitPoint.entryPoint = entryPoint;
				}
				BossSceneController.SetupEvent = setupEvent;
			}
			if (BossSequenceController.bossIndex == 0)
			{
				self.ApplyBindings();
			}
			if (!flag)
			{
				self.OnBossSceneComplete += delegate()
				{
					BossSequenceController.FinishBossScene(self, setupEvent);
				};
			}
		};
		BossSceneController.SetupEvent = setupEvent;
	}

	// Token: 0x06001ED1 RID: 7889 RVA: 0x0008D075 File Offset: 0x0008B275
	private static void IncrementBossIndex()
	{
		BossSequenceController.bossIndex++;
		if (BossSequenceController.bossIndex < BossSequenceController.currentSequence.Count && !BossSequenceController.currentSequence.CanLoad(BossSequenceController.bossIndex))
		{
			BossSequenceController.IncrementBossIndex();
		}
	}

	// Token: 0x06001ED2 RID: 7890 RVA: 0x0008D0AC File Offset: 0x0008B2AC
	private static void FinishBossScene(BossSceneController self, BossSceneController.SetupEventDelegate setupEvent)
	{
		BossSequenceController.IncrementBossIndex();
		if (BossSequenceController.bossIndex >= BossSequenceController.currentSequence.Count)
		{
			BossSequenceController.FinishLastBossScene(self);
			return;
		}
		GameManager.instance.playerData.currentBossSequence = BossSequenceController.currentData;
		PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(self.gameObject, "Dream Enter Next Scene");
		if (playMakerFSM)
		{
			playMakerFSM.FsmVariables.FindFsmString("To Scene").Value = BossSequenceController.currentSequence.GetSceneAt(BossSequenceController.bossIndex);
			playMakerFSM.FsmVariables.FindFsmString("Entry Door").Value = "door_dreamEnter";
			playMakerFSM.SendEvent("DREAM RETURN");
		}
		BossSceneController.SetupEvent = setupEvent;
	}

	// Token: 0x06001ED3 RID: 7891 RVA: 0x0008D154 File Offset: 0x0008B354
	private static void FinishLastBossScene(BossSceneController self)
	{
		BossSequenceController.WasCompleted = true;
		if (!string.IsNullOrEmpty(BossSequenceController.currentSequence.achievementKey))
		{
			GameManager.instance.QueueAchievement(BossSequenceController.currentSequence.achievementKey);
		}
		BossSequenceDoor.Completion previousCompletion = BossSequenceController.currentData.previousCompletion;
		previousCompletion.completed = true;
		if (BossSequenceController.BoundNail)
		{
			previousCompletion.boundNail = true;
		}
		if (BossSequenceController.BoundShell)
		{
			previousCompletion.boundShell = true;
		}
		if (BossSequenceController.BoundCharms)
		{
			previousCompletion.boundCharms = true;
		}
		if (BossSequenceController.BoundSoul)
		{
			previousCompletion.boundSoul = true;
		}
		if (BossSequenceController.BoundNail && BossSequenceController.BoundShell && BossSequenceController.BoundCharms && BossSequenceController.BoundSoul)
		{
			previousCompletion.allBindings = true;
		}
		if (!BossSequenceController.KnightDamaged)
		{
			previousCompletion.noHits = true;
		}
		GameManager.instance.SetPlayerDataVariable<BossSequenceDoor.Completion>(BossSequenceController.currentData.playerData, previousCompletion);
		HeroController.instance.MaxHealth();
		string value = "GG_End_Sequence";
		if (!string.IsNullOrEmpty(BossSequenceController.currentSequence.customEndScene))
		{
			if (string.IsNullOrEmpty(BossSequenceController.currentSequence.customEndScenePlayerData) || !GameManager.instance.GetPlayerDataBool(BossSequenceController.currentSequence.customEndScenePlayerData))
			{
				value = BossSequenceController.currentSequence.customEndScene;
			}
			if (!string.IsNullOrEmpty(BossSequenceController.currentSequence.customEndScenePlayerData))
			{
				GameManager.instance.SetPlayerDataBool(BossSequenceController.currentSequence.customEndScenePlayerData, true);
			}
		}
		StaticVariableList.SetValue("ggEndScene", value, 0);
		self.DoDreamReturn();
	}

	// Token: 0x06001ED4 RID: 7892 RVA: 0x0008D2AE File Offset: 0x0008B4AE
	public static bool CheckIfSequence(BossSequence sequence)
	{
		return BossSequenceController.currentSequence == sequence;
	}

	// Token: 0x06001ED5 RID: 7893 RVA: 0x0008D2BB File Offset: 0x0008B4BB
	private static void SetMinValue(ref int variable, params int[] values)
	{
		variable = Mathf.Min(variable, Mathf.Min(values));
	}

	// Token: 0x06001ED6 RID: 7894 RVA: 0x0008D2CC File Offset: 0x0008B4CC
	private static void SetCharmsEquipped(bool value)
	{
		if (BossSequenceController.currentData != null)
		{
			foreach (int num in BossSequenceController.currentData.previousEquippedCharms)
			{
				GameManager.instance.SetPlayerDataBool("equippedCharm_" + num.ToString(), value);
			}
		}
	}

	// Token: 0x04001DAA RID: 7594
	private static BossSequenceController.BossSequenceData currentData;

	// Token: 0x04001DAB RID: 7595
	private static BossSequence currentSequence;

	// Token: 0x04001DAC RID: 7596
	private static int bossIndex;

	// Token: 0x0200162F RID: 5679
	[Flags]
	public enum ChallengeBindings
	{
		// Token: 0x040089E9 RID: 35305
		None = 0,
		// Token: 0x040089EA RID: 35306
		Nail = 1,
		// Token: 0x040089EB RID: 35307
		Shell = 2,
		// Token: 0x040089EC RID: 35308
		Charms = 4,
		// Token: 0x040089ED RID: 35309
		Soul = 8
	}

	// Token: 0x02001630 RID: 5680
	[Serializable]
	public class BossSequenceData
	{
		// Token: 0x040089EE RID: 35310
		public BossSequenceController.ChallengeBindings bindings;

		// Token: 0x040089EF RID: 35311
		public float timer;

		// Token: 0x040089F0 RID: 35312
		public bool knightDamaged;

		// Token: 0x040089F1 RID: 35313
		public string playerData;

		// Token: 0x040089F2 RID: 35314
		public BossSequenceDoor.Completion previousCompletion;

		// Token: 0x040089F3 RID: 35315
		public int[] previousEquippedCharms;

		// Token: 0x040089F4 RID: 35316
		public bool wasOvercharmed;

		// Token: 0x040089F5 RID: 35317
		public string bossSequenceName;
	}
}
