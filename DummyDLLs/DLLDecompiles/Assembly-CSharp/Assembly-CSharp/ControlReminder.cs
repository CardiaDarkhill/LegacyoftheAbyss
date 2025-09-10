using System;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using InControl;
using TeamCherry.Localization;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using TMProOld;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200061E RID: 1566
public class ControlReminder : MonoBehaviour
{
	// Token: 0x1700065A RID: 1626
	// (get) Token: 0x060037AC RID: 14252 RVA: 0x000F5695 File Offset: 0x000F3895
	private static ControlReminder Instance
	{
		get
		{
			if (ControlReminder._instance)
			{
				return ControlReminder._instance;
			}
			ControlReminder._instance = Object.FindObjectOfType<ControlReminder>();
			if (!ControlReminder._instance)
			{
				Debug.LogError("Couldn't find ControlReminder instance.");
			}
			return ControlReminder._instance;
		}
	}

	// Token: 0x060037AD RID: 14253 RVA: 0x000F56CE File Offset: 0x000F38CE
	private void Awake()
	{
		this.singleTemplate.gameObject.SetActive(false);
	}

	// Token: 0x060037AE RID: 14254 RVA: 0x000F56E4 File Offset: 0x000F38E4
	private void Start()
	{
		foreach (ControlReminder.ConfigBase configBase in this.singleConfigs.Cast<ControlReminder.ConfigBase>().Union(this.doubleConfigs))
		{
			configBase.SubscribeEvents(this);
		}
		this.HideAll(true, false);
		GameManager.instance.NextSceneWillActivate += this.OnInstanceOnNextSceneWillActivate;
	}

	// Token: 0x060037AF RID: 14255 RVA: 0x000F5760 File Offset: 0x000F3960
	private void OnDestroy()
	{
		if (ControlReminder._instance == this)
		{
			ControlReminder._instance = null;
		}
		if (GameManager.instance)
		{
			GameManager.instance.NextSceneWillActivate -= this.OnInstanceOnNextSceneWillActivate;
		}
	}

	// Token: 0x060037B0 RID: 14256 RVA: 0x000F5797 File Offset: 0x000F3997
	private void OnInstanceOnNextSceneWillActivate()
	{
		this.HideAll(true, false);
	}

	// Token: 0x060037B1 RID: 14257 RVA: 0x000F57A4 File Offset: 0x000F39A4
	private void Update()
	{
		int count = this.currentReminders.Count;
		foreach (ControlReminder.ConfigBase configBase in this.currentReminders)
		{
			if (configBase != null)
			{
				configBase.Update();
			}
			if (this.currentReminders.Count != count)
			{
				break;
			}
		}
	}

	// Token: 0x060037B2 RID: 14258 RVA: 0x000F5818 File Offset: 0x000F3A18
	public void ForceUpdateReminderText()
	{
		int num = 0;
		foreach (ControlReminder.ConfigBase configBase in this.currentReminders)
		{
			ControlReminder.SingleConfig singleConfig = configBase as ControlReminder.SingleConfig;
			if (singleConfig != null)
			{
				if (this.spawnedSingleReminders.Count > num)
				{
					ControlReminderSingle controlReminderSingle = this.spawnedSingleReminders[num];
					num++;
					controlReminderSingle.Activate(singleConfig);
				}
			}
			else
			{
				ControlReminder.DoubleConfig doubleConfig = configBase as ControlReminder.DoubleConfig;
				if (doubleConfig != null)
				{
					this.doubleActionPromptText1.text = (doubleConfig.Prompt1.IsEmpty ? string.Empty : doubleConfig.Prompt1);
					this.doubleActionPromptText2.text = (doubleConfig.Prompt2.IsEmpty ? string.Empty : doubleConfig.Prompt2);
					this.doubleActionText.text = doubleConfig.Text;
				}
			}
		}
	}

	// Token: 0x060037B3 RID: 14259 RVA: 0x000F591C File Offset: 0x000F3B1C
	private void Hide(ControlReminder.ConfigBase config, bool isInstant, bool keepMinTime)
	{
		if (this.currentReminders.Contains(config))
		{
			this.HideAll(isInstant, keepMinTime);
		}
	}

