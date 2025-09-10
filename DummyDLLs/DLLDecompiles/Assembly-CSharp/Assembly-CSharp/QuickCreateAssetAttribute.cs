using System;
using UnityEngine;

// Token: 0x020006FD RID: 1789
public class QuickCreateAssetAttribute : PropertyAttribute
{
	// Token: 0x1700074D RID: 1869
	// (get) Token: 0x06004002 RID: 16386 RVA: 0x0011A46A File Offset: 0x0011866A
	// (set) Token: 0x06004003 RID: 16387 RVA: 0x0011A472 File Offset: 0x00118672
	public string FolderPath { get; private set; }

	// Token: 0x1700074E RID: 1870
	// (get) Token: 0x06004004 RID: 16388 RVA: 0x0011A47B File Offset: 0x0011867B
	// (set) Token: 0x06004005 RID: 16389 RVA: 0x0011A483 File Offset: 0x00118683
	public string SourceField { get; private set; }

	// Token: 0x1700074F RID: 1871
	// (get) Token: 0x06004006 RID: 16390 RVA: 0x0011A48C File Offset: 0x0011868C
	// (set) Token: 0x06004007 RID: 16391 RVA: 0x0011A494 File Offset: 0x00118694
	public string TargetField { get; private set; }

	// Token: 0x06004008 RID: 16392 RVA: 0x0011A49D File Offset: 0x0011869D
	public QuickCreateAssetAttribute(string folderPath, string sourceField, string targetField)
	{
		this.FolderPath = folderPath;
		this.SourceField = sourceField;
		this.TargetField = targetField;
	}
}
