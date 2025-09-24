using System;
using UnityEngine;

// Token: 0x02000776 RID: 1910
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class NamedArrayAttribute : PropertyAttribute
{
	// Token: 0x1700079E RID: 1950
	// (get) Token: 0x06004401 RID: 17409 RVA: 0x0012A74C File Offset: 0x0012894C
	public string MethodName { get; }

	// Token: 0x06004402 RID: 17410 RVA: 0x0012A754 File Offset: 0x00128954
	public NamedArrayAttribute(string methodName)
	{
		this.MethodName = methodName;
	}
}
