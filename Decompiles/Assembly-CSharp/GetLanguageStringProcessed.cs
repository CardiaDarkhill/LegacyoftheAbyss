using System;
using HutongGames.PlayMaker;
using TeamCherry.Localization;

// Token: 0x0200040D RID: 1037
[ActionCategory("Game Text")]
public class GetLanguageStringProcessed : FsmStateAction
{
	// Token: 0x0600232E RID: 9006 RVA: 0x000A0EF4 File Offset: 0x0009F0F4
	public override void Reset()
	{
		this.sheetName = null;
		this.convName = null;
		this.storeValue = null;
		this.fontSource = null;
	}

	// Token: 0x0600232F RID: 9007 RVA: 0x000A0F12 File Offset: 0x0009F112
	public override void Awake()
	{
		this.fontSource.Value = this.fontSource.Value;
	}

	// Token: 0x06002330 RID: 9008 RVA: 0x000A0F2C File Offset: 0x0009F12C
	public override void OnEnter()
	{
		string text = Language.Get(this.convName.Value, this.sheetName.Value);
		text = text.Replace("<br>", "\n");
		this.storeValue.Value = text.GetProcessed((LocalisationHelper.FontSource)this.fontSource.Value);
		base.Finish();
	}

	// Token: 0x040021D7 RID: 8663
	[RequiredField]
	public FsmString sheetName;

	// Token: 0x040021D8 RID: 8664
	[RequiredField]
	public FsmString convName;

	// Token: 0x040021D9 RID: 8665
	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmString storeValue;

	// Token: 0x040021DA RID: 8666
	[ObjectType(typeof(LocalisationHelper.FontSource))]
	public FsmEnum fontSource;
}
