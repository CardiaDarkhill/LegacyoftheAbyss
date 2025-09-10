using System;
using UnityEngine;

// Token: 0x02000771 RID: 1905
[AttributeUsage(AttributeTargets.Field)]
public class InspectorValidationAttribute : PropertyModifierAttribute
{
	// Token: 0x1700079C RID: 1948
	// (get) Token: 0x060043F7 RID: 17399 RVA: 0x0012A6E6 File Offset: 0x001288E6
	// (set) Token: 0x060043F8 RID: 17400 RVA: 0x0012A6EE File Offset: 0x001288EE
	public string MethodName { get; private set; }

	// Token: 0x060043F9 RID: 17401 RVA: 0x0012A6F7 File Offset: 0x001288F7
	public InspectorValidationAttribute(string methodName)
	{
		this.MethodName = methodName;
	}

	// Token: 0x060043FA RID: 17402 RVA: 0x0012A706 File Offset: 0x00128906
	public InspectorValidationAttribute()
	{
		this.MethodName = null;
	}

	// Token: 0x04004544 RID: 17732
	private Color initialColor;
}
