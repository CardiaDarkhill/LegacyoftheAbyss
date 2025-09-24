using System;
using TeamCherry.Localization;
using UnityEngine;

namespace GlobalSettings
{
	// Token: 0x020008CD RID: 2253
	[CreateAssetMenu(menuName = "Hornet/Global Settings/Global UI Settings")]
	public class UI : GlobalSettingsBase<UI>
	{
		// Token: 0x170009FE RID: 2558
		// (get) Token: 0x06004EAB RID: 20139 RVA: 0x0016DCBD File Offset: 0x0016BEBD
		public static Color DisabledUiTextColor
		{
			get
			{
				return UI.Get().disabledUiTextColor;
			}
		}

		// Token: 0x170009FF RID: 2559
		// (get) Token: 0x06004EAC RID: 20140 RVA: 0x0016DCC9 File Offset: 0x0016BEC9
		public static float NewDotScaleTime
		{
			get
			{
				return UI.Get().newDotScaleTime;
			}
		}

		// Token: 0x17000A00 RID: 2560
		// (get) Token: 0x06004EAD RID: 20141 RVA: 0x0016DCD5 File Offset: 0x0016BED5
		public static float NewDotScaleDelay
		{
			get
			{
				return UI.Get().newDotScaleDelay;
			}
		}

		// Token: 0x17000A01 RID: 2561
		// (get) Token: 0x06004EAE RID: 20142 RVA: 0x0016DCE1 File Offset: 0x0016BEE1
		public static Vector3 UIMsgPopupStartPosition
		{
			get
			{
				return UI.Get().uiMsgPopupStartPosition;
			}
		}

		// Token: 0x17000A02 RID: 2562
		// (get) Token: 0x06004EAF RID: 20143 RVA: 0x0016DCED File Offset: 0x0016BEED
		public static Vector3 UIMsgPopupStackOffset
		{
			get
			{
				return UI.Get().uiMsgPopupStackOffset;
			}
		}

		// Token: 0x17000A03 RID: 2563
		// (get) Token: 0x06004EB0 RID: 20144 RVA: 0x0016DCF9 File Offset: 0x0016BEF9
		public static CollectableUIMsg CollectableUIMsgPrefab
		{
			get
			{
				return UI.Get().collectableUIMsgPrefab;
			}
		}

		// Token: 0x17000A04 RID: 2564
		// (get) Token: 0x06004EB1 RID: 20145 RVA: 0x0016DD05 File Offset: 0x0016BF05
		public static CrestSlotUnlockMsg CrestSlotUnlockMsgPrefab
		{
			get
			{
				return UI.Get().crestSlotUnlockMsgPrefab;
			}
		}

		// Token: 0x17000A05 RID: 2565
		// (get) Token: 0x06004EB2 RID: 20146 RVA: 0x0016DD11 File Offset: 0x0016BF11
		public static ToolTutorialMsg ToolTutorialMsgPrefab
		{
			get
			{
				return UI.Get().toolTutorialMsgPrefab;
			}
		}

		// Token: 0x17000A06 RID: 2566
		// (get) Token: 0x06004EB3 RID: 20147 RVA: 0x0016DD1D File Offset: 0x0016BF1D
		public static LocalisedString MaxItemsPopup
		{
			get
			{
				return UI.Get().maxItemsPopup;
			}
		}

		// Token: 0x17000A07 RID: 2567
		// (get) Token: 0x06004EB4 RID: 20148 RVA: 0x0016DD29 File Offset: 0x0016BF29
		public static Color MaxItemsTextColor
		{
			get
			{
				return UI.Get().maxItemsTextColor;
			}
		}

		// Token: 0x17000A08 RID: 2568
		// (get) Token: 0x06004EB5 RID: 20149 RVA: 0x0016DD35 File Offset: 0x0016BF35
		public static LocalisedString ItemTakenPopup
		{
			get
			{
				return UI.Get().itemTakenPopup;
			}
		}

		// Token: 0x17000A09 RID: 2569
		// (get) Token: 0x06004EB6 RID: 20150 RVA: 0x0016DD41 File Offset: 0x0016BF41
		public static LocalisedString ItemDepositedPopup
		{
			get
			{
				return UI.Get().itemDepositedPopup;
			}
		}