	// Token: 0x060037B4 RID: 14260 RVA: 0x000F5934 File Offset: 0x000F3B34
	private void HideAll(bool isInstant, bool keepMinTime)
	{
		if (this.fadeOutRoutine != null)
		{
			base.StopCoroutine(this.fadeOutRoutine);
		}
		if (isInstant || this.currentReminders.Count == 0)
		{
			this.StopFadeIn();
			this.DisableReminders();
		}
		else
		{
			float num = this.currentReminders.Max((ControlReminder.ConfigBase c) => c.FadeOutTime);
			float startAlpha = this.fadeGroup.AlphaSelf;
			if (startAlpha <= 0.01f)
			{
				this.StopFadeIn();
			}
			float num2;
			if (keepMinTime)
			{
				num2 = 2.3f - (float)(Time.unscaledTimeAsDouble - this.startFadeInTime);
				if (num2 < 0f)
				{
					num2 = 0f;
				}
			}
			else
			{
				num2 = 0f;
			}
			this.fadeOutRoutine = this.StartTimerRoutine(num2, num * startAlpha, delegate(float time)
			{
				this.fadeGroup.AlphaSelf = Mathf.Lerp(startAlpha, 0f, time);
			}, delegate
			{
				this.StopFadeIn();
				startAlpha = this.fadeGroup.AlphaSelf;
			}, new Action(this.DisableReminders), true);
		}
		this.currentReminders.Clear();
	}

	// Token: 0x060037B5 RID: 14261 RVA: 0x000F5A44 File Offset: 0x000F3C44
	private void StopFadeIn()
	{
		if (this.fadeInRoutine != null)
		{
			base.StopCoroutine(this.fadeInRoutine);
		}
	}

	// Token: 0x060037B6 RID: 14262 RVA: 0x000F5A5C File Offset: 0x000F3C5C
	private void ShowSingle(ControlReminder.SingleConfig config)
	{
		if (this.currentReminders.Contains(config))
		{
			return;
		}
		this.HideAll(true, false);
		if (config == null)
		{
			if (this.pushedSingles.Count == 0)
			{
				return;
			}
		}
		else
		{
			this.pushedSingles.Clear();
			this.pushedSingles.Add(config);
		}
		for (int i = this.pushedSingles.Count - this.spawnedSingleReminders.Count; i > 0; i--)
		{
			ControlReminderSingle item = Object.Instantiate<ControlReminderSingle>(this.singleTemplate, this.singleTemplate.transform.parent);
			this.spawnedSingleReminders.Add(item);
		}
		for (int j = 0; j < this.spawnedSingleReminders.Count; j++)
		{
			ControlReminderSingle controlReminderSingle = this.spawnedSingleReminders[j];
			if (j < this.pushedSingles.Count)
			{
				ControlReminder.SingleConfig singleConfig = this.pushedSingles[j];
				singleConfig.GetButtonAction();
				controlReminderSingle.Activate(singleConfig);
				this.currentReminders.Add(singleConfig);
			}
			else
			{
				controlReminderSingle.gameObject.SetActive(false);
			}
		}
		this.fadeGroup.AlphaSelf = 0f;
		this.layoutGroup.ForceUpdateLayoutNoCanvas();
		float delay = this.currentReminders.Max((ControlReminder.ConfigBase c) => c.FadeInDelay);
		float duration = this.currentReminders.Max((ControlReminder.ConfigBase c) => c.FadeInTime);
		this.startFadeInTime = Time.unscaledTimeAsDouble;
		this.fadeInRoutine = this.StartTimerRoutine(delay, duration, delegate(float time)
		{
			this.fadeGroup.AlphaSelf = time;
		}, null, null, true);
		this.pushedSingles.Clear();
	}

	// Token: 0x060037B7 RID: 14263 RVA: 0x000F5C0C File Offset: 0x000F3E0C
	private void ShowDouble(ControlReminder.DoubleConfig config)
	{
		if (this.currentReminders.Contains(config))
		{
			return;
		}
		this.HideAll(true, false);
		this.fadeGroup.AlphaSelf = 0f;
		this.doubleActionIcon1.SetAction(ControlReminder.MapActionToAction(config.Button1));
		this.doubleActionIcon2.SetAction(ControlReminder.MapActionToAction(config.Button2));
		this.doubleActionPromptText1.text = (config.Prompt1.IsEmpty ? string.Empty : config.Prompt1);
		this.doubleActionPromptText2.text = (config.Prompt2.IsEmpty ? string.Empty : config.Prompt2);
		this.doubleActionText.text = config.Text;
		this.doubleParent.SetActive(true);
		this.layoutGroup.ForceUpdateLayoutNoCanvas();
		this.startFadeInTime = Time.unscaledTimeAsDouble;
		this.fadeInRoutine = this.StartTimerRoutine(config.FadeInDelay, config.FadeInTime, delegate(float time)
		{
			this.fadeGroup.AlphaSelf = time;
		}, null, null, true);
		this.currentReminders.Add(config);
	}

