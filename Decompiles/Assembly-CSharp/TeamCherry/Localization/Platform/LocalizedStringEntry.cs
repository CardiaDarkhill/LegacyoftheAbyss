using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TeamCherry.Localization.Platform
{
	// Token: 0x020008B1 RID: 2225
	[Serializable]
	public class LocalizedStringEntry
	{
		// Token: 0x06004CEA RID: 19690 RVA: 0x001694D0 File Offset: 0x001676D0
		private string GetElementName(int index)
		{
			try
			{
				LocalizedValue localizedValue = this.values[index];
				return string.Format("{0}: {1} : {2}", index, localizedValue.locale, localizedValue.text);
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			return string.Format("Element {0}", index);
		}

		// Token: 0x06004CEB RID: 19691 RVA: 0x00169534 File Offset: 0x00167734
		public void MergeValues(LocalizedStringEntry other)
		{
			if (other == null)
			{
				return;
			}
			if (!other.localisedString.IsEmpty)
			{
				this.localisedString = other.localisedString;
			}
			this.MergeValues(other.values);
		}

		// Token: 0x06004CEC RID: 19692 RVA: 0x00169560 File Offset: 0x00167760
		private void MergeValues(List<LocalizedValue> newValues)
		{
			using (List<LocalizedValue>.Enumerator enumerator = newValues.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					LocalizedValue val = enumerator.Current;
					LocalizedValue localizedValue = this.values.FirstOrDefault((LocalizedValue v) => v.locale == val.locale);
					if (localizedValue != null)
					{
						localizedValue.text = val.text;
					}
					else
					{
						this.values.Add(val);
					}
				}
			}
		}

		// Token: 0x06004CED RID: 19693 RVA: 0x001695F4 File Offset: 0x001677F4
		private static string PrintLocalisedString(LocalisedString localisedString)
		{
			return string.Concat(new string[]
			{
				"!!",
				localisedString.Sheet,
				"/",
				localisedString.Key,
				"!!"
			});
		}

		// Token: 0x06004CEE RID: 19694 RVA: 0x0016962C File Offset: 0x0016782C
		public void AddOrUpdate(LocalizedValue value)
		{
			LocalizedValue localizedValue = this.values.FirstOrDefault((LocalizedValue v) => v.locale == value.locale);
			if (localizedValue != null)
			{
				localizedValue.text = value.text;
				return;
			}
			this.values.Add(value);
		}

		// Token: 0x04004DF9 RID: 19961
		public LocalisedString localisedString;

		// Token: 0x04004DFA RID: 19962
		public string id;

		// Token: 0x04004DFB RID: 19963
		[NamedArray("GetElementName")]
		public List<LocalizedValue> values = new List<LocalizedValue>();
	}
}