		// Token: 0x17000A0A RID: 2570
		// (get) Token: 0x06004EB7 RID: 20151 RVA: 0x0016DD4D File Offset: 0x0016BF4D
		public static LocalisedString ItemGivenPopup
		{
			get
			{
				return UI.Get().itemGivenPopup;
			}
		}

		// Token: 0x17000A0B RID: 2571
		// (get) Token: 0x06004EB8 RID: 20152 RVA: 0x0016DD59 File Offset: 0x0016BF59
		public static float ItemGivenPopupDelay
		{
			get
			{
				return UI.Get().itemGivenPopupDelay;
			}
		}

		// Token: 0x17000A0C RID: 2572
		// (get) Token: 0x06004EB9 RID: 20153 RVA: 0x0016DD65 File Offset: 0x0016BF65
		public static LocalisedString DestroyedPopup
		{
			get
			{
				return UI.Get().destroyedPopup;
			}
		}

		// Token: 0x17000A0D RID: 2573
		// (get) Token: 0x06004EBA RID: 20154 RVA: 0x0016DD71 File Offset: 0x0016BF71
		public static LocalisedString QuestContinuePopup
		{
			get
			{
				return UI.Get().questContinuePopup;
			}
		}

		// Token: 0x17000A0E RID: 2574
		// (get) Token: 0x06004EBB RID: 20155 RVA: 0x0016DD7D File Offset: 0x0016BF7D
		public static AudioEvent QuestContinuePopupSound
		{
			get
			{
				return UI.Get().questContinuePopupSound;
			}
		}

		// Token: 0x17000A0F RID: 2575
		// (get) Token: 0x06004EBC RID: 20156 RVA: 0x0016DD89 File Offset: 0x0016BF89
		public static LocalisedString MainQuestBeginPopup
		{
			get
			{
				return UI.Get().mainQuestBeginPopup;
			}
		}

		// Token: 0x17000A10 RID: 2576
		// (get) Token: 0x06004EBD RID: 20157 RVA: 0x0016DD95 File Offset: 0x0016BF95
		public static LocalisedString MainQuestProgressPopup
		{
			get
			{
				return UI.Get().mainQuestProgressPopup;
			}
		}

		// Token: 0x17000A11 RID: 2577
		// (get) Token: 0x06004EBE RID: 20158 RVA: 0x0016DDA1 File Offset: 0x0016BFA1
		public static LocalisedString MainQuestCompletePopup
		{
			get
			{
				return UI.Get().mainQuestCompletePopup;
			}
		}

		// Token: 0x17000A12 RID: 2578
		// (get) Token: 0x06004EBF RID: 20159 RVA: 0x0016DDAD File Offset: 0x0016BFAD
		public static float ItemQuestMaxPopupDelay
		{
			get
			{
				return UI.Get().itemQuestMaxPopupDelay;
			}
		}

		// Token: 0x17000A13 RID: 2579
		// (get) Token: 0x06004EC0 RID: 20160 RVA: 0x0016DDB9 File Offset: 0x0016BFB9
		public static LocalisedString ItemQuestMaxPopup
		{
			get
			{
				return UI.Get().itemQuestMaxPopup;
			}
		}

		// Token: 0x17000A14 RID: 2580
		// (get) Token: 0x06004EC1 RID: 20161 RVA: 0x0016DDC5 File Offset: 0x0016BFC5
		public static AudioEvent ItemQuestMaxPopupSound
		{
			get
			{
				return UI.Get().itemQuestMaxPopupSound;
			}
		}

		// Token: 0x17000A15 RID: 2581
		// (get) Token: 0x06004EC2 RID: 20162 RVA: 0x0016DDD1 File Offset: 0x0016BFD1
		public static GameObject MapUpdatedPopupPrefab
		{
			get
			{
				return UI.Get().mapUpdatedPopupPrefab;
			}
		}

		// Token: 0x06004EC3 RID: 20163 RVA: 0x0016DDDD File Offset: 0x0016BFDD
		[RuntimeInitializeOnLoadMethod]
		public static void PreWarm()
		{
			GlobalSettingsBase<UI>.StartPreloadAddressable("Global UI Settings");
		}

		// Token: 0x06004EC4 RID: 20164 RVA: 0x0016DDE9 File Offset: 0x0016BFE9
		public static void Unload()
		{
			GlobalSettingsBase<UI>.StartUnload();
		}

		// Token: 0x06004EC5 RID: 20165 RVA: 0x0016DDF0 File Offset: 0x0016BFF0
		private static UI Get()
		{
			return GlobalSettingsBase<UI>.Get("Global UI Settings");
		}

