using System;
using UnityEngine;

// Token: 0x0200077B RID: 1915
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class PlayerDataFieldAttribute : PropertyAttribute
{
	// Token: 0x1700079F RID: 1951
	// (get) Token: 0x0600440B RID: 17419 RVA: 0x0012A888 File Offset: 0x00128A88
	// (set) Token: 0x0600440C RID: 17420 RVA: 0x0012A890 File Offset: 0x00128A90
	public Type FieldType { get; private set; }

	// Token: 0x170007A0 RID: 1952
	// (get) Token: 0x0600440D RID: 17421 RVA: 0x0012A899 File Offset: 0x00128A99
	// (set) Token: 0x0600440E RID: 17422 RVA: 0x0012A8A1 File Offset: 0x00128AA1
	public bool IsRequired { get; private set; }

	// Token: 0x0600440F RID: 17423 RVA: 0x0012A8AA File Offset: 0x00128AAA
	public PlayerDataFieldAttribute(Type fieldType, bool isRequired = true)
	{
		this.FieldType = fieldType;
		this.IsRequired = isRequired;
	}
}
