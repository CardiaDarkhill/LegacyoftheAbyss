using System;
using TeamCherry.Localization;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001284 RID: 4740
	public class ShowCollectableUIMsg : FsmStateAction
	{
		// Token: 0x06007CB0 RID: 31920 RVA: 0x00254270 File Offset: 0x00252470
		public override void Reset()
		{
			this.TranslationSheet = null;
			this.TranslationKey = null;
			this.Icon = null;
			this.IconScale = 1f;
		}

		// Token: 0x06007CB1 RID: 31921 RVA: 0x00254298 File Offset: 0x00252498
		public override void OnEnter()
		{
			CollectableUIMsg.Spawn(new ShowCollectableUIMsg.UIMsgData
			{
				Name = new LocalisedString(this.TranslationSheet.Value, this.TranslationKey.Value),
				Icon = (this.Icon.Value as Sprite),
				IconScale = this.IconScale.Value
			}, null, false);
			base.Finish();
		}

		// Token: 0x04007CCA RID: 31946
		[RequiredField]
		public FsmString TranslationSheet;

		// Token: 0x04007CCB RID: 31947
		[RequiredField]
		public FsmString TranslationKey;

		// Token: 0x04007CCC RID: 31948
		[ObjectType(typeof(Sprite))]
		[RequiredField]
		public FsmObject Icon;

		// Token: 0x04007CCD RID: 31949
		[RequiredField]
		public FsmFloat IconScale;

		// Token: 0x02001BE9 RID: 7145
		private struct UIMsgData : ICollectableUIMsgItem, IUIMsgPopupItem
		{
			// Token: 0x06009A83 RID: 39555 RVA: 0x002B3E20 File Offset: 0x002B2020
			public float GetUIMsgIconScale()
			{
				return this.IconScale;
			}

			// Token: 0x06009A84 RID: 39556 RVA: 0x002B3E28 File Offset: 0x002B2028
			public bool HasUpgradeIcon()
			{
				return false;
			}

			// Token: 0x06009A85 RID: 39557 RVA: 0x002B3E2B File Offset: 0x002B202B
			public string GetUIMsgName()
			{
				return this.Name;
			}

			// Token: 0x06009A86 RID: 39558 RVA: 0x002B3E33 File Offset: 0x002B2033
			public Sprite GetUIMsgSprite()
			{
				return this.Icon;
			}

			// Token: 0x06009A87 RID: 39559 RVA: 0x002B3E3B File Offset: 0x002B203B
			public Object GetRepresentingObject()
			{
				return null;
			}

			// Token: 0x04009F88 RID: 40840
			public string Name;

			// Token: 0x04009F89 RID: 40841
			public Sprite Icon;

			// Token: 0x04009F8A RID: 40842
			public float IconScale;
		}
	}
}
