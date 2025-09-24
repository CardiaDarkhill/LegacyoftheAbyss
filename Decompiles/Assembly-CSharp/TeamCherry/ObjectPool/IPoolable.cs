using System;
using UnityEngine;

namespace TeamCherry.ObjectPool
{
	// Token: 0x020008C2 RID: 2242
	public interface IPoolable<T> where T : Object
	{
		// Token: 0x06004D96 RID: 19862
		void SetReleaser(IPoolReleaser<T> releaser);

		// Token: 0x06004D97 RID: 19863
		void Release();

		// Token: 0x06004D98 RID: 19864
		void OnSpawn();

		// Token: 0x06004D99 RID: 19865
		void OnRelease();
	}
}
