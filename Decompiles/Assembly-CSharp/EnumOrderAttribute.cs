using System;

// Token: 0x0200075D RID: 1885
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class EnumOrderAttribute : Attribute
{
	// Token: 0x1700078F RID: 1935
	// (get) Token: 0x060042B6 RID: 17078 RVA: 0x00126047 File Offset: 0x00124247
	// (set) Token: 0x060042B7 RID: 17079 RVA: 0x0012604F File Offset: 0x0012424F
	public int Order { get; private set; }

	// Token: 0x060042B8 RID: 17080 RVA: 0x00126058 File Offset: 0x00124258
	public EnumOrderAttribute(int order)
	{
		this.Order = order;
	}
}
