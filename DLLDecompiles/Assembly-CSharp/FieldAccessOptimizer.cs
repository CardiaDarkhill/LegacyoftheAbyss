using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

// Token: 0x02000186 RID: 390
public class FieldAccessOptimizer<TTarget, T>
{
	// Token: 0x06000CBC RID: 3260 RVA: 0x00038C78 File Offset: 0x00036E78
	public void SetField(object target, string fieldName, T value)
	{
		FieldAccessOptimizer<TTarget, T>.FieldAccessInfo fieldAccessInfo;
		if (!this.EnsureCompiled(target, fieldName, out fieldAccessInfo))
		{
			return;
		}
		try
		{
			fieldAccessInfo.SetValue(target, value);
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x06000CBD RID: 3261 RVA: 0x00038CB0 File Offset: 0x00036EB0
	public T GetField(object target, string fieldName)
	{
		FieldAccessOptimizer<TTarget, T>.FieldAccessInfo fieldAccessInfo;
		T result;
		if (!this.EnsureCompiled(target, fieldName, out fieldAccessInfo))
		{
			result = default(T);
			return result;
		}
		try
		{
			result = fieldAccessInfo.GetValue(target);
		}
		catch (Exception)
		{
			result = default(T);
		}
		return result;
	}

	// Token: 0x06000CBE RID: 3262 RVA: 0x00038CFC File Offset: 0x00036EFC
	private bool EnsureCompiled(Type targetType, string fieldName, out FieldAccessOptimizer<TTarget, T>.FieldAccessInfo info)
	{
		info = null;
		FieldAccessOptimizer<TTarget, T>.FieldAccessInfo fieldAccessInfo;
		if (!this.accessCache.TryGetValue(fieldName, out fieldAccessInfo))
		{
			try
			{
				fieldAccessInfo = new FieldAccessOptimizer<TTarget, T>.FieldAccessInfo(targetType, fieldName);
				if (fieldAccessInfo.targetType == FieldAccessOptimizer<TTarget, T>.FieldAccessInfo.TargetType.None)
				{
					return false;
				}
				this.accessCache[fieldName] = fieldAccessInfo;
			}
			catch (Exception)
			{
				return false;
			}
		}
		info = fieldAccessInfo;
		return info.targetType > FieldAccessOptimizer<TTarget, T>.FieldAccessInfo.TargetType.None;
	}

	// Token: 0x06000CBF RID: 3263 RVA: 0x00038D64 File Offset: 0x00036F64
	private bool EnsureCompiled(object target, string fieldName, out FieldAccessOptimizer<TTarget, T>.FieldAccessInfo info)
	{
		if (target == null)
		{
			info = null;
			return false;
		}
		return this.EnsureCompiled(target.GetType(), fieldName, out info);
	}

	// Token: 0x06000CC0 RID: 3264 RVA: 0x00038D7C File Offset: 0x00036F7C
	private bool SetFieldUsingReflection(object target, FieldInfo fieldInfo, T value)
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

	// Token: 0x06000CC1 RID: 3265 RVA: 0x00038DBC File Offset: 0x00036FBC
	private T GetFieldUsingReflection(object target, FieldInfo fieldInfo)
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

	// Token: 0x06000CC2 RID: 3266 RVA: 0x00038E0C File Offset: 0x0003700C
	public bool FieldExists(Type targetType, string fieldName)
	{
		FieldAccessOptimizer<TTarget, T>.FieldAccessInfo fieldAccessInfo;
		return this.EnsureCompiled(targetType, fieldName, out fieldAccessInfo);
	}

	// Token: 0x06000CC3 RID: 3267 RVA: 0x00038E24 File Offset: 0x00037024
	public bool FieldExists(object target, string fieldName)
	{
		FieldAccessOptimizer<TTarget, T>.FieldAccessInfo fieldAccessInfo;
		return this.EnsureCompiled(target, fieldName, out fieldAccessInfo);
	}

	// Token: 0x04000C36 RID: 3126
	private readonly Dictionary<string, FieldAccessOptimizer<TTarget, T>.FieldAccessInfo> accessCache = new Dictionary<string, FieldAccessOptimizer<TTarget, T>.FieldAccessInfo>();

	// Token: 0x020014B4 RID: 5300
	private sealed class FieldAccessInfo
	{
		// Token: 0x0600845E RID: 33886 RVA: 0x0026B7A0 File Offset: 0x002699A0
		public FieldAccessInfo(Type targetType, string fieldName)
		{
			try
			{
				this.fieldInfo = targetType.GetField(fieldName);
				if (this.fieldInfo != null)
				{
					this.targetType = FieldAccessOptimizer<TTarget, T>.FieldAccessInfo.TargetType.Field;
				}
				else
				{
					this.propertyInfo = targetType.GetProperty(fieldName);
					if (this.propertyInfo != null)
					{
						this.targetType = FieldAccessOptimizer<TTarget, T>.FieldAccessInfo.TargetType.Property;
					}
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x0600845F RID: 33887 RVA: 0x0026B810 File Offset: 0x00269A10
		public void SetValue(object target, T value)
		{
			if (this.createdSetValueDelegate)
			{
				this._setValueDelegate(target, value);
				return;
			}
			FieldAccessOptimizer<TTarget, T>.FieldAccessInfo.TargetType targetType = this.targetType;
			if (targetType == FieldAccessOptimizer<TTarget, T>.FieldAccessInfo.TargetType.Field)
			{
				this.fieldInfo.SetValue(target, value);
				return;
			}
			if (targetType != FieldAccessOptimizer<TTarget, T>.FieldAccessInfo.TargetType.Property)
			{
				return;
			}
			this.propertyInfo.SetValue(target, value);
		}

		// Token: 0x06008460 RID: 33888 RVA: 0x0026B868 File Offset: 0x00269A68
		public T GetValue(object target)
		{
			if (!this.createdGetValueDelegate)
			{
				FieldAccessOptimizer<TTarget, T>.FieldAccessInfo.TargetType targetType = this.targetType;
				if (targetType == FieldAccessOptimizer<TTarget, T>.FieldAccessInfo.TargetType.Field)
				{
					return (T)((object)this.fieldInfo.GetValue(target));
				}
				if (targetType == FieldAccessOptimizer<TTarget, T>.FieldAccessInfo.TargetType.Property)
				{
					return (T)((object)this.propertyInfo.GetValue(target));
				}
			}
			return this._getValueDelegate(target);
		}

		// Token: 0x06008461 RID: 33889 RVA: 0x0026B8C0 File Offset: 0x00269AC0
		private static Action<object, T> CreateFieldSetter(FieldInfo fieldInfo)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(object));
			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(T));
			return Expression.Lambda<Action<object, T>>(Expression.Assign(Expression.Field(Expression.Convert(parameterExpression, fieldInfo.DeclaringType), fieldInfo), Expression.Convert(parameterExpression2, fieldInfo.FieldType)), new ParameterExpression[]
			{
				parameterExpression,
				parameterExpression2
			}).Compile();
		}

		// Token: 0x06008462 RID: 33890 RVA: 0x0026B928 File Offset: 0x00269B28
		private static Func<object, T> CreateFieldGetter(FieldInfo fieldInfo)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(object));
			return Expression.Lambda<Func<object, T>>(Expression.Convert(Expression.Field(Expression.Convert(parameterExpression, fieldInfo.DeclaringType), fieldInfo), typeof(T)), new ParameterExpression[]
			{
				parameterExpression
			}).Compile();
		}

		// Token: 0x06008463 RID: 33891 RVA: 0x0026B97C File Offset: 0x00269B7C
		private static Action<object, T> CreatePropertySetter(PropertyInfo propertyInfo)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(object));
			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(T));
			MethodInfo setMethod = propertyInfo.GetSetMethod();
			return Expression.Lambda<Action<object, T>>(Expression.Call(Expression.Convert(parameterExpression, propertyInfo.DeclaringType), setMethod, new Expression[]
			{
				Expression.Convert(parameterExpression2, propertyInfo.PropertyType)
			}), new ParameterExpression[]
			{
				parameterExpression,
				parameterExpression2
			}).Compile();
		}

		// Token: 0x06008464 RID: 33892 RVA: 0x0026B9F0 File Offset: 0x00269BF0
		private static Func<object, T> CreatePropertyGetter(PropertyInfo propertyInfo)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(object));
			MethodInfo getMethod = propertyInfo.GetGetMethod();
			return Expression.Lambda<Func<object, T>>(Expression.Convert(Expression.Call(Expression.Convert(parameterExpression, propertyInfo.DeclaringType), getMethod), typeof(T)), new ParameterExpression[]
			{
				parameterExpression
			}).Compile();
		}

		// Token: 0x0400844D RID: 33869
		private readonly Action<object, T> _setValueDelegate;

		// Token: 0x0400844E RID: 33870
		private readonly Func<object, T> _getValueDelegate;

		// Token: 0x0400844F RID: 33871
		public readonly FieldAccessOptimizer<TTarget, T>.FieldAccessInfo.TargetType targetType;

		// Token: 0x04008450 RID: 33872
		private FieldInfo fieldInfo;

		// Token: 0x04008451 RID: 33873
		private PropertyInfo propertyInfo;

		// Token: 0x04008452 RID: 33874
		private bool createdSetValueDelegate;

		// Token: 0x04008453 RID: 33875
		private bool createdGetValueDelegate;

		// Token: 0x02001C0E RID: 7182
		public enum TargetType
		{
			// Token: 0x04009FE1 RID: 40929
			None,
			// Token: 0x04009FE2 RID: 40930
			Field,
			// Token: 0x04009FE3 RID: 40931
			Property
		}
	}
}
