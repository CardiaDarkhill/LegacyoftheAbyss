using System;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000407 RID: 1031
public sealed class ChangePositionByLanguage : MonoBehaviour
{
	// Token: 0x06002306 RID: 8966 RVA: 0x000A0454 File Offset: 0x0009E654
	private void Awake()
	{
		foreach (ChangePositionByLanguage.Override @override in this.offsetOverrides)
		{
			LanguageCode languageCode = (LanguageCode)@override.languageCode;
			if (!this.languageCodeOverrides.ContainsKey(languageCode))
			{
				this.languageCodeOverrides[languageCode] = @override;
			}
		}
		this.originalPosition = base.transform.localPosition;
	}

	// Token: 0x06002307 RID: 8967 RVA: 0x000A04D4 File Offset: 0x0009E6D4
	private void Start()
	{
		this.hasStarted = true;
		this.DoOffset();
	}

	// Token: 0x06002308 RID: 8968 RVA: 0x000A04E4 File Offset: 0x0009E6E4
	private void OnValidate()
	{
		if (Application.isPlaying)
		{
			foreach (ChangePositionByLanguage.Override @override in this.offsetOverrides)
			{
				LanguageCode languageCode = (LanguageCode)@override.languageCode;
				this.languageCodeOverrides[languageCode] = @override;
			}
		}
	}

	// Token: 0x06002309 RID: 8969 RVA: 0x000A054C File Offset: 0x0009E74C
	private void OnEnable()
	{
		if (this.hasStarted && (!this.onStartOnly || CheatManager.ForceLanguageComponentUpdates))
		{
			this.DoOffset();
		}
		this.RegisterEvents();
	}

	// Token: 0x0600230A RID: 8970 RVA: 0x000A0571 File Offset: 0x0009E771
	private void OnDisable()
	{
		this.UnregisterEvents();
	}

	// Token: 0x0600230B RID: 8971 RVA: 0x000A057C File Offset: 0x0009E77C
	private void RegisterEvents()
	{
		if (!this.registeredEvents)
		{
			this.registeredEvents = true;
			GameManager instance = GameManager.instance;
			if (instance != null)
			{
				instance.RefreshLanguageText += this.DoOffset;
			}
			if (this.doHandHeldUpdate)
			{
				Platform.Current.OnScreenModeChanged += this.OnScreenModeChanged;
				this.handHeldEvents = true;
			}
		}
	}

	// Token: 0x0600230C RID: 8972 RVA: 0x000A05E0 File Offset: 0x0009E7E0
	private void UnregisterEvents()
	{
		if (this.registeredEvents)
		{
			this.registeredEvents = false;
			GameManager instance = GameManager.instance;
			if (instance != null)
			{
				this.registeredEvents = true;
				instance.RefreshLanguageText -= this.DoOffset;
			}
			if (this.handHeldEvents)
			{
				Platform.Current.OnScreenModeChanged -= this.OnScreenModeChanged;
				this.handHeldEvents = false;
			}
		}
	}

	// Token: 0x0600230D RID: 8973 RVA: 0x000A0649 File Offset: 0x0009E849
	private void OnScreenModeChanged(Platform.ScreenModeState screenMode)
	{
		if (this.handHeldEvents)
		{
			this.DoOffset();
		}
	}

	// Token: 0x0600230E RID: 8974 RVA: 0x000A065C File Offset: 0x0009E85C
	public void DoOffset()
	{
		LanguageCode key = Language.CurrentLanguage();
		Vector3 localPosition = this.originalPosition;
		ChangePositionByLanguage.Override @override;
		if (this.languageCodeOverrides.TryGetValue(key, out @override))
		{
			if (@override.xOverride.IsEnabled)
			{
				localPosition.x = @override.xOverride.Value;
			}
			if (@override.yOverride.IsEnabled)
			{
				localPosition.y = @override.yOverride.Value;
			}
			if (this.doHandHeldUpdate && Platform.Current.IsRunningOnHandHeld && Platform.Current.IsTargetHandHeld(@override.handHeldOverride.targetHandHeld))
			{
				if (@override.handHeldOverride.xOverride.IsEnabled)
				{
					localPosition.x = @override.handHeldOverride.xOverride.Value;
				}
				if (@override.handHeldOverride.yOverride.IsEnabled)
				{
					localPosition.y = @override.handHeldOverride.yOverride.Value;
				}
			}
		}
		else if (this.doHandHeldUpdate && Platform.Current.IsRunningOnHandHeld && Platform.Current.IsTargetHandHeld(this.handHeldOverride.targetHandHeld))
		{
			if (this.handHeldOverride.xOverride.IsEnabled)
			{
				localPosition.x = this.handHeldOverride.xOverride.Value;
			}
			if (this.handHeldOverride.yOverride.IsEnabled)
			{
				localPosition.y = this.handHeldOverride.yOverride.Value;
			}
		}
		base.transform.localPosition = localPosition;
	}

	// Token: 0x0600230F RID: 8975 RVA: 0x000A07DB File Offset: 0x0009E9DB
	[ContextMenu("Print Local Position")]
	private void PrintLocalPosition()
	{
		Debug.Log(string.Format("{0} local position: {1}", this, base.transform.localPosition), this);
	}

	// Token: 0x040021BD RID: 8637
	[SerializeField]
	private List<ChangePositionByLanguage.Override> offsetOverrides = new List<ChangePositionByLanguage.Override>();

	// Token: 0x040021BE RID: 8638
	[SerializeField]
	private bool onStartOnly;

	// Token: 0x040021BF RID: 8639
	[SerializeField]
	private bool doHandHeldUpdate;

	// Token: 0x040021C0 RID: 8640
	[SerializeField]
	private ChangePositionByLanguage.HandHeldOverride handHeldOverride;

	// Token: 0x040021C1 RID: 8641
	private Dictionary<LanguageCode, ChangePositionByLanguage.Override> languageCodeOverrides = new Dictionary<LanguageCode, ChangePositionByLanguage.Override>();

	// Token: 0x040021C2 RID: 8642
	private Vector3 originalPosition;

	// Token: 0x040021C3 RID: 8643
	private bool hasStarted;

	// Token: 0x040021C4 RID: 8644
	private bool registeredEvents;

	// Token: 0x040021C5 RID: 8645
	private bool handHeldEvents;

	// Token: 0x0200169B RID: 5787
	[Serializable]
	private struct Override
	{
		// Token: 0x04008B80 RID: 35712
		public SupportedLanguages languageCode;

		// Token: 0x04008B81 RID: 35713
		public OverrideFloat xOverride;

		// Token: 0x04008B82 RID: 35714
		public OverrideFloat yOverride;

		// Token: 0x04008B83 RID: 35715
		public ChangePositionByLanguage.HandHeldOverride handHeldOverride;
	}

	// Token: 0x0200169C RID: 5788
	[Serializable]
	private struct HandHeldOverride
	{
		// Token: 0x04008B84 RID: 35716
		public Platform.HandHeldTypes targetHandHeld;

		// Token: 0x04008B85 RID: 35717
		public OverrideFloat xOverride;

		// Token: 0x04008B86 RID: 35718
		public OverrideFloat yOverride;
	}
}
