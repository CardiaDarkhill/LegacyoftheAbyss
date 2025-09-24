using System;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x02000404 RID: 1028
public abstract class ChangeByLanguageOld<T> : ChangeByLanguageBase where T : ChangeByLanguageOld<T>.LanguageOverride
{
	// Token: 0x060022EB RID: 8939 RVA: 0x000A00FE File Offset: 0x0009E2FE
	private void Awake()
	{
		this.CreateLookup(true);
		this.DoAwake();
		this.RecordOriginalValues();
	}

	// Token: 0x060022EC RID: 8940 RVA: 0x000A0114 File Offset: 0x0009E314
	private void CreateLookup(bool log = true)
	{
		foreach (T t in this.offsetOverrides)
		{
			LanguageCode languageCode = (LanguageCode)t.languageCode;
			if (!this.languageCodeOverrides.ContainsKey(languageCode))
			{
				this.languageCodeOverrides[languageCode] = t;
			}
		}
	}

	// Token: 0x060022ED RID: 8941 RVA: 0x000A0188 File Offset: 0x0009E388
	private void Start()
	{
		this.hasStarted = true;
		this.DoStart();
		this.DoUpdate();
	}

	// Token: 0x060022EE RID: 8942 RVA: 0x000A019D File Offset: 0x0009E39D
	protected virtual void DoAwake()
	{
	}

	// Token: 0x060022EF RID: 8943 RVA: 0x000A019F File Offset: 0x0009E39F
	protected virtual void DoStart()
	{
	}

	// Token: 0x060022F0 RID: 8944 RVA: 0x000A01A1 File Offset: 0x0009E3A1
	protected virtual void OnValidate()
	{
		if (Application.isPlaying)
		{
			this.CreateLookup(false);
		}
	}

	// Token: 0x060022F1 RID: 8945 RVA: 0x000A01B1 File Offset: 0x0009E3B1
	protected virtual void OnEnable()
	{
		this.RegisterEvents();
		if (this.hasStarted && (!this.onStartOnly || CheatManager.ForceLanguageComponentUpdates))
		{
			this.DoUpdate();
		}
	}

	// Token: 0x060022F2 RID: 8946 RVA: 0x000A01D6 File Offset: 0x0009E3D6
	private void OnDisable()
	{
		this.UnregisterEvents();
	}

	// Token: 0x060022F3 RID: 8947
	protected abstract void RecordOriginalValues();

	// Token: 0x060022F4 RID: 8948 RVA: 0x000A01E0 File Offset: 0x0009E3E0
	private void RegisterEvents()
	{
		if (!this.registeredEvents)
		{
			this.registeredEvents = true;
			GameManager instance = GameManager.instance;
			if (instance != null)
			{
				instance.RefreshLanguageText += this.DoUpdate;
			}
			if (this.doHandHeldUpdates)
			{
				Platform.Current.OnScreenModeChanged += this.OnScreenModeChanged;
				this.handHeldEvents = true;
			}
		}
	}

	// Token: 0x060022F5 RID: 8949 RVA: 0x000A0244 File Offset: 0x0009E444
	private void UnregisterEvents()
	{
		if (this.registeredEvents)
		{
			this.registeredEvents = false;
			GameManager instance = GameManager.instance;
			if (instance != null)
			{
				instance.RefreshLanguageText -= this.DoUpdate;
			}
			if (this.handHeldEvents)
			{
				Platform.Current.OnScreenModeChanged -= this.OnScreenModeChanged;
				this.handHeldEvents = false;
			}
		}
	}

	// Token: 0x060022F6 RID: 8950 RVA: 0x000A02A7 File Offset: 0x0009E4A7
	private void OnScreenModeChanged(Platform.ScreenModeState screenMode)
	{
		if (this.doHandHeldUpdates)
		{
			this.DoUpdate();
		}
	}

	// Token: 0x060022F7 RID: 8951 RVA: 0x000A02B7 File Offset: 0x0009E4B7
	private void RevertValues()
	{
		if (!this.hasChanged || !this.recorded)
		{
			return;
		}
		this.hasChanged = false;
		this.DoRevertValues();
	}

	// Token: 0x060022F8 RID: 8952
	protected abstract void DoRevertValues();

	// Token: 0x060022F9 RID: 8953
	protected abstract void ApplySetting(T setting);

	// Token: 0x060022FA RID: 8954 RVA: 0x000A02D7 File Offset: 0x0009E4D7
	public virtual void ApplyHandHeld()
	{
	}

	// Token: 0x060022FB RID: 8955 RVA: 0x000A02D9 File Offset: 0x0009E4D9
	protected bool ShouldApplyHandHeld()
	{
		return this.doHandHeldUpdates && Platform.Current.IsRunningOnHandHeld;
	}

	// Token: 0x060022FC RID: 8956 RVA: 0x000A02F0 File Offset: 0x0009E4F0
	public override void DoUpdate()
	{
		this.RevertValues();
		if (this.ShouldApplyHandHeld())
		{
			this.ApplyHandHeld();
			this.hasChanged = true;
		}
		LanguageCode key = Language.CurrentLanguage();
		T setting;
		if (this.languageCodeOverrides.TryGetValue(key, out setting))
		{
			this.ApplySetting(setting);
			this.hasChanged = true;
		}
	}

	// Token: 0x040021B0 RID: 8624
	[SerializeField]
	protected List<T> offsetOverrides = new List<T>();

	// Token: 0x040021B1 RID: 8625
	[SerializeField]
	private bool onStartOnly;

	// Token: 0x040021B2 RID: 8626
	[SerializeField]
	private bool doHandHeldUpdates;

	// Token: 0x040021B3 RID: 8627
	private Dictionary<LanguageCode, T> languageCodeOverrides = new Dictionary<LanguageCode, T>();

	// Token: 0x040021B4 RID: 8628
	private Vector2 originalPosition;

	// Token: 0x040021B5 RID: 8629
	private bool hasStarted;

	// Token: 0x040021B6 RID: 8630
	private bool hasChanged;

	// Token: 0x040021B7 RID: 8631
	protected bool recorded;

	// Token: 0x040021B8 RID: 8632
	private bool registeredEvents;

	// Token: 0x040021B9 RID: 8633
	private bool handHeldEvents;

	// Token: 0x02001698 RID: 5784
	[Serializable]
	public abstract class LanguageOverride
	{
		// Token: 0x04008B7E RID: 35710
		public SupportedLanguages languageCode;
	}
}
