using System;
using System.Collections.Generic;
using System.Reflection;

namespace InControl
{
	// Token: 0x02000941 RID: 2369
	public static class Reflector
	{
		// Token: 0x17000B99 RID: 2969
		// (get) Token: 0x06005440 RID: 21568 RVA: 0x0017FCEB File Offset: 0x0017DEEB
		public static IEnumerable<Type> AllAssemblyTypes
		{
			get
			{
				IEnumerable<Type> result;
				if ((result = Reflector.assemblyTypes) == null)
				{
					result = (Reflector.assemblyTypes = Reflector.GetAllAssemblyTypes());
				}
				return result;
			}
		}

		// Token: 0x06005441 RID: 21569 RVA: 0x0017FD04 File Offset: 0x0017DF04
		private static bool IgnoreAssemblyWithName(string assemblyName)
		{
			foreach (string value in Reflector.ignoreAssemblies)
			{
				if (assemblyName.StartsWith(value))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005442 RID: 21570 RVA: 0x0017FD38 File Offset: 0x0017DF38
		private static IEnumerable<Type> GetAllAssemblyTypes()
		{
			List<Type> list = new List<Type>();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (!Reflector.IgnoreAssemblyWithName(assembly.GetName().Name))
				{
					Type[] array = null;
					try
					{
						array = assembly.GetTypes();
					}
					catch
					{
					}
					if (array != null)
					{
						list.AddRange(array);
					}
				}
			}
			return list;
		}

		// Token: 0x0400539D RID: 21405
		private static readonly string[] ignoreAssemblies = new string[]
		{
			"Unity",
			"UnityEngine",
			"UnityEditor",
			"mscorlib",
			"Microsoft",
			"System",
			"Mono",
			"JetBrains",
			"nunit",
			"ExCSS",
			"ICSharpCode",
			"AssetStoreTools"
		};

		// Token: 0x0400539E RID: 21406
		private static IEnumerable<Type> assemblyTypes;
	}
}
