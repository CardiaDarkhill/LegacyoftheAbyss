using System;

// Token: 0x020007C9 RID: 1993
public class PersistentItemData<T>
{
	// Token: 0x040046AB RID: 18091
	public string ID;

	// Token: 0x040046AC RID: 18092
	public string SceneName;

	// Token: 0x040046AD RID: 18093
	public bool IsSemiPersistent;

	// Token: 0x040046AE RID: 18094
	public T Value;

	// Token: 0x040046AF RID: 18095
	public SceneData.PersistentMutatorTypes Mutator;
}
