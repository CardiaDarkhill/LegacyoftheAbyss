using System;
using UnityEngine;

// Token: 0x02000783 RID: 1923
public static class ScenePrefabInstanceFix
{
	// Token: 0x0600445A RID: 17498 RVA: 0x0012B919 File Offset: 0x00129B19
	public static void CheckField<T>(ref T obj) where T : Object
	{
	}

	// Token: 0x02001A67 RID: 6759
	public interface ICheckFields
	{
		// Token: 0x060096D2 RID: 38610
		void OnPrefabInstanceFix();
	}
}
