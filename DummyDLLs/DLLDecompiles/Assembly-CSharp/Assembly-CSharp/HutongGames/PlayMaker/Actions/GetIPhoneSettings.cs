using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E89 RID: 3721
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Get various iPhone settings.")]
	public class GetIPhoneSettings : FsmStateAction
	{
		// Token: 0x060069C3 RID: 27075 RVA: 0x002115C0 File Offset: 0x0020F7C0
		public override void Reset()
		{
			this.getScreenCanDarken = null;
			this.getUniqueIdentifier = null;
			this.getName = null;
			this.getModel = null;
			this.getSystemName = null;
			this.getGeneration = null;
		}

		// Token: 0x060069C4 RID: 27076 RVA: 0x002115EC File Offset: 0x0020F7EC
		public override void OnEnter()
		{
			base.Finish();
		}

		// Token: 0x040068EB RID: 26859
		[UIHint(UIHint.Variable)]
		[Tooltip("Allows device to fall into 'sleep' state with screen being dim if no touches occurred. Default value is true.")]
		public FsmBool getScreenCanDarken;

		// Token: 0x040068EC RID: 26860
		[UIHint(UIHint.Variable)]
		[Tooltip("A unique device identifier string. It is guaranteed to be unique for every device (Read Only).")]
		public FsmString getUniqueIdentifier;

		// Token: 0x040068ED RID: 26861
		[UIHint(UIHint.Variable)]
		[Tooltip("The user defined name of the device (Read Only).")]
		public FsmString getName;

		// Token: 0x040068EE RID: 26862
		[UIHint(UIHint.Variable)]
		[Tooltip("The model of the device (Read Only).")]
		public FsmString getModel;

		// Token: 0x040068EF RID: 26863
		[UIHint(UIHint.Variable)]
		[Tooltip("The name of the operating system running on the device (Read Only).")]
		public FsmString getSystemName;

		// Token: 0x040068F0 RID: 26864
		[UIHint(UIHint.Variable)]
		[Tooltip("The generation of the device (Read Only).")]
		public FsmString getGeneration;
	}
}
