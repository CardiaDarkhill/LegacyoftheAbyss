using System;

// Token: 0x02000756 RID: 1878
[AttributeUsage(AttributeTargets.Field)]
public class ConditionalAttribute : PropertyModifierAttribute
{
	// Token: 0x1700078B RID: 1931
	// (get) Token: 0x0600429F RID: 17055 RVA: 0x00125CC8 File Offset: 0x00123EC8
	// (set) Token: 0x060042A0 RID: 17056 RVA: 0x00125CD0 File Offset: 0x00123ED0
	public string TargetName { get; private set; }

	// Token: 0x1700078C RID: 1932
	// (get) Token: 0x060042A1 RID: 17057 RVA: 0x00125CD9 File Offset: 0x00123ED9
	// (set) Token: 0x060042A2 RID: 17058 RVA: 0x00125CE1 File Offset: 0x00123EE1
	public bool IsMethod { get; private set; }

	// Token: 0x1700078D RID: 1933
	// (get) Token: 0x060042A3 RID: 17059 RVA: 0x00125CEA File Offset: 0x00123EEA
	// (set) Token: 0x060042A4 RID: 17060 RVA: 0x00125CF2 File Offset: 0x00123EF2
	public bool ExpectedResult { get; private set; }

	// Token: 0x1700078E RID: 1934
	// (get) Token: 0x060042A5 RID: 17061 RVA: 0x00125CFB File Offset: 0x00123EFB
	// (set) Token: 0x060042A6 RID: 17062 RVA: 0x00125D03 File Offset: 0x00123F03
	public bool HideCompletely { get; private set; }

	// Token: 0x060042A7 RID: 17063 RVA: 0x00125D0C File Offset: 0x00123F0C
	public ConditionalAttribute(string targetName, bool expectedResult, bool isMethod = true, bool hideCompletely = true)
	{
		this.TargetName = targetName;
		this.ExpectedResult = expectedResult;
		this.IsMethod = isMethod;
		this.HideCompletely = hideCompletely;
	}

	// Token: 0x04004424 RID: 17444
	private bool wasEnabled;
}
