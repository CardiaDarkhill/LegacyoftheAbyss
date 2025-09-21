using System;
using System.Collections.Generic;
using System.Reflection;

namespace GenericVariableExtension
{
	// Token: 0x020008CE RID: 2254
	public static class VariableExtensionsGeneric
	{
		// Token: 0x06004ECA RID: 20170 RVA: 0x0016DE70 File Offset: 0x0016C070
		public static void ClearCache()
		{
			VariableExtensionsGeneric.variableCache.ClearCache();
		}

		// Token: 0x06004ECB RID: 20171 RVA: 0x0016DE7C File Offset: 0x0016C07C
		public static void SetVariable<T>(this object obj, string fieldName, T value)
		{
			obj.SetVariable(fieldName, value, typeof(T));
		}

		// Token: 0x06004ECC RID: 20172 RVA: 0x0016DE98 File Offset: 0x0016C098
		public static T GetVariable<T>(this object obj, string fieldName)
		{
			object variable = obj.GetVariable(fieldName, typeof(T));
			if (variable != null)
			{
				return (T)((object)variable);
			}
			return default(T);
		}

		// Token: 0x06004ECD RID: 20173 RVA: 0x0016DECC File Offset: 0x0016C0CC
		public static List<T> GetVariables<T>(this object obj, string fieldNameContains)
		{
			Func<string, bool> predicate;
			if (string.IsNullOrEmpty(fieldNameContains))
			{
				predicate = null;
			}
			else
			{
				predicate = ((string name) => name.Contains(fieldNameContains));
			}
			return obj.GetVariables(predicate);
		}

		// Token: 0x06004ECE RID: 20174 RVA: 0x0016DF0C File Offset: 0x0016C10C
		public static List<T> GetVariables<T>(this object obj, Func<string, bool> predicate)
		{
			Type type = obj.GetType();
			VariableExtensionsGeneric.FieldCache.TypeCache typeCache = VariableExtensionsGeneric.variableCache.GetTypeCache(type);
			FieldInfo[] fieldInfos = typeCache.GetFieldInfos(type);
			PropertyInfo[] propertyInfos = typeCache.GetPropertyInfos(type);
			List<T> list = new List<T>();
			foreach (FieldInfo fieldInfo in fieldInfos)
			{
				if (fieldInfo.FieldType == typeof(T) && (predicate == null || predicate(fieldInfo.Name)))
				{
					list.Add((T)((object)fieldInfo.GetValue(obj)));
				}
			}
			foreach (PropertyInfo propertyInfo in propertyInfos)
			{
				if (propertyInfo.PropertyType == typeof(T) && (predicate == null || predicate(propertyInfo.Name)))
				{
					list.Add((T)((object)propertyInfo.GetValue(obj, null)));
				}
			}
			return list;
		}

		// Token: 0x06004ECF RID: 20175 RVA: 0x0016DFF6 File Offset: 0x0016C1F6
		public static void SetVariable(this object obj, string fieldName, object value, Type type)
		{
			VariableExtensionsGeneric.variableCache.SetVariable(obj, fieldName, value, type);
		}

		// Token: 0x06004ED0 RID: 20176 RVA: 0x0016E006 File Offset: 0x0016C206
		public static object GetVariable(this object obj, string fieldName, Type type)
		{
			return VariableExtensionsGeneric.variableCache.GetVariable(obj, fieldName, type);
		}

		// Token: 0x06004ED1 RID: 20177 RVA: 0x0016E015 File Offset: 0x0016C215
		public static bool VariableExists<TVar, TContainer>(string fieldName) where TContainer : class
		{
			return VariableExtensionsGeneric.VariableExists<TContainer>(fieldName, typeof(TVar));
		}

		// Token: 0x06004ED2 RID: 20178 RVA: 0x0016E027 File Offset: 0x0016C227
		public static bool VariableExists<TContainer>(string fieldName, Type type) where TContainer : class
		{
			return VariableExtensionsGeneric.variableCache.VariableExists(typeof(TContainer), fieldName, type);
		}

		// Token: 0x04004F73 RID: 20339
		private static readonly VariableExtensionsGeneric.FieldCache variableCache = new VariableExtensionsGeneric.FieldCache();