		// Token: 0x06004EC6 RID: 20166 RVA: 0x0016DDFC File Offset: 0x0016BFFC
		public static Color GetToolTypeColor(ToolItemType type)
		{
			UI ui = UI.Get();
			ui.EnsureColorsArray();
			return ui.toolTypeColors[(int)type];
		}

		// Token: 0x06004EC7 RID: 20167 RVA: 0x0016DE14 File Offset: 0x0016C014
		private void OnValidate()
		{
			ArrayForEnumAttribute.EnsureArraySize<Color>(ref this.toolTypeColors, typeof(ToolItemType));
		}

		// Token: 0x06004EC8 RID: 20168 RVA: 0x0016DE2B File Offset: 0x0016C02B
		private void EnsureColorsArray()
		{
			if (this.validated)
			{
				return;
			}
			this.validated = true;
			ArrayForEnumAttribute.EnsureArraySize<Color>(ref this.toolTypeColors, typeof(ToolItemType));
		}

		// Token: 0x04004F59 RID: 20313
		[SerializeField]
		private Color disabledUiTextColor;

		// Token: 0x04004F5A RID: 20314
		[Space]
		[SerializeField]
		private float newDotScaleTime = 0.1f;

		// Token: 0x04004F5B RID: 20315
		[SerializeField]
		private float newDotScaleDelay = 0.2f;

		// Token: 0x04004F5C RID: 20316
		[Space]
		[SerializeField]
		private Vector3 uiMsgPopupStartPosition;

		// Token: 0x04004F5D RID: 20317
		[SerializeField]
		private Vector3 uiMsgPopupStackOffset;

		// Token: 0x04004F5E RID: 20318
		[SerializeField]
		private CollectableUIMsg collectableUIMsgPrefab;

		// Token: 0x04004F5F RID: 20319
		[SerializeField]
		private CrestSlotUnlockMsg crestSlotUnlockMsgPrefab;

		// Token: 0x04004F60 RID: 20320
		[Space]
		[SerializeField]
		private ToolTutorialMsg toolTutorialMsgPrefab;

		// Token: 0x04004F61 RID: 20321
		[Space]
		[SerializeField]
		[ArrayForEnum(typeof(ToolItemType))]
		private Color[] toolTypeColors;

		// Token: 0x04004F62 RID: 20322
		[SerializeField]
		private LocalisedString maxItemsPopup;

		// Token: 0x04004F63 RID: 20323
		[SerializeField]
		private Color maxItemsTextColor;

		// Token: 0x04004F64 RID: 20324
		[SerializeField]
		private LocalisedString itemTakenPopup;

		// Token: 0x04004F65 RID: 20325
		[SerializeField]
		private LocalisedString itemDepositedPopup;

		// Token: 0x04004F66 RID: 20326
		[SerializeField]
		private LocalisedString itemGivenPopup;

		// Token: 0x04004F67 RID: 20327
		[SerializeField]
		private float itemGivenPopupDelay;

		// Token: 0x04004F68 RID: 20328
		[SerializeField]
		private LocalisedString destroyedPopup;

		// Token: 0x04004F69 RID: 20329
		[Space]
		[SerializeField]
		private LocalisedString questContinuePopup;

		// Token: 0x04004F6A RID: 20330
		[SerializeField]
		private AudioEvent questContinuePopupSound;

		// Token: 0x04004F6B RID: 20331
		[Space]
		[SerializeField]
		private LocalisedString mainQuestBeginPopup;

		// Token: 0x04004F6C RID: 20332
		[SerializeField]
		private LocalisedString mainQuestProgressPopup;

		// Token: 0x04004F6D RID: 20333
		[SerializeField]
		private LocalisedString mainQuestCompletePopup;

		// Token: 0x04004F6E RID: 20334
		[Space]
		[SerializeField]
		private float itemQuestMaxPopupDelay;

		// Token: 0x04004F6F RID: 20335
		[SerializeField]
		private LocalisedString itemQuestMaxPopup;

		// Token: 0x04004F70 RID: 20336
		[SerializeField]
		private AudioEvent itemQuestMaxPopupSound;

		// Token: 0x04004F71 RID: 20337
		[SerializeField]
		private GameObject mapUpdatedPopupPrefab;

		// Token: 0x04004F72 RID: 20338
		private bool validated;
	}
}
