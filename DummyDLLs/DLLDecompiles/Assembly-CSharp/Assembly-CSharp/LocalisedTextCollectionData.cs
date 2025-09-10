using System;
using System.Linq;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020001DC RID: 476
[Serializable]
public class LocalisedTextCollectionData : ILocalisedTextCollection
{
	// Token: 0x0600128C RID: 4748 RVA: 0x00056358 File Offset: 0x00054558
	public LocalisedTextCollectionData()
	{
		this.template = default(LocalisedString);
	}

	// Token: 0x0600128D RID: 4749 RVA: 0x00056373 File Offset: 0x00054573
	public LocalisedTextCollectionData(LocalisedString template)
	{
		this.template = template;
	}

	// Token: 0x1700020A RID: 522
	// (get) Token: 0x0600128E RID: 4750 RVA: 0x00056389 File Offset: 0x00054589
	public bool IsActive
	{
		get
		{
			return !this.doNotPlay;
		}
	}

	// Token: 0x0600128F RID: 4751 RVA: 0x00056394 File Offset: 0x00054594
	private void Init()
	{
		string sheetTitle = this.template.Sheet;
		string templateKey = this.template.Key;
		if (Language.HasSheet(sheetTitle))
		{
			this.currentTexts = (from key in Language.GetKeys(sheetTitle).Where(delegate(string key)
			{
				if (!key.StartsWith(templateKey, StringComparison.Ordinal))
				{
					return false;
				}
				for (int i = templateKey.Length; i < key.Length; i++)
				{
					char c = key[i];
					if (c != '_' && !char.IsDigit(c))
					{
						return false;
					}
				}
				return true;
			})
			select new LocalisedTextCollectionData.ProbabilityLocalisedString(new LocalisedString(sheetTitle, key))).ToArray<LocalisedTextCollectionData.ProbabilityLocalisedString>();
		}
		else
		{
			Debug.LogErrorFormat("Localisation Sheet: \"{0}\" does not exist!", new object[]
			{
				sheetTitle
			});
		}
		if (this.currentTexts == null || this.currentTexts.Length == 0)
		{
			this.currentTexts = new LocalisedTextCollectionData.ProbabilityLocalisedString[]
			{
				new LocalisedTextCollectionData.ProbabilityLocalisedString(this.template)
			};
		}
		this.isInitialised = true;
	}

	// Token: 0x06001290 RID: 4752 RVA: 0x0005645C File Offset: 0x0005465C
	public LocalisedTextCollectionData ResolveAlternatives()
	{
		if (this.alternatives == null)
		{
			return this;
		}
		LocalisedTextCollectionData.Alternative[] array = this.alternatives;
		int i = 0;
		while (i < array.Length)
		{
			LocalisedTextCollectionData.Alternative alternative = array[i];
			if (alternative.Condition.IsFulfilled)
			{
				LocalisedTextCollection collection = alternative.Collection;
				if (!(collection != null))
				{
					return null;
				}
				return collection.ResolveAlternatives();
			}
			else
			{
				i++;
			}
		}
		return this;
	}

	// Token: 0x06001291 RID: 4753 RVA: 0x000564B4 File Offset: 0x000546B4
	public LocalisedString GetRandom(LocalisedString skipString)
	{
		LocalisedTextCollectionData localisedTextCollectionData = this.ResolveAlternatives();
		if (localisedTextCollectionData != this)
		{
			return localisedTextCollectionData.GetRandom(skipString);
		}
		if (!this.isInitialised)
		{
			this.Init();
		}
		int i = (this.currentTexts.Length > 1) ? Mathf.Min(10, this.currentTexts.Length) : 1;
		int num = this.previousIndex;
		LocalisedString localisedString = default(LocalisedString);
		while (i > 0)
		{
			i--;
			localisedString = Probability.GetRandomItemByProbabilityFair<LocalisedTextCollectionData.ProbabilityLocalisedString, LocalisedString>(this.currentTexts, out num, ref this.currentProbabilities, 2f, null);
			if (num != this.previousIndex && (skipString.IsEmpty || !(localisedString == skipString)))
			{
				break;
			}
		}
		this.previousIndex = num;
		return localisedString;
	}

	// Token: 0x06001292 RID: 4754 RVA: 0x00056560 File Offset: 0x00054760
	public NeedolinTextConfig GetConfig()
	{
		LocalisedTextCollectionData localisedTextCollectionData = this.ResolveAlternatives();
		if (localisedTextCollectionData == this)
		{
			return this.configOverride;
		}
		return localisedTextCollectionData.configOverride;
	}

	// Token: 0x04001158 RID: 4440
	[SerializeField]
	[LocalisedString.NoKeyValidation]
	private LocalisedString template;

	// Token: 0x04001159 RID: 4441
	[SerializeField]
	[AssetPickerDropdown]
	private NeedolinTextConfig configOverride;

	// Token: 0x0400115A RID: 4442
	[Space]
	[SerializeField]
	private LocalisedTextCollectionData.Alternative[] alternatives;

	// Token: 0x0400115B RID: 4443
	[Space]
	[SerializeField]
	private bool doNotPlay;

	// Token: 0x0400115C RID: 4444
	private LocalisedTextCollectionData.ProbabilityLocalisedString[] currentTexts;

	// Token: 0x0400115D RID: 4445
	private float[] currentProbabilities;

	// Token: 0x0400115E RID: 4446
	private int previousIndex = -1;

	// Token: 0x0400115F RID: 4447
	private bool isInitialised;

	// Token: 0x0200151C RID: 5404
	[Serializable]
	private class ProbabilityLocalisedString : Probability.ProbabilityBase<LocalisedString>
	{
		// Token: 0x060085CB RID: 34251 RVA: 0x002712C8 File Offset: 0x0026F4C8
		public ProbabilityLocalisedString()
		{
			this.text = default(LocalisedString);
		}

		// Token: 0x060085CC RID: 34252 RVA: 0x002712DC File Offset: 0x0026F4DC
		public ProbabilityLocalisedString(LocalisedString text)
		{
			this.text = text;
		}

		// Token: 0x17000D5B RID: 3419
		// (get) Token: 0x060085CD RID: 34253 RVA: 0x002712EB File Offset: 0x0026F4EB
		public override LocalisedString Item
		{
			get
			{
				return this.text;
			}
		}

		// Token: 0x040085F7 RID: 34295
		[SerializeField]
		private LocalisedString text;
	}

	// Token: 0x0200151D RID: 5405
	[Serializable]
	private class Alternative
	{
		// Token: 0x040085F8 RID: 34296
		public LocalisedTextCollection Collection;

		// Token: 0x040085F9 RID: 34297
		public PlayerDataTest Condition;
	}
}
