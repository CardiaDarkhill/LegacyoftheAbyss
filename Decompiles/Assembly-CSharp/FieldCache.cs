using System;
using System.Collections.Generic;
using System.Reflection;

// Token: 0x02000188 RID: 392
public sealed class FieldCache
{
	// Token: 0x06000CC6 RID: 3270 RVA: 0x00038E56 File Offset: 0x00037056
	public FieldCache(Type type)
	{
		this.type = type;
	}

	// Token: 0x06000CC7 RID: 3271 RVA: 0x00038E70 File Offset: 0x00037070
	public FieldCache(Type type, bool useBindingFlags, BindingFlags bindingFlags)
	{
		this.type = type;
		this.useBindingFlags = useBindingFlags;
		this.bindingFlags = bindingFlags;
	}

	// Token: 0x06000CC8 RID: 3272 RVA: 0x00038E98 File Offset: 0x00037098
	private bool TryGetField(string variableName, out FieldCache.Cache fieldCache)
	{
		if (!this.fieldInfos.TryGetValue(variableName, out fieldCache))
		{
			if (this.useBindingFlags)
			{
				fieldCache = new FieldCache.Cache(this.type.GetField(variableName, this.bindingFlags));
			}
			else
			{
				fieldCache = new FieldCache.Cache(this.type.GetField(variableName));
			}
			this.fieldInfos.Add(variableName, fieldCache);
		}
		return true;
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x00038EFC File Offset: 0x000370FC
	public T GetValue<T>(string variableName)
	{
		FieldCache.Cache cache;
		if (this.TryGetField(variableName, out cache) && cache.isValid)
		{
			FieldInfo fieldInfo = cache.fieldInfo;
			if (fieldInfo.FieldType == typeof(T))
			{
				return (T)((object)fieldInfo.GetRawConstantValue());
			}
		}
		return default(T);
	}

	// Token: 0x04000C37 RID: 3127
	private readonly Type type;

	// Token: 0x04000C38 RID: 3128
	private bool useBindingFlags;

	// Token: 0x04000C39 RID: 3129
	private BindingFlags bindingFlags;

	// Token: 0x04000C3A RID: 3130
	private readonly Dictionary<string, FieldCache.Cache> fieldInfos = new Dictionary<string, FieldCache.Cache>();

	// Token: 0x020014B5 RID: 5301
	private sealed class Cache
	{
		// Token: 0x06008465 RID: 33893 RVA: 0x0026BA49 File Offset: 0x00269C49
		public Cache(FieldInfo fieldInfo)
		{
			this.fieldInfo = fieldInfo;
			this.isValid = (fieldInfo != null);
		}

		// Token: 0x04008454 RID: 33876
		public bool isValid;

		// Token: 0x04008455 RID: 33877
		public FieldInfo fieldInfo;
	}
}