	// Token: 0x060037B8 RID: 14264 RVA: 0x000F5D2C File Offset: 0x000F3F2C
	private void DisableReminders()
	{
		foreach (ControlReminderSingle controlReminderSingle in this.spawnedSingleReminders)
		{
			if (controlReminderSingle.gameObject.activeSelf)
			{
				controlReminderSingle.gameObject.SetActive(false);
			}
		}
		this.doubleParent.SetActive(false);
	}

	// Token: 0x060037B9 RID: 14265 RVA: 0x000F5DA0 File Offset: 0x000F3FA0
	public static void AddReminder(ControlReminder.ConfigBase config, bool doAppear = false)
	{
		config.SubscribeEvents(ControlReminder.Instance);
		if (doAppear)
		{
			config.DoAppear();
		}
	}

	// Token: 0x060037BA RID: 14266 RVA: 0x000F5DB6 File Offset: 0x000F3FB6
	public static void PushSingle(ControlReminder.SingleConfig config)
	{
		config.SubscribeEvents(ControlReminder.Instance);
		ControlReminder.Instance.pushedSingles.Add(config);
	}

	// Token: 0x060037BB RID: 14267 RVA: 0x000F5DD3 File Offset: 0x000F3FD3
	public static void ShowPushed()
	{
		ControlReminder.Instance.ShowSingle(null);
	}

	// Token: 0x060037BC RID: 14268 RVA: 0x000F5DE0 File Offset: 0x000F3FE0
	public static HeroActionButton MapActionToAction(HeroActionButton button)
	{
		if (!Platform.Current.WasLastInputKeyboard)
		{
			return button;
		}
		if (button <= HeroActionButton.MENU_CANCEL)
		{
			if (button == HeroActionButton.MENU_SUBMIT)
			{
				return HeroActionButton.JUMP;
			}
			if (button == HeroActionButton.MENU_CANCEL)
			{
				return HeroActionButton.CAST;
			}
		}
		else
		{
			if (button == HeroActionButton.MENU_EXTRA)
			{
				return HeroActionButton.DASH;
			}
			if (button == HeroActionButton.MENU_SUPER)
			{
				return HeroActionButton.DREAM_NAIL;
			}
		}
		return button;
	}

	// Token: 0x04003ABB RID: 15035
	[Header("Structure")]
	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	// Token: 0x04003ABC RID: 15036
	[SerializeField]
	private LayoutGroup layoutGroup;

	// Token: 0x04003ABD RID: 15037
	[Space]
	[SerializeField]
	private ControlReminderSingle singleTemplate;

	// Token: 0x04003ABE RID: 15038
	[Space]
	[SerializeField]
	private GameObject doubleParent;

	// Token: 0x04003ABF RID: 15039
	[SerializeField]
	private ActionButtonIcon doubleActionIcon1;

	// Token: 0x04003AC0 RID: 15040
	[SerializeField]
	private TMP_Text doubleActionPromptText1;

	// Token: 0x04003AC1 RID: 15041
	[SerializeField]
	private ActionButtonIcon doubleActionIcon2;

	// Token: 0x04003AC2 RID: 15042
	[SerializeField]
	private TMP_Text doubleActionPromptText2;

	// Token: 0x04003AC3 RID: 15043
	[SerializeField]
	private TMP_Text doubleActionText;

	// Token: 0x04003AC4 RID: 15044
	[Header("Configs")]
	[SerializeField]
	private ControlReminder.SingleConfig[] singleConfigs;

	// Token: 0x04003AC5 RID: 15045
	[SerializeField]
	private ControlReminder.DoubleConfig[] doubleConfigs;

	// Token: 0x04003AC6 RID: 15046
	private Coroutine fadeOutRoutine;

	// Token: 0x04003AC7 RID: 15047
	private Coroutine fadeInRoutine;

	// Token: 0x04003AC8 RID: 15048
	private double startFadeInTime;

	// Token: 0x04003AC9 RID: 15049
	private readonly List<ControlReminder.ConfigBase> currentReminders = new List<ControlReminder.ConfigBase>();

	// Token: 0x04003ACA RID: 15050
	private readonly List<ControlReminder.SingleConfig> pushedSingles = new List<ControlReminder.SingleConfig>();

	// Token: 0x04003ACB RID: 15051
	private readonly List<ControlReminderSingle> spawnedSingleReminders = new List<ControlReminderSingle>();

	// Token: 0x04003ACC RID: 15052
	private static ControlReminder _instance;

