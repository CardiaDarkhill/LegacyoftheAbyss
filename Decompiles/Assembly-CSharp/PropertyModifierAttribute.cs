using System;

// Token: 0x02000773 RID: 1907
[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
public abstract class PropertyModifierAttribute : Attribute
{
	// Token: 0x1700079D RID: 1949
	// (get) Token: 0x060043FC RID: 17404 RVA: 0x0012A715 File Offset: 0x00128915
	// (set) Token: 0x060043FD RID: 17405 RVA: 0x0012A71D File Offset: 0x0012891D
	public int order { get; set; }
}