		// Token: 0x02001B55 RID: 6997
		private sealed class FieldCache
		{
			// Token: 0x060099C4 RID: 39364 RVA: 0x002B253C File Offset: 0x002B073C
			public VariableExtensionsGeneric.FieldCache.TypeCache GetTypeCache(Type type)
			{
				VariableExtensionsGeneric.FieldCache.TypeCache typeCache;
				if (!this.typeCaches.TryGetValue(type, out typeCache))
				{
					typeCache = new VariableExtensionsGeneric.FieldCache.TypeCache();
					this.typeCaches.Add(type, typeCache);
				}
				return typeCache;
			}

			// Token: 0x060099C5 RID: 39365 RVA: 0x002B256D File Offset: 0x002B076D
			public void ClearCache()
			{
				this.typeCaches.Clear();
			}

			// Token: 0x060099C6 RID: 39366 RVA: 0x002B257C File Offset: 0x002B077C
			private VariableExtensionsGeneric.FieldCache.VariableCache GetVariableCache(Type objType, string fieldName)
			{
				VariableExtensionsGeneric.FieldCache.TypeCache typeCache = this.GetTypeCache(objType);
				VariableExtensionsGeneric.FieldCache.VariableCache variableCache;
				if (!typeCache.lookup.TryGetValue(fieldName, out variableCache))
				{
					FieldInfo field = objType.GetField(fieldName);
					if (field != null)
					{
						variableCache = new VariableExtensionsGeneric.FieldCache.FieldInfoCache(field);
					}
					else
					{
						PropertyInfo property = objType.GetProperty(fieldName);
						if (property != null)
						{
							variableCache = new VariableExtensionsGeneric.FieldCache.PropertyInfoCache(property);
						}
						else
						{
							variableCache = new VariableExtensionsGeneric.FieldCache.InvalidInfoCache();
						}
					}
					typeCache.lookup.Add(fieldName, variableCache);
				}
				return variableCache;
			}

			// Token: 0x060099C7 RID: 39367 RVA: 0x002B25EC File Offset: 0x002B07EC
			public object GetVariable(object obj, string fieldName, Type expectedType)
			{
				Type type = obj.GetType();
				VariableExtensionsGeneric.FieldCache.VariableCache variableCache = this.GetVariableCache(type, fieldName);
				if (!variableCache.IsCorrectType(expectedType))
				{
					return null;
				}
				return variableCache.GetVariable(obj);
			}

			// Token: 0x060099C8 RID: 39368 RVA: 0x002B261C File Offset: 0x002B081C
			public void SetVariable(object obj, string fieldName, object value, Type expectedType)
			{
				Type type = obj.GetType();
				VariableExtensionsGeneric.FieldCache.VariableCache variableCache = this.GetVariableCache(type, fieldName);
				if (!variableCache.IsCorrectType(expectedType))
				{
					return;
				}
				variableCache.SetVariable(obj, value);
			}

			// Token: 0x060099C9 RID: 39369 RVA: 0x002B264C File Offset: 0x002B084C
			public bool VariableExists(Type objType, string fieldName, Type expectedType)
			{
				return this.GetVariableCache(objType, fieldName).IsCorrectType(expectedType);
			}

			// Token: 0x04009C3E RID: 39998
			private Dictionary<Type, VariableExtensionsGeneric.FieldCache.TypeCache> typeCaches = new Dictionary<Type, VariableExtensionsGeneric.FieldCache.TypeCache>();

			// Token: 0x02001C34 RID: 7220
			public sealed class TypeCache
			{
				// Token: 0x06009B09 RID: 39689 RVA: 0x002B4EF1 File Offset: 0x002B30F1
				public FieldInfo[] GetFieldInfos(Type objType)
				{
					if (!this.cachedFieldInfos)
					{
						this.cachedFieldInfos = true;
						this.fieldInfos = objType.GetFields();
					}
					return this.fieldInfos;
				}

				// Token: 0x06009B0A RID: 39690 RVA: 0x002B4F14 File Offset: 0x002B3114
				public PropertyInfo[] GetPropertyInfos(Type objType)
				{
					if (!this.cachedPropertyInfos)
					{
						this.cachedPropertyInfos = true;
						this.propertyInfos = objType.GetProperties();
					}
					return this.propertyInfos;
				}

				// Token: 0x0400A048 RID: 41032
				private bool cachedFieldInfos;

				// Token: 0x0400A049 RID: 41033
				private FieldInfo[] fieldInfos;

				// Token: 0x0400A04A RID: 41034
				private bool cachedPropertyInfos;

				// Token: 0x0400A04B RID: 41035
				private PropertyInfo[] propertyInfos;