	// Token: 0x0200192E RID: 6446
	public abstract class ConfigBase
	{
		// Token: 0x06009376 RID: 37750 RVA: 0x0029E52C File Offset: 0x0029C72C
		public void SubscribeEvents(ControlReminder owner)
		{
			if (this.Owner)
			{
				return;
			}
			this.Owner = owner;
			if (!owner)
			{
				return;
			}
			if (!string.IsNullOrEmpty(this.AppearEvent))
			{
				EventRegister.GetRegisterGuaranteed(owner.gameObject, this.AppearEvent).ReceivedEvent += this.DoAppear;
			}
			if (!string.IsNullOrEmpty(this.DisappearEvent))
			{
				EventRegister.GetRegisterGuaranteed(owner.gameObject, this.DisappearEvent).ReceivedEvent += delegate()
				{
					this.Disappear(false, false);
				};
			}
		}

		// Token: 0x06009377 RID: 37751 RVA: 0x0029E5B5 File Offset: 0x0029C7B5
		public void Disappear(bool isInstant, bool keepMinTime = true)
		{
			this.Owner.Hide(this, isInstant, keepMinTime);
		}

		// Token: 0x06009378 RID: 37752 RVA: 0x0029E5C8 File Offset: 0x0029C7C8
		public void DoAppear()
		{
			if (!string.IsNullOrEmpty(this.PlayerDataBool))
			{
				PlayerData instance = PlayerData.instance;
				if (instance.GetVariable(this.PlayerDataBool))
				{
					return;
				}
				instance.SetVariable(this.PlayerDataBool, true);
			}
			this.Appear();
		}

		// Token: 0x06009379 RID: 37753
		protected abstract void Appear();

		// Token: 0x0600937A RID: 37754
		public abstract void Update();

		// Token: 0x040094B4 RID: 38068
		public string AppearEvent;

		// Token: 0x040094B5 RID: 38069
		public string DisappearEvent;

		// Token: 0x040094B6 RID: 38070
		[PlayerDataField(typeof(bool), false)]
		public string PlayerDataBool;

		// Token: 0x040094B7 RID: 38071
		[Space]
		public float FadeInDelay;

		// Token: 0x040094B8 RID: 38072
		public float FadeInTime = 1f;

		// Token: 0x040094B9 RID: 38073
		public float FadeOutTime = 0.5f;

		// Token: 0x040094BA RID: 38074
		protected ControlReminder Owner;
	}

	// Token: 0x0200192F RID: 6447
	[Serializable]
	public class SingleConfig : ControlReminder.ConfigBase
	{
		// Token: 0x0600937D RID: 37757 RVA: 0x0029E632 File Offset: 0x0029C832
		protected override void Appear()
		{
			this.Owner.ShowSingle(this);
			this.GetButtonAction();
		}

		// Token: 0x0600937E RID: 37758 RVA: 0x0029E646 File Offset: 0x0029C846
		public void GetButtonAction()
		{
			if (this.DisappearOnButtonPress)
			{
				this.buttonAction = ManagerSingleton<InputHandler>.Instance.ActionButtonToPlayerAction(this.Button);
			}
		}

		// Token: 0x0600937F RID: 37759 RVA: 0x0029E666 File Offset: 0x0029C866
		public override void Update()
		{
			if (this.buttonAction == null)
			{
				return;
			}
			if (this.buttonAction.IsPressed)
			{
				base.Disappear(false, false);
			}
		}

		// Token: 0x040094BB RID: 38075
		[Space]
		public LocalisedString Text;

		// Token: 0x040094BC RID: 38076
		[LocalisedString.NotRequiredAttribute]
		public LocalisedString Prompt;

		// Token: 0x040094BD RID: 38077
		public HeroActionButton Button;

		// Token: 0x040094BE RID: 38078
		public bool DisappearOnButtonPress;

		// Token: 0x040094BF RID: 38079
		private PlayerAction buttonAction;
	}

	// Token: 0x02001930 RID: 6448
	[Serializable]
	public class DoubleConfig : ControlReminder.ConfigBase
	{
		// Token: 0x06009381 RID: 37761 RVA: 0x0029E68E File Offset: 0x0029C88E
		protected override void Appear()
		{
			this.Owner.ShowDouble(this);
		}

		// Token: 0x06009382 RID: 37762 RVA: 0x0029E69C File Offset: 0x0029C89C
		public override void Update()
		{
		}

		// Token: 0x040094C0 RID: 38080
		[Space]
		public LocalisedString Text;

		// Token: 0x040094C1 RID: 38081
		[LocalisedString.NotRequiredAttribute]
		public LocalisedString Prompt1;

		// Token: 0x040094C2 RID: 38082
		public HeroActionButton Button1;

		// Token: 0x040094C3 RID: 38083
		[LocalisedString.NotRequiredAttribute]
		public LocalisedString Prompt2;

		// Token: 0x040094C4 RID: 38084
		public HeroActionButton Button2;
	}
}
