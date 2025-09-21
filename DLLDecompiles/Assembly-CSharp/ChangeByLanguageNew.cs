using System;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x02000403 RID: 1027
public abstract class ChangeByLanguageNew<T> : ChangeByLanguageBase where T : ChangeByLanguageNew<T>.OverrideValue
{
	// Token: 0x060022D9 RID: 8921 RVA: 0x0009FE89 File Offset: 0x0009E089
	private void Awake()
	{
		this.CreateLookup(true);
		this.DoAwake();
		this.RecordOriginalValues();
	}

	// Token: 0x060022DA RID: 8922 RVA: 0x0009FEA0 File Offset: 0x0009E0A0
	private void CreateLookup(bool log = true)
	{
		foreach (ChangeByLanguageNew<T>.LanguageOverride languageOverride in this.offsetOverrides)
		{
			LanguageCode languageCode = (LanguageCode)languageOverride.languageCode;
			if (!this.languageCodeOverrides.ContainsKey(languageCode))
			{
				this.languageCodeOverrides[languageCode] = languageOverride;
			}
		}
	}

	// Token: 0x060022DB RID: 8923 RVA: 0x0009FF10 File Offset: 0x0009E110
	private void Start()
	{
		this.hasStarted = true;
		this.DoStart();
		this.DoUpdate();
	}

	// Token: 0x060022DC RID: 8924 RVA: 0x0009FF25 File Offset: 0x0009E125
	protected virtual void DoAwake()
	{
	}

	// Token: 0x060022DD RID: 8925 RVA: 0x0009FF27 File Offset: 0x0009E127
	protected virtual void DoStart()
	{
	}

	// Token: 0x060022DE RID: 8926 RVA: 0x0009FF29 File Offset: 0x0009E129
	protected virtual void OnValidate()
	{
		if (Application.isPlaying)
		{
			this.CreateLookup(false);
		}
	}

	// Token: 0x060022DF RID: 8927 RVA: 0x0009FF39 File Offset: 0x0009E139
	protected virtual void OnEnable()
	{
		this.RegisterEvents();
		if (this.hasStarted && (!this.onStartOnly || CheatManager.ForceLanguageComponentUpdates))
		{
			this.DoUpdate();
		}
	}

	// Token: 0x060022E0 RID: 8928 RVA: 0x0009FF5E File Offset: 0x0009E15E
	private void OnDisable()
	{
		this.UnregisterEvents();
	}

	// Token: 0x060022E1 RID: 8929
	protected abstract void RecordOriginalValues();

	// Token: 0x060022E2 RID: 8930 RVA: 0x0009FF68 File Offset: 0x0009E168
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

	// Token: 0x060022E3 RID: 8931 RVA: 0x0009FFCC File Offset: 0x0009E1CC
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

	// Token: 0x060022E4 RID: 8932 RVA: 0x000A002F File Offset: 0x0009E22F
	private void OnScreenModeChanged(Platform.ScreenModeState screenMode)
	{
		if (this.doHandHeldUpdates)
		{
			this.DoUpdate();
		}
	}

	// Token: 0x060022E5 RID: 8933 RVA: 0x000A003F File Offset: 0x0009E23F
	private void RevertValues()
	{
		if (!this.hasChanged || !this.recorded)
		{
			return;
		}
		this.hasChanged = false;
		this.DoRevertValues();
	}

	// Token: 0x060022E6 RID: 8934
	protected abstract void DoRevertValues();

	// Token: 0x060022E7 RID: 8935
	protected abstract void ApplySetting(T setting);

	// Token: 0x060022E8 RID: 8936 RVA: 0x000A005F File Offset: 0x0009E25F
	protected bool ShouldApplyHandHeld()
	{
		return this.doHandHeldUpdates && Platform.Current.IsRunningOnHandHeld;
	}

	// Token: 0x060022E9 RID: 8937 RVA: 0x000A0078 File Offset: 0x0009E278
	public override void DoUpdate()
	{
		this.RevertValues();
		LanguageCode key = Language.CurrentLanguage();
		bool flag = this.ShouldApplyHandHeld();
		if (flag)
		{
			this.ApplySetting(this.handHeldOverrides);
			this.hasChanged = true;
		}
		ChangeByLanguageNew<T>.LanguageOverride languageOverride;
		if (this.languageCodeOverrides.TryGetValue(key, out languageOverride))
		{
			this.ApplySetting(languageOverride.languageOverrides);
			if (flag)
			{
				this.ApplySetting(languageOverride.handHeldOverrides);
			}
			this.hasChanged = true;
		}
	}

	// Token: 0x040021A5 RID: 8613
	[SerializeField]
	protected List<ChangeByLanguageNew<T>.LanguageOverride> offsetOverrides = new List<ChangeByLanguageNew<T>.LanguageOverride>();

	// Token: 0x040021A6 RID: 8614
	[SerializeField]
	private bool onStartOnly;

	// Token: 0x040021A7 RID: 8615
	[SerializeField]
	private bool doHandHeldUpdates;

	// Token: 0x040021A8 RID: 8616
	[SerializeField]
	private T handHeldOverrides;

	// Token: 0x040021A9 RID: 8617
	private Dictionary<LanguageCode, ChangeByLanguageNew<T>.LanguageOverride> languageCodeOverrides = new Dictionary<LanguageCode, ChangeByLanguageNew<T>.LanguageOverride>();

	// Token: 0x040021AA RID: 8618
	private Vector2 originalPosition;

	// Token: 0x040021AB RID: 8619
	private bool hasStarted;

	// Token: 0x040021AC RID: 8620
	private bool hasChanged;

	// Token: 0x040021AD RID: 8621
	protected bool recorded;

	// Token: 0x040021AE RID: 8622
	private bool registeredEvents;

	// Token: 0x040021AF RID: 8623
	private bool handHeldEvents;

	// Token: 0x02001696 RID: 5782
	[Serializable]
	public sealed class LanguageOverride
	{
		// Token: 0x04008B7B RID: 35707
		public SupportedLanguages languageCode;

		// Token: 0x04008B7C RID: 35708
		public T languageOverrides;

		// Token: 0x04008B7D RID: 35709
		public T handHeldOverrides;
	}

	// Token: 0x02001697 RID: 5783
	[Serializable]
	public abstract class OverrideValue
	{
	}
}