				// Token: 0x0400A04C RID: 41036
				public Dictionary<string, VariableExtensionsGeneric.FieldCache.VariableCache> lookup = new Dictionary<string, VariableExtensionsGeneric.FieldCache.VariableCache>();
			}

			// Token: 0x02001C35 RID: 7221
			public abstract class VariableCache
			{
				// Token: 0x06009B0C RID: 39692
				public abstract bool IsCorrectType(Type expectedType);

				// Token: 0x06009B0D RID: 39693
				public abstract Type GetVariableType();

				// Token: 0x06009B0E RID: 39694
				public abstract object GetVariable(object obj);

				// Token: 0x06009B0F RID: 39695
				public abstract void SetVariable(object obj, object value);
			}

			// Token: 0x02001C36 RID: 7222
			private sealed class FieldInfoCache : VariableExtensionsGeneric.FieldCache.VariableCache
			{
				// Token: 0x06009B11 RID: 39697 RVA: 0x002B4F52 File Offset: 0x002B3152
				public FieldInfoCache(FieldInfo fieldInfo)
				{
					this.fieldInfo = fieldInfo;
				}

				// Token: 0x06009B12 RID: 39698 RVA: 0x002B4F61 File Offset: 0x002B3161
				public override bool IsCorrectType(Type expectedType)
				{
					return this.fieldInfo.FieldType == expectedType;
				}

				// Token: 0x06009B13 RID: 39699 RVA: 0x002B4F74 File Offset: 0x002B3174
				public override Type GetVariableType()
				{
					return this.fieldInfo.FieldType;
				}

				// Token: 0x06009B14 RID: 39700 RVA: 0x002B4F81 File Offset: 0x002B3181
				public override object GetVariable(object obj)
				{
					return this.fieldInfo.GetValue(obj);
				}

				// Token: 0x06009B15 RID: 39701 RVA: 0x002B4F8F File Offset: 0x002B318F
				public override void SetVariable(object obj, object value)
				{
					this.fieldInfo.SetValue(obj, value);
				}

				// Token: 0x0400A04D RID: 41037
				private FieldInfo fieldInfo;
			}

			// Token: 0x02001C37 RID: 7223
			private sealed class PropertyInfoCache : VariableExtensionsGeneric.FieldCache.VariableCache
			{
				// Token: 0x06009B16 RID: 39702 RVA: 0x002B4F9E File Offset: 0x002B319E
				public PropertyInfoCache(PropertyInfo propertyInfo)
				{
					this.propertyInfo = propertyInfo;
				}

				// Token: 0x06009B17 RID: 39703 RVA: 0x002B4FAD File Offset: 0x002B31AD
				public override bool IsCorrectType(Type expectedType)
				{
					return this.propertyInfo.PropertyType == expectedType;
				}

				// Token: 0x06009B18 RID: 39704 RVA: 0x002B4FC0 File Offset: 0x002B31C0
				public override Type GetVariableType()
				{
					return this.propertyInfo.PropertyType;
				}

				// Token: 0x06009B19 RID: 39705 RVA: 0x002B4FCD File Offset: 0x002B31CD
				public override object GetVariable(object obj)
				{
					return this.propertyInfo.GetValue(obj, null);
				}

				// Token: 0x06009B1A RID: 39706 RVA: 0x002B4FDC File Offset: 0x002B31DC
				public override void SetVariable(object obj, object value)
				{
					this.propertyInfo.SetValue(obj, value, null);
				}

				// Token: 0x0400A04E RID: 41038
				private PropertyInfo propertyInfo;
			}

			// Token: 0x02001C38 RID: 7224
			private sealed class InvalidInfoCache : VariableExtensionsGeneric.FieldCache.VariableCache
			{
				// Token: 0x06009B1B RID: 39707 RVA: 0x002B4FEC File Offset: 0x002B31EC
				public override bool IsCorrectType(Type expectedType)
				{
					return false;
				}

				// Token: 0x06009B1C RID: 39708 RVA: 0x002B4FEF File Offset: 0x002B31EF
				public override Type GetVariableType()
				{
					return null;
				}

				// Token: 0x06009B1D RID: 39709 RVA: 0x002B4FF2 File Offset: 0x002B31F2
				public override object GetVariable(object obj)
				{
					return null;
				}

				// Token: 0x06009B1E RID: 39710 RVA: 0x002B4FF5 File Offset: 0x002B31F5
				public override void SetVariable(object obj, object value)
				{
				}
			}
		}
	}
}
