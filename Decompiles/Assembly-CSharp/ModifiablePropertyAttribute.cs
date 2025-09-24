using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000774 RID: 1908
[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class ModifiablePropertyAttribute : PropertyAttribute
{
	// Token: 0x04004546 RID: 17734
	public List<PropertyModifierAttribute> modifiers;
}
