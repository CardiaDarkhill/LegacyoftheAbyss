using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001288 RID: 4744
	public class GetRandomLocalisedString : FsmStateAction
	{
		// Token: 0x06007CBC RID: 31932 RVA: 0x002543CB File Offset: 0x002525CB
		public bool IsUsingCollection()
		{
			return this.Collection.Value;
		}

		// Token: 0x06007CBD RID: 31933 RVA: 0x002543DD File Offset: 0x002525DD
		public override void Reset()
		{
			this.Collection = null;
			this.Template = null;
			this.StoreString = null;
		}

		// Token: 0x06007CBE RID: 31934 RVA: 0x002543F4 File Offset: 0x002525F4
		public override void OnEnter()
		{
			LocalisedTextCollection localisedTextCollection = this.Collection.Value as LocalisedTextCollection;
			ILocalisedTextCollection localisedTextCollection2;
			if (localisedTextCollection)
			{
				localisedTextCollection2 = localisedTextCollection;
			}
			else
			{
				localisedTextCollection2 = new LocalisedTextCollectionData(this.Template);
			}
			this.StoreString.Value = localisedTextCollection2.GetRandom(default(LocalisedString));
			base.Finish();
		}

		// Token: 0x04007CD2 RID: 31954
		[ObjectType(typeof(LocalisedTextCollection))]
		public FsmObject Collection;

		// Token: 0x04007CD3 RID: 31955
		[HideIf("IsUsingCollection")]
		public LocalisedFsmString Template;

		// Token: 0x04007CD4 RID: 31956
		[UIHint(UIHint.Variable)]
		public FsmString StoreString;
	}
}
