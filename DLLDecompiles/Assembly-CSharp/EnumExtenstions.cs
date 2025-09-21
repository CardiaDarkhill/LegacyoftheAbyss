using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// Token: 0x0200075E RID: 1886
public static class EnumExtenstions
{
	// Token: 0x060042B9 RID: 17081 RVA: 0x00126067 File Offset: 0x00124267
	public static IEnumerable<string> GetNamesWithOrder(this Enum enumVal)
	{
		return enumVal.GetType().GetNamesWithOrder();
	}

	// Token: 0x060042BA RID: 17082 RVA: 0x00126074 File Offset: 0x00124274
	public static IEnumerable<string> GetNamesWithOrder(this Type type)
	{
		if (!type.IsEnum)
		{
			throw new ArgumentException("Type must be an enum");
		}
		return from field in type.GetFields()
		where field.IsStatic
		select new
		{
			field = field,
			attribute = field.GetCustomAttributes(false).OfType<EnumOrderAttribute>().FirstOrDefault<EnumOrderAttribute>()
		} into fieldInfo
		select new
		{
			name = fieldInfo.field.Name,
			order = ((fieldInfo.attribute != null) ? fieldInfo.attribute.Order : 0)
		} into field
		orderby field.order
		select field.name;
	}

	// Token: 0x060042BB RID: 17083 RVA: 0x0012614E File Offset: 0x0012434E
	public static IEnumerable<int> GetValuesWithOrder(this Enum enumVal)
	{
		return enumVal.GetType().GetValuesWithOrder();
	}

	// Token: 0x060042BC RID: 17084 RVA: 0x0012615C File Offset: 0x0012435C
	public static IEnumerable<int> GetValuesWithOrder(this Type type)
	{
		if (!type.IsEnum)
		{
			throw new ArgumentException("Type must be an enum");
		}
		return from field in type.GetFields()
		where field.IsStatic
		select new
		{
			field = field,
			attribute = field.GetCustomAttributes(false).OfType<EnumOrderAttribute>().FirstOrDefault<EnumOrderAttribute>()
		} into fieldInfo
		select new
		{
			value = (int)fieldInfo.field.GetRawConstantValue(),
			order = ((fieldInfo.attribute != null) ? fieldInfo.attribute.Order : 0)
		} into field
		orderby field.order
		select field.value;
	}
}
