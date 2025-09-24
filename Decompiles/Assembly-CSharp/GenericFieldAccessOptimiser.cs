using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x02000185 RID: 389
public sealed class GenericFieldAccessOptimiser
{
	// Token: 0x06000CB3 RID: 3251 RVA: 0x00038960 File Offset: 0x00036B60
	public void SetField<T>(object target, string fieldName, T value)
	{
		GenericFieldAccessOptimiser.FieldAccessInfo<T> fieldAccessInfo;
		if (!this.EnsureCompiled<T>(target, fieldName, out fieldAccessInfo))
		{
			this.SetFieldUsingReflection<T>(target, (fieldAccessInfo != null) ? fieldAccessInfo.fieldInfo : null, value);
			return;
		}
		if (!Platform.UseFieldInfoCache)
		{
			try
			{
				fieldAccessInfo.Setter(target, value);
			}
			catch (Exception)
			{
			}
			return;
		}
		FieldInfo fieldInfo = fieldAccessInfo.fieldInfo;
		if (fieldInfo == null)
		{
			return;
		}
		try
		{
			fieldInfo.SetValue(target, value);
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x06000CB4 RID: 3252 RVA: 0x000389EC File Offset: 0x00036BEC
	public T GetField<T>(object target, string fieldName)
	{
		GenericFieldAccessOptimiser.FieldAccessInfo<T> fieldAccessInfo;
		if (!this.EnsureCompiled<T>(target, fieldName, out fieldAccessInfo))
		{
			return this.GetFieldUsingReflection<T>(target, (fieldAccessInfo != null) ? fieldAccessInfo.fieldInfo : null);
		}
		T result;
		if (Platform.UseFieldInfoCache)
		{
			FieldInfo fieldInfo = fieldAccessInfo.fieldInfo;
			if (fieldInfo == null)
			{
				result = default(T);
				return result;
			}
			try
			{
				return (T)((object)fieldInfo.GetValue(target));
			}
			catch (Exception)
			{
				return default(T);
			}
		}
		try
		{
			result = fieldAccessInfo.Getter(target);
		}
		catch (Exception)
		{
			result = default(T);
		}
		return result;
	}

	// Token: 0x06000CB5 RID: 3253 RVA: 0x00038A94 File Offset: 0x00036C94
	private bool EnsureCompiled<T>(Type targetType, string fieldName, out GenericFieldAccessOptimiser.FieldAccessInfo<T> info)
	{
		info = null;
		object obj;
		if (!this.accessCache.TryGetValue(fieldName, out obj))
		{
			try
			{
				obj = new GenericFieldAccessOptimiser.FieldAccessInfo<T>(targetType, fieldName);
				this.accessCache[fieldName] = obj;
			}
			catch (Exception)
			{
				return false;
			}
		}
		bool result;
		try
		{
			info = (GenericFieldAccessOptimiser.FieldAccessInfo<T>)obj;
			result = info.IsCompiled;
		}
		catch (Exception)
		{
			result = false;
		}
		return result;
	}

	// Token: 0x06000CB6 RID: 3254 RVA: 0x00038B08 File Offset: 0x00036D08
	private bool EnsureCompiled<T>(object target, string fieldName, out GenericFieldAccessOptimiser.FieldAccessInfo<T> info)
	{
		info = null;
		object obj;
		if (!this.accessCache.TryGetValue(fieldName, out obj))
		{
			try
			{
				obj = new GenericFieldAccessOptimiser.FieldAccessInfo<T>(target, fieldName);
				this.accessCache[fieldName] = obj;
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				return false;
			}
		}
		bool result;
		try
		{
			info = (GenericFieldAccessOptimiser.FieldAccessInfo<T>)obj;
			result = info.IsCompiled;
		}
		catch (Exception)
		{
			result = false;
		}
		return result;
	}

	// Token: 0x06000CB7 RID: 3255 RVA: 0x00038B80 File Offset: 0x00036D80
	private bool SetFieldUsingReflection<T>(object target, FieldInfo fieldInfo, T value)
	{
		if (fieldInfo == null)
		{
			return false;
		}
		bool result;
		try
		{
			fieldInfo.SetValue(target, value);
			result = true;
		}
		catch (Exception)
		{
			result = false;
		}
		return result;
	}

	// Token: 0x06000CB8 RID: 3256 RVA: 0x00038BC0 File Offset: 0x00036DC0
	private T GetFieldUsingReflection<T>(object target, FieldInfo fieldInfo)
	{
		T result;
		if (fieldInfo == null)
		{
			result = default(T);
			return result;
		}
		try
		{
			result = (T)((object)fieldInfo.GetValue(target));
		}
		catch (Exception)
		{
			result = default(T);
		}
		return result;
	}

	// Token: 0x06000CB9 RID: 3257 RVA: 0x00038C10 File Offset: 0x00036E10
	public bool FieldExists<T>(Type targetType, string fieldName)
	{
		GenericFieldAccessOptimiser.FieldAccessInfo<T> fieldAccessInfo;
		this.EnsureCompiled<T>(targetType, fieldName, out fieldAccessInfo);
		return fieldAccessInfo != null && fieldAccessInfo.fieldInfo != null;
	}

	// Token: 0x06000CBA RID: 3258 RVA: 0x00038C3C File Offset: 0x00036E3C
	public bool FieldExists<T>(object target, string fieldName)
	{
		GenericFieldAccessOptimiser.FieldAccessInfo<T> fieldAccessInfo;
		this.EnsureCompiled<T>(target, fieldName, out fieldAccessInfo);
		return fieldAccessInfo != null && fieldAccessInfo.fieldInfo != null;
	}

	// Token: 0x04000C35 RID: 3125
	private readonly Dictionary<string, object> accessCache = new Dictionary<string, object>();

	// Token: 0x020014B3 RID: 5299
	private sealed class FieldAccessInfo<T>
	{
		// Token: 0x17000D02 RID: 3330
		// (get) Token: 0x06008457 RID: 33879 RVA: 0x0026B598 File Offset: 0x00269798
		// (set) Token: 0x06008458 RID: 33880 RVA: 0x0026B5A0 File Offset: 0x002697A0
		public Action<object, T> Setter { get; private set; }

		// Token: 0x17000D03 RID: 3331
		// (get) Token: 0x06008459 RID: 33881 RVA: 0x0026B5A9 File Offset: 0x002697A9
		// (set) Token: 0x0600845A RID: 33882 RVA: 0x0026B5B1 File Offset: 0x002697B1
		public Func<object, T> Getter { get; private set; }

		// Token: 0x0600845B RID: 33883 RVA: 0x0026B5BC File Offset: 0x002697BC
		public FieldAccessInfo(Type targetType, string fieldName)
		{
			GenericFieldAccessOptimiser.FieldAccessInfo<T> <>4__this = this;
			try
			{
				this.fieldInfo = targetType.GetField(fieldName);
				if (!(this.fieldInfo == null))
				{
					if (!(this.fieldInfo.FieldType != typeof(T)))
					{
						Task.Run(delegate()
						{
							<>4__this.CompileAndCacheDelegate(targetType);
						});
					}
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x0600845C RID: 33884 RVA: 0x0026B64C File Offset: 0x0026984C
		public FieldAccessInfo(object target, string fieldName)
		{
			GenericFieldAccessOptimiser.FieldAccessInfo<T> <>4__this = this;
			try
			{
				Type targetType = target.GetType();
				this.fieldInfo = targetType.GetField(fieldName);
				if (!(this.fieldInfo == null))
				{
					if (!(this.fieldInfo.FieldType != typeof(T)))
					{
						Task.Run(delegate()
						{
							<>4__this.CompileAndCacheDelegate(targetType);
						});
					}
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}

		// Token: 0x0600845D RID: 33885 RVA: 0x0026B6E4 File Offset: 0x002698E4
		private void CompileAndCacheDelegate(Type targetType)
		{
			try
			{
				ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "target");
				ParameterExpression parameterExpression2 = Expression.Parameter(typeof(T), "value");
				MemberExpression memberExpression = Expression.Field(Expression.Convert(parameterExpression, targetType), this.fieldInfo);
				this.Setter = Expression.Lambda<Action<object, T>>(Expression.Assign(memberExpression, parameterExpression2), new ParameterExpression[]
				{
					parameterExpression,
					parameterExpression2
				}).Compile();
				this.Getter = Expression.Lambda<Func<object, T>>(Expression.Convert(memberExpression, typeof(T)), new ParameterExpression[]
				{
					parameterExpression
				}).Compile();
				this.IsCompiled = true;
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}

		// Token: 0x0400844B RID: 33867
		public volatile bool IsCompiled;

		// Token: 0x0400844C RID: 33868
		public FieldInfo fieldInfo;
	}
}
