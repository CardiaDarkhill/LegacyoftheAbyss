using System;

// Token: 0x020007DA RID: 2010
[Serializable]
public class SecureplayerPrefsDemoClass
{
	// Token: 0x170007FD RID: 2045
	// (get) Token: 0x0600469F RID: 18079 RVA: 0x0013B09C File Offset: 0x0013929C
	// (set) Token: 0x060046A0 RID: 18080 RVA: 0x0013B0A4 File Offset: 0x001392A4
	public string playID { get; set; }

	// Token: 0x170007FE RID: 2046
	// (get) Token: 0x060046A1 RID: 18081 RVA: 0x0013B0AD File Offset: 0x001392AD
	// (set) Token: 0x060046A2 RID: 18082 RVA: 0x0013B0B5 File Offset: 0x001392B5
	public int type { get; set; }

	// Token: 0x170007FF RID: 2047
	// (get) Token: 0x060046A3 RID: 18083 RVA: 0x0013B0BE File Offset: 0x001392BE
	// (set) Token: 0x060046A4 RID: 18084 RVA: 0x0013B0C6 File Offset: 0x001392C6
	public bool incremental { get; set; }

	// Token: 0x060046A5 RID: 18085 RVA: 0x0013B0CF File Offset: 0x001392CF
	public SecureplayerPrefsDemoClass()
	{
		this.playID = "";
		this.type = 0;
		this.incremental = false;
	}
}
